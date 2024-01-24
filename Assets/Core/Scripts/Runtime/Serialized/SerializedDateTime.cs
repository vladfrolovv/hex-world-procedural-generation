#region

using System;
using System.Globalization;
using UnityEngine;

#endregion

namespace Core.Runtime.Serialized
{
    [Serializable]
    public class SerializedDateTime
    {
        [SerializeField]
        private string _date;

        [SerializeField]
        private bool _isLocal;

        [SerializeField]
        [HideInInspector]
        private string _minLimit;

        [SerializeField]
        [HideInInspector]
        private string _maxLimit;

        private string _cachedDateString;
        private DateTime? _cachedDateTime;


        private SerializedDateTime(DateTime date)
        {
            _date = date.ToString(CultureInfo.InvariantCulture);
        }

        public DateTime Date
        {
            get
            {
                if (_cachedDateString != _date)
                {
                    _cachedDateString = string.IsNullOrEmpty(_date) ? GetDateTimeMinValueString() : _date;
                    _cachedDateTime = null;
                }

                if (_cachedDateTime == null)
                {
                    _cachedDateTime = Parse(_cachedDateString);
                }

                return _cachedDateTime.Value;
            }
        }


        private DateTimeKind _kind => _isLocal ? DateTimeKind.Local : DateTimeKind.Utc;


        public static implicit operator SerializedDateTime(DateTime from)
        {
            return new SerializedDateTime(from);
        }


        public static implicit operator DateTime(SerializedDateTime from)
        {
            return from.Date;
        }


        private string GetDateTimeMinValueString()
        {
            return DateTime.MinValue.ToString(CultureInfo.InvariantCulture);
        }


        private DateTime Parse(string dateTimeString)
        {
            return DateTime.SpecifyKind(DateTime.Parse(dateTimeString, CultureInfo.InvariantCulture), _kind);
        }
    }
}
