using System;
using System.Diagnostics;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace QuickReturnHeaderListView
{
    public class DebugListView : ListView
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            Debug.WriteLine("Containers: " + ++GetContainerCount);

            return base.GetContainerForItemOverride();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            Debug.WriteLine("Container preparations: " + ++PrepareContainerCount);

            base.PrepareContainerForItemOverride(element, item);
        }

        private int GetContainerCount { get; set; }
        private int PrepareContainerCount { get; set; }
    }
}
