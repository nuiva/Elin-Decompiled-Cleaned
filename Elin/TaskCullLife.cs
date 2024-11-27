using System;
using System.Collections.Generic;
using System.Linq;

public class TaskCullLife : Task
{
	public static bool CanCull(Card c)
	{
		return c.c_minionType == MinionType.Friend && !c.IsPCFaction;
	}

	public override bool CanPerform()
	{
		return Act.TP.FindChara((Chara c) => TaskCullLife.CanCull(c)) != null;
	}

	public override bool CanManualCancel()
	{
		return true;
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		for (;;)
		{
			TaskCullLife.<>c__DisplayClass4_0 CS$<>8__locals1 = new TaskCullLife.<>c__DisplayClass4_0();
			Chara target = TaskCullLife.GetTarget();
			if (this.dest != null)
			{
				target = this.dest.FindChara((Chara c) => TaskCullLife.CanCull(c));
				this.dest = null;
			}
			if (target == null)
			{
				yield return base.Success(null);
			}
			CS$<>8__locals1.fail = false;
			yield return base.DoGoto(target, delegate()
			{
				CS$<>8__locals1.fail = true;
				return AIAct.Status.Running;
			});
			if (!CS$<>8__locals1.fail && target.IsAliveInCurrentZone)
			{
				if (this.owner.Dist(target) > 1)
				{
					yield return this.Cancel();
				}
				this.owner.PlaySound("shear", 1f, true);
				Msg.Say("cull_life", EClass.pc, target, null, null);
				target.Die(null, null, AttackSource.None);
				Thing t = ThingGen.Create("ecopo", -1, -1).SetNum(EClass.rndHalf(target.LV / 10 + 6));
				EClass.pc.Pick(t, true, true);
				yield return base.KeepRunning();
				CS$<>8__locals1 = null;
				target = null;
			}
		}
		yield break;
	}

	public static Chara GetTarget()
	{
		List<Chara> list = new List<Chara>();
		foreach (Chara chara in EClass._map.charas)
		{
			if (TaskCullLife.CanCull(chara))
			{
				list.Add(chara);
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		list.Sort((Chara a, Chara b) => EClass.pc.Dist(a) - EClass.pc.Dist(b));
		return list.First<Chara>();
	}

	public Point dest;
}
