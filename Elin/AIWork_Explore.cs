using System;
using System.Collections.Generic;

public class AIWork_Explore : AIWork
{
	public override AIWork.Work_Type WorkType
	{
		get
		{
			return AIWork.Work_Type.Explore;
		}
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		yield return base.DoIdle(3);
		yield break;
	}
}
