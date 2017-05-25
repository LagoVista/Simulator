using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LagoVista.Simulator.Models
{
    public class EditForm
    {
        Func<bool> _validationMethod;

        public EditForm()
        {
            FormItems = new ObservableCollection<FormField>();
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
    }
}
