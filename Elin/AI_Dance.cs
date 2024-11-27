using System;
using System.Collections.Generic;

public class AI_Dance : AIAct
{
	public override IEnumerable<AIAct.Status> Run()
	{
		int num;
		for (int i = 0; i < 12; i = num + 1)
		{
			this.owner.Rotate(false);
			yield return base.DoWait(1);
			num = i;
		}
		yield break;
	}
}
