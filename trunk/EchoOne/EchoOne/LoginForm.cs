using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using com.echo.EchoOne.WebServices;

namespace com.echo.EchoOne
{
    public partial class LoginForm : Form
    {
        private com.echo.EchoOne.WebServices.EchoOneWS ws;
        public User CurUser;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            ws = new com.echo.EchoOne.WebServices.EchoOneWS();
            cbDepts.Items.Clear();
            cbDepts.Items.AddRange(ws.GetDepts());
        }

        private void cbRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbUser.Items.Clear();
            cbUser.Items.AddRange(ws.GetUserByDept(cbDepts.Text));
        }

        private void tbPassword_KeyUp(object sender, KeyEventArgs e)
        {
            btnLogin.Enabled = tbPassword.Text.Length > 0;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (ws.ValidUser(cbUser.Text, tbPassword.Text))
            {
                CurUser = ws.GetUserByName(cbUser.Text);
                lbMsg.Visible = false;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                tbPassword.Focus();
                tbPassword.SelectAll();
                lbMsg.Visible = true;
            }
        }
    }
}
