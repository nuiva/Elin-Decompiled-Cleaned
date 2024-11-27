using System;

public class AM_NoMap : ActionMode
{
	public override bool ShowActionHint
	{
		get
		{
			return false;
		}
	}

	public override bool AllowHotbar
	{
		get
		{
			return false;
		}
	}

	public override bool AllowGeneralInput
	{
		get
		{
			return false;
		}
	}

	public override bool IsNoMap
	{
		get
		{
			return true;
		}
	}

	public override BaseGameScreen TargetGameScreen
	{
		get
		{
			return EClass.scene.screenNoMap;
		}
	}

	public override void OnActivate()
	{
		EClass.ui.layerFloat.SetActive(false);
	}

	public override void OnDeactivate()
	{
		EClass.ui.layerFloat.SetActive(true);
	}
}
