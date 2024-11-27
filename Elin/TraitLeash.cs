using System;

public class TraitLeash : TraitTool
{
	public override void TrySetHeldAct(ActPlan p)
	{
		p.pos.ListCards(false).ForEach(delegate(Card a)
		{
			Chara c = a.Chara;
			if (c == null || !c.IsPCParty || c.IsPC)
			{
				return;
			}
			if (p.IsSelfOrNeighbor && EClass.pc.CanSee(a))
			{
				p.TrySetAct(c.isLeashed ? "actUnleash" : "actLeash", delegate()
				{
					EClass.pc.Say(c.isLeashed ? "use_leash2" : "use_leash", c, this.owner, null, null);
					EClass.pc.PlaySound("ride", 1f, true);
					c.isLeashed = !c.isLeashed;
					if (c.isLeashed)
					{
						c.Talk("pervert2", null, null, false);
					}
					c.PlayAnime(AnimeID.Shiver, false);
					return false;
				}, c, null, 1, false, true, false);
			}
		});
	}
}
