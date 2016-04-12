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

using LearnTHU.ViewModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LearnTHU.View
{
    public sealed partial class FileItemControl : UserControl
    {
        FileVM vm = null;

        public FileItemControl()
        {
            this.InitializeComponent();
            vm = DataContext as FileVM;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as FileVM).Download();
        }

        private void RaiseMenu(object sender, RoutedEventArgs e)
        {
            if ((DataContext as FileVM).Status == Model.File.FileStatus.Undownload)
            {
                FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
            }
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as FileVM).Status = Model.File.FileStatus.Ignored;
            CourseViewModel.Current.UpdateNumbers();
        }
    }
}
