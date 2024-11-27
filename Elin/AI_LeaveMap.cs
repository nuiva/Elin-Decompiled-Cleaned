using System;
using System.Collections.Generic;

public class AI_LeaveMap : AIAct
{
	public override IEnumerable<AIAct.Status> Run()
	{
		Trait random = EClass._map.Installed.traits.GetTraitSet<TraitSpotExit>().GetRandom();
		Card card = (random != null) ? random.owner : null;
		if (card != null)
		{
			yield return base.DoGoto(card, null);
		}
		this.owner.visitorState = VisitorState.AboutToLeave;
		yield break;
	}
}
