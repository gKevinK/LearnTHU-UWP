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
            await VM.ChangeCourse(id);
        }
        
        public async void NavTo(string courseId, ListKind listKind = ListKind.Null, string itemId = null)
        {
            if (courseId != VM.CourseId)
            {
                await VM.ChangeCourse(courseId);
                if (listKind == ListKind.Null || listKind == ListKind.Notice)
                {
                    await VM.PrepareNoticeList();
                }
                else if (listKind == ListKind.File)
                {
                    await VM.PrepareFileList();
                }
                else
                {
                    await VM.PrepareWorkList();
                }
                noticeDetailControl.Visibility = Visibility.Collapsed;
                workDetailControl.Visibility = Visibility.Collapsed;
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
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            noticeDetailControl.Visibility = Visibility.Collapsed;
            detailColumn.Width = new GridLength(0);
        }

        private void fileList_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void workList_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void listPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            new Windows.UI.Popups.MessageDialog("!").ShowAsync();
        }
    }
}
