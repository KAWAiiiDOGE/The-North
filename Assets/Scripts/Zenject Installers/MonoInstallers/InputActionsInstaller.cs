using Zenject;

namespace TheNorth.Installers
{
    public class InputActionsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<InputActions>()
                     .AsSingle()
                     .NonLazy();
        }
    }
}