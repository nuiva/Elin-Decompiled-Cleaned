using System;

public class TraitPotionEmpty : TraitDrink
{
	public override ThrowType ThrowType
	{
		get
		{
			return ThrowType.Vase;
		}
	}

	public override bool CanDrink(Chara c)
	{
		return false;
	}

	public override bool CanUse(Chara c, Point p)
	{
		return this.GetWell(p) != null || p.cell.IsTopWaterAndNoSnow;
	}

	public override bool OnUse(Chara c, Point p)
	{
		TraitWell well = this.GetWell(p);
		if (well != null && well.Charges <= 0)
		{
			c.Say("drinkWell_empty", c, well.owner, null, null);
			return false;
		}
		SE.Play("water_farm");
		this.owner.ModNum(-1, true);
		Thing thing;
		if (well != null && well.IsHoly)
		{
			thing = ThingGen.Create((this.owner.id == "bucket_empty") ? "bucket" : "water", -1, -1);
			thing.SetBlessedState(BlessedState.Blessed);
		}
		else
		{
			thing = ThingGen.Create("potion", -1, 10);
		}
		c.Say("drawWater", this.owner.Duplicate(1), thing, null, null);
		c.Pick(thing, true, true);
		if (well != null)
		{
			well.ModCharges(-1);
		}
		return true;
	}

	public override void OnDrink(Chara c)
	{
	}

	public TraitWell GetWell(Point p)
	{
		foreach (Card card in p.ListCards(false))
		{
			TraitWell traitWell = card.trait as TraitWell;
			if (traitWell != null)
			{
				return traitWell;
			}
		}
		return null;
	}
}
