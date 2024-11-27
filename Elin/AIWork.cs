using System;
using System.Collections.Generic;

public class AIWork : AIAct
{
	public override string Name
	{
		get
		{
			return this.sourceWork.name_JP;
		}
	}

	public virtual int destDist
	{
		get
		{
			return 0;
		}
	}

	public override int MaxRestart
	{
		get
		{
			return 3;
		}
	}

	public virtual AIWork.Work_Type WorkType
	{
		get
		{
			return AIWork.Work_Type.Default;
		}
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		yield return base.DoIdle(100);
		AIWork.Work_Type workType = this.WorkType;
		this.SetDestPos();
		if (this.destPos != null)
		{
			yield return base.DoGoto(this.destPos, this.destDist, false, null);
		}
		else
		{
			this.destPos = new Point(this.owner.pos);
		}
		AIAct work = this.GetWork(this.destPos);
		if (work != null)
		{
			this.owner.Talk("work_" + this.sourceWork.talk, null, null, false);
			yield return base.Do(work, new Func<AIAct.Status>(base.KeepRunning));
		}
		yield return base.Restart();
		yield break;
	}

	public virtual AIAct GetWork(Point p)
	{
		return new AI_Idle();
	}

	public bool SetDestination()
	{
		if (this.WorkType == AIWork.Work_Type.Explore)
		{
			return EClass.world.date.IsExpired(this.owner.GetInt(51, null));
		}
		if (!this.sourceWork.workTag.IsEmpty())
		{
			if (this.destArea != null)
			{
				this.destThing = EClass._map.FindThing(this.sourceWork.workTag, this.destArea, null);
			}
			else
			{
				this.destThing = EClass._map.FindThing(this.sourceWork.workTag, this.owner);
			}
			if (this.destThing != null)
			{
				return true;
			}
		}
		if (this.sourceWork.destTrait.IsEmpty())
		{
			this.SetDestPos();
			return this.destPos != null;
		}
		if (this.destArea != null)
		{
			this.destThing = EClass._map.FindThing(Type.GetType("Trait" + this.sourceWork.destTrait + ", Elin"), this.destArea, null);
			return this.destThing != null;
		}
		this.destThing = EClass._map.FindThing(Type.GetType("Trait" + this.sourceWork.destTrait + ", Elin"), this.owner);
		return this.destThing != null;
	}

	public virtual void SetDestPos()
	{
		if (this.destThing != null && this.destThing.ExistsOnMap)
		{
			this.destPos = this.destThing.trait.GetRandomPoint(new Func<Point, bool>(this._FuncWorkPoint), null);
		}
	}

	public bool _FuncWorkPoint(Point p)
	{
		Room room = this.destThing.pos.cell.room;
		return p.cell.room == room && this.FuncWorkPoint(p);
	}

	public virtual bool FuncWorkPoint(Point p)
	{
		return true;
	}

	public virtual WorkSession GetSession()
	{
		WorkSession workSession = new WorkSession
		{
			id = this.sourceWork.id,
			efficiency = 80
		};
		if (this.destThing != null)
		{
			workSession.efficiency = this.destThing.GetEfficiency();
		}
		else if (this.destArea != null)
		{
			workSession.efficiency = this.destArea.type.GetEfficiency();
		}
		this.OnGetSession(workSession);
		return workSession;
	}

	public virtual void OnGetSession(WorkSession s)
	{
	}

	public void AddDeliverable(Thing t)
	{
		this.owner.GetWorkSummary().AddThing(t);
	}

	public virtual void OnPerformWork(bool realtime)
	{
	}

	public void DailyOutcome()
	{
	}

	public BaseArea destArea;

	public Point destPos;

	public Thing destThing;

	public SourceHobby.Row sourceWork;

	public enum Work_Type
	{
		Default,
		Explore
	}
}
