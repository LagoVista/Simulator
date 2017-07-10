using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.Core.ViewModels;
using LagoVista.UserAdmin.ViewModels.Organization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Client.Core.ViewModels.Users
{
    public class OrganizationViewModel : FormViewModelBase<CreateOrganizationViewModel>
    {
        IClientAppInfo _clientAppInfo;

        public OrganizationViewModel(IClientAppInfo clientAppInfo)
        {
            _clientAppInfo = clientAppInfo;
        }


        public override void SaveAsync()
        {
            base.SaveAsync();
            if (FormAdapter.Validate())
            {
                ViewToModel(FormAdapter, Model);
                PerformNetworkOperation(async () =>
                {
                    InvokeResult result;


                    if (LaunchArgs.LaunchType == LaunchTypes.Create)
                    {
                        result = await RestClient.AddAsync("/api/org", this.Model);
                    }
                    else
                    {
                        result = await RestClient.UpdateAsync("/api/org", this.Model);
                    }

                    var launchArgs = new ViewModelLaunchArgs(){ ViewModelType = _clientAppInfo.MainViewModel };
                    await ViewModelNavigation.NavigateAsync(launchArgs);
                });
            }
        }

        public override async Task InitAsync()
        {
            await PerformNetworkOperation(async () =>
            {
                var newMessageTemplate = await RestClient.CreateNewAsync("/api/org/factory");

                var form = new EditFormAdapter(newMessageTemplate.Model, newMessageTemplate.View, ViewModelNavigation);
                Model = newMessageTemplate.Model;
                View = newMessageTemplate.View;

                form.AddViewCell(nameof(Model.Name));
                form.AddViewCell(nameof(Model.Namespace));
                form.AddViewCell(nameof(Model.WebSite));
            });
        }
    }
}
