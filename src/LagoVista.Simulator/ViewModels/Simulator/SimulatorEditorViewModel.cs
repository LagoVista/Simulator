using LagoVista.Core.Commanding;
using LagoVista.Core.Models.UIMetaData;
using System.Collections.ObjectModel;
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

            var valid = true;
            
            foreach(var formItem in FormItems)
            {
                valid &= formItem.Validate() & valid;

                var prop = modelProperties.Where(prp => prp.Name.ToLower() == formItem.Name.ToLower()).FirstOrDefault();
                switch(formItem.FieldType)
                {
                    case Controls.FormViewer.CHECKBOX:
                        if (bool.TryParse(formItem.Value, out bool result))
                        {
                            prop.SetValue(base.Model, result);
                        }
                        break;

                    case Controls.FormViewer.MULTILINE:
                    case Controls.FormViewer.TEXT:
                    case Controls.FormViewer.KEY:
                        prop.SetValue(base.Model, formItem.Value);
                        break;
                }                
            }

            return valid;
        }

        public async void SaveAsync()
        {
            if(ViewToModel())
            {
                IsBusy = true;
                await RestClient.AddAsync("/api/simulator", this.Model);
                this.ViewModelNavigation.GoBack();
                IsBusy = false;
            }
            
        }

        public bool CanSave()
        {
            return true;
        }

        public async override Task InitAsync()
        {
            IsBusy = true;
            var newSimulator = await RestClient.CreateNewAsync("/api/simulator/factory");
            Model = newSimulator.Model;

            var items  = new ObservableCollection<FormField>();
            
            foreach(var field in newSimulator.View)
            {
                items.Add(field.Value);
            }

            FormItems = items;
            IsBusy = false;
        }        

        ObservableCollection<FormField> _formItems;
        public ObservableCollection<FormField> FormItems
        {
            get { return _formItems; }
            set { Set(ref _formItems, value); }
        }

        public RelayCommand SaveCommand { get; private set; }
    }
}
