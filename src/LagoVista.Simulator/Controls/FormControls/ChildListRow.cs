using FormsPlugin.Iconize;
using LagoVista.Core.Interfaces;
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
        Image _addImage;
        StackLayout _childItemList;

        public event EventHandler<string> Add;

        public event EventHandler<ItemSelectedEventArgs> ItemSelected;

        public ChildListRow(FormViewer formViewer, FormField field) : base(formViewer, field)
        {
            var titleBar = new Grid();
            HorizontalOptions = new LayoutOptions(LayoutAlignment.Fill, true);
            titleBar.BackgroundColor = Color.FromRgb(0xBB, 0xDB, 0xFB);
            titleBar.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
            titleBar.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            titleBar.HorizontalOptions = new LayoutOptions(LayoutAlignment.Fill, true);
            titleBar.HeightRequest = 48;

            _label = new Label();
            _label.FontSize = 24;
            _label.Margin = new Thickness(10, 6, 0, 0);
            _label.TextColor = Color.FromRgb(0x5B, 0x5B, 0x5B);
            _label.Text = field.Label;

            _addImage = new Image();
            _addImage.SetValue(Grid.ColumnProperty, 1);
            _addImage.Margin = new Thickness(0, 12, 28, 0);
            _addImage.WidthRequest = 24;
            _addImage.HeightRequest = 24;

            var tapGenerator = new TapGestureRecognizer();
            tapGenerator.Tapped += Add_Tapped;

            _addImage.GestureRecognizers.Add(tapGenerator);
            _addImage.Source = new FileImageSource() { File = "add.png" };
            
            _childItemList = new StackLayout();

            titleBar.Children.Add(_label);
            titleBar.Children.Add(_addImage);

            Children.Add(titleBar);
            Children.Add(_childItemList);
        }

        private void Add_Tapped(object sender, EventArgs e)
        {
            Add?.Invoke(this, Field.Name);
        }

        private void Item_Tapped(object sender, EventArgs e)
        {
            var childItem = (sender as Grid).BindingContext as EntityHeader;

            ItemSelected?.Invoke(this, new ItemSelectedEventArgs()
            {
                 Id = childItem.Id,
                 Type = Field.Name
            });
        }

        ObservableCollection<IEntityHeader> _childItems;
        public ObservableCollection<IEntityHeader> ChildItems
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
                    foreach (var child in _childItems)
                    {
                        var label = new Label();
                        label.Margin = new Thickness(15,10,10,10);
                        label.FontSize = 24;
                        label.TextColor = Color.FromRgb(0x5B, 0x5B, 0x5B);
                        label.Text = child.Text;

                        var grid = new Grid();
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                        grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
                        grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                        var boxView = new BoxView();
                        boxView.HeightRequest = 1;
                        boxView.Color = Color.SlateGray;

                        var tapGenerator = new TapGestureRecognizer();
                        grid.BindingContext = child;
                        tapGenerator.Tapped += Item_Tapped;

                        var img = new Image();
                        img.Source = new FileImageSource() { File = "chevron_right.png" };
                        img.Margin = new Thickness(2, 10, 30, 0);
                        img.HeightRequest = 24;
                        img.WidthRequest = 24;
                        img.SetValue(Grid.ColumnProperty, 1);
                        
                        boxView.SetValue(Grid.ColumnSpanProperty, 2);
                        boxView.SetValue(Grid.RowProperty, 1);

                        grid.GestureRecognizers.Add(tapGenerator);

                        grid.Children.Add(label);
                        grid.Children.Add(boxView);
                        grid.Children.Add(img);

                        _childItemList.Children.Add(grid);
                    }
                }
            }
        }

        private void _childItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            
        }
        

        public override bool Validate()
        {
            return true;
        }
    }

    public class ItemSelectedEventArgs : EventArgs
    {
        public String Type { get; set; }
        public String Id { get; set; }
    }
}
