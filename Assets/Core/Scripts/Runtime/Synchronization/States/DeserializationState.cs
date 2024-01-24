#region

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Runtime.Synchronization.Nodes;

#endregion

namespace Core.Runtime.Synchronization.States
{
    public class DeserializationState : State
    {
        private readonly LinkedList<int> _listIndexStack = new LinkedList<int>();
        private List<Type> _imaginaryHierarchyStack = new List<Type>();


        public DeserializationState(DictionaryNode root) :
            base(root)
        {
        }


        public override void BeginDictionary(string id)
        {
            base.BeginDictionary(id);

            if (IsChildImaginary(id))
            {
                _imaginaryHierarchyStack.Add(typeof(DictionaryNode));
            }
            else
            {
                CurrentNode = CurrentDictionary.GetDictionary(id);
            }
        }


        public override void EndDictionary()
        {
            base.EndDictionary();

            if (IsImaginary())
            {
                _imaginaryHierarchyStack.RemoveAt(
                    _imaginaryHierarchyStack.Count - 1
                );
            }
            else
            {
                EndContainer();
            }
        }


        public override void BeginList(string id)
        {
            base.BeginList(id);

            if (IsChildImaginary(id))
            {
                _imaginaryHierarchyStack.Add(typeof(ListNode));
            }
            else
            {
                CurrentNode = CurrentDictionary.GetList(id);
            }

            _listIndexStack.AddFirst(0);
        }


        public override void EndList()
        {
            base.EndList();

            if (IsImaginary())
            {
                _imaginaryHierarchyStack.RemoveAt(
                    _imaginaryHierarchyStack.Count - 1
                );
            }
            else
            {
                EndContainer();
            }

            _listIndexStack.RemoveFirst();
        }


        public override void BeginDictionaryElement()
        {
            base.BeginDictionaryElement();

            int index = _listIndexStack.First.Value;

            if (IsChildElementImaginary(index))
            {
                _imaginaryHierarchyStack.Add(typeof(DictionaryNode));
            }
            else
            {
                CurrentNode = CurrentList.GetDictionary(index);
            }
        }


        public override void EndDictionaryElement()
        {
            base.EndDictionaryElement();

            if (IsImaginary())
            {
                _imaginaryHierarchyStack.RemoveAt(
                    _imaginaryHierarchyStack.Count - 1
                );
            }
            else
            {
                EndContainer();
            }

            AdvanceListElementIndex();
        }


        public override void BeginListElement()
        {
            base.BeginListElement();

            int index = _listIndexStack.First.Value;

            if (IsChildElementImaginary(index))
            {
                _imaginaryHierarchyStack.Add(typeof(ListNode));
            }
            else
            {
                CurrentNode = CurrentList.GetList(_listIndexStack.First.Value);
            }

            _listIndexStack.AddFirst(0);
        }


        public override void EndListElement()
        {
            base.EndListElement();

            if (IsImaginary())
            {
                _imaginaryHierarchyStack.RemoveAt(
                    _imaginaryHierarchyStack.Count - 1
                );
            }
            else
            {
                EndContainer();
            }

            _listIndexStack.RemoveFirst();
            AdvanceListElementIndex();
        }


        public override void SyncObject<T>(string id, ref T obj, Func<T> creator)
        {
            if (IsChildImaginary(id))
            {
                return;
            }

            if (CurrentDictionary.IsNull(id))
            {
                obj = null;
            }
            else
            {
                obj = creator();
                SyncObject(id, obj);
            }
        }


        public override void SyncValueElement<T>(ref T value, ISynchronizer<T> synchronizer = null)
        {
            base.SyncValueElement(ref value, synchronizer);

            int index = _listIndexStack.First.Value++;
            if (!IsChildElementImaginary(index))
            {
                value = synchronizer != null ? synchronizer.Deserialize(CurrentList.GetNode(index)) : CurrentList.GetValue<T>(index);
            }
        }


