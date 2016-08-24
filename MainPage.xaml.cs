using System;
using System.Collections.ObjectModel;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace QuickReturnHeaderListView
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private ObservableCollection<object> itemsSource;
        public ObservableCollection<object> ItemsSource
        {
            get
            {
                if (itemsSource == null)
                {
                    var items = new ObservableCollection<object>();

                    for (int i = 0; i < 250; i++)
                        items.Add(new Item { Name = "Item " + i });

                    itemsSource = items;
                }

                return itemsSource;
            }
        }

        private void ToggleQuickReturnButton_Click(object sender, RoutedEventArgs e)
        {
            MyHeader.IsQuickReturnEnabled = !MyHeader.IsQuickReturnEnabled;
        }

        private void ToggleIsStickyButton_Click(object sender, RoutedEventArgs e)
        {
            MyHeader.IsSticky = !MyHeader.IsSticky;
        }

        private void ShowHeaderButton_Click(object sender, RoutedEventArgs e)
        {
            MyHeader.Show();
        }
    }

    public class Item
    {
        public string Name { get; set; }
    }
}
