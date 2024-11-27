using System;

public class AM_EditArea : AM_BaseTileSelect
{
	public override BuildMenu.Mode buildMenuMode
	{
		get
		{
			return BuildMenu.Mode.Area;
		}
	}

	public override bool IsBuildMode
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
			return true;
		}
	}

	public override AreaHighlightMode AreaHihlight
	{
		get
		{
			return AreaHighlightMode.Edit;
		}
	}

	public override BaseTileSelector.SelectType selectType
	{
		get
		{
			return BaseTileSelector.SelectType.Single;
		}
	}

	public override void OnUpdateCursor()
	{
		base.SetCursorOnMap(CursorSystem.Select);
	}

	public override HitResult HitTest(Point point, Point start)
	{
		if (point.area != null)
		{
			return HitResult.Valid;
		}
		return base.HitTest(point, start);
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		if (point.area != null)
		{
			Area a = point.area;
			UIContextMenu uicontextMenu = EClass.ui.CreateContextMenuInteraction();
			uicontextMenu.AddButton("expandArea", delegate()
			{
				ActionMode.ExpandArea.Activate(a, false);
			}, true);
			uicontextMenu.AddButton("shrinkArea", delegate()
			{
				ActionMode.ExpandArea.Activate(a, true);
			}, true);
			uicontextMenu.AddButton("delete", delegate()
			{
				SE.Play("trash");
				EClass._map.rooms.RemoveArea(a);
			}, true);
			uicontextMenu.Show();
		}
	}
}
