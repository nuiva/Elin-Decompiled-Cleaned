using System;
using UnityEngine;

public class AM_Populate : AM_BaseTerrain
{
	public override int SubMenuModeIndex
	{
		get
		{
			return (int)this.mode;
		}
	}

	public override void OnClickSubMenu(int a)
	{
		this.mode = a.ToEnum<AM_Populate.Mode>();
	}

	public override string OnSetSubMenuButton(int a, UIButton b)
	{
		if (a < 3)
		{
			return "populate" + a.ToEnum<AM_Populate.Mode>().ToString();
		}
		return null;
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		if (this.timer < 0.1f)
		{
			return;
		}
		this.timer = 0f;
		Cell cell = point.cell;
		EClass._map.ForeachSphere(point.x, point.z, (float)this.brushRadius, delegate(Point p)
		{
			if (this.mode == AM_Populate.Mode.Delete || (this.mode == AM_Populate.Mode.New && this.firstClick))
			{
				p.SetObj(0, 1, 0);
			}
			if (this.mode != AM_Populate.Mode.Delete && !p.HasBlock && !p.HasObj)
			{
				for (int i = 0; i < 200; i++)
				{
					if (EClass.rnd(Mathf.Clamp(this.brushRadius * this.brushRadius - 3, 1, 10)) == 0)
					{
						if (EInput.isShiftDown)
						{
							bool flag = false;
							for (int j = 0; j < 10000; j++)
							{
								p.cell.biome.Populate(p, false);
								if (p.growth != null && p.growth.IsTree)
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								p.SetObj(0, 1, 0);
							}
						}
						else
						{
							p.cell.biome.Populate(p, false);
						}
					}
					if (this.brushRadius > 1 || p.HasObj)
					{
						break;
					}
				}
			}
		});
	}

	public AM_Populate.Mode mode;

	public enum Mode
	{
		New,
		Override,
		Delete
	}
}
