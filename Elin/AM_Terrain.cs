using System;
using UnityEngine;

public class AM_Terrain : AM_BaseTerrain
{
	public override int SubMenuModeIndex
	{
		get
		{
			return (int)this.mode;
		}
	}

	public override int TopHeight(Point p)
	{
		return (int)p.cell.height;
	}

	public override bool FixedPointer
	{
		get
		{
			return true;
		}
	}

	public override string OnSetSubMenuButton(int a, UIButton b)
	{
		if (a < 3)
		{
			return "terrain" + a.ToEnum<AM_Terrain.Mode>().ToString();
		}
		return null;
	}

	public override void OnClickSubMenu(int a)
	{
		this.mode = a.ToEnum<AM_Terrain.Mode>();
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		if (this.timer < 0.1f)
		{
			return;
		}
		if (this.lastPoint != null)
		{
			point = this.lastPoint;
		}
		else
		{
			this.lastPoint = new Point(point);
		}
		this.timer = 0f;
		Cell cell = point.cell;
		EClass._map.ForeachSphere(point.x, point.z, (float)this.brushRadius, delegate(Point p)
		{
			int num = p.Distance(point);
			if (this.mode == AM_Terrain.Mode.Flatten)
			{
				Cell cell;
				if (p.cell.IsFloorWater != cell.IsFloorWater)
				{
					return;
				}
				int num2 = (int)(p.cell.height - cell.height);
				if (!EInput.isShiftDown)
				{
					num2 = ((num2 >= 0) ? Mathf.Clamp(this.brushRadius - num, 1, num2) : Mathf.Clamp((this.brushRadius - num) * -1, num2, -1));
				}
				cell = p.cell;
				cell.height -= (byte)num2;
				if (p.cell._bridge != 0)
				{
					Cell cell2 = p.cell;
					cell2.bridgeHeight -= (byte)num2;
				}
			}
			else
			{
				int num3 = this.brushRadius - num;
				if (EInput.isShiftDown)
				{
					num3 = 1;
				}
				if (this.mode == AM_Terrain.Mode.Down)
				{
					num3 *= -1;
				}
				if ((int)p.cell.height + num3 < 0)
				{
					num3 = (int)(-(int)p.cell.height);
				}
				else if ((int)p.cell.height + num3 > EClass.setting.maxGenHeight)
				{
					num3 = EClass.setting.maxGenHeight - (int)p.cell.height;
				}
				Cell cell3 = p.cell;
				cell3.height += (byte)num3;
				if (p.cell._bridge != 0)
				{
					Cell cell4 = p.cell;
					cell4.bridgeHeight += (byte)num3;
				}
			}
			p.RefreshNeighborTiles();
			if (p.cell.room != null)
			{
				p.cell.room.SetDirty();
			}
		});
	}

	public AM_Terrain.Mode mode;

	public enum Mode
	{
		Flatten,
		Up,
		Down
	}
}