        protected override void SyncValue<T>(
            string id,
            ref T value,
            T defaultValue,
            ISynchronizer<T> synchronizer = null)
        {
            base.SyncValue(id, ref value, defaultValue, synchronizer);

            if (!IsChildImaginary(id))
            {
                if (synchronizer != null)
                {
                    Node node = CurrentDictionary.FindNode<Node>(id);
                    value = node != null ? synchronizer.Deserialize(node) : defaultValue;
                }
                else
                {
                    value = CurrentDictionary.GetValue(id, defaultValue);
                }
            }
            else
            {
                value = defaultValue;
            }
        }


        protected override void SyncValue<T>(string id, ref T value, ISynchronizer<T> synchronizer = null)
        {
            base.SyncValue(id, ref value, synchronizer);

            if (!IsChildImaginary(id))
            {
                value = synchronizer != null ? synchronizer.Deserialize(CurrentDictionary.GetNode(id)) : CurrentDictionary.GetValue<T>(id);
            }
        }


        protected override void SyncValueArray<T>(string id, ref T[] array, bool isResizeAllowed)
        {
            base.SyncValueArray(id, ref array, isResizeAllowed);

            if (!IsChildImaginary(id) &&
                CurrentDictionary.IsNull(id))
            {
                if (isResizeAllowed)
                {
                    array = null;
                }
                else
                {
                    ThrowNullDataArrayWithoutResize();
                }

                return;
            }

            BeginList(id);
            try
            {
                SyncArray(ref array, isResizeAllowed);
            }
            finally
            {
                EndList();
            }
        }


        protected override void SyncValueList<T>(string id, ref List<T> list, bool isResizeAllowed)
        {
            base.SyncValueList(id, ref list, isResizeAllowed);

            if (!IsChildImaginary(id) &&
                CurrentDictionary.IsNull(id))
            {
                if (isResizeAllowed)
                {
                    list = null;
                }
                else
                {
                    ThrowNullDataArrayWithoutResize();
                }

                return;
            }

            BeginList(id);
            try
            {
                SyncList(ref list, isResizeAllowed);
            }
            finally
            {
                EndList();
            }
        }


        protected override void SyncValueDictionary<T>(
            string id,
            ref Dictionary<string, T> dictionary,
            bool isResizeAllowed)
        {
            base.SyncValueDictionary(id, ref dictionary, isResizeAllowed);

            if (!IsChildImaginary(id) &&
                CurrentDictionary.IsNull(id))
            {
                if (isResizeAllowed)
                {
                    dictionary = null;
                }
                else
                {
                    ThrowNullDataArrayWithoutResize();
                }

                return;
            }

            BeginDictionary(id);
            try
            {
                SyncDictionary(ref dictionary, isResizeAllowed);
            }
            finally
            {
                EndDictionary();
            }
        }


        protected override void SyncValueArrayElement<T>(ref T[] array, bool isResizeAllowed)
        {
            base.SyncValueArrayElement(ref array, isResizeAllowed);

            int index = _listIndexStack.First.Value;
            if (!IsChildElementImaginary(index) &&
                CurrentList.IsNull(index))
            {
                ++_listIndexStack.First.Value;
                if (isResizeAllowed)
                {
                    array = null;
                }
                else
                {
                    ThrowNullDataArrayWithoutResize();
                }

                return;
            }

            BeginListElement();
            try
            {
                SyncArray(ref array, isResizeAllowed);
            }
            finally
            {
                EndListElement();
            }
        }


        protected override void SyncValueListElement<T>(ref List<T> list, bool isResizeAllowed)
        {
            base.SyncValueListElement(ref list, isResizeAllowed);

            int index = _listIndexStack.First.Value;
            if (!IsChildElementImaginary(index) &&
                CurrentList.IsNull(index))
            {
                ++_listIndexStack.First.Value;
                if (isResizeAllowed)
                {
                    list = null;
                }
                else
                {
                    ThrowNullDataArrayWithoutResize();
                }

                return;
            }

            BeginListElement();
            try
            {
                SyncList(ref list, isResizeAllowed);
            }
            finally
            {
                EndListElement();
            }
        }


