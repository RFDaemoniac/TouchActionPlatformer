using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;


namespace NodeCanvas{

	///Helper reflection extension methods to work with NETFX_CORE
	public static class NCReflection {

		private static IEnumerable GetBaseTypes(Type type){
			
			yield return type;
			Type baseType;

			#if NETFX_CORE
			baseType = type.GetTypeInfo().BaseType;
			#else
			baseType = type.BaseType;
			#endif

			if (baseType != null){
				foreach (var t in GetBaseTypes(baseType)){
					yield return t;
				}
			}
		}

		public static bool NCIsAssignableFrom(this Type type, Type second){
			#if NETFX_CORE
			return type.GetTypeInfo().IsAssignableFrom(second.GetTypeInfo());
			#else
			return type.IsAssignableFrom(second);
			#endif
		}

		public static bool NCIsAbstract(this Type type){
			#if NETFX_CORE
			return type.GetTypeInfo().IsAbstract;
			#else
			return type.IsAbstract;
			#endif			
		}

		public static bool NCIsInterface(this Type type){
			#if NETFX_CORE
			return type.GetTypeInfo().IsInterface;
			#else
			return type.IsInterface;
			#endif			
		}

		public static FieldInfo NCGetField(this Type type, string name){
			#if NETFX_CORE
			return GetBaseTypes(type).OfType<Type>().Select(baseType => baseType.GetTypeInfo().GetDeclaredField(name)).FirstOrDefault(field => field != null);
			#else
			return type.GetField(name, BindingFlags.Instance | BindingFlags.Public);
			#endif
		}

		public static PropertyInfo NCGetProperty(this Type type, string name){
			#if NETFX_CORE
			return GetBaseTypes(type).OfType<Type>().Select(baseType => baseType.GetTypeInfo().GetDeclaredProperty(name)).FirstOrDefault(property => property != null);
			#else
			return type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
			#endif
		}

		public static MethodInfo NCGetMethod(this Type type, string name, bool includePrivate = false){

			#if NETFX_CORE
			var methods = GetBaseTypes(type).OfType<Type>().Select(baseType => baseType.GetTypeInfo().DeclaredMethods).ToList();
			foreach (MethodInfo m in methods){
				if (m.Name == name){
					if (m.IsPrivate && includePrivate)
						return m;
					if (m.IsPublic)
						return m;
				}
			}

			#else
			if (includePrivate)
				return type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			return type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public);
			#endif
		}

		public static MethodInfo NCGetMethod(this Type type, string name, Type[] paramTypes){
			#if NETFX_CORE
			return type.NCGetMethod(name);
			#else
			return type.GetMethod(name, paramTypes);
			#endif
		}

		public static EventInfo NCGetEvent(this Type type, string name){
			#if NETFX_CORE
			return GetBaseTypes(type).OfType<Type>().Select(baseType => baseType.GetTypeInfo().GetDeclaredEvent(name)).FirstOrDefault(method => method != null);
			#else
			return type.GetEvent(name, BindingFlags.Instance | BindingFlags.Public);
			#endif			
		}

		public static FieldInfo[] NCGetFields(this Type type){

			#if NETFX_CORE
			var fields = new List<FieldInfo>();
			foreach (Type t in GetBaseTypes(type).OfType<Type>())
				fields.AddRange(t.GetTypeInfo().DeclaredFields);
			return fields.ToArray();
			#else
			return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			#endif
		}


		public static Attribute NCGetAttribute(this Type type, Type attType, bool inherited){
			#if NETFX_CORE
			return type.GetTypeInfo().GetCustomAttributes(attType, inherited).FirstOrDefault() as Attribute;
			#else
			return type.GetCustomAttributes(attType, inherited).FirstOrDefault() as Attribute;
			#endif			
		}

        public static Type[] NCGetGenericArguments(this Type type){
			#if NETFX_CORE
            return type.GetTypeInfo().GenericTypeArguments;
			#else
            return type.GetGenericArguments();
			#endif
        }

        public static Type[] NCGetEmptyTypes(){
			#if NETFX_CORE
			return new Type[0];
			#else
            return Type.EmptyTypes;
			#endif
        }
        
        public static System.Delegate NCCreateDelegate(Type type, object arg, MethodInfo methodInfo){
			#if NETFX_CORE
			return methodInfo.CreateDelegate(type, arg);
			#else
            return System.Delegate.CreateDelegate(type, arg, methodInfo);
			#endif
        }

		private static List<Assembly> loadedAssemblies;
		public static List<Type> GetAssemblyTypes() {
			#if NETFX_CORE
		    if (loadedAssemblies != null)
		        return loadedAssemblies.Select(t => t.GetType()).ToList();

		    var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;

		    loadedAssemblies = new List<Assembly>();
		    var folderFilesAsync = folder.GetFilesAsync();
		    folderFilesAsync.AsTask().Wait();

		    foreach (var file in folderFilesAsync.GetResults()){
		        if (file.FileType == ".dll" || file.FileType == ".exe"){
		            try
		            {
		                var filename = file.Name.Substring(0, file.Name.Length - file.FileType.Length);
		                AssemblyName name = new AssemblyName { Name = filename };
		                Assembly asm = Assembly.Load(name);
		                loadedAssemblies.Add(asm);
		            }
		            catch (BadImageFormatException)
		            {
		                // Thrown reflecting on C++ executable files for which the C++ compiler stripped the relocation addresses
		            }
		        }
		    }
		    return loadedAssemblies.Select(t => t.GetType()).ToList();
			#else

			var types = new List<Type>();
		    foreach(Assembly ass in System.AppDomain.CurrentDomain.GetAssemblies())
		    	try
		    	{
		    		types.AddRange(ass.GetTypes());
		    	}
		    	catch
		    	{
		    		Debug.Log(ass.FullName + " will be excluded");
		    		continue;
		    	}
		    return types;
		    #endif
		}
	}
}