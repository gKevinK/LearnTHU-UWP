using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearnTHU.Model;

namespace LearnTHU.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        MainModel model = new MainModel();

        //CourseList

        public MainViewModel()
        {

        }
    }
}
