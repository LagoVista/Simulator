using LagoVista.XPlat.Core.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LagoVista.Simulator.Views.Simulator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SimulatorEditorView : LagoVistaContentPage
    {
        public SimulatorEditorView()
        {
            InitializeComponent();
        }
    }
}