        protected override void SyncValueDictionaryElement<T>(
            ref Dictionary<string, T> dictionary,
            bool isResizeAllowed)
        {
            base.SyncValueDictionaryElement(ref dictionary, isResizeAllowed);

            int index = _listIndexStack.First.Value;
            if (!IsChildElementImaginary(index) &&
                CurrentList.IsNull(index))
            {
                ++_listIndexStack.First.Value;
                if (isResizeAllowed)
                {
                    dictionary = null;
                }
                else
                {
                    ThrowNullDataArrayWithoutResize();
                    return;
                }

                return;
            }

            BeginDictionaryElement();
            try
            {
                SyncDictionary(ref dictionary, isResizeAllowed);
            }
            finally
            {
                EndDictionaryElement();
            }
        }


        protected override void SyncObjectArrayElements<T>(
            ref T[] array,
            ISynchronizationFactory<T> factory)
        {
            InitArray(ref array);

            for (int i = 0; i < CurrentList.Count; ++i)
            {
                T obj = null;
                SyncObjectElement(ref obj, factory);
                array[i] = obj;
            }
        }


        protected override void SyncObjectListElements<T>(
            ref List<T> list,
            ISynchronizationFactory<T> factory)
        {
            InitList(ref list);

            for (int i = 0; i < CurrentList.Count; ++i)
            {
                T obj = null;
                SyncObjectElement(ref obj, factory);
                list[i] = obj;
            }
        }


        protected override void SyncObjectCollectionElements<T>(
            ICollection<T> collection,
            ISynchronizationFactory<T> factory)
        {
            collection.Clear();

            foreach (Node node in CurrentList)
            {
                T val = null;
                SyncObjectElement(ref val, factory);
                collection.Add(val);
            }
        }


        protected override void SyncDictionaryObjects<T>(
            ref Dictionary<string, T> dictionary,
            ISynchronizationFactory<T> factory)
        {
            InitDictionary(ref dictionary);

            foreach (Node node in CurrentDictionary)
            {
                T val = null;
                SyncObject(node.Id, ref val, factory);
                dictionary.Add(node.Id, val);
            }
        }


        protected override void SyncObjectArrayElements<T>(
            ref T[] array,
            ISynchronizer<T> synchronizer)
        {
            InitArray(ref array);

            for (int i = 0; i < CurrentList.Count; ++i)
            {
                T obj = default;
                SyncObjectElement(ref obj, synchronizer);
                array[i] = obj;
            }
        }


        protected override void SyncObjectListElements<T>(
            ref List<T> list,
            ISynchronizer<T> synchronizer)
        {
            InitList(ref list);

            for (int i = 0; i < CurrentList.Count; ++i)
            {
                T obj = default;
                SyncObjectElement(ref obj, synchronizer);
                list[i] = obj;
            }
        }


        protected override void SyncObjectCollectionElements<T>(
            ICollection<T> collection,
            ISynchronizer<T> synchronizer)
        {
            collection.Clear();

            foreach (Node node in CurrentList)
            {
                T val = default;
                SyncObjectElement(ref val, synchronizer);
                collection.Add(val);
            }
        }


        protected override void SyncDictionaryObjects<T>(
            ref Dictionary<string, T> dictionary,
            ISynchronizer<T> synchronizer)
        {
            InitDictionary(ref dictionary);

            foreach (Node node in CurrentDictionary)
            {
                T val = default;
                SyncObject(node.Id, ref val, synchronizer);
                dictionary.Add(node.Id, val);
            }
        }


        protected override void SyncObjectArrayElements<T>(
            ref T[] array,
            Func<T> creator)
        {
            InitArray(ref array);

            for (int i = 0; i < CurrentList.Count; ++i)
            {
                T obj = null;

                if (!CurrentList.IsNull(i))
                {
                    obj = creator();
                    SyncObjectElement(ref obj);
                }
                else
                {
                    AdvanceListElementIndex();
                }

                array[i] = obj;
            }
        }


