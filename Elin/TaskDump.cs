using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public class TaskDump : Task
{
	public static void TryPerform()
	{
		if (!EClass.ui.AllowInventoryInteractions)
		{
			return;
		}
		if (!EClass._zone.IsPCFaction && !(EClass._zone is Zone_Tent))
		{
			Msg.Say("dump_invalid");
			return;
		}
		if (!EClass.pc.HasNoGoal)
		{
			SE.Beep();
			return;
		}
		EClass.pc.SetAIImmediate(new TaskDump());
	}

	public override bool CanManualCancel()
	{
		return true;
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		List<Thing> containers = new List<Thing>();
		EClass._map.things.ForEach(delegate(Thing t)
		{
			if (this.IsValidContainer(t))
			{
				containers.Add(t);
			}
		});
		while (containers.Count > 0)
		{
			bool flag = true;
			int num = TaskDump.<Run>g__SortVal|2_1(containers.First<Thing>());
			using (List<Thing>.Enumerator enumerator = containers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (TaskDump.<Run>g__SortVal|2_1(enumerator.Current) != num)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				containers.Sort((Thing a, Thing b) => EClass.pc.Dist(b) - EClass.pc.Dist(a));
			}
			else
			{
				containers.Sort((Thing a, Thing b) => TaskDump.<Run>g__SortVal|2_1(a) - TaskDump.<Run>g__SortVal|2_1(b));
			}
			Thing c = containers.LastItem<Thing>();
			containers.RemoveAt(containers.Count - 1);
			if (this.IsValidContainer(c))
			{
				EClass.ui.CloseLayers();
				Point pos = c.pos.Copy();
				if (c.IsMultisize)
				{
					int minDist = 999;
					c.ForeachPoint(delegate(Point p, bool isCenter)
					{
						if (!p.IsValid || !p.IsInBounds)
						{
							return;
						}
						PathProgress pathProgress = PathManager.Instance.RequestPathImmediate(EClass.pc.pos, p, EClass.pc, PathManager.MoveType.Default, -1, 1);
						if (pathProgress.HasPath && pathProgress.nodes.Count < minDist)
						{
							minDist = pathProgress.nodes.Count;
							pos = p.Copy();
						}
					});
				}
				yield return base.DoGoto(pos, 1, false, () => AIAct.Status.Running);
				if (this.IsValidContainer(c))
				{
					if (!c.ExistsOnMap)
					{
						Msg.Say("dump_noExist", c, null, null, null);
					}
					else if (EClass.pc.Dist(c) > 1)
					{
						Msg.Say("dump_tooFar", c, null, null, null);
					}
					else
					{
						List<Thing> list = this.ListThingsToPut(c);
						int num2 = 0;
						if (c.trait is TraitShippingChest)
						{
							c = EClass.game.cards.container_shipping;
						}
						foreach (Thing thing in list)
						{
							if (!c.things.IsFull(thing, true, true) && !thing.isDestroyed)
							{
								thing.PlaySoundDrop(false);
								if (c.parent is Card)
								{
									(c.parent as Card).SetDirtyWeight();
								}
								Msg.Say("dump_item", thing, c, null, null);
								c.AddCard(thing);
								num2++;
							}
						}
						if (num2 > 0)
						{
							Msg.Say("dump_dumped", num2.ToString() ?? "", c.Name, null, null);
						}
						c = null;
					}
				}
			}
		}
		yield break;
	}

	public override void OnCancelOrSuccess()
	{
		Msg.Say("dump_end");
		SE.Play("auto_dump");
	}

	public bool IsValidContainer(Thing c)
	{
		if ((!c.ExistsOnMap && !c.IsInstalled) || !c.CanSearchContents)
		{
			return false;
		}
		if (c.trait is TraitShippingChest)
		{
			c = EClass.game.cards.container_shipping;
		}
		return c.GetWindowSaveData() != null && c.GetWindowSaveData().autodump != AutodumpFlag.none && this.ListThingsToPut(c).Count != 0;
	}

	public List<Thing> ListThingsToPut(Thing c)
	{
		TaskDump.<>c__DisplayClass5_0 CS$<>8__locals1 = new TaskDump.<>c__DisplayClass5_0();
		CS$<>8__locals1.list = new List<Thing>();
		if (c.trait is TraitShippingChest)
		{
			c = EClass.game.cards.container_shipping;
		}
		CS$<>8__locals1.data = c.GetWindowSaveData();
		if (CS$<>8__locals1.data == null)
		{
			return CS$<>8__locals1.list;
		}
		switch (CS$<>8__locals1.data.autodump)
		{
		case AutodumpFlag.existing:
			using (List<Thing>.Enumerator enumerator = c.things.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Thing ct = enumerator.Current;
					EClass.pc.things.Foreach(delegate(Thing t)
					{
						if (CS$<>8__locals1.<ListThingsToPut>g__ExcludeDump|0(t))
						{
							return;
						}
						if (!t.CanStackTo(ct))
						{
							return;
						}
						CS$<>8__locals1.list.Add(t);
					}, true);
				}
				goto IL_17B;
			}
			break;
		case AutodumpFlag.sameCategory:
			break;
		case AutodumpFlag.none:
			goto IL_17B;
		case AutodumpFlag.distribution:
			EClass.pc.things.Foreach(delegate(Thing t)
			{
				if (base.<ListThingsToPut>g__ExcludeDump|0(t))
				{
					return;
				}
				if (CS$<>8__locals1.data.advDistribution)
				{
					using (HashSet<int>.Enumerator enumerator2 = CS$<>8__locals1.data.cats.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							int num = enumerator2.Current;
							if (t.category.uid == num)
							{
								CS$<>8__locals1.list.Add(t);
								break;
							}
						}
						return;
					}
				}
				ContainerFlag containerFlag = t.category.GetRoot().id.ToEnum(true);
				if (containerFlag == ContainerFlag.none)
				{
					containerFlag = ContainerFlag.other;
				}
				if (!CS$<>8__locals1.data.flag.HasFlag(containerFlag))
				{
					CS$<>8__locals1.list.Add(t);
				}
			}, true);
			goto IL_17B;
		default:
			goto IL_17B;
		}
		if (c.things.Count != 0)
		{
			HashSet<SourceCategory.Row> cats = new HashSet<SourceCategory.Row>();
			foreach (Thing thing in c.things)
			{
				cats.Add(thing.category);
			}
			EClass.pc.things.Foreach(delegate(Thing t)
			{
				if (CS$<>8__locals1.<ListThingsToPut>g__ExcludeDump|0(t))
				{
					return;
				}
				if (!cats.Contains(t.category))
				{
					return;
				}
				CS$<>8__locals1.list.Add(t);
			}, true);
		}
		IL_17B:
		return CS$<>8__locals1.list;
	}

	[CompilerGenerated]
	internal static int <Run>g__SortVal|2_1(Thing t)
	{
		Window.SaveData windowSaveData = t.GetWindowSaveData();
		if (windowSaveData != null)
		{
			return windowSaveData.priority;
		}
		return 0;
	}
}
