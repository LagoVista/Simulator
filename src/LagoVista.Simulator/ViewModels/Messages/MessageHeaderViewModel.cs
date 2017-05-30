using LagoVista.Core.ViewModels;
using LagoVista.IoT.Simulator.Admin.Models;
using LagoVista.Simulator.Models;
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
            var newMessageTemplate = await RestClient.CreateNewAsync("/api/simulator/messageheader/factory");
            Model = (IsEdit) ? this.LaunchArgs.GetChild<MessageHeader>() : newMessageTemplate.Model;
            View = newMessageTemplate.View;
            View[nameof(Model.Key).ToFieldKey()].IsUserEditable = false;

            var form = new EditFormAdapter(Model, newMessageTemplate.View, ViewModelNavigation);
            form.AddViewCell(nameof(Model.Name));
            form.AddViewCell(nameof(Model.Key));
            form.AddViewCell(nameof(Model.Value));
            form.AddViewCell(nameof(Model.Description));
            ModelToView(Model, form);

            FormAdapter = form;
        }
    }
}
