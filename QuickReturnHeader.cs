using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace QuickReturnHeaderListView
{
    public class QuickReturnHeader : ContentControl
    {
        public QuickReturnHeader()
        {
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
        }

        protected override void OnApplyTemplate()
        {
            if (TargetListView != null)
            {
                scrollViewer = GetScrollViewer(TargetListView);

                // Place items below header
                var panel = TargetListView.ItemsPanelRoot;
                Canvas.SetZIndex(panel, -1);
            }

            if (scrollViewer != null)
            {
                scrollViewer.ViewChanged += (sender, args) =>
                {
                    if (animationProperties != null)
                    {
                        float oldOffsetY = 0.0f;
                        animationProperties.TryGetScalar("OffsetY", out oldOffsetY);

                        var delta = scrollViewer.VerticalOffset - previousVerticalScrollOffset;
                        previousVerticalScrollOffset = scrollViewer.VerticalOffset;

                        var newOffsetY = oldOffsetY - (float)delta;

                        // Keep values within negativ header size and 0
                        FrameworkElement header = (FrameworkElement)TargetListView.Header;
                        newOffsetY = Math.Max((float)-header.ActualHeight, newOffsetY);
                        newOffsetY = Math.Min(0, newOffsetY);

                        if (oldOffsetY != newOffsetY)
                            animationProperties.InsertScalar("OffsetY", newOffsetY);
                    }
                };
            }

            SizeChanged += (sender, args) =>
            {
                if (TargetListView != null)
                {
                    if (QuickReturnEnabled)
                        startAnimation();
                }
            };
        }

        public ListView TargetListView { get; set; }

        public bool QuickReturnEnabled
        {
            get { return (bool)GetValue(QuickReturnEnabledProperty); }
            set { SetValue(QuickReturnEnabledProperty, value); }
        }

        public static readonly DependencyProperty QuickReturnEnabledProperty =
            DependencyProperty.Register("QuickReturnEnabled", typeof(bool),
                typeof(QuickReturnHeader),
                new PropertyMetadata(true, onQuickReturnEnabledChanged));

        private static void onQuickReturnEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = d as QuickReturnHeader;

            if (me.QuickReturnEnabled)
                me.startAnimation();
            else
                me.stopAnimation();
        }

        private void startAnimation()
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

            headerVisual = ElementCompositionPreview.GetElementVisual((UIElement)TargetListView.Header);

            if (headerVisual != null && QuickReturnEnabled)
                headerVisual.StartAnimation("Offset.Y", expressionAnimation);
        }

        private void stopAnimation()
        {
            if(headerVisual != null)
            {
                headerVisual.StopAnimation("Offset.Y");
                animationProperties.InsertScalar("OffsetY", 0.0f);

                var offset = headerVisual.Offset;
                offset.Y = 0.0f;
                headerVisual.Offset = offset;
            }
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

        ScrollViewer scrollViewer;
        private double previousVerticalScrollOffset;
        private CompositionPropertySet scrollProperties;
        private CompositionPropertySet animationProperties;
        private Visual headerVisual;
    }
}
