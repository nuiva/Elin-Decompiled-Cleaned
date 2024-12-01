using System.Collections.Generic;

public class AI_Equip : AIAct
{
	public Thing target;

	public override IEnumerable<Status> Run()
	{
		if (target.ExistsOnMap)
		{
			yield return DoGoto(target);
		}
		if (target.ExistsOnMap)
		{
			owner.Pick(target);
		}
		if (target.parent != owner)
		{
			yield return Cancel();
		}
		owner.body.Equip(target);
	}
}
