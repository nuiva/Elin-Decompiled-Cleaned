using System;

public class AM_Bird : AM_MiniGame
{
	public override void OnActivate()
	{
		EClass.scene.transFocus = EClass.scene.flock._roamers.RandomItem<FlockChild>().transform;
		EClass.screen.SetTargetZoomIndex(1);
		EClass.screen.RefreshTilt();
		EClass.ui.layerFloat.SetActive(false);
	}

	public override void OnUpdateInput()
	{
		if (EInput.wheel != 0)
		{
			FlockChild component = EClass.scene.transFocus.GetComponent<FlockChild>();
			if (EInput.wheel > 0)
			{
				EClass.scene.transFocus = EClass.scene.flock._roamers.NextItem(component).transform;
			}
			else
			{
				EClass.scene.transFocus = EClass.scene.flock._roamers.PrevItem(component).transform;
			}
			EClass.screen.targetZoom = Rand.Range(0.25f, 0.5f);
		}
		if (EInput.rightMouse.down)
		{
			base.Deactivate();
		}
	}

	public override void OnDeactivate()
	{
		EClass.scene.transFocus = null;
		EClass.screen.RefreshTilt();
		EClass.screen.SetTargetZoomIndex(1);
		EClass.ui.layerFloat.SetActive(true);
	}
}
