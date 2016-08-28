﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            InitializeComponent();

            Loaded += SetCourseList;
            Current = this;
        }

        private async void SetCourseList(object sender, RoutedEventArgs e)
        {
            progressRing.IsActive = true;
            await VM.GetCourseList();
            progressRing.IsActive = false;
        }

        private void splitViewToggle_Click(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
            NavListView.Visibility = splitView.IsPaneOpen ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            progressRing.IsActive = true;
            await VM.Model.RefreshCourseList(true);
            await VM.GetCourseList();
            progressRing.IsActive = false;
        }

        private async void NavListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            CourseVM item = e.ClickedItem as CourseVM;
            if (MainFrame.SourcePageType != typeof(CoursePage) || CoursePage.Current == null)
            {
                MainFrame.Navigate(typeof(CoursePage), item.Id);
            }
            else
            {
                await CoursePage.Current.NavTo(item.Id);
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