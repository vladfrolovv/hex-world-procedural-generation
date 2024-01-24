#region

using Core.Runtime.Base;
using UnityEngine;

#endregion

namespace Core.Runtime.Saves
{
    [CreateAssetMenu(fileName = "SaveId", menuName = "SwyTapp/Core/Runtime/Save Id")]
    public class SaveId : BaseScriptableObject
    {
        [SerializeField]
        private string _fileName;

        public string FileName => _fileName;
    }
}
