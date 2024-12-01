using Newtonsoft.Json;
using UnityEngine;

public class GlobalGoalVisitAndStay : GlobalGoal
{
	[JsonProperty]
	public int uidZone;

	public Zone destZone => RefZone.Get(uidZone);

	public override void OnAdvanceHour()
	{
		if (owner.currentZone == EClass.game.activeZone)
		{
			return;
		}
		if (uidZone == 0)
		{
			if (GetDestZone() == null)
			{
				Kill();
				return;
			}
			uidZone = GetDestZone().uid;
			if (EClass.debug.logAdv)
			{
				Debug.Log(owner.Name + " -> " + destZone.Name + " / " + this);
			}
		}
		if (owner.currentZone == destZone)
		{
			OnStay();
			if (hours > 64 && EClass.rnd(48) == 0)
			{
				Complete();
			}
			return;
		}
		OnTravel();
		if (hours > 6 && EClass.rnd(4) == 0 && destZone != EClass.game.activeZone)
		{
			if (EClass.debug.logAdv)
			{
				Debug.Log(owner.Name + " reached " + destZone.Name);
			}
			owner.MoveZone(destZone, ZoneTransition.EnterState.RandomVisit);
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
		return owner.homeZone;
	}
}
