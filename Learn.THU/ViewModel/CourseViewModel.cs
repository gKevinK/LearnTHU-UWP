using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LearnTHU.View;
using LearnTHU.Model;
using Windows.UI.Xaml;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Net;
using Windows.Web.Http;

namespace LearnTHU.ViewModel
{
    class CourseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainModel Model = null;
        public static CourseViewModel Current = null;

        public string CourseId;
        public Course Course;
        public string CourseName;

        public ObservableCollection<NoticeVM> NoticeList { get; private set; } = new ObservableCollection<NoticeVM>();
        public NoticeVM NoticeDetail { get; set; }
        public int NoticeNum { get { return Course.NewNoticeCount; } }
        public bool NoticeListLoaded;

        public ObservableCollection<FileVM> FileList { get; private set; } = new ObservableCollection<FileVM>();
        public FileVM FileDetail { get; set; }
        public int FileNum { get { return Course.NewFileCount; } }
        public bool FileListLoaded;

        public ObservableCollection<WorkVM> WorkList { get; private set; } = new ObservableCollection<WorkVM>();
        public WorkVM WorkDetail { get; set; }
        public int WorkNum { get { return Course.UnhandWorkCount; } }
        public bool WorkListLoaded;

        public CourseViewModel()
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

        public void ChangeCourse(string courseId)
        {
            if (courseId == CourseId)
            {
                return;
            }
            CourseId = courseId;
            Course = (Model.GetCourseList()).Find(c => c.Id == courseId);
            CourseName = Course.Name;
            NoticeList.Clear();
            NoticeListLoaded = false;
            FileList.Clear();
            FileListLoaded = false;
            WorkList.Clear();
            WorkListLoaded = false;
            UpdateNumbers();
            RaisePropertyChanged("CourseName");
        }

        public async Task PrepareNoticeList()
        {
            if (NoticeListLoaded) return;
            List<Notice> list = Model.GetNoticeList(CourseId);
            NoticeList.Clear();
            foreach (Notice notice in list)
            {
                NoticeList.Add(new NoticeVM(notice));
            }

            string id = CourseId;
            if (await Model.RefNoticeList(CourseId) == MainModel.UpdateResult.Success)
            {
                if (id == CourseId)
                {
                    NoticeList.Clear();
                    foreach (Notice notice in list)
                    {
                        NoticeList.Add(new NoticeVM(notice));
                    }
                }
            }
            NoticeListLoaded = true;
            RaisePropertyChanged("NoticeList");
        }

        public async Task ChangeNoticeDetail(NoticeVM noticeVM)
        {
            NoticeDetail = noticeVM;
            RaisePropertyChanged("NoticeDetail");
            if (noticeVM.IsRead == false)
            {
                noticeVM.IsRead = true;
                UpdateNumbers();
            }

            if (await Model.RefNotice(CourseId, noticeVM.Id) == MainModel.UpdateResult.Success)
            {
                RaisePropertyChanged("NoticeDetail");
            }
        }

        public async Task PrepareFileList()
        {
            if (FileListLoaded) return;
            List<File> list = Model.GetFileList(CourseId);
            FileList.Clear();
            foreach (File file in list)
            {
                FileList.Add(new FileVM(file));
            }

            string id = CourseId;
            if (await Model.RefFileList(CourseId) == MainModel.UpdateResult.Success)
            {
                if (id == CourseId)
                {
                    FileList.Clear();
                    foreach (File file in list)
                    {
                        FileList.Add(new FileVM(file));
                    }
                }
            }
            FileListLoaded = true;
            RaisePropertyChanged("FileList");
        }

        public async Task PrepareWorkList()
        {
            if (WorkListLoaded) return;
            List<Work> list = Model.GetWorkList(CourseId);
            WorkList.Clear();
            foreach (Work work in list)
            {
                WorkList.Add(new WorkVM(work));
            }

            string id = CourseId;
            if (await Model.RefWorkList(CourseId) == MainModel.UpdateResult.Success)
            {
                if (id == CourseId)
                {
                    WorkList.Clear();
                    foreach (Work work in list)
                    {
                        WorkList.Add(new WorkVM(work));
                    }
                }
            }
            WorkListLoaded = true;
            RaisePropertyChanged("WorkList");
        }

        public async Task ChangeWorkDetail(WorkVM workVM)
        {
            WorkDetail = workVM;
            if (workVM.Work.Content == null)
            {
                await Model.RefWork(CourseId, workVM.Work);
            }
            RaisePropertyChanged("WorkDetail");
        }

        public HttpRequestMessage GetHRM()
        {
            return Model.GetHRM(CourseId);
        }

        public void UpdateNumbers()
        {
            RaisePropertyChanged("NoticeNum");
            RaisePropertyChanged("FileNum");
            RaisePropertyChanged("WorkNum");
            MainViewModel.Current.RaiseCourseDataChanged(CourseId);
        }

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class NoticeVM : INotifyPropertyChanged
    {
        private Notice _notice;

        public event PropertyChangedEventHandler PropertyChanged;

        public Notice Notice { get { return _notice; } }
        public string Id { get { return _notice.Id; } }
        public string Title { get { return _notice.Title; } }
        public string Date { get { return $"{_notice.Date.Year}-{_notice.Date.Month}-{_notice.Date.Day}"; } }
        public string Publisher { get { return _notice.Publisher; } }
        public string Content { get { return _notice.Content == null ? string.Empty : _notice.Content; } }

        public bool IsRead
        {
            get { return _notice.IsRead; }
            set
            {
                _notice.IsRead = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRead"));
            }
        }

        public NoticeVM(Notice notice)
        {
            _notice = notice;
        }
    }

    public class FileVM : INotifyPropertyChanged
    {
        private File _file;

        public event PropertyChangedEventHandler PropertyChanged;

        public File File { get { return _file; } }
        public string Title { get { return _file.Name; } }
        public string Note { get { return _file.Note; } }
        public double FileSize { get { return _file.FileSize; } }
        public string UploadTime { get { return $"{_file.UploadDate.Year}-{_file.UploadDate.Month}-{_file.UploadDate.Day}"; } }

        public File.FileStatus Status
        {
            get { return _file.Status; }
            set
            {
                _file.Status = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Status"));
                }
                CourseViewModel.Current.UpdateNumbers();
            }
        }

        public FileVM(File file)
        {
            _file = file;
        }

        public async void Download()
        {
            await CourseViewModel.Current.Model.DownloadFile(File);
            await new Windows.UI.Popups.MessageDialog("下载完成").ShowAsync();
            Status = File.FileStatus.Downloaded;
            CourseViewModel.Current.UpdateNumbers();
        }
    }

    public class WorkVM : INotifyPropertyChanged
    {
        private Work _work;

        public event PropertyChangedEventHandler PropertyChanged;

        public Work Work { get { return _work; } }
        public string Title { get { return _work.Title; } }
        public string BeginDate { get { return $"{_work.BeginDate.Year}-{_work.BeginDate.Month}-{_work.EndDate.Day}"; } }
        public string EndDate { get { return $"{_work.EndDate.Year}-{_work.EndDate.Month}-{_work.EndDate.Day}"; } }
        public bool HaveAttachment { get { return _work.Attachment != null; } }
        public string Content { get { return _work.Content; } }
        public Work.WorkStatus Status
        {
            get { return _work.Status; }
            set
            {
                if (_work.Status != value)
                {
                    _work.Status = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Status"));
                    CourseViewModel.Current.UpdateNumbers();
                }
            }
        }

        public WorkVM(Work work)
        {
            _work = work;
        }
    }
}