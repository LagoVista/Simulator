using LagoVista.XPlat.Core.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LagoVista.Simulator.Views.Messages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SendMessageView : LagoVistaContentPage
    {
        public SendMessageView()
        {
            InitializeComponent();
        }
    }
}