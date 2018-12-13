using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Xml.Linq;
using System.Linq;
using System.Globalization;

namespace UnityTK.Prototypes
{
	public abstract class ValueTypePrototypeSerializer<T> : IPrototypeDataSerializer
	{
		public bool CanBeUsedFor(Type type)
		{
			return ReferenceEquals(type, typeof(T));
		}

		public object Deserialize(Type type, XElement value, PrototypeParserState state)
		{
			return _Deserialize(value.Value, state);
		}

		protected abstract T _Deserialize(string value, PrototypeParserState state);
	}
	
	public class PrototypeSerializer_Float : ValueTypePrototypeSerializer<float>
	{
		protected override float _Deserialize(string value, PrototypeParserState state)
		{
			return float.Parse(value, CultureInfo.InvariantCulture);
		}
	}
	
	public class PrototypeSerializer_Int : ValueTypePrototypeSerializer<int>
	{
		protected override int _Deserialize(string value, PrototypeParserState state)
		{
			return int.Parse(value);
		}
	}
	
	public class PrototypeSerializer_String : ValueTypePrototypeSerializer<string>
	{
		protected override string _Deserialize(string value, PrototypeParserState state)
		{
			return value;
		}
	}
	
	public class PrototypeSerializer_Double : ValueTypePrototypeSerializer<double>
	{
		protected override double _Deserialize(string value, PrototypeParserState state)
		{
			return double.Parse(value, CultureInfo.InvariantCulture);
		}
	}
	
	public class PrototypeSerializer_Short : ValueTypePrototypeSerializer<short>
	{
		protected override short _Deserialize(string value, PrototypeParserState state)
		{
			return short.Parse(value);
		}
	}
	
	public class PrototypeSerializer_Byte : ValueTypePrototypeSerializer<byte>
	{
		protected override byte _Deserialize(string value, PrototypeParserState state)
		{
			return byte.Parse(value);
		}
	}
	
	public class PrototypeSerializer_Bool : ValueTypePrototypeSerializer<bool>
	{
		protected override bool _Deserialize(string value, PrototypeParserState state)
		{
			return bool.Parse(value);
		}
	}

	public class PrototypeSerializer_Enum : IPrototypeDataSerializer
	{
		public bool CanBeUsedFor(Type type)
		{
			return type.IsEnum;
		}

		public object Deserialize(Type type, XElement value, PrototypeParserState state)
		{
			return Enum.Parse(type, value.Value as string);
		}
	}

	public class PrototypeSerializer_Type : ValueTypePrototypeSerializer<Type>
	{
		protected override Type _Deserialize(string value, PrototypeParserState state)
		{
			// Create std namespace name string by prepending std namespace to value
			bool doStdNamespaceCheck = !value.Contains('.');
			string stdNamespacePrepended = doStdNamespaceCheck ? state.parameters.standardNamespace + "." + value : null;

			// Look for type
			foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				var t = asm.GetType(value, false, false);
				if (doStdNamespaceCheck && ReferenceEquals(t, null))
					t = asm.GetType(stdNamespacePrepended, false, false);

				if (!ReferenceEquals(t, null))
					return t;
			}

			return null;
		}
	}
}