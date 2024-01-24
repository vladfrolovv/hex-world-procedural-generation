#region

using System;
using System.Collections.Generic;
using Core.Runtime.Synchronization.Nodes;
using JetBrains.Annotations;

#endregion

namespace Core.Runtime.Synchronization.States
{
    public abstract class State
    {


        protected State(DictionaryNode root)
        {
            Root = root;
            CurrentNode = Root;
        }


        protected DictionaryNode Root { get; }

        protected Node CurrentNode { get; set; }

        protected DictionaryNode CurrentDictionary => CurrentNode as DictionaryNode;

        protected ListNode CurrentList => CurrentNode as ListNode;

        protected string ErrorHeader => $"[{GetStateTypeName(CurrentNode.GetType())}: '{CurrentNode.Id}'] ";
        public virtual void SyncValueElement<T>(ref T value, ISynchronizer<T> synchronizer = null)
        {
            CheckCurrentNodeType<ListNode>();
        }


        protected virtual void SyncValue<T>(
            string id,
            ref T value,
            T defaultValue,
            ISynchronizer<T> synchronizer = null)
        {
            CheckCurrentNodeType<DictionaryNode>();
        }


        protected virtual void SyncValue<T>(string id, ref T value, ISynchronizer<T> synchronizer = null)
        {
            CheckCurrentNodeType<DictionaryNode>();
        }


        protected virtual void SyncValueArray<T>(string id, ref T[] array, bool isResizeAllowed)
        {
            CheckCurrentNodeType<DictionaryNode>();
        }


        protected virtual void SyncValueList<T>(string id, ref List<T> list, bool isResizeAllowed)
        {
            CheckCurrentNodeType<DictionaryNode>();
        }


        protected virtual void SyncValueCollection<T>(string id, [NotNull] ICollection<T> collection)
        {
            CheckCurrentNodeType<DictionaryNode>();
        }


        protected virtual void SyncValueDictionary<T>(
            string id,
            ref Dictionary<string, T> dictionary,
            bool isResizeAllowed)
        {
            CheckCurrentNodeType<DictionaryNode>();
        }


        protected virtual void SyncValueArrayElement<T>(ref T[] array, bool isResizeAllowed)
        {
            CheckCurrentNodeType<ListNode>();
        }


        protected virtual void SyncValueListElement<T>(ref List<T> list, bool isResizeAllowed)
        {
            CheckCurrentNodeType<ListNode>();
        }


        protected virtual void SyncValueDictionaryElement<T>(ref Dictionary<string, T> dictionary, bool isResizeAllowed)
        {
            CheckCurrentNodeType<ListNode>();
        }


        protected abstract void SyncObjectArrayElements<T>(
            ref T[] array,
            [NotNull] ISynchronizationFactory<T> factory) where T : class, ISynchronizable;


        protected abstract void SyncObjectListElements<T>(
            ref List<T> list,
            [NotNull] ISynchronizationFactory<T> factory) where T : class, ISynchronizable;


        protected abstract void SyncObjectCollectionElements<T>(
            [NotNull] ICollection<T> collection,
            [NotNull] ISynchronizationFactory<T> factory) where T : class, ISynchronizable;


        protected abstract void SyncDictionaryObjects<T>(
            ref Dictionary<string, T> dictionary,
            [NotNull] ISynchronizationFactory<T> factory) where T : class, ISynchronizable;


        protected abstract void SyncObjectArrayElements<T>(
            ref T[] array,
            [NotNull] ISynchronizer<T> synchronizer);


        protected abstract void SyncObjectListElements<T>(
            ref List<T> list,
            [NotNull] ISynchronizer<T> synchronizer);


        protected abstract void SyncObjectCollectionElements<T>(
            [NotNull] ICollection<T> collection,
            [NotNull] ISynchronizer<T> synchronizer);


        protected abstract void SyncDictionaryObjects<T>(
            ref Dictionary<string, T> dictionary,
            [NotNull] ISynchronizer<T> synchronizer);


        protected abstract void SyncObjectArrayElements<T>(
            ref T[] array,
            [NotNull] Func<T> creator) where T : class, ISynchronizable;