        protected override void SyncObjectListElements<T>(
            ref List<T> list,
            Func<T> creator)
        {
            InitList(ref list);

            for (int i = 0; i < CurrentList.Count; ++i)
            {
                T obj = default;

                if (!CurrentList.IsNull(i))
                {
                    obj = creator();
                    SyncObjectElement(ref obj);
                }

                list[i] = obj;
            }
        }


        protected override void SyncObjectCollectionElements<T>(
            ICollection<T> collection,
            Func<T> creator)
        {
            collection.Clear();

            foreach (Node node in CurrentList)
            {
                T obj = null;

                if (!(node is NullNode))
                {
                    obj = creator();
                    SyncObjectElement(ref obj);
                }

                collection.Add(obj);
            }
        }


        protected override void SyncDictionaryObjects<T>(
            ref Dictionary<string, T> dictionary,
            Func<T> creator)
        {
            InitDictionary(ref dictionary);

            foreach (Node node in CurrentDictionary)
            {
                T obj = null;

                if (!(node is NullNode))
                {
                    obj = creator();
                    SyncObject(node.Id, obj);
                }

                dictionary.Add(node.Id, obj);
            }
        }


        protected override void SyncDictionaryObjects<T, TU>(
            ref Dictionary<T, TU> dictionary,
            IKeySynchronizer<T> keySynchronizer,
            Func<TU> creator)
        {
            InitDictionary(ref dictionary);

            foreach (Node node in CurrentDictionary)
            {
                T key = null;
                TU obj = null;

                if (!(node is NullNode))
                {
                    obj = creator();
                    key = keySynchronizer.Deserialize(node.Id);
                    SyncObject(node.Id, obj);
                }

                dictionary.Add(key, obj);
            }
        }


        protected override void SyncDictionaryObjects<T, TU>(
            ref Dictionary<T, TU> dictionary,
            IKeySynchronizer<T> keySynchronizer,
            ISynchronizer<TU> synchronizer)
        {
            InitDictionary(ref dictionary);

            foreach (Node node in CurrentDictionary)
            {
                T key = default;
                TU value = default;

                if (!(node is NullNode))
                {
                    key = keySynchronizer.Deserialize(node.Id);
                    SyncObject(node.Id, ref value, synchronizer);
                }

                dictionary.Add(key, value);
            }
        }


        protected override void SyncDictionaryObjects<T, TU>(
            ref Dictionary<T, TU> dictionary,
            IKeySynchronizer<T> keySynchronizer,
            ISynchronizationFactory<TU> synchronizer)
        {
            InitDictionary(ref dictionary);

            foreach (Node node in CurrentDictionary)
            {
                T key = default;
                TU value = default;

                if (!(node is NullNode))
                {
                    key = keySynchronizer.Deserialize(node.Id);
                    if (key != null)
                    {
                        SyncObject(node.Id, ref value, synchronizer);
                        dictionary.Add(key, value);
                    }
                }
            }
        }


        protected override bool CheckObjectNode<TNode>(string id)
        {
            if (IsChildImaginary(id))
            {
                return false;
            }

            if (!(CurrentDictionary.GetNode(id) is TNode))
            {
                throw new InvalidOperationException($"{ErrorHeader} Invalid object node with id {id}.");
            }

            return true;
        }


        protected override bool CheckObjectElementNode<TNode>()
        {
            int index = _listIndexStack.First.Value;
            if (IsChildElementImaginary(index))
            {
                return false;
            }

            if (!(CurrentList.GetNode(index) is TNode))
            {
                throw new InvalidOperationException($"{ErrorHeader} Invalid object node at index: {index}.");
            }

            return true;
        }


        protected override void SyncFactory<T>(string id, ISynchronizationFactory<T> factory, ref T obj)
        {
            if (IsImaginary())
            {
                throw new InvalidOperationException(
                    $"{ErrorHeader} Can't deserialize object with factory on imaginary node.");
            }

            obj = factory.Deserialize(CurrentDictionary.GetNode(id));
        }


