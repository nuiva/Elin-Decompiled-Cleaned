using System;

public class AM_ExpandArea : AM_BaseTileSelect
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

	public void Activate(Area a, bool _shrink = false)
	{
		this.area = a;
		this.shrink = _shrink;
		base.Activate(true, false);
	}

	public override HitResult HitTest(Point point, Point start)
	{
		if (this.shrink)
		{
			if (this.area.points.Count <= 1)
			{
				return HitResult.Invalid;
			}
			if (point.area == this.area)
			{
				return HitResult.Valid;
			}
			if (point.area != null)
			{
				return HitResult.Invalid;
			}
			return base.HitTest(point, start);
		}
		else
		{
			HitResult hitResult = EClass._map.rooms.GetHitResult(point, start);
			if (hitResult != HitResult.Default)
			{
				return hitResult;
			}
			return base.HitTest(point, start);
		}
	}

	public override void OnSelectStart(Point point)
	{
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		if (this.shrink)
		{
			this.area.RemovePoint(point);
			SE.Trash();
			return;
		}
		this.area.AddPoint(point.Copy(), false);
		EClass.Sound.Play("build_area");
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

	public bool shrink;
}
