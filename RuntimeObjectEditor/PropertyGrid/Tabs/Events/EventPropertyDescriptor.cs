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
using System.Reflection;
using RuntimeObjectEditor.Utils;

namespace RuntimeObjectEditor.PropertyGrid.Tabs.Events
{
    /// <summary>
    ///     EventPropertyDescriptor
    /// </summary>
    public class EventPropertyDescriptor : AbstractPropertyDescriptor
    {
        #region EnumHelperEnum

        public enum EnumHelperEnum
        {
            RefreshListeners
            //AddListener,
            //RemoveListener
        }

        #endregion

        private readonly EventInfoConverter _converter;

        public EventPropertyDescriptor(object component, EventInfo eventInfo, EventHandlerList eventHandlerList)
            : base(eventInfo.Name)
        {
            Component = component;
            EventInfo = eventInfo;
            EventHandlerList = eventHandlerList;

            _converter = new EventInfoConverter(this);
        }

        public override TypeConverter Converter => _converter;

        public override Type PropertyType => typeof(EnumHelperEnum);

        public override AttributeCollection Attributes
        {
            get
            {
                var attr = base.Attributes;
                return attr;
            }
        }

        protected override void FillAttributes(IList attributeList)
        {
            base.FillAttributes(attributeList);

            var allEventAttributes = EventInfo.GetCustomAttributes(false);

            foreach (Attribute attr in allEventAttributes)
                attributeList.Add(attr);
        }


        public override void SetValue(object component, object value)
        {
            _converter.ReadListeners();
        }

        public override object GetValue(object component)
        {
            return _converter;
        }

        #region PropertyDescriptor implementation

        public override Type ComponentType => Component.GetType();

        public override bool IsReadOnly => false;

        #endregion

        #region Properies

        public object Component { get; }

        public EventInfo EventInfo { get; }

        public EventHandlerList EventHandlerList { get; }

        #endregion
    }
}