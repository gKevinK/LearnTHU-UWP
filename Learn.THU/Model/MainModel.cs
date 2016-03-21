using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace LearnTHU.Model
{
    class MainModel
    {
        public List<Course> CourseList { get; set; }
        private DateTime lastRefreshTime = new DateTime(2000, 1, 1);

        Web web;

        public static MainModel Current { get; private set; }

        public MainModel()
        {
            if (Current != null)
                return;
            if (web == null)
                web = new Web();
            if (CourseList == null)
                CourseList = new List<Course>();
            Current = this;
        }

        public MainModel(Web web2)
        {
            if (Current != null)
            {
                Current.web = web2;
            }
            if (CourseList == null)
                CourseList = new List<Course>();
            web = web2;
            Current = this;
        }

        public async Task<RefreshResult> RefreshAllData()
        {
            // TODO
            return RefreshResult.Success;
        }

        public async Task<RefreshResult> RefreshCourseList()
        {
            try
            {
                var newList = await web.GetCourseListOld();
                //Merge.CourseList(CourseList, newList);
                CourseList = newList;
                lastRefreshTime = DateTime.Now;
                return RefreshResult.Success;
            }
            catch
            {
                return RefreshResult.Error;
            }
        }

        public async Task<RefreshResult> RefreshCourse(string courseId)
        {
            try
            {
                return RefreshResult.Success;
            }
            catch
            {
                return RefreshResult.Error;
            }
        }

        public async Task<bool> Serialize()
        {
            try
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                System.Runtime.Serialization.Json.DataContractJsonSerializer JsonSerializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(CourseList.GetType());
                StorageFile file = await folder.CreateFileAsync(@"CourseData.json", CreationCollisionOption.ReplaceExisting);
                Stream stream = await file.OpenStreamForWriteAsync();
                JsonSerializer.WriteObject(stream, CourseList);
                stream.Flush();
                stream.Dispose();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<bool> Unserialize()
        {
            try
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                System.Runtime.Serialization.Json.DataContractJsonSerializer JsonSerializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(CourseList.GetType());
                StorageFile file = await folder.GetFileAsync(@"CourseData.json");
                Stream stream = await file.OpenStreamForReadAsync();
                CourseList = (List<Course>)JsonSerializer.ReadObject(stream);
                stream.Flush();
                stream.Dispose();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<List<Course>> GetCourseList()
        {
            if (CourseList == null)
            {
                CourseList = await web.GetCourseListOld();
                lastRefreshTime = DateTime.Now;
                return CourseList;
            }
            if (DateTime.Now - lastRefreshTime > new TimeSpan(2, 0, 0))
            {
                await RefreshCourseList();
            }
            return CourseList;
        }

        public async Task<List<Notice>> GetNoticeList(string courseId)
        {
            Course course = CourseList.Find(c => c.Id == courseId);
            if (course == null || course.NoticeList == null)
            {
                throw new Exception("No record of this course.");
            }
            return course.NoticeList;
        }

        public async Task<RefreshResult> RefNoticeList(string courseId, bool forceRefresh = false)
        {
            Course course = CourseList.Find(c => c.Id == courseId);
            if (course == null)
            {
                throw new Exception("No record of this course.");
            }
            if (course.NeedRefresh || DateTime.Now - course.RefreshTime > new TimeSpan(2, 0, 0) || forceRefresh)
            {
                List<Notice> list = course.IsNewWebLearning ?
                   await web.GetNoticeListNew(course.Id) : await web.GetNoticeListOld(course.Id);
                Merge.NoticeList(course.NoticeList, list);
                return RefreshResult.Success;
            }
            else
                return RefreshResult.No;
        }

        public async Task<Notice> GetNotice(string courseId, string noticeId)
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

        public async Task<RefreshResult> RefNotice(string courseId, string noticeId)
        {
            Notice newNotice = courseId.Length < 10 ?
                await web.GetNoticeContentOld(courseId, noticeId) : await web.GetNoticeContentNew(courseId, noticeId);
            Notice oldNotice = await GetNotice(courseId, noticeId);
            oldNotice.Content = newNotice.Content;
            return RefreshResult.Success;
        }

        public async Task<RefreshResult> GetFile(string courseId, string url)
        {
            // TODO
            return RefreshResult.Success;
        }

        public async Task<RefreshResult> GetWork(string courseId, string workId)
        {
            // TODO
            return RefreshResult.Success;
        }

        public enum RefreshResult
        {
            Success, No, Failed, Error
        }
    }
}
