using System.Collections.Generic;

public class GoalWait : Goal
{
	public override bool UseTurbo => false;

	public override bool InformCancel => false;

	public override IEnumerable<Status> Run()
	{
		owner.renderer.NextFrame();
		yield return Success();
	}
}
