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
                Form = form;
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
                Form = form;
            }

            IsBusy = false;
        }

        private void Form_ItemSelected(object sender, Controls.FormControls.ItemSelectedEventArgs e)
        {
            switch(e.Type)
            {
                case "messageTemplates":
                    var child = Model.MessageTemplates.Where(msg => msg.Id == e.Id).FirstOrDefault();
                    ViewModelNavigation.NavigateAndEditAsync<MessageEditorViewModel>(Model, child);
                    break;
            }
        }

        private void Form_Add(object sender, string e)
        {
            switch (e)
            {
                case "messageTemplates":
                    ViewModelNavigation.NavigateAndCreateAsync<MessageEditorViewModel>(Model);
                    break;
            }

        }
    }
}