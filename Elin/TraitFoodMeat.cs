using System;

public class TraitFoodMeat : TraitFood
{
	public override string LangUse
	{
		get
		{
			return "invBury";
		}
	}

	public override bool CanUse(Chara c, Card tg)
	{
		return tg.trait is TraitGrave;
	}

	public override bool OnUse(Chara c, Card tg)
	{
		c.Say("bury", this.owner, tg, null, null);
		c.PlaySound("mud", 1f, true);
		this.owner.Destroy();
		tg.ModEncLv(1);
		return true;
	}
}
