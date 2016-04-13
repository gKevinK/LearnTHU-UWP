using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace LearnTHU.Model
{
    class Notifer
    {
        static Notifer Current = null;

        public static Notifer GetCurrent()
        {
            if (Current == null)
            {
                Current = new Notifer();
            }
            return Current;
        }

        public Notifer()
        {
        }

        public void Notify(string courseName, NotifyEventArgs args)
        {
            // TODO
        }
    }

    class NotifyEventArgs : EventArgs
    {
        public int NoticeNum { get; set; }
        public int FileNum { get; set; }
        public int WorkNum { get; set; }

        public NotifyEventArgs(int noticeNum, int fileNum, int workNum)
        {
            NoticeNum = noticeNum;
            FileNum = fileNum;
            WorkNum = workNum;
        }
    }
}
