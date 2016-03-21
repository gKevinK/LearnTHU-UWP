using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnTHU.Model
{
    public partial class Course
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsNewWebLearning { get; set; }
        public bool IsActive { get; set; }
        public bool NeedRefresh { get; set; }
        public DateTime RefreshTime { get; set; }

        public List<Notice> NoticeList { get; set; } = new List<Notice>();
        public List<File> FileList { get; set; } = new List<File>();
        public List<Work> WorkList { get; set; } = new List<Work>();

        private int nwc;
        public int NewNoticeCount
        {
            get { return (NeedRefresh) ? nwc : NoticeList.Count(notice => notice.IsRead == false); }
        }

        private int nfc;
        public int NewFileCount
        {
            get { return (NeedRefresh) ?
                    nfc : FileList.Count(file => file.Status == File.FileStatus.Undownload); }
        }

        private int uwc;
        public int UnhandWorkCount
        {
            get { return (NeedRefresh) ?
                    uwc - WorkList.Count(work => work.Status == Work.WorkStatus.Ignored) :
                    WorkList.Count(work => work.Status == Work.WorkStatus.Unhand); }
        }

        public void InitNewCount(int newNotice, int newFile, int unhandWork)
        {
            nwc = newNotice;
            nfc = newFile;
            uwc = unhandWork;
        }
    }

    public class Notice
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Publisher { get; set; }
        public DateTime Date { get; set; }
        public bool IsRead { get; set; }
        public string Content { get; set; }
    }

    public class File
    {
        public string Name { get; set; }
        public string Note { get; set; }
        public DateTime UploadDate { get; set; }
        public string GroupName { get; set; }
        public double FileSize { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }

        public enum FileStatus
        {
            Undownload, Downloaded, Removed, Ignored
        }
        public FileStatus Status { get; set; }
    }

    public class Work
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Content { get; set; }
        public WorkFile Attachment { get; set; }

        public enum WorkStatus
        {
            Unhand, Submitted, Ignored
        }
        public WorkStatus Status { get; set; }
    }

    public class WorkFile
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
