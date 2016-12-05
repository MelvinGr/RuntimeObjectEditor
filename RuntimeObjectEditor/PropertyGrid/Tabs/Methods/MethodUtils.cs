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
using System.Text;

namespace RuntimeObjectEditor.PropertyGrid.Tabs.Methods
{
    /// <summary>
    ///     Summary description for MethodUtils.
    /// </summary>
    public sealed class MethodUtils
    {
        private MethodUtils()
        {
        }

        public static PropertyDescriptorCollection GetMethodProperties(object obj)
        {
            var type = obj.GetType();

            if (obj is MethodPropertyDescriptor.MethodPropertyValueHolder)
            {
                var mobj = obj as MethodPropertyDescriptor.MethodPropertyValueHolder;
                return mobj.Method.GetChildProperties(null, null);
            }

            var methodDesc = new ArrayList();

            var depth = 1;
            do
            {
                var methods =
                    type.GetMethods(BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public |
                                    BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

                for (var i = 0; i < methods.Length; i++)
                    try
                    {
                        var method = methods[i];
                        if (!method.IsSpecialName || method.Name.EndsWith("get_Item") ||
                            method.Name.EndsWith("set_Item"))
                            methodDesc.Add(new MethodPropertyDescriptor(obj, method, type, depth));
                    }
                    catch (Exception)
                    {
                    }

                if (type == typeof(object))
                    break;

                type = type.BaseType;
                depth++;
            } while (true);

            methodDesc.Sort(new MethodNameComparer());

            var methodsDesc = (MethodPropertyDescriptor[]) methodDesc.ToArray(typeof(MethodPropertyDescriptor));
            return new PropertyDescriptorCollection(methodsDesc);
        }

        public static ArrayList GetMethodParams(MethodPropertyDescriptor methodDesc)
        {
            var list = new ArrayList();
            var paramInfo = methodDesc.MethodInfo.GetParameters();
            for (var i = 0; i < paramInfo.Length; i++)
            {
                var param = paramInfo[i];
                list.Add(new ParameterPropertyDescriptor(methodDesc, param));
            }
            return list;
        }


        public static string GetMethodSignature(MethodInfo method)
        {
            // Build Method Signature
            var builder = new StringBuilder();

            builder.Append(GetMethodAccess(method));

            builder.Append(method.ReturnType.Name);
            builder.Append(" ");

            builder.Append(method.Name);
            builder.Append(" ( ");

            var param = method.GetParameters();
            foreach (var parameterInfo in param)
            {
                builder.Append(parameterInfo.ParameterType.Name);
                builder.Append(" ");
                builder.Append(parameterInfo.Name);
                builder.Append(", ");
            }
            if (param.Length > 0)
                builder.Remove(builder.Length - 2, 2);

            builder.Append(" )");

            return builder.ToString();
        }

        public static string GetMethodAccess(MethodInfo method)
        {
            var builder = new StringBuilder();
            if (method.IsPrivate)
                builder.Append("private ");
            if (method.IsPublic)
                builder.Append("public ");
            if (method.IsAssembly)
                builder.Append("internal ");
            if (method.IsFamily)
                builder.Append("protected ");
            if (method.IsFamilyOrAssembly)
                builder.Append("internal protected ");
            if (method.IsFamilyAndAssembly)
                builder.Append("internal+protected ");

            if (method.IsStatic)
                builder.Append("static ");
            if (method.IsVirtual)
                builder.Append("virtual ");
            if (method.IsAbstract)
                builder.Append("abstract ");

            return builder.ToString();
        }

        public static string GetMethodAccessShort(MethodInfo method)
        {
            var builder = new StringBuilder();
            if (method.IsPublic)
                builder.Append("public ");

            if (method.IsFamilyAndAssembly || method.IsFamilyOrAssembly || method.IsFamily || method.IsPrivate ||
                method.IsAssembly)
                builder.Append("non-public ");

            if (method.IsStatic)
                builder.Append("static ");
            if (method.IsVirtual)
                builder.Append("virtual ");
            if (method.IsAbstract)
                builder.Append("abstract ");

            return builder.ToString();
        }

        private class MethodNameComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                if ((x == null) && (y == null))
                    return 0;
                if (x == null)
                    return 1;
                if (y == null)
                    return -1;
                var mx = (MethodPropertyDescriptor) x;
                var my = (MethodPropertyDescriptor) y;
                return string.Compare(mx.MethodInfo.Name, my.MethodInfo.Name);
            }
        }
    }
}