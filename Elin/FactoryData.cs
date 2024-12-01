using Newtonsoft.Json;

public class FactoryData : EClass
{
	[JsonProperty]
	public QueueList<TaskCraft> queues = new QueueList<TaskCraft>();

	public void OnAddedToMap()
	{
	}

	public void OnRemoveFromMap()
	{
		TaskCraft[] array = queues.items.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Destroy();
		}
	}
}
