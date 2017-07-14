using LagoVista.Core.ViewModels;
using LagoVista.IoT.Simulator.Admin.Models;
using System.Threading.Tasks;
using LagoVista.Core;
using System.Linq;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Client.Core.ViewModels;
using LagoVista.Core.Validation;
using System;

namespace LagoVista.Simulator.Core.ViewModels.Messages
{
    public class MessageEditorViewModel : FormViewModelBase<MessageTemplate>
    {
        public override void SaveAsync()
        {
            base.SaveAsync();
            if (FormAdapter.Validate())
            {
                ViewToModel(FormAdapter, Model);
                if (LaunchArgs.LaunchType == LaunchTypes.Create)
                {
                    var parent = LaunchArgs.GetParent<IoT.Simulator.Admin.Models.Simulator>();
                    parent.MessageTemplates.Add(Model);
                }

                CloseScreen();
            }
        }

        protected override void BuildForm(EditFormAdapter form)
        {
            if (IsCreate)
            {
                var parent = LaunchArgs.GetParent<IoT.Simulator.Admin.Models.Simulator>();
                Model.EndPoint = parent.DefaultEndPoint;
                Model.Port = parent.DefaultPort;
                Model.Transport = parent.DefaultTransport;
                Model.PayloadType = parent.DefaultPayloadType;

                View[nameof(Model.TextPayload).ToFieldKey()].IsVisible = false;
                View[nameof(Model.BinaryPayload).ToFieldKey()].IsVisible = false;
            }

            View[nameof(Model.TextPayload).ToFieldKey()].IsVisible = Model.PayloadType.Value == PaylodTypes.String;
            View[nameof(Model.BinaryPayload).ToFieldKey()].IsVisible = Model.PayloadType.Value == PaylodTypes.Binary;

            form.OptionSelected += Form_OptionSelected;
            form.DeleteItem += Form_DeleteItem;
            View[nameof(Model.Key).ToFieldKey()].IsUserEditable = IsCreate;
            form.AddViewCell(nameof(Model.Name));
            form.AddViewCell(nameof(Model.Key));
            form.AddViewCell(nameof(Model.Transport));
            form.AddViewCell(nameof(Model.EndPoint));
            form.AddViewCell(nameof(Model.Port));

            form.AddViewCell(nameof(Model.Topic));
            form.AddViewCell(nameof(Model.AppendCR));
            form.AddViewCell(nameof(Model.AppendLF));
            form.AddViewCell(nameof(Model.PayloadType));
            form.AddViewCell(nameof(Model.HttpVerb));
            form.AddViewCell(nameof(Model.TextPayload));
            form.AddViewCell(nameof(Model.BinaryPayload));
            form.AddViewCell(nameof(Model.PathAndQueryString));

            if (Model.Transport.Value == TransportTypes.RestHttp ||
                Model.Transport.Value == TransportTypes.RestHttps)
            {
                form.AddChildList<MessageHeaderViewModel>(nameof(Model.MessageHeaders), Model.MessageHeaders);
            }

            form.AddChildList<DynamicAttributeViewModel>(nameof(Model.DynamicAttributes), Model.DynamicAttributes);

            ModelToView(Model, form);


            switch (Model.Transport.Value)
            {
                case TransportTypes.MQTT: SetForMQTT(); break;
                case TransportTypes.TCP: SetForTCP(); break;
                case TransportTypes.UDP: SetForUDP(); break;
                case TransportTypes.RestHttp:
                case TransportTypes.RestHttps: SetForREST(); break;
            }
        }

        protected override string GetRequestUri()
        {
            return "/api/simulator/messagetemplate/factory";
        }
       
        private void Form_DeleteItem(object sender, DeleteItemEventArgs e)
        {
            if(e.Type == nameof(Model.MessageHeaders))
            {
                var hdr = Model.MessageHeaders.Where(itm => itm.Id == e.Id).FirstOrDefault();
                Model.MessageHeaders.Remove(hdr);
            }

            if (e.Type == nameof(Model.DynamicAttributes))
            {
                var attr = Model.DynamicAttributes.Where(itm => itm.Id == e.Id).FirstOrDefault();
                Model.DynamicAttributes.Remove(attr);
            }
        }

