using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class ClassWithBits
{
	[JsonProperty]
	public int _bits;

	public BitArray32 bits;

	[OnSerializing]
	internal void OnSerializing(StreamingContext context)
	{
		_bits = (int)bits.Bits;
	}

	[OnDeserialized]
	internal void _OnDeserialized(StreamingContext context)
	{
		bits.Bits = (uint)_bits;
	}
}
