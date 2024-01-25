#region

using Core.Runtime.Base;
using UnityEngine;

#endregion

namespace Game.Runtime.Cameras
{
    public class CameraInstaller : BaseBehaviour
    {
        [SerializeField] private float _offset = -.25f;
        [SerializeField] private Camera _camera;

        public void Install(Vector2Int size)
        {
            int max = Mathf.Max(size.x, size.y);
            _camera.transform.position = new Vector3(_offset * max, max, _offset * max);
        }

    }
}
