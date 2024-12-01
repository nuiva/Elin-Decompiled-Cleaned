public class DesignationList<T> : TaskList<T> where T : TaskDesignation
{
	public TaskManager.Designations Designations => EClass._map.tasks.designations;

	public override void OnAdd(T t)
	{
		base.OnAdd(t);
		t.pos.ForeachMultiSize(t.W, t.H, delegate(Point p, bool main)
		{
			Designations.mapAll.Add(p.index, t);
			p.cell.GetOrCreateDetail().designation = t;
		});
	}

	protected override bool TryAdd(T t)
	{
		if (!CanAdd(t.pos))
		{
			return false;
		}
		return base.TryAdd(t);
	}

	public bool CanAdd(Point p)
	{
		if (!p.IsValid || Designations.mapAll.ContainsKey(p.index))
		{
			return false;
		}
		return true;
	}

	protected override void Remove(T t)
	{
		base.Remove(t);
		t.pos.ForeachMultiSize(t.W, t.H, delegate(Point p, bool main)
		{
			Designations.mapAll.Remove(p.index);
			p.cell.GetOrCreateDetail().designation = null;
			p.cell.TryDespawnDetail();
		});
	}
}
