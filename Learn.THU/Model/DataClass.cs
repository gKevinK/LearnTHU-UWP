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
        public DateTime UpdateTime;
        public DateTime UpdateNoticeTime;
        public DateTime UpdateFileTime;
        public DateTime UpdateWorkTime;
        public string Path;

        public bool IsNewWebLearning { get { return Id.Length > 10; } }

        public List<Notice> NoticeList { get; set; } = new List<Notice>();
        public List<File> FileList { get; set; } = new List<File>();
        public List<Work> WorkList { get; set; } = new List<Work>();

        public int NewNoticeCountOriginal;
        public int NewNoticeCount
        {
            get { return (UpdateTime >= UpdateNoticeTime) ? NewNoticeCountOriginal : NoticeList.Count(notice => notice.IsRead == false); }
        }

        public int NewFileCountOriginal;
        public int NewFileCount
        {
            get { return (UpdateTime >= UpdateFileTime) ?
                    NewFileCountOriginal - FileList.Count(file => file.Status == File.FileStatus.Ignored)
                    : FileList.Count(file => file.Status == File.FileStatus.Undownload); }
        }

        public int UnhandWorkCountOriginal;
        public int UnhandWorkCount
        {
            get { return (UpdateTime >= UpdateWorkTime) ?
                    UnhandWorkCountOriginal - WorkList.Count(work => work.Status == Work.WorkStatus.Ignored)
                    : WorkList.Count(work => work.Status == Work.WorkStatus.Unhand); }
        }

        public void InitNewCount(int newNotice, int newFile, int unhandWork)
        {
            NewNoticeCountOriginal = newNotice;
            NewFileCountOriginal = newFile;
            UnhandWorkCountOriginal = unhandWork;
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
        public string Id { get; set; }

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
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Content { get; set; }
        public WorkFile Attachment { get; set; }
        public string Mark { get; set; }
        public string Marker { get; set; }
        public string MarkedDate { get; set; }
        public string MarkContent { get; set; }

        public enum WorkStatus
        {
            Unhand, Submitted, Ignored, Marked
        }
        public WorkStatus Status { get; set; }
    }

    public class WorkFile
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
