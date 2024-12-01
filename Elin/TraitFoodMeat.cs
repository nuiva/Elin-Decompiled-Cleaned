public class TraitFoodMeat : TraitFood
{
	public override string LangUse => "invBury";

	public override bool CanUse(Chara c, Card tg)
	{
		return tg.trait is TraitGrave;
	}

	public override bool OnUse(Chara c, Card tg)
	{
		c.Say("bury", owner, tg);
		c.PlaySound("mud");
		owner.Destroy();
		tg.ModEncLv(1);
		return true;
	}
}
