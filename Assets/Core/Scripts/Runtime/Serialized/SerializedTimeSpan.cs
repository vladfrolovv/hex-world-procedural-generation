#region

using System;
using UnityEngine;

#endregion

namespace Core.Runtime.Serialized
{
    [Serializable]
    public struct SerializedTimeSpan
    {
        [SerializeField]
        private long _ticks;


        private SerializedTimeSpan(long ticks)
        {
            _ticks = ticks;
        }

        public long Ticks => _ticks;

        public float TotalSeconds => (float)((TimeSpan)this).TotalSeconds;


        public static implicit operator SerializedTimeSpan(TimeSpan from)
        {
            return new SerializedTimeSpan(from.Ticks);
        }


        public static implicit operator TimeSpan(SerializedTimeSpan from)
        {
            return TimeSpan.FromTicks(from._ticks);
        }
    }
}