        protected abstract void SyncObjectListElements<T>(
            ref List<T> list,
            [NotNull] Func<T> creator) where T : ISynchronizable;


        protected abstract void SyncObjectCollectionElements<T>(
            [NotNull] ICollection<T> collection,
            [NotNull] Func<T> creator) where T : class, ISynchronizable;


        protected abstract void SyncDictionaryObjects<T>(
            ref Dictionary<string, T> dictionary,
            [NotNull] Func<T> creator) where T : class, ISynchronizable;


        protected abstract void SyncDictionaryObjects<T, TU>(
            ref Dictionary<T, TU> dictionary,
            [NotNull] IKeySynchronizer<T> keySynchronizer,
            [NotNull] Func<TU> creator)
            where T : class
            where TU : class, ISynchronizable;


        protected abstract void SyncDictionaryObjects<T, TU>(
            ref Dictionary<T, TU> dictionary,
            [NotNull] IKeySynchronizer<T> keySynchronizer,
            [NotNull] ISynchronizer<TU> synchronizer);


        protected abstract void SyncDictionaryObjects<T, TU>(
            ref Dictionary<T, TU> dictionary,
            IKeySynchronizer<T> keySynchronizer,
            ISynchronizationFactory<TU> synchronizer)
            where T : class
            where TU : class, ISynchronizable;


        protected abstract bool CheckObjectNode<TNode>(string id);

        protected abstract bool CheckObjectElementNode<TNode>();


        protected abstract void SyncFactory<T>(string id, [NotNull] ISynchronizationFactory<T> factory, ref T obj)
            where T : class, ISynchronizable;


        protected abstract bool CanSyncObjectInDictionary(string id);


        protected void ThrowContainerMethodsPairingException()
        {
            throw new InvalidOperationException(
                $"Invalid call of {nameof(EndList)}/{nameof(EndListElement)}/" +
                $"{nameof(EndDictionary)}/{nameof(EndDictionaryElement)}. " +
                $"Method calls of {nameof(BeginList)}/{nameof(BeginListElement)}/" +
                $"{nameof(BeginDictionary)}/{nameof(BeginDictionaryElement)} " +
                $"are not properly paired with method calls of {nameof(EndList)}/" +
                $"{nameof(EndListElement)}/{nameof(EndDictionary)}/{nameof(EndDictionaryElement)}."
            );
        }


        protected void CheckCurrentNodeType<T>()
        {
            if (!GetCurrentNodeType().IsAssignableFrom(typeof(T)))
            {
                throw new InvalidOperationException(
                    ErrorHeader +
                    "Invalid method type. " +
                    $"Method is not supported by the {GetStateTypeName(CurrentNode.GetType())} state."
                );
            }
        }


        protected abstract Type GetCurrentNodeType();

        protected abstract Type GetCurrentNodeParentType();


        protected void EndContainer()
        {
            if (CurrentNode == Root)
            {
                ThrowContainerMethodsPairingException();
                return;
            }

            CurrentNode = CurrentNode.Parent;
        }


        private void SyncObjectData<T>(ref T obj, [NotNull] ISynchronizationFactory<T> factory)
            where T : class, ISynchronizable
        {
            SyncFactory("Factory", factory, ref obj);

            if (obj != null)
            {
                BeginDictionary("Object");
                try
                {
                    obj.Sync(this);
                }
                finally
                {
                    EndDictionary();
                }
            }
        }


        private void CheckCurrentNodeParentType<T>()
        {
            Type parentNodeType = GetCurrentNodeParentType();
            if (parentNodeType == null || !parentNodeType.IsAssignableFrom(typeof(T)))
            {
                ThrowContainerMethodsPairingException();
            }
        }


        private void CheckObjectNotNull<T>(T obj) where T : class
        {
            if (obj == null)
            {
                throw new InvalidOperationException(
                    ErrorHeader + "Synchronized argument can't be null."
                );
            }
        }


        private void CheckObjectElementsNotNull<T>(IEnumerable<T> enumerable)
            where T : class
        {
            foreach (T obj in enumerable)
            {
                CheckObjectNotNull(obj);
            }
        }


