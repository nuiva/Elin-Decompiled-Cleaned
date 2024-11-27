using System;
using System.Collections.Generic;

public class GoalWait : Goal
{
	public override bool UseTurbo
	{
		get
		{
			return false;
		}
	}

	public override bool InformCancel
	{
		get
		{
			return false;
		}
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		this.owner.renderer.NextFrame();
		yield return base.Success(null);
		yield break;
	}
}
