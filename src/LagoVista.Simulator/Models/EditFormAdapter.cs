using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.ViewModels;
using LagoVista.Simulator.Controls.FormControls;
using System;
using System.Linq;
using LagoVista.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LagoVista.Simulator.ViewModels;

namespace LagoVista.Simulator.Models
{
    public class EditFormAdapter
    {
        Func<bool> _validationMethod;
        IViewModelNavigation _navigationService;

        Dictionary<string, Type> _editorTypes;
        Dictionary<string, IEnumerable<IEntityHeaderEntity>> _entityLists;

        object _parent;


        public EditFormAdapter(object parent, IViewModelNavigation navigationService)
        {
            _entityLists = new Dictionary<string, IEnumerable<IEntityHeaderEntity>>();
            _editorTypes = new Dictionary<string, Type>();

            _parent = parent;
            _navigationService = navigationService;
            FormItems = new ObservableCollection<FormField>();
            ChildLists = new Dictionary<string, ObservableCollection<IEntityHeader>>();
        }


        public void InvokeAdd(string type)
        {
            _navigationService.NavigateAsync(new ViewModelLaunchArgs()
            {
                Parent = _parent,
                ViewModelType = _editorTypes[type],
                LaunchType = LaunchTypes.Create
            });
        }

        public void InvokeItemSelected(ItemSelectedEventArgs selectedItem)
        {
            _navigationService.NavigateAsync(new ViewModelLaunchArgs()
            {
                Parent = _parent,
                Child = _entityLists[selectedItem.Type].Where(itm => itm.ToEntityHeader().Id == selectedItem.Id).First(),
                ViewModelType = _editorTypes[selectedItem.Type],
                LaunchType = LaunchTypes.Edit
            });
        }


        ObservableCollection<FormField> _formItems;
        public ObservableCollection<FormField> FormItems
        {
            get { return _formItems; }
            set { _formItems = value; }
        }

        public void SetValidationMethod(Func<bool> validationMethod)
        {
            _validationMethod = validationMethod;
        }

        public bool Validate()
        {
            if (_validationMethod == null)
            {
                throw new InvalidOperationException("Must call SetValidationMethod prior to calling validate with a method that will iterate through all the form items and perform validation");
            }
            return _validationMethod.Invoke();
        }

        public Dictionary<string, ObservableCollection<IEntityHeader>> ChildLists
        {
            get; set;
        }

        public void AddChildList<TEditorType>(string name, IEnumerable<IEntityHeaderEntity> items) where TEditorType : ViewModelBase
        {
            var propertyName = $"{name.Substring(0, 1).ToLower()}{name.Substring(1)}";

            ChildLists.Add(propertyName, (from item in items select item.ToEntityHeader()).ToObservableCollection());
            _entityLists.Add(propertyName, items);
            _editorTypes.Add(propertyName, typeof(TEditorType));

        }
    }
}