        protected override bool CanSyncObjectInDictionary(string id)
        {
            return CurrentDictionary.Contains(id);
        }


        protected override Type GetCurrentNodeType()
        {
            if (IsImaginary())
            {
                return _imaginaryHierarchyStack.Last();
            }

            return CurrentNode.GetType();
        }


        protected override Type GetCurrentNodeParentType()
        {
            if (IsImaginary())
            {
                return _imaginaryHierarchyStack.Count > 1 ? _imaginaryHierarchyStack[_imaginaryHierarchyStack.Count - 2] : CurrentNode.GetType();
            }

            return CurrentNode.Parent?.GetType();
        }


        private void SyncArray<T>(ref T[] array, bool isResizeAllowed)
        {
            if (!isResizeAllowed && array == null)
            {
                ThrowCantDeserializeNulledContainer();
                return;
            }

            if (IsImaginary())
            {
                return;
            }

            if (isResizeAllowed)
            {
                InitArray(ref array);
            }

            FillList(array);
        }


        private void SyncList<T>(ref List<T> list, bool isResizeAllowed)
        {
            if (!isResizeAllowed && list == null)
            {
                ThrowCantDeserializeNulledContainer();
                return;
            }

            if (IsImaginary())
            {
                return;
            }

            if (isResizeAllowed)
            {
                InitList(ref list);
            }

            FillList(list);
        }


        private void SyncDictionary<T>(ref Dictionary<string, T> dictionary, bool isResizeAllowed)
        {
            if (!isResizeAllowed && dictionary == null)
            {
                ThrowCantDeserializeNulledContainer();
                return;
            }

            if (IsImaginary())
            {
                return;
            }

            if (isResizeAllowed)
            {
                InitDictionary(ref dictionary);

                foreach (Node node in CurrentDictionary)
                {
                    T val = default;
                    SyncValue(node.Id, ref val);
                    dictionary.Add(node.Id, val);
                }
            }
            else
            {
                string[] keys = dictionary.Keys.ToArray();
                foreach (string key in keys)
                {
                    T val = default;
                    SyncValue(key, ref val);
                    dictionary[key] = val;
                }
            }
        }


        private void InitArray<T>(ref T[] array)
        {
            if (array == null || array.Length != CurrentList.Count)
            {
                array = new T[CurrentList.Count];
            }
        }


        private void InitList<T>(ref List<T> list)
        {
            if (list == null || list.Count != CurrentList.Count)
            {
                list = new List<T>(Enumerable.Repeat(default(T), CurrentList.Count));
            }
        }


        private void InitDictionary<T, TU>(ref Dictionary<T, TU> dictionary)
        {
            if (dictionary == null)
            {
                dictionary = new Dictionary<T, TU>();
            }
            else
            {
                dictionary.Clear();
            }
        }


        private void FillList<T>(IList<T> list)
        {
            int minSize = Math.Min(list.Count, CurrentList.Count);
            for (int i = 0; i < minSize; ++i)
            {
                T val = default;
                SyncValueElement(ref val);
                list[i] = val;
            }
        }


        private void AdvanceListElementIndex()
        {
            if (_listIndexStack.Count > 0)
            {
                _listIndexStack.First.Value++;
            }
            else
            {
                ThrowContainerMethodsPairingException();
            }
        }


        private bool IsImaginary()
        {
            return _imaginaryHierarchyStack.Count > 0;
        }


        private bool IsChildImaginary(string id)
        {
            return IsImaginary() ||
                   !CurrentDictionary.Contains(id);
        }


        private bool IsChildElementImaginary(int index)
        {
            return IsImaginary() ||
                   !CurrentList.Contains(index);
        }


        private void ThrowNullDataArrayWithoutResize()
        {
            throw new InvalidOperationException(
                $"{ErrorHeader} Can't deserialize container with null data  while isResizeAllowed is false.");
        }


        private void ThrowCantDeserializeNulledContainer()
        {
            throw new InvalidOperationException(
                $"{ErrorHeader} Can't deserialize nulled container while isResizeAllowed is false.");
        }
    }
}
