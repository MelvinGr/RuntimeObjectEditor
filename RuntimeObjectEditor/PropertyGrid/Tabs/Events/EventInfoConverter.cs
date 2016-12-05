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
using System.Reflection;
using RuntimeObjectEditor.Utils;

namespace RuntimeObjectEditor.PropertyGrid.Tabs.Events
{
    public class EventInfoConverter : TypeConverter
    {
        // standard winforms event key objects: "private static readonly object EventLayout;"

        private readonly EventPropertyDescriptor _eventDescriptor;
        private EventHandlerList _eventHandlerList;
        private FieldAccesor _handlerAccesor;
        private FieldAccesor _keyAccesor;

        public EventInfoConverter(EventPropertyDescriptor eventDescriptor)
        {
            _eventDescriptor = eventDescriptor;
        }

        public ArrayList EventListeners { get; private set; }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            return true;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Enum.GetNames(typeof(EventPropertyDescriptor.EnumHelperEnum)));
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value,
            Attribute[] attributes)
        {
            return GetProperties();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            if (EventListeners == null)
                ReadListeners();
            if (EventListeners != null)
                return
                    new PropertyDescriptorCollection(
                        (PropertyDescriptor[]) EventListeners.ToArray(typeof(PropertyDescriptor)));
            return null;
        }

        public override string ToString()
        {
            if (EventListeners == null)
                return "{Select to analize}";
            return "{Listeners:" + EventListeners.Count + "}";
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
            if (value is string)
                return Enum.Parse(typeof(EventPropertyDescriptor.EnumHelperEnum), value as string);
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

        #region Read Listeners

        internal void ReadListeners()
        {
            // for examples we use event with name "Load"
            // first try to read key representing the static eventKey (EventLoad as key object static)
            var eventKeyAccesor = new FieldAccesor(_eventDescriptor.Component, "Event" + _eventDescriptor.Name);
            if (ReadEventHandlersFromHandlerList(eventKeyAccesor))
                return;
            if (_eventDescriptor.Name.EndsWith("Changed"))
            {
                // try "EventBackColor" for "BackColorChanged" event
                var shortName = _eventDescriptor.Name.Substring(0, _eventDescriptor.Name.IndexOf("Changed"));
                eventKeyAccesor = new FieldAccesor(_eventDescriptor.Component, "Event" + shortName);
                if (ReadEventHandlersFromHandlerList(eventKeyAccesor))
                    return;
            }
            eventKeyAccesor = new FieldAccesor(_eventDescriptor.Component, "EVENT_" + _eventDescriptor.Name);
            if (ReadEventHandlersFromHandlerList(eventKeyAccesor))
                return;

            // try to read the delegate on the object directly!
            // try "Load"
            var delegateDirectAccesor = new FieldAccesor(_eventDescriptor.Component, _eventDescriptor.Name);
            if (!delegateDirectAccesor.IsValid)
                delegateDirectAccesor = new FieldAccesor(_eventDescriptor.Component,
                    "on" + _eventDescriptor.Name + "Delegate");
            if (!delegateDirectAccesor.IsValid)
                delegateDirectAccesor = new FieldAccesor(_eventDescriptor.Component, "on" + _eventDescriptor.Name);

            if (delegateDirectAccesor.IsValid)
            {
                delegateDirectAccesor.Save();
                var eventHandlers = delegateDirectAccesor.Value as Delegate;
                if (eventHandlers != null)
                {
                    EventListeners = new ArrayList();
                    var invocationList = eventHandlers.GetInvocationList();
                    foreach (var del in invocationList)
                        EventListeners.Add(new EventListenerPropertyDescriptor(_eventDescriptor, del));
                }
            }
        }

        #region Use EventHandlerList 

        private bool ReadEventHandlersFromHandlerList(FieldAccesor eventKeyAccesor)
        {
            if (!eventKeyAccesor.IsValid)
                return false;

            if (EventListeners == null)
                EventListeners = new ArrayList();

            eventKeyAccesor.Save();
            var eventKey = eventKeyAccesor.Value;
            if (eventKey == null)
                return true;

            _eventHandlerList = _eventDescriptor.EventHandlerList;
            if (_eventHandlerList != null)
            {
                var entry = GetHead();
                if (entry != null)
                    do
                    {
                        // first read the target
                        if (_keyAccesor == null)
                            _keyAccesor = new FieldAccesor(entry, "key");
                        _keyAccesor.Target = entry;
                        _keyAccesor.Save();
                        if (_keyAccesor.Value == eventKey)
                        {
                            if (_handlerAccesor == null)
                                _handlerAccesor = new FieldAccesor(entry, "handler");

                            _handlerAccesor.Target = entry;
                            _handlerAccesor.Save();

                            if (_handlerAccesor.Value != null)
                                EventListeners.Add(new EventListenerPropertyDescriptor(_eventDescriptor,
                                    _handlerAccesor.Value as Delegate));
                        }
                        entry = GetListEntry(entry);
                    } while (entry != null);
            }
            return true;
        }

        #endregion

        //[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
        public object GetHead()
        {
            var field = _eventHandlerList.GetType()
                .GetField("head", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
            if (field != null)
                return field.GetValue(_eventHandlerList);
            return null;
        }

        private FieldInfo _nextField;

        public object GetListEntry(object entry)
        {
            if (entry != null)
            {
                if (_nextField == null)
                    _nextField = entry.GetType()
                        .GetField("next", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
                if (_nextField != null)
                    return _nextField.GetValue(entry);
            }
            return null;
        }

        #endregion
    }
}