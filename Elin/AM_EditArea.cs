public class AM_EditArea : AM_BaseTileSelect
{
	public override BuildMenu.Mode buildMenuMode => BuildMenu.Mode.Area;

	public override bool IsBuildMode => true;

	public override bool ShowMouseoverTarget => true;

	public override AreaHighlightMode AreaHihlight => AreaHighlightMode.Edit;

	public override BaseTileSelector.SelectType selectType => BaseTileSelector.SelectType.Single;

	public override void OnUpdateCursor()
	{
		SetCursorOnMap(CursorSystem.Select);
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
			UIContextMenu uIContextMenu = EClass.ui.CreateContextMenuInteraction();
			uIContextMenu.AddButton("expandArea", delegate
			{
				ActionMode.ExpandArea.Activate(a);
			});
			uIContextMenu.AddButton("shrinkArea", delegate
			{
				ActionMode.ExpandArea.Activate(a, _shrink: true);
			});
			uIContextMenu.AddButton("delete", delegate
			{
				SE.Play("trash");
				EClass._map.rooms.RemoveArea(a);
			});
			uIContextMenu.Show();
		}
	}
}
