using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class Point : EClass
{
	public static Point GetShared(int x, int z)
	{
		Point.shared.x = x;
		Point.shared.z = z;
		return Point.shared;
	}

	public int index
	{
		get
		{
			return this.x + this.z * Point.map.Size;
		}
	}

	public Cell cell
	{
		get
		{
			return Point.map.cells[this.x, this.z];
		}
	}

	public SourceMaterial.Row matRoofBlock
	{
		get
		{
			return this.cell.matRoofBlock;
		}
	}

	public SourceMaterial.Row matBlock
	{
		get
		{
			return this.cell.matBlock;
		}
	}

	public SourceMaterial.Row matFloor
	{
		get
		{
			return this.cell.matFloor;
		}
	}

	public SourceMaterial.Row matBridge
	{
		get
		{
			return this.cell.matBridge;
		}
	}

	public SourceBlock.Row sourceRoofBlock
	{
		get
		{
			return this.cell.sourceRoofBlock;
		}
	}

	public SourceBlock.Row sourceBlock
	{
		get
		{
			return this.cell.sourceBlock;
		}
	}

	public SourceFloor.Row sourceFloor
	{
		get
		{
			return this.cell.sourceFloor;
		}
	}

	public SourceFloor.Row sourceBridge
	{
		get
		{
			return this.cell.sourceBridge;
		}
	}

	public SourceObj.Row sourceObj
	{
		get
		{
			return this.cell.sourceObj;
		}
	}

	public CellDetail detail
	{
		get
		{
			return this.cell.detail;
		}
	}

	public Area area
	{
		get
		{
			CellDetail detail = this.detail;
			if (detail == null)
			{
				return null;
			}
			return detail.area;
		}
	}

	public Point Front
	{
		get
		{
			return new Point(this.x + 1, this.z - 1);
		}
	}

	public GrowSystem growth
	{
		get
		{
			return this.cell.growth;
		}
	}

	public int eloX
	{
		get
		{
			return EClass.scene.elomap.minX + this.x;
		}
	}

	public int eloY
	{
		get
		{
			return EClass.scene.elomap.minY + this.z;
		}
	}

	public bool HasDesignation
	{
		get
		{
			CellDetail detail = this.cell.detail;
			return ((detail != null) ? detail.designation : null) != null;
		}
	}

	public bool HasDirt
	{
		get
		{
			return this.cell.decal > 0;
		}
	}

	public bool IsValid
	{
		get
		{
			return this.x >= 0 && this.z >= 0 && this.x < Point.map.Size && this.z < Point.map.Size;
		}
		set
		{
			this.x = (value ? 0 : -999);
		}
	}

	public bool IsInBounds
	{
		get
		{
			return this.x >= Point.map.bounds.x && this.z >= Point.map.bounds.z && this.x <= Point.map.bounds.maxX && this.z <= Point.map.bounds.maxZ;
		}
	}

	public bool IsInBoundsPlus
	{
		get
		{
			return this.x >= 0 && this.z >= 0 && this.x <= Point.map.Size - 1 && this.z <= Point.map.Size - 1;
		}
	}

	public bool Within(int _x, int _z, int _w, int _h)
	{
		return this.x >= _x && this.z >= _z && this.x < _x + _w && this.z < _z + _h;
	}

	public bool IsFarmField
	{
		get
		{
			return (this.HasBridge ? this.sourceBridge : this.sourceFloor).alias == "field";
		}
	}

	public bool IsWater
	{
		get
		{
			return this.cell.IsTopWater;
		}
	}

	public bool HasRamp
	{
		get
		{
			return this.cell.HasRamp;
		}
	}

	public bool HasRail
	{
		get
		{
			return this.sourceObj.tag.Contains("rail");
		}
	}

	public bool HasRampOrLadder
	{
		get
		{
			return this.cell.HasRampOrLadder;
		}
	}

	public bool HasObj
	{
		get
		{
			return this.cell.obj > 0;
		}
	}

	public bool HasDecal
	{
		get
		{
			return this.cell.decal > 0;
		}
	}

	public bool HasBlock
	{
		get
		{
			return this.cell._block > 0;
		}
	}

	public bool HasMinableBlock
	{
		get
		{
			return this.HasObj && this.sourceBlock.tileType.IsFullBlock;
		}
	}

	public bool HasWallOrFence
	{
		get
		{
			return this.cell.HasWallOrFence;
		}
	}

	public bool HasWall
	{
		get
		{
			return this.cell._block != 0 && this.cell.sourceBlock.tileType.IsWall;
		}
	}

	public bool HasFence
	{
		get
		{
			return this.cell._block != 0 && this.cell.sourceBlock.tileType.IsFence;
		}
	}

	public bool HasNonWallBlock
	{
		get
		{
			return this.HasBlock && !this.cell.sourceBlock.tileType.IsWallOrFence;
		}
	}

	public bool HasTaskBuild
	{
		get
		{
			return this.cell.detail != null && this.cell.detail.designation is TaskBuild;
		}
	}

	public bool HasBlockRecipe
	{
		get
		{
			CellDetail detail = this.cell.detail;
			TaskBuild taskBuild = ((detail != null) ? detail.designation : null) as TaskBuild;
			return taskBuild != null && taskBuild.isBlock;
		}
	}

	public bool HasFloor
	{
		get
		{
			return this.cell._floor > 0;
		}
	}

	public bool HasBridge
	{
		get
		{
			return this.cell._bridge > 0;
		}
	}

	public bool IsSky
	{
		get
		{
			return this.sourceFloor.tileType == TileType.Sky && !this.HasBridge;
		}
	}

	public bool HasArea
	{
		get
		{
			CellDetail detail = this.detail;
			return ((detail != null) ? detail.area : null) != null;
		}
	}

	public bool HasChara
	{
		get
		{
			CellDetail detail = this.detail;
			return detail != null && detail.charas.Count > 0;
		}
	}

	public bool HasThing
	{
		get
		{
			CellDetail detail = this.detail;
			return detail != null && detail.things.Count > 0;
		}
	}

	public bool HasMultipleChara
	{
		get
		{
			return this.detail != null && this.detail.charas.Count > 1;
		}
	}

	public Chara FirstChara
	{
		get
		{
			CellDetail detail = this.detail;
			if (detail == null || detail.charas.Count <= 0)
			{
				return null;
			}
			return this.detail.charas[0];
		}
	}

	public Chara FirstVisibleChara()
	{
		if (this.detail != null)
		{
			foreach (Chara chara in this.detail.charas)
			{
				if (EClass.pc.CanSee(chara))
				{
					return chara;
				}
			}
		}
		return null;
	}

	public Thing FirstThing
	{
		get
		{
			CellDetail detail = this.detail;
			if (detail == null || detail.things.Count <= 0)
			{
				return null;
			}
			return this.detail.things[0];
		}
	}

	public Thing LastThing
	{
		get
		{
			CellDetail detail = this.detail;
			if (detail == null || detail.things.Count <= 0)
			{
				return null;
			}
			return this.detail.things.LastItem<Thing>();
		}
	}

	public Thing Installed
	{
		get
		{
			if (this.detail == null || this.detail.things.Count <= 0 || !this.detail.things[0].IsInstalled)
			{
				return null;
			}
			return this.detail.things[0];
		}
	}

	public List<Thing> Things
	{
		get
		{
			return this.cell.Things;
		}
	}

	public List<Chara> Charas
	{
		get
		{
			return this.cell.Charas;
		}
	}

	public bool IsSeen
	{
		get
		{
			return this.cell.isSeen;
		}
	}

	public bool IsSync
	{
		get
		{
			return this.cell.pcSync;
		}
	}

	public bool IsHidden
	{
		get
		{
			return !this.cell.isSeen || (this.cell.room != null && !EClass.screen.tileMap.hideRoomFog && !this.cell.hasDoor && this.cell.HasRoof);
		}
	}

	public bool IsBlocked
	{
		get
		{
			return !this.IsValid || this.cell.blocked;
		}
	}

	public bool IsHotSpring
	{
		get
		{
			return this.cell.IsTopWaterAndNoSnow && (EClass._zone.elements.Has(3701) || this.IsInSpot<TraitGeyser>());
		}
	}

	public bool HasRoomOrArea(BaseArea a)
	{
		return this.area == a || this.cell.room == a;
	}

	public bool IsInSpot<T>() where T : TraitSpot
	{
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.trait is TraitSpot)
			{
				foreach (Point obj in thing.trait.ListPoints(null, true))
				{
					if (this.Equals(obj))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public bool IsPublicSpace()
	{
		if (this.area != null)
		{
			return this.area.type.IsPublicArea;
		}
		return this.cell.room == null || this.cell.room.type.IsPublicArea;
	}

	public Point()
	{
	}

	public Point(Point p)
	{
		this.x = p.x;
		this.z = p.z;
	}

	public Point(int _x, int _z)
	{
		this.x = _x;
		this.z = _z;
	}

	public Point(int _index)
	{
		this.Set(_index);
	}

	public Point Copy()
	{
		return new Point(this.x, this.z);
	}

	public T Copy<T>() where T : Point
	{
		T t = Activator.CreateInstance<T>();
		t.Set(this);
		return t;
	}

	public Point Set(int _x, int _z)
	{
		this.x = _x;
		this.z = _z;
		return this;
	}

	public Point Set(int index)
	{
		this.x = index % Point.map.Size;
		this.z = index % Point.map.SizeXZ / Point.map.Size;
		return this;
	}

	public Point Set(Point point)
	{
		this.x = point.x;
		this.z = point.z;
		return this;
	}

	public override string ToString()
	{
		return string.Concat(new string[]
		{
			"(",
			this.x.ToString(),
			" / ",
			this.z.ToString(),
			")"
		});
	}

	public void Set(Vector3 v)
	{
		v.x -= 0.64f;
		v.y -= 0.64f;
		int num = Mathf.RoundToInt(v.x / Point._screen.tileAlign.x);
		int num2 = Mathf.RoundToInt(v.y / Point._screen.tileAlign.y);
		this.x = (num2 - num) / 2 * -1;
		this.z = num - this.x;
	}

	public ref Vector3 PositionAuto()
	{
		if (EClass._zone.IsRegion)
		{
			return this.PositionTopdown();
		}
		return this.Position();
	}

	public ref Vector3 Position(int height)
	{
		if (height == -1)
		{
			return this.Position();
		}
		this.vCache.x = (float)(this.x + this.z) * Point._screen.tileAlign.x;
		this.vCache.y = (float)(this.z - this.x) * Point._screen.tileAlign.y + (float)height * Point._screen.tileMap._heightMod.y;
		this.vCache.z = 1000f + this.vCache.x * Point._screen.tileWeight.x + this.vCache.y * Point._screen.tileWeight.z + (float)height * Point._screen.tileMap._heightMod.z;
		return ref this.vCache;
	}

	public ref Vector3 Position()
	{
		byte b = (this.cell.bridgeHeight == 0) ? this.cell.height : this.cell.bridgeHeight;
		this.vCache.x = (float)(this.x + this.z) * Point._screen.tileAlign.x;
		this.vCache.y = (float)(this.z - this.x) * Point._screen.tileAlign.y + (float)b * Point._screen.tileMap._heightMod.y;
		this.vCache.z = 1000f + this.vCache.x * Point._screen.tileWeight.x + this.vCache.y * Point._screen.tileWeight.z + (float)b * Point._screen.tileMap._heightMod.z;
		return ref this.vCache;
	}

	public ref Vector3 PositionTopdown()
	{
		byte b = (this.cell.bridgeHeight == 0) ? this.cell.height : this.cell.bridgeHeight;
		this.vCache.x = (float)this.x * Point._screen.tileAlign.x;
		this.vCache.y = (float)this.z * Point._screen.tileAlign.y + (float)b * Point._screen.tileMap._heightMod.y;
		this.vCache.z = 1000f + this.vCache.x * Point._screen.tileWeight.x + this.vCache.y * Point._screen.tileWeight.z + (float)b * Point._screen.tileMap._heightMod.z;
		return ref this.vCache;
	}

	public ref Vector3 PositionTopdownTreasure()
	{
		GameScreenElona screenElona = EClass.scene.screenElona;
		this.vCache.x = (float)(this.x - EClass.scene.elomap.minX) * screenElona.tileAlign.x;
		this.vCache.y = (float)(this.z - EClass.scene.elomap.minY) * screenElona.tileAlign.y;
		this.vCache.z = 1400f;
		return ref this.vCache;
	}

	public ref Vector3 PositionCenter()
	{
		if (EClass._zone.IsRegion)
		{
			this.PositionTopdown();
		}
		else
		{
			this.Position();
		}
		this.vCache.x = this.vCache.x + EClass.screen.tileWorldSize.x * 0.5f;
		this.vCache.y = this.vCache.y + EClass.screen.tileWorldSize.y * 0.75f;
		return ref this.vCache;
	}

	public Quaternion GetRotation(Point to)
	{
		return Quaternion.Euler(0f, 0f, this.GetAngle(to));
	}

	public unsafe float GetAngle(Point to)
	{
		Vector3 vector = *to.Position() - *this.Position();
		return Mathf.Atan2(vector.y, vector.x) * 57.29578f;
	}

	public float GetAngle2(Point to)
	{
		int num = to.x - this.x;
		return Mathf.Atan2((float)(to.z - this.z), (float)num) * 57.29578f;
	}

	public Point GetNearestPoint(bool allowBlock = false, bool allowChara = true, bool allowInstalled = true, bool ignoreCenter = false)
	{
		Point.<>c__DisplayClass149_0 CS$<>8__locals1;
		CS$<>8__locals1.ignoreCenter = ignoreCenter;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.allowBlock = allowBlock;
		CS$<>8__locals1.allowChara = allowChara;
		CS$<>8__locals1.allowInstalled = allowInstalled;
		CS$<>8__locals1.p = new Point();
		int num = 1;
		int num2 = this.x;
		int num3 = this.z;
		if (this.<GetNearestPoint>g__IsValid|149_0(num2, num3, ref CS$<>8__locals1))
		{
			return CS$<>8__locals1.p;
		}
		if (this.<GetNearestPoint>g__IsValid|149_0(num2, num3 + 1, ref CS$<>8__locals1))
		{
			return CS$<>8__locals1.p;
		}
		for (int i = 0; i < 30; i++)
		{
			for (int j = 0; j < num; j++)
			{
				num2++;
				if (this.<GetNearestPoint>g__IsValid|149_0(num2, num3, ref CS$<>8__locals1))
				{
					return CS$<>8__locals1.p;
				}
			}
			num++;
			for (int k = 0; k < num; k++)
			{
				num3++;
				if (this.<GetNearestPoint>g__IsValid|149_0(num2, num3, ref CS$<>8__locals1))
				{
					return CS$<>8__locals1.p;
				}
			}
			for (int l = 0; l < num; l++)
			{
				num2--;
				if (this.<GetNearestPoint>g__IsValid|149_0(num2, num3, ref CS$<>8__locals1))
				{
					return CS$<>8__locals1.p;
				}
			}
			num++;
			for (int m = 0; m < num; m++)
			{
				num3--;
				if (this.<GetNearestPoint>g__IsValid|149_0(num2, num3, ref CS$<>8__locals1))
				{
					return CS$<>8__locals1.p;
				}
			}
		}
		CS$<>8__locals1.p.Set(this);
		return CS$<>8__locals1.p;
	}

	public bool ForeachNearestPoint(Func<Point, bool> endFunc, bool allowBlock = false, bool allowChara = true, bool allowInstalled = true, bool ignoreCenter = false)
	{
		Point.<>c__DisplayClass150_0 CS$<>8__locals1;
		CS$<>8__locals1.ignoreCenter = ignoreCenter;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.allowBlock = allowBlock;
		CS$<>8__locals1.allowChara = allowChara;
		CS$<>8__locals1.allowInstalled = allowInstalled;
		CS$<>8__locals1.p = new Point();
		int num = 1;
		int num2 = this.x;
		int num3 = this.z;
		if (this.<ForeachNearestPoint>g__IsValid|150_0(num2, num3, ref CS$<>8__locals1) && endFunc(CS$<>8__locals1.p))
		{
			return true;
		}
		if (this.<ForeachNearestPoint>g__IsValid|150_0(num2, num3 + 1, ref CS$<>8__locals1) && endFunc(CS$<>8__locals1.p))
		{
			return true;
		}
		for (int i = 0; i < 30; i++)
		{
			for (int j = 0; j < num; j++)
			{
				num2++;
				if (this.<ForeachNearestPoint>g__IsValid|150_0(num2, num3, ref CS$<>8__locals1) && endFunc(CS$<>8__locals1.p))
				{
					return true;
				}
			}
			num++;
			for (int k = 0; k < num; k++)
			{
				num3++;
				if (this.<ForeachNearestPoint>g__IsValid|150_0(num2, num3, ref CS$<>8__locals1) && endFunc(CS$<>8__locals1.p))
				{
					return true;
				}
			}
			for (int l = 0; l < num; l++)
			{
				num2--;
				if (this.<ForeachNearestPoint>g__IsValid|150_0(num2, num3, ref CS$<>8__locals1) && endFunc(CS$<>8__locals1.p))
				{
					return true;
				}
			}
			num++;
			for (int m = 0; m < num; m++)
			{
				num3--;
				if (this.<ForeachNearestPoint>g__IsValid|150_0(num2, num3, ref CS$<>8__locals1) && endFunc(CS$<>8__locals1.p))
				{
					return true;
				}
			}
		}
		Debug.Log("ForeachNearestPoint Fail:" + ((this != null) ? this.ToString() : null));
		return false;
	}

	public Point GetRandomNeighbor()
	{
		int num = (EClass.rnd(2) == 0) ? 1 : -1;
		if (EClass.rnd(2) == 0)
		{
			return new Point(Mathf.Clamp(this.x + num, 0, Point.map.Size - 1), this.z);
		}
		return new Point(this.x, Mathf.Clamp(this.z + num, 0, Point.map.Size - 1));
	}

	public Point GetRandomPoint(int radius, bool requireLos = true, bool allowChara = true, bool allowBlocked = false, int tries = 100)
	{
		Point point = new Point();
		for (int i = 0; i < tries; i++)
		{
			point.Set(this.x - radius + EClass.rnd(radius * 2 + 1), this.z - radius + EClass.rnd(radius * 2 + 1));
			if (point.IsValid && (allowBlocked || !point.IsBlocked) && (!requireLos || Los.IsVisible(this, point, null)) && (allowChara || !point.HasChara))
			{
				return point;
			}
		}
		return null;
	}

	public Point GetRandomPointInRadius(int minRadius, int maxRadius, bool requireLos = true, bool allowChara = true, bool allowBlocked = false, int tries = 2000)
	{
		Point point = new Point();
		for (int i = 0; i < tries; i++)
		{
			point.Set(this.x - maxRadius + EClass.rnd(maxRadius * 2 + 1), this.z - maxRadius + EClass.rnd(maxRadius * 2 + 1));
			if (point.IsValid && (allowBlocked || !point.IsBlocked) && (allowChara || !point.HasChara))
			{
				int num = point.Distance(this);
				if (num >= minRadius && num <= maxRadius && (!requireLos || Los.IsVisible(this, point, null)))
				{
					return point;
				}
			}
		}
		return null;
	}

	public Point GetPointTowards(Point dest)
	{
		Point point = new Point(this);
		int num = dest.x - point.x;
		int num2 = dest.z - point.z;
		if (Mathf.Abs(num) > 1 || Mathf.Abs(num2) > 1)
		{
			int num3 = Mathf.Max(Mathf.Abs(num), Mathf.Abs(num2));
			point.x += num / num3;
			point.z += num2 / num3;
		}
		return point;
	}

	public void TalkWitnesses(Chara criminal, string idTalk, int radius = 4, WitnessType type = WitnessType.everyone, Func<Chara, bool> talkIf = null, int chance = 3)
	{
		if (talkIf == null)
		{
			talkIf = ((Chara c) => EClass.rnd(chance) == 0);
		}
		foreach (Chara chara in this.ListWitnesses(criminal, radius, type, null))
		{
			if (talkIf(chara) && !chara.renderer.IsTalking())
			{
				chara.Talk(idTalk, null, null, false);
			}
		}
	}

	public List<Chara> ListWitnesses(Chara criminal, int radius = 4, WitnessType type = WitnessType.crime, Chara target = null)
	{
		List<Chara> list = new List<Chara>();
		foreach (Point point in Point.map.ListPointsInCircle(this, (float)radius, false, true))
		{
			CellDetail detail = point.detail;
			List<Chara> list2 = (detail != null) ? detail.charas : null;
			if (list2 != null && list2.Count != 0)
			{
				foreach (Chara chara in list2)
				{
					if (chara != criminal && !chara.IsPC && (chara == target || chara.CanWitness) && (!chara.HasCondition<ConDim>() || EClass.rnd(2) != 0) && chara.conSuspend == null && !chara.isParalyzed && !chara.IsDisabled)
					{
						switch (type)
						{
						case WitnessType.crime:
							if (criminal == null || chara.isBlind || chara.isConfused || (criminal.IsPCParty && (chara.IsPCFaction || chara.IsPCFactionMinion)))
							{
								continue;
							}
							if (target == null)
							{
								if (chara.OriginalHostility < Hostility.Neutral)
								{
									continue;
								}
							}
							else if (!target.IsFriendOrAbove(chara))
							{
								continue;
							}
							break;
						case WitnessType.music:
							if (chara.hostility <= Hostility.Enemy)
							{
								continue;
							}
							break;
						case WitnessType.ally:
							if (!criminal.IsPCFaction)
							{
								continue;
							}
							if (chara.hostility <= Hostility.Neutral)
							{
								continue;
							}
							break;
						}
						list.Add(chara);
					}
				}
			}
		}
		return list;
	}

	public bool TryWitnessCrime(Chara criminal, Chara target = null, int radius = 4, Func<Chara, bool> funcWitness = null)
	{
		List<Chara> list = this.ListWitnesses(criminal, radius, WitnessType.crime, target);
		bool result = false;
		if (funcWitness == null)
		{
			funcWitness = ((Chara c) => EClass.rnd(10) == 0);
		}
		foreach (Chara chara in list)
		{
			if (funcWitness(chara))
			{
				this.CallGuard(criminal, chara);
				result = true;
				if (target != null)
				{
					target.DoHostileAction(criminal, false);
				}
				if (chara != target)
				{
					chara.DoHostileAction(criminal, false);
					break;
				}
				break;
			}
		}
		return result;
	}

	public void CallGuard(Chara criminal, Chara caller)
	{
		caller.Talk("callGuards", null, null, false);
		List<Chara> list = (from c in EClass._map.charas
		where c.trait is TraitGuard && !c.IsInCombat
		select c).ToList<Chara>();
		if (list.Count > 0)
		{
			Chara chara = list.RandomItem<Chara>();
			caller.Say("calledGuard", caller, null, null);
			chara.DoHostileAction(criminal, false);
		}
	}

	public void SetBlock(int idMat = 0, int idBlock = 0)
	{
		Point.map.SetBlock(this.x, this.z, idMat, idBlock);
	}

	public void SetFloor(int idMat = 0, int idFloor = 0)
	{
		Point.map.SetFloor(this.x, this.z, idMat, idFloor);
	}

	public void SetObj(int id = 0, int value = 1, int dir = 0)
	{
		Point.map.SetObj(this.x, this.z, id, value, dir);
	}

	public void ModFire(int value)
	{
		Point.map.ModFire(this.x, this.z, value);
	}

	public void Plow()
	{
		if (this.IsFarmField)
		{
			if (this.cell.Right.HasWallOrFence)
			{
				this.Set(this.cell.Right.GetPoint());
			}
			else if (this.cell.Front.HasWallOrFence)
			{
				this.Set(this.cell.Front.GetPoint());
			}
		}
		this.SetFloor(4, 4);
	}

	public bool Equals(int _x, int _z)
	{
		return this.x == _x && this.z == _z;
	}

	public override bool Equals(object obj)
	{
		Point point = obj as Point;
		if (point == null)
		{
			Point point2 = point;
			string str = (point2 != null) ? point2.ToString() : null;
			string str2 = ":";
			Type type = point.GetType();
			Debug.Log(str + str2 + ((type != null) ? type.ToString() : null) + "is not Point");
			return false;
		}
		return this.x == point.x && this.z == point.z;
	}

	public override int GetHashCode()
	{
		return this.x + this.z * Point.map.Size;
	}

	public int Distance(Point p)
	{
		return Fov.Distance(p.x, p.z, this.x, this.z);
	}

	public int Distance(int tx, int tz)
	{
		return Fov.Distance(tx, tz, this.x, this.z);
	}

	public bool IsBlockByHeight(Point p)
	{
		return Mathf.Abs((int)(p.cell.minHeight - this.cell.topHeight)) > 8 && Mathf.Abs((int)(p.cell.topHeight - this.cell.minHeight)) > 8;
	}

	public Point Clamp(bool useBounds = false)
	{
		if (useBounds)
		{
			if (this.x < Point.map.bounds.x)
			{
				this.x = Point.map.bounds.x;
			}
			else if (this.x >= Point.map.bounds.maxX)
			{
				this.x = Point.map.bounds.maxX;
			}
			if (this.z < Point.map.bounds.z)
			{
				this.z = Point.map.bounds.z;
			}
			else if (this.z >= Point.map.bounds.maxZ)
			{
				this.z = Point.map.bounds.maxZ;
			}
		}
		else
		{
			if (this.x < 0)
			{
				this.x = 0;
			}
			else if (this.x >= Point.map.Size)
			{
				this.x = Point.map.Size - 1;
			}
			if (this.z < 0)
			{
				this.z = 0;
			}
			else if (this.z >= Point.map.Size)
			{
				this.z = Point.map.Size - 1;
			}
		}
		return this;
	}

	public List<Card> ListCards(bool includeMasked = false)
	{
		Point.listCard.Clear();
		bool flag = EClass.scene.actionMode != null && EClass.scene.actionMode.IsRoofEditMode(null);
		if (this.detail != null)
		{
			foreach (Thing thing in this.detail.things)
			{
				if (!thing.isHidden && (includeMasked || !thing.isMasked) && thing.isRoofItem == flag)
				{
					Point.listCard.Add(thing);
				}
			}
			if (!flag)
			{
				foreach (Chara item in this.detail.charas)
				{
					Point.listCard.Add(item);
				}
			}
		}
		return Point.listCard;
	}

	public List<Card> ListVisibleCards()
	{
		Point.listCard.Clear();
		if (this.detail != null)
		{
			foreach (Thing thing in this.detail.things)
			{
				if (!thing.isHidden && !thing.isMasked && !thing.isRoofItem)
				{
					Point.listCard.Add(thing);
				}
			}
			foreach (Chara chara in this.detail.charas)
			{
				if (EClass.pc.CanSee(chara))
				{
					Point.listCard.Add(chara);
				}
			}
		}
		return Point.listCard;
	}

	public Card FindAttackTarget()
	{
		foreach (Card card in this.ListCards(false))
		{
			if (card.isChara || card.trait.CanBeAttacked)
			{
				return card;
			}
		}
		return null;
	}

	public Chara FindChara(Func<Chara, bool> func)
	{
		if (this.detail != null)
		{
			foreach (Chara chara in this.detail.charas)
			{
				if (func(chara))
				{
					return chara;
				}
			}
		}
		return null;
	}

	public T FindThing<T>() where T : Trait
	{
		if (this.detail != null)
		{
			foreach (Thing thing in this.detail.things)
			{
				if (thing.trait is T)
				{
					return thing.trait as T;
				}
			}
		}
		return default(T);
	}

	public List<Card> ListThings<T>(bool onlyInstalled = true) where T : Trait
	{
		Point.listCard.Clear();
		if (this.detail != null)
		{
			foreach (Thing thing in this.detail.things)
			{
				if (!thing.isHidden && !thing.isRoofItem && !thing.isMasked && (!onlyInstalled || thing.IsInstalled) && thing.trait is T)
				{
					Point.listCard.Add(thing);
				}
			}
		}
		return Point.listCard;
	}

	public List<Chara> ListCharas()
	{
		Point.listChara.Clear();
		if (this.detail != null)
		{
			foreach (Chara item in this.detail.charas)
			{
				Point.listChara.Add(item);
			}
		}
		return Point.listChara;
	}

	public List<Chara> ListVisibleCharas()
	{
		Point.listChara.Clear();
		if (this.detail != null)
		{
			foreach (Chara chara in this.detail.charas)
			{
				if (EClass.pc.CanSee(chara))
				{
					Point.listChara.Add(chara);
				}
			}
		}
		return Point.listChara;
	}

	public List<Chara> ListCharasInNeighbor(Func<Chara, bool> func)
	{
		Point.listChara.Clear();
		this.ForeachNeighbor(delegate(Point p)
		{
			if (p.detail == null)
			{
				return;
			}
			foreach (Chara chara in p.detail.charas)
			{
				if (func(chara))
				{
					Point.listChara.Add(chara);
				}
			}
		}, true);
		return Point.listChara;
	}

	public List<Chara> ListCharasInRadius(Chara cc, int dist, Func<Chara, bool> func)
	{
		Point.listChara.Clear();
		foreach (Chara chara in EClass._map.charas)
		{
			if (func(chara) && chara.Dist(cc) < dist && Los.IsVisible(chara, cc))
			{
				Point.listChara.Add(chara);
			}
		}
		return Point.listChara;
	}

	public T GetInstalled<T>() where T : Trait
	{
		if (this.detail != null)
		{
			foreach (Thing thing in this.detail.things)
			{
				if (thing.IsInstalled && thing.trait is T)
				{
					return thing.trait as T;
				}
			}
		}
		return default(T);
	}

	public Effect PlayEffect(string id)
	{
		return Effect.Get(id).Play(this, 0f, null, null);
	}

	public unsafe SoundSource PlaySound(string id, bool synced = true, float v = 1f, bool spatial = true)
	{
		Vector3 vector = default(Vector3);
		if (spatial)
		{
			if (!this.IsValid)
			{
				return null;
			}
			vector = (EClass._zone.IsRegion ? (*this.PositionTopdown()) : (*this.Position()));
			vector.z = 0f;
			if (Vector3.Distance(vector, EClass.scene.transAudio.position) > EClass.core.gameSetting.audio.maxRange)
			{
				return null;
			}
			if (!synced)
			{
				v -= 0.4f;
			}
		}
		return EClass.Sound.Play(id, vector, (!spatial) ? 1f : (v * ((EClass.screen.Zoom >= 1f) ? 1f : EClass.screen.Zoom)));
	}

	public void RefreshNeighborTiles()
	{
		Point.map.RefreshNeighborTiles(this.x, this.z);
	}

	public void RefreshTile()
	{
		Point.map.RefreshSingleTile(this.x, this.z);
	}

	public bool HasNonHomeProperty(Thing exclude = null)
	{
		if (this.FirstThing == null)
		{
			return false;
		}
		foreach (Thing thing in this.detail.things)
		{
			if (thing != exclude && thing.isNPCProperty)
			{
				return true;
			}
		}
		return false;
	}

	public void Animate(AnimeID id, bool animeBlock = false)
	{
		if (!this.IsValid)
		{
			return;
		}
		CellDetail orCreateDetail = this.cell.GetOrCreateDetail();
		if (orCreateDetail.anime != null)
		{
			return;
		}
		TransAnime transAnime = new TransAnime
		{
			data = ResourceCache.Load<TransAnimeData>("Scene/Render/Anime/" + id.ToString()),
			point = this,
			animeBlock = animeBlock
		}.Init();
		orCreateDetail.anime = transAnime;
		EClass._map.pointAnimes.Add(transAnime);
		if (id == AnimeID.Quake)
		{
			transAnime.drawBlock = true;
		}
	}

	public RenderParam ApplyAnime(RenderParam p)
	{
		if (this.detail == null || this.detail.anime == null)
		{
			return p;
		}
		p.x += this.detail.anime.v.x;
		p.y += this.detail.anime.v.y;
		p.z += this.detail.anime.v.z;
		p.v.x = p.v.x + this.detail.anime.v.x;
		p.v.y = p.v.y + this.detail.anime.v.y;
		p.v.z = p.v.z + this.detail.anime.v.z;
		return p;
	}

	public Vector3 ApplyAnime(ref Vector3 p)
	{
		if (this.detail == null || this.detail.anime == null)
		{
			return p;
		}
		p.x += this.detail.anime.v.x;
		p.y += this.detail.anime.v.y;
		p.z += this.detail.anime.v.z;
		return p;
	}

	public List<IInspect> ListInspectorTargets()
	{
		List<IInspect> list = new List<IInspect>();
		CellDetail detail = this.detail;
		if (detail != null)
		{
			foreach (Chara chara in detail.charas)
			{
				if (chara.isSynced)
				{
					list.Add(chara);
				}
			}
			foreach (Thing item in detail.things)
			{
				list.Add(item);
			}
			if (detail.designation != null && !(detail.designation is TaskCut) && !(detail.designation is TaskMine))
			{
				list.Add(detail.designation);
			}
		}
		if (ObjInfo._CanInspect(this))
		{
			list.Add(ObjInfo.GetTempList(this));
		}
		if (BlockInfo._CanInspect(this))
		{
			list.Add(BlockInfo.GetTempList(this));
		}
		return list;
	}

	public void ForeachMultiSize(int w, int h, Action<Point, bool> action)
	{
		for (int i = 0; i < h; i++)
		{
			for (int j = 0; j < w; j++)
			{
				Point.shared3.Set(this.x - j, this.z + i);
				action(Point.shared3, j == 0 && i == 0);
			}
		}
	}

	public void ForeachNeighbor(Action<Point> action, bool diagonal = true)
	{
		Point point = new Point();
		for (int i = this.x - 1; i <= this.x + 1; i++)
		{
			for (int j = this.z - 1; j <= this.z + 1; j++)
			{
				if (diagonal || i == this.x || j == this.z)
				{
					point.Set(i, j);
					if (point.IsValid)
					{
						action(point);
					}
				}
			}
		}
	}

	public Point ToRegionPos()
	{
		Point point = new Point(this.x, this.z);
		point.x += EClass.scene.elomap.minX;
		point.z += EClass.scene.elomap.minY;
		return point;
	}

	[CompilerGenerated]
	private bool <GetNearestPoint>g__IsValid|149_0(int dx, int dz, ref Point.<>c__DisplayClass149_0 A_3)
	{
		A_3.p.Set(dx, dz);
		return A_3.p.IsInBounds && Point.map.Contains(dx, dz) && (!A_3.ignoreCenter || dx != this.x || dz != this.z) && (A_3.allowBlock || (!A_3.p.cell.blocked && !A_3.p.cell.hasDoor && (A_3.p.cell.growth == null || !A_3.p.cell.growth.IsTree))) && (A_3.allowChara || !A_3.p.HasChara) && (A_3.allowInstalled || A_3.p.Installed == null);
	}

	[CompilerGenerated]
	private bool <ForeachNearestPoint>g__IsValid|150_0(int dx, int dz, ref Point.<>c__DisplayClass150_0 A_3)
	{
		A_3.p.Set(dx, dz);
		return A_3.p.IsInBounds && Point.map.Contains(dx, dz) && (!A_3.ignoreCenter || dx != this.x || dz != this.z) && (A_3.allowBlock || (!A_3.p.cell.blocked && !A_3.p.cell.hasDoor && (A_3.p.cell.growth == null || !A_3.p.cell.growth.IsTree))) && (A_3.allowChara || !A_3.p.HasChara) && (A_3.allowInstalled || A_3.p.Installed == null);
	}

	public static readonly XY[] Surrounds = new XY[]
	{
		new XY(0, -1),
		new XY(1, 0),
		new XY(0, 1),
		new XY(-1, 0)
	};

	public static Vector3 fixedPos;

	public static Point shared = new Point();

	public static Point shared2 = new Point();

	public static Point shared3 = new Point();

	public static Map map;

	public static Point Invalid = new Point
	{
		IsValid = false
	};

	public static Point Zero = new Point();

	[JsonProperty]
	public int x;

	[JsonProperty]
	public int z;

	private Vector3 vCache;

	public static BaseGameScreen _screen;

	private static List<Card> listCard = new List<Card>();

	private static List<Chara> listChara = new List<Chara>();
}
