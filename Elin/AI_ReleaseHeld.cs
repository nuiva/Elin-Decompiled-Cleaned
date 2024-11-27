using System;
using System.Collections.Generic;

public class AI_ReleaseHeld : AIAct
{
	public override IEnumerable<AIAct.Status> Run()
	{
		yield return base.DoGoto(this.dest, 0, false, null);
		this.owner.DropHeld(null);
		yield break;
	}

	public Point dest;
}
