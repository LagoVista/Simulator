using LagoVista.Core.Models.UIMetaData;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LagoVista.Simulator.ViewModels.Simulator
{
    public class SimulatorEditorViewModel : SimulatorViewModelBase<IoT.Simulator.Admin.Models.Simulator, IoT.Simulator.Admin.Models.SimulatorSummary>
    {

        public async override Task InitAsync()
        {
            IsBusy = true;
            var newSimulator = await RestClient.CreateNewAsync("/api/simulator/factory");
            Simulator = newSimulator.Model;

            FormItems.Clear();
            foreach(var field in newSimulator.View)
            {
                Debug.WriteLine(field.Value.Label);
                FormItems.Add(field.Value);
            }
            IsBusy = false;
        }

        IoT.Simulator.Admin.Models.Simulator _simulator;
        public IoT.Simulator.Admin.Models.Simulator Simulator
        {
            get { return _simulator; }
            set { Set(ref _simulator, value); }
        }

        ObservableCollection<FormField> _formItems = new ObservableCollection<FormField>();
        public ObservableCollection<FormField> FormItems
        {
            get { return _formItems; }
            set { Set(ref _formItems, value); }
        }


    }
}
