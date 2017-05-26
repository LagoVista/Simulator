using LagoVista.Core.ViewModels;
using LagoVista.IoT.Simulator.Admin.Models;
using LagoVista.Simulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Simulator.ViewModels.Messages
{
    public class DynamicAttributeViewModel : SimulatorViewModelBase<MessageDynamicAttribute>
    {
        public override void SaveAsync()
        {
            base.SaveAsync();
            if (Form.Validate())
            {
                ViewToModel(Form, Model);
                if (LaunchArgs.LaunchType == LaunchTypes.Create)
                {
                    var parent = LaunchArgs.GetParent<IoT.Simulator.Admin.Models.MessageTemplate>();
                    parent.DynamicAttributes.Add(Model);
                }

                CloseScreen();
            }
        }

        public override async Task InitAsync()
        {
            var form = new EditFormAdapter(this, ViewModelNavigation);

            var newMessageTemplate = await RestClient.CreateNewAsync("/api/simulator/dyanimaicAttribute/factory");
            if (this.LaunchArgs.LaunchType == Core.ViewModels.LaunchTypes.Edit)
            {
                Model = this.LaunchArgs.GetChild<MessageDynamicAttribute>();
            }
            else
            {
                Model = newMessageTemplate.Model;
            }

            foreach (var field in newMessageTemplate.View)
            {
                form.FormItems.Add(field.Value);
            }

            ModelToView(Model, form);

            Form = form;
        }
    }
}