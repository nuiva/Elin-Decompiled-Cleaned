using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class ClassWithBits
{
	[OnSerializing]
	internal void OnSerializing(StreamingContext context)
	{
		this._bits = (int)this.bits.Bits;
	}

	[OnDeserialized]
	internal void _OnDeserialized(StreamingContext context)
	{
		this.bits.Bits = (uint)this._bits;
	}

	[JsonProperty]
	public int _bits;

	public BitArray32 bits;
}
