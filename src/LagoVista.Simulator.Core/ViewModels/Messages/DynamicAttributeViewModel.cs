using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Simulator.Admin.Models;
using System.Linq;
using System.Threading.Tasks;
using LagoVista.Client.Core.Resources;

namespace LagoVista.Simulator.Core.ViewModels.Messages
{
    public class DynamicAttributeViewModel : SimulatorViewModelBase<MessageDynamicAttribute>
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
                    if(parent.DynamicAttributes.Where(attr=>attr.Key == Model.Key).Any())
                    {
                        Popups.ShowAsync(ClientResources.Common_KeyInUse);                        
                        return;
                    }
                    parent.DynamicAttributes.Add(Model);
                }

                CloseScreen();
            }
        }

        public override Task InitAsync()
        {
            return PerformNetworkOperation(async () =>
            {
                var attribute = await RestClient.CreateNewAsync("/api/simulator/dyanimaicAttribute/factory");

                Model = IsEdit ? this.LaunchArgs.GetChild<MessageDynamicAttribute>() : attribute.Model;
                View = attribute.View;

                var form = new EditFormAdapter(Model, attribute.View, ViewModelNavigation);                
                form.AddViewCell(nameof(Model.Name));
                form.AddViewCell(nameof(Model.Key));
                form.AddViewCell(nameof(Model.ParameterType));
                form.AddViewCell(nameof(Model.DefaultValue));
                form.AddViewCell(nameof(Model.Description));
                ModelToView(Model, form);

                FormAdapter = form;
            });
        }
    }
}