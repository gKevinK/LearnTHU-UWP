using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;

namespace LearnTHU.Model
{
    public class SaveLoad
    {
        public static async Task SaveData(List<Course> courses)
        {
            MainModel model = MainModel.Current;
            if (model == null) return;
            var vault = new Windows.Security.Credentials.PasswordVault();
            string userId = vault.FindAllByResource("LearnTHU")[0].UserName;
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

            JsonArray jsonArr = new JsonArray();
            foreach (Course c in courses)
            {
                jsonArr.Add(courseToJson(c));
            }
            string json = jsonArr.Stringify();

            try
            {
                StorageFile file = await storageFolder.CreateFileAsync(userId + ".json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, json);
            }
            catch (Exception e)
            {
                await new Windows.UI.Popups.MessageDialog(e.Message).ShowAsync();
                return;
            }
            
        }

        private static IJsonValue courseToJson(Course c)
        {
            JsonObject jsonObj = new JsonObject();
            jsonObj["Id"] = JsonValue.CreateStringValue(c.Id);
            jsonObj["Name"] = JsonValue.CreateStringValue(c.Name);
            jsonObj["UpdateTime"] = JsonValue.CreateStringValue(c.UpdateTime.ToString());
            jsonObj["UpdateNoticeTime"] = JsonValue.CreateStringValue(c.UpdateNoticeTime.ToString());
            jsonObj["UpdateFileTime"] = JsonValue.CreateStringValue(c.UpdateFileTime.ToString());
            jsonObj["UpdateWorkTime"] = JsonValue.CreateStringValue(c.UpdateWorkTime.ToString());
            jsonObj["nnc"] = JsonValue.CreateNumberValue(c.NewNoticeCountOriginal);
            jsonObj["nfc"] = JsonValue.CreateNumberValue(c.NewFileCountOriginal);
            jsonObj["uwc"] = JsonValue.CreateNumberValue(c.UnhandWorkCountOriginal);

            JsonArray notices = new JsonArray();
            foreach (Notice n in c.NoticeList)
            {
                notices.Add(noticeToJson(n));
            }
            jsonObj["Notices"] = notices;

            JsonArray files = new JsonArray();
            foreach (File f in c.FileList)
            {
                files.Add(fileToJson(f));
            }
            jsonObj["Files"] = files;

            JsonArray works = new JsonArray();
            foreach (Work w in c.WorkList)
            {
                works.Add(workToJson(w));
            }
            jsonObj["Works"] = works;
            return jsonObj;
        }

        private static IJsonValue noticeToJson(Notice n)
        {
            JsonObject jsonObj = new JsonObject();
            jsonObj["Id"] = JsonValue.CreateStringValue(n.Id);
            jsonObj["Title"] = JsonValue.CreateStringValue(n.Title);
            jsonObj["Publisher"] = JsonValue.CreateStringValue(n.Publisher);
            jsonObj["Date"] = JsonValue.CreateStringValue(n.Date.ToString());
            jsonObj["IsRead"] = JsonValue.CreateBooleanValue(n.IsRead);
            jsonObj["Content"] = n.Content == null ? JsonValue.CreateNullValue() : JsonValue.CreateStringValue(n.Content);
            return jsonObj;
        }

        private static IJsonValue fileToJson(File f)
        {
            JsonObject jsonObj = new JsonObject();
            jsonObj["Id"] = JsonValue.CreateStringValue(f.Id);
            jsonObj["Name"] = JsonValue.CreateStringValue(f.Name);
            jsonObj["Note"] = JsonValue.CreateStringValue(f.Note);
            jsonObj["UploadDate"] = JsonValue.CreateStringValue(f.UploadDate.ToString());
            jsonObj["Status"] = JsonValue.CreateNumberValue((int)f.Status);
            jsonObj["GroupName"] = JsonValue.CreateStringValue(f.GroupName);

            jsonObj["FileSize"] = JsonValue.CreateNumberValue(f.FileSize);
            jsonObj["FileName"] = f.FileName == null ? JsonValue.CreateNullValue() : JsonValue.CreateStringValue(f.FileName);
            return jsonObj;
        }

        private static IJsonValue workToJson(Work w)
        {
            JsonObject jsonObj = new JsonObject();
            jsonObj["Id"] = JsonValue.CreateStringValue(w.Id);
            jsonObj["Title"] = JsonValue.CreateStringValue(w.Title);
            jsonObj["BeginTime"] = JsonValue.CreateStringValue(w.BeginTime.ToString());
            jsonObj["EndTime"] = JsonValue.CreateStringValue(w.EndTime.ToString());
            jsonObj["Content"] = w.Content == null ? JsonValue.CreateNullValue() : JsonValue.CreateStringValue(w.Content);
            jsonObj["Attachment"] = w.Attachment == null ? JsonValue.CreateNullValue() : workFileToJson(w.Attachment);
            jsonObj["Status"] = JsonValue.CreateNumberValue((int)w.Status);
            return jsonObj;
        }

        private static IJsonValue workFileToJson(WorkFile wf)
        {
            JsonObject jsonObj = new JsonObject();
            jsonObj["Name"] = JsonValue.CreateStringValue(wf.Name);
            jsonObj["Url"] = JsonValue.CreateStringValue(wf.Url);
            return jsonObj;
        }

        public static async Task LoadData(List<Course> courses)
        {
            courses.Clear();
            var vault = new Windows.Security.Credentials.PasswordVault();
            if (vault.RetrieveAll().Count == 0 || vault.FindAllByResource("LearnTHU")[0].Password == "") return;
            string userId = vault.FindAllByResource("LearnTHU")[0].UserName;
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.GetFileAsync(userId + ".json");
            if (file == null) return;
            string json = await FileIO.ReadTextAsync(file);
            JsonArray jsonArr = JsonArray.Parse(json);
            foreach (IJsonValue obj in jsonArr)
            {
                if (obj.ValueType == JsonValueType.Object)
                {
                    courses.Add(jsonToCourse(obj.GetObject()));
                }
            }
            return;
        }
        
        private static Course jsonToCourse(JsonObject jsonObj)
        {
            Course c = new Course();

            c.Id = jsonObj.GetNamedString("Id");
            c.Name = jsonObj.GetNamedString("Name");
            c.UpdateTime = DateTime.Parse(jsonObj.GetNamedString("UpdateTime"));
            c.UpdateNoticeTime = DateTime.Parse(jsonObj.GetNamedString("UpdateNoticeTime"));
            c.UpdateFileTime = DateTime.Parse(jsonObj.GetNamedString("UpdateFileTime"));
            c.UpdateWorkTime = DateTime.Parse(jsonObj.GetNamedString("UpdateWorkTime"));
            c.NewNoticeCountOriginal = (int)jsonObj.GetNamedNumber("nnc");
            c.NewFileCountOriginal = (int)jsonObj.GetNamedNumber("nfc");
            c.UnhandWorkCountOriginal = (int)jsonObj.GetNamedNumber("uwc");

            c.NoticeList = new List<Notice>();
            JsonArray noticeArr = jsonObj.GetNamedArray("Notices");
            foreach (IJsonValue n in noticeArr)
            {
                if (n.ValueType == JsonValueType.Object)
                {
                    c.NoticeList.Add(jsonToNotice(n.GetObject()));
                }
            }

            c.FileList = new List<File>();
            JsonArray fileArr = jsonObj.GetNamedArray("Files");
            foreach (IJsonValue f in fileArr)
            {
                if (f.ValueType == JsonValueType.Object)
                {
                    c.FileList.Add(jsonToFile(f.GetObject()));
                }
            }

            c.WorkList = new List<Work>();
            JsonArray workArr = jsonObj.GetNamedArray("Works");
            foreach (IJsonValue w in workArr)
            {
                if (w.ValueType == JsonValueType.Object)
                {
                    c.WorkList.Add(jsonToWork(w.GetObject()));
                }
            }
            return c;
        }

        private static Notice jsonToNotice(JsonObject jsonObj)
        {
            Notice n = new Notice()
            {
                Id = jsonObj.GetNamedString("Id"),
                Title = jsonObj.GetNamedString("Title"),
                Publisher = jsonObj.GetNamedString("Publisher"),
                Date = DateTime.Parse(jsonObj.GetNamedString("Date")),
                IsRead = jsonObj.GetNamedBoolean("IsRead"),
                Content = jsonObj.GetNamedString("Content", null),
            };
            return n;
        }

        private static File jsonToFile(JsonObject jsonObj)
        {
            File f = new File()
            {
                Id = jsonObj.GetNamedString("Id"),
                Name = jsonObj.GetNamedString("Name"),
                Note = jsonObj.GetNamedString("Note"),
                UploadDate = DateTime.Parse(jsonObj.GetNamedString("UploadDate")),
                Status = (File.FileStatus)(int)jsonObj.GetNamedNumber("Status"),
                GroupName = jsonObj.GetNamedString("GroupName"),
                FileSize = jsonObj.GetNamedNumber("FileSize"),
                FileName = jsonObj.GetNamedValue("FileName").ValueType == JsonValueType.Null ?
                    null : jsonObj.GetNamedString("FileName"),
            };
            return f;
        }

        private static Work jsonToWork(JsonObject jsonObj)
        {
            Work w = new Work()
            {
                Id = jsonObj.GetNamedString("Id"),
                Title = jsonObj.GetNamedString("Title"),
                BeginTime = DateTime.Parse(jsonObj.GetNamedString("BeginTime")),
                EndTime = DateTime.Parse(jsonObj.GetNamedString("EndTime")),
                Content = jsonObj.GetNamedString("Content", ""),
                Status = (Work.WorkStatus)(int)jsonObj.GetNamedNumber("Status"),
                Attachment = null,
            };
            if (jsonObj.GetNamedObject("Attachment", null) != null)
            {
                WorkFile wf = new WorkFile()
                {
                    Name = jsonObj.GetNamedObject("Attachment").GetNamedString("Name"),
                    Url = jsonObj.GetNamedObject("Attachment").GetNamedString("Url"),
                };
                w.Attachment = wf;
            }
            return w;
        }
    }
}
