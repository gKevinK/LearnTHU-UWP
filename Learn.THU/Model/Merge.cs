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
            foreach (Course c in newList)
            {

            }
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
    }
}