        private void CheckDictionaryElementsNotNull<T>(Dictionary<string, T> dictionary)
            where T : class
        {
            foreach (KeyValuePair<string, T> pair in dictionary)
            {
                CheckObjectNotNull(pair.Value);
            }
        }


        private string GetStateTypeName(Type type)
        {
            if (type == typeof(DictionaryNode))
            {
                return "Dictionary";
            }

            if (type == typeof(ListNode))
            {
                return "List";
            }

            return "";
        }


        #region Dictionary operations

        #region Control operations

        public virtual void BeginDictionary(string id)
        {
            CheckCurrentNodeType<DictionaryNode>();
        }


        public virtual void EndDictionary()
        {
            CheckCurrentNodeType<DictionaryNode>();
            CheckCurrentNodeParentType<DictionaryNode>();
        }


        public virtual void BeginList(string id)
        {
            CheckCurrentNodeType<DictionaryNode>();
        }


        public virtual void EndList()
        {
            CheckCurrentNodeType<ListNode>();
            CheckCurrentNodeParentType<DictionaryNode>();
        }

        #endregion

        #region Value operations

        public void SyncBool(string id, ref bool value, bool defaultValue)
        {
            SyncValue(id, ref value, defaultValue);
        }


        public void SyncBool(string id, ref bool value)
        {
            SyncValue(id, ref value);
        }


        public void SyncInt(string id, ref int value, int defaultValue)
        {
            SyncValue(id, ref value, defaultValue);
        }


        public void SyncInt(string id, ref int value)
        {
            SyncValue(id, ref value);
        }


        public void SyncFloat(string id, ref float value, float defaultValue)
        {
            SyncValue(id, ref value, defaultValue);
        }


        public void SyncFloat(string id, ref float value)
        {
            SyncValue(id, ref value);
        }


        public void SyncString(string id, ref string value, string defaultValue)
        {
            SyncValue(id, ref value, defaultValue);
        }


        public void SyncString(string id, ref string value)
        {
            SyncValue(id, ref value);
        }


        public void SyncObject<T>(string id, ref T value, [NotNull] ISynchronizer<T> synchronizer)
        {
            SyncValue(id, ref value, synchronizer);
        }


        public void SyncObject<T>(string id, ref T value, T defaultValue, [NotNull] ISynchronizer<T> synchronizer)
        {
            SyncValue(id, ref value, defaultValue, synchronizer);
        }


        public void SyncObject<T>(string id, [NotNull] T obj)
            where T : class, ISynchronizable
        {
            CheckCurrentNodeType<DictionaryNode>();
            CheckObjectNotNull(obj);

            BeginDictionary(id);
            try
            {
                obj.Sync(this);
            }
            finally
            {
                EndDictionary();
            }
        }


        public void SyncObject<T>(string id, ref T obj, [NotNull] ISynchronizationFactory<T> factory)
            where T : class, ISynchronizable
        {
            CheckCurrentNodeType<DictionaryNode>();
            if (CheckObjectNode<DictionaryNode>(id))
            {
                BeginDictionary(id);
                try
                {
                    SyncObjectData(ref obj, factory);
                }
                finally
                {
                    EndDictionary();
                }
            }
        }


        public abstract void SyncObject<T>(string id, ref T obj, [NotNull] Func<T> creator)
            where T : class, ISynchronizable;

        #endregion

        #region Array operations

        public void SyncBoolArray(string id, ref bool[] array, bool isResizeAllowed)
        {
            CheckCurrentNodeType<DictionaryNode>();
            SyncValueArray(id, ref array, isResizeAllowed);
        }


        public void SyncIntArray(string id, ref int[] array, bool isResizeAllowed)
        {
            CheckCurrentNodeType<DictionaryNode>();
            SyncValueArray(id, ref array, isResizeAllowed);
        }


        public void SyncFloatArray(string id, ref float[] array, bool isResizeAllowed)
        {
            CheckCurrentNodeType<DictionaryNode>();
            SyncValueArray(id, ref array, isResizeAllowed);
        }


