/* ****************************************************************************
 *  RuntimeObjectEditor
 * 
 * Copyright (c) 2005 Corneliu I. Tusnea
 * 
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the author be held liable for any damages arising from 
 * the use of this software.
 * Permission to use, copy, modify, distribute and sell this software for any 
 * purpose is hereby granted without fee, provided that the above copyright 
 * notice appear in all copies and that both that copyright notice and this 
 * permission notice appear in supporting documentation.
 * 
 * Corneliu I. Tusnea (corneliutusnea@yahoo.com.au)
 * www.acorns.com.au
 * ****************************************************************************/

using System.ComponentModel;
using System.Windows.Forms;
using RuntimeObjectEditor.PropertyGrid;

namespace RuntimeObjectEditor
{
    /// <summary>
    ///     RuntimeEditor - The editor with the properties window and the window finder
    /// </summary>
    public partial class RuntimeEditor : Form
    {
        #region Windows Form Designer generated code

        /// <summary>
        ///     Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Windows Form Designer generated code

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RuntimeEditor));
            this._propertyGrid = new XPropertyGrid();
            this._windowFinder = new RuntimeObjectEditor.WindowFinder();
            this._txtType = new System.Windows.Forms.TextBox();
            this._txtToString = new System.Windows.Forms.TextBox();
            this._btnCollapseAll = new System.Windows.Forms.Button();
            this._toolTip = new System.Windows.Forms.ToolTip(this.components);
            this._btnRefresh = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _propertyGrid
            // 
            this._propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._propertyGrid.CommandsActiveLinkColor = System.Drawing.SystemColors.ActiveCaption;
            this._propertyGrid.CommandsDisabledLinkColor = System.Drawing.SystemColors.ControlDark;
            this._propertyGrid.CommandsLinkColor = System.Drawing.SystemColors.ActiveCaption;
            this._propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this._propertyGrid.Location = new System.Drawing.Point(1, 44);
            this._propertyGrid.Name = "_propertyGrid";
            this._propertyGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this._propertyGrid.Size = new System.Drawing.Size(322, 309);
            this._propertyGrid.TabIndex = 0;
            this._propertyGrid.SelectRequest += new XPropertyGrid.SelectedObjectRequestHandler(this.propertyGrid_SelectRequest);
            // 
            // _windowFinder
            // 
            this._windowFinder.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("_windowFinder.BackgroundImage")));
            this._windowFinder.Location = new System.Drawing.Point(0, 4);
            this._windowFinder.Name = "_windowFinder";
// TODO: Code generation for '' failed because of Exception 'Invalid Primitive Type: System.IntPtr. Consider using CodeObjectCreateExpression.'.
            this._windowFinder.Size = new System.Drawing.Size(32, 32);
            this._windowFinder.TabIndex = 0;
            this._toolTip.SetToolTip(this._windowFinder, "WindowFinder - click & drag to select any .Net window from any process.");
            this._windowFinder.ActiveWindowChanged += new System.EventHandler(this.windowFinder_ActiveWindowChanged);
            this._windowFinder.ActiveWindowSelected += new System.EventHandler(this.windowFinder_ActiveWindowSelected);
            // 
            // _txtType
            // 
            this._txtType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._txtType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._txtType.Location = new System.Drawing.Point(32, 0);
            this._txtType.Name = "_txtType";
            this._txtType.ReadOnly = true;
            this._txtType.Size = new System.Drawing.Size(292, 20);
            this._txtType.TabIndex = 1;
            this._txtType.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this._toolTip.SetToolTip(this._txtType, "Object Type");
            // 
            // _txtToString
            // 
            this._txtToString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._txtToString.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._txtToString.Location = new System.Drawing.Point(32, 20);
            this._txtToString.Name = "_txtToString";
            this._txtToString.ReadOnly = true;
            this._txtToString.Size = new System.Drawing.Size(252, 20);
            this._txtToString.TabIndex = 2;
            this._txtToString.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this._toolTip.SetToolTip(this._txtToString, "Object.ToString()");
            // 
            // _btnCollapseAll
            // 
            this._btnCollapseAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnCollapseAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnCollapseAll.Location = new System.Drawing.Point(288, 22);
            this._btnCollapseAll.Name = "_btnCollapseAll";
            this._btnCollapseAll.Size = new System.Drawing.Size(16, 16);
            this._btnCollapseAll.TabIndex = 3;
            this._btnCollapseAll.Text = "-";
            this._toolTip.SetToolTip(this._btnCollapseAll, "Collapse All");
            this._btnCollapseAll.Click += new System.EventHandler(this.btnCollapseAll_Click);
            // 
            // _btnRefresh
            // 
            this._btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnRefresh.Location = new System.Drawing.Point(306, 22);
            this._btnRefresh.Name = "_btnRefresh";
            this._btnRefresh.Size = new System.Drawing.Size(16, 16);
            this._btnRefresh.TabIndex = 4;
            this._btnRefresh.Text = "*";
            this._toolTip.SetToolTip(this._btnRefresh, "Refresh");
            this._btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // RuntimeEditor
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(324, 354);
            this.Controls.Add(this._btnRefresh);
            this.Controls.Add(this._btnCollapseAll);
            this.Controls.Add(this._txtToString);
            this.Controls.Add(this._txtType);
            this.Controls.Add(this._windowFinder);
            this.Controls.Add(this._propertyGrid);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RuntimeEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Runtime Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
		
		
        private Button _btnCollapseAll;
        private Button _btnRefresh;

        private IContainer components;
        private XPropertyGrid _propertyGrid;
        private ToolTip _toolTip;
        private TextBox _txtToString;
        private TextBox _txtType;
        private WindowFinder _windowFinder;
    }
}