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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace LearnTHU.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MainViewModel VM { get; set; } = new MainViewModel();
        public MainPage Current = null;

        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += SetCourseList;
            Current = this;
        }

        private async void SetCourseList(object sender, RoutedEventArgs e)
        {
            await VM.GetCourseList();
        }

        private void splitViewToggle_Click(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
            NavListView.Visibility = splitView.IsPaneOpen ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            await VM.Model.RefreshCourseList(true);
            await VM.GetCourseList();
        }

        private void NavListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            CourseVM item = e.ClickedItem as CourseVM;
            if (MainFrame.SourcePageType != typeof(CoursePage) || CoursePage.Current == null)
            {
                MainFrame.Navigate(typeof(CoursePage), item.Id);
            }
            else
            {
                CoursePage.Current.NavTo(item.Id);
            }
        }

        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(typeof(Setting));
        }

        public async void NotifyUser(string message)
        {
            // TODO
            await new Windows.UI.Popups.MessageDialog(message).ShowAsync();
        }


    }
}
