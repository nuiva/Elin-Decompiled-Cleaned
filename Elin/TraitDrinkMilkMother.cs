using System;
using System.Collections.Generic;
using UnityEngine;

public class TraitDrinkMilkMother : TraitDrinkMilk
{
	public override bool HoldAsDefaultInteraction
	{
		get
		{
			return true;
		}
	}

	public override Emo2 GetHeldEmo(Chara c)
	{
		if (c.Evalue(1232) > 0)
		{
			return Emo2.baby;
		}
		return Emo2.none;
	}

	public override void OnDrink(Chara c)
	{
		if (c.Evalue(1232) > 0)
		{
			c.SetFeat(1232, c.Evalue(1232) - 1, false);
			Chara chara = CharaGen.Create(this.owner.c_idRefCard, -1);
			chara.SetLv(Mathf.Clamp(5 + this.owner.encLV * 5, 1, 20 + EClass.pc.Evalue(237)));
			Debug.Log(chara.id + "/" + chara.LV.ToString());
			List<Element> list = chara.elements.ListBestAttributes();
			List<Element> list2 = chara.elements.ListBestSkills();
			int num = 100;
			int num2 = 0;
			foreach (Element element in list)
			{
				Element element2 = c.elements.GetElement(element.id);
				int num3 = element.ValueWithoutLink * (element2.Potential - element2.vTempPotential) / num / 2;
				if (num3 > 0)
				{
					num2 += num3;
					Debug.Log(string.Concat(new string[]
					{
						element.source.alias,
						"/",
						num3.ToString(),
						" org:",
						element.ValueWithoutLink.ToString()
					}));
					c.elements.ModBase(element.id, num3);
				}
				num += 50;
			}
			num = 100;
			foreach (Element element3 in list2)
			{
				Element element4 = c.elements.GetElement(element3.id);
				if (element4 != null && element4.ValueWithoutLink != 0)
				{
					int num4 = element3.ValueWithoutLink * (element4.Potential - element4.vTempPotential) / num / 2;
					if (num4 > 0)
					{
						Debug.Log(string.Concat(new string[]
						{
							element3.source.alias,
							"/",
							num4.ToString(),
							" org:",
							element3.ValueWithoutLink.ToString()
						}));
						c.elements.ModBase(element3.id, num4);
					}
					num += 50;
				}
			}
			c.feat += this.owner.encLV;
			if (c.Evalue(1232) == 0)
			{
				c.Say("grow_adult", c, null, null);
				c.PlaySound("mutation", 1f, true);
				c.PlayEffect("mutation", true, 0f, default(Vector3));
			}
			else
			{
				c.Say("grow_baby", c, null, null);
				c.PlaySound("ding_potential", 1f, true);
			}
			c.PlayEffect("buff", true, 0f, default(Vector3));
			EClass.pc.ModExp(237, 100);
			return;
		}
		base.OnDrink(c);
	}
}
