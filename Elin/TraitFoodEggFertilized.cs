using System;

public class TraitFoodEggFertilized : TraitFoodEgg
{
	public override int DecaySpeed
	{
		get
		{
			return 1;
		}
	}

	public static Chara Incubate(Thing egg, Point pos, Card incubator = null)
	{
		egg.SetSale(false);
		string str = "";
		CardRow refCard = egg.refCard;
		if (refCard != null)
		{
			str = refCard.id;
			if (refCard.id == "chara" || refCard.quality == 4)
			{
				str = "";
			}
		}
		if (egg.IsDecayed)
		{
			str = "zombie";
		}
		Chara chara = CharaGen.Create(str.IsEmpty("chicken"), -1);
		EClass._zone.AddCard(chara, pos.GetNearestPoint(false, false, true, false) ?? EClass.pc.pos);
		chara.SetLv(1);
		chara.SetMainElement(egg.c_idMainElement, 10, true);
		chara.SetFeat(1232, (incubator != null) ? 3 : 2, true);
		chara.things.DestroyAll(null);
		foreach (Element element in chara.elements.dict.Values)
		{
			if ((!(element.source.category != "attribute") || !(element.source.category != "skill")) && (!(element.source.category == "attribute") || element.source.tag.Contains("primary")) && element.ValueWithoutLink != 0)
			{
				element.vTempPotential = element.vTempPotential * 2 + 100;
				element.vPotential += 30;
			}
		}
		if (!egg.isNPCProperty)
		{
			FactionBranch factionBranch = EClass.Branch ?? EClass.pc.homeBranch;
			if (factionBranch != null)
			{
				factionBranch.AddMemeber(chara);
				factionBranch.ChangeMemberType(chara, EClass._zone.IsPCFaction ? FactionMemberType.Livestock : FactionMemberType.Default);
				if (!EClass._zone.IsPCFaction)
				{
					EClass.pc.party.AddMemeber(chara);
				}
			}
		}
		Msg.Say("incubate", chara, null, null, null);
		return chara;
	}

	public override bool CanStackTo(Thing to)
	{
		return to.c_idMainElement == this.owner.c_idMainElement && base.CanStackTo(to);
	}
}
