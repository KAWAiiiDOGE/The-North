using Zenject;

namespace TheNorth.Installers
{
    public class GameFlowManagerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameFlowManager>()
                     .AsSingle()
                     .NonLazy();
        }
    }
}