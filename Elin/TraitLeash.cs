public class TraitLeash : TraitTool
{
	public override void TrySetHeldAct(ActPlan p)
	{
		p.pos.ListCards().ForEach(delegate(Card a)
		{
			Chara c = a.Chara;
			if (c != null && c.IsPCParty && !c.IsPC && p.IsSelfOrNeighbor && EClass.pc.CanSee(a))
			{
				p.TrySetAct(c.isLeashed ? "actUnleash" : "actLeash", delegate
				{
					EClass.pc.Say(c.isLeashed ? "use_leash2" : "use_leash", c, owner);
					EClass.pc.PlaySound("ride");
					c.isLeashed = !c.isLeashed;
					if (c.isLeashed)
					{
						c.Talk("pervert2");
					}
					c.PlayAnime(AnimeID.Shiver);
					return false;
				}, c);
			}
		});
	}
}
