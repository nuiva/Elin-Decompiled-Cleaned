using System;

public class GameBlueprint : ZoneBlueprint
{
	public override void OnCreate()
	{
		this.RerollChara();
	}

	public void RerollChara()
	{
		this.charas.Clear();
		this.charas.Add(EClass.player.chara);
	}
}
