﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RWLayout.alpha2.FastAccess
{
    #region Delegates

    public delegate TResult ByRefFunc<TInstance, out TResult>(ref TInstance instance) where TInstance : struct;
    public delegate TResult ByRefFunc<TInstance, TArg0, out TResult>(ref TInstance instance, TArg0 arg0) where TInstance : struct;
    public delegate TResult ByRefFunc<TInstance, TArg0, TArg1, out TResult>(ref TInstance instance, TArg0 arg0, TArg1 arg1) where TInstance : struct;
    public delegate TResult ByRefFunc<TInstance, TArg0, TArg1, TArg2, out TResult>(ref TInstance instance, TArg0 arg0, TArg1 arg1, TArg2 arg2) where TInstance : struct;
    public delegate TResult ByRefFunc<TInstance, TArg0, TArg1, TArg2, TArg3, out TResult>(ref TInstance instance, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3) where TInstance : struct;
    public delegate TResult ByRefFunc<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4, out TResult>(ref TInstance instance, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4) where TInstance : struct;

    public delegate void ByRefAction<TInstance>(ref TInstance instance) where TInstance : struct;
    public delegate void ByRefAction<TInstance, TArg0>(ref TInstance instance, TArg0 arg0) where TInstance : struct;
    public delegate void ByRefAction<TInstance, TArg0, TArg1>(ref TInstance instance, TArg0 arg0, TArg1 arg1) where TInstance : struct;
    public delegate void ByRefAction<TInstance, TArg0, TArg1, TArg2>(ref TInstance instance, TArg0 arg0, TArg1 arg1, TArg2 arg2) where TInstance : struct;
    public delegate void ByRefAction<TInstance, TArg0, TArg1, TArg2, TArg3>(ref TInstance instance, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3) where TInstance : struct;
    public delegate void ByRefAction<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4>(ref TInstance instance, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4) where TInstance : struct;

    #endregion

    public partial class Dynamic
    {
        public static ByRefFunc<TInstance, TResult> StructRetMethod<TInstance, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : struct
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            if (method == null)
            {
                throw new MemberNotFoundException(methodName, typeof(TInstance));
            }
            return StructRetMethod<TInstance, TResult>(method);
        }

        public static ByRefFunc<TInstance, TArg0, TResult> StructRetMethod<TInstance, TArg0, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : struct
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            if (method == null)
            {
                throw new MemberNotFoundException(methodName, typeof(TInstance));
            }
            return StructRetMethod<TInstance, TArg0, TResult>(method);
        }

        public static ByRefFunc<TInstance, TArg0, TArg1, TResult> StructRetMethod<TInstance, TArg0, TArg1, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : struct
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            if (method == null)
            {
                throw new MemberNotFoundException(methodName, typeof(TInstance));
            }
            return StructRetMethod<TInstance, TArg0, TArg1, TResult>(method);
        }

        public static ByRefFunc<TInstance, TArg0, TArg1, TArg2, TResult> StructRetMethod<TInstance, TArg0, TArg1, TArg2, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : struct
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            if (method == null)
            {
                throw new MemberNotFoundException(methodName, typeof(TInstance));
            }
            return StructRetMethod<TInstance, TArg0, TArg1, TArg2, TResult>(method);
        }

        public static ByRefFunc<TInstance, TArg0, TArg1, TArg2, TArg3, TResult> StructRetMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : struct
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            if (method == null)
            {
                throw new MemberNotFoundException(methodName, typeof(TInstance));
            }
            return StructRetMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TResult>(method);
        }

        public static ByRefFunc<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4, TResult> StructRetMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : struct
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            if (method == null)
            {
                throw new MemberNotFoundException(methodName, typeof(TInstance));
            }
            return StructRetMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4, TResult>(method);
        }




        public static ByRefFunc<TInstance, TResult> StructRetMethod<TInstance, TResult>(MethodInfo method) where TInstance : struct
        {
            return CreateMethodCaller<ByRefFunc<TInstance, TResult>>(method);
        }

        public static ByRefFunc<TInstance, TArg0, TResult> StructRetMethod<TInstance, TArg0, TResult>(MethodInfo method) where TInstance : struct
        {
            return CreateMethodCaller<ByRefFunc<TInstance, TArg0, TResult>>(method);
        }

        public static ByRefFunc<TInstance, TArg0, TArg1, TResult> StructRetMethod<TInstance, TArg0, TArg1, TResult>(MethodInfo method) where TInstance : struct
        {
            return CreateMethodCaller<ByRefFunc<TInstance, TArg0, TArg1, TResult>>(method);
        }

        public static ByRefFunc<TInstance, TArg0, TArg1, TArg2, TResult> StructRetMethod<TInstance, TArg0, TArg1, TArg2, TResult>(MethodInfo method) where TInstance : struct
        {
            return CreateMethodCaller<ByRefFunc<TInstance, TArg0, TArg1, TArg2, TResult>>(method);
        }

        public static ByRefFunc<TInstance, TArg0, TArg1, TArg2, TArg3, TResult> StructRetMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TResult>(MethodInfo method) where TInstance : struct
        {
            return CreateMethodCaller<ByRefFunc<TInstance, TArg0, TArg1, TArg2, TArg3, TResult>>(method);
        }

        public static ByRefFunc<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4, TResult> StructRetMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4, TResult>(MethodInfo method) where TInstance : struct
        {
            return CreateMethodCaller<ByRefFunc<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4, TResult>>(method);
        }


        public static ByRefAction<TInstance> StructVoidMethod<TInstance>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : struct
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StructVoidMethod<TInstance>(method);
        }

        public static ByRefAction<TInstance, TArg0> StructVoidMethod<TInstance, TArg0>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : struct
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StructVoidMethod<TInstance, TArg0>(method);
        }

        public static ByRefAction<TInstance, TArg0, TArg1> StructVoidMethod<TInstance, TArg0, TArg1>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : struct
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StructVoidMethod<TInstance, TArg0, TArg1>(method);
        }

        public static ByRefAction<TInstance, TArg0, TArg1, TArg2> StructVoidMethod<TInstance, TArg0, TArg1, TArg2>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : struct
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StructVoidMethod<TInstance, TArg0, TArg1, TArg2>(method);
        }

        public static ByRefAction<TInstance, TArg0, TArg1, TArg2, TArg3> StructVoidMethod<TInstance, TArg0, TArg1, TArg2, TArg3>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : struct
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StructVoidMethod<TInstance, TArg0, TArg1, TArg2, TArg3>(method);
        }

        public static ByRefAction<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4> StructVoidMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : struct
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StructVoidMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4>(method);
        }




        public static ByRefAction<TInstance> StructVoidMethod<TInstance>(MethodInfo method) where TInstance : struct
        {
            return CreateMethodCaller<ByRefAction<TInstance>>(method);
        }

        public static ByRefAction<TInstance, TArg0> StructVoidMethod<TInstance, TArg0>(MethodInfo method) where TInstance : struct
        {
            return CreateMethodCaller<ByRefAction<TInstance, TArg0>>(method);
        }

        public static ByRefAction<TInstance, TArg0, TArg1> StructVoidMethod<TInstance, TArg0, TArg1>(MethodInfo method) where TInstance : struct
        {
            return CreateMethodCaller<ByRefAction<TInstance, TArg0, TArg1>>(method);
        }

        public static ByRefAction<TInstance, TArg0, TArg1, TArg2> StructVoidMethod<TInstance, TArg0, TArg1, TArg2>(MethodInfo method) where TInstance : struct
        {
            return CreateMethodCaller<ByRefAction<TInstance, TArg0, TArg1, TArg2>>(method);
        }

        public static ByRefAction<TInstance, TArg0, TArg1, TArg2, TArg3> StructVoidMethod<TInstance, TArg0, TArg1, TArg2, TArg3>(MethodInfo method) where TInstance : struct
        {
            return CreateMethodCaller<ByRefAction<TInstance, TArg0, TArg1, TArg2, TArg3>>(method);
        }

        public static ByRefAction<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4> StructVoidMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4>(MethodInfo method) where TInstance : struct
        {
            return CreateMethodCaller<ByRefAction<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4>>(method);
        }

    }
}
