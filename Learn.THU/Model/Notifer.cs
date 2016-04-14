using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace LearnTHU.Model
{
    static class Notifer
    {
        public static void Notify(string courseName, NotifyEventArgs args)
        {
            // TODO
        }
    }

    class NotifyEventArgs : EventArgs
    {
        public string CourseName { get; set; }
        public int NoticeNum { get; set; }
        public int FileNum { get; set; }
        public int WorkNum { get; set; }

        public NotifyEventArgs(string courseName, int noticeNum, int fileNum, int workNum)
        {
            CourseName = courseName;
            NoticeNum = noticeNum;
            FileNum = fileNum;
            WorkNum = workNum;
        }
    }
}
