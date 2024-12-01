using UnityEngine;

public class ZonePreEnterBoutWin : ZonePreEnterEvent
{
	public Chara target;

	public override void Execute()
	{
		if (target == null)
		{
			Debug.LogError("exception: target not found:" + target);
			return;
		}
		Thing thing = null;
		int num = 0;
		foreach (Thing thing2 in target.things)
		{
			if (thing2.isEquipped && !thing2.isGifted && (thing2.GetPrice() > num || thing2.rarity >= Rarity.Artifact))
			{
				thing = thing2;
				num = thing2.GetPrice();
			}
		}
		if (thing == null)
		{
			thing = ThingGen.Create("plat").SetNum(EClass.rndHalf(target.LV / 10 + 2));
		}
		else
		{
			target.RemoveCard(thing);
			target.EQ_CAT(thing.category.id);
		}
		target.SetInt(111, target.GetInt(111) + 1);
		SE.Play("questComplete");
		EClass._map.TrySmoothPick(EClass.pc.pos, thing, EClass.pc);
		EClass.player.ModFame(EClass.rndHalf(Mathf.Max(10, 10 + (target.LV - EClass.pc.LV) * 3)));
	}
}
