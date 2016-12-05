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
using System.ComponentModel.Design;
using System.Reflection;
using RuntimeObjectEditor.PropertyGrid.Tabs.Methods;
using RuntimeObjectEditor.Utils;

namespace RuntimeObjectEditor.PropertyGrid.Tabs.Fields
{
    /// <summary>
    ///     Summary description for FieldPropertyDescriptor.
    /// </summary>
    public class FieldPropertyDescriptor : AbstractPropertyDescriptor
    {
        private readonly int _depth;
        private readonly Type _ownerType;

        public FieldPropertyDescriptor(object component, FieldInfo field, Type ownerType, int depth)
            : base(field.Name)
        {
            Component = component;
            FieldInfo = field;
            _ownerType = ownerType;
            _depth = depth;
        }

        public override AttributeCollection Attributes => base.Attributes;

        public override TypeConverter Converter => base.Converter;

        public override Type PropertyType => FieldInfo.FieldType;

        protected override void FillAttributes(IList attributeList)
        {
            base.FillAttributes(attributeList);

            attributeList.Add(new CategoryAttribute(_depth + ". " + _ownerType.Name));
            attributeList.Add(new RefreshPropertiesAttribute(RefreshProperties.Repaint));
            ;
            attributeList.Add(new DesignerAttribute(typeof(MethodDesigner), typeof(IDesigner)));
            attributeList.Add(new DesignTimeVisibleAttribute(false));
        }

        public override PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter)
        {
            return base.GetChildProperties(instance, filter);
        }


        public override void SetValue(object component, object value)
        {
            FieldInfo.SetValue(component, value);
        }

        public override object GetValue(object component)
        {
            return FieldInfo.GetValue(component);
        }

        #region PropertyDescriptor implementation

        public override Type ComponentType => Component.GetType();

        public override bool IsReadOnly => false;

        #endregion

        #region Properties

        public object Component { get; }

        public FieldInfo FieldInfo { get; }

        #endregion
    }
}