using System;
using System.Collections.Generic;

public class GoalGraze : Goal
{
	public override IEnumerable<AIAct.Status> Run()
	{
		Point pos = this.GetPos();
		if (pos != null)
		{
			yield return base.DoGoto(pos, 0, false, null);
		}
		yield return base.DoIdle(20);
		yield break;
	}

	public Point GetPos()
	{
		this.owner.ClearBed(null);
		Thing thing = null;
		Rand.SetSeed(this.owner.uid);
		if (thing == null)
		{
			thing = EClass._map.FindThing(typeof(TraitSpotRanch), this.owner);
		}
		Rand.SetSeed(-1);
		if (thing != null)
		{
			return thing.trait.GetRandomPoint(null, null);
		}
		return null;
	}

	public override void OnSimulatePosition()
	{
		Point pos = this.GetPos();
		if (pos != null)
		{
			this.owner.MoveImmediate(pos, true, true);
		}
	}
}
