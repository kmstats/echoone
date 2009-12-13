using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace com.echo.PartyMemberOM
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
            //LoginForm form1 = new LoginForm();
            //if (form1.ShowDialog() == DialogResult.OK)
            //{
            //    com.echo.PartyMemberOM.WebServices.User u = form1.CurUser;
            //    form1.Close();
            //    Application.Run(new mainForm(u));
            //}
            Application.Run(new mainForm(null));
        }
    }
}
