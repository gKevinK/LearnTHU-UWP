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
        public static List<Course> CourseList { get; set; }

        static Web web;

        public MainModel()
        {
            if (web == null)
                web = new Web();
            if (CourseList == null)
                CourseList = new List<Course>();
        }

        public MainModel(Web web2)
        {
            if (CourseList == null)
                CourseList = new List<Course>();
            web = web2;
        }

        public async Task<RefreshResult> RefreshAllData()
        {
            // TODO
            return RefreshResult.Success;
        }

        public async Task<RefreshResult> RefreshCourseList()
        {
            // TODO
            return RefreshResult.Success;
        }

        public async Task<RefreshResult> RefreshCourse(string courseId)
        {
            // TODO
            return RefreshResult.Success;
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
            }
            return CourseList;
        }

        public async Task<List<Notice>> GetNoticeList(string courseId)
        {
            if (CourseList == null)
            {
                CourseList = await web.GetCourseListOld();
            }
            Course course = CourseList.Find(c => c.Id == courseId);
            if (course == null)
            {
                throw new Exception("No record of this course.");
            }
            List<Notice> list = course.NoticeList;
            if (list == null)
            {
                course.NoticeList = await web.GetNoticeListOld(course.Id);
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
