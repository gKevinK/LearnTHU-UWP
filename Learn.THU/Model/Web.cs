using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;

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
                string html1 = await Request(@"http://learn.cic.tsinghua.edu.cn/index");
                Regex reg = new Regex(@"method=""post"" action=(\S+?)>");
                string url2 = reg.Match(html1).Groups[1].Value;
                
                HttpClientHandler hch = new HttpClientHandler();
                hch.AllowAutoRedirect = false;
                hch.CookieContainer = cc;
                hch.UseCookies = true;
                HttpClient hc = new HttpClient(hch);
                string form = String.Format(@"i_user={0}&i_pass={1}", userId, WebUtility.UrlEncode(userPwd));
                StringContent sc = new StringContent(form);
                sc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(@"application/x-www-form-urlencoded");
                HttpResponseMessage hrm = await hc.PostAsync(url2, sc);
                if (hrm.StatusCode != HttpStatusCode.Redirect && hrm.StatusCode != HttpStatusCode.MovedPermanently)
                {
                    return LoginResult.Error;
                }
                string url3 = hrm.Headers.Location.ToString();
                if (url3.Contains(@"SUCCESS"))
                {
                    await Request(url3);
                    return LoginResult.Success;
                }
                else
                {
                    return LoginResult.Failed;
                }
            }
            catch
            {
                return LoginResult.Error;
            }
        }

        public enum LoginResult
        {
            Success, Failed, Error
        }

        public async Task<List<Course>> GetCourseListOld()
        {
            string html = await Request("http://learn.tsinghua.edu.cn/MultiLanguage/lesson/student/MyCourse.jsp?language=cn");
            return Parse.CourseListOld(html);
        }

        public async Task<List<Notice>> GetNoticeListOld(string courseId)
        {
            string url = string.Format(@"http://learn.tsinghua.edu.cn/MultiLanguage/public/bbs/getnoteid_student.jsp?course_id={0}", courseId);
            string html = await Request(url);
            return Parse.NoticeListOld(html);
        }

        public async Task<Notice> GetNoticeContentOld(string courseId, string noticeId)
        {
            string url = string.Format(@"http://learn.tsinghua.edu.cn/MultiLanguage/public/bbs/note_reply.jsp?bbs_type=课程公告&id={0}&course_id={1}", noticeId, courseId);
            string html = await Request(url);
            return Parse.NoticeOld(html);
        }

        public async Task<List<File>> GetFileGroupListOld(string courseId)
        {
            string url = string.Format(@"http://learn.tsinghua.edu.cn/MultiLanguage/lesson/student/download.jsp?course_id={0}", courseId);
            string html = await Request(url);
            return Parse.FileListOld(html);
        }

        public async Task<List<Work>> GetWorkListOld(string courseId)
        {
            string url = string.Format(@"http://learn.tsinghua.edu.cn/MultiLanguage/lesson/student/hom_wk_brw.jsp?course_id={0}", courseId);
            string html = await Request(url);
            return Parse.WorkListOld(html);
        }



        /// <summary>
        /// 访问Url，获取返回数据
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns>返回数据</returns>
        public async Task<string> Request(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.CookieContainer = cc;
            HttpWebResponse res = (HttpWebResponse)await req.GetResponseAsync();
            string html;
            if (res.ContentLength == -1)
            {
                html = await ReadAllAsync(res.GetResponseStream());
            }
            else
            {
                html = await new StreamReader(res.GetResponseStream()).ReadToEndAsync();
            }
            if (res.Cookies.Count > 0)
            {
                cc.Add(new Uri(@"http://" + new Uri(url).Host), res.Cookies);
            }
            res.Dispose();
            return html;
        }

        /// <summary>
        /// 从Stream中读取并返回字符串
        /// </summary>
        /// <param name="stream">输入流</param>
        /// <returns>返回字符串</returns>
        private async Task<string> ReadAllAsync(Stream stream)
        {
            byte[] buf = new byte[4096];
            Stream ms = new MemoryStream();
            int count = await stream.ReadAsync(buf, 0, 4096);
            while (count > 0)
            {
                ms.Write(buf, 0, count);
                count = stream.Read(buf, 0, 4096);
            }
            ms.Seek(0, SeekOrigin.Begin);
            string a = new StreamReader(ms, Encoding.UTF8).ReadToEnd();
            return a;
        }
    }
}
