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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using RuntimeObjectEditor.Properties;

namespace RuntimeObjectEditor
{
    /// <summary>
    ///     WindowFinder - Control to help find other windows/controls.
    /// </summary>
    [DefaultEvent("ActiveWindowChanged")]
    public class WindowFinder : UserControl
    {
        private bool _searching;

        public WindowFinder()
        {
            Window = new WindowProperties();
            MouseDown += WindowFinder_MouseDown;
            Size = new Size(32, 32);

            InitializeComponent();
        }

        public override Image BackgroundImage => Resources.Eye;

        #region DetectedWindowProperties

        public WindowProperties Window { get; }

        #endregion

        public object SelectedObject => Window.ActiveWindow;

        public IntPtr SelectedHandle
        {
            get { return Window.DetectedWindow; }
            set
            {
                Window.SetWindowHandle(value);
                InvokeActiveWindowChanged();
            }
        }

        public bool IsManagedByClassName => Window.IsManagedByClassName;
        public event EventHandler ActiveWindowChanged;
        public event EventHandler ActiveWindowSelected;

        protected override void Dispose(bool disposing)
        {
            Window.Dispose();
            base.Dispose(disposing);
        }

        #region Designer Generated

        private void InitializeComponent()
        {
            // 
            // WindowFinder
            // 
            Name = "WindowFinder";
            Size = new Size(32, 32);
        }

        #endregion

