public class TraitDoorman_Mage : TraitGuildDoorman
{
	public override bool IsGuildMember => EClass.player.IsMageGuildMember;

	public override void GiveTrial()
	{
		EClass.game.quests.Start("guild_mage", base.owner, assignQuest: false);
	}
}
