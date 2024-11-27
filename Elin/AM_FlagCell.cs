using System;

public class AM_FlagCell : AM_BaseTileSelect
{
	public override bool IsBuildMode
	{
		get
		{
			return true;
		}
	}

	public override bool UseSubMenu
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

	public override int SubMenuModeIndex
	{
		get
		{
			return (int)this.mode;
		}
	}

	public override BaseTileSelector.SelectType selectType
	{
		get
		{
			if (this.mode != AM_FlagCell.Mode.flagWallPillar)
			{
				return BaseTileSelector.SelectType.Multiple;
			}
			return BaseTileSelector.SelectType.Single;
		}
	}

	public override HitResult HitTest(Point point, Point start)
	{
		switch (this.mode)
		{
		case AM_FlagCell.Mode.flagSnow:
			if (start != null && start.cell.isClearSnow != point.cell.isClearSnow)
			{
				return HitResult.Default;
			}
			break;
		case AM_FlagCell.Mode.flagFloat:
			if (start != null && start.cell.isForceFloat != point.cell.isForceFloat)
			{
				return HitResult.Default;
			}
			break;
		case AM_FlagCell.Mode.flagWallPillar:
			if (!point.cell.HasWall)
			{
				return HitResult.Default;
			}
			break;
		case AM_FlagCell.Mode.flagClear:
			if (start != null && start.cell.isClearArea != point.cell.isClearArea)
			{
				return HitResult.Default;
			}
			break;
		case AM_FlagCell.Mode.flagShadow:
			if (point.Things.Count == 0)
			{
				return HitResult.Default;
			}
			break;
		}
		return HitResult.Valid;
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		switch (this.mode)
		{
		case AM_FlagCell.Mode.flagSnow:
			point.cell.isClearSnow = !point.cell.isClearSnow;
			break;
		case AM_FlagCell.Mode.flagFloat:
			point.cell.isForceFloat = !point.cell.isForceFloat;
			break;
		case AM_FlagCell.Mode.flagWallPillar:
			point.cell.isToggleWallPillar = !point.cell.isToggleWallPillar;
			break;
		case AM_FlagCell.Mode.flagClear:
			point.cell.isClearArea = !point.cell.isClearArea;
			if (point.cell.isClearArea)
			{
				point.Things.ForeachReverse(delegate(Thing t)
				{
					t.Destroy();
				});
			}
			break;
		case AM_FlagCell.Mode.flagShadow:
			point.Things.ForeachReverse(delegate(Thing t)
			{
				t.noShadow = !t.noShadow;
			});
			break;
		}
		point.RefreshNeighborTiles();
	}

	public override void OnClickSubMenu(int a)
	{
		this.mode = a.ToEnum<AM_FlagCell.Mode>();
		base.tileSelector.start = null;
	}

	public override string OnSetSubMenuButton(int a, UIButton b)
	{
		if (a >= 3 && !EClass.debug.enable)
		{
			return null;
		}
		if (a < 5)
		{
			return a.ToEnum<AM_FlagCell.Mode>().ToString();
		}
		return null;
	}

	public AM_FlagCell.Mode mode;

	public enum Mode
	{
		flagSnow,
		flagFloat,
		flagWallPillar,
		flagClear,
		flagShadow,
		flagWall
	}
}
