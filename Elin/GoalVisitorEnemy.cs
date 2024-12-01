using System.Collections.Generic;

public class GoalVisitorEnemy : Goal
{
	public override int MaxRestart => 10;

	public override IEnumerable<Status> Run()
	{
		yield return Do(new AI_GotoHearth());
		yield return Do(new AI_AttackHome());
		yield return Restart();
	}
}
