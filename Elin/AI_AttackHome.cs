using System.Collections.Generic;

public class AI_AttackHome : AIAct
{
	public override IEnumerable<Status> Run()
	{
		Chara target = EClass.Branch.members.RandomItem();
		if (target != null)
		{
			yield return DoGoto(target);
			owner.DoHostileAction(target.Chara);
		}
	}
}
