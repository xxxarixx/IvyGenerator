using Zenject;
using UnityEngine;
using Player;
namespace DependencyLearn
{
    class ConfigInstaller : MonoInstaller<ConfigInstaller>
    {
        [SerializeField]
        PlayerStatsConfig playerStatsConfig;
        public override void InstallBindings()
        {
            Container.Bind<PlayerStatsConfig>().FromInstance(playerStatsConfig).AsSingle();
        }
    }
}
