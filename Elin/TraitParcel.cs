using System.Collections.Generic;
using System.Linq;

public class TraitParcel : TraitItem
{
	public override bool CanUseInUserZone => false;

	public override bool CanStack => false;

	public override string LangUse => "actOpen";

	public override bool OnUse(Chara c)
	{
		if (EClass._zone.IsRegion)
		{
			Msg.SayCannotUseHere();
			return false;
		}
		EClass.pc.Say("openDoor", EClass.pc, owner);
		List<Thing> list = owner.things.ToList();
		if (list.Count > 0)
		{
			SE.Play("dropReward");
			foreach (Thing item in list)
			{
				EClass.pc.Pick(item);
			}
		}
		owner.ModNum(-1);
		return base.OnUse(c);
	}

	public override void SetName(ref string s)
	{
		if (!owner.c_idRefName.IsEmpty() && owner.c_altName.IsEmpty())
		{
			s = "_written".lang(owner.c_idRefName, s);
		}
	}
}
