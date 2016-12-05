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
using RuntimeObjectEditor.PropertyGrid.Tabs.Events;
using RuntimeObjectEditor.PropertyGrid.Tabs.Fields;

namespace RuntimeObjectEditor.Utils
{
    /// <summary>
    ///     PropertyDescriptorUtils
    /// </summary>
    public class PropertyDescriptorUtils
    {
        public static PropertyDescriptorCollection GetAllProperties(ITypeDescriptorContext context, object component,
            Attribute[] attributes)
        {
            PropertyDescriptorCollection properties;

            Attribute[] attributeArray1 = {};
            attributes = attributeArray1; // replace the attribute array to allow all properties to be visible.

            if (context == null)
            {
                properties = TypeDescriptor.GetProperties(component, attributes);
            }
            else
            {
                var remapDescriptor = context.PropertyDescriptor as RemapPropertyDescriptor;
                if (remapDescriptor != null)
                    component = remapDescriptor.OriginalComponent;

                var converter1 = context.PropertyDescriptor == null
                    ? TypeDescriptor.GetConverter(component)
                    : context.PropertyDescriptor.Converter;
                if ((converter1 != null) && converter1.GetPropertiesSupported(context))
                    properties = converter1.GetProperties(context, component, attributes);
                else
                    properties = TypeDescriptor.GetProperties(component, attributes);
            }
            return properties;
        }

        public static PropertyDescriptorCollection GetStaticProperties(Type componentType)
        {
            var properties = componentType.GetProperties();
            var propDesc = new ArrayList();
            foreach (var prop in properties)
                propDesc.Add(new StaticPropertyDescriptor(componentType, prop));
            return GetProperties(propDesc);
        }

        public static PropertyDescriptorCollection GetInstanceEvents(object component)
        {
            var componentType = component.GetType();

            EventHandlerList eventHandlerList = null;

            var componentEvents = component.GetType()
                .GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            if (componentEvents != null)
                eventHandlerList = componentEvents.GetValue(component, new object[] {}) as EventHandlerList;

            var eventsInfo = componentType.GetEvents();
            var propDesc = new ArrayList();
            foreach (var eventInfo in eventsInfo)
                propDesc.Add(new EventPropertyDescriptor(component, eventInfo, eventHandlerList));
            return GetProperties(propDesc);
        }

        public static PropertyDescriptorCollection RemapComponent(PropertyDescriptorCollection propertyDescriptors,
            object remappedComponent, object originalComponent, string displayNamePrefix, TypeConverter typeConverter)
        {
            var newProperties = new PropertyDescriptor[propertyDescriptors.Count];

            for (var i = 0; i < propertyDescriptors.Count; i++)
            {
                var originalDescriptor = propertyDescriptors[i];
                if (originalDescriptor.PropertyType.IsPrimitive || originalDescriptor.PropertyType.IsEnum ||
                    (originalDescriptor.PropertyType == typeof(string))
                    || typeof(ICollection).IsAssignableFrom(originalDescriptor.PropertyType))
                {
                    newProperties[i] = new RemapPropertyDescriptor(originalDescriptor, remappedComponent,
                        originalComponent, displayNamePrefix, new ShowChildListConverter(originalDescriptor.Converter));
                }
                else
                {
                    if (typeConverter == null)
                        newProperties[i] = new RemapPropertyDescriptor(originalDescriptor, remappedComponent,
                            originalComponent, displayNamePrefix,
                            new ShowChildListConverter(originalDescriptor.Converter));
                    else
                        newProperties[i] = new RemapPropertyDescriptor(originalDescriptor, remappedComponent,
                            originalComponent, displayNamePrefix, new ShowChildListConverter(typeConverter));
                }
            }

            var retCollection = new PropertyDescriptorCollection(newProperties);
            return retCollection;
        }

        public static ArrayList GetProperties(PropertyDescriptorCollection collection)
        {
            var attributes = new ArrayList();
            foreach (PropertyDescriptor attr in collection)
                attributes.Add(attr);
            return attributes;
        }

        public static PropertyDescriptorCollection GetProperties(ArrayList properties)
        {
            var collection =
                new PropertyDescriptorCollection((PropertyDescriptor[]) properties.ToArray(typeof(PropertyDescriptor)));
            return collection;
        }

        public static PropertyDescriptorCollection MergeProperties(PropertyDescriptorCollection originalCollection,
            PropertyDescriptorCollection toMerge)
        {
            var originalList = GetProperties(originalCollection);
            var toMergeList = GetProperties(toMerge);
            originalList.AddRange(toMergeList);
            return GetProperties(originalList);
        }

        public static PropertyDescriptorCollection GetAllFields(ITypeDescriptorContext context, object component,
            Attribute[] attributes)
        {
            var type = component.GetType();

            var list = new ArrayList();

            var depth = 1;
            do
            {
                var fieldInfos =
                    type.GetFields(BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public |
                                   BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

                for (var i = 0; i < fieldInfos.Length; i++)
                    try
                    {
                        var fieldInfo = fieldInfos[i];
                        list.Add(new FieldPropertyDescriptor(component, fieldInfo, type, depth));
                    }
                    catch (Exception)
                    {
                    }

                if (type == typeof(object))
                    break;

                type = type.BaseType;
                depth++;
            } while (true);

            var fieldDesc = (FieldPropertyDescriptor[]) list.ToArray(typeof(FieldPropertyDescriptor));
            return new PropertyDescriptorCollection(fieldDesc);
        }
    }
}