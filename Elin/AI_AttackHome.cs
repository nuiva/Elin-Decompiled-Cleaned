using System;
using System.Collections.Generic;

public class AI_AttackHome : AIAct
{
	public override IEnumerable<AIAct.Status> Run()
	{
		Chara target = EClass.Branch.members.RandomItem<Chara>();
		if (target != null)
		{
			yield return base.DoGoto(target, null);
			this.owner.DoHostileAction(target.Chara, false);
		}
		yield break;
	}
}
