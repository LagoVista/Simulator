using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.Core.ViewModels;
using LagoVista.UserAdmin.ViewModels.Organization;

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
                        result = await FormRestClient.AddAsync("/api/org", this.Model);
                    }
                    else
                    {
                        result = await FormRestClient.UpdateAsync("/api/org", this.Model);
                    }

                    var launchArgs = new ViewModelLaunchArgs(){ ViewModelType = _clientAppInfo.MainViewModel };
                    await ViewModelNavigation.NavigateAsync(launchArgs);
                });
            }
        }

        protected override void BuildForm(EditFormAdapter form)
        {
            form.AddViewCell(nameof(Model.Name));
            form.AddViewCell(nameof(Model.Namespace));
            form.AddViewCell(nameof(Model.WebSite));
        }

        protected override string GetRequestUri()
        {
            return "/api/org/factory";
        }
    }
}
