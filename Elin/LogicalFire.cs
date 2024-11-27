using System;

public class LogicalFire : LogicalPoint
{
	public int fireAmount
	{
		get
		{
			CellEffect effect = base.cell.effect;
			if (effect == null)
			{
				return 0;
			}
			return effect.FireAmount;
		}
	}

	public override LogicalPointManager manager
	{
		get
		{
			return Point.map.effectManager;
		}
	}

	public override void Update()
	{
		this.manager.refreshList.Add(this);
		bool flag = false;
		foreach (XY xy in Point.Surrounds)
		{
			Point.shared.Set(this.x + xy.x, this.z + xy.y);
			if (Point.shared.IsValid)
			{
				Cell cell = Point.shared.cell;
				if (cell.fireAmount != this.fireAmount)
				{
					if (cell.fireAmount > this.fireAmount)
					{
						this.manager.GetOrCreate(Point.shared);
					}
					else if (this.fireAmount <= 0)
					{
						base.cell.effect = null;
					}
					else
					{
						byte b = (byte)(this.fireAmount - cell.fireAmount);
						if (cell.fireAmount == 0)
						{
							if (this.fireAmount >= 2 && b >= 2)
							{
								if (EClass.rnd(2) == 0)
								{
									this.Transfer(Point.shared, 1);
								}
								base.ModFire(-1);
								flag = true;
							}
						}
						else if (b == 1)
						{
							if (this.life < 2)
							{
								base.ModFire(-1);
							}
							else if (EClass.rnd(3) == 0)
							{
								this.Transfer(Point.shared, 1);
								base.ModFire(1);
								flag = true;
							}
						}
						else
						{
							if (b > 6)
							{
								b = 6;
							}
							this.Transfer(Point.shared, b - 1);
							flag = true;
						}
					}
				}
			}
		}
		if (this.fireAmount == 0)
		{
			base.cell.effect = null;
		}
		else
		{
			EClass._map.Burn(this.x, this.z, false);
		}
		if (!flag)
		{
			this.life--;
		}
		if (this.life < 0)
		{
			this.Kill();
		}
	}

	public void Transfer(Point p, byte amount = 1)
	{
		base.ModFire((int)(-(int)amount));
		if (!p.cell.HasFire)
		{
			SE.Play("fire");
		}
		p.ModFire((int)amount);
	}
}