        public void SyncStringArray(string id, ref string[] array, bool isResizeAllowed)
        {
            CheckCurrentNodeType<DictionaryNode>();
            SyncValueArray(id, ref array, isResizeAllowed);
        }


        public virtual void SyncObjectArray<T>(
            string id,
            [NotNull] T[] array) where T : class, ISynchronizable
        {
            SyncObjectList(id, array);
        }


        public virtual void SyncObjectArray<T>(
            string id,
            ref T[] array,
            [NotNull] ISynchronizationFactory<T> factory) where T : class, ISynchronizable
        {
            CheckCurrentNodeType<DictionaryNode>();

            if (CheckObjectNode<ListNode>(id))
            {
                BeginList(id);
                try
                {
                    SyncObjectArrayElements(ref array, factory);
                }
                finally
                {
                    EndList();
                }
            }
        }


        public virtual void SyncObjectArray<T>(
            string id,
            ref T[] array,
            [NotNull] ISynchronizer<T> synchronizer)
        {
            CheckCurrentNodeType<DictionaryNode>();

            if (CheckObjectNode<ListNode>(id))
            {
                BeginList(id);
                try
                {
                    SyncObjectArrayElements(ref array, synchronizer);
                }
                finally
                {
                    EndList();
                }
            }
        }


        public virtual void SyncObjectArray<T>(
            string id,
            ref T[] array,
            [NotNull] Func<T> creator) where T : class, ISynchronizable
        {
            CheckCurrentNodeType<DictionaryNode>();

            if (CheckObjectNode<ListNode>(id))
            {
                BeginList(id);
                try
                {
                    SyncObjectArrayElements(ref array, creator);
                }
                finally
                {
                    EndList();
                }
            }
        }

        #endregion

        #region List operations

        public void SyncBoolList(string id, ref List<bool> list, bool isResizeAllowed)
        {
            SyncValueList(id, ref list, isResizeAllowed);
        }


        public void SyncIntList(string id, ref List<int> list, bool isResizeAllowed)
        {
            SyncValueList(id, ref list, isResizeAllowed);
        }


        public void SyncFloatList(string id, ref List<float> list, bool isResizeAllowed)
        {
            SyncValueList(id, ref list, isResizeAllowed);
        }


        public void SyncStringList(string id, ref List<string> list, bool isResizeAllowed)
        {
            SyncValueList(id, ref list, isResizeAllowed);
        }


