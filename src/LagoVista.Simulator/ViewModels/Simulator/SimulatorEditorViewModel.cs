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
                var existingSimulator = await RestClient.CreateNewAsync($"/api/simulator/{LaunchArgs.ChildId}");

                var form = new EditFormAdapter(existingSimulator.Model, ViewModelNavigation);
                foreach (var field in existingSimulator.View)
                {
                    form.FormItems.Add(field.Value);
                    Model = existingSimulator.Model;
                }

                Model = existingSimulator.Model;

                ModelToView(existingSimulator.Model, form);
                form.AddChildList<MessageEditorViewModel>(nameof(Model.MessageTemplates), Model.MessageTemplates);
                FormAdapter = form;
            }
            else
            {
                var newSimulator = await RestClient.CreateNewAsync("/api/simulator/factory");
                var form = new EditFormAdapter(newSimulator.Model, ViewModelNavigation);
                if (newSimulator != null)
                {
                    Model = newSimulator.Model;
                    foreach (var field in newSimulator.View)
                    {
                        form.FormItems.Add(field.Value);
                        Model = newSimulator.Model;
                    }
                }
                FormAdapter = form;
            }

            IsBusy = false;
        }

       
    }
}