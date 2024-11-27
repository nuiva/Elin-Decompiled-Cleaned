using System;
using System.Collections.Generic;

public class AI_ArmPillow : AI_Massage
{
	public override IEnumerable<AIAct.Status> Run()
	{
		this.target.Say("armpillow_start", this.target, this.owner, null, null);
		this.isFail = (() => !this.target.IsAliveInCurrentZone || this.owner.Dist(this.target) > 3);
		int i = 0;
		while (this.target.HasCondition<ConSleep>())
		{
			yield return base.DoGoto(this.target.pos, 1, false, null);
			this.owner.LookAt(this.target);
			if (i % 30 == 20)
			{
				this.owner.Talk("goodBoy", null, null, false);
			}
			int num = i;
			i = num + 1;
		}
		this.target.Say("armpillow_end", this.target, this.owner, null, null);
		base.Finish(this.owner, this.target, 50);
		yield break;
	}
}
