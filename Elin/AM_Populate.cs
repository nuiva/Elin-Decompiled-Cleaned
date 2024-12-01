using UnityEngine;

public class AM_Populate : AM_BaseTerrain
{
	public enum Mode
	{
		New,
		Override,
		Delete
	}

	public Mode mode;

	public override int SubMenuModeIndex => (int)mode;

	public override void OnClickSubMenu(int a)
	{
		mode = a.ToEnum<Mode>();
	}

	public override string OnSetSubMenuButton(int a, UIButton b)
	{
		if (a < 3)
		{
			return "populate" + a.ToEnum<Mode>();
		}
		return null;
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		if (timer < 0.1f)
		{
			return;
		}
		timer = 0f;
		_ = point.cell;
		EClass._map.ForeachSphere(point.x, point.z, brushRadius, delegate(Point p)
		{
			if (mode == Mode.Delete || (mode == Mode.New && firstClick))
			{
				p.SetObj();
			}
			if (mode != Mode.Delete && !p.HasBlock && !p.HasObj)
			{
				for (int i = 0; i < 200; i++)
				{
					if (EClass.rnd(Mathf.Clamp(brushRadius * brushRadius - 3, 1, 10)) == 0)
					{
						if (EInput.isShiftDown)
						{
							bool flag = false;
							for (int j = 0; j < 10000; j++)
							{
								p.cell.biome.Populate(p);
								if (p.growth != null && p.growth.IsTree)
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								p.SetObj();
							}
						}
						else
						{
							p.cell.biome.Populate(p);
						}
					}
					if (brushRadius > 1 || p.HasObj)
					{
						break;
					}
				}
			}
		});
	}
}
