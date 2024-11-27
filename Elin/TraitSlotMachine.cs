using System;

public class TraitSlotMachine : TraitGamble
{
	public override string idMsg
	{
		get
		{
			return "use_slot";
		}
	}

	public override bool OnUse(Chara c)
	{
		MiniGame.Activate(MiniGame.Type.Slot);
		return false;
	}
}
