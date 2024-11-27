using System;

public class QueueList<T> : TaskList<T> where T : Task
{
	public override Task GetTask(Chara chara, int radius = -1)
	{
		if (this.items.Count == 0)
		{
			return null;
		}
		foreach (T t in this.items)
		{
			if (t.IsRunning)
			{
				return null;
			}
			if (t.nextTry <= EClass.game.sessionMin && t.CanPerformTask(chara, radius))
			{
				return t;
			}
		}
		return null;
	}
}
