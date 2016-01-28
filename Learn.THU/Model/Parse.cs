﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LearnTHU.Model
{
    public static class Parse
    {
        public static List<Course> CourseListOld(string html)
        {
            List<Course> courseList = new List<Course>();
            string pattern = @"course_id=([0-9]+)"" target=""_blank"">
(.*)\(.+?\)</a>.+
.+text"">(.+)</span>.+
.+text"">(.+)</span>.+
.+text"">(.+)</span>";
            Regex regex = new Regex(pattern);
            foreach (Match match in regex.Matches(html))
            {
                Course course = new Course()
                {
                    Id = match.Groups[1].Value,
                    Name = match.Groups[2].Value.Trim(),
                    IsNewWebLearning = false,
                    IsActive = true,
                };
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
                    Title = Regex.Replace(match.Groups[2].Value, "<[^>]+>", ""),
                    Publisher = match.Groups[3].Value,
                    Date = DateTime.Parse(match.Groups[4].Value),
                    IsRead = match.Groups[5].Value == @"已读" ? true : false,
                };
                noticeList.Add(notice);
            }
            return noticeList;
        }

        public static Notice NoticeOld(string html)
        {
            Notice notice = new Notice();
            if (notice == null)
            {
                notice = new Notice();
            }
            Regex regex = new Regex(@"hidden;"">([\s\S]*?)&nbsp;[\s]+</td>");
            string text = regex.Match(html).Groups[1].Value;
            text = Regex.Replace(text, "<[^>]+>", "");
            text = WebUtility.HtmlDecode(text);
            notice.Content = text;
            return notice;
        }

        public static List<FileGroup> FileListOld(string html)
        {
            List<FileGroup> fileGroupList = new List<FileGroup>();
            Regex reg1 = new Regex(@"height=""26""  onClick=[\s\S]+?>(.+?)</td>");
            Regex reg2 = new Regex(@"<div class=""layerbox""([\s\S]+?)</div>");
            int groupCount = reg1.Matches(html).Count;
            for (int i = 0; i < groupCount; i += 1)
            {
                Match match1 = reg1.Matches(html)[i];
                Match match2 = reg2.Matches(html)[i];
                FileGroup fg = new FileGroup() { GroupName = match1.Groups[1].Value, Files = new List<File>() };
                Regex reg3 = new Regex(@"<a target=""_top"" href=""(.+?)"" >
(.+)
[\s\S]+?center"">(.*)</td>
.+center"">(.+)</td>
.+center"">(.+)</td>
.+center'>([\s\S]+?)</td>");
//                reg3 = new Regex(@"<a target=""_top"" href=""(.+?)"" >
//(.+)
//[\s\S]+?center"">(.*)</td>
//.+center"">(.+)</td>");
                foreach (Match m in reg3.Matches(match2.Groups[1].Value))
                {
                    fg.Files.Add(new File()
                    {
                        Url = @"http://learn.tsinghua.edu.cn" + m.Groups[1].Value,
                        Name = m.Groups[2].Value.Trim(),
                        Note = m.Groups[3].Value,
                        FileSize = FileSize(m.Groups[4].Value),
                        //UploadDate = DateTime.Parse(m.Groups[5].Value),
                        //Status = m.Groups[6].Value.Contains("新文件") ? File.FileStatus.Undownload : File.FileStatus.Downloaded,
                    });
                }
                fileGroupList.Add(fg);
            }
            return fileGroupList;
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