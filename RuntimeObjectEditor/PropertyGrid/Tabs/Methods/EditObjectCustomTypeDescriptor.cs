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
    ///     Summary description for CustomTypeDescriptor.
    /// </summary>
    public class EditObjectCustomTypeDescriptor : ICustomTypeDescriptor
    {
        private readonly object _editedObject;
        private readonly MethodEditor _methodEditor;

        public EditObjectCustomTypeDescriptor(object editedObject)
        {
            _editedObject = editedObject;
            _methodEditor = new MethodEditor();
        }

        #region ICustomTypeDescriptor Members

        public TypeConverter GetConverter()
        {
            return new TypeConverter();
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(_editedObject, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return _editedObject;
        }

        public AttributeCollection GetAttributes()
        {
            var attributes = TypeDescriptor.GetAttributes(_editedObject, true);

            //attributes = AttributeUtils.ReplaceAttribute(attributes, new DesignerAttribute(typeof(MethodDesigner), typeof(IDesigner)));
            //attributes = AttributeUtils.ReplaceAttribute(attributes, new DesignTimeVisibleAttribute(true));
            //attributes = AttributeUtils.ReplaceAttribute(attributes, new DesignTimeVisibleAttribute(true));

            return attributes;
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(_editedObject);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return GetEvents();
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            //return MethodUtils.GetMethodProperties(this.editedObject);
            return TypeDescriptor.GetProperties(_editedObject);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            //return MethodUtils.GetMethodProperties(this.editedObject);
            return TypeDescriptor.GetProperties(_editedObject);
        }

        public object GetEditor(Type editorBaseType)
        {
            return _methodEditor;
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(_editedObject);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(_editedObject);
        }

        public string GetClassName()
        {
            return _editedObject.GetType().Name;
        }

        #endregion
    }
}