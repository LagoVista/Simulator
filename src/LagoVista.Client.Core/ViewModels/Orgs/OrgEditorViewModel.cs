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
    public class OrgEditorViewModel : IoTAppViewModelBase<CreateOrganizationViewModel>
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

        public async Task<bool> SaveChangesAsync()
        {
            var saveResult = await RestClient.AddAsync("/api/org", this.Model);
            if (saveResult.Successful)
            {
                var refreshResult = await RefreshUserFromServerAsync();
                if (refreshResult.Successful)
                {
                    var launchArgs = new ViewModelLaunchArgs() { ViewModelType = _clientAppInfo.MainViewModel };
                    await ViewModelNavigation.NavigateAsync(launchArgs);
                }
                else
                {
                    await ShowServerErrorMessageAsync(refreshResult);
                }
            }
            else
            {
                await ShowServerErrorMessageAsync(saveResult);
            }

            return true;
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

        public async Task<bool> PopulateUIAsync()
        {
            var newOrgTemplate = await RestClient.CreateNewAsync("/api/org/factory");

            var form = new EditFormAdapter(newOrgTemplate.Model, newOrgTemplate.View, ViewModelNavigation);
            Model = newOrgTemplate.Model;
            View = newOrgTemplate.View;

            form.AddViewCell(nameof(Model.Name));
            form.AddViewCell(nameof(Model.Namespace));
            form.AddViewCell(nameof(Model.WebSite));

            ModelToView(Model, form);
            FormAdapter = form;

            return true;
        }

        public override async Task InitAsync()
        {
            await PerformNetworkOperation(PopulateUIAsync);
        }


        public RelayCommand LogoutCommand { get; private set; }
    }
}