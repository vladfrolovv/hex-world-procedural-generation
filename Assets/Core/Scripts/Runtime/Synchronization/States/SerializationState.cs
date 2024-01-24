#region

using System;
using System.Collections.Generic;
using Core.Runtime.Synchronization.Nodes;
using JetBrains.Annotations;

#endregion

namespace Core.Runtime.Synchronization.States
{
    public class SerializationState : State
    {
        public SerializationState() :
            base(new DictionaryNode("root"))
        {
        }


        public new DictionaryNode Root => base.Root;


        public override void BeginDictionary(string id)
        {
            base.BeginDictionary(id);

            CurrentNode = CurrentDictionary.SetDictionary(id);
        }


        public override void EndDictionary()
        {
            base.EndDictionary();

            EndContainer();
        }


        public override void BeginList(string id)
        {
            base.BeginList(id);

            CurrentNode = CurrentDictionary.SetList(id);
        }


        public override void EndList()
        {
            base.EndList();

            EndContainer();
        }


        public override void BeginDictionaryElement()
        {
            base.BeginDictionaryElement();

            CurrentNode = CurrentList.AddDictionary();
        }


        public override void EndDictionaryElement()
        {
            base.EndDictionaryElement();

            EndContainer();
        }


        public override void BeginListElement()
        {
            base.BeginListElement();

            CurrentNode = CurrentList.AddList();
        }


        public override void EndListElement()
        {
            base.EndListElement();

            EndContainer();
        }


        public override void SyncObject<T>(string id, ref T obj, Func<T> creator)
        {
            if (obj == null)
            {
                CurrentDictionary.SetNull(id);
            }
            else
            {
                SyncObject(id, obj);
            }
        }


        public override void SyncValueElement<T>(ref T value, ISynchronizer<T> synchronizer = null)
        {
            base.SyncValueElement(ref value);
            if (synchronizer != null)
            {
                CurrentList.AddNode(synchronizer.Serialize(value));
            }
            else
            {
                CurrentList.AddValue(value);
            }
        }


        protected override void SyncValue<T>(
            string id,
            ref T value,
            T defaultValue,
            ISynchronizer<T> synchronizer = null)
        {
            base.SyncValue(id, ref value, defaultValue, synchronizer);

            if (!EqualityComparer<T>.Default.Equals(value, defaultValue))
            {
                if (synchronizer != null)
                {
                    CurrentDictionary.SetNode(id, synchronizer.Serialize(value));
                }
                else
                {
                    CurrentDictionary.SetValue(id, value);
                }
            }
        }


        protected override void SyncValue<T>(string id, ref T value, ISynchronizer<T> synchronizer = null)
        {
            base.SyncValue(id, ref value, synchronizer);

            if (synchronizer != null)
            {
                CurrentDictionary.SetNode(id, synchronizer.Serialize(value));
            }
            else
            {
                CurrentDictionary.SetValue(id, value);
            }
        }


        protected override bool CheckObjectNode<TNode>(string id)
        {
            return true;
        }


        protected override bool CheckObjectElementNode<TNode>()
        {
            return true;
        }


        protected override void SyncValueArray<T>(string id, ref T[] array, bool isResizeAllowed)
        {
            base.SyncValueArray(id, ref array, isResizeAllowed);

            SyncListValues(id, array);
        }


        protected override void SyncValueList<T>(string id, ref List<T> list, bool isResizeAllowed)
        {
            base.SyncValueList(id, ref list, isResizeAllowed);

            SyncListValues(id, list);
        }


        protected override void SyncValueDictionary<T>(
            string id,
            ref Dictionary<string, T> dictionary,
            bool isResizeAllowed)
        {
            base.SyncValueDictionary(id, ref dictionary, isResizeAllowed);

            BeginDictionary(id);
            try
            {
                SyncDictionary(ref dictionary);
            }
            finally
            {
                EndDictionary();
            }
        }


        protected override void SyncValueArrayElement<T>(ref T[] array, bool isResizeAllowed)
        {
            base.SyncValueArrayElement(ref array, isResizeAllowed);

            SyncListValues(array);
        }


        protected override void SyncValueListElement<T>(ref List<T> list, bool isResizeAllowed)
        {
            base.SyncValueListElement(ref list, isResizeAllowed);

            SyncListValues(list);
        }


