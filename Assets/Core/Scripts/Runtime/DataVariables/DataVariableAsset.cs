#region

using System;
using Core.Runtime.Base;
using UnityEngine;

#endregion

namespace Core.Runtime.DataVariables
{
    [CreateAssetMenu(fileName = "DataEvent", menuName = "SwyTapp/Core/Runtime/Data Variables/Object")]
    public class DataVariableAsset : BaseScriptableObject
    {
        private object _value;

        public object Value
        {
            get => _value;
            set
            {
                if (Value != value)
                {
                    _value = value;
                    ValueChanged?.Invoke(Value);
                }
            }
        }

        public event Action<object> ValueChanged;


        public T GetValue<T>() where T : class
        {
            return Value as T;
        }


        public bool CheckEquality(object other)
        {
            return Value != null && other != null && other.Equals(Value);
        }
    }
}
