using System;
using System.Collections.Generic;
using System.Linq;

public class TraitParcel : TraitItem
{
	public override bool CanUseInUserZone
	{
		get
		{
			return false;
		}
	}

	public override bool CanStack
	{
		get
		{
			return false;
		}
	}

	public override string LangUse
	{
		get
		{
			return "actOpen";
		}
	}

	public override bool OnUse(Chara c)
	{
		if (EClass._zone.IsRegion)
		{
			Msg.SayCannotUseHere();
			return false;
		}
		EClass.pc.Say("openDoor", EClass.pc, this.owner, null, null);
		List<Thing> list = this.owner.things.ToList<Thing>();
		if (list.Count > 0)
		{
			SE.Play("dropReward");
			foreach (Thing t in list)
			{
				EClass.pc.Pick(t, true, true);
			}
		}
		this.owner.ModNum(-1, true);
		return base.OnUse(c);
	}

	public override void SetName(ref string s)
	{
		if (!this.owner.c_idRefName.IsEmpty() && this.owner.c_altName.IsEmpty())
		{
			s = "_written".lang(this.owner.c_idRefName, s, null, null, null);
		}
	}
}
