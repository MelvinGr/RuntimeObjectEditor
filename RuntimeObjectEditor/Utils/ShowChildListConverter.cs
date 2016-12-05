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

namespace RuntimeObjectEditor.Utils
{
    /// <summary>
    ///     Summary description for ShowChildListConverter.
    /// </summary>
    public class ShowChildListConverter : TypeConverter
    {
        private readonly TypeConverter _originalConverter;

        public ShowChildListConverter(TypeConverter originalConverter)
        {
            _originalConverter = originalConverter;
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            // try to convert to IList
            if ((context != null) && context.PropertyDescriptor.GetValue(context.Instance) is ICollection)
                return true;
            return _originalConverter.GetPropertiesSupported(context);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value,
            Attribute[] attributes)
        {
            var propCollection = _originalConverter.GetProperties(context, value, attributes);
            if (value is IEnumerable)
            {
                // build a descriptor for each item!!!
                var valueCollection = value as IEnumerable;
                var newProps = new ArrayList();
                var index = 0;
                foreach (var child in valueCollection)
                {
                    newProps.Add(new SimpleChildDescriptor(value, index, child));
                    index++;
                }
                var itemsCollection = PropertyDescriptorUtils.GetProperties(newProps);
                return itemsCollection;
                //return PropertyDescriptorUtils.MergeProperties(itemsCollection, propCollection);
            }
            return propCollection;
        }

        #region SimpleChildDescriptor

        public class SimpleChildDescriptor : AbstractPropertyDescriptor, IRealValueHolder
        {
            private readonly int _index;
            private readonly object _value;

            public SimpleChildDescriptor(object value, int index, object childValue)
                : base("Item:" + index)
            {
                _value = value;
                _index = index;
                RealValue = childValue;
            }

            public override Type ComponentType => _value.GetType();

            public override Type PropertyType => GetType();

            #region IRealValueHolder Members

            public object RealValue { get; }

            #endregion

            public override object GetValue(object component)
            {
                return this;
            }

            public override string ToString()
            {
                if (RealValue != null)
                    return RealValue.ToString();
                return null;
            }
        }

        #endregion

        #region Delegates

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return _originalConverter.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return _originalConverter.CanConvertTo(context, destinationType);
        }


        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return _originalConverter.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            return _originalConverter.ConvertTo(context, culture, value, destinationType);
        }


        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            return _originalConverter.CreateInstance(context, propertyValues);
        }

//		protected Exception GetConvertFromException(object value)
//		{
//			return originalConverter.GetConvertFromException(value);
//		}
//
//		protected Exception GetConvertToException(object value, Type destinationType)
//		{
//			return originalConverter.GetConvertToException(value, destinationType);
//		}


        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return _originalConverter.GetCreateInstanceSupported(context);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return _originalConverter.GetStandardValues(context);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return _originalConverter.GetStandardValuesExclusive(context);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return _originalConverter.GetStandardValuesSupported(context);
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            return _originalConverter.IsValid(context, value);
        }

//		protected PropertyDescriptorCollection SortProperties(PropertyDescriptorCollection props, string[] names)
//		{
//			return originalConverter.SortProperties(props, names);
//		}

        #endregion
    }
}