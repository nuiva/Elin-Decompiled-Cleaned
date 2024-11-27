using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class TileType : EClass
{
	public static void Init()
	{
		TileType.dict.Clear();
		foreach (FieldInfo fieldInfo in typeof(TileType).GetFields(BindingFlags.Static | BindingFlags.Public))
		{
			if (typeof(TileType).IsAssignableFrom(fieldInfo.FieldType))
			{
				TileType.dict[fieldInfo.Name] = (TileType)fieldInfo.GetValue(null);
			}
		}
	}

	public virtual string LangPlaceType
	{
		get
		{
			return "place_Obj";
		}
	}

	public virtual bool CanStack
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanRotate(bool buildMode)
	{
		return true;
	}

	public virtual bool ChangeBlockDir
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsSkipLowBlock
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsSkipFloor
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsUseBlockDir
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsFloorOrBridge
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsWall
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsFloor
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsBridge
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsWallOrFence
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsWallOrFullBlock
	{
		get
		{
			return false;
		}
	}

	public bool IsRamp
	{
		get
		{
			return this.Ramp > TileType.RampType.None;
		}
	}

	public virtual TileType.RampType Ramp
	{
		get
		{
			return TileType.RampType.None;
		}
	}

	public virtual bool IsLadder
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsBlockPass
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsOccupyCell
	{
		get
		{
			return true;
		}
	}

	public virtual bool IsBlockSight
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsOpenSight
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsBlockLiquid
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsWater
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsDeepWater
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsBlockMount
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsFullBlock
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsFence
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsFloodBlock
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsPlayFootSound
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanSpawnOnWater
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsWaterTop
	{
		get
		{
			return false;
		}
	}

	public virtual bool CastShadowSelf
	{
		get
		{
			return false;
		}
	}

	public virtual bool CastShadowBack
	{
		get
		{
			return false;
		}
	}

	public virtual bool CastAmbientShadow
	{
		get
		{
			return false;
		}
	}

	public virtual bool CastAmbientShadowBack
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanBuiltOnArea
	{
		get
		{
			return true;
		}
	}

	public virtual bool CanBuiltOnWater
	{
		get
		{
			return true;
		}
	}

	public virtual bool CanBuiltOnThing
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanBuiltOnBlock
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsDoor
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanBuiltOnFloor
	{
		get
		{
			return true;
		}
	}

	public virtual bool CanBuiltOnBridge
	{
		get
		{
			return true;
		}
	}

	public virtual bool CanInstaComplete
	{
		get
		{
			return false;
		}
	}

	public virtual int MinAltitude
	{
		get
		{
			return 1;
		}
	}

	public virtual int MaxAltitude
	{
		get
		{
			return 10;
		}
	}

	public virtual bool AltitudeAsDir
	{
		get
		{
			return false;
		}
	}

	public virtual bool UseLowWallTiles
	{
		get
		{
			return false;
		}
	}

	public virtual bool UseMountHeight
	{
		get
		{
			return false;
		}
	}

	public virtual bool UseHangZFix
	{
		get
		{
			return false;
		}
	}

	public virtual bool UseLowBlock
	{
		get
		{
			return false;
		}
	}

	public virtual bool RemoveOnFloorChange
	{
		get
		{
			return true;
		}
	}

	public virtual bool AllowObj
	{
		get
		{
			return false;
		}
	}

	public virtual bool AllowMultiInstall
	{
		get
		{
			return true;
		}
	}

	public virtual bool FreeStyle
	{
		get
		{
			return false;
		}
	}

	public virtual byte slopeHeight
	{
		get
		{
			return 0;
		}
	}

	public virtual float MountHeight
	{
		get
		{
			return 0f;
		}
	}

	public virtual float FloorHeight
	{
		get
		{
			return 0f;
		}
	}

	public virtual float RepeatSize
	{
		get
		{
			return 1f;
		}
	}

	public virtual int FloorAltitude
	{
		get
		{
			return 0;
		}
	}

	public virtual int LiquidLV
	{
		get
		{
			return 0;
		}
	}

	public virtual bool AllowLitter
	{
		get
		{
			return true;
		}
	}

	public virtual bool AllowBlood
	{
		get
		{
			return true;
		}
	}

	public virtual bool ShowPillar
	{
		get
		{
			return true;
		}
	}

	public virtual bool AlwaysShowShadow
	{
		get
		{
			return false;
		}
	}

	public virtual bool RepeatBlock
	{
		get
		{
			return false;
		}
	}

	public virtual bool ForceRpeatBlock
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanBeHeld
	{
		get
		{
			return true;
		}
	}

	public virtual bool EditorTile
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsFloodDoor
	{
		get
		{
			return false;
		}
	}

	public virtual bool Invisible
	{
		get
		{
			return false;
		}
	}

	public virtual bool IgnoreBuildRule
	{
		get
		{
			return false;
		}
	}

	public virtual bool RenderWaterBlock
	{
		get
		{
			return true;
		}
	}

	public virtual BaseTileSelector.SelectType SelectType
	{
		get
		{
			return BaseTileSelector.SelectType.Single;
		}
	}

	public virtual BaseTileSelector.BoxType BoxType
	{
		get
		{
			return BaseTileSelector.BoxType.Box;
		}
	}

	public virtual BaseTileSelector.HitType HitType
	{
		get
		{
			return BaseTileSelector.HitType.Block;
		}
	}

	public virtual BlockRenderMode blockRenderMode
	{
		get
		{
			return BlockRenderMode.Default;
		}
	}

	public bool IsMountBlock
	{
		get
		{
			return this.MountHeight > 0f;
		}
	}

	public HitResult _HitTest(Point pos, Card target, bool canIgnore = true)
	{
		if ((EClass.debug.ignoreBuildRule || this.IgnoreBuildRule) && canIgnore)
		{
			return HitResult.Valid;
		}
		if (!this.CanBuiltOnArea && pos.HasArea)
		{
			return HitResult.Invalid;
		}
		if (pos.cell.IsTopWater)
		{
			if (!this.CanBuiltOnWater)
			{
				return HitResult.Invalid;
			}
		}
		else if (!this.CanBuiltOnFloor)
		{
			return HitResult.Invalid;
		}
		if (!this.CanBuiltOnBridge && pos.cell._bridge != 0)
		{
			return HitResult.Invalid;
		}
		if (target != null)
		{
			if (target.sourceCard.multisize && (pos.Installed != null || pos.cell.blocked || (pos.HasChara && pos.FirstChara.IsHostile(EClass.pc))))
			{
				return HitResult.Invalid;
			}
			if (pos.Installed != null && pos.Installed != target)
			{
				TileType tileType = pos.Installed.TileType;
				TileType tileType2 = target.TileType;
				if (!tileType.AllowMultiInstall || !tileType2.AllowMultiInstall)
				{
					return HitResult.Invalid;
				}
			}
			if (!target.trait.CanBuiltAt(pos))
			{
				return HitResult.Invalid;
			}
		}
		else if (pos.Installed != null && !this.CanBuiltOnThing && !pos.cell.hasDoor)
		{
			return HitResult.Invalid;
		}
		if (pos.HasBlock)
		{
			if (!this.CanBuiltOnBlock && pos.sourceBlock.tileType.IsOccupyCell)
			{
				return HitResult.Invalid;
			}
			if (this.IsDoor && pos.HasWallOrFence && pos.cell.blockDir == 2)
			{
				return HitResult.Invalid;
			}
		}
		else if (this.IsDoor)
		{
			return HitResult.Invalid;
		}
		return this.HitTest(pos);
	}

	protected virtual HitResult HitTest(Point pos)
	{
		if (pos.HasObj)
		{
			return HitResult.Warning;
		}
		return HitResult.Valid;
	}

	public virtual int GetDesiredDir(Point p, int d)
	{
		return -1;
	}

	public virtual void GetMountHeight(ref Vector3 v, Point p, int d, Card target = null)
	{
		v += EClass.screen.tileMap.altitudeFix * (float)target.altitude;
	}

	public Vector3 GetRampFix(int dir)
	{
		int num = this.Ramp * TileType.RampType.Half - TileType.RampType.Half + ((dir <= 1) ? 0 : 1);
		Vector3 vector = EClass.setting.render.rampFix[num];
		return new Vector3(vector.x * (float)((dir % 2 == 0) ? 1 : -1), vector.y, vector.z);
	}

	public static TileTypeNone None = new TileTypeNone();

	public static TileTypeInvisibleBlock InvisiBlock = new TileTypeInvisibleBlock();

	public static TileTypeBlock Block = new TileTypeBlock();

	public static TileTypeBlockShip BlockShip = new TileTypeBlockShip();

	public static TileTypeSlope Slope = new TileTypeSlope();

	public static TileTypeHalfBlock HalfBlock = new TileTypeHalfBlock();

	public static TileTypeStairs Stairs = new TileTypeStairs();

	public static TileTypeStairs StairsHalf = new TileTypeStairsHalf();

	public static TileTypeRooftop Rooftop = new TileTypeRooftop();

	public static TileTypeScaffold Scaffold = new TileTypeScaffold();

	public static TileTypeLadder Ladder = new TileTypeLadder();

	public static TileTypePillar Pillar = new TileTypePillar();

	public static TileTypeWaterfall Waterfall = new TileTypeWaterfall();

	public static TileTypeWall Wall = new TileTypeWall();

	public static TileTypeWallOpen WallOpen = new TileTypeWallOpen();

	public static TileTypeFence Fence = new TileTypeFence();

	public static TileTypeFenceClosed FenceClosed = new TileTypeFenceClosed();

	public static TileTypeFloor Floor = new TileTypeFloor();

	public static TileTypeFloorScaffold FloorScaffold = new TileTypeFloorScaffold();

	public static TileTypeWater FloorWater = new TileTypeWater();

	public static TileTypeWaterShallow FloorWaterShallow = new TileTypeWaterShallow();

	public static TileTypeWaterDeep FloorWaterDeep = new TileTypeWaterDeep();

	public static TileTypeBridge Bridge = new TileTypeBridge();

	public static TileTypeBridgeDeco BridgeDeco = new TileTypeBridgeDeco();

	public static TileTypeBridgePillar BridgePillar = new TileTypeBridgePillar();

	public static TileTypeSky Sky = new TileTypeSky();

	public static TileTypeObj Obj = new TileTypeObj();

	public static TileTypeObjBig ObjBig = new TileTypeObjBig();

	public static TileTypeObjHuge ObjHuge = new TileTypeObjHuge();

	public static TileTypeObjCeil ObjCeil = new TileTypeObjCeil();

	public static TileTypeObjFloat ObjFloat = new TileTypeObjFloat();

	public static TileTypeObjWater ObjWater = new TileTypeObjWater();

	public static TileTypeObjWaterTop ObjWaterTop = new TileTypeObjWaterTop();

	public static TileTypeIllumination Illumination = new TileTypeIllumination();

	public static TileTypeSeed Seed = new TileTypeSeed();

	public static TileTypeTree Tree = new TileTypeTree();

	public static TileTypeDoor Door = new TileTypeDoor();

	public static TileTypeWallHang WallHang = new TileTypeWallHang();

	public static TileTypeWallHangSign WallHangSign = new TileTypeWallHangSign();

	public static TileTypeVine Vine = new TileTypeVine();

	public static TileTypeWallMount WallMount = new TileTypeWallMount();

	public static TileTypePaint Paint = new TileTypePaint();

	public static TileTypeWindow Window = new TileTypeWindow();

	public static TileTypeRoof Roof = new TileTypeRoof();

	public static TileTypeRoad Road = new TileTypeRoad();

	public static TileTypeChasm Chasm = new TileTypeChasm();

	public static TileTypeBoat Boat = new TileTypeBoat();

	public static TileTypeLiquid Liquid = new TileTypeLiquid();

	public static TileTypeMarker Marker = new TileTypeMarker();

	public static Dictionary<string, TileType> dict = new Dictionary<string, TileType>();

	public enum RampType
	{
		None,
		Full,
		Half
	}
}
