using Zenject;
using UnityEngine;
using Props;

namespace DependencyLearn
{
    class SceneInstaller : MonoInstaller<SceneInstaller>
    {
        [SerializeField]
        GameObject _propWithRb;
        [SerializeField]
        Transform _propsHolder;
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<RandomPropsSystem>()
                     .FromInstance(new RandomPropsSystem(_propWithRb, _propsHolder))
                     .AsSingle();
        }
    }
}
