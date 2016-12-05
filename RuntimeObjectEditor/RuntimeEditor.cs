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

using System;
using System.Drawing;
using System.Windows.Forms;
using RuntimeObjectEditor.Utils;

namespace RuntimeObjectEditor
{
    /// <summary>
    ///     RuntimeEditor - The editor with the properties window and the window finder
    /// </summary>
    public partial class RuntimeEditor : Form
    {
        public RuntimeEditor()
        {
            InitializeComponent();
            Text = "RuntimeObjectEditor " + GetType().Assembly.GetName().Version.ToString(3);
        }

        private void propertyGrid_SelectRequest(object newObject)
        {
            ChangeSelectedObject(newObject);
        }

        private void windowFinder_ActiveWindowChanged(object sender, EventArgs e)
        {
            var selectedObject = _windowFinder.SelectedObject;

            ChangeSelectedObject(selectedObject);
        }

        private void windowFinder_ActiveWindowSelected(object sender, EventArgs e)
        {
            var selectedObject = _windowFinder.SelectedObject;

            if (ChangeSelectedObject(selectedObject) || (_windowFinder.SelectedHandle == IntPtr.Zero))
                return;

            if (NativeUtils.IsTargetInDifferentProcess(_windowFinder.SelectedHandle)
                /*&& this.windowFinder.IsManagedByClassName*/)
                RuntimeEditorHook.Hook(_windowFinder.SelectedHandle, Handle);
        }

        private bool ChangeSelectedObject(object selectedObject)
        {
            _propertyGrid.SelectedObject = selectedObject;
            if (selectedObject != null)
            {
                var ctl = selectedObject as Control;
                if (ctl != null)
                    Text = "Runtime Editor:" + ctl.Name;

                _txtType.Text = selectedObject.GetType().FullName;

                try
                {
                    _txtToString.Text = selectedObject.ToString();
                }
                catch (Exception ex)
                {
                    _txtToString.Text = "<ex:>" + ex.Message;
                }

                ShowTail(_txtType);
                ShowTail(_txtToString);
                return true;
            }
            _txtToString.Text = "";
            if (NativeUtils.IsTargetInDifferentProcess(_windowFinder.SelectedHandle))
            {
                _txtType.Text = _windowFinder.IsManagedByClassName
                    ? "<target in different process. release selection to hook>"
                    : "<target not in a managed process>";
            }
            else
            {
                if (_windowFinder.Window.IsValid)
                {
                    _txtType.Text = _windowFinder.Window.Text;
                    _txtToString.Text = "ClassName:" + _windowFinder.Window.ClassName;
                }
                else
                {
                    _txtType.Text = "<no selection or unknown window>";
                }
            }
            return false;
        }

        private void ShowTail(TextBox txtBox)
        {
            txtBox.SelectionStart = txtBox.Text.Length;
            txtBox.SelectionLength = 0;
        }

        #region OnLoad

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var top = SystemInformation.WorkingArea.Bottom - Height;
            Location = new Point(0, top);
        }

        #endregion

        private void btnCollapseAll_Click(object sender, EventArgs e)
        {
            _propertyGrid.Visible = false;
            _propertyGrid.CollapseAllGridItems();
            _propertyGrid.Visible = true;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            var oldSelectedObject = SelectedObject;
            _propertyGrid.Refresh();
            //this.SelectedObject = null;
        }

        #region Properties

        public object SelectedObject
        {
            get { return _propertyGrid.SelectedObject; }
            set { ChangeSelectedObject(value); }
        }

        internal IntPtr SelectedWindowHandle
        {
            get { return _windowFinder.SelectedHandle; }
            set { _windowFinder.SelectedHandle = value; }
        }

        #endregion
    }
}