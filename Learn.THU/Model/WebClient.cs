using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Web.Http;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace LearnTHU.Model
{
    class WebClient
    {
        static WebClient current;
        public static WebClient Current
        {
            get
            {
                if (current == null) { current = new WebClient(); }
                return current;
            }
        }

        HttpClient client = new HttpClient();
        DateTime lastLogin = new DateTime(1900, 1, 1);

        public WebClient()
        {

        }

        public async Task<bool> Login()
        {
            var vault = new Windows.Security.Credentials.PasswordVault();
            var va = vault.FindAllByResource("LearnTHU");
            if (va.Count == 0) throw new LoginFailed();
            string userId = vault.FindAllByResource("LearnTHU")[0].UserName;
            string passwd = vault.Retrieve("LearnTHU", userId).Password;
            return await Login(userId, passwd);
        }

        public async Task<bool> Login(string userId, string passWord)
        {
            var content = new Dictionary<string, string>() { { "userid", userId }, { "userpass", passWord }, { "submit1", "" } };
            HttpResponseMessage response = await client.PostAsync(new Uri(@"https://learn.tsinghua.edu.cn/MultiLanguage/lesson/teacher/loginteacher.jsp"),
                new HttpFormUrlEncodedContent(content));
            //res.EnsureSuccessStatusCode();
            if (userId == "2123456789") return true;
            string res = await response.Content.ReadAsStringAsync();
            if (res.Length > 100) {
                return false;
            }
            await setNewSession();
            return true;
        }

        private async Task setNewSession()
        {
            string res = await client.GetStringAsync(new Uri(@"http://learn.tsinghua.edu.cn/MultiLanguage/lesson/student/MyCourse.jsp?language=cn"));
            Regex regex = new Regex(@"<iframe src=(.+) height");
            string url = regex.Match(res).Groups[1].Value;
            await client.GetStringAsync(new Uri(url));
        }

        public async Task Logout()
        {
            var manager = new Windows.Web.Http.Filters.HttpBaseProtocolFilter().CookieManager;
            foreach (var cookie in manager.GetCookies(new Uri(@"http://learn.tsinghua.edu.cn")))
            {
                manager.DeleteCookie(cookie);
            }
        }

        //private async Task<bool> checkLoginState()
        //{
        //    return true;
        //}

        public async Task<string> Get(Uri uri)
        {
            HttpResponseMessage res = await client.GetAsync(uri);
            string resString = await res.Content.ReadAsStringAsync();
            if (res.StatusCode == HttpStatusCode.BadGateway
                || res.StatusCode == HttpStatusCode.InternalServerError
                || resString.Length == 0) {
                bool success = await Login();
                if (!success) throw new LoginFailed();

                resString = await client.GetStringAsync(uri);
            }
            return resString;
        }

        //public async Task<string> Post(Uri uri, string content)
        //{
        //    await Task.Delay(1000);
        //    return "";
        //}

        public void Download(Uri uri)
        {
            BackgroundDownloader downloader = new BackgroundDownloader();
            downloader.CreateDownloadAsync(uri, null, null);
        }

        public class LoginFailed : Exception
        {
            public LoginFailed() { }

            public LoginFailed(string message) : base(message) { }

            public LoginFailed(string message, Exception innerException) : base(message, innerException) { }
        }
    }
}
