using System.Collections.Generic;

public class GoalVisitorGuest : Goal
{
	public override int MaxRestart => 10;

	public override IEnumerable<Status> Run()
	{
		if (EClass.rnd(10) == 0 && owner.FindBed() == null)
		{
			owner.TryAssignBed();
		}
		yield return DoIdle();
		yield return Restart();
	}
}
