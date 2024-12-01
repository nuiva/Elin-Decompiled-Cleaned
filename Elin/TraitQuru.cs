public class TraitQuru : TraitUniqueChara
{
	public override bool CanJoinParty
	{
		get
		{
			if (!EClass.game.quests.IsCompleted("vernis_gold"))
			{
				return EClass.debug.enable;
			}
			return true;
		}
	}

	public override bool CanBeBanished => false;
}
