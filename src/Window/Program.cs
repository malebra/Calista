using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calista.MainWindow
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            Application.Run(new MainWindow());
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.ToLower().Contains("fireplaysupport"))
            {
                try
                {
                    var ass = Assembly.GetExecutingAssembly();
                    var sss = ass.GetManifestResourceNames();
                    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Calista.MainWindow.Calista.FireplaySupport.dll"))
                    {
                        byte[] data = new byte[stream.Length];
                        stream.Read(data, 0, data.Length);
                        return Assembly.Load(data);
                    }
                }
                catch (Exception)
                {
                    return null;
                } 
            }
            return null;
        }
    }
}
