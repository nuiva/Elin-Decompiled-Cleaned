using System;
using System.Collections.Generic;

public class AI_Wait : AIAct
{
	public override IEnumerable<AIAct.Status> Run()
	{
		int num;
		for (int i = 0; i < this.count; i = num + 1)
		{
			yield return AIAct.Status.Running;
			num = i;
		}
		yield break;
	}

	public int count;
}
