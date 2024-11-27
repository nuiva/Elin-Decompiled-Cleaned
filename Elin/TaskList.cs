using System;

public class TaskList : EClass
{
	public virtual Task GetTask(Chara chara, int radius = -1)
	{
		return null;
	}

	public virtual void OnLoad()
	{
	}

	public virtual void SetAstLastItem(Task t)
	{
	}

	public virtual bool TryAdd(Task t)
	{
		return false;
	}

	public virtual void Remove(Task t)
	{
	}
}
