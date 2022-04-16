﻿using Stratus.OdinSerializer;

using System.IO;

namespace Stratus.Serialization
{
	/// <summary>
	/// A serializer using the binary format
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class StratusBinarySerializer<T> : StratusSerializer<T>
		where T : class, new()
	{
		protected override T OnDeserialize(string filePath)
		{
			byte[] serialization = File.ReadAllBytes(filePath);
			return SerializationUtility.DeserializeValue<T>(serialization, DataFormat.Binary);
		}

		protected override void OnSerialize(T value, string filePath)
		{
			byte[] serialization = SerializationUtility.SerializeValue(value, DataFormat.Binary);
			File.WriteAllBytes(filePath, serialization);
		}
	}

	/// <summary>
	/// A serializer using the binary format
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class StratusBinarySerializer : StratusSerializer
	{
		protected override object OnDeserialize(string filePath)
		{
			byte[] serialization = File.ReadAllBytes(filePath);
			return SerializationUtility.DeserializeValueWeak(serialization, DataFormat.Binary);
		}

		protected override void OnSerialize(object value, string filePath)
		{
			byte[] serialization = SerializationUtility.SerializeValue(value, DataFormat.Binary);
			File.WriteAllBytes(filePath, serialization);
		}
	}
}