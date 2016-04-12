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
using System.Threading.Tasks;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace LearnTHU.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CoursePage : Page
    {
        public static CoursePage Current = null;

        CourseViewModel VM { get; set; } = new CourseViewModel();

        public CoursePage()
        {
            this.InitializeComponent();
            Current = this;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string id = e.Parameter as string;
            await NavTo(id);
        }
        
        public async Task NavTo(string courseId, ListKind listKind = ListKind.Null, string itemId = null)
        {
            if (courseId != VM.CourseId)
            {
                await VM.ChangeCourse(courseId);
                if (listKind == ListKind.Null || listKind == ListKind.Notice)
                {
                    listPivot.SelectedIndex = 0;
                    await VM.PrepareNoticeList();
                }
                else if (listKind == ListKind.File)
                {
                    listPivot.SelectedIndex = 1;
                    await VM.PrepareFileList();
                }
                else
                {
                    listPivot.SelectedIndex = 2;
                    await VM.PrepareWorkList();
                }
                HideDetailColumn();
            }
        }

        public enum ListKind
        {
            Null, Notice, File, Work
        }

        private async void noticeList_ItemClick(object sender, ItemClickEventArgs e)
        {
            noticeDetailControl.Visibility = Visibility.Visible;
            detailColumn.Width = new GridLength(1, GridUnitType.Star);

            await VM.ChangeNoticeDetail(e.ClickedItem as NoticeVM);
            detailContentWebView.NavigateToString(VM.NoticeDetail.Content);
            MainViewModel.Current.RaiseCourseDataChanged(VM.CourseId);
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            HideDetailColumn();
        }

        private void fileList_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private async void workList_ItemClick(object sender, ItemClickEventArgs e)
        {
            workDetailControl.Visibility = Visibility.Visible;
            detailColumn.Width = new GridLength(1, GridUnitType.Star);

            WorkVM workVM = e.ClickedItem as WorkVM;
            await VM.ChangeWorkDetail(workVM);
            workContent.NavigateToString(workVM.Content);
        }

        private async void listPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot pivot = sender as Pivot;
            HideDetailColumn();
            SetNavBtn(pivot.SelectedIndex);
            switch (pivot.SelectedIndex)
            {
                case (0):
                    await VM.PrepareNoticeList(); break;
                case (1):
                    await VM.PrepareFileList(); break;
                case (2):
                    await VM.PrepareWorkList(); break;
            }
        }

        private void WebViewBtn_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(WebsitePage), VM.GetHRM());
        }

        private void HideDetailColumn()
        {
            noticeDetailControl.Visibility = Visibility.Collapsed;
            workDetailControl.Visibility = Visibility.Collapsed;
            detailColumn.Width = new GridLength(0);
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var workVM = sender as WorkVM;
            workVM.Status = Model.Work.WorkStatus.Ignored;
        }

        private void Border0_Tapped(object sender, TappedRoutedEventArgs e)
        {
            listPivot.SelectedIndex = 0;
        }

        private void Border1_Tapped(object sender, TappedRoutedEventArgs e)
        {
            listPivot.SelectedIndex = 1;

        }
        private void Border2_Tapped(object sender, TappedRoutedEventArgs e)
        {
            listPivot.SelectedIndex = 2;
        }

        private void SetNavBtn(int index)
        {
            noticeBtnRec.Visibility = Visibility.Collapsed;
            fileBtnRec.Visibility = Visibility.Collapsed;
            workBtnRec.Visibility = Visibility.Collapsed;
            switch (index)
            {
                case 0:
                    noticeBtnRec.Visibility = Visibility.Visible; break;
                case 1:
                    fileBtnRec.Visibility = Visibility.Visible; break;
                case 2:
                    workBtnRec.Visibility = Visibility.Visible; break;
            }
        }
    }
}
