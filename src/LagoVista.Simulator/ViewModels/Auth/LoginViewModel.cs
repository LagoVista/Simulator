using LagoVista.Core.Authentication.Interfaces;
using LagoVista.Core.Authentication.Models;
using LagoVista.Core.Commanding;
using LagoVista.Core.ViewModels;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LagoVista.Simulator.ViewModels.Auth
{
    public class LoginViewModel : ViewModelBase
    {
        IAuthClient _authClient;

        public LoginViewModel(IAuthClient authClient)
        {
            LoginCommand = new RelayCommand(Login);

            _authClient = authClient;

        }
        

        public async void Login()
        {
            var loginInfo = new AuthRequest()
            {
                Email = EmailAddress,
                Password = Password,
                UserName = EmailAddress
            };

            var result = await _authClient.LoginAsync(loginInfo);
            Debug.WriteLine(result.Success);
            if (result.Success)
            {
                var authResult = result.Result;
                await Storage.StoreKVP("AUTH", authResult);
                ShowViewModel<MainViewModel>();                
            }
        }

        public RelayCommand LoginCommand { get; private set; }

        private string _emailAddress;
        private string _password;

        public string EmailAddress
        {
            get { return _emailAddress; }
            set { Set(ref _emailAddress, value); }
        }

        public string Password
        {
            get { return _password; }
            set { Set(ref _password, value); }
        }
    }
}
