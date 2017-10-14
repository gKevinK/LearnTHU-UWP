using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Data.Json;

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
        public List<Assignment> WorkList { get; set; } = new List<Assignment>();
        public List<Courseware> FileList { get; set; } = new List<Courseware>();

        public Course() { }

        public static Course ParseHtml(string html)
        {
            Regex regex = new Regex("");
            Course course = new Course();

            throw new NotImplementedException();
        }

        public static List<Course> ParseHtmlList(string html)
        {
            List<Course> list = new List<Course>();
            string pattern = @"href=""(.+)"" target.+
(.*)\(.+?\)\(.+?\)</a>.+
.+text"">(.+)</span>.+
.+text"">(.+)</span>.+
.+text"">(.+)</span>";
            Regex regex = new Regex(pattern);
            foreach (Match match in regex.Matches(html)) {
                Course course = new Course() {
                    Name = match.Groups[2].Value.Trim()
                };
                string link = match.Groups[1].Value;
                if (link.Contains("course_id")) {
                    course.ID = Regex.Replace(link, ".+id=", "");
                } else {
                    course.ID = Regex.Replace(link, ".+coursehome/", "");
                }
                //course.InitNewCount(int.Parse(match.Groups[4].Value),
                //    int.Parse(match.Groups[5].Value), int.Parse(match.Groups[3].Value));
                course.UpdateTime = DateTime.Now;
                list.Add(course);
            }
            return list;
        }

        public static Course ParseJson(JsonObject json)
        {
            Course course = new Course();
            course.ID = json.GetNamedString("ID");
            course.Name = json.GetNamedString("Name");
            return course;
        }

        public JsonObject ToJsonObject()
        {
            var obj = new JsonObject {
                ["ID"] = JsonValue.CreateStringValue(ID),
                ["Name"] = JsonValue.CreateStringValue(Name),
            };
            return obj;
        }

        public static Course LoadJson(string json)
        {
            Course course = new Course();
            return course;
        }
    }
}
