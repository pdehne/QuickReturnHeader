using System;
using System.Diagnostics;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace QuickReturnHeaderListView
{
    public class QuickReturnListView : ListView
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

            if(scrollViewer != null)
            {
                scrollViewer.ViewChanged += (sender, args) =>
                {
                    float oldOffsetY = 0.0f;
                    animationProperties.TryGetScalar("OffsetY", out oldOffsetY);

                    var delta = scrollViewer.VerticalOffset - previousVerticalScrollOffset;
                    previousVerticalScrollOffset = scrollViewer.VerticalOffset;

                    var newOffsetY = oldOffsetY - (float)delta;

                    // Keep values within negativ header size and 0
                    FrameworkElement header = (FrameworkElement)Header;
                    newOffsetY = Math.Max((float)-header.ActualHeight, newOffsetY);
                    newOffsetY = Math.Min(0, newOffsetY);

                    if (oldOffsetY != newOffsetY)
                        animationProperties.InsertScalar("OffsetY", newOffsetY);
                };
            }

            SizeChanged += (sender, args) =>
            {
                if (scrollProperties == null)
                    scrollProperties = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);

                var compositor = scrollProperties.Compositor;

                if (animationProperties == null)
                {
                    animationProperties = compositor.CreatePropertySet();
                    animationProperties.InsertScalar("OffsetY", 0.0f);
                }

                var expressionAnimation = compositor.CreateExpressionAnimation("animationProperties.OffsetY - ScrollingProperties.Translation.Y");

                expressionAnimation.SetReferenceParameter("ScrollingProperties", scrollProperties);
                expressionAnimation.SetReferenceParameter("animationProperties", animationProperties);

                var headerVisual = ElementCompositionPreview.GetElementVisual((UIElement)Header);
                headerVisual.StartAnimation("Offset.Y", expressionAnimation);
            };
        }

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
        private double previousVerticalScrollOffset = 0.0;
        private CompositionPropertySet scrollProperties;
        private CompositionPropertySet animationProperties;

        private int GetContainerCount { get; set; }
        private int PrepareContainerCount { get; set; }
    }
}
