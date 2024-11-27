using System;
using System.Collections.Generic;

public class GoalIdle : Goal
{
	public override IEnumerable<AIAct.Status> Run()
	{
		if (EClass.rnd(EClass._zone.IsTown ? 500 : 12) == 0 && !this.owner.c_isPrayed)
		{
			TraitAltar altar = AI_Pray.GetAltar(this.owner);
			this.owner.c_isPrayed = true;
			if (altar != null)
			{
				yield return base.Do(new AI_Pray
				{
					altar = altar
				}, null);
			}
		}
		yield return base.DoIdle(3);
		yield break;
	}
}