        protected override void SyncValueDictionaryElement<T>(
            ref Dictionary<string, T> dictionary,
            bool isResizeAllowed)
        {
            base.SyncValueDictionaryElement(ref dictionary, isResizeAllowed);

            BeginDictionaryElement();
            try
            {
                SyncDictionary(ref dictionary);
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
            SyncEnumerableObjectElements(array, factory);
        }


        protected override void SyncObjectListElements<T>(
            ref List<T> list,
            ISynchronizationFactory<T> factory)
        {
            SyncEnumerableObjectElements(list, factory);
        }


        protected override void SyncObjectCollectionElements<T>(
            [NotNull] ICollection<T> collection,
            ISynchronizationFactory<T> factory)
        {
            SyncEnumerableObjectElements(collection, factory);
        }


        protected override void SyncDictionaryObjects<T>(
            ref Dictionary<string, T> dictionary,
            ISynchronizationFactory<T> factory)
        {
            var keys = new List<string>(dictionary.Keys);
            foreach (string key in keys)
            {
                T obj = dictionary[key];
                SyncObject(key, ref obj, factory);
                dictionary[key] = obj;
            }
        }


        protected override void SyncObjectArrayElements<T>(
            ref T[] array,
            ISynchronizer<T> synchronizer)
        {
            SyncEnumerableObjectElements(array, synchronizer);
        }


        protected override void SyncObjectListElements<T>(
            ref List<T> list,
            ISynchronizer<T> synchronizer)
        {
            SyncEnumerableObjectElements(list, synchronizer);
        }


        protected override void SyncObjectCollectionElements<T>(
            [NotNull] ICollection<T> collection,
            ISynchronizer<T> synchronizer)
        {
            SyncEnumerableObjectElements(collection, synchronizer);
        }


        protected override void SyncDictionaryObjects<T>(
            ref Dictionary<string, T> dictionary,
            ISynchronizer<T> synchronizer)
        {
            var keys = new List<string>(dictionary.Keys);
            foreach (string key in keys)
            {
                T obj = dictionary[key];
                SyncObject(key, ref obj, synchronizer);
            }
        }


        protected override void SyncObjectArrayElements<T>(
            ref T[] array,
            Func<T> creator)
        {
            SyncEnumerableObjectElements(array);
        }


        protected override void SyncObjectListElements<T>(
            ref List<T> list,
            Func<T> creator)
        {
            SyncEnumerableObjectElements(list);
        }


        protected override void SyncObjectCollectionElements<T>(
            ICollection<T> collection,
            Func<T> creator)
        {
            SyncEnumerableObjectElements(collection);
        }


        protected override void SyncDictionaryObjects<T>(
            ref Dictionary<string, T> dictionary,
            Func<T> creator)
        {
            var keys = new List<string>(dictionary.Keys);
            foreach (string key in keys)
            {
                T obj = dictionary[key];
                SyncObject(key, ref obj, creator);
                dictionary[key] = obj;
            }
        }


        protected override void SyncDictionaryObjects<T, TU>(
            ref Dictionary<T, TU> dictionary,
            IKeySynchronizer<T> keySynchronizer,
            Func<TU> creator)
        {
            var keys = new List<T>(dictionary.Keys);
            foreach (T key in keys)
            {
                string keyId = keySynchronizer.Serialize(key);
                TU obj = dictionary[key];
                SyncObject(keyId, ref obj, creator);
                dictionary[key] = obj;
            }
        }


        protected override void SyncDictionaryObjects<T, TU>(
            ref Dictionary<T, TU> dictionary,
            IKeySynchronizer<T> keySynchronizer,
            [NotNull] ISynchronizer<TU> synchronizer)
        {
            var keys = new List<T>(dictionary.Keys);
            foreach (T key in keys)
            {
                string keyId = keySynchronizer.Serialize(key);
                TU obj = dictionary[key];
                SyncObject(keyId, ref obj, synchronizer);
                dictionary[key] = obj;
            }
        }


        protected override void SyncDictionaryObjects<T, TU>(
            ref Dictionary<T, TU> dictionary,
            IKeySynchronizer<T> keySynchronizer,
            ISynchronizationFactory<TU> synchronizer)
        {
            var keys = new List<T>(dictionary.Keys);
            foreach (T key in keys)
            {
                string keyId = keySynchronizer.Serialize(key);
                TU obj = dictionary[key];
                SyncObject(keyId, ref obj, synchronizer);
                dictionary[key] = obj;
            }
        }


        protected override void SyncFactory<T>(string id, ISynchronizationFactory<T> factory, ref T obj)
        {
            CurrentDictionary.SetNode(id, factory.Serialize(obj));
        }


        protected override bool CanSyncObjectInDictionary(string id)
        {
            return true;
        }


        protected override Type GetCurrentNodeType()
        {
            return CurrentNode.GetType();
        }


        protected override Type GetCurrentNodeParentType()
        {
            return CurrentNode.Parent?.GetType();
        }


        private void SyncEnumerableObjectElements<T>(
            IEnumerable<T> enumerable,
            ISynchronizationFactory<T> factory) where T : class, ISynchronizable
        {
            foreach (T obj in enumerable)
            {
                T element = obj;
                SyncObjectElement(ref element, factory);
            }
        }


        private void SyncEnumerableObjectElements<T>(
            IEnumerable<T> enumerable,
            ISynchronizer<T> synchronizer)
        {
            foreach (T obj in enumerable)
            {
                T element = obj;
                SyncObjectElement(ref element, synchronizer);
            }
        }


        private void SyncEnumerableObjectElements<T>(IEnumerable<T> enumerable) where T : ISynchronizable
        {
            foreach (T obj in enumerable)
            {
                if (EqualityComparer<T>.Default.Equals(obj, default(T)))
                {
                    CurrentList.AddNull();
                }
                else
                {
                    T o = obj;
                    SyncObjectElement(ref o);
                }
            }
        }


        private void SyncDictionary<T>(ref Dictionary<string, T> dictionary)
        {
            foreach (KeyValuePair<string, T> pair in dictionary)
            {
                T val = pair.Value;
                SyncValue(pair.Key, ref val);
            }
        }


        private void SyncListValues<T>(string id, IEnumerable<T> list)
        {
            if (list == null)
            {
                CurrentDictionary.SetNull(id);
                return;
            }

            BeginList(id);
            try
            {
                SyncEnumerableValues(list);
            }
            finally
            {
                EndList();
            }
        }


        private void SyncListValues<T>(IEnumerable<T> list)
        {
            if (list == null)
            {
                CurrentList.AddNull();
                return;
            }

            BeginListElement();
            try
            {
                SyncEnumerableValues(list);
            }
            finally
            {
                EndListElement();
            }
        }


        private void SyncEnumerableValues<T>(IEnumerable<T> list)
        {
            foreach (T value in list)
            {
                T val = value;
                SyncValueElement(ref val);
            }
        }
    }
}
