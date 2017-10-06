using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core;
using LagoVista.Core.ViewModels;
using LagoVista.Simulator.Core.ViewModels.Messages;
using LagoVista.Core.Validation;
using LagoVista.IoT.Simulator.Admin.Models;
using LagoVista.Client.Core.ViewModels;
using System;
using System.Threading.Tasks;

namespace LagoVista.Simulator.Core.ViewModels.Simulator
{
    public class SimulatorEditorViewModel : FormViewModelBase<IoT.Simulator.Admin.Models.Simulator>
    {

        public SimulatorEditorViewModel()
        {

        }

        public override Task<InvokeResult> SaveRecordAsync()
        {
            return PerformNetworkOperation(() =>
            {
                if (LaunchArgs.LaunchType == LaunchTypes.Create)
                {
                    return FormRestClient.AddAsync("/api/simulator", this.Model);
                }
                else
                {
                    return FormRestClient.UpdateAsync("/api/simulator", this.Model);
                }
            });
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
            View[nameof(Model.DefaultTransport).ToFieldKey()].IsEnabled = LaunchArgs.LaunchType == LaunchTypes.Create;
            form.AddViewCell(nameof(Model.Name));
            form.AddViewCell(nameof(Model.Key));
            form.AddViewCell(nameof(Model.DefaultTransport));
            form.AddViewCell(nameof(Model.DefaultEndPoint));
//            form.AddViewCell(nameof(Model.TLSSSL));
            form.AddViewCell(nameof(Model.DefaultPort));
            form.AddViewCell(nameof(Model.DeviceId));
            form.AddViewCell(nameof(Model.UserName));
            form.AddViewCell(nameof(Model.Password));
            form.AddViewCell(nameof(Model.AccessKeyName));
            form.AddViewCell(nameof(Model.AccessKey));
            form.AddViewCell(nameof(Model.HubName));
            form.AddViewCell(nameof(Model.QueueName));
            form.AddViewCell(nameof(Model.Subscription));            
            form.AddViewCell(nameof(Model.DefaultPayloadType));
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
            HideAll();
            switch (transportType)
            {
                case TransportTypes.MQTT: SetForMQTT(); break;
                case TransportTypes.TCP: SetForTCP(); break;
                case TransportTypes.UDP: SetForUDP(); break;
                case TransportTypes.RestHttp:
                case TransportTypes.RestHttps: SetForREST(transportType); break;
                case TransportTypes.AzureEventHub: SetForAzureEventHub(); break;
                case TransportTypes.AzureIoTHub: SetForIoTHub(); break;
                case TransportTypes.AzureServiceBus: SetForAzureServiceBus(); break;
                default: HideAll(); break;
            }
        }

        protected override string GetHelpLink()
        {
            return "http://support.nuviot.com";
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
            HideRow(nameof(Model.DefaultPayloadType));
            HideRow(nameof(Model.HubName));
            HideRow(nameof(Model.DefaultPort));
            //HideRow(nameof(Model.TLSSSL));
            HideRow(nameof(Model.DefaultEndPoint));
            HideRow(nameof(Model.UserName));
            HideRow(nameof(Model.Password));
            HideRow(nameof(Model.QueueName));
            HideRow(nameof(Model.AccessKeyName));
            HideRow(nameof(Model.AccessKey));
            HideRow(nameof(Model.Subscription));
        }

        private void SetForAzureServiceBus()
        {
            ShowRow(nameof(Model.DefaultPayloadType));
            ShowRow(nameof(Model.DefaultEndPoint));
            ShowRow(nameof(Model.AccessKeyName));
            ShowRow(nameof(Model.AccessKey));
            ShowRow(nameof(Model.HubName));
            ShowRow(nameof(Model.QueueName));
        }

        private void SetForAzureEventHub()
        {
            ShowRow(nameof(Model.DefaultPayloadType));
            ShowRow(nameof(Model.DefaultEndPoint));
            ShowRow(nameof(Model.AccessKeyName));
            ShowRow(nameof(Model.AccessKey));
            ShowRow(nameof(Model.HubName));
        }


        private void SetForIoTHub()
        {
            ShowRow(nameof(Model.DefaultEndPoint));
            ShowRow(nameof(Model.AccessKey));            
            ShowRow(nameof(Model.DefaultPayloadType));
        }

        private void SetForMQTT()
        {
            SetValue(nameof(Model.DefaultPort), 1883.ToString());

            ShowRow(nameof(Model.DefaultPort));
            ShowRow(nameof(Model.DefaultEndPoint));
            ShowRow(nameof(Model.DefaultPayloadType));
            ShowRow(nameof(Model.Subscription));
            ShowRow(nameof(Model.UserName));
            ShowRow(nameof(Model.Password));
        }

        private void SetForTCP()
        {
            ShowRow(nameof(Model.DefaultPayloadType));
            ShowRow(nameof(Model.DefaultEndPoint));
            ShowRow(nameof(Model.DefaultPort));
        }

        private void SetForUDP()
        {
            ShowRow(nameof(Model.DefaultPayloadType));
            ShowRow(nameof(Model.DefaultEndPoint));
            ShowRow(nameof(Model.DefaultPort));
        }

        private void SetForREST(TransportTypes transportType)
        {
            SetValue(nameof(Model.DefaultPort), transportType == TransportTypes.RestHttp ? 80.ToString() : 443.ToString());

            ShowRow(nameof(Model.DefaultEndPoint));
            ShowRow(nameof(Model.DefaultPort));
            ShowRow(nameof(Model.UserName));
            ShowRow(nameof(Model.Password));            
        }
    }
}