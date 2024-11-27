using System;

public class TraitDoorman_Fighter : TraitGuildDoorman
{
	public override bool IsGuildMember
	{
		get
		{
			return EClass.player.IsFighterGuildMember;
		}
	}

	public override void GiveTrial()
	{
		EClass.game.quests.Start("guild_fighter", base.owner, false);
	}

	public override void OnJoinGuild()
	{
		base.owner.orgPos = new Point(59, 98);
	}
}
