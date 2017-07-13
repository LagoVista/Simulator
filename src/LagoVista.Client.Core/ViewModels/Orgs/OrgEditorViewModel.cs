using LagoVista.Core.Commanding;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.Core.ViewModels;
using LagoVista.UserAdmin.ViewModels.Organization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Client.Core.ViewModels.Orgs
{
    public class OrgEditorViewModel : FormViewModelBase<CreateOrganizationViewModel>
    {
        IClientAppInfo _clientAppInfo;

        public OrgEditorViewModel(IClientAppInfo clientAppInfo)
        {
            _clientAppInfo = clientAppInfo;

            LogoutCommand = new RelayCommand(Logout);

            MenuItems = new List<MenuItem>()
            {
                new MenuItem()
                {
                    Command = LogoutCommand,
                    Name = "Logout",
                    FontIconKey = "fa-sign-out"
                }
            };
        }

        public async Task<InvokeResult> SaveChangesAsync()
        {
            var saveResult = await FormRestClient.AddAsync("/api/org", this.Model);
            if (!saveResult.Successful) return saveResult;

            var refreshResult = await RefreshUserFromServerAsync();
            if (!refreshResult.Successful) return refreshResult;

            var launchArgs = new ViewModelLaunchArgs() { ViewModelType = _clientAppInfo.MainViewModel };
            await ViewModelNavigation.NavigateAsync(launchArgs);

            return InvokeResult.Success;
        }

        public override async void SaveAsync()
        {
            base.SaveAsync();
            if (FormAdapter.Validate())
            {
                ViewToModel(FormAdapter, Model);
                await PerformNetworkOperation(SaveChangesAsync);
            }
        }

        protected override string GetRequestUri()
        {
            return "/api/org/factory";
        }

        protected override void BuildForm(EditFormAdapter form)
        {
            form.AddViewCell(nameof(Model.Name));
            form.AddViewCell(nameof(Model.Namespace));
            form.AddViewCell(nameof(Model.WebSite));
        }

        public RelayCommand LogoutCommand { get; private set; }
    }
}