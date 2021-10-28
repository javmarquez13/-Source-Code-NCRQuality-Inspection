using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NCRQuality_Inspection
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //Debug
            //MainWindow wnd = new MainWindow(e.Args);                    
            //wnd.Show();


            Login_Application wnd = new Login_Application();
            wnd.Show();
        }
    }
}
