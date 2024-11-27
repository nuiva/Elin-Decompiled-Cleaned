using System;
using System.Collections.Generic;

public class GoalVisitorGuest : Goal
{
	public override int MaxRestart
	{
		get
		{
			return 10;
		}
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		if (EClass.rnd(10) == 0 && this.owner.FindBed() == null)
		{
			this.owner.TryAssignBed();
		}
		yield return base.DoIdle(3);
		yield return base.Restart();
		yield break;
	}
}
