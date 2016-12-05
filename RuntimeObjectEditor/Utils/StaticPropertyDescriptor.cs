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
using System.Reflection;

namespace RuntimeObjectEditor.Utils
{
    /// <summary>
    ///     StaticPropertyDescriptor
    /// </summary>
    public class StaticPropertyDescriptor : PropertyDescriptor
    {
        private readonly PropertyInfo _propInfo;

        public StaticPropertyDescriptor(Type objectType, PropertyInfo propInfo)
            : base(propInfo.Name, null)
        {
            ComponentType = objectType;
            _propInfo = propInfo;
        }

        public override Type ComponentType { get; }

        public override bool IsReadOnly => !_propInfo.CanWrite;

        public override Type PropertyType => _propInfo.PropertyType;

        public override object GetValue(object component)
        {
            return _propInfo.GetValue(null, new object[] {});
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
            _propInfo.SetValue(null, value, new object[] {});
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}