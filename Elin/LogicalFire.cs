public class LogicalFire : LogicalPoint
{
	public int fireAmount => base.cell.effect?.FireAmount ?? 0;

	public override LogicalPointManager manager => Point.map.effectManager;

	public override void Update()
	{
		manager.refreshList.Add(this);
		bool flag = false;
		XY[] surrounds = Point.Surrounds;
		for (int i = 0; i < surrounds.Length; i++)
		{
			XY xY = surrounds[i];
			Point.shared.Set(x + xY.x, z + xY.y);
			if (!Point.shared.IsValid)
			{
				continue;
			}
			Cell cell = Point.shared.cell;
			if (cell.fireAmount == fireAmount)
			{
				continue;
			}
			if (cell.fireAmount > fireAmount)
			{
				manager.GetOrCreate(Point.shared);
				continue;
			}
			if (fireAmount <= 0)
			{
				base.cell.effect = null;
				continue;
			}
			byte b = (byte)(fireAmount - cell.fireAmount);
			if (cell.fireAmount == 0)
			{
				if (fireAmount >= 2 && b >= 2)
				{
					if (EClass.rnd(2) == 0)
					{
						Transfer(Point.shared, 1);
					}
					ModFire(-1);
					flag = true;
				}
			}
			else if (b == 1)
			{
				if (life < 2)
				{
					ModFire(-1);
				}
				else if (EClass.rnd(3) == 0)
				{
					Transfer(Point.shared, 1);
					ModFire(1);
					flag = true;
				}
			}
			else
			{
				if (b > 6)
				{
					b = 6;
				}
				Transfer(Point.shared, (byte)(b - 1));
				flag = true;
			}
		}
		if (fireAmount == 0)
		{
			base.cell.effect = null;
		}
		else
		{
			EClass._map.Burn(x, z);
		}
		if (!flag)
		{
			life--;
		}
		if (life < 0)
		{
			Kill();
		}
	}

	public void Transfer(Point p, byte amount = 1)
	{
		ModFire(-amount);
		if (!p.cell.HasFire)
		{
			SE.Play("fire");
		}
		p.ModFire(amount);
	}
}
