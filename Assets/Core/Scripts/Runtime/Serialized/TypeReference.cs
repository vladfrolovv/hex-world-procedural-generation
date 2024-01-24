#region

using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

#endregion

namespace Core.Runtime.Serialized
{
    public abstract class DrawableTypeReference
    {
    }

    [Serializable]
    public class TypeReference<TBaseType> : DrawableTypeReference, ISerializationCallbackReceiver
        where TBaseType : class
    {
        [SerializeField]
        protected string _serializedType = "";


        public TypeReference(Type type, bool areAbstractTypesAllowed)
        {
            Assert.IsTrue(IsValidType(type, areAbstractTypesAllowed));
            _serializedType = Serialize(type);
        }


        protected TypeReference()
        {
        }


        public Type Type { get; private set; }


        public void OnBeforeSerialize()
        {
        }


        public void OnAfterDeserialize()
        {
            Type = Type.GetType(_serializedType);
        }


        [Obfuscation(Exclude = false, Feature = "-rename")]
        public static string Serialize(Type type)
        {
            return type.AssemblyQualifiedName;
        }


        [Obfuscation(Exclude = false, Feature = "-rename")]
        public static bool IsValidType(Type type, bool areAbstractTypesAllowed)
        {
            return (areAbstractTypesAllowed || !type.IsAbstract) && typeof(TBaseType).IsAssignableFrom(type);
        }
    }

    /// <summary>
    ///     Attribute for TypeReference
    /// </summary>
    public class AllowAbstractTypesAttribute : Attribute
    {
    }
}
