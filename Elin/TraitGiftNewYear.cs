using System;

public class TraitGiftNewYear : TraitGiftPack
{
	public override bool OnUse(Chara c)
	{
		if (EClass._zone.IsRegion)
		{
			Msg.SayCannotUseHere();
			return false;
		}
		EClass.pc.Say("openDoor", EClass.pc, this.owner, null, null);
		SE.Play("dropReward");
		EClass.pc.Pick(ThingGen.Create("kagamimochi", -1, -1), true, true);
		EClass.pc.Pick(ThingGen.Create("1134", -1, -1), true, true);
		Chara chara = CharaGen.Create("putty_snow", -1);
		EClass._zone.AddCard(chara, (this.owner.ExistsOnMap ? this.owner.pos : EClass.pc.pos).GetNearestPoint(false, false, true, false));
		Msg.Say("package_chara", chara, this.owner, null, null);
		chara.MakeAlly(true);
		for (int i = 0; i < 3; i++)
		{
			chara = CharaGen.Create("bell_silver", -1);
			chara.c_originalHostility = (chara.hostility = Hostility.Neutral);
			EClass._zone.AddCard(chara, EClass._map.GetRandomSurface(false, true, false));
		}
		this.owner.ModNum(-1, true);
		return true;
	}
}
