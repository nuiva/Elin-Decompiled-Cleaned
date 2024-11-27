using System;
using UnityEngine;

public class AM_BaseTerrain : AM_BaseTileSelect
{
	public override BaseTileSelector.SelectType selectType
	{
		get
		{
			return BaseTileSelector.SelectType.Single;
		}
	}

	public override bool UseSubMenu
	{
		get
		{
			return true;
		}
	}

	public override bool UseSubMenuSlider
	{
		get
		{
			return true;
		}
	}

	public override bool SubMenuAsGroup
	{
		get
		{
			return true;
		}
	}

	public override bool ContinuousClick
	{
		get
		{
			return true;
		}
	}

	public override bool IsBuildMode
	{
		get
		{
			return true;
		}
	}

	public virtual bool FixedPointer
	{
		get
		{
			return false;
		}
	}

	public override void SEExecuteSummary()
	{
	}

	public override MeshPass GetGuidePass(Point point)
	{
		return EClass.screen.guide.passGuideFloor;
	}

	public override HitResult HitTest(Point point, Point start)
	{
		if (this.lastPoint != null)
		{
			point = this.lastPoint;
		}
		if (point.IsValid)
		{
			return HitResult.Valid;
		}
		return base.HitTest(point, start);
	}

	public override void OnRenderTile(Point point, HitResult result, int dir)
	{
		if (this.lastPoint != null)
		{
			point = this.lastPoint;
		}
		this.timer += Core.delta;
		if (!Input.GetMouseButton(0))
		{
			this.firstClick = true;
		}
		if (point.IsValid)
		{
			EClass._map.ForeachSphere(point.x, point.z, (float)this.brushRadius, delegate(Point p)
			{
				if (p.IsValid)
				{
					this.<>n__0(p, result, dir);
				}
			});
			return;
		}
		base.OnRenderTile(point, result, dir);
	}

	public override void OnUpdateInput()
	{
		base.OnUpdateInput();
		if (this.FixedPointer && (!EInput.leftMouse.pressing || EInput.leftMouse.dragging))
		{
			this.lastPoint = null;
		}
	}

	public override void OnAfterProcessTiles(Point start, Point end)
	{
		this.firstClick = false;
	}

	public int power = 1;

	public float timer;

	public bool firstClick;

	public Point lastPoint;
}
