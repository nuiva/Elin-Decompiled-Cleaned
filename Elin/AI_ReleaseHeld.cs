using System.Collections.Generic;

public class AI_ReleaseHeld : AIAct
{
	public Point dest;

	public override IEnumerable<Status> Run()
	{
		yield return DoGoto(dest);
		owner.DropHeld();
	}
}
