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

using LearnTHU.Model;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace LearnTHU.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            progressRing.Visibility = Visibility.Visible;
            progressRing.IsActive = true;
            LoginBtn.IsEnabled = false;
            try {
                var web = Model.WebClient.Current;
                var result = await web.Login(UserID.Text, Passwd.Password);
                if (result == true) {
                    var vault = new Windows.Security.Credentials.PasswordVault();
                    vault.Add(new Windows.Security.Credentials.PasswordCredential(
                        "LearnTHU", UserID.Text, Passwd.Password));
                    new MainModel();
                    ((Frame)Window.Current.Content).Navigate(typeof(MainPage));
                    ((Frame)Window.Current.Content).BackStack.Clear();
                    return;
                } else if (result == false) {
                    progressRing.Visibility = Visibility.Collapsed;
                    progressRing.IsActive = false;
                    LoginBtn.IsEnabled = true;
                    await new Windows.UI.Popups.MessageDialog(@"用户名或密码错误").ShowAsync();
                    return;
                }
            } catch { }
            progressRing.Visibility = Visibility.Collapsed;
            progressRing.IsActive = false;
            LoginBtn.IsEnabled = true;
            await new Windows.UI.Popups.MessageDialog(@"登录遇到了问题…").ShowAsync();
        }
    }
}
