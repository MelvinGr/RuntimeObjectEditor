/* ****************************************************************************
 * RuntimeObjectEditor
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


using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using RuntimeObjectEditor.PropertyGrid.Tabs.Events;
using RuntimeObjectEditor.PropertyGrid.Tabs.Fields;
using RuntimeObjectEditor.PropertyGrid.Tabs.Methods;
using RuntimeObjectEditor.PropertyGrid.Tabs.ProcessInfo;
using RuntimeObjectEditor.PropertyGrid.Tabs.Properties;
using RuntimeObjectEditor.Utils;

namespace RuntimeObjectEditor.PropertyGrid
{
    /// <summary>
    ///     Summary description for XPropertyGrid.
    /// </summary>
    public class XPropertyGrid : System.Windows.Forms.PropertyGrid
    {
        public delegate void SelectedObjectRequestHandler(object newObject);

        private readonly ArrayList _historyObjects = new ArrayList();

        private int _activeObject = -1;

        private ContextMenuStrip _contextMenu;
        private ToolStripMenuItem _goBackOneItem;
        private ToolStripMenuItem _goForwardOneItem;
        private ToolStripMenuItem _selectThisItem;

        public XPropertyGrid()
        {
            InitializeComponent();
        }

        public event SelectedObjectRequestHandler SelectRequest;

        #region Context Menu

        private void InitContextMenu()
        {
            _contextMenu = new ContextMenuStrip(components);
            _selectThisItem = new ToolStripMenuItem();
            _goBackOneItem = new ToolStripMenuItem();
            _goForwardOneItem = new ToolStripMenuItem();
            _contextMenu.SuspendLayout();

            // 
            // contextMenu
            // 
            _contextMenu.Items.AddRange(new ToolStripItem[]
            {
                _selectThisItem,
                _goBackOneItem,
                _goForwardOneItem
            });
            _contextMenu.Name = "contextMenu";
            _contextMenu.Size = new Size(126, 70);
            // 
            // selectThisItem
            // 
            _selectThisItem.Name = "selectThisItem";
            _selectThisItem.Size = new Size(125, 22);
            _selectThisItem.Text = "Select";
            _selectThisItem.Click += selectThisItem_Click;
            // 
            // goBackOneItem
            // 
            _goBackOneItem.Name = "goBackOneItem";
            _goBackOneItem.Size = new Size(125, 22);
            _goBackOneItem.Text = "Back";
            _goBackOneItem.Click += goBackOneItem_Click;
            // 
            // goForwardOneItem
            // 
            _goForwardOneItem.Name = "goForwardOneItem";
            _goForwardOneItem.Size = new Size(125, 22);
            _goForwardOneItem.Text = "Forward";
            _goForwardOneItem.Click += goForwardOneItem_Click;
            _contextMenu.ResumeLayout(false);
        }

        #endregion

        protected override void OnCreateControl()
        {
            DrawFlatToolbar = true;
            HelpVisible = true;

            PropertySort = PropertySort.Alphabetical;

            InitContextMenu();

            ContextMenuStrip = _contextMenu;
            _goBackOneItem.Enabled = false;
            _goForwardOneItem.Enabled = false;

            base.OnCreateControl();

            // Add New Tabs here
            PropertyTabs.AddTabType(typeof(AllPropertiesTab));
            PropertyTabs.AddTabType(typeof(AllFieldsTab));
            PropertyTabs.AddTabType(typeof(InstanceEventsTab));
            PropertyTabs.AddTabType(typeof(MethodsTab));
            PropertyTabs.AddTabType(typeof(ProcessInfoTab));

            _historyObjects.Clear();
        }

        private void XPropertyGrid_PropertyTabChanged(object s, PropertyTabChangedEventArgs e)
        {
        }

        #region Component Designer generated code

        /// <summary>
        ///     Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

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

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            SuspendLayout();
            PropertyTabChanged += new PropertyTabChangedEventHandler(XPropertyGrid_PropertyTabChanged);
            ResumeLayout(false);
        }

        #endregion

        #region Properties

        public override bool CanShowCommands => true;

        public override bool CommandsVisibleIfAvailable
        {
            get { return true; }
            set { base.CommandsVisibleIfAvailable = value; }
        }

        #endregion

        #region Navigation

        protected override void OnSelectedObjectsChanged(EventArgs e)
        {
            // put in history
            if (SelectedObject != null)
                if (!_historyObjects.Contains(SelectedObject))
                {
                    if (_activeObject < _historyObjects.Count - 1)
                        _historyObjects.RemoveRange(_activeObject + 1, _historyObjects.Count - _activeObject - 1);
                    _activeObject = _historyObjects.Add(SelectedObject);
                    _goBackOneItem.Enabled = true;
                    _goForwardOneItem.Enabled = false;

                    if (_historyObjects.Count > 10)
                        _historyObjects.RemoveRange(0, _historyObjects.Count - 10);
                }
                else
                {
                    _activeObject = _historyObjects.IndexOf(SelectedObject);
                }

            base.OnSelectedObjectsChanged(e);
        }

        private void selectThisItem_Click(object sender, EventArgs e)
        {
            var selectedGridItem = SelectedGridItem;
            if (selectedGridItem == null)
                return;

            var value = selectedGridItem.Value;
            var valueHolder = value as IRealValueHolder;
            if (valueHolder != null)
                value = valueHolder.RealValue;
            InvokeSelectRequest(value);
        }

        private void goBackOneItem_Click(object sender, EventArgs e)
        {
            if (_activeObject > 0)
            {
                _activeObject--;
                _goForwardOneItem.Enabled = true;
            }
            else
            {
                _goBackOneItem.Enabled = false;
            }
            InvokeSelectRequest();
        }

        private void goForwardOneItem_Click(object sender, EventArgs e)
        {
            if (_activeObject < _historyObjects.Count)
            {
                _activeObject++;
                _goBackOneItem.Enabled = true;
            }
            else
            {
                _goForwardOneItem.Enabled = false;
            }
            InvokeSelectRequest();
        }

        private object GetActiveObject()
        {
            if ((_activeObject >= 0) && (_activeObject < _historyObjects.Count))
                return _historyObjects[_activeObject];
            return null;
        }

        #endregion

        #region Invoke

        private void InvokeSelectRequest()
        {
            SelectRequest?.Invoke(GetActiveObject());
        }

        private void InvokeSelectRequest(object newOBject)
        {
            SelectRequest?.Invoke(newOBject);
        }

        #endregion
    }
}