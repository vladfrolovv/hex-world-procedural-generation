#region

using System;
using UnityEngine;

#endregion

namespace Core.Runtime.Serialized
{
    [Serializable]
    public class SerializedDateTimeInterval
    {
        [SerializeField]
        private SerializedDateTime _from;

        [SerializeField]
        private SerializedDateTime _to;

        [SerializeField]
        [HideInInspector]
        private string _minLimit;

        [SerializeField]
        [HideInInspector]
        private string _maxLimit;


        public SerializedDateTimeInterval(DateTime from, DateTime to)
        {
            From = from;
            To = to;
        }


        public DateTime From
        {
            get => _from.Date.ToLocalTime();
            set => _from = value.ToUniversalTime();
        }

        public DateTime To
        {
            get => _to.Date.ToLocalTime();
            set => _to = value.ToUniversalTime();
        }


        public bool IsActual(DateTime now)
        {
            return To >= now;
        }


        public bool IsInInterval(DateTime now)
        {
            return now >= From && To >= now;
        }


        public bool Intersect(SerializedDateTimeInterval other)
        {
            return IsInInterval(other.From) || IsInInterval(other.To);
        }
    }
}
