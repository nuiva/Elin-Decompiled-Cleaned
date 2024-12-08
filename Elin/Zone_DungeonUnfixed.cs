public class Zone_DungeonUnfixed : Zone_Dungeon
{
	public override bool WillAutoSave => false;

	public override float BigDaddyChance => 0f;

	public override float ShrineChance => 0f;

	public override int ExpireDays => 1;

	public override bool RegenerateOnEnter => true;
}
