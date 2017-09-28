using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnTHU.Model
{
    class Course
    {
        public string ID { get; set; }
        public string Name { get; set; }

        // Settings
        public string Directory { get; set; }
        public DateTime UpdateTime { get; set; }

        public List<Notice> NoticeList { get; set; } = new List<Notice>();
        public List<Work> WorkList { get; set; } = new List<Work>();
        public List<File> FileList { get; set; } = new List<File>();
    }
}
