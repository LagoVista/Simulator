using LagoVista.Client.Core.ViewModels;
using LagoVista.IoT.DeviceAdmin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.DeviceManager.Core.ViewModels
{
    public class DeviceTypePickerViewModel : ListViewModelBase<DeviceTypeSummary>
    {
        protected override string GetListURI()
        {
            return $"/api/devicetypes";
        }

        protected async override void ItemSelected(DeviceTypeSummary model)
        {
            LaunchArgs.SelectedAction(model);
            await ViewModelNavigation.GoBackAsync();
        }
    }
}
