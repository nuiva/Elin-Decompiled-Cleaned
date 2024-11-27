using System;
using System.Collections.Generic;

public class AI_Clean : AIAct
{
	public static Point GetCleanPoint(Chara c, int r = -1, int tries = 10)
	{
		Point point = new Point();
		if (r != -1)
		{
			for (int i = 0; i < tries; i++)
			{
				if (i == 0)
				{
					point.Set(c.pos);
				}
				else
				{
					point.Set(c._x - r + EClass.rnd(r * 2 + 1), c.pos.z - r + EClass.rnd(r * 2 + 1));
				}
				if (point.IsValid)
				{
					if (point.HasDirt || point.cell.HasLiquid)
					{
						return point;
					}
					foreach (Card card in point.ListCards(false))
					{
					}
				}
			}
		}
		return null;
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		for (;;)
		{
			if (this.pos == null)
			{
				this.pos = AI_Clean.GetCleanPoint(this.owner, 4, 10);
			}
			if (this.pos != null)
			{
				break;
			}
			yield return base.DoIdle(10);
		}
		yield return base.DoGoto(this.pos, 0, false, null);
		if (this.owner.pos.HasDirt || this.owner.pos.cell.HasLiquid)
		{
			EClass._map.SetDecal(this.pos.x, this.pos.z, 0, 1, true);
			EClass._map.SetLiquid(this.pos.x, this.pos.z, 0, 0);
			this.pos.PlayEffect("vanish");
			this.owner.Say("clean", this.owner, null, null);
			this.owner.PlaySound("clean_floor", 1f, true);
			yield return base.KeepRunning();
		}
		List<Card> list = this.owner.pos.ListCards(false);
		bool flag = false;
		foreach (Card card in list)
		{
			if (card.isThing)
			{
				bool isInstalled = card.IsInstalled;
			}
		}
		if (flag)
		{
			this.owner.Talk("clean", null, null, false);
		}
		yield return base.Success(null);
		yield break;
	}

	public Point pos;

	public int maxRadius = -1;
}
