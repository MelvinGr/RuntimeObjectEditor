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
using System.Drawing.Design;
using System.Reflection;

namespace RuntimeObjectEditor.Utils
{
    /// <summary>
    ///     Remaps a Propertie's Descriptor Component type
    /// </summary>
    public class RemapPropertyDescriptor : PropertyDescriptor
    {
        private readonly string _displayNamePrefix;
        private readonly object _originalComponent;
        private readonly PropertyDescriptor _originalPropertyDescriptor;

        public RemapPropertyDescriptor(PropertyDescriptor originalPropertyDescriptor, object reamappedComponent,
            object originalComponent, string displayNamePrefix, TypeConverter typeConverter)
            : base(originalPropertyDescriptor.Name, null)
        {
            _originalPropertyDescriptor = originalPropertyDescriptor;
            RemappedComponent = reamappedComponent;
            _originalComponent = originalComponent;
            _displayNamePrefix = displayNamePrefix;
            Converter = typeConverter;
        }

        public override string Name => _originalPropertyDescriptor.Name;

        public override string Description => _originalPropertyDescriptor.Description;

        public override bool IsBrowsable => base.IsBrowsable;

        public object RemappedComponent { get; }

        public object OriginalComponent => _originalPropertyDescriptor.GetValue(_originalComponent);

        public override AttributeCollection Attributes
        {
            get
            {
                var tempAttributes = new ArrayList();

                var originalAttributes = _originalPropertyDescriptor.Attributes;
                //AttributeUtils.PrintAttributes(originalAttributes);
                tempAttributes.AddRange(originalAttributes);
                /*tempAttributes.AddRange( AttributeUtils.DeleteNonRelevatAttributes(originalAttributes) );

                tempAttributes.Add(new EditorAttribute(typeof (UITypeEditor), typeof (UITypeEditor)));
                tempAttributes.Add(new RefreshPropertiesAttribute(RefreshProperties.Repaint));
                tempAttributes.Add(new DesignerAttribute(typeof (ComponentDesigner), typeof (IDesigner)));
*/
                return AttributeUtils.GetAttributes(tempAttributes);
            }
        }

        public override bool DesignTimeOnly => false;

        public override bool IsReadOnly => _originalPropertyDescriptor.IsReadOnly;

        public override Type PropertyType => _originalPropertyDescriptor.PropertyType;

        public override Type ComponentType => RemappedComponent.GetType();

        public override string DisplayName
        {
            get
            {
                if (_displayNamePrefix != null)
                    return _displayNamePrefix + ":" + _originalPropertyDescriptor.DisplayName;
                return _originalPropertyDescriptor.DisplayName;
            }
        }

        public override TypeConverter Converter { get; }

        protected override void FillAttributes(IList attributeList)
        {
            // this method is not used - we overrride the Attributes property and create there a new set of attributes
            base.FillAttributes(attributeList);
            attributeList.Add(new EditorAttribute(typeof(UITypeEditor), typeof(UITypeEditor)));
            attributeList.Add(new RefreshPropertiesAttribute(RefreshProperties.Repaint));
            attributeList.Add(new DesignerAttribute(typeof(ComponentDesigner), typeof(IDesigner)));
        }

        protected override AttributeCollection CreateAttributeCollection()
        {
            return base.CreateAttributeCollection();
        }

        public override PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter)
        {
            //return base.GetChildProperties(instance, filter);
            return _originalPropertyDescriptor.GetChildProperties(_originalComponent);
        }

        public override object GetEditor(Type editorBaseType)
        {
            return _originalPropertyDescriptor.GetEditor(editorBaseType);
            //return new MethodEditor();
        }

        public override bool CanResetValue(object component)
        {
            return _originalPropertyDescriptor.CanResetValue(_originalComponent);
        }

        public override void ResetValue(object component)
        {
            _originalPropertyDescriptor.ResetValue(_originalComponent);
        }

        public override void SetValue(object component, object value)
        {
            _originalPropertyDescriptor.SetValue(_originalComponent, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            try
            {
                return _originalPropertyDescriptor.GetValue(_originalComponent);
            }
            catch (TargetInvocationException ex)
            {
                return "Ex:" + ex.InnerException.Message;
            }
            catch (Exception ex)
            {
                return "Ex:" + ex.Message;
            }
        }
    }
}