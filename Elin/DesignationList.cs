using System;

public class DesignationList<T> : TaskList<T> where T : TaskDesignation
{
	public TaskManager.Designations Designations
	{
		get
		{
			return EClass._map.tasks.designations;
		}
	}

	public override void OnAdd(T t)
	{
		base.OnAdd(t);
		t.pos.ForeachMultiSize(t.W, t.H, delegate(Point p, bool main)
		{
			this.Designations.mapAll.Add(p.index, t);
			p.cell.GetOrCreateDetail().designation = t;
		});
	}

	protected override bool TryAdd(T t)
	{
		return this.CanAdd(t.pos) && base.TryAdd(t);
	}

	public bool CanAdd(Point p)
	{
		return p.IsValid && !this.Designations.mapAll.ContainsKey(p.index);
	}

	protected override void Remove(T t)
	{
		base.Remove(t);
		t.pos.ForeachMultiSize(t.W, t.H, delegate(Point p, bool main)
		{
			this.Designations.mapAll.Remove(p.index);
			p.cell.GetOrCreateDetail().designation = null;
			p.cell.TryDespawnDetail();
		});
	}
}
