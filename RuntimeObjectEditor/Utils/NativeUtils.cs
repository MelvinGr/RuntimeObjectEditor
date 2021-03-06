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
using System.Runtime.InteropServices;
using System.Text;

namespace RuntimeObjectEditor.Utils
{
    public delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);

    public enum GetWindowCmd
    {
        GwHwndfirst = 0,
        GwHwndlast,
        GwHwndnext,
        GwHwndprev,
        GwOwner,
        GwChild
    }

    public struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public class NativeUtils
    {
        [DllImport("user32.dll")]
        public static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern bool EnumThreadWindows(int threadId, WindowEnumProc func, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hwnd, StringBuilder buffer, int bufferLen);

        [DllImport("user32.dll")]
        public static extern int GetClassName(IntPtr hwnd, StringBuilder buffer, int bufferLen);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetParent(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int processId);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hwnd, int x, int y, int width, int height, bool repaint);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hwnd, int cmd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hwnd, ref Rect rc);

        [DllImport("user32.dll")]
        public static extern IntPtr GetClientRect(IntPtr hwnd, ref Rect rc);

        public static string GetWindowText(IntPtr hwnd)
        {
            var bufLen = GetWindowTextLength(hwnd) + 1;

            var buffer = new StringBuilder(bufLen);
            GetWindowText(hwnd, buffer, bufLen);

            return buffer.ToString();
        }

        public static string GetClassName(IntPtr hwnd)
        {
            var buffer = new StringBuilder(256);
            GetClassName(hwnd, buffer, 256);

            return buffer.ToString();
        }


        public static bool IsTargetInDifferentProcess(IntPtr targetWindowHandle)
        {
            int targetProcessId;
            int thisProcessId;

            var targetThreadId = GetWindowThreadProcessId(targetWindowHandle, out targetProcessId);

            if (targetProcessId == -1)
                return true;

            thisProcessId = Process.GetCurrentProcess().Id;

            //NativeUtils.GetWindowThreadProcessId(this.Handle, out thisProcessId);

            return thisProcessId != targetProcessId;
        }
    }
}