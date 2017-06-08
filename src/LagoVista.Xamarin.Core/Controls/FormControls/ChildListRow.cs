using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace LagoVista.XPlat.Core.Controls.FormControls
{
    public class ChildListRow : FormControl
    {
        Label _label;
        IconButton _addImage;
        StackLayout _childItemList;

        public event EventHandler<string> Add;

        public event EventHandler<ItemSelectedEventArgs> ItemSelected;

        public ChildListRow(FormViewer formViewer, FormField field) : base(formViewer, field)
        {
            var titleBar = new Grid();
            HorizontalOptions = new LayoutOptions(LayoutAlignment.Fill, true);
            titleBar.BackgroundColor = AppStyle.MenuBarBackground.ToXamFormsColor();
            titleBar.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
            titleBar.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            titleBar.HorizontalOptions = new LayoutOptions(LayoutAlignment.Fill, true);
            titleBar.HeightRequest = 48;

            _label = new Label();
            _label.FontSize = 20;
            _label.Margin = new Thickness(10, 0, 0, 0);
            _label.VerticalOptions = new LayoutOptions(LayoutAlignment.Center, false);
            _label.TextColor = AppStyle.MenuBarForeground.ToXamFormsColor();
            _label.Text = field.Label;

            _addImage = new IconButton();
            _addImage.SetValue(Grid.ColumnProperty, 1);
            _addImage.Margin = new Thickness(0, 0, 20, 0);
            _addImage.VerticalOptions = new LayoutOptions(LayoutAlignment.Center, false);
            _addImage.IconKey = "fa-plus";
            _addImage.WidthRequest = 48;
            _addImage.HeightRequest = 48;
            _addImage.FontSize = 22;
            _addImage.Clicked += _addImage_Clicked;

            _childItemList = new StackLayout();

            titleBar.Children.Add(_label);
            titleBar.Children.Add(_addImage);

            Children.Add(titleBar);
            Children.Add(_childItemList);
        }

        private void _addImage_Clicked(object sender, EventArgs e)
        {
            Add?.Invoke(this, Field.Name);
        }        

        private void Item_Tapped(object sender, EventArgs e)
        {
            var childItem = (sender as Grid).BindingContext as IEntityHeaderEntity;

            ItemSelected?.Invoke(this, new ItemSelectedEventArgs()
            {
                Id = childItem.ToEntityHeader().Id,
                Type = Field.Name
            });
        }

        public override void Refresh()
        {
            _childItemList.Children.Clear();

            if (_childItems != null)
            {
                foreach (var child in _childItems)
                {
                    var label = new Label();
                    label.Margin = new Thickness(15, 10, 10, 10);
                    label.FontSize = 24;
                    label.TextColor = Color.FromRgb(0x5B, 0x5B, 0x5B);
                    label.Text = child.ToEntityHeader().Text;

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

        IEnumerable<IEntityHeaderEntity> _childItems;
        public IEnumerable<IEntityHeaderEntity> ChildItems
        {
            get { return _childItems; }
            set
            {
                _childItems = value;
                Refresh();
            }
        }


        public override bool Validate()
        {
            return true;
        }
    }
}
