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
            if(Form.Validate())
            {
                ViewToModel(Form, Model);
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
            var form = new EditForm();

            var newMessageTemplate = await RestClient.CreateNewAsync("/api/simulator/messagetemplate/factory");
            if (this.LaunchArgs.LaunchType == Core.ViewModels.LaunchTypes.Edit)
            {
                Model = this.LaunchArgs.GetChild<MessageTemplate>();                
            }
            else
            {
                var parent = LaunchArgs.GetParent<IoT.Simulator.Admin.Models.Simulator>();
                Model = newMessageTemplate.Model;
                Model.EndPoint = parent.DefaultEndPoint;
                Model.Port = parent.DefaultPort;
                Model.Transport = parent.DefaultTransport;
            }

            foreach (var field in newMessageTemplate.View)
            {
                form.FormItems.Add(field.Value);
            }

            ModelToView(Model, form);

            form.Add += Form_Add;            

            Form = form;
        }

        private void Form_Add(object sender, string e)
        {
        
        }
    }
}