using LearnTHU.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LearnTHU.View
{
    public sealed partial class WorkItemControl : UserControl
    {
        public WorkItemControl()
        {
            this.InitializeComponent();
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as WorkVM).Status = Model.Work.WorkStatus.Ignored;
            CourseViewModel.Current.UpdateNumbers();
        }

        private void RaiseMenu(object sender, RoutedEventArgs e)
        {
            if ((DataContext as WorkVM).Status == Model.Work.WorkStatus.Ignored)
                FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
    }
}
