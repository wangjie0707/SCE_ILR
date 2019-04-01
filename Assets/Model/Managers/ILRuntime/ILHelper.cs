using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Generated;
using ILRuntime.Runtime.Intepreter;
using UnityEngine;

namespace Myth
{
	public static class ILHelper
	{
		public static void InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
		{
            // 注册重定向函数

            // 注册委托
            appdomain.DelegateManager.RegisterMethodDelegate<object>();
            appdomain.DelegateManager.RegisterMethodDelegate<object[]>();
            appdomain.DelegateManager.RegisterMethodDelegate<long>();
            appdomain.DelegateManager.RegisterMethodDelegate<ulong>();
            appdomain.DelegateManager.RegisterMethodDelegate<int>();
            appdomain.DelegateManager.RegisterMethodDelegate<uint>();
            appdomain.DelegateManager.RegisterMethodDelegate<short>();
            appdomain.DelegateManager.RegisterMethodDelegate<ushort>();
            appdomain.DelegateManager.RegisterMethodDelegate<char>();
            appdomain.DelegateManager.RegisterMethodDelegate<string>();
            appdomain.DelegateManager.RegisterMethodDelegate<object, object>();
            appdomain.DelegateManager.RegisterMethodDelegate<int, object>();
            appdomain.DelegateManager.RegisterMethodDelegate<long, object>();
            appdomain.DelegateManager.RegisterMethodDelegate<string, object>();

            appdomain.DelegateManager.RegisterMethodDelegate<List<object>>();
			appdomain.DelegateManager.RegisterMethodDelegate<ILTypeInstance>();

            //ILRegType.RegisterMethodDelegate(appdomain);
            //ILRegType.RegisterFunctionDelegate(appdomain);
            //ILRegType.RegisterDelegateConvertor(appdomain);

            //CLR绑定
            CLRBindings.Initialize(appdomain);

            // 注册适配器
            Assembly assembly = typeof(GameEntry).Assembly;
			foreach (Type type in assembly.GetTypes())
			{
				object[] attrs = type.GetCustomAttributes(typeof(ILAdapterAttribute), false);
				if (attrs.Length == 0)
				{
					continue;
				}
                Debug.Log(type.FullName);
                object obj = Activator.CreateInstance(type);
				CrossBindingAdaptor adaptor = obj as CrossBindingAdaptor;
				if (adaptor == null)
				{
					continue;
				}

				appdomain.RegisterCrossBindingAdaptor(adaptor);
			}

			LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(appdomain);
		}
	}
}