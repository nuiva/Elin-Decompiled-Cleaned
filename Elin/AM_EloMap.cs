using System;

public class AM_EloMap : AM_BaseSim
{
	public override bool ShouldPauseGame
	{
		get
		{
			return true;
		}
	}

	public EloMapActor actor
	{
		get
		{
			return EClass.scene.elomapActor;
		}
	}

	public override void OnActivate()
	{
		ActionMode.DefaultMode = this;
	}
}
