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
using System.Reflection;

namespace RuntimeObjectEditor.Utils
{
    /// <summary>
    ///     Summary description for FieldAccesor.
    /// </summary>
    public class FieldAccesor
    {
        private readonly string _fieldName;
        private readonly Type _targetType;
        private FieldInfo _fieldInfo;

        public FieldAccesor(object target, string fieldName)
            : this(target.GetType(), target, fieldName)
        {
        }

        public FieldAccesor(Type targetType, string fieldName)
            : this(targetType, null, fieldName)
        {
        }

        public FieldAccesor(Type targetType, object target, string fieldName)
        {
            Target = target;
            _targetType = targetType;
            _fieldName = fieldName;

            do
            {
                TryReadField(BindingFlags.Default);
                TryReadField(BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                TryReadField(BindingFlags.Static | BindingFlags.FlattenHierarchy);

                TryReadField(BindingFlags.NonPublic | BindingFlags.Instance);

                TryReadField(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                TryReadField(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy |
                             BindingFlags.GetField);
                TryReadField(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);

                TryReadField(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy |
                             BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.IgnoreCase |
                             BindingFlags.IgnoreReturn | BindingFlags.Instance | BindingFlags.PutDispProperty |
                             BindingFlags.PutRefDispProperty | BindingFlags.SetField | BindingFlags.Static);

                TryReadField(BindingFlags.NonPublic | BindingFlags.Static);
                TryReadField(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy |
                             BindingFlags.GetField);
                TryReadField(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField);

                if (_fieldInfo == null)
                {
                    _targetType = _targetType.BaseType;
                    if (_targetType == typeof(object))
                        break;
                }
            } while (_fieldInfo == null);
        }

        public object Target { get; set; }

        public bool IsValid => _fieldInfo != null;

        public object Value { get; private set; }

        private void SearchForField(BindingFlags bindingFlags)
        {
            if (_fieldInfo != null)
                return;

            var allFields = _targetType.GetFields(bindingFlags);
            foreach (var field in allFields)
                if (field.Name == _fieldName)
                {
                    _fieldInfo = field;
                    return;
                }
        }


        private void TryReadField(BindingFlags bindingFlags)
        {
            if (_fieldInfo != null)
                return;
            _fieldInfo = _targetType.GetField(_fieldName, bindingFlags);
            if (_fieldInfo == null)
                SearchForField(bindingFlags);
        }

        public void Save()
        {
            Value = _fieldInfo.GetValue(Target);
        }

        public void Clear()
        {
            _fieldInfo.SetValue(Target, null);
        }

        public void Restore()
        {
            _fieldInfo.SetValue(Target, Value);
        }

        public void Restore(object newValue)
        {
            _fieldInfo.SetValue(Target, newValue);
            Value = newValue;
        }
    }
}