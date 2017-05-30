using LagoVista.Core.ViewModels;
using LagoVista.IoT.Simulator.Admin.Models;
using LagoVista.Simulator.Models;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Simulator.ViewModels.Messages
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
                        Popups.ShowAsync(Resources.SimulatorResources.Common_KeyInUse);                        
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
                View[nameof(Model.Key).ToFieldKey()].IsUserEditable = false;

                var form = new EditFormAdapter(Model, attribute.View, ViewModelNavigation);
                form.AddViewCell(nameof(Model.Name));
                form.AddViewCell(nameof(Model.Key));
                form.AddViewCell(nameof(Model.ParameterType));
                form.AddViewCell(nameof(Model.DefaultValue));
                form.AddViewCell(nameof(Model.Description));

                View[nameof(Model.Key)].IsUserEditable = this.LaunchArgs.LaunchType == LaunchTypes.Create;

                ModelToView(Model, form);

                FormAdapter = form;
            });
        }
    }
}