using System;

public class TraitQuru : TraitUniqueChara
{
	public override bool CanJoinParty
	{
		get
		{
			return EClass.game.quests.IsCompleted("vernis_gold") || EClass.debug.enable;
		}
	}

	public override bool CanBeBanished
	{
		get
		{
			return false;
		}
	}
}
