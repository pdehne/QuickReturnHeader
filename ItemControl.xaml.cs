using Windows.UI.Xaml.Controls;

namespace QuickReturnHeaderListView
{
    public sealed partial class ItemControl : UserControl
    {
        private static int instanceCount;

        public ItemControl()
        {
            this.InitializeComponent();

            instanceCount++;
            System.Diagnostics.Debug.WriteLine("ItemControl instances " + instanceCount);
        }
    }
}