        private void SetForMQTT()
        {
            View[nameof(Model.Topic).ToFieldKey()].IsVisible = true;

            View[nameof(Model.HttpVerb).ToFieldKey()].IsVisible = false;
            View[nameof(Model.PathAndQueryString).ToFieldKey()].IsVisible = false;
            View[nameof(Model.Transport).ToFieldKey()].IsVisible = false;
            View[nameof(Model.EndPoint).ToFieldKey()].IsVisible = false;
            View[nameof(Model.Port).ToFieldKey()].IsVisible = false;
            View[nameof(Model.AppendCR).ToFieldKey()].IsVisible = false;
            View[nameof(Model.AppendLF).ToFieldKey()].IsVisible = false;
        }

        private void SetForREST()
        {
            View[nameof(Model.HttpVerb).ToFieldKey()].IsVisible = true;
            View[nameof(Model.PathAndQueryString).ToFieldKey()].IsVisible = true;
            View[nameof(Model.PayloadType).ToFieldKey()].IsVisible = false;


            View[nameof(Model.Topic).ToFieldKey()].IsVisible = false;
            View[nameof(Model.Topic).ToFieldKey()].IsVisible = false;
            View[nameof(Model.AppendCR).ToFieldKey()].IsVisible = false;
            View[nameof(Model.AppendLF).ToFieldKey()].IsVisible = false;
        }

        private void SetForTCP()
        {
            View[nameof(Model.Transport).ToFieldKey()].IsVisible = false;
            View[nameof(Model.EndPoint).ToFieldKey()].IsVisible = false;
            View[nameof(Model.Port).ToFieldKey()].IsVisible = false;
            View[nameof(Model.Topic).ToFieldKey()].IsVisible = false;
            View[nameof(Model.PathAndQueryString).ToFieldKey()].IsVisible = false;

            View[nameof(Model.PayloadType).ToFieldKey()].IsVisible = false;

            View[nameof(Model.AppendCR).ToFieldKey()].IsVisible = true;
            View[nameof(Model.AppendLF).ToFieldKey()].IsVisible = true;
        }

        private void SetForUDP()
        {
            View[nameof(Model.Transport).ToFieldKey()].IsVisible = false;
            View[nameof(Model.EndPoint).ToFieldKey()].IsVisible = false;
            View[nameof(Model.Port).ToFieldKey()].IsVisible = false;
            View[nameof(Model.Topic).ToFieldKey()].IsVisible = false;
            View[nameof(Model.PathAndQueryString).ToFieldKey()].IsVisible = false;

            View[nameof(Model.PayloadType).ToFieldKey()].IsVisible = false;

            View[nameof(Model.AppendCR).ToFieldKey()].IsVisible = true;
            View[nameof(Model.AppendLF).ToFieldKey()].IsVisible = true;

        }

        private void Form_OptionSelected(object sender, OptionSelectedEventArgs e)
        {
            if (e.Key == nameof(Model.PayloadType))
            {
                if (e.Value == MessageTemplate.PayloadTypes_Binary)
                {
                    FormAdapter.HideView(nameof(Model.TextPayload));
                    FormAdapter.ShowView(nameof(Model.BinaryPayload));
                }
                else if (e.Value == MessageTemplate.PayloadTypes_Text)
                {
                    FormAdapter.ShowView(nameof(Model.TextPayload));
                    FormAdapter.HideView(nameof(Model.BinaryPayload));
                }
                else
                {
                    FormAdapter.HideView(nameof(Model.TextPayload));
                    FormAdapter.HideView(nameof(Model.BinaryPayload));
                }
            }

            if (e.Key == nameof(Model.Transport))
            {
                if (e.Value == LagoVista.IoT.Simulator.Admin.Models.Simulator.Transport_RestHttp)
                {
                    SetForREST();
                }
            }
        }
    }
}