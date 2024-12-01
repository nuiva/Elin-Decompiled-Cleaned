public class GameBlueprint : ZoneBlueprint
{
	public override void OnCreate()
	{
		RerollChara();
	}

	public void RerollChara()
	{
		charas.Clear();
		charas.Add(EClass.player.chara);
	}
}
