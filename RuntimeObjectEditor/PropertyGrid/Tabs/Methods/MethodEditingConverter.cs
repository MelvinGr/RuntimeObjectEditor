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
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace RuntimeObjectEditor.PropertyGrid.Tabs.Methods
{
    /// <summary>
    ///     Summary description for MethodEditingConverter.
    /// </summary>
    internal class MethodEditingConverter : TypeConverter
    {
        private readonly MethodPropertyDescriptor _method;
        public readonly object RequestInvokeValue = new object();

        public MethodEditingConverter()
        {
        }

        public MethodEditingConverter(MethodPropertyDescriptor method)
        {
            _method = method;
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            if (_method.ParametersCount != 0)
                return true;
            if ((_method.MethodInfo.ReturnType == typeof(void)) || _method.MethodInfo.ReturnType.IsPrimitive)
                return false;
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value,
            Attribute[] attributes)
        {
            return _method.GetChildProperties(null, null);
        }

        #region Convert

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if ((destinationType == typeof(string)) && (value != null))
                return value.ToString();
            return value;
        }

        #endregion

        #region Get

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            if (_method.ParametersCount == 0)
                return true;
            return false;
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var actions = new ArrayList();
            actions.Add("Invoke Now");
            return new StandardValuesCollection(actions);
        }

        #endregion
    }
}