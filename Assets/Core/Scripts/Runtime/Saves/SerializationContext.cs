#region

using System;
using System.Collections.Generic;
using Core.Runtime.Base;
using Core.Runtime.Synchronization;
using Core.Runtime.Synchronization.States;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

#endregion

namespace Core.Runtime.Saves
{
    public class SerializationContext :
        BaseBehaviour,
        IDisposable,
        ISynchronizable
    {
        [SerializeField]
        private SaveId _id;

        [Inject]
        private SerializationManager _serializationManager;

        private Dictionary<string, ISynchronizable> _synchronizables = new Dictionary<string, ISynchronizable>();

        public SaveId Id => _id;

        public bool IsLoaded { get; private set; }


        protected void Start()
        {
            IsLoaded = true;
            _serializationManager.RegisterContext(this);

            DeserializationState deserializationState = _serializationManager.SerializationHelper.LoadStateFromFile(_id.FileName);
            if (deserializationState != null)
            {
                ISynchronizable self = this;
                self.Sync(deserializationState);
            }

            Loaded?.Invoke();
        }


        void IDisposable.Dispose()
        {
            _serializationManager.UnregisterContext(this);

            IsLoaded = false;
            _synchronizables.Clear();
        }


        void ISynchronizable.Sync(State state)
        {
            foreach (KeyValuePair<string, ISynchronizable> pair in _synchronizables)
            {
                string id = pair.Key;
                ISynchronizable synchronizable = pair.Value;

                try
                {
                    state.SyncObject(id, synchronizable);
                }
                catch (Exception exception)
                {
#if UNITY_EDITOR
                    Debug.LogException(exception);
#else
                    Debug.Log("Error while synchronizing " + id, synchronizable as Object);
                    Debug.Log(exception.Message);
#endif
                }
            }
        }

        public event Action Loaded;


        public void RegisterSynchronizable(string id, ISynchronizable saveable)
        {
            if (IsLoaded)
            {
                Debug.Log("Registration of {id} happens after synchronization has occured.", saveable as Object);
                return;
            }

            if (_synchronizables.ContainsKey(id))
            {
                Debug.Log($"Save group '{name}' already contains synchronizable with id '{id}'.", saveable as Object);
                return;
            }

            _synchronizables.Add(id, saveable);
        }
    }
}
