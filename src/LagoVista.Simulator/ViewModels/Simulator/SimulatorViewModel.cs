using LagoVista.Core.Commanding;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.ViewModels;
using LagoVista.IoT.Simulator.Admin.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Simulator.ViewModels.Simulator
{
    public class SimulatorViewModel : ViewModelBase
    {
        public bool Validate()
        {
            return true;
        }

        ObservableCollection<FormField> _formItems;
        public ObservableCollection<FormField> FormItems
        {
            get { return _formItems; }
            set { Set(ref _formItems, value); }
        }
    }
}
