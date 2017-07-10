using System.Threading.Tasks;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core;
using LagoVista.Core.ViewModels;
using LagoVista.Simulator.Core.ViewModels.Messages;
using System.Linq;
using LagoVista.Core.Validation;
using System.Diagnostics;
using LagoVista.Simulator.Core.Resources;
using LagoVista.IoT.Simulator.Admin.Models;
using LagoVista.Client.Core.ViewModels;

namespace LagoVista.Simulator.Core.ViewModels.Simulator
{
    public class SimulatorEditorViewModel : IoTAppViewModelBase<IoT.Simulator.Admin.Models.Simulator>
    {

        public SimulatorEditorViewModel()
        {

        }

        public override async void SaveAsync()
        {
            if (ViewToModel(FormAdapter, Model))
            {
                InvokeResult result;
                IsBusy = true;
                if (LaunchArgs.LaunchType == LaunchTypes.Create)
                {
                    result = await RestClient.AddAsync("/api/simulator", this.Model);
                }
                else
                {
                    result = await RestClient.UpdateAsync("/api/simulator", this.Model);
                }

                IsBusy = false;

                if (result.Successful)
                {
                    await this.ViewModelNavigation.GoBackAsync();
                }
                else
                {
                    Debug.WriteLine(result.Errors.First().Message);
                }
            }
        }

        public override bool CanSave()
        {
            return true;
        }

        public override Task InitAsync()
        {
            return PerformNetworkOperation(async () =>
            {
                var uri = this.LaunchArgs.LaunchType == LaunchTypes.Edit ? $"/api/simulator/{LaunchArgs.ChildId}" : "/api/simulator/factory";
                var simulator = await RestClient.GetAsync(uri);               

                var form = new EditFormAdapter(simulator.Model, simulator.View, ViewModelNavigation);
                Model = simulator.Model;
                View = simulator.View;
                View[nameof(Model.Key).ToFieldKey()].IsUserEditable = LaunchArgs.LaunchType == LaunchTypes.Create;
                form.AddViewCell(nameof(Model.Name));
                form.AddViewCell(nameof(Model.Key));
                form.AddViewCell(nameof(Model.DefaultTransport));
                form.AddViewCell(nameof(Model.DefaultEndPoint));
                form.AddViewCell(nameof(Model.DefaultPort));
                form.AddViewCell(nameof(Model.DefaultPayloadType));
                form.AddViewCell(nameof(Model.DeviceId));
                form.AddViewCell(nameof(Model.UserName));
                form.AddViewCell(nameof(Model.HubName));
                form.AddViewCell(nameof(Model.Password));
                form.AddViewCell(nameof(Model.AuthToken));
                form.AddViewCell(nameof(Model.Description));
                form.AddChildList<MessageEditorViewModel>(nameof(Model.MessageTemplates), Model.MessageTemplates);
                ModelToView(Model, form);
                FormAdapter = form;

                switch (Model.DefaultTransport.Value)
                {
                    case TransportTypes.MQTT: SetForMQTT(); break;
                    case TransportTypes.TCP: SetForTCP(); break;
                    case TransportTypes.UDP: SetForUDP(); break;
                    case TransportTypes.RestHttp:
                    case TransportTypes.RestHttps: SetForREST(); break;
                    case TransportTypes.AzureEventHub: SetForAzureEventHub(); break;
                }
            });
        }     
        
        private void SetForAzureEventHub()
        {
            View[nameof(Model.DefaultEndPoint).ToFieldKey()].Label = SimulatorCoreResources.EditSimulator_EventHubName;
            View[nameof(Model.DefaultEndPoint).ToFieldKey()].IsVisible = true;
            View[nameof(Model.AuthToken).ToFieldKey()].IsVisible = true;


            View[nameof(Model.DefaultPort).ToFieldKey()].IsVisible = false;
            View[nameof(Model.UserName).ToFieldKey()].IsVisible = false;
            View[nameof(Model.Password).ToFieldKey()].IsVisible = false;
        }

        private void SetForMQTT()
        {

        }

        private void SetForTCP()
        {

        }

        private void SetForUDP()
        {

        }

        private void SetForREST()
        {
            View[nameof(Model.HubName).ToFieldKey()].IsVisible = false;
            View[nameof(Model.DefaultPort).ToFieldKey()].IsVisible = true;
            View[nameof(Model.UserName).ToFieldKey()].IsVisible = true;
            View[nameof(Model.Password).ToFieldKey()].IsVisible = true;
        }
    }
}