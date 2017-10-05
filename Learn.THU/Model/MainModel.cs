using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;

namespace LearnTHU.Model
{
    class MainModel
    {
        static MainModel current;
        public static MainModel Current
        {
            get
            {
                if (current == null) current = new MainModel();
                return current;
            }
        }

        List<Course> courses;
        public List<Course> Courses { get; }

        public MainModel() { }


        private async Task Load()
        {
            var vault = new Windows.Security.Credentials.PasswordVault();
            if (vault.FindAllByResource("LearnTHU").Count == 0) return;
            string userId = vault.FindAllByResource("LearnTHU")[0].UserName;
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = (StorageFile)await storageFolder.TryGetItemAsync(userId + ".json");
            if (file == null) return;
            string json = await FileIO.ReadTextAsync(file);
            JsonObject jsonObj = JsonObject.Parse(json);
            JsonArray jsonArr = jsonObj.GetNamedArray("courses");
            foreach (IJsonValue obj in jsonArr) {
                if (obj.ValueType == JsonValueType.Object) {
                    courses.Add(Course.ParseJson(obj.GetObject()));
                }
            }
        }

        public async Task Save()
        {
            if (Courses.Count == 0) return;
            JsonObject jsonObj = new JsonObject();
            JsonArray jsonArr = new JsonArray();
            foreach (Course c in Courses) {
                jsonArr.Add(c.ToJsonObject());
            }
            jsonObj["courses"] = jsonArr;
            string json = jsonObj.Stringify();
            try {
                var vault = new Windows.Security.Credentials.PasswordVault();
                string userId = vault.FindAllByResource("LearnTHU")[0].UserName;
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = await storageFolder.CreateFileAsync(userId + ".json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, json);
            } catch (Exception e) {
                await new Windows.UI.Popups.MessageDialog(e.Message).ShowAsync();
                return;
            }
        }
    }
}
