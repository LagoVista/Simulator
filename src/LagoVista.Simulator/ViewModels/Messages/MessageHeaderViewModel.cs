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
    public class MessageHeaderViewModel : SimulatorViewModelBase<MessageHeader>
    {
        public override void SaveAsync()
        {
            base.SaveAsync();
            if (FormAdapter.Validate())
            {
                ViewToModel(FormAdapter, Model);
                if (LaunchArgs.LaunchType == LaunchTypes.Create)
                {
                    var parent = LaunchArgs.GetParent<IoT.Simulator.Admin.Models.MessageTemplate>();
                    parent.MessageHeaders.Add(Model);
                }

                CloseScreen();
            }
        }

        public override async Task InitAsync()
        {
            var form = new EditFormAdapter(this, ViewModelNavigation);

            var newMessageTemplate = await RestClient.CreateNewAsync("/api/simulator/messageheader/factory");
            if (this.LaunchArgs.LaunchType == Core.ViewModels.LaunchTypes.Edit)
            {
                Model = this.LaunchArgs.GetChild<MessageHeader>();
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

            FormAdapter = form;
        }

    }
}
