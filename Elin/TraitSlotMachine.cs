public class TraitSlotMachine : TraitGamble
{
	public override string idMsg => "use_slot";

	public override bool CanUse(Chara c)
	{
		return owner.isOn;
	}

	public override bool OnUse(Chara c)
	{
		MiniGame.Activate(MiniGame.Type.Slot);
		return false;
	}
}
