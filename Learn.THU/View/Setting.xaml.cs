using LearnTHU.Model;
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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace LearnTHU.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Setting : Page
    {
        public Setting()
        {
            InitializeComponent();

            Loaded += OnLoad;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            try
            {
                var vault = new Windows.Security.Credentials.PasswordVault();
                UserId.Text = vault.FindAllByResource("LearnTHU")[0].UserName;
            }
            catch { }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await MainModel.Current.Save();
                var vault = new Windows.Security.Credentials.PasswordVault();
                foreach (var record in vault.FindAllByResource("LearnTHU"))
                    vault.Remove(record);
            }
            finally
            {
                ((Frame)Window.Current.Content).Navigate(typeof(Login));
            }
        }
    }
}
