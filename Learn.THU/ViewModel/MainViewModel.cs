using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearnTHU.Model;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;

namespace LearnTHU.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        MainModel model = null;

        public ObservableCollection<CourseVM> Courses { get; set; }

        public ObservableCollection<Notice> Notices { get; set; }

        public MainViewModel()
        {
            if (MainModel.Current != null)
            {
                model = MainModel.Current;
            }
            else
            {
                model = new MainModel();
            }
        }

        public async Task GetCourseList()
        {
            var courses = await model.GetCourseList();
            Courses = new ObservableCollection<CourseVM>();
            foreach (var course in courses)
            {
                Courses.Add(new CourseVM(course));
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Courses"));
            }
        }
    }

    public class CourseVM
    {
        private Course _course;
        public string Id { get { return _course.Id; } }
        public string Label { get { return _course.Name; } }
        public int NewNotice { get { return _course.NewNoticeCount; } }
        public int NewFile { get { return _course.NewFileCount; } }
        public int NewWork { get { return _course.UnhandWorkCount; } }

        public CourseVM(Course course)
        {
            _course = course;
        }
    }
}
