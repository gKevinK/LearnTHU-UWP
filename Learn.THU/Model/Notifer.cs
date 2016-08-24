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
            // TODO
            string content = ""; // TODO
            string toastXml = $@"<toast scenario='reminder'>
    <visual>
        <binding template='ToastGeneric'>
            <text>网络学堂 - 新消息</text>
            <text>{courseName} 更新了 {content}.</text>
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
