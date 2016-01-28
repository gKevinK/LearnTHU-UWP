using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnTHU.Model
{
    class Model
    {
        public List<Course> CourseList { get; set; }

        Web web = new Web();

        public async Task<RefreshResult> RefreshAllData()
        {
            // TODO
            return RefreshResult.Success;
        }

        public async Task<RefreshResult> RefreshCourseList()
        {
            // TODO
            return RefreshResult.Success;
        }

        public async Task<RefreshResult> RefreshCourse(string courseId)
        {
            // TODO
            return RefreshResult.Success;
        }


        public enum RefreshResult
        {
            Success, Failed, Error
        }
    }
}
