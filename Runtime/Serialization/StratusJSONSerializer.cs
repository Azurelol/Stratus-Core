using Stratus.OdinSerializer;

using System.IO;

using UnityEngine;

namespace Stratus.Serialization
{
	/// <summary>
	/// A serializer using the JSON format
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class StratusJSONSerializer<T> : StratusSerializer<T>
		where T : class, new()
	{
		protected override T OnDeserialize(string filePath)
		{
			byte[] serialization = File.ReadAllBytes(filePath);
			return SerializationUtility.DeserializeValue<T>(serialization, DataFormat.JSON);
		}

		protected override void OnSerialize(T value, string filePath)
		{
			byte[] serialization = SerializationUtility.SerializeValue(value, DataFormat.JSON);
			File.WriteAllBytes(filePath, serialization);
		}
	}

	/// <summary>
	/// A serializer using the JSON format
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class StratusJSONSerializer : StratusSerializer
	{
		protected override object OnDeserialize(string filePath)
		{
			byte[] serialization = File.ReadAllBytes(filePath);
			return SerializationUtility.DeserializeValueWeak(serialization, DataFormat.JSON);
		}

		protected override void OnSerialize(object value, string filePath)
		{
			byte[] serialization = SerializationUtility.SerializeValue(value, DataFormat.JSON);
			File.WriteAllBytes(filePath, serialization);
		}
	}

	public static class StratusJSONSerializerUtility
	{
		public static string Serialize(object value)
		{
			return JsonUtility.ToJson(value);
		}

		public static T Deserialize<T>(string serialization)
		{
			return JsonUtility.FromJson<T>(serialization);
		}

	}
}