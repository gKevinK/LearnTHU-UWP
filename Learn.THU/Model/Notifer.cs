using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace LearnTHU.Model
{
    static class Notifer
    {
        public static void Notify(string courseName, NotifyEventArgs args)
        {
            if (args.NoticeNum + args.FileNum + args.WorkNum == 0) return;
            List<string> contents = new List<string>();
            if (args.NoticeNum > 0)
                contents.Add($"{args.NoticeNum} 个新公告");
            if (args.FileNum > 0)
                contents.Add($"{args.FileNum} 个新文件");
            if (args.WorkNum > 0)
                contents.Add($"{args.WorkNum} 个新作业");
            string content = string.Join("，", contents);
            string toastXml = $@"<toast scenario='reminder'>
    <visual>
        <binding template='ToastGeneric'>
            <text>网络学堂 - 新消息</text>
            <text>{courseName} 发布了 {content}.</text>
        </binding>
    </visual>
</toast>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(toastXml);
            ToastNotification toast = new ToastNotification(doc);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
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
