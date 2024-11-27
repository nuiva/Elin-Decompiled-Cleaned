using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class PointList : List<Point>
{
	[OnSerializing]
	private void OnSerializing(StreamingContext context)
	{
		this.data = new int[base.Count * 2];
		int num = 0;
		foreach (Point point in this)
		{
			this.data[num] = point.x;
			this.data[num + 1] = point.z;
			num += 2;
		}
	}

	[OnDeserialized]
	private void OnDeserialized(StreamingContext context)
	{
		this.Deserialize();
	}

	public void Deserialize()
	{
		base.Capacity = this.data.Length / 2 + 8;
		for (int i = 0; i < this.data.Length / 2; i++)
		{
			base.Add(new Point(this.data[i * 2], this.data[i * 2 + 1]));
		}
	}

	[JsonProperty]
	public int[] data;
}
