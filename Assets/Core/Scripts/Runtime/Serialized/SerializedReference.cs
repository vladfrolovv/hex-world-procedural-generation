#region

using System;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

namespace Core.Runtime.Serialized
{
    public abstract partial class BaseSerializedReference
    {
    }

    [Serializable]
    public partial class SerializedReference<T>
        where T : class
    {
        [SerializeField]
        private Object _object;

        private T _value;

        public T Value
        {
            get
            {
                if (_value == null)
                {
                    _value = _object as T;

                    if (_object != null && _value == null)
                    {
                        Debug.Log($"Invalid reference: object of type {_object.GetType().Name} isn't assignable to {typeof(T).Name}");
                    }
                }

                return _value;
            }
        }
    }
}
