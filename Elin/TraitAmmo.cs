using System.Linq;

public class TraitAmmo : TraitItem
{
	public virtual bool ConsumeOnMiss => true;

	public override int DefaultStock => 20 + EClass.rnd(200);

	public override int CraftNum => 40;

	public override string LangUse => "ActReload";

	public override bool OnUse(Chara c)
	{
		if (EClass.pc.HasCondition<ConReload>())
		{
			Msg.Say("isReloading");
			return false;
		}
		TraitToolRange traitToolRange = EClass.player.currentHotItem.Thing?.trait as TraitToolRange;
		if (traitToolRange == null)
		{
			foreach (BodySlot slot in EClass.pc.body.slots)
			{
				if (slot.thing != null && slot.thing.trait is TraitToolRange)
				{
					traitToolRange = slot.thing.trait as TraitToolRange;
					break;
				}
			}
		}
		if (traitToolRange == null || !traitToolRange.IsAmmo(owner as Thing))
		{
			if (traitToolRange == null)
			{
				Msg.Say("invalidAction");
			}
			else
			{
				Msg.Say("wrongAmmo", owner, traitToolRange.owner);
			}
			return false;
		}
		return ActRanged.TryReload(traitToolRange.owner.Thing, owner.Thing);
	}

	public override bool CanStackTo(Thing to)
	{
		if (owner.elements.dict.Count() != to.elements.dict.Count())
		{
			return false;
		}
		foreach (Element value in owner.elements.dict.Values)
		{
			Element element = to.elements.GetElement(value.id);
			if (element == null || value.vBase != element.vBase)
			{
				return false;
			}
		}
		return base.CanStackTo(to);
	}
}
