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
            if (ViewToModel(Form, Model))
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

            var form = new EditForm();
            form.Add += Form_Add;

            if (this.LaunchArgs.LaunchType == Core.ViewModels.LaunchTypes.Edit)
            {
                var existingSimulator = await RestClient.CreateNewAsync($"/api/simulator/{LaunchArgs.ChildId}");

                foreach (var field in existingSimulator.View)
                {
                    form.FormItems.Add(field.Value);
                    Model = existingSimulator.Model;
                }

                Model = existingSimulator.Model;

                ModelToView(existingSimulator.Model, form);
                //TODO: Need better keying system for child list, very fragile right now.
                form.ChildLists.Add("messageTemplates", (from items in Model.MessageTemplates select new EntityHeader() { Id = items.Id, Text = items.Name }).ToObservableCollection());
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
            }

            Form = form;

            IsBusy = false;
        }

        private void Form_Add(object sender, string e)
        {
            switch (e)
            {
                case "messageTemplates":
                    ViewModelNavigation.NavigateAndCreateAsync<MessageEditorViewModel, IoT.Simulator.Admin.Models.Simulator>(Model);
                    break;
            }

        }
    }
}