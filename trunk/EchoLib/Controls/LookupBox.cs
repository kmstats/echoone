using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace com.echo.Controls
{
    [System.ComponentModel.LookupBindingProperties("DataSource","DisplayMember","ValueMember","LookupMember")]
    public partial class LookupBox : UserControl
    {
        public object DataSource
        {
            get { return comboBox1.DataSource; }
            set { comboBox1.DataSource = value; }
        }

        public string DisplayMember
        {
            get { return comboBox1.DisplayMember; }
            set { comboBox1.DisplayMember = value; }
        }

        public string ValueMember
        {
            get { return comboBox1.ValueMember; }
            set { comboBox1.ValueMember = value; }
        }

        public string LookupMember
        {
            get { return comboBox1.SelectedValue.ToString(); }
            set { comboBox1.SelectedValue = value; }
        }

        public LookupBox()
        {
            InitializeComponent();
        }
    }
}
