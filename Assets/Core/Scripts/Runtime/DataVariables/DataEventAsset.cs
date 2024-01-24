#region

using System;
using Core.Runtime.Base;
using UnityEngine;

#endregion

namespace Core.Runtime.DataVariables
{
    [CreateAssetMenu(fileName = "DataEvent", menuName = "SwyTapp/Core/Runtime/Data Variables/Event")]
    public class DataEventAsset : BaseScriptableObject
    {
        public event Action Event;
        public event Action<object> ObjectEvent;


        public void Raise()
        {
            Event?.Invoke();
        }

        public void Raise(object obj)
        {
            ObjectEvent?.Invoke(obj);
        }
    }
}
