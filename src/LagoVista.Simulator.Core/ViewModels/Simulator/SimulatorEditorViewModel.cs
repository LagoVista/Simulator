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
using System;

namespace LagoVista.Simulator.Core.ViewModels.Simulator
{
    public class SimulatorEditorViewModel : FormViewModelBase<IoT.Simulator.Admin.Models.Simulator>
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
                    result = await FormRestClient.AddAsync("/api/simulator", this.Model);
                }
                else
                {
                    result = await FormRestClient.UpdateAsync("/api/simulator", this.Model);
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

        protected override string GetRequestUri()
        {
            return this.LaunchArgs.LaunchType == LaunchTypes.Edit ? $"/api/simulator/{LaunchArgs.ChildId}" : "/api/simulator/factory";
        }


        protected override void BuildForm(EditFormAdapter form)
        {
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
            if (Model.DefaultTransport != null)
            {
                ShowFieldsForTransport(Model.DefaultTransport.Value);
            }
            else
            {
                HideAll();
            }
        }


        private void ShowFieldsForTransport(TransportTypes transportType)
        {
            switch (transportType)
            {
                case TransportTypes.MQTT: SetForMQTT(); break;
                case TransportTypes.TCP: SetForTCP(); break;
                case TransportTypes.UDP: SetForUDP(); break;
                case TransportTypes.RestHttp:
                case TransportTypes.RestHttps: SetForREST(transportType); break;
                case TransportTypes.AzureEventHub: SetForAzureEventHub(); break;
                default: HideAll(); break;
            }
        }

        protected override void OptionSelected(string name, string value)
        {
            if (name == nameof(Model.DefaultTransport))
            {
                ShowFieldsForTransport((TransportTypes)Enum.Parse(typeof(TransportTypes), value, true));
            }
        }

        private void HideAll()
        {
            HideRow(nameof(Model.HubName));
            HideRow(nameof(Model.DefaultPort));
            HideRow(nameof(Model.DefaultEndPoint));
            HideRow(nameof(Model.UserName));
            HideRow(nameof(Model.Password));
            HideRow(nameof(Model.AuthToken));
        }

        private void SetForAzureEventHub()
        {
            ShowRow(nameof(Model.DefaultEndPoint));
            ShowRow(nameof(Model.AuthToken));
            ShowRow(nameof(Model.HubName));

            HideRow(nameof(Model.DefaultPort).ToFieldKey());
            HideRow(nameof(Model.UserName).ToFieldKey());
            HideRow(nameof(Model.Password).ToFieldKey());
        }

        private void SetForMQTT()
        {
            SetValue(nameof(Model.DefaultPort), 1883.ToString());
            ShowRow(nameof(Model.DefaultEndPoint));
            ShowRow(nameof(Model.DefaultPort));

            ShowRow(nameof(Model.UserName));
            ShowRow(nameof(Model.Password));

            HideRow(nameof(Model.HubName));
            HideRow(nameof(Model.AuthToken));
        }

        private void SetForTCP()
        {
            HideRow(nameof(Model.HubName));
            HideRow(nameof(Model.AuthToken));

            ShowRow(nameof(Model.DefaultEndPoint));
            ShowRow(nameof(Model.DefaultPort));

            ShowRow(nameof(Model.UserName));
            ShowRow(nameof(Model.Password));
        }

        private void SetForUDP()
        {
            HideRow(nameof(Model.HubName));
            HideRow(nameof(Model.AuthToken));

            ShowRow(nameof(Model.DefaultEndPoint));
            ShowRow(nameof(Model.DefaultPort));

            ShowRow(nameof(Model.UserName));
            ShowRow(nameof(Model.Password));
        }

        private void SetForREST(TransportTypes transportType)
        {
            SetValue(nameof(Model.DefaultPort), transportType == TransportTypes.RestHttp ? 80.ToString() : 443.ToString());

            HideRow(nameof(Model.HubName));
            HideRow(nameof(Model.AuthToken));

            ShowRow(nameof(Model.DefaultPort));
            ShowRow(nameof(Model.UserName));
            ShowRow(nameof(Model.Password));
        }
    }
}