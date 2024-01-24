#if UNITY_EDITOR

#region

using System;
using UnityEditor;
using Object = UnityEngine.Object;

#endregion

namespace Core.Runtime.Serialized
{
    public abstract partial class BaseSerializedReference : SerializedReferenceDrawerTarget
    {
        public abstract Object FieldValue { get; }

        public abstract Type FieldType { get; }

        public abstract void SetFieldValue(SerializedProperty property, Object asset);
    }

    public partial class SerializedReference<T> : BaseSerializedReference, SerializedReference<T>.IEditorApi
        where T : class
    {
        public override Object FieldValue => _object;

        public override Type FieldType => typeof(T);


        void IEditorApi.SetValue(T value)
        {
            if (value is Object)
            {
                _object = value as Object;
            }
            else
            {
                throw new ArgumentException(
                    "Invalid reference value type. Value need to inherit UnityEngine.Object.",
                    nameof(value)
                );
            }
        }


        public override void SetFieldValue(SerializedProperty property, Object asset)
        {
            property = property.FindPropertyRelative(nameof(_object));
            property.objectReferenceValue = asset;
        }


        #region IEditorApi

        public interface IEditorApi
        {
            void SetValue(T value);
        }

        #endregion
    }

    public abstract class SerializedReferenceDrawerTarget
    {
    }
}

#endif
