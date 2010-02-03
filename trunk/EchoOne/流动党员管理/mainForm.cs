using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace com.echo.PartyMemberOM
{
    public partial class mainForm : Form
    {
        private com.echo.PartyMemberOM.WebServices.User curUser;
        System.Windows.Forms.CheckBox cbJL;
        com.echo.Controls.ComboBoxTree cbPartyType;
        com.echo.DB.IDataProvider myDb;
        
        public mainForm()
        {


            InitializeComponent();
            AddControls();
        }

        private void AddControls()
        {
            //加入控件 级联
            cbJL = new System.Windows.Forms.CheckBox();
            cbJL.BackColor = Color.Transparent;
            cbJL.Text = "是否级联";
            cbJL.RightToLeft = RightToLeft.Yes;
            cbJL.CheckStateChanged += new EventHandler(GetPartyByOrgID);
            ToolStripControlHost h = new ToolStripControlHost(cbJL);
            mainToolBar.Items.Insert(2, h);

            //加入控件 分类
            cbPartyType = new com.echo.Controls.ComboBoxTree();
            cbPartyType.BranchSeparator = ".";
            cbPartyType.Width = 400;
            cbPartyType.AutoSize = true;
            cbPartyType.AbsoluteChildrenSelectableOnly = false;
            cbPartyType.AfterSelect += cbPartyType_AfterSelect;
            h = new ToolStripControlHost(cbPartyType);
            mainToolBar.Items.Insert(4, h);
        }

        public mainForm(com.echo.PartyMemberOM.WebServices.User u)
        {
            curUser = u;
            InitializeComponent();
            AddControls();

            System.Diagnostics.Process p = System.Diagnostics.Process.Start("regsvr32.exe", " /s kdbole6.dll");
            p.WaitForInputIdle();//等待执行注册命令完毕  
        }

        private void OnExit(object sender, EventArgs e)
        {
            Close();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            this.Update();
            FillOrg();
            GetColumnsHeaderText();
            GetPartyType();
        }

        /// <summary>
        // 填充组织树（初始），含根节点和跟节点的子节点
        /// </summary>
        private void FillOrg()
        {
            try 
	        {	        
        		D01TableAdapter.FillByTopOrg(XT2007DataSet.D01);

                TreeNode root = null;
                if (XT2007DataSet.D01 != null)
                {
                    XT2007DataSet.D01Row r = XT2007DataSet.D01.Rows[0] as XT2007DataSet.D01Row;
                    root = orgTree.Nodes.Add(r.D0107,r.D0101);                                             //增加根组织
                    root.Name = r.D0107;
                    root.ToolTipText = "(" + r.D0107 + ")" + r.D0101;
                }

                if (root != null)
                {
                    D01TableAdapter.FillByPID(XT2007DataSet.D01,root.Name);

                    if (XT2007DataSet.D01 != null)
                    {
                        foreach (XT2007DataSet.D01Row row in XT2007DataSet.D01)
                        {
                            TreeNode subNode = root.Nodes.Add(row.D0107, row.D0101);
                            subNode.Name = row.D0107;
                            subNode.ToolTipText = "(" + row.D0107 + ")" + row.D0101;
                        }
                    }
                }
	        }
	        catch (InvalidOperationException ex)
	        {

                System.Diagnostics.Process p = System.Diagnostics.Process.Start("regsvr32.exe", " kdbole6.dll");
                p.WaitForInputIdle();//等待执行注册命令完毕  
	        }
        }

        /// <summary>
        // 增加子节点的子节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void orgTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            foreach (TreeNode node in e.Node.Nodes)
            {
                if (node.Nodes.Count == 0)
                {
                    D01TableAdapter.FillByPID(XT2007DataSet.D01, node.Name);
                    if (XT2007DataSet.D01.Rows.Count > 0)
                    {
                        foreach (XT2007DataSet.D01Row row in XT2007DataSet.D01.Rows)
                        {
                            TreeNode subNode = node.Nodes.Add(row.D0107, row.D0101);
                            subNode.Name = row.D0107;
                            subNode.ToolTipText = "(" + row.D0107 + ")" + row.D0101;
                        }
                    }
                }
            }
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        // 显示流动党员信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void orgTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            GetPartyByOrgID(null, null);
        }

        private void ShowInfo()
        {
            if (orgTree.SelectedNode != null)
            {
                lbOrg.Text = "当前组织：" + orgTree.SelectedNode.Name + " " + orgTree.SelectedNode.Text;
                lbCount.Text = "记录数：" + gvParty.RowCount.ToString();
                lbSelectCount.Text = "选中：" + gvParty.SelectedRows.Count.ToString();
            }
        }

        /// <summary>
        // 切换显示组织树
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShiftOrgTree(object sender, EventArgs e)
        {
            acOrgTree.Image = mainPanel.Panel1Collapsed ? Properties.Resources.Kombine_toolbar_007 : Properties.Resources.Kombine_toolbar_025;
            mainPanel.Panel1Collapsed = !mainPanel.Panel1Collapsed;
        }

        private void acOrgTree_Update(object sender, EventArgs e)
        {
            acOrgTree.Text = mainPanel.Panel1Collapsed ? "显示组织树(&O)" : "隐藏组织树(&O)";
            acOrgTree.ToolTipText = acOrgTree.Text;
        }

        private void GetPartyType()
        {
            KZ38TableAdapter.Fill(XT2007DataSet.KZ38);
            foreach (XT2007DataSet.KZ38Row row in XT2007DataSet.KZ38)
            {
                TreeNode node = null;
                try
                {
                    node = cbPartyType.Nodes.Find(row.CODE.ToString().Substring(0, 1), true)[0];
                }
                catch (IndexOutOfRangeException)
                {
                }

                if (node == null)
                {
                    node = cbPartyType.Nodes.Add(row.CODE, row.CODE_NAME);
                    node.Name = row.CODE;
                }
                else
                {
                    TreeNode subNode = node.Nodes.Add(row.CODE, row.CODE_NAME);
                    subNode.Name = row.CODE;
                }
            }
        }

        /// <summary>
        //取得党员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetPartyByOrgID(object sender, EventArgs e)
        {
            if (orgTree.SelectedNode != null)
            {
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    if (cbJL.Checked)
                    {
                        A01TableAdapter.FillByOrgIDLike(XT2007DataSet.A01, orgTree.SelectedNode.Name); //级联
                    }
                    else
                    {
                        A01TableAdapter.FillByOrgID(XT2007DataSet.A01, orgTree.SelectedNode.Name); //非级联
                    }
                }
                catch (Exception)
                {
                    throw;
                }

                if (cbPartyType.SelectedNode != null)
                    cbPartyType_AfterSelect(sender, e);

                Cursor.Current = Cursors.Default;
            }

            lbJL.Text = cbJL.Checked ? "级联" : "不级联";
        }

        private void cbPartyType_AfterSelect(object sender, EventArgs e)
        {
            DataView dv = XT2007DataSet.A01.DefaultView;
            dv.RowFilter = "C0921 Like '" + cbPartyType.SelectedNode.Name + "*'";
            gvParty.DataSource = dv;
        }

        private void gvParty_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ShowInfo();
            gvParty.AutoResizeColumns();
            gvParty.AllowUserToResizeColumns = true;
        }

        /// <summary>
        //  取得字段中文名
        /// </summary>
        private void GetColumnsHeaderText()
        {
            List<string> l = new List<string>();

            foreach (DataGridViewColumn row in gvParty.Columns)
            {
                row.Name = row.DataPropertyName;
                l.Add(row.Name);
            }

            int i = 0;
            foreach (string colName in l)
            {
                T_DICT_COLUMNTableAdapter.FillByCname(XT2007DataSet.T_DICT_COLUMN, colName);
                XT2007DataSet.T_DICT_COLUMNRow r = XT2007DataSet.T_DICT_COLUMN.Rows[0] as XT2007DataSet.T_DICT_COLUMNRow;

                try
                {
                    if ((r.REFTABLE != "") && (r.REFTABLE.StartsWith("K") || r.REFTABLE.StartsWith("Z") || r.REFTABLE.StartsWith("G")))
                    {
                        AddComboBoxColumns(r.REFTABLE, r.CNAME, i);
                    }
                }
                catch (StrongTypingException)
                {
                    
                }
                i += 1;
                gvParty.Columns[colName].HeaderText = r.CAPTION;
            }
        }

        private void gvParty_SelectionChanged(object sender, EventArgs e)
        {
            lbSelectCount.Text = "选中：" + gvParty.SelectedRows.Count.ToString();
        }

        private void AddComboBoxColumns(string reftable, string fieldName, int index)
        {
            myDb = com.echo.DB.DataProvider.CreateDataProvider(com.echo.DB.DataProvider.DataProviderType.KingbaseProvider);
            DataGridViewComboBoxColumn comboboxColumn;
            comboboxColumn = new DataGridViewComboBoxColumn();
            {
                comboboxColumn.DataPropertyName = fieldName;
                comboboxColumn.HeaderText = fieldName;
            }

            comboboxColumn.DataSource = myDb.RetriveDataSet("select code,code_name from " + reftable).Tables[0];
            comboboxColumn.ValueMember = "code";
            comboboxColumn.DisplayMember = "code_name";
            comboboxColumn.Name = fieldName;
            
            comboboxColumn.FlatStyle = FlatStyle.Flat;
            comboboxColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            comboboxColumn.SortMode = DataGridViewColumnSortMode.Automatic;

            gvParty.Columns.Remove(fieldName);
            gvParty.Columns.Insert(index,comboboxColumn);
        }

        private void gvParty_Sorted(object sender, EventArgs e)
        {
            gvParty_DataBindingComplete(sender, null);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lbTime.Text = DateTime.Now.ToString();
        }

        private void acQuery_Update(object sender, EventArgs e)
        {
            acQuery.Enabled = tbName.Text.Length > 0 && gvParty.RowCount>0;
        }

        private void acQuery_Execute(object sender, EventArgs e)
        {
            int i = IsChinese(tbName.Text) ? 1 : 0;
            int curRowindex = gvParty.SelectedRows[0].Index;
            Boolean find = false;

            switch (i)
            {
                case 0:     //简拼
                    foreach (DataGridViewRow row in gvParty.Rows)
                    {
                        if (row.Index > curRowindex && row.Cells["AKKY"].Value.ToString() == tbName.Text)
                        {
                            gvParty.ClearSelection();
                            row.Selected = true;
                            gvParty.FirstDisplayedScrollingRowIndex = row.Index;
                            find = true;
                            break;
                        }
                    }
                    if (!find)
                    {
                        if (MessageBox.Show("没有找到，是否从头查找？", "查询", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            gvParty.ClearSelection();
                            gvParty.Rows[0].Selected = true;
                            acQuery_Execute(sender, e);
                        }
                    }
                    break;
                case 1:     //姓名
                    foreach (DataGridViewRow row in gvParty.Rows)
                    {
                        if (row.Index > curRowindex && row.Cells["A0101"].Value.ToString() == tbName.Text)
                        {
                            gvParty.ClearSelection();
                            row.Selected = true;
                            gvParty.FirstDisplayedScrollingRowIndex = row.Index;
                            find = true;
                            break;
                        }
                    }
                    if (!find)
                    {
                        if (MessageBox.Show("没有找到，是否从头查找？", "查询", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            gvParty.ClearSelection();
                            gvParty.Rows[0].Selected = true;
                            acQuery_Execute(sender, e);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private Boolean IsChinese(string s)
        {
            Regex r = new Regex(@"^[\u4e00-\u9fa5]+$");
            Boolean result = r.IsMatch(s) ? true : false;
            return result;
        }
    }
}
