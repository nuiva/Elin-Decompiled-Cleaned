using System.Collections.Generic;

public class AI_ArmPillow : AI_Massage
{
	public override IEnumerable<Status> Run()
	{
		target.Say("armpillow_start", target, owner);
		isFail = () => !target.IsAliveInCurrentZone || owner.Dist(target) > 3;
		int i = 0;
		while (target.HasCondition<ConSleep>())
		{
			yield return DoGoto(target.pos, 1);
			owner.LookAt(target);
			if (i % 30 == 20)
			{
				owner.Talk("goodBoy");
			}
			i++;
		}
		target.Say("armpillow_end", target, owner);
		Finish(owner, target, 50);
	}
}
