#region

using System;
using Core.Runtime.Attributes;
using UnityEngine;

#endregion

namespace Core.Runtime.Base
{
    public abstract class BaseScriptableObject : ScriptableObject, IEquatable<BaseScriptableObject>
    {
        [SerializeField]
        [ReadOnly]
        private string _guid;

        public string GUID => _guid;

        bool IEquatable<BaseScriptableObject>.Equals(BaseScriptableObject other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return _guid == other._guid;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            BaseScriptableObject other = obj as BaseScriptableObject;

            return GUID.Equals(other.GUID);
        }

        public override int GetHashCode()
        {
            return !string.IsNullOrEmpty(_guid) ? _guid.GetHashCode() : base.GetHashCode();
        }

        public static bool operator ==(BaseScriptableObject lhs, BaseScriptableObject rhs)
        {
            if (ReferenceEquals(null, lhs) && ReferenceEquals(null, rhs))
            {
                return true;
            }

            if (ReferenceEquals(null, lhs))
            {
                return false;
            }

            if (ReferenceEquals(null, rhs))
            {
                return false;
            }

            return lhs._guid == rhs._guid;
        }

        public static bool operator !=(BaseScriptableObject lhs, BaseScriptableObject rhs)
        {
            return !(lhs == rhs);
        }
    }
}
