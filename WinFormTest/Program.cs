using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CLAP;

namespace WinFormTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Parser.RunWinForms<App>(args);
        }
    }

    class App
    {
        [Verb]
        static void Run(int count)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var forms = Enumerable.Range(1, count).Select(i => new Form1());

            forms.ToList().ForEach(f => f.Show());

            Application.Run(forms.Last());
        }
    }
}