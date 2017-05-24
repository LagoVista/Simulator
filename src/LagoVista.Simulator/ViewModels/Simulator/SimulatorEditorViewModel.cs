using LagoVista.Core.Commanding;
using LagoVista.Core.Models.UIMetaData;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LagoVista.Simulator.ViewModels.Simulator
{
    public class SimulatorEditorViewModel : SimulatorViewModelBase<IoT.Simulator.Admin.Models.Simulator, IoT.Simulator.Admin.Models.SimulatorSummary>
    {
        public SimulatorEditorViewModel()
        {
            SaveCommand = new RelayCommand(Save, CanSave);

            
        }

        public void Save()
        {
            this.ViewModelNavigation.GoBack();
        }

        public bool CanSave()
        {
            return true;
        }

        public async override Task InitAsync()
        {
            IsBusy = true;
            var newSimulator = await RestClient.CreateNewAsync("/api/simulator/factory");
            Simulator = newSimulator.Model;

            var items  = new ObservableCollection<FormField>();
            
            foreach(var field in newSimulator.View)
            {
                items.Add(field.Value);
            }

            FormItems = items;
            IsBusy = false;
        }

        IoT.Simulator.Admin.Models.Simulator _simulator;
        public IoT.Simulator.Admin.Models.Simulator Simulator
        {
            get { return _simulator; }
            set { Set(ref _simulator, value); }
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
