using System;
using System.Collections.Generic;

public class GoalNeeds : Goal
{
	public override IEnumerable<AIAct.Status> Run()
	{
		int num;
		for (int i = 0; i < 5; i = num + 1)
		{
			switch (EClass.rnd(5))
			{
			case 0:
				if (this.owner.hunger.value > 70)
				{
					yield return base.Do(new AI_Eat(), null);
				}
				break;
			case 3:
				if (this.owner.bladder.value < 40)
				{
					yield return base.Do(new AI_Bladder(), null);
				}
				break;
			}
			yield return AIAct.Status.Running;
			num = i;
		}
		yield break;
	}
}
