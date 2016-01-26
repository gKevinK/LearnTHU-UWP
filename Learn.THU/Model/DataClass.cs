using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnTHU.Model
{
    class Course
    {
        public string Id;
        public string Name;
        public bool IsNewWebLearning;

        public List<Notice> NoticeList;
        public List<File> FileList;
        public List<Work> WorkList;
    }

    class Notice
    {
        public string Id;
        public string Title;
        public DateTime Date;
        public bool IsRead;
        public string Content;
    }

    class File
    {
        public string Id;
        public string Name;
        public string Note;
        public DateTime UploadDate;
        public double FileSize;
        public string FileName;

        public enum FileStatus
        {
            Undownload, Downloaded, Removed
        }
        public FileStatus Status;
    }

    class Work
    {
        public string Id;
        public string Title;
        public DateTime BeginTime;
        public DateTime EndTime;
        public string Content;

        public enum WorkStatus
        {
            Unfinished, Submitted, Ignored
        }
        public WorkStatus Status;
    }
}
