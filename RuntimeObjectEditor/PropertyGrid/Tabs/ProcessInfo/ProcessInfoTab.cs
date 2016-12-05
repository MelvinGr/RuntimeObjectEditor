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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using RuntimeObjectEditor.Properties;
using RuntimeObjectEditor.Utils;

namespace RuntimeObjectEditor.PropertyGrid.Tabs.ProcessInfo
{
    /// <summary>
    ///     ProcessInfoTab - Tab showing all the methods of an object
    /// </summary>
    internal class ProcessInfoTab : PropertyTab
    {
        private ApplicationInfo _cachedApplicationInfo;
        private object _cachedComponentInfo;

        public override Bitmap Bitmap => Resources.ProcessInfo;

        public override string TabName => "ProcessInfo";

        public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
        {
            return GetProperties(null, component, attributes);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component,
            Attribute[] attributes)
        {
            if (context.PropertyDescriptor is RemapPropertyDescriptor)
            {
                var remapDescriptor = context.PropertyDescriptor as RemapPropertyDescriptor;
                if (remapDescriptor.Name == "CurrentApplication")
                {
                    var realProperties = PropertyDescriptorUtils.GetStaticProperties(typeof(Application));
                    var remappedProperties = PropertyDescriptorUtils.RemapComponent(realProperties, component, null,
                        null, new ChildTypeConverter());
                    return remappedProperties;
                }
                else
                {
                    var realProperties = ReadProperties(null, component, attributes, component, null);
                    return realProperties;
                }
            }
            if (_cachedComponentInfo != component)
            {
                _cachedApplicationInfo = new ApplicationInfo();
                _cachedComponentInfo = component;
            }

            var result = ReadProperties(context, component, attributes, _cachedApplicationInfo, null);
            //PropertyDescriptorCollection appProps = PropertyDescriptorUtils.GetStaticProperties(typeof(Application));
            //result = PropertyDescriptorUtils.MergeProperties(result, appProps);

            return result;

            //temp = ReadProperties(context, component, attributes, cachedApplicationInfo.CurrentPrincipal, "CurrentPrincipal");
            //result = PropertyDescriptorUtils.MergeProperties(result, temp);

            //temp = ReadProperties(context, component, attributes, cachedApplicationInfo.CurrentPrincipal.Identity, "CurrentIdentity");
            //result = PropertyDescriptorUtils.MergeProperties(result, temp);

            //temp = ReadProperties(context, component, attributes, cachedApplicationInfo.CurrentPrincipal, "CurrentPrincipal");
            //result = PropertyDescriptorUtils.MergeProperties(result, temp);

            //temp = ReadProperties(context, component, attributes, cachedApplicationInfo.CurrentThread, "CurrentThread");
            //result = PropertyDescriptorUtils.MergeProperties(result, temp);

            //temp = ReadProperties(context, component, attributes, cachedApplicationInfo.CurrentProcess, "CurrentProcess");
            //result = PropertyDescriptorUtils.MergeProperties(result, temp);
        }

        private PropertyDescriptorCollection ReadProperties(ITypeDescriptorContext context, object realComponent,
            Attribute[] attributes, object realObject, string name)
        {
            PropertyDescriptorCollection remappedProperties;
            var realProperties = PropertyDescriptorUtils.GetAllProperties(context, realObject, attributes);
            remappedProperties = PropertyDescriptorUtils.RemapComponent(realProperties, realComponent, realObject, name,
                new ChildTypeConverter());
            return remappedProperties;
        }
    }
}