using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace QuickReturnHeaderListView
{
    public class MyListView : ListView
    {
        public ScrollViewer ScrollViewer
        {
            get
            {
                return scrollViewer;
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            scrollViewer = GetScrollViewer(this);
        }

        private static ScrollViewer GetScrollViewer(DependencyObject o)
        {
            if (o is ScrollViewer)
                return o as ScrollViewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);

                var result = GetScrollViewer(child);
                if (result != null)
                    return result;
            }

            return null;
        }

        ScrollViewer scrollViewer = null;
    }
}
