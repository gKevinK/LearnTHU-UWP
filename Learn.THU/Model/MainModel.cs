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
            //if (CourseList == null)
            //{
            //    CourseList = await web.GetCourseListOld();
            //}
            Course course = CourseList.Find(c => c.Id == courseId);
            if (course == null)
            {
                throw new Exception("No record of this course.");
            }
            List<Notice> list = course.NoticeList;
            if (list == null || course.NeedRefresh || DateTime.Now - course.RefreshTime > new TimeSpan(2, 0, 0))
            {
                course.NoticeList = await web.GetNoticeListOld(course.Id);
                // TODO Merge
                list = course.NoticeList;
            }
            return list;
        }

        public async Task<RefreshResult> GetNotice(string courseId, string noticeId)
        {
            // TODO
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
            Success, Failed, Error
        }
    }
}
