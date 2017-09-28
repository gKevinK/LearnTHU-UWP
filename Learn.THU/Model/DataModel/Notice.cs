using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnTHU.Model
{
    class Notice
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Publisher { get; set; }
        public DateTime Date { get; set; }
        public bool IsRead { get; set; }
        public string Content { get; set; }
    }
}
