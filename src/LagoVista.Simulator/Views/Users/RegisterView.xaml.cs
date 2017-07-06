using LagoVista.XPlat.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LagoVista.Simulator.Views.Users
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterView : LagoVistaContentPage
    {
        public RegisterView()
        {
            InitializeComponent();
            BindingContext = new RegisterViewViewModel();
        }
    }

    class RegisterViewViewModel : INotifyPropertyChanged
    {

        public RegisterViewViewModel()
        {
            IncreaseCountCommand = new Command(IncreaseCount);
        }

        int count;

        string countDisplay = "You clicked 0 times.";
        public string CountDisplay
        {
            get { return countDisplay; }
            set { countDisplay = value; OnPropertyChanged(); }
        }

        public ICommand IncreaseCountCommand { get; }

        void IncreaseCount() =>
            CountDisplay = $"You clicked {++count} times";


        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}