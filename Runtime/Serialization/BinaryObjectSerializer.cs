using Stratus.OdinSerializer;

using System.IO;

namespace Stratus.Serialization
{
	/// <summary>
	/// A serializer using the binary format
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class BinaryObjectSerializer : ObjectSerializer
	{
		protected override Result<T> OnDeserialize<T>(string filePath)
		{
			byte[] serialization = File.ReadAllBytes(filePath);
			return SerializationUtility.DeserializeValue<T>(serialization, DataFormat.Binary);
		}

		protected override Result OnSerialize<T>(T value, string filePath)
		{
			byte[] serialization = SerializationUtility.SerializeValue(value, DataFormat.Binary);
			File.WriteAllBytes(filePath, serialization);
			return true;
		}
	}
}