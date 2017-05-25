using LagoVista.Core.Commanding;
using LagoVista.Simulator.Models;
using System.Threading.Tasks;

namespace LagoVista.Simulator.ViewModels.Simulator
{
    public class SimulatorEditorViewModel : SimulatorViewModelBase<IoT.Simulator.Admin.Models.Simulator>
    {

        public SimulatorEditorViewModel()
        {
            SaveCommand = new RelayCommand(SaveAsync, CanSave);
        }

      

        public async void SaveAsync()
        {
            if(ViewToModel(Form, Model))
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

        public bool CanSave()
        {
            return true;
        }

        public async override Task InitAsync()
        {
            IsBusy = true;

            var form = new EditForm();

            if(this.LaunchArgs.LaunchType == Core.ViewModels.LaunchTypes.Edit)
            {
                var existingSimulator = await RestClient.CreateNewAsync($"/api/simulator/{LaunchArgs.ChildId}");

                foreach (var field in existingSimulator.View)
                {
                    form.FormItems.Add(field.Value);
                    Model = existingSimulator.Model;
                }                

                ModelToView(existingSimulator.Model, form);

                Model = existingSimulator.Model;
                Form = form;
            }
            else
            {
                var newSimulator = await RestClient.CreateNewAsync("/api/simulator/factory");
                if (newSimulator != null)
                {
                    Model = newSimulator.Model;
                    
                    foreach (var field in newSimulator.View)
                    {
                        form.FormItems.Add(field.Value);
                        Model = newSimulator.Model;
                    }
                }

                Form = form;
            }

            
            IsBusy = false;
        }        

        public RelayCommand SaveCommand { get; private set; }
    }
}