using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnTHU.Model
{
    class Merge
    {
        public static void CourseList(List<Course> oldList, List<Course> newList)
        {
            foreach (Course newCourse in newList)
            {
                Course oldCourse = oldList.Find(oc => oc.Id == newCourse.Id);
                if (oldCourse == null) break;
                newCourse.NoticeList = oldCourse.NoticeList;
                newCourse.UpdateNoticeTime = oldCourse.UpdateNoticeTime;
                newCourse.FileList = oldCourse.FileList;
                newCourse.UpdateFileTime = oldCourse.UpdateFileTime;
                newCourse.WorkList = oldCourse.WorkList;
                newCourse.UpdateWorkTime = oldCourse.UpdateWorkTime;
            }
            oldList.Clear();
            foreach (Course newCourse in newList)
            {
                oldList.Add(newCourse);
            }
            return;
        }

        public static int NoticeList(List<Notice> oldList, List<Notice> newList)
        {
            int newNum = 0;
            foreach (Notice newNotice in newList)
            {
                Notice oldNotice = oldList.Find(on => on.Id == newNotice.Id);
                if (oldNotice == null || newNotice.IsRead == false)
                {
                    newNum += 1; break;
                }
                if (newNotice.IsRead == true)
                {
                    newNotice.Content = oldNotice.Content;
                }
                newNotice.IsRead = oldNotice.IsRead && newNotice.IsRead;
            }
            oldList.Clear();
            foreach (Notice newNotice in newList)
                oldList.Add(newNotice);
            return newNum;
        }

        public static int FileList(List<File> oldList, List<File> newList)
        {
            int newNum = 0;
            foreach (File newFile in newList)
            {
                File oldFile = oldList.Find(of => of.Id == newFile.Id);
                if (oldFile == null)
                {
                    newNum += 1; break;
                }
                // TODO
                newFile.FileName = oldFile.FileName;
                if (oldFile.Status == File.FileStatus.Ignored)
                    newFile.Status = File.FileStatus.Ignored;
            }
            oldList.Clear();
            foreach (File newFile in newList)
                oldList.Add(newFile);
            return newNum;
        }

        public static int WorkList(List<Work> oldList, List<Work> newList)
        {
            int newNum = 0;
            foreach (Work newWork in newList)
            {
                Work oldWork = oldList.Find(ow => ow.Id == newWork.Id);
                if (oldWork == null)
                {
                    newNum += 1; break;
                }
                newWork.Content = oldWork.Content;
                newWork.Attachment = oldWork.Attachment;
                if (newWork.Status == Work.WorkStatus.Unhand && oldWork.Status == Work.WorkStatus.Ignored)
                    newWork.Status = Work.WorkStatus.Ignored;
            }
            oldList.Clear();
            foreach (Work newWork in newList)
            {
                oldList.Add(newWork);
            }
            return newNum;
        }

    }
}
