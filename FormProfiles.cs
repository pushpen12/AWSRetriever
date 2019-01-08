﻿using Amazon;
using AWSRetriver.Controls;
using CloudOps;
using Retriever.Model;
using System;
using System.Windows.Forms;

namespace Retriever
{
    public partial class FormProfiles : Form
    {
        private Profile profile;

        public Profile Profile { get => profile; set => profile = value; }

        public FormProfiles()
        {
            InitializeComponent();
            PopulateToolBar();            
        }

        private void PopulateToolBar()
        {
            foreach (RegionEndpoint region in RegionEndpoint.EnumerableAllRegions)
            {                
                ToolStripButton button = new ToolStripButton(region.SystemName);
                button.CheckOnClick = true;
                button.CheckState = CheckState.Checked;
                this.toolStrip.Items.Add(button);
                button.ToolTipText = String.Format("Toggle {0} for the entire system.", region.DisplayName);
                button.Tag = region.SystemName;
                button.AutoSize = true;
                button.Click += RegionButtoneClicked;
            }
        }

        private void RegionButtoneClicked(object sender, EventArgs e)
        {
            ToolStripButton button = (sender as ToolStripButton);
            string region = button.Tag.ToString();
            profile.EnableRegion(region, button.Checked);
            PopulateSelectedServiceOperations();
        }

        private void PopulateFromProfile()
        {
            foreach (string serviceName in this.profile.Services())
            {
                ListViewItem item = listViewServices.Items.Add(serviceName);
            }
            this.listViewServices.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            foreach (ProfileRecord profileRecord in this.profile)
            {
                ListViewItem item = listViewProfileRecords.Items.Add(profileRecord.ServiceName);                
                if (!profileRecord.Enabled)
                {
                    item.Checked = profileRecord.Enabled;
                }
                item.Checked = profileRecord.Enabled;
                item.Tag = profileRecord;
                item.SubItems.Add(profileRecord.Name);                                
                Operation op = Profile.FindOpeartion(profileRecord);
                if (op != null)
                {
                    string desc = op.Description;
                    string[] sentences = desc.Split('.');
                    string caption = desc;
                    if (sentences.Length > 0)
                    {
                        caption = sentences[0];
                    }
                    item.SubItems.Add(caption);
                    item.ToolTipText = desc;
                }
            }
            this.listViewProfileRecords.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void PopulateSelectedServiceOperations()
        {            
            if (this.listViewServices.SelectedItems.Count > 0)
            {
                ListViewItem item = listViewServices.SelectedItems[0];
                PopulateOperations(item.Text);                
            }            
        }

        private void PopulateOperations(string serviceName)
        {
            Cursor.Current = Cursors.WaitCursor;
            SuspendLayout();
            try
            {
                this.panelOperations.Controls.Clear();
                foreach (ProfileRecord p in this.profile)
                {
                    if (p.ServiceName == serviceName)
                    {
                        AddProfilePanel(p);
                    }
                }
            }
            finally
            {
                ResumeLayout();
                Cursor.Current = Cursors.Default;
            }
        }

        private void AddProfilePanel(ProfileRecord p)
        {                        
            Operation op = Profile.FindOpeartion(p);
            if (op != null)
            {
                ProfileRecordControl rc = new ProfileRecordControl();
                rc.Dock = DockStyle.Top;
                rc.AutoSize = true;
                rc.CheckBox.Checked = p.Enabled;                
                rc.LinkLabel.Text = op.Name;
                rc.LinkLabel.Tag = op.Description;
                string pageSize = p.PageSize.ToString();
                if (p.PageSize == 0) {                 
                    pageSize = Properties.Settings.Default.PageSize.ToString();
                }
                rc.TextPageSize.Text = pageSize;
                rc.LinkLabel.Click += LinkLabel_Click;                
                rc.RegionsTextbox.Regions = p.Regions;                
                panelOperations.Controls.Add(rc);
                rc.Tag = p;
                rc.Leave += Rc_Leave;
            }
        }

        private void Rc_Leave(object sender, EventArgs e)
        {
            ProfileRecordControl recordControl = sender as ProfileRecordControl;
            ProfileRecord p = recordControl.Tag as ProfileRecord;
            p.Enabled = recordControl.CheckBox.Checked;
            p.Regions = recordControl.RegionsTextbox.Regions;
            int pageSize;
            if (Int32.TryParse(recordControl.TextPageSize.Text, out pageSize))
            {
                p.PageSize = pageSize;
            }
            this.profile.Set(p);
        }

        private void LinkLabel_Click(object sender, EventArgs e)
        {
            this.txtDescription.Text = (sender as LinkLabel).Tag.ToString();
        }

        private void ListViewServices_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateSelectedServiceOperations();
        }

        private void ListViewProfileRecords_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            ProfileRecord profileRecord = e.Item.Tag as ProfileRecord;
            if (profileRecord != null)
            {
                profileRecord.Enabled = e.Item.Checked;
                this.profile.Set(profileRecord);
            }
        }

        private void FormProfiles_Load(object sender, EventArgs e)
        {
            PopulateFromProfile();
        }
    }
}