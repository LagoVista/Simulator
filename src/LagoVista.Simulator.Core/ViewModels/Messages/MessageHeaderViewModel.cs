using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.ViewModels;
using LagoVista.IoT.Simulator.Admin.Models;
using System.Threading.Tasks;
using LagoVista.Core;
using System.Linq;
using LagoVista.Client.Core.Resources;
using LagoVista.Client.Core.ViewModels;

namespace LagoVista.Simulator.Core.ViewModels.Messages
{
    public class MessageHeaderViewModel : IoTAppViewModelBase<MessageHeader>
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
                    if (parent.DynamicAttributes.Where(attr => attr.Key == Model.Key).Any())
                    {
                        Popups.ShowAsync(ClientResources.Common_KeyInUse);
                        return;
                    }
                    parent.MessageHeaders.Add(Model);
                }

                CloseScreen();
            }
        }

        public override  Task InitAsync()
        {
            return PerformNetworkOperation(async () =>
            {
                var newMessageTemplate = await RestClient.CreateNewAsync("/api/simulator/messageheader/factory");
                Model = (IsEdit) ? this.LaunchArgs.GetChild<MessageHeader>() : newMessageTemplate.Model;
                View = newMessageTemplate.View;
                View[nameof(Model.Key).ToFieldKey()].IsUserEditable = IsCreate;

                var form = new EditFormAdapter(Model, newMessageTemplate.View, ViewModelNavigation);
                form.AddViewCell(nameof(Model.Name));
                form.AddViewCell(nameof(Model.Key));
                form.AddViewCell(nameof(Model.HeaderName));
                form.AddViewCell(nameof(Model.Value));
                form.AddViewCell(nameof(Model.Description));
                ModelToView(Model, form);

                FormAdapter = form;
            });
        }
    }
}