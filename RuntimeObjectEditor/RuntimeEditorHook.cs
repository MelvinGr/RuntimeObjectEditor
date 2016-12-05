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
using System.Windows.Forms;
using RuntimeObjectEditor.Utils;

//using wfspy;

namespace RuntimeObjectEditor
{
    /// <summary>
    ///     Helper used to hook into other processed.
    /// </summary>
    public class RuntimeEditorHook // : IHookInstall
    {
        public static bool Hook(IntPtr targetWindowHandle, IntPtr thisHandle)
        {
            try
            {
                if (targetWindowHandle != IntPtr.Zero)
                {
                    int processId;
                    var threadId = NativeUtils.GetWindowThreadProcessId(targetWindowHandle, out processId);

                    var panelHandle = thisHandle.ToInt32();
                    var targetHandle = targetWindowHandle.ToInt32();

                    var b1 = BitConverter.GetBytes(panelHandle);
                    var b2 = BitConverter.GetBytes(targetHandle);

                    var data = new byte[b1.Length + b2.Length];
                    Array.Copy(b1, data, b1.Length);
                    Array.Copy(b2, 0, data, b1.Length, b2.Length);

#warning FIX
                    // Pickup an idle message from the queue
                    //fixHookHelper.InstallIdleHandler(processId, threadId, typeof (RuntimeEditorHook).Assembly.Location, typeof (RuntimeEditorHook).FullName, data);

                    // send an idle ;;)
                    NativeUtils.SendMessage(targetWindowHandle, 0, IntPtr.Zero, IntPtr.Zero);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "RuntimeObjectEditor");
                return false;
            }
        }

        #region IHookInstall

        public void OnInstallHook(byte[] data)
        {
            // Kick a timer so we can show a form from within this app
            var parentWindow = (IntPtr) BitConverter.ToInt32(data, 0);
            var spyWindow = (IntPtr) BitConverter.ToInt32(data, 4);

            NativeUtils.SendMessage(parentWindow, 0x0010, IntPtr.Zero, IntPtr.Zero); // close

            ObjectEditor.Instance.Enable();
            ObjectEditor.Instance.Show();
            ObjectEditor.Instance.ActiveEditor.SelectedWindowHandle = spyWindow;
        }

        #endregion
    }
}