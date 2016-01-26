using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace LearnTHU.Model
{
    /// <summary>
    /// 网络组件，用于访问网络学堂获取信息
    /// </summary>
    public class Web
    {
        public CookieContainer cc = new CookieContainer();

        /// <summary>
        /// 登录旧版网络学堂，获取Cookie
        /// </summary>
        /// <param name="userId">用户名</param>
        /// <param name="userPwd">密码</param>
        /// <returns>登录结果</returns>
        public async Task<LoginResult> LoginOldAsync(string userId, string userPwd)
        {
            string url1 = @"https://learn.tsinghua.edu.cn/MultiLanguage/lesson/teacher/loginteacher.jsp";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url1);
            request.Method = "POST";
            string form = String.Format(@"userid={0}&userpass={1}&submit1=%E7%99%BB%E5%BD%95",
                WebUtility.UrlEncode(userId),
                WebUtility.UrlEncode(userPwd));
            request.ContentType = @"application/x-www-form-urlencoded";
            request.CookieContainer = cc;
            using (Stream requestStream = await request.GetRequestStreamAsync())
            {
                StreamWriter requestStreamWriter = new StreamWriter(requestStream);
                requestStreamWriter.Write(form);
                requestStreamWriter.Flush();
                requestStream.Flush();
                requestStreamWriter.Dispose();
            }

            // 分析返回内容
            try
            {
                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
                if (response.ContentLength > 100)
                {
                    return LoginResult.Failed;
                }
                cc.Add(new Uri(@"https://learn.tsinghua.edu.cn"), response.Cookies);
                response.Dispose();
            }
            catch
            {
                return LoginResult.Error;
            }
            return LoginResult.Success;
        }

        /// <summary>
        /// 登录新版网络学堂，获取Cookie
        /// </summary>
        /// <param name="userId">用户名</param>
        /// <param name="userPwd">密码</param>
        /// <returns>登录结果</returns>
        public async Task<LoginResult> LoginNewAsync(string userId, string userPwd)
        {
            try
            {
                string url1 = @"https://learn.cic.tsinghua.edu.cn/index";
                HttpWebRequest req1 = (HttpWebRequest)WebRequest.Create(url1);
                HttpWebResponse res1 = (HttpWebResponse)await req1.GetResponseAsync();
                string html1 = await new StreamReader(res1.GetResponseStream()).ReadToEndAsync();
                res1.Dispose();
                Regex reg = new Regex(@"method=""post"" action=(\S+?)>");
                string url2 = reg.Match(html1).Groups[1].Value;

                HttpWebRequest req2 = (HttpWebRequest)WebRequest.Create(url2);
                req2.Method = "POST";
                string form = String.Format(@"i_user={0}&i_pass={1}",
                    WebUtility.UrlEncode(userId), WebUtility.UrlEncode(userPwd));
                req2.ContentType = @"application/x-www-form-urlencoded";
                req2.CookieContainer = cc;
                using (Stream requestStream = await req2.GetRequestStreamAsync())
                {
                    StreamWriter requestStreamWriter = new StreamWriter(requestStream);
                    requestStreamWriter.Write(form);
                    requestStreamWriter.Flush();
                    requestStream.Flush();
                    requestStreamWriter.Dispose();
                }
                HttpWebResponse res2 = (HttpWebResponse)await req2.GetResponseAsync();
            }
            catch
            {
                return LoginResult.Error;
            }
            return LoginResult.Success;
        }

        public enum LoginResult
        {
            Success, Failed, Error
        }
    }
}
