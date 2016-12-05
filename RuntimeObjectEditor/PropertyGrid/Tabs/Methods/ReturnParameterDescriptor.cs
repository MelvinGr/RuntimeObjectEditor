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

namespace RuntimeObjectEditor.PropertyGrid.Tabs.Methods
{
    /// <summary>
    ///     Property descriptor for the Return type of a method.
    /// </summary>
    internal class ReturnParameterDescriptor : PropertyDescriptor
    {
        private readonly MethodPropertyDescriptor _method;
        private readonly Type _returnType;

        public ReturnParameterDescriptor(MethodPropertyDescriptor method)
            : base("Return (" + method.MethodInfo.ReturnType.Name + ")", null)
        {
            _method = method;
            _returnType = method.MethodInfo.ReturnType;
        }

        public override bool IsReadOnly => true;

        public override Type PropertyType => _returnType;

        public override Type ComponentType => _returnType;

        public override TypeConverter Converter => _method.Converter;

        public object ReturnValue { get; set; }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
            ReturnValue = value;
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            return ReturnValue;
        }
    }
}