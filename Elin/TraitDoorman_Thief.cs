public class TraitDoorman_Thief : TraitGuildDoorman
{
	public override bool IsGuildMember => EClass.player.IsThiefGuildMember;

	public override void GiveTrial()
	{
		EClass.game.quests.Start("guild_thief", base.owner, assignQuest: false);
	}
}
