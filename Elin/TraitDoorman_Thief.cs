using System;

public class TraitDoorman_Thief : TraitGuildDoorman
{
	public override bool IsGuildMember
	{
		get
		{
			return EClass.player.IsThiefGuildMember;
		}
	}

	public override void GiveTrial()
	{
		EClass.game.quests.Start("guild_thief", base.owner, false);
	}
}
