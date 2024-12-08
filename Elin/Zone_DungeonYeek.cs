public class Zone_DungeonYeek : Zone_DungeonUnfixed
{
	public override bool LockExit => base.lv <= -4;
}
