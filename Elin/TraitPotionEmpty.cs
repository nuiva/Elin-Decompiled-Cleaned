public class TraitPotionEmpty : TraitDrink
{
	public override ThrowType ThrowType => ThrowType.Vase;

	public override bool CanDrink(Chara c)
	{
		return false;
	}

	public override bool CanUse(Chara c, Point p)
	{
		if (GetWell(p) == null)
		{
			return p.cell.IsTopWaterAndNoSnow;
		}
		return true;
	}

	public override bool OnUse(Chara c, Point p)
	{
		TraitWell well = GetWell(p);
		if (well != null && well.Charges <= 0)
		{
			c.Say("drinkWell_empty", c, well.owner);
			return false;
		}
		SE.Play("water_farm");
		owner.ModNum(-1);
		Thing thing = null;
		if (well != null && well.IsHoly)
		{
			thing = ThingGen.Create((owner.id == "bucket_empty") ? "bucket" : "water");
			thing.SetBlessedState(BlessedState.Blessed);
		}
		else
		{
			thing = ThingGen.Create("potion", -1, 10);
		}
		c.Say("drawWater", owner.Duplicate(1), thing);
		c.Pick(thing);
		well?.ModCharges(-1);
		return true;
	}

	public override void OnDrink(Chara c)
	{
	}

	public TraitWell GetWell(Point p)
	{
		foreach (Card item in p.ListCards())
		{
			if (item.trait is TraitWell result)
			{
				return result;
			}
		}
		return null;
	}
}
