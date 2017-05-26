using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Simulator.Controls.FormControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LagoVista.Simulator.Models
{
    public class EditForm
    {
        Func<bool> _validationMethod;

        public event EventHandler<string> Add;
        public event EventHandler<ItemSelectedEventArgs> ItemSelected;

        public void InvokeAdd(string type)
        {
            Add?.Invoke(this, type);
        }

        public void InvokeItemSelected(ItemSelectedEventArgs selectedItem)
        {
            ItemSelected?.Invoke(this, selectedItem);
        }
        
        public EditForm()
        {
            FormItems = new ObservableCollection<FormField>();
            ChildLists = new Dictionary<string, ObservableCollection<EntityHeader>>();
        }

        ObservableCollection<FormField> _formItems;
        public ObservableCollection<FormField> FormItems
        {
            get { return _formItems; }
            set { _formItems = value; }
        }

        public void SetValidationMethod( Func<bool> validationMethod)
        {
            _validationMethod = validationMethod;
        }

        public bool Validate()
        {
            if(_validationMethod == null)
            {
                throw new InvalidOperationException("Must call SetValidationMethod prior to calling validate with a method that will iterate through all the form items and perform validation");
            }
            return _validationMethod.Invoke();
        }

        public Dictionary<string, ObservableCollection<EntityHeader>> ChildLists
        {
            get; set;
        }       
        
        public void AddChildList(string name, List<IIDEntity> items  )
        {

        }
    }
    
}
