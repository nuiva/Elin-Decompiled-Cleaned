using System.Collections.Generic;

public class GoalNeeds : Goal
{
	public override IEnumerable<Status> Run()
	{
		for (int i = 0; i < 5; i++)
		{
			switch (EClass.rnd(5))
			{
			case 0:
				if (owner.hunger.value > 70)
				{
					yield return Do(new AI_Eat());
				}
				break;
			case 3:
				if (owner.bladder.value < 40)
				{
					yield return Do(new AI_Bladder());
				}
				break;
			}
			yield return Status.Running;
		}
	}
}
