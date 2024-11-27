using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class TaskList<T> : TaskList where T : Task
{
	public override bool TryAdd(Task t)
	{
		return this.TryAdd(t as T);
	}

	public override void Remove(Task t)
	{
		this.Remove(t as T);
	}

	public override void SetAstLastItem(Task t)
	{
		this.SetAstLastItem(t as T);
	}

	public override void OnLoad()
	{
		foreach (T t in this.items)
		{
			this.OnAdd(t);
		}
	}

	protected virtual bool TryAdd(T t)
	{
		this.items.Add(t);
		this.OnAdd(t);
		t.OnAdd();
		return true;
	}

	public virtual void OnAdd(T t)
	{
		t.taskList = this;
	}

	protected virtual void Remove(T t)
	{
		this.items.Remove(t);
	}

	public virtual void SetAstLastItem(T t)
	{
		this.items.Remove(t);
		this.items.Add(t);
	}

	public override Task GetTask(Chara chara, int radius = -1)
	{
		if (this.items.Count == 0)
		{
			return null;
		}
		foreach (T t in this.items)
		{
			if (t.owner == null && t.nextTry <= EClass.game.sessionMin && t.CanPerformTask(chara, radius))
			{
				return t;
			}
		}
		return null;
	}

	public bool HaveTask(Point p)
	{
		using (List<T>.Enumerator enumerator = this.items.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if ((enumerator.Current as TaskPoint).pos.Equals(p))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void DestroyAll<T1>() where T1 : T
	{
		foreach (T t in this.items.Copy<T>())
		{
			if (t is T1)
			{
				t.Destroy();
			}
		}
	}

	public void DestroyAll(Func<T, bool> func)
	{
		foreach (T t in this.items.Copy<T>())
		{
			if (func(t))
			{
				t.Destroy();
			}
		}
	}

	[JsonProperty]
	public List<T> items = new List<T>();
}
