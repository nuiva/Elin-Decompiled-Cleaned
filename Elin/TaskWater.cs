using System;
using System.Collections.Generic;

public class TaskWater : Task
{
	public TraitToolWaterCan waterCan
	{
		get
		{
			Chara chara = this.owner ?? Act.CC;
			object obj;
			if (chara == null)
			{
				obj = null;
			}
			else
			{
				Card held = chara.held;
				obj = ((held != null) ? held.trait : null);
			}
			return obj as TraitToolWaterCan;
		}
	}

	public override string GetText(string str = "")
	{
		if (this.dest == null || !this.dest.cell.HasFire)
		{
			return base.GetText(str);
		}
		return "TaskWaterFire".lang();
	}

	public override bool CanPerform()
	{
		return TaskWater.ShouldWater(Act.TP) && this.IsWaterCanValid(false);
	}

	public override bool CanManualCancel()
	{
		return true;
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		this.dest = new Point(this.dest);
		List<Point> list = this.ListPoints();
		for (;;)
		{
			TaskWater.<>c__DisplayClass6_0 CS$<>8__locals1 = new TaskWater.<>c__DisplayClass6_0();
			if (list.Count == 0)
			{
				break;
			}
			list.Sort((Point a, Point b) => a.Distance(this.dest) - b.Distance(this.dest));
			Point p = list[0];
			this.dest.Set(p);
			list.RemoveAt(0);
			if (TaskWater.ShouldWater(p))
			{
				if (!this.IsWaterCanValid(true))
				{
					yield return this.Cancel();
				}
				CS$<>8__locals1.fail = false;
				yield return base.DoGoto(p, 1, false, delegate()
				{
					CS$<>8__locals1.fail = true;
					return AIAct.Status.Running;
				});
				if (!CS$<>8__locals1.fail)
				{
					if (!this.IsWaterCanValid(true))
					{
						yield return this.Cancel();
					}
					if (TaskWater.ShouldWater(p))
					{
						if (this.owner.Dist(this.dest) > 1)
						{
							yield return this.Cancel();
						}
						p.cell.isWatered = true;
						if (!p.cell.blocked && EClass.rnd(5) == 0)
						{
							EClass._map.SetLiquid(p.x, p.z, 1, 1);
						}
						if (p.cell.HasFire)
						{
							EClass._map.ModFire(p.x, p.z, -50);
						}
						this.owner.PlaySound("water_farm", 1f, true);
						this.owner.Say("water_farm", this.owner, p.cell.GetFloorName(), null);
						this.waterCan.owner.ModCharge(-1, false);
						this.owner.ModExp(286, 15);
						if (!this.IsWaterCanValid(true))
						{
							yield return this.Cancel();
						}
						yield return base.KeepRunning();
						CS$<>8__locals1 = null;
						p = null;
					}
				}
			}
		}
		yield break;
	}

	public static bool ShouldWater(Point p)
	{
		Cell cell = p.cell;
		if (cell.HasFire)
		{
			return true;
		}
		if (cell.isWatered || cell.IsTopWater || cell.IsSnowTile || !p.IsFarmField)
		{
			return false;
		}
		if (p.cell.detail != null)
		{
			using (List<Thing>.Enumerator enumerator = p.Things.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.trait is TraitSeed)
					{
						return true;
					}
				}
			}
		}
		return p.growth != null;
	}

	public bool IsWaterCanValid(bool msg = true)
	{
		bool flag = this.waterCan != null && this.waterCan.owner.c_charges > 0;
		if (!flag && msg)
		{
			Msg.Say("water_deplete");
		}
		return flag;
	}

	public List<Point> ListPoints()
	{
		List<Point> list = new List<Point>();
		EClass._map.bounds.ForeachPoint(delegate(Point p)
		{
			if (TaskWater.ShouldWater(p))
			{
				list.Add(p.Copy());
			}
		});
		return list;
	}

	public Point dest;
}
