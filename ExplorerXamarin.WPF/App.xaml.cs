using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ExplorerXamarin.WPF
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
#if DEBUG
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;
#endif

        private void Application_Startup(object sender, StartupEventArgs e)
        {
#if DEBUG
            AttachConsole(ATTACH_PARENT_PROCESS);
#endif
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Send);
            Dispatcher.Run();
        }
    }
}
