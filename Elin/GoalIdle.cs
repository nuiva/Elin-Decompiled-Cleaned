using System.Collections.Generic;

public class GoalIdle : Goal
{
	public override IEnumerable<Status> Run()
	{
		if (EClass.rnd(EClass._zone.IsTown ? 500 : 12) == 0 && !owner.c_isPrayed)
		{
			TraitAltar altar = AI_Pray.GetAltar(owner);
			owner.c_isPrayed = true;
			if (altar != null)
			{
				if (owner.noMove)
				{
					AI_Pray.Pray(owner);
				}
				else
				{
					yield return Do(new AI_Pray
					{
						altar = altar
					});
				}
			}
		}
		yield return DoIdle();
	}
}
