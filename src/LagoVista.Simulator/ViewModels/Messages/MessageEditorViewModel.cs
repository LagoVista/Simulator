using LagoVista.Core.ViewModels;
using LagoVista.IoT.Simulator.Admin.Models;
using LagoVista.Simulator.Models;
using System.Threading.Tasks;

namespace LagoVista.Simulator.ViewModels.Messages
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
            FormAdapter = null;

            var newMessageTemplate = await RestClient.CreateNewAsync("/api/simulator/messagetemplate/factory");
            if (this.LaunchArgs.LaunchType == Core.ViewModels.LaunchTypes.Edit)
            {
                Model = this.LaunchArgs.GetChild<MessageTemplate>();
                var form = new EditFormAdapter(Model, ViewModelNavigation);


                foreach (var field in newMessageTemplate.View)
                {
                    form.FormItems.Add(field.Value);
                }

                form.AddChildList<MessageHeaderViewModel>(nameof(Model.MessageHeaders), Model.MessageHeaders);
                form.AddChildList<DynamicAttributeViewModel>(nameof(Model.DynamicAttributes), Model.DynamicAttributes);

                ModelToView(Model, form);

                FormAdapter = form;
            }
            else
            {
                var parent = LaunchArgs.GetParent<IoT.Simulator.Admin.Models.Simulator>();
                Model = newMessageTemplate.Model;

                var form = new EditFormAdapter(Model, ViewModelNavigation);
                
                Model.EndPoint = parent.DefaultEndPoint;
                Model.Port = parent.DefaultPort;
                Model.Transport = parent.DefaultTransport;

                foreach (var field in newMessageTemplate.View)
                {
                    form.FormItems.Add(field.Value);
                }

                form.AddChildList<MessageHeaderViewModel>(nameof(Model.MessageHeaders), Model.MessageHeaders);
                form.AddChildList<DynamicAttributeViewModel>(nameof(Model.DynamicAttributes), Model.DynamicAttributes);

                ModelToView(Model, form);

                FormAdapter = form;
            }
        }
    }
}