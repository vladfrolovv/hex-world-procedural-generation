#region

using System;
using Core.Runtime.Serialized;
using Core.Runtime.Synchronization.Nodes;
using Core.Runtime.Synchronization.States;
using Core.Runtime.Synchronization.Synchronizers;
using UnityEngine;

#endregion

namespace Core.Runtime.Synchronization
{
    public static class SynchronizationExtensions
    {
        public static void SyncDateTime(
            this State state,
            string id,
            ref DateTime dateTime)
        {
            state.SyncObject(id, ref dateTime, new DateTimeSynchronizer());
        }


        public static void SyncEnum<T>(this State state, string id, ref T value, T defaultValue = default)
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException($"{typeof(T)} must be enum");
            }

            state.SyncObject(id, ref value, defaultValue, new EnumSynchronizer<T>());
        }


        public static void SyncVector2(this State state, string id, ref Vector2 value)
        {
            float x = value.x;
            float y = value.y;
            state.SyncFloat($"{id}.x", ref x);
            state.SyncFloat($"{id}.y", ref y);
            value = new Vector2(x, y);
        }


        public static void SyncVector3(this State state, string id, ref Vector3 value)
        {
            float x = value.x;
            float y = value.y;
            float z = value.z;
            state.SyncFloat($"{id}.x", ref x);
            state.SyncFloat($"{id}.y", ref y);
            state.SyncFloat($"{id}.z", ref z);
            value = new Vector3(x, y, z);
        }


        public static void SyncTimeTicks(this State state, string id, ref SerializedTimeSpan value)
        {
            string ticksStr = value.Ticks.ToString();
            state.SyncString($"{id}.ticks", ref ticksStr);
            if (long.TryParse(ticksStr, out long ticks))
            {
                value = new TimeSpan(ticks);
            }
            else
            {
                Debug.Log($"Error syncing '{id}' parameter. Couldn't parse long value");
            }
        }


        public static void SyncTimeTicks(this State state, string id, ref TimeSpan value)
        {
            string ticksStr = value.Ticks.ToString();
            state.SyncString($"{id}.ticks", ref ticksStr);
            if (long.TryParse(ticksStr, out long ticks))
            {
                value = new TimeSpan(ticks);
            }
            else
            {
                Debug.Log($"Error syncing '{id}' parameter. Couldn't parse long value");
            }
        }


        public static DateTime GetDateTime(this DictionaryNode node, string id)
        {
            string stringValue = node.GetString(id);
            return DateTime.Parse(stringValue);
        }
    }
}
