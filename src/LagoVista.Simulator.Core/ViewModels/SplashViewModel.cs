using LagoVista.Core.Commanding;
using System.Threading.Tasks;

namespace LagoVista.Simulator.Core.ViewModels
{
    public class SplashViewModel : SimulatorViewModelBase
    {

        public SplashViewModel()
        {
            LoginCommand = new RelayCommand(Login);
        }

        public override async Task InitAsync()
        {
            await AuthManager.LoadAsync();
            if(AuthManager.IsAuthenticated)
            {
                await ViewModelNavigation.SetAsNewRootAsync<MainViewModel>();
            }
        }

        public async void Login()
        {
            await  ViewModelNavigation.SetAsNewRootAsync<Auth.LoginViewModel>();
        }

        public RelayCommand LoginCommand { get; private set; }
    }
}