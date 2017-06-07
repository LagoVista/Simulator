using System;
using System.Windows.Input;

namespace LagoVista.Client.Core.ViewModels
{
    public class MenuItem
    {
        public String FontIconKey { get; set; }

        public String Name { get; set; }

        public ICommand Command { get; set; }

        public object CommandParameter { get; set; }
    }
}
