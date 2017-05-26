using LagoVista.Core.Commanding;
using LagoVista.Simulator.Models;
using System.Threading.Tasks;
using System.Linq;
using LagoVista.Core.Models;
using LagoVista.Core;
using LagoVista.Simulator.ViewModels.Messages;

namespace LagoVista.Simulator.ViewModels.Simulator
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
                if (LaunchArgs.LaunchType == Core.ViewModels.LaunchTypes.Create)
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

        public async override Task InitAsync()
        {
            IsBusy = true;
            

            if (this.LaunchArgs.LaunchType == Core.ViewModels.LaunchTypes.Edit)
            {
                var existingSimulator = await RestClient.GetAsync($"/api/simulator/{LaunchArgs.ChildId}");
                Model = existingSimulator.Model;


                existingSimulator.View["key"].IsUserEditable = false;
                var form = new EditFormAdapter(existingSimulator.Model, existingSimulator.View, ViewModelNavigation);
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
                ModelToView(existingSimulator.Model, form);

                FormAdapter = form;
            }
            else
            {
                var newSimulator = await RestClient.CreateNewAsync("/api/simulator/factory");
                var form = new EditFormAdapter(newSimulator.Model, newSimulator.View, ViewModelNavigation);
                Model = newSimulator.Model;
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
                ModelToView(newSimulator.Model, form);
                FormAdapter = form;
            }

            IsBusy = false;
        }
    }
}