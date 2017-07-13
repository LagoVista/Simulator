using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.ViewModels;
using LagoVista.IoT.Simulator.Admin.Models;
using System.Linq;
using LagoVista.Client.Core.Resources;
using LagoVista.Client.Core.ViewModels;

namespace LagoVista.Simulator.Core.ViewModels.Messages
{
    public class MessageHeaderViewModel : FormViewModelBase<MessageHeader>
    {
        public override void SaveAsync()
        {
            base.SaveAsync();
            if (FormAdapter.Validate())
            {
                ViewToModel(FormAdapter, Model);
                if (LaunchArgs.LaunchType == LaunchTypes.Create)
                {
                    var parent = LaunchArgs.GetParent<MessageTemplate>();
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

        protected override string GetRequestUri()
        {
            return "/api/simulator/messageheader/factory";
        }

        protected override void BuildForm(EditFormAdapter form)
        {
            form.AddViewCell(nameof(Model.Name));
            form.AddViewCell(nameof(Model.Key));
            form.AddViewCell(nameof(Model.HeaderName));
            form.AddViewCell(nameof(Model.Value));
            form.AddViewCell(nameof(Model.Description));
        }
    }
}