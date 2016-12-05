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
using RuntimeObjectEditor.Utils;

namespace RuntimeObjectEditor.PropertyGrid.Tabs.Methods
{
    /// <summary>
    ///     Summary description for MethodPropertyDescriptor.
    /// </summary>
    public class MethodPropertyDescriptor : PropertyDescriptor
    {
        private readonly int _depth;

        private readonly object _monitoredObject;
        private readonly Type _ownerType;
        private readonly MethodPropertyValueHolder _valueHolder;
        private TypeConverter _converter;

        private ArrayList _parameterDescriptors; // as ParameterPropertyDescriptor
        private PropertyDescriptorCollection _propertyDescriptorCollection;
        private ReturnParameterDescriptor _returnDescriptor;

        public MethodPropertyDescriptor(object monitoredObject, MethodInfo method, Type ownerType, int depth)
            : base((method.IsPublic ? "+ " : "- ") + method.Name, null)
        {
            _monitoredObject = monitoredObject;
            MethodInfo = method;
            _ownerType = ownerType;
            _depth = depth;
            _valueHolder = new MethodPropertyValueHolder(this);
        }

        protected override void FillAttributes(IList attributeList)
        {
            base.FillAttributes(attributeList);
            attributeList.Add(new EditorAttribute(typeof(MethodEditor), typeof(UITypeEditor)));
            attributeList.Add(new RefreshPropertiesAttribute(RefreshProperties.Repaint));
            attributeList.Add(
                new CategoryAttribute(_depth + ". " + _ownerType.Name + "( " +
                                      MethodUtils.GetMethodAccessShort(MethodInfo) + ")"));
            attributeList.Add(new DesignerAttribute(typeof(MethodDesigner), typeof(IDesigner)));
        }

        #region Help

        public enum Teste
        {
            InvokeNow
        }

        [TypeConverter(typeof(MethodEditingConverter))]
        public class MethodPropertyValueHolder : IRealValueHolder
        {
            public MethodPropertyValueHolder(MethodPropertyDescriptor method)
            {
                Method = method;
            }

            public MethodPropertyDescriptor Method { get; }

            #region IRealValueHolder Members

            public object RealValue => Method.ValueOfLastRun;

            #endregion

            public override string ToString()
            {
                if (Method.ValueOfLastRun != null)
                    return Method.ValueOfLastRun.ToString();
                if (Method.ParametersCount == 0)
                    return "(select to invoke)";
                return "";
            }
        }

        #endregion

        #region Override

        public override object GetValue(object component)
        {
            return _valueHolder;
        }

        public override void SetValue(object component, object value)
        {
            if (ParametersCount == 0)
                Invoke();
        }

        public override Type ComponentType => _monitoredObject.GetType();

        public override Type PropertyType => _valueHolder.GetType();

        public override TypeConverter Converter
        {
            get
            {
                if (_converter == null)
                    _converter = new MethodEditingConverter(this);
                return _converter;
            }
        }

        public override string Description => MethodUtils.GetMethodSignature(MethodInfo);

        #endregion

        #region Parameter Ops

        public override PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter)
        {
            if (_propertyDescriptorCollection == null)
            {
                ResolveParameters();
                var list = _parameterDescriptors.Clone() as ArrayList;
                list.Add(_returnDescriptor);
                var paramDesc = (PropertyDescriptor[]) list.ToArray(typeof(PropertyDescriptor));
                _propertyDescriptorCollection = new PropertyDescriptorCollection(paramDesc);
            }
            return _propertyDescriptorCollection;
        }

        private void ResolveParameters()
        {
            if (_parameterDescriptors != null)
                return;
            _parameterDescriptors = MethodUtils.GetMethodParams(this);
            _returnDescriptor = new ReturnParameterDescriptor(this);
        }

        #endregion

        #region Override

        public override bool IsReadOnly => false;

        public override bool IsBrowsable => true;

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override void ResetValue(object component)
        {
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override AttributeCollection Attributes => base.Attributes;

        public override bool DesignTimeOnly => false;

        #endregion

        #region Properties

        public MethodInfo MethodInfo { get; }

        public int ParametersCount => MethodInfo.GetParameters().Length;

        public bool IsVoidMethdod => MethodInfo.ReturnType == typeof(void);

        public object ValueOfLastRun { get; private set; }

        public void Invoke()
        {
            // invoke the method
            if (_parameterDescriptors == null)
                ResolveParameters();

            // Now, invoke
            var param = new object[_parameterDescriptors.Count];
            for (var i = 0; i < _parameterDescriptors.Count; i++)
            {
                var para = _parameterDescriptors[i] as ParameterPropertyDescriptor;
                param[i] = para.GetValue(_monitoredObject);
            }
            Invoke(param);
            _returnDescriptor.SetValue(_monitoredObject, ValueOfLastRun);
        }

        private void Invoke(object[] param)
        {
            try
            {
                ValueOfLastRun = MethodInfo.Invoke(_monitoredObject, param);
                if (IsVoidMethdod)
                    ValueOfLastRun = "<void>";
            }
            catch (TargetInvocationException ex)
            {
                ValueOfLastRun = ex.InnerException.ToString();
            }
            catch (Exception ex)
            {
                ValueOfLastRun = ex.ToString();
            }
        }

        #endregion
    }
}