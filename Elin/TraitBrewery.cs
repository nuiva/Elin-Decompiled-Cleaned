using System;
using System.Collections.Generic;
using UnityEngine;

public class TraitBrewery : TraitContainer
{
	public override int DecaySpeedChild
	{
		get
		{
			return 500;
		}
	}

	public virtual string idMsg
	{
		get
		{
			return "brew";
		}
	}

	public virtual TraitBrewery.Type type
	{
		get
		{
			return TraitBrewery.Type.Drink;
		}
	}

	public virtual bool IsFood
	{
		get
		{
			return false;
		}
	}

	public override bool CanChildDecay(Card c)
	{
		string id = c.id;
		return !(id == "48") && !(id == "cheese") && !(id == "jerky") && (c.trait is TraitDrinkMilk || c.material.id == 94);
	}

	public override bool OnChildDecay(Card c, bool firstDecay)
	{
		TraitBrewery.Type type = this.type;
		if (type > TraitBrewery.Type.Drink)
		{
			if (type != TraitBrewery.Type.Fertilizer && !firstDecay)
			{
				return true;
			}
		}
		else
		{
			if (!firstDecay)
			{
				return true;
			}
			if (!c.IsFood && !(c.trait is TraitDrinkMilk))
			{
				return true;
			}
			if (c.category.IsChildOf("meal"))
			{
				return true;
			}
		}
		string productID = this.GetProductID(c);
		if (productID == null)
		{
			return true;
		}
		Thing thing = c.Duplicate(c.Num);
		c.Destroy();
		type = this.type;
		if (type != TraitBrewery.Type.Food)
		{
			if (type != TraitBrewery.Type.Fertilizer)
			{
				c = ThingGen.Create(productID, -1, -1).SetNum(thing.Num);
			}
			else
			{
				int num = 20 + thing.SelfWeight;
				if (num > 100)
				{
					num = 100 + (int)Mathf.Sqrt((float)((num - 100) * 10));
				}
				int num2 = 0;
				for (int i = 0; i < thing.Num; i++)
				{
					num2 += num / 100 + ((num % 100 > EClass.rnd(100)) ? 1 : 0);
				}
				if (num2 <= 0)
				{
					return false;
				}
				c = ThingGen.Create(productID, -1, -1).SetNum(num2);
			}
		}
		else
		{
			c = CraftUtil.MixIngredients(productID, new List<Thing>
			{
				c.Thing
			}, CraftUtil.MixType.Food, 0, EClass.pc).SetNum(thing.Num);
		}
		if (this.type != TraitBrewery.Type.Fertilizer)
		{
			c.MakeFoodRef(thing, null);
		}
		if (this.type == TraitBrewery.Type.Drink)
		{
			c.c_priceAdd = thing.GetValue(false) * 125 / 100;
		}
		this.OnProduce(c);
		this.owner.AddThing(c.Thing, false, -1, -1);
		this.owner.GetRootCard().Say(this.idMsg, thing, c, null, null);
		return false;
	}

	public virtual string GetProductID(Card c)
	{
		string id = c.id;
		if (id == "crim" || id == "drug_crim")
		{
			return "crimAle";
		}
		if (c.category.IsChildOf("mushroom") || c.category.IsChildOf("nuts"))
		{
			return "54";
		}
		return "48";
	}

	public virtual void OnProduce(Card c)
	{
	}

	public enum Type
	{
		Food,
		Drink,
		Fertilizer
	}
}
