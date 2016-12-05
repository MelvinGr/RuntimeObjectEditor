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
using System.Drawing;
using System.Windows.Forms.Design;
using RuntimeObjectEditor.Properties;

namespace RuntimeObjectEditor.PropertyGrid.Tabs.Methods
{
    /// <summary>
    ///     MethodsTab - Tab showing all the methods of an object
    /// </summary>
    internal class MethodsTab : PropertyTab
    {
        public override Bitmap Bitmap => Resources.Methods;

        public override string TabName => "Methods";

        public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
        {
            if (component != null)
                return MethodUtils.GetMethodProperties(component);
            return null;
        }
    }
}