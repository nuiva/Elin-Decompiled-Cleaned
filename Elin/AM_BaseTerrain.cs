using UnityEngine;

public class AM_BaseTerrain : AM_BaseTileSelect
{
	public int power = 1;

	public float timer;

	public bool firstClick;

	public Point lastPoint;

	public override BaseTileSelector.SelectType selectType => BaseTileSelector.SelectType.Single;

	public override bool UseSubMenu => true;

	public override bool UseSubMenuSlider => true;

	public override bool SubMenuAsGroup => true;

	public override bool ContinuousClick => true;

	public override bool IsBuildMode => true;

	public virtual bool FixedPointer => false;

	public override void SEExecuteSummary()
	{
	}

	public override MeshPass GetGuidePass(Point point)
	{
		return EClass.screen.guide.passGuideFloor;
	}

	public override HitResult HitTest(Point point, Point start)
	{
		if (lastPoint != null)
		{
			point = lastPoint;
		}
		if (point.IsValid)
		{
			return HitResult.Valid;
		}
		return base.HitTest(point, start);
	}

	public override void OnRenderTile(Point point, HitResult result, int dir)
	{
		if (lastPoint != null)
		{
			point = lastPoint;
		}
		timer += Core.delta;
		if (!Input.GetMouseButton(0))
		{
			firstClick = true;
		}
		if (point.IsValid)
		{
			EClass._map.ForeachSphere(point.x, point.z, brushRadius, delegate(Point p)
			{
				if (p.IsValid)
				{
					base.OnRenderTile(p, result, dir);
				}
			});
		}
		else
		{
			base.OnRenderTile(point, result, dir);
		}
	}

	public override void OnUpdateInput()
	{
		base.OnUpdateInput();
		if (FixedPointer && (!EInput.leftMouse.pressing || EInput.leftMouse.dragging))
		{
			lastPoint = null;
		}
	}

	public override void OnAfterProcessTiles(Point start, Point end)
	{
		firstClick = false;
	}
}
