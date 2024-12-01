using System.Collections.Generic;

public class AI_LeaveMap : AIAct
{
	public override IEnumerable<Status> Run()
	{
		Card card = EClass._map.Installed.traits.GetTraitSet<TraitSpotExit>().GetRandom()?.owner;
		if (card != null)
		{
			yield return DoGoto(card);
		}
		owner.visitorState = VisitorState.AboutToLeave;
	}
}
