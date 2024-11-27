using System;

public class TraitDoorman_Mage : TraitGuildDoorman
{
	public override bool IsGuildMember
	{
		get
		{
			return EClass.player.IsMageGuildMember;
		}
	}

	public override void GiveTrial()
	{
		EClass.game.quests.Start("guild_mage", base.owner, false);
	}
}
