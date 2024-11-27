using System;
using System.Collections.Generic;

public class AI_GotoHearth : AIAct
{
	public override IEnumerable<AIAct.Status> Run()
	{
		Trait random = EClass._map.Installed.traits.GetTraitSet<TraitHearth>().GetRandom();
		Thing card = (random != null) ? random.owner.Thing : null;
		yield return base.DoGoto(card, null);
		this.owner.Talk("visitor_greet", null, null, false);
		this.owner.visitorState = VisitorState.Idle;
		yield break;
	}
}
