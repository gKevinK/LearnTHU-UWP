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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace LearnTHU.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Login : Page
    {
        public Login()
        {
            this.InitializeComponent();
        }

        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            progressRing.Visibility = Visibility.Visible;
            progressRing.IsActive = true;
            LoginBtn.IsEnabled = false;
            try
            {
                var web = new Web();
                var result = await web.Login(UserID.Text, Passwd.Password);
                if (result == Web.LoginResult.Success)
                {
                    var vault = new Windows.Security.Credentials.PasswordVault();
                    vault.Add(new Windows.Security.Credentials.PasswordCredential(
                        "LearnTHU", UserID.Text, Passwd.Password));
                    new MainModel(web);
                    ((Frame)Window.Current.Content).Navigate(typeof(MainPage));
                    return;
                }
                else if (result == Web.LoginResult.Failed)
                {
                    progressRing.Visibility = Visibility.Collapsed;
                    progressRing.IsActive = false;
                    LoginBtn.IsEnabled = true;
                    await new Windows.UI.Popups.MessageDialog(@"用户名或密码错误").ShowAsync();
                    return;
                }
            }
            catch { }
            progressRing.Visibility = Visibility.Collapsed;
            progressRing.IsActive = false;
            LoginBtn.IsEnabled = true;
            await new Windows.UI.Popups.MessageDialog(@"登录遇到了问题…").ShowAsync();
        }
    }
}
