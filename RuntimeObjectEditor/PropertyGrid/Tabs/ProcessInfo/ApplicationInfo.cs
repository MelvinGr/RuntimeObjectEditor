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
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Remoting.Contexts;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace RuntimeObjectEditor.PropertyGrid.Tabs.ProcessInfo
{
    /// <summary>
    ///     ApplicationInfo - wrapper for the Application object.
    /// </summary>
    public class ApplicationInfo
    {
        #region Process

        [Category("Process")]
        public Process CurrentProcess => Process.GetCurrentProcess();

        //[Category("Process")]
        //public string CurrentProcessName
        //{
        //    get { return CurrentProcess.ProcessName; }
        //}

        #endregion

        #region Domain

        [Category("Domain")]
        public AppDomain CurrentDomain => AppDomain.CurrentDomain;

        #endregion

        #region Application

        [Category("Application")]
        public object CurrentApplication => Application.ProductName;

        #endregion

        #region GC

        public GcDetails GcInfo { get; } = new GcDetails();

        #endregion

        public class GcDetails
        {
            [Description("Returns the number of times garbage collection has occurred for the generation 0 of objects")]
            [RefreshProperties(RefreshProperties.All)]
            public int CollectionCount0 => GC.CollectionCount(0);

            [Description("Returns the number of times garbage collection has occurred for the generation 1 of objects")]
            [RefreshProperties(RefreshProperties.All)]
            public int CollectionCount1 => GC.CollectionCount(1);

            [Description("Returns the number of times garbage collection has occurred for the generation 2 of objects")]
            [RefreshProperties(RefreshProperties.All)]
            public int CollectionCount2 => GC.CollectionCount(2);

            [Description("Retrieves the number of bytes currently thought to be allocated (after a collect)")]
            [RefreshProperties(RefreshProperties.All)]
            public string TotalMemoryKb => Format(GC.GetTotalMemory(true)/1024);

            [Description("Gets the maximum allowable working set size for the associated process.")]
            [RefreshProperties(RefreshProperties.All)]
            public int MaxWorkingSetKb
            {
                get { return Process.GetCurrentProcess().MaxWorkingSet.ToInt32()/1024; }
                set { Process.GetCurrentProcess().MaxWorkingSet = new IntPtr(value*1024); }
            }

            [Description("Gets or Sets the Minimum allowable working set size for the associated process.")]
            [RefreshProperties(RefreshProperties.All)]
            public int MinWorkingSetKb
            {
                get { return Process.GetCurrentProcess().MinWorkingSet.ToInt32()/1024; }
                set { Process.GetCurrentProcess().MinWorkingSet = new IntPtr(value*1024); }
            }

            [Description("Gets the amount of physical memory allocated for the associated process.")]
            public string WorkingSetKb => Format(Process.GetCurrentProcess().WorkingSet64/1024);

            public override string ToString()
            {
                return "{Mem:" + TotalMemoryKb + " Max:" + WorkingSetKb + "}";
            }

            private string Format(long value) => value.ToString("###,##0");
        }

        #region Thread

        [Category("Thread")]
        public Context CurrentContext => Thread.CurrentContext;

        [Category("Thread")]
        public IPrincipal CurrentPrincipal => Thread.CurrentPrincipal;

        [Category("Thread")]
        public Thread CurrentThread => Thread.CurrentThread;

        //[Category("Thread")]
        //public int CurrentThreadId
        //{
        //    get { return Thread.CurrentThread.ManagedThreadId; }
        //}
        //[Category("Thread")]
        //public string CurrentThreadName
        //{
        //    get { return Thread.CurrentThread.Name; }
        //}

        #endregion
    }
}