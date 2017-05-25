using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace LagoVista.Simulator.Controls.FormControls
{
    public class ChildListRow : FormControl
    {
        Label _label;
        Button _addButton;
        StackLayout _childItemList;

        public event EventHandler<string> Add;

        public ChildListRow(FormViewer formViewer, FormField field) : base(formViewer, field)
        {

            var titleBar = new Grid();
            HorizontalOptions = new LayoutOptions(LayoutAlignment.Fill, true);
            titleBar.BackgroundColor = Color.SlateGray;
            titleBar.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
            titleBar.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            _label = new Label();
            _label.TextColor = Color.White;
            _label.Text = field.Label;

            Orientation = StackOrientation.Horizontal;

            _addButton = new Button();
            _addButton.SetValue(Grid.ColumnProperty, 1);
            _addButton.HeightRequest = 42;
            _addButton.WidthRequest = 42;
            _addButton.BackgroundColor = Color.Transparent;
            _addButton.Image = new FileImageSource() { File = "add.png" };
            _addButton.Clicked += _addButton_Clicked;

            _childItemList = new StackLayout();

            titleBar.Children.Add(_label);
            titleBar.Children.Add(_addButton);

            Children.Add(titleBar);
            Children.Add(_childItemList);
        }

        ObservableCollection<EntityHeader> _childItems;
        public ObservableCollection<EntityHeader> ChildItems
        {
            get { return _childItems; }
            set
            {
                if (_childItems != null)
                {
                    _childItems.CollectionChanged -= _childItems_CollectionChanged;
                }
                _childItems = value;

                _childItemList.Children.Clear();

                if (_childItems != null)
                {
                    _childItems.CollectionChanged += _childItems_CollectionChanged;
                    foreach(var child in _childItems)
                    {
                        var label = new Label();
                        label.Text = child.Text;
                        _childItemList.Children.Add(label);
                    }
                }
            }
        }

        private void _childItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            
        }

        private void _addButton_Clicked(object sender, EventArgs e)
        {
            Add?.Invoke(this, Field.Name);
        }

        public override bool Validate()
        {
            return true;
        }
    }
}
