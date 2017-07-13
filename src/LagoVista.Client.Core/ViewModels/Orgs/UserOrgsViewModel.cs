using LagoVista.Core.Validation;
using LagoVista.UserAdmin.Models.Orgs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.Client.Core.ViewModels.Orgs
{
    public class UserOrgsViewModel : AppViewModelBase
    {
        public async Task<InvokeResult> RequestUserOrgsAsync()
        {
            var orgResult = await RestClient.GetAsync<List<OrgUser>>($"/api/user/{AuthManager.User.Id}/orgs");
            if(orgResult.Successful)
            {
                UserOrgs = orgResult.Result;
            }

            return orgResult.ToInvokeResult();
        }

        public override Task InitAsync()
        {
            return PerformNetworkOperation(RequestUserOrgsAsync);
        }

        public List<OrgUser> UserOrgs { get; private set; }
    }
}
