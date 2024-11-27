using System;
using Newtonsoft.Json;

public class FactoryData : EClass
{
	public void OnAddedToMap()
	{
	}

	public void OnRemoveFromMap()
	{
		TaskCraft[] array = this.queues.items.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Destroy();
		}
	}

	[JsonProperty]
	public QueueList<TaskCraft> queues = new QueueList<TaskCraft>();
}
