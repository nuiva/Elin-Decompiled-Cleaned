using System;

public class AM_NewZone : ActionMode
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

	public override BaseTileSelector.HitType hitType
	{
		get
		{
			return BaseTileSelector.HitType.None;
		}
	}

	public override void OnActivate()
	{
		ActionMode.DefaultMode = this;
		EClass.ui.AddLayer<LayerNewZone>();
	}
}
