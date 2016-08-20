using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace QuickReturnHeaderListView
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            Loaded += (sender, args) =>
            {
                List.ScrollViewer.ViewChanged += (s, a) =>
                {
                    float oldOffsetY = 0.0f;
                    animationProperties.TryGetScalar("OffsetY", out oldOffsetY);

                    var delta = List.ScrollViewer.VerticalOffset - previousVerticalScrollOffset;
                    previousVerticalScrollOffset = List.ScrollViewer.VerticalOffset;

                    var newOffsetY = oldOffsetY - (float)delta;

                    // Keep values within - header size, 0
                    FrameworkElement header = (FrameworkElement)List.Header;
                    newOffsetY = Math.Max((float)-header.ActualHeight, newOffsetY);
                    newOffsetY = Math.Min(0, newOffsetY);

                    if (oldOffsetY != newOffsetY)
                        animationProperties.InsertScalar("OffsetY", newOffsetY);
                };
            };


            SizeChanged += (sender, args) =>
            {
                FrameworkElement header = (FrameworkElement)List.Header;

                if (scrollProperties == null)
                    scrollProperties = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(List.ScrollViewer);

                var compositor = scrollProperties.Compositor;

                if (animationProperties == null)
                {
                    animationProperties = compositor.CreatePropertySet();
                    animationProperties.InsertScalar("OffsetY", 0.0f);
                }

                var expressionAnimation = compositor.CreateExpressionAnimation("animationProperties.OffsetY - ScrollingProperties.Translation.Y");

                expressionAnimation.SetReferenceParameter("ScrollingProperties", scrollProperties);
                expressionAnimation.SetReferenceParameter("animationProperties", animationProperties);

                var headerVisual = ElementCompositionPreview.GetElementVisual((UIElement)List.Header);
                headerVisual.StartAnimation("Offset.Y", expressionAnimation);
            };
        }

        private ObservableCollection<object> itemsSource;
        public ObservableCollection<object> ItemsSource
        {
            get
            {
                if (itemsSource == null)
                {
                    var items = new ObservableCollection<object>();

                    for (int i = 0; i < 500; i++)
                        items.Add(new Item { Name = "Item " + i });

                    itemsSource = items;
                }

                return itemsSource;
            }
        }

        private double previousVerticalScrollOffset = 0.0;
        private CompositionPropertySet scrollProperties;
        private CompositionPropertySet animationProperties;
    }

    public class Item
    {
        public string Name { get; set; }
    }
}