        public virtual void SyncObjectList<T>(
            string id,
            IList<T> list) where T : ISynchronizable
        {
            CheckCurrentNodeType<DictionaryNode>();
            CheckObjectNotNull(list);

            BeginList(id);
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    T element = list[i];
                    SyncObjectElement(ref element);
                    list[i] = element;
                }
            }
            finally
            {
                EndList();
            }
        }


        public virtual void SyncObjectList<T>(
            string id,
            ref List<T> list,
            [NotNull] ISynchronizationFactory<T> factory) where T : class, ISynchronizable
        {
            CheckCurrentNodeType<DictionaryNode>();

            if (CheckObjectNode<ListNode>(id))
            {
                BeginList(id);
                try
                {
                    SyncObjectListElements(ref list, factory);
                }
                finally
                {
                    EndList();
                }
            }
        }


        public virtual void SyncObjectList<T>(
            string id,
            ref List<T> list,
            [NotNull] ISynchronizer<T> synchronizer)
        {
            CheckCurrentNodeType<DictionaryNode>();

            if (CheckObjectNode<ListNode>(id))
            {
                BeginList(id);
                try
                {
                    SyncObjectListElements(ref list, synchronizer);
                }
                finally
                {
                    EndList();
                }
            }
        }


        public void SyncObjectList<T>(
            string id,
            ref List<T> list,
            [NotNull] Func<T> creator) where T : ISynchronizable
        {
            CheckCurrentNodeType<DictionaryNode>();

            if (CheckObjectNode<ListNode>(id))
            {
                BeginList(id);
                try
                {
                    SyncObjectListElements(ref list, creator);
                }
                finally
                {
                    EndList();
                }
            }
        }

        #endregion

        #region Collection operations

        public void SyncBoolCollection(string id, [NotNull] ICollection<bool> collection)
        {
            SyncValueCollection(id, collection);
        }


        public void SyncIntCollection(string id, [NotNull] ICollection<int> collection)
        {
            SyncValueCollection(id, collection);
        }


        public void SyncFloatCollection(string id, [NotNull] ICollection<float> collection)
        {
            SyncValueCollection(id, collection);
        }


        public void SyncStringCollection(string id, [NotNull] ICollection<string> collection)
        {
            SyncValueCollection(id, collection);
        }


        public virtual void SyncObjectCollection<T>(
            string id,
            [NotNull] ICollection<T> collection,
            [NotNull] ISynchronizationFactory<T> factory) where T : class, ISynchronizable
        {
            CheckCurrentNodeType<DictionaryNode>();
            CheckObjectNotNull(collection);

            if (CheckObjectNode<ListNode>(id))
            {
                BeginList(id);
                try
                {
                    SyncObjectCollectionElements(collection, factory);
                }
                finally
                {
                    EndList();
                }
            }
        }


        public virtual void SyncObjectCollection<T>(
            string id,
            [NotNull] ICollection<T> collection,
            [NotNull] ISynchronizer<T> synchronizer)
        {
            CheckCurrentNodeType<DictionaryNode>();
            CheckObjectNotNull(collection);

            if (CheckObjectNode<ListNode>(id))
            {
                BeginList(id);
                try
                {
                    SyncObjectCollectionElements(collection, synchronizer);
                }
                finally
                {
                    EndList();
                }
            }
        }


        public virtual void SyncObjectCollection<T>(
            string id,
            [NotNull] ICollection<T> collection,
            [NotNull] Func<T> creator) where T : class, ISynchronizable
        {
            CheckCurrentNodeType<DictionaryNode>();
            CheckObjectNotNull(collection);

            if (CheckObjectNode<ListNode>(id))
            {
                BeginList(id);
                try
                {
                    SyncObjectCollectionElements(collection, creator);
                }
                finally
                {
                    EndList();
                }
            }
        }

        #endregion

        #region Dictionary operations

        public void SyncBoolDictionary(string id, ref Dictionary<string, bool> dictionary, bool isResizeAllowed)
        {
            CheckCurrentNodeType<DictionaryNode>();
            SyncValueDictionary(id, ref dictionary, isResizeAllowed);
        }


        public void SyncIntDictionary(string id, ref Dictionary<string, int> dictionary, bool isResizeAllowed)
        {
            CheckCurrentNodeType<DictionaryNode>();
            SyncValueDictionary(id, ref dictionary, isResizeAllowed);
        }


        public void SyncFloatDictionary(string id, ref Dictionary<string, float> dictionary, bool isResizeAllowed)
        {
            CheckCurrentNodeType<DictionaryNode>();
            SyncValueDictionary(id, ref dictionary, isResizeAllowed);
        }


        public void SyncStringDictionary(
            string id,
            ref Dictionary<string, string> dictionary,
            bool isResizeAllowed)
        {
            CheckCurrentNodeType<DictionaryNode>();
            SyncValueDictionary(id, ref dictionary, isResizeAllowed);
        }


        public virtual void SyncObjectDictionary<T>(
            string id,
            ref Dictionary<string, T> dictionary) where T : class, ISynchronizable
        {
            CheckCurrentNodeType<DictionaryNode>();
            CheckObjectNotNull(dictionary);
            CheckDictionaryElementsNotNull(dictionary);

            BeginDictionary(id);
            try
            {
                var keys = new List<string>(dictionary.Keys);
                foreach (string key in keys)
                {
                    if (CanSyncObjectInDictionary(key))
                    {
                        T obj = dictionary[key];
                        SyncObject(key, obj);
                        dictionary[key] = obj;
                    }
                }
            }
            finally
            {
                EndDictionary();
            }
        }


        public virtual void SyncObjectDictionary<T>(
            string id,
            ref Dictionary<string, T> dictionary,
            [NotNull] ISynchronizationFactory<T> factory) where T : class, ISynchronizable
        {
            CheckCurrentNodeType<DictionaryNode>();

            if (CheckObjectNode<DictionaryNode>(id))
            {
                BeginDictionary(id);
                try
                {
                    SyncDictionaryObjects(ref dictionary, factory);
                }
                finally
                {
                    EndDictionary();
                }
            }
        }


        public virtual void SyncObjectDictionary<T>(
            string id,
            ref Dictionary<string, T> dictionary,
            [NotNull] ISynchronizer<T> synchronizer)
        {
            CheckCurrentNodeType<DictionaryNode>();

            if (CheckObjectNode<DictionaryNode>(id))
            {
                BeginDictionary(id);
                try
                {
                    SyncDictionaryObjects(ref dictionary, synchronizer);
                }
                finally
                {
                    EndDictionary();
                }
            }
        }


        public virtual void SyncObjectDictionary<T>(
            string id,
            ref Dictionary<string, T> dictionary,
            [NotNull] Func<T> creator) where T : class, ISynchronizable
        {
            CheckCurrentNodeType<DictionaryNode>();

            if (CheckObjectNode<DictionaryNode>(id))
            {
                BeginDictionary(id);
                try
                {
                    SyncDictionaryObjects(ref dictionary, creator);
                }
                finally
                {
                    EndDictionary();
                }
            }
        }


        public virtual void SyncObjectDictionary<T, TU>(
            string id,
            ref Dictionary<T, TU> dictionary,
            [NotNull] IKeySynchronizer<T> keyProvider,
            [NotNull] Func<TU> creator)
            where T : class
            where TU : class, ISynchronizable
        {
            CheckCurrentNodeType<DictionaryNode>();

            if (CheckObjectNode<DictionaryNode>(id))
            {
                BeginDictionary(id);
                try
                {
                    SyncDictionaryObjects(ref dictionary, keyProvider, creator);
                }
                finally
                {
                    EndDictionary();
                }
            }
        }


        public virtual void SyncObjectDictionary<T, TU>(
            string id,
            ref Dictionary<T, TU> dictionary,
            [NotNull] IKeySynchronizer<T> keySynchronizer,
            [NotNull] ISynchronizer<TU> synchronizer)
        {
            CheckCurrentNodeType<DictionaryNode>();

            if (CheckObjectNode<DictionaryNode>(id))
            {
                BeginDictionary(id);
                try
                {
                    SyncDictionaryObjects(ref dictionary, keySynchronizer, synchronizer);
                }
                finally
                {
                    EndDictionary();
                }
            }
        }


        public virtual void SyncObjectDictionary<T, TU>(
            string id,
            ref Dictionary<T, TU> dictionary,
            [NotNull] IKeySynchronizer<T> keyProvider,
            [NotNull] ISynchronizationFactory<TU> factory)
            where T : class
            where TU : class, ISynchronizable
        {
            CheckCurrentNodeType<DictionaryNode>();

            if (CheckObjectNode<DictionaryNode>(id))
            {
                BeginDictionary(id);
                try
                {
                    SyncDictionaryObjects(ref dictionary, keyProvider, factory);
                }
                finally
                {
                    EndDictionary();
                }
            }
        }

        #endregion

        #endregion

        #region List operations

        #region Control operations

        public virtual void BeginDictionaryElement()
        {
            CheckCurrentNodeType<ListNode>();
        }


        public virtual void EndDictionaryElement()
        {
            CheckCurrentNodeType<DictionaryNode>();
            CheckCurrentNodeParentType<ListNode>();
        }


        public virtual void BeginListElement()
        {
            CheckCurrentNodeType<ListNode>();
        }


        public virtual void EndListElement()
        {
            CheckCurrentNodeType<ListNode>();
            CheckCurrentNodeParentType<ListNode>();
        }

        #endregion

        #region Value operations

        public void SyncBoolElement(ref bool value)
        {
            SyncValueElement(ref value);
        }


        public void SyncIntElement(ref int value)
        {
            SyncValueElement(ref value);
        }


        public void SyncFloatElement(ref float value)
        {
            SyncValueElement(ref value);
        }


        public void SyncStringElement(ref string value)
        {
            SyncValueElement(ref value);
        }


        public void SyncObjectElement<T>(ref T obj) where T : ISynchronizable
        {
            CheckCurrentNodeType<ListNode>();

            BeginDictionaryElement();
            try
            {
                obj.Sync(this);
            }
            finally
            {
                EndDictionaryElement();
            }
        }


        public void SyncObjectElement<T>(ref T obj, [NotNull] ISynchronizationFactory<T> factory)
            where T : class, ISynchronizable
        {
            CheckCurrentNodeType<ListNode>();

            if (CheckObjectElementNode<DictionaryNode>())
            {
                BeginDictionaryElement();
                try
                {
                    SyncObjectData(ref obj, factory);
                }
                finally
                {
                    EndDictionaryElement();
                }
            }
        }


        public virtual void SyncObjectElement<T>(ref T obj, [NotNull] ISynchronizer<T> synchronizer)
        {
            SyncValueElement(ref obj, synchronizer);
        }

        #endregion

        #region Array operations

        public void SyncBoolArrayElement(ref bool[] array, bool isResizeAllowed)
        {
            SyncValueArrayElement(ref array, isResizeAllowed);
        }


        public void SyncIntArrayElement(ref int[] array, bool isResizeAllowed)
        {
            SyncValueArrayElement(ref array, isResizeAllowed);
        }


        public void SyncFloatArrayElement(ref float[] array, bool isResizeAllowed)
        {
            SyncValueArrayElement(ref array, isResizeAllowed);
        }


        public void SyncStringArrayElement(ref string[] array, bool isResizeAllowed)
        {
            SyncValueArrayElement(ref array, isResizeAllowed);
        }


        public virtual void SyncObjectArrayElement<T>(T[] array)
            where T : class, ISynchronizable
        {
            SyncObjectListElement(array);
        }


        public virtual void SyncObjectArrayElement<T>(
            ref T[] array,
            [NotNull] ISynchronizationFactory<T> factory) where T : class, ISynchronizable
        {
            CheckCurrentNodeType<ListNode>();

            if (CheckObjectElementNode<ListNode>())
            {
                BeginListElement();
                try
                {
                    SyncObjectArrayElements(ref array, factory);
                }
                finally
                {
                    EndListElement();
                }
            }
        }


        public virtual void SyncObjectArrayElement<T>(
            ref T[] array,
            [NotNull] ISynchronizer<T> synchronizer)
        {
            CheckCurrentNodeType<ListNode>();

            if (CheckObjectElementNode<ListNode>())
            {
                BeginListElement();
                try
                {
                    SyncObjectArrayElements(ref array, synchronizer);
                }
                finally
                {
                    EndListElement();
                }
            }
        }

        #endregion

        #region List operations

        public void SyncBoolListElement(ref List<bool> list, bool isResizeAllowed)
        {
            SyncValueListElement(ref list, isResizeAllowed);
        }


        public void SyncIntListElement(ref List<int> list, bool isResizeAllowed)
        {
            SyncValueListElement(ref list, isResizeAllowed);
        }


        public void SyncFloatListElement(ref List<float> list, bool isResizeAllowed)
        {
            SyncValueListElement(ref list, isResizeAllowed);
        }


        public void SyncStringListElement(ref List<string> list, bool isResizeAllowed)
        {
            SyncValueListElement(ref list, isResizeAllowed);
        }


        public virtual void SyncObjectListElement<T>(IList<T> list)
            where T : class, ISynchronizable
        {
            CheckCurrentNodeType<ListNode>();
            CheckObjectNotNull(list);
            CheckObjectElementsNotNull(list);

            BeginListElement();
            try
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    T s = list[i];
                    SyncObjectElement(ref s);
                    list[i] = s;
                }
            }
            finally
            {
                EndListElement();
            }
        }


        public virtual void SyncObjectListElement<T>(
            ref List<T> list,
            [NotNull] ISynchronizationFactory<T> factory) where T : class, ISynchronizable
        {
            CheckCurrentNodeType<ListNode>();

            if (CheckObjectElementNode<ListNode>())
            {
                BeginListElement();
                try
                {
                    SyncObjectListElements(ref list, factory);
                }
                finally
                {
                    EndListElement();
                }
            }
        }


        public virtual void SyncObjectListElement<T>(
            ref List<T> list,
            [NotNull] ISynchronizer<T> synchronizer)
        {
            CheckCurrentNodeType<ListNode>();

            if (CheckObjectElementNode<ListNode>())
            {
                BeginListElement();
                try
                {
                    SyncObjectListElements(ref list, synchronizer);
                }
                finally
                {
                    EndListElement();
                }
            }
        }

        #endregion

        #region Collection operations

        public virtual void SyncObjectCollectionElement<T>(
            [NotNull] ICollection<T> collection,
            [NotNull] ISynchronizationFactory<T> factory) where T : class, ISynchronizable
        {
            CheckCurrentNodeType<ListNode>();
            CheckObjectNotNull(collection);

            if (CheckObjectElementNode<ListNode>())
            {
                BeginListElement();
                try
                {
                    SyncObjectCollectionElements(collection, factory);
                }
                finally
                {
                    EndListElement();
                }
            }
        }


        public virtual void SyncObjectCollectionElement<T>(
            [NotNull] ICollection<T> collection,
            [NotNull] ISynchronizer<T> synchronizer)
        {
            CheckCurrentNodeType<ListNode>();
            CheckObjectNotNull(collection);

            if (CheckObjectElementNode<ListNode>())
            {
                BeginListElement();
                try
                {
                    SyncObjectCollectionElements(collection, synchronizer);
                }
                finally
                {
                    EndListElement();
                }
            }
        }

        #endregion

        #region Dictionary operations

        public void SyncBoolDictionaryElement(
            ref Dictionary<string, bool> dictionary,
            bool isResizeAllowed)
        {
            SyncValueDictionaryElement(ref dictionary, isResizeAllowed);
        }


        public void SyncIntDictionaryElement(
            ref Dictionary<string, int> dictionary,
            bool isResizeAllowed)
        {
            SyncValueDictionaryElement(ref dictionary, isResizeAllowed);
        }


        public void SyncFloatDictionaryElement(
            ref Dictionary<string, float> dictionary,
            bool isResizeAllowed)
        {
            SyncValueDictionaryElement(ref dictionary, isResizeAllowed);
        }


        public void SyncStringDictionaryElement(
            ref Dictionary<string, string> dictionary,
            bool isResizeAllowed)
        {
            SyncValueDictionaryElement(ref dictionary, isResizeAllowed);
        }


        public virtual void SyncObjectDictionaryElement<T>(Dictionary<string, T> dictionary) where T : class, ISynchronizable
        {
            CheckCurrentNodeType<ListNode>();
            CheckObjectNotNull(dictionary);
            CheckDictionaryElementsNotNull(dictionary);

            BeginDictionaryElement();
            try
            {
                var keys = new List<string>(dictionary.Keys);
                foreach (string key in keys)
                {
                    T obj = dictionary[key];
                    SyncObject(key, obj);
                    dictionary[key] = obj;
                }
            }
            finally
            {
                EndDictionaryElement();
            }
        }


        public virtual void SyncObjectDictionaryElement<T>(
            ref Dictionary<string, T> dictionary,
            [NotNull] ISynchronizationFactory<T> factory) where T : class, ISynchronizable
        {
            CheckCurrentNodeType<ListNode>();

            if (CheckObjectElementNode<DictionaryNode>())
            {
                BeginDictionaryElement();
                try
                {
                    SyncDictionaryObjects(ref dictionary, factory);
                }
                finally
                {
                    EndDictionaryElement();
                }
            }
        }


        public virtual void SyncObjectDictionaryElement<T>(
            ref Dictionary<string, T> dictionary,
            [NotNull] ISynchronizer<T> synchronizer)
        {
            CheckCurrentNodeType<ListNode>();

            if (CheckObjectElementNode<DictionaryNode>())
            {
                BeginDictionaryElement();
                try
                {
                    SyncDictionaryObjects(ref dictionary, synchronizer);
                }
                finally
                {
                    EndDictionaryElement();
                }
            }
        }

        #endregion

        #endregion
    }
}
