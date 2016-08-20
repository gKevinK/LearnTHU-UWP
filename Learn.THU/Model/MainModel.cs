using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Web.Http;

namespace LearnTHU.Model
{
    class MainModel
    {
        public List<Course> CourseList { get; set; } = new List<Course>();
        public bool Loaded = false;
        private DateTime lastRefreshTime = new DateTime(2000, 1, 1);

        private Web web;

        public static MainModel Current { get; private set; }

        public MainModel()
        {
            if (web == null)
                web = new Web();
            Current = this;
        }

        public MainModel(Web web2)
        {
            web = web2;
            Current = this;
        }

        public async Task Save()
        {
            await SaveLoad.SaveData(CourseList);
        }

        public async Task Load()
        {
            await SaveLoad.LoadData(CourseList);
            Loaded = true;
        }

        public async Task<UpdateResult> RefreshCourseList(bool force = false)
        {
            if (DateTime.Now - lastRefreshTime < new TimeSpan(2, 0, 0) && force == false)
                return UpdateResult.No;
            try
            {
                var newList = await web.GetCourseListOld();
                Merge.CourseList(CourseList, newList);
                lastRefreshTime = DateTime.Now;
                return UpdateResult.Success;
            }
            catch
            {
                return UpdateResult.Error;
            }
        }

        public List<Course> GetCourseList()
        {
            return CourseList;
        }

        public List<Notice> GetNoticeList(string courseId)
        {
            Course course = CourseList.Find(c => c.Id == courseId);
            if (course == null || course.NoticeList == null)
            {
                throw new Exception("No record of this course.");
            }
            return course.NoticeList;
        }

        public async Task<UpdateResult> RefNoticeList(string courseId, bool forceRefresh = false)
        {
            Course course = CourseList.Find(c => c.Id == courseId);
            if (course == null)
            {
                throw new Exception("No record of this course.");
            }
            if (!(course.UpdateNoticeTime < course.UpdateTime ||
                DateTime.Now - course.UpdateNoticeTime > new TimeSpan(2, 0, 0) ||
                forceRefresh))
            {
                return UpdateResult.No;
            }
            List<Notice> list = course.IsNewWebLearning ?
                await web.GetNoticeListNew(course.Id) : await web.GetNoticeListOld(course.Id);
            Merge.NoticeList(course.NoticeList, list);
            course.UpdateNoticeTime = DateTime.Now;
            return UpdateResult.Success;
        }

        public Notice GetNotice(string courseId, string noticeId)
        {
            Course course = CourseList.Find(c => c.Id == courseId);
            if (course == null)
            {
                throw new Exception("No record of this course.");
            }
            Notice notice = course.NoticeList.Find(n => n.Id == noticeId);
            if (notice == null)
            {
                throw new Exception("No record of this notice.");
            }
            return notice;
        }

        public async Task<UpdateResult> RefNotice(string courseId, string noticeId, bool force = false)
        {
            Notice oldNotice = GetNotice(courseId, noticeId);
            if (oldNotice.Content != null && oldNotice.IsRead == true && force == false)
            {
                return UpdateResult.No;
            }
            Notice newNotice = courseId.Length < 10 ?
                await web.GetNoticeContentOld(courseId, noticeId) : await web.GetNoticeContentNew(courseId, noticeId);
            oldNotice.Content = newNotice.Content;
            return UpdateResult.Success;
        }

        public List<File> GetFileList(string courseId)
        {
            Course course = CourseList.Find(c => c.Id == courseId);
            if (course == null || course.FileList == null)
            {
                throw new Exception("No record of this course.");
            }
            return course.FileList;
        }

        public async Task<UpdateResult> RefFileList(string courseId, bool forceRefresh = false)
        {
            Course course = CourseList.Find(c => c.Id == courseId);
            if (course == null)
            {
                throw new Exception("No record of this course.");
            }
            if (!(course.UpdateFileTime < course.UpdateTime ||
                DateTime.Now - course.UpdateFileTime > new TimeSpan(2, 0, 0) ||
                forceRefresh))
            {
                return UpdateResult.No;
            }
            List<File> list = course.IsNewWebLearning ?
                await web.GetFileListNew(course.Id) : await web.GetFileListOld(course.Id);
            Merge.FileList(course.FileList, list);
            course.UpdateFileTime = DateTime.Now;
            return UpdateResult.Success;
        }

        public async Task<UpdateResult> DownloadFile(File f)
        {
            if (f.Id.Contains("uploadFile/downloadFile"))
            {
                await web.DownloadFile("http://learn.tsinghua.edu.cn" + f.Id);
            }
            else
            {
                await web.DownloadFile("http://learn.cic.tsinghua.edu.cn/b/resource/downloadFileStream/" + f.Id, f.FileName);
            }
            return UpdateResult.Success;
        }

        public async Task DownloadFile(string url)
        {
            await web.DownloadFile(url);
        }

        public List<Work> GetWorkList(string courseId)
        {
            Course course = CourseList.Find(c => c.Id == courseId);
            if (course == null || course.WorkList == null)
            {
                throw new Exception("No record of this course.");
            }
            return course.WorkList;
        }

        public async Task<UpdateResult> RefWorkList(string courseId)
        {
            Course course = CourseList.Find(c => c.Id == courseId);
            List<Work> newList = course.IsNewWebLearning ?
                await web.GetWorkListNew(course.Id):await web.GetWorkListOld(course.Id);
            Merge.WorkList(course.WorkList, newList);
            return UpdateResult.Success;
        }

        public async Task<UpdateResult> RefWork(string courseId, Work work)
        {
            Work workWithContent = await web.GetWorkContent(courseId, work.Id);
            work.Content = workWithContent.Content;
            work.Attachment = workWithContent.Attachment;
            return UpdateResult.Success;
        }

        public HttpRequestMessage GetCoursePageHRM(string courseId)
        {
            Course course = CourseList.Find(c => c.Id == courseId);
            HttpRequestMessage hrm;
            if (course.IsNewWebLearning)
            {
                string url = string.Format(@"http://learn.cic.tsinghua.edu.cn/f/student/coursehome/{0}", course.Id);
                hrm = new HttpRequestMessage(HttpMethod.Get, new Uri(url));
                foreach (System.Net.Cookie cookie in web.cc.GetCookies(new Uri("http://learn.cic.tsinghua.edu.cn")))
                {
                    hrm.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue(cookie.Name, cookie.Value));
                }
            }
            else
            {
                string url = string.Format(@"http://learn.tsinghua.edu.cn/MultiLanguage/lesson/student/course_locate.jsp?course_id={0}", course.Id);
                hrm = new HttpRequestMessage(HttpMethod.Get, new Uri(url));
                foreach (System.Net.Cookie cookie in web.cc.GetCookies(new Uri("http://learn.tsinghua.edu.cn")))
                {
                    hrm.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue(cookie.Name, cookie.Value));
                }
            }
            return hrm;
        }

        public enum UpdateResult
        {
            Success, No, Failed, Error
        }
    }
}
