using System.Collections.Generic;

public class AI_GotoHearth : AIAct
{
	public override IEnumerable<Status> Run()
	{
		Thing card = EClass._map.Installed.traits.GetTraitSet<TraitHearth>().GetRandom()?.owner.Thing;
		yield return DoGoto(card);
		owner.Talk("visitor_greet");
		owner.visitorState = VisitorState.Idle;
	}
}
