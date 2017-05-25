using LagoVista.Core.Commanding;
using LagoVista.Core.Models;
using LagoVista.Simulator.Models;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LagoVista.Simulator.ViewModels.Simulator
{
    public class SimulatorEditorViewModel : SimulatorViewModelBase<IoT.Simulator.Admin.Models.Simulator, IoT.Simulator.Admin.Models.SimulatorSummary>
    {

        public SimulatorEditorViewModel()
        {
            SaveCommand = new RelayCommand(SaveAsync, CanSave);
        }

        public bool ViewToModel()
        {
            var modelProperties = typeof(IoT.Simulator.Admin.Models.Simulator).GetTypeInfo().DeclaredProperties;

            if (!Form.Validate())
            {
                return false;
            }
            
            foreach(var formItem in Form.FormItems)
            {
                var prop = modelProperties.Where(prp => prp.Name.ToLower() == formItem.Name.ToLower()).FirstOrDefault();
                switch(formItem.FieldType)
                {
                    case Controls.FormViewer.CHECKBOX:
                        if (bool.TryParse(formItem.Value, out bool result))
                        {
                            prop.SetValue(base.Model, result);
                        }
                        break;
                    case Controls.FormViewer.PICKER:
                        if(String.IsNullOrEmpty(formItem.Value))
                        {
                            prop.SetValue(base.Model, null);
                        }
                        else
                        {
                            var eh = Activator.CreateInstance(prop.PropertyType) as EntityHeader;
                            eh.Id = formItem.Value;
                            eh.Text = formItem.Options.Where(opt => opt.Key == formItem.Value).First().Label;

                            prop.SetValue(base.Model, eh);
                        }
                        break;
                    case Controls.FormViewer.INTEGER:
                        if(!String.IsNullOrEmpty(formItem.Value))
                        {
                            if(int.TryParse(formItem.Value, out int intValue))
                            {
                                prop.SetValue(base.Model, intValue);
                            }
                        }

                        break;
                    case Controls.FormViewer.DECIMAL:
                        if (!String.IsNullOrEmpty(formItem.Value))
                        {
                            if (double.TryParse(formItem.Value, out double intValue))
                            {
                                prop.SetValue(base.Model, intValue);
                            }
                        }

                        break;
                    case Controls.FormViewer.MULTILINE:
                    case Controls.FormViewer.TEXT:
                    case Controls.FormViewer.KEY:                        
                        prop.SetValue(base.Model, formItem.Value);
                        break;
                }                
            }

            return true;
        }

        public async void SaveAsync()
        {
            if(ViewToModel())
            {
                await RestClient.AddAsync("/api/simulator", this.Model);
                this.ViewModelNavigation.GoBack();
            }            
        }

        public bool CanSave()
        {
            return true;
        }

        EditForm _form;
        public EditForm Form
        {
            get { return _form; }
            set { Set(ref _form, value); }
        }

        public async override Task InitAsync()
        {
            IsBusy = true;
            var newSimulator = await RestClient.CreateNewAsync("/api/simulator/factory");
            Model = newSimulator.Model;

            var form = new EditForm();
            
            foreach(var field in newSimulator.View)
            {
                form.FormItems.Add(field.Value);
            }

            Form = form;
            IsBusy = false;
        }        

        public RelayCommand SaveCommand { get; private set; }
    }
}