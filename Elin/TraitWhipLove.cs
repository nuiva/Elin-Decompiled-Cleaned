using System;
using System.Collections.Generic;

public class TraitWhipLove : TraitTool
{
	public override bool HasCharges
	{
		get
		{
			return true;
		}
	}

	public override void OnCreate(int lv)
	{
		this.owner.c_charges = EClass.rnd(7) + 3;
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		p.pos.ListCards(false).ForEach(delegate(Card a)
		{
			Chara c = a.Chara;
			if (c == null)
			{
				return;
			}
			List<Hobby> list = c.ListWorks(false);
			List<Hobby> list2 = c.ListHobbies(false);
			if (p.IsSelfOrNeighbor && EClass.pc.CanSee(a) && c.IsPCFaction && c.homeBranch != null && list.Count > 0 && list2.Count > 0)
			{
				p.TrySetAct("actWhip", delegate()
				{
					EClass.pc.Say("use_whip", c, this.owner, null, null);
					EClass.pc.Say("use_scope2", c, null, null);
					c.Talk("pervert2", null, null, false);
					EClass.pc.PlaySound("whip", 1f, true);
					c.PlayAnime(AnimeID.Shiver, false);
					c.OnInsulted();
					if (this is TraitWhipInterest)
					{
						c.bio.idInterest = EClass.rnd(Enum.GetNames(typeof(Interest)).Length);
						c.SetRandomTone();
						c.SetRandomTalk();
						EClass.pc.Say("use_whip4", c, null, null);
					}
					else
					{
						c.RerollHobby(true);
						Hobby hobby = c.ListWorks(false)[0];
						Hobby hobby2 = c.ListHobbies(false)[0];
						EClass.pc.Say("use_whip2", c, hobby.Name, hobby2.Name);
						c.RefreshWorkElements(c.homeBranch.elements);
					}
					this.owner.ModCharge(-1, false);
					if (this.owner.c_charges <= 0)
					{
						EClass.pc.Say("spellbookCrumble", this.owner, null, null);
						this.owner.Destroy();
					}
					return false;
				}, c, null, 1, false, true, false);
			}
		});
	}
}
