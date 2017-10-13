using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LagoVista.XPlat.Core
{
    public class KVPRepeater : StackLayout
    {
        private IEnumerable<KeyValuePair<string, object>> ItemSource
        {
            get { return (IEnumerable<KeyValuePair<string, object>>)base.GetValue(ItemSourceProperty); }
            set
            {
                var oldCollection = ItemSource as ObservableCollection<KeyValuePair<string, object>>;
                if(oldCollection != null)
                {
                    oldCollection.CollectionChanged -= Collection_CollectionChanged;
                }

                var collection = value as ObservableCollection<KeyValuePair<string, object>>;
                if (collection != null)
                {
                    collection.CollectionChanged += Collection_CollectionChanged;
                }

                base.SetValue(ItemSourceProperty, value);
                this.Children.Clear();
                var grid = new Grid();
                for (var idx = 0; idx < value.Count(); ++idx)
                {
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                }

                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(33, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(66, GridUnitType.Star) });

                var rowIdx = 0;
                foreach (var pair in value)
                {
                    var label = new Label();
                    label.FontAttributes = FontAttributes.Bold;
                    var valueLabel = new Label();
                    label.SetValue(Grid.ColumnProperty, 0);
                    valueLabel.SetValue(Grid.ColumnProperty, 1);

                    label.SetValue(Grid.RowProperty, rowIdx);
                    valueLabel.SetValue(Grid.RowProperty, rowIdx);

                    label.Text = pair.Key;
                    valueLabel.Text = pair.Value.ToString();

                    grid.Children.Add(label);
                    grid.Children.Add(valueLabel);

                    ++rowIdx;
                }



                Children.Add(grid);
            }
        }

        public static BindableProperty ItemSourceProperty = BindableProperty.Create(
                                                    propertyName: nameof(ItemSource),
                                                    returnType: typeof(IEnumerable<KeyValuePair<string, object>>),
                                                    declaringType: typeof(FormViewer),
                                                    defaultValue: null,
                                                    defaultBindingMode: BindingMode.Default,
                                                    propertyChanged: HandleFormFieldsAssigned);

        private static void HandleFormFieldsAssigned(BindableObject bindable, object oldValue, object newValue)
        {
            var kvpRepeater = (KVPRepeater)bindable;
            kvpRepeater.ItemSource = newValue as IEnumerable<KeyValuePair<string, object>>;
        }

        private static void Collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
