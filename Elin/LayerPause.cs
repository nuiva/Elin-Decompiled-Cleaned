using UnityEngine;

public class LayerPause : ELayer
{
	public override void OnUpdateInput()
	{
		if (Input.anyKeyDown)
		{
			Close();
		}
	}

	public override void OnKill()
	{
		Game.isPaused = false;
		SE.ClickOk();
	}
}
