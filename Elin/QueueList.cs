public class QueueList<T> : TaskList<T> where T : Task
{
	public override Task GetTask(Chara chara, int radius = -1)
	{
		if (items.Count == 0)
		{
			return null;
		}
		foreach (T item in items)
		{
			if (item.IsRunning)
			{
				return null;
			}
			if (item.nextTry <= EClass.game.sessionMin && item.CanPerformTask(chara, radius))
			{
				return item;
			}
		}
		return null;
	}
}
