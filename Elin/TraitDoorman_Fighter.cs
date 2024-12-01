public class TraitDoorman_Fighter : TraitGuildDoorman
{
	public override bool IsGuildMember => EClass.player.IsFighterGuildMember;

	public override void GiveTrial()
	{
		EClass.game.quests.Start("guild_fighter", base.owner, assignQuest: false);
	}

	public override void OnJoinGuild()
	{
		base.owner.orgPos = new Point(59, 98);
	}
}
