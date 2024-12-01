using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class PointList : List<Point>
{
	[JsonProperty]
	public int[] data;

	[OnSerializing]
	private void OnSerializing(StreamingContext context)
	{
		data = new int[base.Count * 2];
		int num = 0;
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			Point current = enumerator.Current;
			data[num] = current.x;
			data[num + 1] = current.z;
			num += 2;
		}
	}

	[OnDeserialized]
	private void OnDeserialized(StreamingContext context)
	{
		Deserialize();
	}

	public void Deserialize()
	{
		base.Capacity = data.Length / 2 + 8;
		for (int i = 0; i < data.Length / 2; i++)
		{
			Add(new Point(data[i * 2], data[i * 2 + 1]));
		}
	}
}