        private void WindowFinder_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_searching)
                StartSearch();
        }

        private void WindowFinder_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_searching)
                EndSearch();

            // Grab the window from the screen location of the mouse.
            var windowPoint = POINT.FromPoint(PointToScreen(new Point(e.X, e.Y)));
            var found = WindowFromPoint(windowPoint);

            // we have a valid window handle
            if (found != IntPtr.Zero)
                if (ScreenToClient(found, ref windowPoint))
                {
                    // check if there is some hidden/disabled child window at this point
                    var childWindow = ChildWindowFromPoint(found, windowPoint);
                    if (childWindow != IntPtr.Zero)
                        found = childWindow;
                }

            // Is this the same window as the last detected one?
            if (found != Window.DetectedWindow)
            {
                Window.SetWindowHandle(found);
                Trace.WriteLine("FoundWindow:" + Window.Name + ":" + Window.Text + " Managed:" + Window.IsManaged);
                InvokeActiveWindowChanged();
            }
        }

        private void InvokeActiveWindowChanged()
        {
            ActiveWindowChanged?.Invoke(this, EventArgs.Empty);
        }

        private void WindowFinder_MouseUp(object sender, MouseEventArgs e)
        {
            EndSearch();
        }

        #region WindowProperties

        public class WindowProperties : IDisposable
        {
            private static readonly Pen DrawPen = new Pen(Brushes.Red, 2);
            //private static Regex classNameRegex = new Regex(@"WindowsForms10\..*\.app[\da-eA-E]*$", RegexOptions.Singleline);	

            public IntPtr DetectedWindow { get; private set; } = IntPtr.Zero;

            public Control ActiveWindow => DetectedWindow != IntPtr.Zero ? FromHandle(DetectedWindow) : null;

            public string Name => ActiveWindow?.Name;

            public string Text
            {
                get
                {
                    if (!IsValid)
                        return null;
                    if (IsManaged)
                        return ActiveWindow.Text;
                    var builder = new StringBuilder();
                    GetWindowText(DetectedWindow, builder, 255);
                    return builder.ToString();
                }
            }

            public string ClassName
            {
                get
                {
                    if (!IsValid)
                        return null;
                    var builder = new StringBuilder(256);
                    GetClassName(DetectedWindow, builder, builder.Capacity);
                    return builder.ToString();
                }
            }

            public bool IsManagedByClassName
            {
                get
                {
                    var className = ClassName;
                    if ((className != null) && className.StartsWith("WindowsForms10"))
                        return true;
                    return false;
                    //Match match = classNameRegex.Match(ClassName);
                    //return match.Success;
                }
            }

            public bool IsValid => DetectedWindow != IntPtr.Zero;

            public bool IsManaged => ActiveWindow != null;

            #region IDisposable Members

            public void Dispose()
            {
                Refresh();
            }

            #endregion

            internal void SetWindowHandle(IntPtr handle)
            {
                Refresh();
                DetectedWindow = handle;
                Refresh();
                Highlight();
            }

            public void Refresh()
            {
                if (!IsValid)
                    return;
                var toUpdate = DetectedWindow;
                var parentWindow = GetParent(toUpdate);
                if (parentWindow != IntPtr.Zero)
                    toUpdate = parentWindow; // using parent

                InvalidateRect(toUpdate, IntPtr.Zero, true);
                UpdateWindow(toUpdate);
                var result = RedrawWindow(toUpdate, IntPtr.Zero, IntPtr.Zero,
                    RdwFrame | RdwInvalidate | RdwUpdatenow | RdwErasenow | RdwAllchildren);

                //Trace.WriteLine ( "Highlight:" + this.Text + " Rect:" + zeroRect + "  " + result );
            }

            public void Highlight()
            {
                Rect windowRect;
                GetWindowRect(DetectedWindow, out windowRect);

                var parentWindow = GetParent(DetectedWindow);
                var windowDc = GetWindowDC(DetectedWindow);
                if (windowDc != IntPtr.Zero)
                {
                    var graph = Graphics.FromHdc(windowDc, DetectedWindow);
                    graph.DrawRectangle(DrawPen, 1, 1, windowRect.Width - 2, windowRect.Height - 2);
                    graph.Dispose();
                    ReleaseDC(DetectedWindow, windowDc);
                }
            }
        }

        #endregion

        #region PInvoke

        #region Consts

        private const uint RdwInvalidate = 0x0001;
        private const uint RdwInternalpaint = 0x0002;
        private const uint RdwErase = 0x0004;

        private const uint RdwValidate = 0x0008;
        private const uint RdwNointernalpaint = 0x0010;
        private const uint RdwNoerase = 0x0020;

        private const uint RdwNochildren = 0x0040;
        private const uint RdwAllchildren = 0x0080;

        private const uint RdwUpdatenow = 0x0100;
        private const uint RdwErasenow = 0x0200;

        private const uint RdwFrame = 0x0400;
        private const uint RdwNoframe = 0x0800;

        #endregion

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(POINT point);

        [DllImport("user32.dll")]
        private static extern IntPtr ChildWindowFromPoint(IntPtr hWndParent, POINT point);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr hWnd, [Out] StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        [DllImport("user32.dll")]
        private static extern bool UpdateWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool RedrawWindow(IntPtr hWnd, IntPtr lpRect, IntPtr hrgnUpdate, uint flags);

        [DllImport("user32.dll")]
        private static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreatePen(int fnPenStyle, int nWidth, uint crColor);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDc);

        #region RECT

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public Rect(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public int Height => Bottom - Top;

            public int Width => Right - Left;

            public Size Size => new Size(Width, Height);


            public Point Location => new Point(Left, Top);


            // Handy method for converting to a System.Drawing.Rectangle
            public Rectangle ToRectangle()
            {
                return Rectangle.FromLTRB(Left, Top, Right, Bottom);
            }


            public static Rect FromRectangle(Rectangle rectangle)
            {
                return new Rect(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
            }
        }

        #endregion

        #region POINT

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public readonly int x;
            public readonly int y;

            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public POINT ToPoint()
            {
                return new POINT(x, y);
            }

            public static POINT FromPoint(Point pt)
            {
                return new POINT(pt.X, pt.Y);
            }

            public override bool Equals(object obj)
            {
                // I wish we could use the "as" operator
                // and check the type compatibility only
                // once here, just like with reference 
                // types. Maybe in the v2.0 :)
                if (!(obj is POINT))
                    return false;
                var point = (POINT) obj;
                if (point.x == x)
                    return point.y == y;
                return false;
            }

            public override int GetHashCode()
            {
                // this is the Microsoft implementation for the
                // System.Drawing.Point's GetHashCode() method.
                return x ^ y;
            }

            public override string ToString()
            {
                return string.Format("{{X={0}, Y={1}}}", x, y);
            }
        }

        #endregion

        #endregion

        #region Start/Stop Search

        public void StartSearch()
        {
            _searching = true;

            using (var stream = new MemoryStream(Resources.Eye1))
            {
                Cursor.Current = new Cursor(stream);
            }

            Capture = true;

            MouseMove += WindowFinder_MouseMove;
            MouseUp += WindowFinder_MouseUp;
        }

        public void EndSearch()
        {
            MouseMove -= WindowFinder_MouseMove;
            Capture = false;
            _searching = false;
            Cursor.Current = Cursors.Default;

            ActiveWindowSelected?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}