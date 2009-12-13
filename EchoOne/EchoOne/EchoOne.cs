using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace com.echo.EchoOne
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            LoginForm form1 = new LoginForm();
            if  (form1.ShowDialog() == DialogResult.OK)
            {
                com.echo.EchoOne.WebServices.User u = form1.CurUser;
                form1.Close();
                Application.Run(new MainForm(u));
            }
        }
    }
}
