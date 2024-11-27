using System;
using System.Collections.Generic;
using System.Linq;

public class TraitAmmo : TraitItem
{
	public virtual bool ConsumeOnMiss
	{
		get
		{
			return true;
		}
	}

	public override int DefaultStock
	{
		get
		{
			return 20 + EClass.rnd(200);
		}
	}

	public override int CraftNum
	{
		get
		{
			return 40;
		}
	}

	public override string LangUse
	{
		get
		{
			return "ActReload";
		}
	}

	public override bool OnUse(Chara c)
	{
		if (EClass.pc.HasCondition<ConReload>())
		{
			Msg.Say("isReloading");
			return false;
		}
		Thing thing = EClass.player.currentHotItem.Thing;
		TraitToolRange traitToolRange = ((thing != null) ? thing.trait : null) as TraitToolRange;
		if (traitToolRange == null)
		{
			foreach (BodySlot bodySlot in EClass.pc.body.slots)
			{
				if (bodySlot.thing != null && bodySlot.thing.trait is TraitToolRange)
				{
					traitToolRange = (bodySlot.thing.trait as TraitToolRange);
					break;
				}
			}
		}
		if (traitToolRange == null || !traitToolRange.IsAmmo(this.owner as Thing))
		{
			if (traitToolRange == null)
			{
				Msg.Say("invalidAction");
			}
			else
			{
				Msg.Say("wrongAmmo", this.owner, traitToolRange.owner, null, null);
			}
			return false;
		}
		return ActRanged.TryReload(traitToolRange.owner.Thing, this.owner.Thing);
	}

	public override bool CanStackTo(Thing to)
	{
		if (this.owner.elements.dict.Count<KeyValuePair<int, Element>>() != to.elements.dict.Count<KeyValuePair<int, Element>>())
		{
			return false;
		}
		foreach (Element element in this.owner.elements.dict.Values)
		{
			Element element2 = to.elements.GetElement(element.id);
			if (element2 == null || element.vBase != element2.vBase)
			{
				return false;
			}
		}
		return base.CanStackTo(to);
	}
}
