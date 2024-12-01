using System.Collections.Generic;

public class AI_Dance : AIAct
{
	public override IEnumerable<Status> Run()
	{
		for (int i = 0; i < 12; i++)
		{
			owner.Rotate();
			yield return DoWait();
		}
	}
}
