using System;
using System.Collections.Generic;

public class GoalVisitorEnemy : Goal
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
		yield return base.Do(new AI_GotoHearth(), null);
		yield return base.Do(new AI_AttackHome(), null);
		yield return base.Restart();
		yield break;
	}
}
