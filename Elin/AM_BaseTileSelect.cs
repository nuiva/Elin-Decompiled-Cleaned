using System;

public class AM_BaseTileSelect : ActionMode
{
	public override CursorInfo DefaultCursor
	{
		get
		{
			return CursorSystem.Select;
		}
	}

	public override bool enableMouseInfo
	{
		get
		{
			return true;
		}
	}

	public override bool ShowMouseoverTarget
	{
		get
		{
			return false;
		}
	}

	public override bool CanSelectTile
	{
		get
		{
			return true;
		}
	}

	public override AreaHighlightMode AreaHihlight
	{
		get
		{
			return AreaHighlightMode.Build;
		}
	}

	public TaskManager.Designations Designations
	{
		get
		{
			return EClass._map.tasks.designations;
		}
	}

	public override string idSound
	{
		get
		{
			return "actionMode";
		}
	}

	public override bool AllowMiddleClickFunc
	{
		get
		{
			return true;
		}
	}

	public override bool ShowMaskedThings
	{
		get
		{
			return true;
		}
	}
}
