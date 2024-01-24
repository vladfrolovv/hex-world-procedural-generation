#region

using Game.Runtime.Maps.MapObjects;
using UnityEngine;
using Zenject;

#endregion

namespace Game.Runtime.Dependencies
{
    [CreateAssetMenu(fileName = "Configs", menuName = "Game/Runtime/Configs")]
    public class ConfigsInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private MapObjectsConfig _mapObjectsConfig;


        public override void InstallBindings()
        {
            Container.BindInstance(_mapObjectsConfig);
        }
    }
}
