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
        static async void SaveData(List<Course> courses)
        {
            MainModel model = MainModel.Current;
            if (model == null) return;
            var vault = new Windows.Security.Credentials.PasswordVault();
            if (vault.RetrieveAll().Count == 0 || vault.FindAllByResource("LearnTHU")[0].Password == "") return;
            string userId = vault.FindAllByResource("LearnTHU")[0].UserName;
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.CreateFileAsync(userId, CreationCollisionOption.ReplaceExisting);
            JsonArray jsonArr = new JsonArray();
            foreach (Course c in courses)
            {
                jsonArr.Add(courseToJson(c));
            }
            string json = jsonArr.Stringify();
            await FileIO.WriteTextAsync(file, json);
        }

        private static IJsonValue courseToJson(Course c)
        {
            JsonObject jsonObj = new JsonObject();
            jsonObj["Id"] = JsonValue.CreateStringValue(c.Id);
            jsonObj["Name"] = JsonValue.CreateStringValue(c.Name);

            JsonArray notices = new JsonArray();
            foreach (Notice n in c.NoticeList)
            {
                notices.Add(noticeToJson(n));
            }
            jsonObj["Notices"] = notices;

            JsonArray files = new JsonArray();
            foreach (File f in c.FileList)
            {

            }
            jsonObj["Files"] = files;
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
            jsonObj["Url"] = JsonValue.CreateStringValue(f.Url);
            jsonObj["Name"] = JsonValue.CreateStringValue(f.Name);
            jsonObj["Note"] = JsonValue.CreateStringValue(f.Note);
            jsonObj["UploadDate"] = JsonValue.CreateStringValue(f.UploadDate.ToString());
            jsonObj["Status"] = JsonValue.CreateStringValue(f.Status.ToString());

            return jsonObj;
        }

        private static IJsonValue workToJson(Work w)
        {
            JsonObject jsonObj = new JsonObject();

            return jsonObj;
        }
    }
}
