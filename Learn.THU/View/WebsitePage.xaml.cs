using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace LearnTHU.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class WebsitePage : Page
    {
        public WebsitePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            webView.UnviewableContentIdentified += WebView_UnviewableContentIdentified;

            HttpRequestMessage hrm = e.Parameter as HttpRequestMessage;
            if (hrm.RequestUri.OriginalString.Contains("0000000"))
            {
                testTip.Visibility = Visibility.Visible;
            }
            else
            {
                webView.NavigateWithHttpRequestMessage(hrm);
            }
        }

        private async void WebView_UnviewableContentIdentified(WebView sender, WebViewUnviewableContentIdentifiedEventArgs args)
        {
            await Model.MainModel.Current.DownloadFile(args.Uri.OriginalString);
            await new Windows.UI.Popups.MessageDialog("下载完成").ShowAsync();
        }
        
        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).GoBack();
        }
    }
}
