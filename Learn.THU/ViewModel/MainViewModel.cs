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

        public MainModel Model = null;

        public static MainViewModel Current = null;

        public ObservableCollection<CourseVM> Courses { get; set; } = new ObservableCollection<CourseVM>();

        public MainViewModel()
        {
            if (MainModel.Current != null)
            {
                Model = MainModel.Current;
            }
            else
            {
                Model = new MainModel();
            }
            Current = this;
        }

        public async Task GetCourseList()
        {
            if (Model.Loaded == false)
                await Model.Load();
            var courses = await Model.GetCourseList();
            Courses.Clear();
            foreach (var course in courses)
            {
                Courses.Add(new CourseVM(course));
            }
            RaisePropertyChanged("Courses");
        }

        public void RaiseCourseDataChanged(string courseId)
        {
            var courseVM = Courses.First(c => c.Id == courseId);
            courseVM.RaisePropertyChanged();
        }

        public void RaisePropertyChanged(string propertyName = "Courses")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class CourseVM : INotifyPropertyChanged
    {
        private Course _course;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Id { get { return _course.Id; } }
        public string Label { get { return _course.Name; } }
        public int NewNotice { get { return _course.NewNoticeCount; } }
        public int NewFile { get { return _course.NewFileCount; } }
        public int NewWork { get { return _course.UnhandWorkCount; } }

        public CourseVM(Course course)
        {
            _course = course;
        }

        public void RaisePropertyChanged()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("NewNotice"));
                PropertyChanged(this, new PropertyChangedEventArgs("NewFile"));
                PropertyChanged(this, new PropertyChangedEventArgs("NewWork"));
            }
        }
    }
}
