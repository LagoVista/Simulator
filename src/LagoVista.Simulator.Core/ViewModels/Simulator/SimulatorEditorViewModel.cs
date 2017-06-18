using System.Threading.Tasks;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core;
using LagoVista.Core.ViewModels;
using LagoVista.Simulator.Core.ViewModels.Messages;
using System.Linq;

namespace LagoVista.Simulator.Core.ViewModels.Simulator
{
    public class SimulatorEditorViewModel : SimulatorViewModelBase<IoT.Simulator.Admin.Models.Simulator>
    {

        public SimulatorEditorViewModel()
        {

        }

        public override async void SaveAsync()
        {
            if (ViewToModel(FormAdapter, Model))
            {
                IsBusy = true;
                if (LaunchArgs.LaunchType == LaunchTypes.Create)
                {
                    await RestClient.AddAsync("/api/simulator", this.Model);
                }
                else
                {
                    await RestClient.UpdateAsync("/api/simulator", this.Model);
                }
                IsBusy = false;

                await this.ViewModelNavigation.GoBackAsync();
            }
        }

        public override bool CanSave()
        {
            return true;
        }

        public override Task InitAsync()
        {
            return PerformNetworkOperation(async () =>
            {
                var uri = this.LaunchArgs.LaunchType == LaunchTypes.Edit ? $"/api/simulator/{LaunchArgs.ChildId}" : "/api/simulator/factory";

                var simulator = await RestClient.GetAsync(uri);               

                var form = new EditFormAdapter(simulator.Model, simulator.View, ViewModelNavigation);
                Model = simulator.Model;
                View = simulator.View;
                View[nameof(Model.Key).ToFieldKey()].IsUserEditable = LaunchArgs.LaunchType == LaunchTypes.Create;
                form.AddViewCell(nameof(Model.Name));
                form.AddViewCell(nameof(Model.Key));
                form.AddViewCell(nameof(Model.DefaultTransport));
                form.AddViewCell(nameof(Model.DefaultEndPoint));
                form.AddViewCell(nameof(Model.DefaultPort));
                form.AddViewCell(nameof(Model.DeviceId));
                form.AddViewCell(nameof(Model.UserName));
                form.AddViewCell(nameof(Model.Password));
                form.AddViewCell(nameof(Model.AuthToken));
                form.AddViewCell(nameof(Model.Description));
                form.AddChildList<MessageEditorViewModel>(nameof(Model.MessageTemplates), Model.MessageTemplates);
                ModelToView(Model, form);
                FormAdapter = form;
            });
        }       
    }
}