using System.Runtime.Serialization;
using Newtonsoft.Json;

public class AreaData : EClass
{
	[JsonProperty]
	public string name;

	[JsonProperty]
	public AreaType type;

	[JsonProperty]
	public int[] ints = new int[5];

	public BitArray32 bits;

	public int group
	{
		get
		{
			return ints[1];
		}
		set
		{
			ints[1] = value;
		}
	}

	public int maxHeight
	{
		get
		{
			return ints[2];
		}
		set
		{
			ints[2] = value;
		}
	}

	public BaseArea.AccessType accessType
	{
		get
		{
			return ints[3].ToEnum<BaseArea.AccessType>();
		}
		set
		{
			ints[3] = (int)value;
		}
	}

	public bool showWallItem
	{
		get
		{
			return bits[0];
		}
		set
		{
			bits[0] = value;
		}
	}

	public bool atrium
	{
		get
		{
			return bits[1];
		}
		set
		{
			bits[1] = value;
		}
	}

	public bool visited
	{
		get
		{
			return bits[2];
		}
		set
		{
			bits[2] = value;
		}
	}

	[OnSerializing]
	private void _OnSerializing(StreamingContext context)
	{
		ints[0] = bits.ToInt();
	}

	[OnDeserialized]
	private void _OnDeserialized(StreamingContext context)
	{
		bits.SetInt(ints[0]);
	}
}
