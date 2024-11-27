using System;
using UnityEngine;

public class ZonePreEnterBoutWin : ZonePreEnterEvent
{
	public override void Execute()
	{
		if (this.target == null)
		{
			string str = "exception: target not found:";
			Chara chara = this.target;
			Debug.LogError(str + ((chara != null) ? chara.ToString() : null));
			return;
		}
		Thing thing = null;
		int num = 0;
		foreach (Thing thing2 in this.target.things)
		{
			if (thing2.isEquipped && !thing2.isGifted && (thing2.GetPrice(CurrencyType.Money, false, PriceType.Default, null) > num || thing2.rarity >= Rarity.Artifact))
			{
				thing = thing2;
				num = thing2.GetPrice(CurrencyType.Money, false, PriceType.Default, null);
			}
		}
		if (thing == null)
		{
			thing = ThingGen.Create("plat", -1, -1).SetNum(EClass.rndHalf(this.target.LV / 10 + 2));
		}
		else
		{
			this.target.RemoveCard(thing);
			this.target.EQ_CAT(thing.category.id);
		}
		this.target.SetInt(111, this.target.GetInt(111, null) + 1);
		SE.Play("questComplete");
		EClass._map.TrySmoothPick(EClass.pc.pos, thing, EClass.pc);
		EClass.player.ModFame(EClass.rndHalf(Mathf.Max(10, 10 + (this.target.LV - EClass.pc.LV) * 3)));
	}

	public Chara target;
}
