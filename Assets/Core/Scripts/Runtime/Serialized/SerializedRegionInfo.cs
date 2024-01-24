#region

using System;
using System.Globalization;
using UnityEngine;

#endregion

namespace Core.Runtime.Serialized
{
    [Serializable]
    public class SerializedRegionInfo
    {
        [SerializeField]
        private int _lcid;

        private RegionInfo _cachedRegionInfo;

        public RegionInfo Info
        {
            get
            {
                if (_cachedRegionInfo == null)
                {
                    _cachedRegionInfo = new RegionInfo(_lcid);
                }

                return _cachedRegionInfo;
            }
        }
    }
}
