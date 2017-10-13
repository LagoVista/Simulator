using LagoVista.Client.Core.ViewModels;
using LagoVista.Core.Commanding;
using LagoVista.Core.ViewModels;
using LagoVista.IoT.DeviceManagement.Core.Models;

namespace LagoVista.DeviceManager.Core.ViewModels
{
    public class DeviceExplorerViewModel : ListViewModelBase<DeviceSummary>
    {
        public DeviceExplorerViewModel()
        {
            AddNewDeviceCommand = new RelayCommand(AddNewDevice);
        }

        public async void AddNewDevice()
        {
            await NavigateAndCreateAsync<ProvisionDeviceViewModel>(LaunchArgs.ChildId);
        }


        protected override void ItemSelected(DeviceSummary model)
        {
            base.ItemSelected(model);
            var launchArgs = new ViewModelLaunchArgs()
            {
                ViewModelType = typeof(MonitorDeviceViewModel),
                LaunchType = LaunchTypes.View
            };

            launchArgs.Parameters.Add(MonitorDeviceViewModel.DeviceRepoId, LaunchArgs.ChildId);
            launchArgs.Parameters.Add(MonitorDeviceViewModel.DeviceId, model.Id);

            ViewModelNavigation.NavigateAsync(launchArgs);
        }

        protected override string GetListURI()
        {
            return $"/api/devices/{LaunchArgs.ChildId}";
        }

        public RelayCommand AddNewDeviceCommand { get; private set; }
    }
}
