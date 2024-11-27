using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

public class AreaData : EClass
{
	public int group
	{
		get
		{
			return this.ints[1];
		}
		set
		{
			this.ints[1] = value;
		}
	}

	public int maxHeight
	{
		get
		{
			return this.ints[2];
		}
		set
		{
			this.ints[2] = value;
		}
	}

	public BaseArea.AccessType accessType
	{
		get
		{
			return this.ints[3].ToEnum<BaseArea.AccessType>();
		}
		set
		{
			this.ints[3] = (int)value;
		}
	}

	public bool showWallItem
	{
		get
		{
			return this.bits[0];
		}
		set
		{
			this.bits[0] = value;
		}
	}

	public bool atrium
	{
		get
		{
			return this.bits[1];
		}
		set
		{
			this.bits[1] = value;
		}
	}

	public bool visited
	{
		get
		{
			return this.bits[2];
		}
		set
		{
			this.bits[2] = value;
		}
	}

	[OnSerializing]
	private void _OnSerializing(StreamingContext context)
	{
		this.ints[0] = this.bits.ToInt();
	}

	[OnDeserialized]
	private void _OnDeserialized(StreamingContext context)
	{
		this.bits.SetInt(this.ints[0]);
	}

	[JsonProperty]
	public string name;

	[JsonProperty]
	public AreaType type;

	[JsonProperty]
	public int[] ints = new int[5];

	public BitArray32 bits;
}
