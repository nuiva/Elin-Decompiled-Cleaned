using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Algorithms;
using UnityEngine;

public class Zone_Dungeon : Zone
{
	public override string IDGenerator
	{
		get
		{
			return "Dungeon";
		}
	}

	public override bool DisableRooms
	{
		get
		{
			return !base.IsPCFaction;
		}
	}

	public override bool UseFog
	{
		get
		{
			return true;
		}
	}

	public override bool ShowDangerLv
	{
		get
		{
			return true;
		}
	}

	public override float PrespawnRate
	{
		get
		{
			if (!this.idExport.IsEmpty())
			{
				return 0f;
			}
			return 1f;
		}
	}

	public override bool IsReturnLocation
	{
		get
		{
			return base.GetDeepestLv() == base.lv;
		}
	}

	public override int StartLV
	{
		get
		{
			return -1;
		}
	}

	public override bool BlockBorderExit
	{
		get
		{
			return true;
		}
	}

	public override float EvolvedChance
	{
		get
		{
			if (!EClass.debug.test)
			{
				return 0.08f;
			}
			return 1f;
		}
	}

	public override float BigDaddyChance
	{
		get
		{
			if (!EClass.debug.test)
			{
				return 0.08f;
			}
			return 1f;
		}
	}

	public override float ShrineChance
	{
		get
		{
			if (!EClass.debug.test)
			{
				return 0.35f;
			}
			return 1f;
		}
	}

	public override bool IsSnowCovered
	{
		get
		{
			return false;
		}
	}

	public override bool GrowPlant
	{
		get
		{
			return true;
		}
	}

	public override bool GrowWeed
	{
		get
		{
			return false;
		}
	}

	public override bool CountDeepestLevel
	{
		get
		{
			return true;
		}
	}

	public override ZoneTransition.EnterState RegionEnterState
	{
		get
		{
			return ZoneTransition.EnterState.Down;
		}
	}

	public override string GetDungenID()
	{
		if (EClass.rnd(2) == 0)
		{
			return "DungeonRuins";
		}
		if (EClass.rnd(4) == 0)
		{
			return "Cavern";
		}
		if (EClass.rnd(8) == 0)
		{
			return "RoundRooms";
		}
		return "Dungeon";
	}

	public override void OnGenerateMap()
	{
		if (!this.idExport.IsEmpty())
		{
			return;
		}
		base.TryGenerateOre();
		base.TryGenerateBigDaddy();
		base.TryGenerateEvolved(false, null);
		base.TryGenerateShrine();
	}

	public void PlaceRail(Zone_Dungeon.RailType railType = Zone_Dungeon.RailType.Mine)
	{
		Zone_Dungeon.<>c__DisplayClass35_0 CS$<>8__locals1;
		CS$<>8__locals1.idRail = 31;
		CS$<>8__locals1.idTrolley = "390";
		CS$<>8__locals1.placeStopper = true;
		int num = 8;
		if (railType == Zone_Dungeon.RailType.Factoy)
		{
			CS$<>8__locals1.idRail = 110;
			CS$<>8__locals1.placeStopper = false;
			num = EClass.rnd(4) + 1;
		}
		PathManager.Instance._pathfinder.PunishChangeDirection = true;
		PathManager.Instance._pathfinder.Diagonals = false;
		TraitStairsDown traitStairsDown = EClass._map.FindThing<TraitStairsDown>();
		TraitStairsUp traitStairsUp = EClass._map.FindThing<TraitStairsUp>();
		if (traitStairsDown != null && traitStairsUp != null)
		{
			Zone_Dungeon.<PlaceRail>g__TryPlaceRail|35_0(traitStairsDown.owner.pos, traitStairsUp.owner.pos, ref CS$<>8__locals1);
		}
		int num2 = 0;
		for (int i = 0; i < 200; i++)
		{
			if (Zone_Dungeon.<PlaceRail>g__TryPlaceRail|35_0(EClass._map.bounds.GetRandomSurface(false, true, false), EClass._map.bounds.GetRandomSurface(false, true, false), ref CS$<>8__locals1))
			{
				num2++;
				if (num2 > num)
				{
					break;
				}
			}
		}
		PathManager.Instance._pathfinder.Diagonals = true;
	}

	[CompilerGenerated]
	internal static bool <PlaceRail>g__TryPlaceRail|35_0(Point p1, Point p2, ref Zone_Dungeon.<>c__DisplayClass35_0 A_2)
	{
		if (p1.Distance(p2) < 20)
		{
			return false;
		}
		PathProgress pathProgress = PathManager.Instance.RequestPathImmediate(p1, p2, EClass.pc, PathManager.MoveType.Default, -1, 0);
		if (!pathProgress.HasPath)
		{
			return false;
		}
		Point point = new Point();
		Point point2 = new Point();
		bool flag = false;
		int num = 0;
		foreach (PathFinderNode pathFinderNode in pathProgress.nodes)
		{
			Point shared = Point.GetShared(pathFinderNode.X, pathFinderNode.Z);
			for (int i = -1; i < 2; i++)
			{
				int j = -1;
				while (j < 2)
				{
					point2.Set(shared.x + j, shared.z + i);
					if ((Mathf.Abs(j) != 1 || Mathf.Abs(i) != 1) && point2.IsValid && point2.sourceObj.id == 31 && !point2.Equals(point))
					{
						flag = true;
						if (num == 0)
						{
							return false;
						}
						break;
					}
					else
					{
						j++;
					}
				}
			}
			if (shared.sourceObj.id == A_2.idRail)
			{
				break;
			}
			shared.SetBlock(0, 0);
			shared.SetObj(A_2.idRail, 1, 0);
			shared.Things.ForeachReverse(delegate(Thing t)
			{
				if (!t.trait.CanBeDestroyed)
				{
					return;
				}
				t.Destroy();
			});
			if (flag)
			{
				break;
			}
			num++;
			point.Set(shared);
		}
		if (!flag & A_2.placeStopper)
		{
			Point shared2 = Point.GetShared(pathProgress.nodes.LastItem<PathFinderNode>().X, pathProgress.nodes.LastItem<PathFinderNode>().Z);
			Cell cell = shared2.cell;
			int dir = (cell.Front.obj == 31) ? 0 : ((cell.Right.obj == 31) ? 1 : ((cell.Back.obj == 31) ? 2 : 3));
			if (!shared2.HasThing)
			{
				EClass._zone.AddCard(ThingGen.Create("1208", -1, -1), shared2).Install().dir = dir;
			}
		}
		if (num > 2)
		{
			Point shared3 = Point.GetShared(pathProgress.nodes.First<PathFinderNode>().X, pathProgress.nodes.First<PathFinderNode>().Z);
			if (!shared3.HasThing)
			{
				EClass._zone.AddCard(ThingGen.Create(A_2.idTrolley, -1, -1), shared3).Install();
			}
		}
		return true;
	}

	public enum RailType
	{
		Mine,
		Factoy
	}
}
