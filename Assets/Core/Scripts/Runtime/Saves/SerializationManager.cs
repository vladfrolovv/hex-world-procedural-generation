#region

using System;
using System.Collections.Generic;
using Core.Runtime.Base;
using Core.Runtime.FileFolders;
using Core.Runtime.Synchronization;
using Core.Runtime.Synchronization.Serializers;
using Core.Runtime.Synchronization.States;
using UnityEngine;

#endregion

namespace Core.Runtime.Saves
{
    public class SerializationManager : BaseBehaviour
    {
        private FileFolder _regularSavesFolder;
        private HashSet<SerializationContext> _saveContexts = new HashSet<SerializationContext>();

        public SerializationHelper SerializationHelper { get; private set; }

        public bool IsSavingEnabled { get; set; } = true;


        protected void Awake()
        {
            _regularSavesFolder = new FileFolder();
            _regularSavesFolder.PushPath(Application.persistentDataPath);
            SerializationHelper = new SerializationHelper(_regularSavesFolder, new JsonNodeSerializer());
        }


        protected void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                Save();
            }
        }


        protected void OnApplicationQuit()
        {
            Save();
        }

        public event Action Saved;

        public event Action SaveStarted;


        public void Save()
        {
            if (IsSavingEnabled)
            {
                SaveStarted?.Invoke();

                foreach (SerializationContext saveContext in _saveContexts)
                {
                    SerializationState state = new SerializationState();
                    ISynchronizable synchronizable = saveContext;
                    synchronizable.Sync(state);

                    SerializationHelper.SaveStateToFile(state, saveContext.Id.FileName);
                }

                Saved?.Invoke();
            }
        }


        public void RegisterContext(SerializationContext serializationContext)
        {
            if (_saveContexts.Contains(serializationContext))
            {
                throw new ArgumentException($"Context with id {serializationContext.Id.name} is already registered.");
            }

            _saveContexts.Add(serializationContext);
        }


        public void UnregisterContext(SerializationContext serializationContext)
        {
            if (_saveContexts.Contains(serializationContext))
            {
                _saveContexts.Remove(serializationContext);
            }
        }
    }
}
