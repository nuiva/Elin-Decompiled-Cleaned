using System;

public class AM_CreateArea : AM_BaseTileSelect
{
	public override bool IsBuildMode
	{
		get
		{
			return true;
		}
	}

	public override string textHintTitle
	{
		get
		{
			return this.area.Name;
		}
	}

	public override void OnUpdateCursor()
	{
		base.SetCursorOnMap(CursorSystem.Build);
	}

	public override string idSound
	{
		get
		{
			return null;
		}
	}

	public override AreaHighlightMode AreaHihlight
	{
		get
		{
			return AreaHighlightMode.Edit;
		}
	}

	public void CreateNew(Area a)
	{
		this.area = Area.Create(a.type.id);
		this.area.data.name = null;
	}

	public void SetArea(Area a)
	{
		this.CreateNew(a);
	}

	public override HitResult HitTest(Point point, Point start)
	{
		HitResult hitResult = EClass._map.rooms.GetHitResult(point, start);
		if (hitResult != HitResult.Default)
		{
			return hitResult;
		}
		return base.HitTest(point, start);
	}

	public override void OnSelectStart(Point point)
	{
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		this.area.SetRandomName(-1);
		EClass._map.rooms.AddArea(this.area, point);
		EClass.Sound.Play("build_area");
	}

	public override void OnAfterProcessTiles(Point start, Point end)
	{
		this.CreateNew(this.area);
	}

	public override void OnDeactivate()
	{
		this.area = null;
		if (BuildMenu.Instance)
		{
			BuildMenu.Instance.Unselect();
		}
	}

	public override void OnCancel()
	{
		ActionMode.EditArea.Activate(true, false);
	}

	public Area area;
}
