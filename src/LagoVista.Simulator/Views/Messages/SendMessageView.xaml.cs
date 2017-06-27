using LagoVista.XPlat.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public void ShowDynamicArgsClick(object sender, EventArgs args)
        {
            if (AttributeEditor.IsVisible)
            {
                AttributeEditor.IsVisible = false;
            }
            else
            {
                AttributeEditor.IsVisible = true;
            }
        }
    }
}