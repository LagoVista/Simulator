using LagoVista.Core.ViewModels;
using LagoVista.IoT.Simulator.Admin.Models;
using System.Threading.Tasks;
using LagoVista.Core;
using System.Linq;
using LagoVista.Core.Models.UIMetaData;

namespace LagoVista.Simulator.Core.ViewModels.Messages
{
    public class MessageEditorViewModel : SimulatorViewModelBase<MessageTemplate>
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

        public override async Task InitAsync()
        {
            var newMessageTemplate = await RestClient.CreateNewAsync("/api/simulator/messagetemplate/factory");
            Model = (IsEdit) ? this.LaunchArgs.GetChild<MessageTemplate>() : newMessageTemplate.Model;

            View = newMessageTemplate.View;
            if (IsCreate)
            {

                var parent = LaunchArgs.GetParent<IoT.Simulator.Admin.Models.Simulator>();

                Model.EndPoint = parent.DefaultEndPoint;

                Model.Port = parent.DefaultPort;

                Model.Transport = parent.DefaultTransport;

                View[nameof(Model.TextPayload).ToFieldKey()].IsVisible = false;

                View[nameof(Model.BinaryPayload).ToFieldKey()].IsVisible = false;
            }
            else
            {
                View[nameof(Model.TextPayload).ToFieldKey()].IsVisible = Model.PayloadType.Value == PaylodTypes.String;
                View[nameof(Model.BinaryPayload).ToFieldKey()].IsVisible = Model.PayloadType.Value == PaylodTypes.Binary;
            }

            var form = new EditFormAdapter(Model, newMessageTemplate.View, ViewModelNavigation);
            form.OptionSelected += Form_OptionSelected;
            form.DeleteItem += Form_DeleteItem;
            View[nameof(Model.Key).ToFieldKey()].IsUserEditable = IsCreate;
            form.AddViewCell(nameof(Model.Name));
            form.AddViewCell(nameof(Model.Key));
            form.AddViewCell(nameof(Model.Transport));
            form.AddViewCell(nameof(Model.EndPoint));
            form.AddViewCell(nameof(Model.Port));

            form.AddViewCell(nameof(Model.PayloadType));
            form.AddViewCell(nameof(Model.HttpVerb));
            form.AddViewCell(nameof(Model.TextPayload));
            form.AddViewCell(nameof(Model.BinaryPayload));
            form.AddViewCell(nameof(Model.PathAndQueryString));

            form.AddChildList<MessageHeaderViewModel>(nameof(Model.MessageHeaders), Model.MessageHeaders);
            form.AddChildList<DynamicAttributeViewModel>(nameof(Model.DynamicAttributes), Model.DynamicAttributes);

            ModelToView(Model, form);

            FormAdapter = form;
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

        }

        private void SetForAMQP()
        {

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
                    FormAdapter.ShowView(nameof(Model.HttpVerb));
                    FormAdapter.HideView(nameof(Model.PayloadType));

                }
            }

        }
    }
}