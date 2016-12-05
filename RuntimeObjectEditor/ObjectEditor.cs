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
using System.Diagnostics;
using System.Windows.Forms;
using RuntimeObjectEditor.Hotkey;

namespace RuntimeObjectEditor
{
    /// <summary>
    ///     Singleton class that takes care of showing the Runtime ObjectEditor form with it's window finder.
    ///     To use this you have to enable it:
    ///     <code>RuntimeObjectEditor.ObjectEditor.Instance.Enable();</code>
    ///     The default shortcut key used it "Control+Shift+R".
    ///     If you want to use a different shortcut, change the <see cref="ObjectEditor.HotKey" />
    /// </summary>
    public sealed class ObjectEditor
    {
        private bool _enabled;
        private HotKeyWatch _hotKeyWatch;

        private ObjectEditor()
        {
        }

        /// <summary>
        ///     Gets or sets the default shortcut used to popup the Object Editor.
        ///     Default value is: Control+Shift+R.
        ///     You can specify any key combination like: Control+Shift+Alt+F1.
        /// </summary>
        public string HotKey { get; set; } = "Control+Shift+R";

        internal RuntimeEditor ActiveEditor { get; set; }

        public object SelectedObject
        {
            get { return ActiveEditor.SelectedObject; }
            set { ActiveEditor.SelectedObject = value; }
        }

        #region Instance

        /// <summary>
        ///     Singleton instance of the ObjectEditor.
        /// </summary>
        public static ObjectEditor Instance { get; } = new ObjectEditor();

        #endregion

        /// <summary>
        ///     Enable the Object Editor to listen for the shortcut key.
        /// </summary>
        /// <returns></returns>
        public bool Enable()
        {
            if (_enabled)
                return true; // already enabled.

            _hotKeyWatch = new HotKeyWatch();
            if (!_hotKeyWatch.RegisterHotKey(HotKey))
                return false; // didn't work

            _hotKeyWatch.HotKeyPressed += hotKeyWatch_HotKeyPressed;
            _enabled = true;
            Trace.WriteLine("ObjectEditor enabled on shorcut:" + HotKey);
            return true;
        }

        /// <summary>
        ///     Disable the object editor.
        /// </summary>
        public void Disable()
        {
            if (!_enabled)
                return;

            _hotKeyWatch.HotKeyPressed -= hotKeyWatch_HotKeyPressed;
            _hotKeyWatch.UnregisterKey();
            _hotKeyWatch = null;
            _enabled = false;
            Trace.WriteLine("ObjectEditor disabled.");
        }

        private void hotKeyWatch_HotKeyPressed(object sender, EventArgs e)
        {
            Show();
        }

        private void runtimeEditor_Closed(object sender, EventArgs e)
        {
            ActiveEditor = null;
        }

        /// <summary>
        ///     Show the object editor form.
        /// </summary>
        public void Show()
        {
            object activeSelectedObject = null;
            if (ActiveEditor != null)
            {
                activeSelectedObject = ActiveEditor.SelectedObject;
                ActiveEditor.Close();
            }

            ActiveEditor = new RuntimeEditor();
            ActiveEditor.Show();
            ActiveEditor.Closed += runtimeEditor_Closed;
            ActiveEditor.SelectedObject = activeSelectedObject;
        }

        /// <summary>
        ///     Show the editor with the selectedObject selected.
        /// </summary>
        /// <param name="selectObject">The object to be selected in the editor</param>
        public void Show(object selectObject)
        {
            ActiveEditor?.Close();

            ActiveEditor = new RuntimeEditor();
            ActiveEditor.Show();
            ActiveEditor.Closed += runtimeEditor_Closed;
            ActiveEditor.SelectedObject = selectObject;
        }

        public Form CreateEditor()
        {
            ActiveEditor?.Close();
            ActiveEditor = new RuntimeEditor();
            return ActiveEditor;
        }
    }
}