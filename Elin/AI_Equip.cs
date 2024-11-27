using System;
using System.Collections.Generic;

public class AI_Equip : AIAct
{
	public override IEnumerable<AIAct.Status> Run()
	{
		if (this.target.ExistsOnMap)
		{
			yield return base.DoGoto(this.target, null);
		}
		if (this.target.ExistsOnMap)
		{
			this.owner.Pick(this.target, true, true);
		}
		if (this.target.parent != this.owner)
		{
			yield return this.Cancel();
		}
		this.owner.body.Equip(this.target, null, true);
		yield break;
	}

	public Thing target;
}
