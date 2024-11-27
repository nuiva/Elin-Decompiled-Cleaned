using System;
using Newtonsoft.Json;
using UnityEngine;

public class GlobalGoalVisitAndStay : GlobalGoal
{
	public Zone destZone
	{
		get
		{
			return RefZone.Get(this.uidZone);
		}
	}

	public override void OnAdvanceHour()
	{
		if (this.owner.currentZone == EClass.game.activeZone)
		{
			return;
		}
		if (this.uidZone == 0)
		{
			if (this.GetDestZone() == null)
			{
				base.Kill();
				return;
			}
			this.uidZone = this.GetDestZone().uid;
			if (EClass.debug.logAdv)
			{
				Debug.Log(string.Concat(new string[]
				{
					this.owner.Name,
					" -> ",
					this.destZone.Name,
					" / ",
					(this != null) ? this.ToString() : null
				}));
			}
		}
		if (this.owner.currentZone == this.destZone)
		{
			this.OnStay();
			if (this.hours > 64 && EClass.rnd(48) == 0)
			{
				base.Complete();
				return;
			}
		}
		else
		{
			this.OnTravel();
			if (this.hours > 6 && EClass.rnd(4) == 0 && this.destZone != EClass.game.activeZone)
			{
				if (EClass.debug.logAdv)
				{
					Debug.Log(this.owner.Name + " reached " + this.destZone.Name);
				}
				this.owner.MoveZone(this.destZone, ZoneTransition.EnterState.RandomVisit);
			}
		}
	}

	public virtual void OnStay()
	{
	}

	public virtual void OnTravel()
	{
	}

	public virtual Zone GetDestZone()
	{
		return this.owner.homeZone;
	}

	[JsonProperty]
	public int uidZone;
}
