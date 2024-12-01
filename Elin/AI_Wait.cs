using System.Collections.Generic;

public class AI_Wait : AIAct
{
	public int count;

	public override IEnumerable<Status> Run()
	{
		for (int i = 0; i < count; i++)
		{
			yield return Status.Running;
		}
	}
}
