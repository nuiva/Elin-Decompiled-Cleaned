using System;
using System.Collections.Generic;
using Newtonsoft.Json;

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
public class TaskList<T> : TaskList where T : Task
{
	[JsonProperty]
	public List<T> items = new List<T>();

	public override bool TryAdd(Task t)
	{
		return TryAdd(t as T);
	}

	public override void Remove(Task t)
	{
		Remove(t as T);
	}

	public override void SetAstLastItem(Task t)
	{
		SetAstLastItem(t as T);
	}

	public override void OnLoad()
	{
		foreach (T item in items)
		{
			OnAdd(item);
		}
	}

	protected virtual bool TryAdd(T t)
	{
		items.Add(t);
		OnAdd(t);
		t.OnAdd();
		return true;
	}

	public virtual void OnAdd(T t)
	{
		t.taskList = this;
	}

	protected virtual void Remove(T t)
	{
		items.Remove(t);
	}

	public virtual void SetAstLastItem(T t)
	{
		items.Remove(t);
		items.Add(t);
	}

	public override Task GetTask(Chara chara, int radius = -1)
	{
		if (items.Count == 0)
		{
			return null;
		}
		foreach (T item in items)
		{
			if (item.owner == null && item.nextTry <= EClass.game.sessionMin && item.CanPerformTask(chara, radius))
			{
				return item;
			}
		}
		return null;
	}

	public bool HaveTask(Point p)
	{
		foreach (T item in items)
		{
			if ((item as TaskPoint).pos.Equals(p))
			{
				return true;
			}
		}
		return false;
	}

	public void DestroyAll<T1>() where T1 : T
	{
		foreach (T item in items.Copy())
		{
			if (item is T1)
			{
				item.Destroy();
			}
		}
	}

	public void DestroyAll(Func<T, bool> func)
	{
		foreach (T item in items.Copy())
		{
			if (func(item))
			{
				item.Destroy();
			}
		}
	}
}
