using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace LearnTHU.Model
{
    public sealed class UpdateBgTask : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            NotifyEventArgs args = new NotifyEventArgs("Undefined", 0, 0, 0);

            // TODO

            Notifer.Notify(args);
            _deferral.Complete();
        }
    }
}
