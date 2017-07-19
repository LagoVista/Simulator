using LagoVista.Client.Core.ViewModels;
using LagoVista.Core.Commanding;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.DeviceManager.Core.ViewModels
{
    public class DeviceExplorerViewModel : ListViewModelBase<IoT.DeviceManagement.Core.Models.DeviceSummary>
    {
        public DeviceExplorerViewModel()
        {
            AddNewDeviceCommand = new RelayCommand(AddNewDevice);
        }

        public async void AddNewDevice()
        {
            await NavigateAndCreateAsync<ProvisionDeviceViewModel>(LaunchArgs.ChildId);
        }

        protected override string GetListURI()
        {
            return $"/api/org/{AuthManager.User.CurrentOrganization.Id}/devices/{LaunchArgs.ChildId}";
        }

        public RelayCommand AddNewDeviceCommand { get; private set; } 
    }
}
