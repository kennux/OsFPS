using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Linq;
using System.Linq;

namespace UnityTK.Prototypes
{
	/// <summary>
	/// UnityTK static cache class.
	/// 
	/// This will generate a type and serializer cache when <see cref="LazyInit"/> is called.
	/// LazyInit can be called whenever this is about to be accessed in order to make sure the prototype cache is ready.
	/// </summary>
	public static class PrototypeCaches
	{
		private static List<IPrototypeDataSerializer> serializers;
		private static Dictionary<Type, SerializableTypeCache> typeCache = new Dictionary<Type, SerializableTypeCache>();
		private static Type[] allTypes = null;

		/// <summary>
		/// Returns the best known data serializer for the specified type.
		/// Currently this will always return the first serializer found, TODO: Implement serializer rating and selecting the most appropriate one.
		/// </summary>
		/// <param name="type">The type to get a serializer for.</param>
		/// <returns>Null if not found, the serializer otherwise.</returns>
		public static IPrototypeDataSerializer GetBestSerializerFor(Type type)
		{
			if (ReferenceEquals(serializers, null))
			{
				serializers = new List<IPrototypeDataSerializer>();
				LazyAllTypesInit();
				
				int len = allTypes.Length;
				for (int i = 0; i < len; i++)
				{
					Type t = allTypes[i];
					if (t.IsClass && !t.IsAbstract && typeof(IPrototypeDataSerializer).IsAssignableFrom(t))
						serializers.Add(Activator.CreateInstance(t) as IPrototypeDataSerializer);
				}
			}

			foreach (var instance in serializers)
			{
				if (instance.CanBeUsedFor(type))
					return instance;
			}
			return null;
		}

		/// <summary>
		/// Returns the serializable type cache if known for the specified type.
		/// Will be cached just in time.
		/// </summary>
		public static SerializableTypeCache GetSerializableTypeCacheFor(Type type)
		{
			SerializableTypeCache cache;
			if (!typeCache.TryGetValue(type, out cache))
			{
				cache = SerializableTypeCache.TryBuild(type);
				typeCache.Add(type, cache);
			}
			return cache;
		}

		private static void LazyAllTypesInit()
		{
			if (ReferenceEquals(allTypes, null))
			{
				List<Type> types = new List<Type>();
				// Init all types cache
				foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
				{
					foreach (var type in asm.GetTypes())
					{
						types.Add(type);
					}
				}
				allTypes = types.ToArray();
			}
		}

		public static SerializableTypeCache GetSerializableTypeCacheFor(string writtenName, string preferredNamespace)
		{
			Type foundType = null;
			bool dontDoNamespaceCheck = string.IsNullOrEmpty(preferredNamespace);
			LazyAllTypesInit();

			int len = allTypes.Length;
			for (int i = 0; i < len; i++)
			{
				Type t = allTypes[i];
				if (t.Name.Equals(writtenName) && (dontDoNamespaceCheck || t.Namespace.Equals(preferredNamespace)))
				{
					foundType = t;
					break;
				}
			}

			if (!ReferenceEquals(foundType, null))
				return GetSerializableTypeCacheFor(foundType);

			return null;
		}
	}
}
