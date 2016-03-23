using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace LearnTHU.Model
{
    public static class Parse
    {
        public static List<Course> CourseListOld(string html)
        {
            List<Course> courseList = new List<Course>();
            string pattern = @"href=""(.+)"" target=""_blank"">
(.*)\(.+?\)\(.+?\)</a>.+
.+text"">(.+)</span>.+
.+text"">(.+)</span>.+
.+text"">(.+)</span>";
            Regex regex = new Regex(pattern);
            foreach (Match match in regex.Matches(html))
            {
                Course course = new Course()
                {
                    Name = match.Groups[2].Value.Trim(), IsActive = true, NeedRefresh = true,
                };
                string link = match.Groups[1].Value;
                if (link.Contains("course_id"))
                {
                    course.Id = Regex.Replace(link, ".+id=", "");
                    course.IsNewWebLearning = false;
                }
                else
                {
                    course.Id = Regex.Replace(link, ".+coursehome/", "");
                    course.IsNewWebLearning = true;
                }
                
                course.InitNewCount(int.Parse(match.Groups[4].Value),
                    int.Parse(match.Groups[5].Value), int.Parse(match.Groups[3].Value));
                courseList.Add(course);
            }
            return courseList;
        }

        public static List<Notice> NoticeListOld(string html)
        {
            List<Notice> noticeList = new List<Notice>();
            string pattern = @"&id=(.+)&course.+'>(.+)</a>[\s\S]+?25>(.+)</[\s\S]+?25>(.+)</[\s\S]+?25>(.+)</td>";
            Regex regex = new Regex(pattern);
            foreach (Match match in regex.Matches(html))
            {
                Notice notice = new Notice
                {
                    Id = match.Groups[1].Value,
                    Title = WebUtility.HtmlDecode(Regex.Replace(match.Groups[2].Value, "<[^>]+>", "")),
                    Publisher = match.Groups[3].Value,
                    Date = DateTime.Parse(match.Groups[4].Value),
                    IsRead = match.Groups[5].Value == @"已读" ? true : false,
                };
                noticeList.Add(notice);
            }
            return noticeList;
        }

        public static List<Notice> NoticeListNew(string jsonString)
        {
            List<Notice> noticeList = new List<Notice>();
            JsonObject jsonObject = JsonObject.Parse(jsonString);
            string message = jsonObject.GetNamedString("message", "");
            if (message != "success")
            {
                throw new Exception("公告列表获取失败");
            }
            JsonArray recordList = jsonObject.GetNamedObject("paginationList").GetNamedArray("recordList", new JsonArray());
            foreach(IJsonValue record in recordList)
            {
                JsonObject courseNotice = record.GetObject().GetNamedObject("courseNotice");
                Notice notice = new Notice()
                {
                    Id = courseNotice.GetNamedNumber("id", 0).ToString(),
                    Title = courseNotice.GetNamedString("title", ""),
                    Date = DateTime.Parse(courseNotice.GetNamedString("regDate")),
                    Publisher = courseNotice.GetNamedString("owner", ""),
                    IsRead = record.GetObject().GetNamedString("status").Contains("1") ? true : false,
                };
                noticeList.Add(notice);
            }
            return noticeList;
        }

        public static Notice NoticeOld(string html)
        {
            Notice notice = new Notice();
            Regex regex = new Regex(@"hidden;"">([\s\S]*?)&nbsp;[\s]+</td>");
            string text = regex.Match(html).Groups[1].Value;
            // text = Regex.Replace(text, "<[^>]+>", "");
            // text = WebUtility.HtmlDecode(text);
            notice.Content = text;
            return notice;
        }

        public static Notice NoticeNew(string json)
        {
            Notice notice = new Notice();
            JsonObject jsonObject = JsonObject.Parse(json);
            string message = jsonObject.GetNamedString("message", "");
            if (message != "success")
            {
                throw new Exception();
            }
            notice.Content = jsonObject.GetNamedObject("dataSingle").GetNamedString("detail");
            return notice;
        }

        public static List<File> FileListOld(string html)
        {
            List<File> fileList = new List<File>();
            Regex reg1 = new Regex(@"height=""26""  onClick=[\s\S]+?>(.+?)</td>");
            Regex reg2 = new Regex(@"<div class=""layerbox""([\s\S]+?)</div>");
            int groupCount = reg1.Matches(html).Count;
            for (int i = 0; i < groupCount; i += 1)
            {
                Match match1 = reg1.Matches(html)[i];
                Match match2 = reg2.Matches(html)[i];
                string groupName = match1.Groups[1].Value;
                Regex reg3 = new Regex(@"<a target=""_top"" href=""(.+?)"" >
(.+)
[\s\S]+?center"">(.*)</td>
.+center"">(.+)</td>
.+center"">(.+)</td>
.+center'>([\s\S]+?)</td>");
                foreach (Match m in reg3.Matches(match2.Groups[1].Value))
                {
                    fileList.Add(new File()
                    {
                        Id = m.Groups[1].Value,
                        Name = m.Groups[2].Value.Trim(),
                        Note = m.Groups[3].Value,
                        GroupName = groupName,
                        FileSize = FileSize(m.Groups[4].Value),
                        UploadDate = DateTime.Parse(m.Groups[5].Value),
                        Status = m.Groups[6].Value.Contains("新文件") ? File.FileStatus.Undownload : File.FileStatus.Downloaded,
                    });
                }
            }
            return fileList;
        }

        public static List<File> FileListNew(string json)
        {
            List<File> files = new List<File>();
            JsonObject jsonObj = JsonObject.Parse(json);
            if (jsonObj.GetNamedString("message", "") != "success")
            {
                throw new Exception("文件列表获取失败");
            }
            if (jsonObj.GetNamedObject("resultList").Values.Count == 0)
            {
                return files;
            }
            JsonObject node = jsonObj.GetNamedObject("resultList").Values.First().GetObject().GetNamedObject("childMapData");
            foreach (IJsonValue group in node.Values)
            {
                if (group.ValueType == JsonValueType.Object)
                {
                    JsonObject groupObj = group.GetObject();
                    string groupName = groupObj.GetNamedObject("courseOutlines").GetNamedString("title");
                    JsonArray fileArr = groupObj.GetNamedArray("courseCoursewareList");
                    foreach (IJsonValue fileJson in fileArr)
                    {
                        if (fileJson.ValueType == JsonValueType.Object)
                        {
                            JsonObject fileObj = fileJson.GetObject();
                            files.Add(new File() {
                                Id = fileObj.GetNamedObject("resourcesMappingByFileId").GetNamedString("fileId"),
                                Name = fileObj.GetNamedString("title"),
                                Note = fileObj.GetNamedValue("detail").ValueType == JsonValueType.Null ?
                                    "" : fileObj.GetNamedString("detail"),
                                Status = fileObj.GetNamedObject("resourcesMappingByFileId").GetNamedNumber("resourcesStatus") == 1 ?
                                    File.FileStatus.Undownload : File.FileStatus.Downloaded,
                                GroupName = groupName,
                                FileName = fileObj.GetNamedObject("resourcesMappingByFileId").GetNamedString("fileName"),
                                FileSize = Double.Parse(fileObj.GetNamedObject("resourcesMappingByFileId").GetNamedString("fileSize")),
                                UploadDate = new DateTime(1970, 1, 1).AddMilliseconds(
                                    fileObj.GetNamedObject("resourcesMappingByFileId").GetNamedNumber("regDate")),
                            });
                        }
                    }
                }
            }
            return files;
        }

        public static List<Work> WorkListOld(string html)
        {
            List<Work> workList = new List<Work>();
            string pattern = @"hom_wk_detail[\.]jsp[?](.+)"">(.+)</a></td>
.+10%"">(.+)</td>
.+10%"">(.+)</td>
.+15%"" >([\s\S]+?)</td>";
            Regex regex = new Regex(pattern); ;
            foreach (Match match in regex.Matches(html))
            {
                Work work = new Work()
                {
                    Id = match.Groups[1].Value,
                    Title = WebUtility.HtmlDecode(match.Groups[2].Value),
                    BeginTime = DateTime.Parse(match.Groups[3].Value),
                    EndTime = DateTime.Parse(match.Groups[4].Value).AddHours(23).AddMinutes(59),
                    Status = match.Groups[5].Value.Trim() == "已经提交" ? Work.WorkStatus.Submitted : Work.WorkStatus.Unhand,
                };
                workList.Add(work);
            }
            return workList;
        }

        public static void WorkOld(string html, ref Work work)
        {
            Regex regex = new Regex(@"wrap=VIRTUAL>(.*?)</textarea>[\s\S+?]&nbsp;([\s\S]+?)</td>");
            work.Content = regex.Match(html).Groups[1].Value;
            if (regex.Match(html).Groups[2].Value.Contains("无相关文件"))
            {
                string input = regex.Match(html).Groups[2].Value;
                Regex regex2 = new Regex(@"href=""(.+)"">(.+)</a>");
                Match match = regex2.Match(input);
                work.Attachment = new WorkFile()
                {
                    Name = match.Groups[2].Value,
                    Url = @"http://learn.tsinghua.edu.cn" + match.Groups[1].Value,
                };
            }
        }

        private static double FileSize(string input)
        {
            char unit = input.Last();
            double size = double.Parse(input.Replace(unit, ' '));
            switch (unit)
            {
                case 'B':
                    break;
                case 'K':
                    size *= 1000;
                    break;
                case 'M':
                    size *= 1e6;
                    break;
                case 'G':
                    size *= 1e9;
                    break;
                default:
                    size = double.NaN;
                    break;
            }
            return size;
        }
    }
}
