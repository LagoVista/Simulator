using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Simulator.Admin.Models;
using System.Linq;
using LagoVista.Client.Core.Resources;
using LagoVista.Client.Core.ViewModels;

namespace LagoVista.Simulator.Core.ViewModels.Messages
{
    public class DynamicAttributeViewModel : FormViewModelBase<MessageDynamicAttribute>
    {
        public override void SaveAsync()
        {
            base.SaveAsync();
            if (FormAdapter.Validate())
            {
                ViewToModel(FormAdapter, Model);
                if (IsCreate)
                {
                    var parent = LaunchArgs.GetParent<IoT.Simulator.Admin.Models.MessageTemplate>();
                    if (parent.DynamicAttributes.Where(attr => attr.Key == Model.Key).Any())
                    {
                        Popups.ShowAsync(ClientResources.Common_KeyInUse);
                        return;
                    }
                    parent.DynamicAttributes.Add(Model);
                }

                CloseScreen();
            }
        }

        protected override void BuildForm(EditFormAdapter form)
        {
            form.AddViewCell(nameof(Model.Key));
            form.AddViewCell(nameof(Model.ParameterType));
            form.AddViewCell(nameof(Model.DefaultValue));
            form.AddViewCell(nameof(Model.Description));
        }

        protected override string GetRequestUri()
        {
            return "/api/simulator/dyanimaicAttribute/factory";
        }
    }
}