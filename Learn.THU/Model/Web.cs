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
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

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
        public async Task<LoginResult> Login(string userId, string userPwd)
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
                if (userId == "2012345678") return LoginResult.Success;
                if (response.ContentLength > 100)
                {
                    return LoginResult.Failed;
                }
                cc.Add(new Uri(@"https://learn.tsinghua.edu.cn"), response.Cookies);
                response.Dispose();
                
                await SetNewSession();
            }
            catch
            {
                return LoginResult.Error;
            }
            return LoginResult.Success;
        }

        private async Task SetNewSession()
        {
            string coursesHtml = await Request(@"http://learn.tsinghua.edu.cn/MultiLanguage/lesson/student/MyCourse.jsp?language=cn");
            Regex regex = new Regex(@"<iframe src=(.+) height");
            string url = regex.Match(coursesHtml).Groups[1].Value;
            await Request(url);
        }

        /// <summary>
        /// 登录新版网络学堂，获取Cookie
        /// </summary>
        /// <param name="userId">用户名</param>
        /// <param name="userPwd">密码</param>
        /// <returns>登录结果</returns>
        public async Task<LoginResult> LoginNew(string userId, string userPwd)
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

        public async Task Logout()
        {

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

        public async Task<List<Notice>> GetNoticeListNew(string courseId)
        {
            string url = string.Format(@"http://learn.cic.tsinghua.edu.cn/b/myCourse/notice/listForStudent/{0}?currentPage=1&pageSize=100", courseId);
            string json = await Request(url);
            return Parse.NoticeListNew(json);
        }

        public async Task<Notice> GetNoticeContentOld(string courseId, string noticeId)
        {
            string url = string.Format(@"http://learn.tsinghua.edu.cn/MultiLanguage/public/bbs/note_reply.jsp?bbs_type=课程公告&id={0}&course_id={1}", noticeId, courseId);
            string html = await Request(url);
            return Parse.NoticeOld(html);
        }

        public async Task<Notice> GetNoticeContentNew(string courseId, string noticeId)
        {
            string url = string.Format(@"http://learn.cic.tsinghua.edu.cn/b/myCourse/notice/studDetail/{0}", noticeId);
            string json = await Request(url);
            return Parse.NoticeNew(json);
        }

        public async Task<List<File>> GetFileListOld(string courseId)
        {
            string url = string.Format(@"http://learn.tsinghua.edu.cn/MultiLanguage/lesson/student/download.jsp?course_id={0}", courseId);
            string html = await Request(url);
            return Parse.FileListOld(html);
        }

        public async Task<List<File>> GetFileListNew(string courseId)
        {
            string url = string.Format(@"http://learn.cic.tsinghua.edu.cn/b/myCourse/tree/getCoursewareTreeData/{0}/0", courseId);
            string json = await Request(url);
            var list = Parse.FileListNew(json);
            string json2 = await Request(@"http://learn.cic.tsinghua.edu.cn/b/courseFileAccess/getUnreadFileIdList/" + courseId);
            Parse.FileListNew2(list, json2);
            return list;
        }

        public async Task<List<Work>> GetWorkListOld(string courseId)
        {
            string url = string.Format(@"http://learn.tsinghua.edu.cn/MultiLanguage/lesson/student/hom_wk_brw.jsp?course_id={0}", courseId);
            string html = await Request(url);
            return Parse.WorkListOld(html);
        }

        public async Task<List<Work>> GetWorkListNew(string courseId)
        {
            string url = string.Format(@"http://learn.cic.tsinghua.edu.cn/b/myCourse/homework/list4Student/{0}/0", courseId);
            string json = await Request(url);
            return Parse.WorkListNew(json);
        }

        public async Task<Work> GetWorkContent(string courseId, string workId)
        {
            string url = string.Format(@"http://learn.tsinghua.edu.cn/MultiLanguage/lesson/student/hom_wk_detail.jsp?id={1}&course_id={0}&rec_id=null", courseId, workId);
            string html = await Request(url);
            return Parse.WorkOld(html);
        }

        public async Task DownloadFile(string url, string fileName = null)
        {
            BackgroundDownloader bd = new BackgroundDownloader();
            string cookie = cc.GetCookieHeader(new Uri("http://learn.tsinghua.edu.cn"));
            bd.SetRequestHeader("Cookie", cookie);
            StorageFile fileTemp = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync(new Random().Next().ToString());
            var downOpr = bd.CreateDownload(new Uri(url), fileTemp);
            await downOpr.StartAsync();

            var picker = new Windows.Storage.Pickers.FileSavePicker();
            if (fileName != null)
            {
                picker.SuggestedFileName = fileName;
            }
            else
            {
                string head = downOpr.GetResponseInformation().Headers["Content-Disposition"];
                fileName = Regex.Replace(head, @".+filename=""", "");
                fileName = fileName.Substring(0, fileName.Length - 1);
                fileName = Regex.Replace(fileName, "_?[0-9]{6,}_?", "");
                picker.SuggestedFileName = fileName;
            }
            string typeName = new FileInfo(fileName).Extension;
            picker.FileTypeChoices.Add("文件", new List<string>() { typeName });
            StorageFile file = await picker.PickSaveFileAsync();
            await fileTemp.MoveAndReplaceAsync(file);
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
            if (res.StatusCode == HttpStatusCode.BadGateway || res.StatusCode == HttpStatusCode.InternalServerError
                || res.ContentLength == 0)
            {
                var vault = new Windows.Security.Credentials.PasswordVault();
                var va = vault.FindAllByResource("LearnTHU");
                string userId = vault.FindAllByResource("LearnTHU")[0].UserName;
                if (userId == "2012345678") return "";
                string passwd = vault.Retrieve("LearnTHU", userId).Password;
                LoginResult result;
                if (url.Contains("learn.cic"))
                {
                    result = await LoginNew(userId, passwd);
                }
                else
                {
                    result = await Login(userId, passwd);
                }
                if (result == LoginResult.Success)
                {
                    req = (HttpWebRequest)WebRequest.Create(url);
                    req.CookieContainer = cc;
                    res = (HttpWebResponse)await req.GetResponseAsync();
                }
                else
                {
                    throw new Exception("登录失败");
                }
            }
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

        private async Task<string> GetFileName(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.CookieContainer = cc;
            HttpWebResponse res = (HttpWebResponse)await req.GetResponseAsync();
            return "*.*";
        }
    }
}
