using System;

public class AM_ViewMap : AM_BaseTileSelect
{
	public override bool AllowBuildModeShortcuts
	{
		get
		{
			return false;
		}
	}

	public override bool ShowActionHint
	{
		get
		{
			return false;
		}
	}

	public override bool ShowMouseoverTarget
	{
		get
		{
			return false;
		}
	}

	public override bool ShowMaskedThings
	{
		get
		{
			return false;
		}
	}

	public override bool enableMouseInfo
	{
		get
		{
			return false;
		}
	}

	public override BaseTileSelector.SelectType selectType
	{
		get
		{
			return BaseTileSelector.SelectType.Single;
		}
	}

	public override bool IsBuildMode
	{
		get
		{
			return true;
		}
	}

	public override bool ShowBuildWidgets
	{
		get
		{
			return false;
		}
	}

	public override BuildMenu.Mode buildMenuMode
	{
		get
		{
			return BuildMenu.Mode.None;
		}
	}

	public override void OnActivate()
	{
		EClass.ui.layerFloat.SetActive(true);
		foreach (Layer layer in EClass.ui.layerFloat.layers)
		{
			if (!(layer is LayerTreasureMap))
			{
				layer.SetActive(false);
			}
		}
	}

	public override void OnDeactivate()
	{
		foreach (Layer c in EClass.ui.layerFloat.layers)
		{
			c.SetActive(true);
		}
	}

	public override void OnCancel()
	{
		base.Deactivate();
	}
}
