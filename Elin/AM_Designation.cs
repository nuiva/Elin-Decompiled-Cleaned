using System;

public class AM_Designation<T> : AM_BaseTileSelect where T : TaskDesignation
{
	public virtual bool CanInstaComplete(T t)
	{
		return EClass.player.instaComplete || this.ForcedInstaComplete(t);
	}

	public virtual bool ForcedInstaComplete(T t)
	{
		return false;
	}

	public override string idSound
	{
		get
		{
			return "actionMode";
		}
	}

	public override bool IsBuildMode
	{
		get
		{
			return true;
		}
	}

	public virtual HitResult HitResultOnDesignation(Point p)
	{
		return HitResult.Outline;
	}

	public override void OnActivate()
	{
		this.CreateNewMold(false);
	}

	public void CreateNewMold(bool processing = false)
	{
		this.mold = Activator.CreateInstance<T>();
		this.OnCreateMold(processing);
	}

	public virtual void OnCreateMold(bool processing = false)
	{
	}

	public override HitResult HitTest(Point point, Point start)
	{
		if (!base.Summary.CanExecute())
		{
			return HitResult.Invalid;
		}
		if (point.HasDesignation)
		{
			return this.HitResultOnDesignation(point);
		}
		this.mold.pos.Set(point);
		HitResult hitResult = this.mold.GetHitResult();
		if (hitResult != HitResult.Default)
		{
			return hitResult;
		}
		return base.HitTest(point, start);
	}

	public override bool CanProcessTiles()
	{
		return base.Summary.CanExecute() && base.Summary.countValid != 0 && base.CanProcessTiles();
	}

	public override void OnBeforeProcessTiles()
	{
		this.CreateNewMold(true);
		EClass._map.tasks.undo.NewItem();
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		this.mold.pos.Set(point);
		if (this.list.TryAdd(this.mold))
		{
			EClass._map.tasks.undo.Add(this.mold);
			this.CreateNewMold(true);
		}
		T t = this.list.items.LastItem<T>();
		if (t != null && this.CanInstaComplete(t))
		{
			t.owner = EClass.player.Agent;
			EClass.player.Agent.pos.Set(t.pos);
			t.OnProgressComplete();
			t.Destroy();
		}
	}

	public override void OnAfterProcessTiles(Point start, Point end)
	{
		this.CreateNewMold(false);
	}

	public override void OnRefreshSummary(Point point, HitResult result, HitSummary summary)
	{
		summary.count++;
		if (result == HitResult.Valid || result == HitResult.Warning)
		{
			this.mold.pos.Set(point);
			if (this.CanInstaComplete(this.mold) && !this.ForcedInstaComplete(this.mold))
			{
				summary.money += this.CostMoney;
			}
			summary.countValid++;
		}
	}

	public TaskList<T> list;

	public T mold;
}
