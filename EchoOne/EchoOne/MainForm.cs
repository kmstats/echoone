using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using com.echo.EchoOne.WebServices;
using com.echo.EchoOne.Properties;
using System.Deployment.Application;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace com.echo.EchoOne
{
    public partial class MainForm : Form
    {
        IntPtr appWin;

        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId", SetLastError = true,
             CharSet = CharSet.Unicode, ExactSpelling = true,
             CallingConvention = CallingConvention.StdCall)]
        private static extern long GetWindowThreadProcessId(long hWnd, long lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongA", SetLastError = true)]
        private static extern long GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongA", SetLastError = true)]
        private static extern long SetWindowLong(IntPtr hwnd, int nIndex, long dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long SetWindowPos(IntPtr hwnd, long hWndInsertAfter, long x, long y, long cx, long cy, long wFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

        [DllImport("user32.dll", EntryPoint = "PostMessageA", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hwnd, uint Msg, long wParam, long lParam);

        private const int SWP_NOOWNERZORDER = 0x200;
        private const int SWP_NOREDRAW = 0x8;
        private const int SWP_NOZORDER = 0x4;
        private const int SWP_SHOWWINDOW = 0x0040;
        private const int WS_EX_MDICHILD = 0x40;
        private const int SWP_FRAMECHANGED = 0x20;
        private const int SWP_NOACTIVATE = 0x10;
        private const int SWP_ASYNCWINDOWPOS = 0x4000;
        private const int SWP_NOMOVE = 0x2;
        private const int SWP_NOSIZE = 0x1;
        private const int GWL_STYLE = (-16);
        private const int WS_VISIBLE = 0x10000000;
        private const int WM_CLOSE = 0x10;
        private const int WS_CHILD = 0x40000000;


        public User CurUser;
        private com.echo.EchoOne.WebServices.EchoOneWS ws;

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(User user)
        {
            CurUser = user;
            InitializeComponent();
        }

        private void OnClose(object sender, EventArgs e)
        {
            Close();
        }

        private void OnShowFunctionTree(object sender, EventArgs e)
        {
            splitContainer1.Panel1Collapsed = !splitContainer1.Panel1Collapsed;
            acFunction.Image = tvFunction.Visible ? Resources.Kombine_toolbar_007 : Resources.Kombine_toolbar_025;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            OnCheckUpdate();

            ws = new EchoOneWS();

            GetCurUser();

            GetFunction();
        }

        private void GetFunction()
        {
            Function[] f = ws.GetFunctionByUserId(CurUser.UserId);
            foreach (Function item in f)
            {
                TreeNode[] nodes = tvFunction.Nodes.Find(item.FuncType1.Id.ToString(),true);

                TreeNode node = null;
                if (nodes.Length == 0)
                    node = tvFunction.Nodes.Add(item.FuncType1.Id.ToString(), item.FuncType1.Name);
                else
                    node = nodes[0];

                node.ImageIndex = item.FuncType1.Id;
                node.SelectedImageIndex = item.FuncType1.Id;

                TreeNode fNode;
                fNode = node.Nodes.Add(item.Id.ToString(), item.Name);
                fNode.ImageIndex = item.ImgIndex;
                fNode.SelectedImageIndex = item.ImgIndex;
            }
        }

        private void GetCurUser()
        {
            lbDept.Text = CurUser.UserInDept.Dept.DeptName;
            lbUser.Text = CurUser.UserName;
        }

        private static void OnCheckUpdate()
        {
            ApplicationDeployment ad;
            UpdateCheckInfo info;

            try
            {
                ad = ApplicationDeployment.CurrentDeployment;
            }
            catch (InvalidDeploymentException)
            {
                return;
            }
            
            info = ad.CheckForDetailedUpdate();
            Boolean doUpdate = true;

            if (!info.UpdateAvailable)
            {
                return;
            }


            if (info.IsUpdateRequired)
            {
                MessageBox.Show("软件有必须的更新，正在下载！");
            }
            else
            {
                if (MessageBox.Show("软件有更新，是否下载？", "更新提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    doUpdate = false;
                }
            }

            if (doUpdate)
            {
                ad.Update();
                MessageBox.Show("更新已经下载，重启动程序");
                Application.Restart();
            }
        }

        private void acFunction_Update(object sender, EventArgs e)
        {
            acFunction.Text = tvFunction.Visible ? "隐藏功能树" : "显示功能树";
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            RunApplication(@"D:\Program Files\Microsoft Office\OFFICE11\winword.EXE");
        }

        private void RunApplication(string appPath)
        {
            appWin = IntPtr.Zero;

            Process p = null;
            try
            {
                p = System.Diagnostics.Process.Start(appPath);
                p.WaitForInputIdle();
                appWin = p.MainWindowHandle;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error");
            }

            // Put it into this form
            SetParent(appWin, splitContainer1.Panel2.Handle);
            splitContainer1_Panel2_Resize(null, null);
            // Remove border and whatnot
            SetWindowLong(appWin, GWL_STYLE, WS_VISIBLE);

            // Move the window to overlay it on this window
            MoveWindow(appWin, 0, 0, splitContainer1.Panel2.Width, splitContainer1.Panel2.Height, true);
        }

        private void splitContainer1_Panel2_Resize(object sender, EventArgs e)
        {
            if (this.appWin != IntPtr.Zero)
            {
                MoveWindow(appWin, 0, 0, this.Width, this.Height, true);
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            RunApplication(@"D:\Users\echo\Documents\Visual Studio 2008\Projects\EchoOne\流动党员管理\bin\Debug\EchoPartyMemberOM.exe");
        }
    }
}
