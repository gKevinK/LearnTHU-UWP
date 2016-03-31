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

        public async Task Save()
        {
            await SaveLoad.SaveData(CourseList);
        }

        public async Task Load()
        {
            await SaveLoad.LoadData(CourseList);
        }

        public async Task<UpdateResult> RefreshAllData()
        {
            // TODO
            return UpdateResult.Success;
        }

        public async Task<UpdateResult> RefreshCourseList()
        {
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
            if (!(course.NeedRefresh || DateTime.Now - course.UpdateNoticeTime > new TimeSpan(2, 0, 0) || forceRefresh))
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
            if (!(course.NeedRefresh || DateTime.Now - course.UpdateFileTime > new TimeSpan(2, 0, 0) || forceRefresh))
            {
                return UpdateResult.No;
            }
            List<File> list = course.IsNewWebLearning ?
                await web.GetFileListNew(course.Id) : await web.GetFileListOld(course.Id);
            Merge.FileList(course.FileList, list);
            course.UpdateFileTime = DateTime.Now;
            return UpdateResult.Success;
        }

        public async Task<UpdateResult> DowloadFile(string courseId, string fileId)
        {
            // TODO
            return UpdateResult.Success;
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
            // TODO
            return UpdateResult.Success;
        }

        //public async Task<UpdateResult> GetWork(string courseId, string workId)
        //{
        //    // TODO
        //    return UpdateResult.Success;
        //}

        public enum UpdateResult
        {
            Success, No, Failed, Error
        }
    }
}
