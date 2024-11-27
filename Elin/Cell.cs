using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Algorithms;
using FloodSpill;
using UnityEngine;

public class Cell : WeightCell, IFloodCell
{
	public override bool Equals(object obj)
	{
		return false;
	}

	public override string ToString()
	{
		string[] array = new string[11];
		array[0] = "Cell(";
		array[1] = this.x.ToString();
		array[2] = ",";
		array[3] = this.z.ToString();
		array[4] = " ";
		int num = 5;
		SourceMaterial.Row matBlock = this.matBlock;
		array[num] = ((matBlock != null) ? matBlock.ToString() : null);
		array[6] = "/";
		int num2 = 7;
		SourceMaterial.Row matFloor = this.matFloor;
		array[num2] = ((matFloor != null) ? matFloor.ToString() : null);
		array[8] = "/";
		array[9] = this._block.ToString();
		array[10] = ")";
		return string.Concat(array);
	}

	public override int GetHashCode()
	{
		return this.index;
	}

	public int index
	{
		get
		{
			return (int)this.x + (int)this.z * Cell.Size;
		}
	}

	public byte TopHeight
	{
		get
		{
			if (this.bridgeHeight != 0)
			{
				return this.bridgeHeight;
			}
			return this.height;
		}
	}

	public Point GetPoint()
	{
		return new Point((int)this.x, (int)this.z);
	}

	public Point GetSharedPoint()
	{
		return Point.shared.Set((int)this.x, (int)this.z);
	}

	public Cell Front
	{
		get
		{
			if (this.z <= 0)
			{
				return Cell.Void;
			}
			return Cell.cells[(int)this.x, (int)(this.z - 1)];
		}
	}

	public Cell Right
	{
		get
		{
			if ((int)(this.x + 1) >= Cell.Size)
			{
				return Cell.Void;
			}
			return Cell.cells[(int)(this.x + 1), (int)this.z];
		}
	}

	public Cell Back
	{
		get
		{
			if ((int)(this.z + 1) >= Cell.Size)
			{
				return Cell.Void;
			}
			return Cell.cells[(int)this.x, (int)(this.z + 1)];
		}
	}

	public Cell Left
	{
		get
		{
			if (this.x <= 0)
			{
				return Cell.Void;
			}
			return Cell.cells[(int)(this.x - 1), (int)this.z];
		}
	}

	public Cell FrontRight
	{
		get
		{
			if ((int)(this.x + 1) >= Cell.Size || this.z <= 0)
			{
				return Cell.Void;
			}
			return Cell.cells[(int)(this.x + 1), (int)(this.z - 1)];
		}
	}

	public Cell FrontLeft
	{
		get
		{
			if (this.x <= 0 || this.z <= 0)
			{
				return Cell.Void;
			}
			return Cell.cells[(int)(this.x - 1), (int)(this.z - 1)];
		}
	}

	public Cell BackRight
	{
		get
		{
			if ((int)(this.x + 1) >= Cell.Size || (int)(this.z + 1) >= Cell.Size)
			{
				return Cell.Void;
			}
			return Cell.cells[(int)(this.x + 1), (int)(this.z + 1)];
		}
	}

	public Cell BackLeft
	{
		get
		{
			if (this.x <= 0 || (int)(this.z + 1) >= Cell.Size)
			{
				return Cell.Void;
			}
			return Cell.cells[(int)(this.x - 1), (int)(this.z + 1)];
		}
	}

	public GrowSystem growth
	{
		get
		{
			GrowSystem.cell = this;
			return this.sourceObj.growth;
		}
	}

	public bool isSurrounded
	{
		get
		{
			return this.bits[0];
		}
		set
		{
			this.bits[0] = value;
		}
	}

	public bool isShadowed
	{
		get
		{
			return this.bits[1];
		}
		set
		{
			this.bits[1] = value;
		}
	}

	public bool hasDoor
	{
		get
		{
			return this.bits[3];
		}
		set
		{
			this.bits[3] = value;
		}
	}

	public bool skipRender
	{
		get
		{
			return this.bits[4];
		}
		set
		{
			this.bits[4] = value;
		}
	}

	public bool isSeen
	{
		get
		{
			return this.bits[6];
		}
		set
		{
			this.bits[6] = value;
		}
	}

	public bool isSurrounded4d
	{
		get
		{
			return this.bits[7];
		}
		set
		{
			this.bits[7] = value;
		}
	}

	public bool isForceFloat
	{
		get
		{
			return this.bits[8];
		}
		set
		{
			this.bits[8] = value;
		}
	}

	public bool blockSight
	{
		get
		{
			return this.bits[9];
		}
		set
		{
			this.bits[9] = value;
		}
	}

	public bool isHarvested
	{
		get
		{
			return this.bits[10];
		}
		set
		{
			this.bits[10] = value;
		}
	}

	public bool isWatered
	{
		get
		{
			return this.bits[11];
		}
		set
		{
			this.bits[11] = value;
		}
	}

	public bool isSlopeEdge
	{
		get
		{
			return this.bits[12];
		}
		set
		{
			this.bits[12] = value;
		}
	}

	public bool isBridgeEdge
	{
		get
		{
			return this.bits[13];
		}
		set
		{
			this.bits[13] = value;
		}
	}

	public bool ignoreObjShadow
	{
		get
		{
			return this.bits[14];
		}
		set
		{
			this.bits[14] = value;
		}
	}

	public bool nonGradient
	{
		get
		{
			return this.bits[15];
		}
		set
		{
			this.bits[15] = value;
		}
	}

	public bool impassable
	{
		get
		{
			return this.bits[16];
		}
		set
		{
			this.bits[16] = value;
		}
	}

	public bool outOfBounds
	{
		get
		{
			return this.bits[17];
		}
		set
		{
			this.bits[17] = value;
		}
	}

	public bool isWallEdge
	{
		get
		{
			return this.bits[20];
		}
		set
		{
			this.bits[20] = value;
		}
	}

	public bool isModified
	{
		get
		{
			return this.bits[21];
		}
		set
		{
			this.bits[21] = value;
		}
	}

	public bool isClearSnow
	{
		get
		{
			return this.bits[22];
		}
		set
		{
			this.bits[22] = value;
		}
	}

	public bool fullWall
	{
		get
		{
			return this.bits[23];
		}
		set
		{
			this.bits[23] = value;
		}
	}

	public bool isFloating
	{
		get
		{
			return this.bits[24];
		}
		set
		{
			this.bits[24] = value;
		}
	}

	public bool lotWall
	{
		get
		{
			return this.bits[25];
		}
		set
		{
			this.bits[25] = value;
		}
	}

	public bool hasDoorBoat
	{
		get
		{
			return this.bits[26];
		}
		set
		{
			this.bits[26] = value;
		}
	}

	public bool isDeck
	{
		get
		{
			return this.bits[27];
		}
		set
		{
			this.bits[27] = value;
		}
	}

	public bool castFloorShadow
	{
		get
		{
			return this.bits[28];
		}
		set
		{
			this.bits[28] = value;
		}
	}

	public bool lotShade
	{
		get
		{
			return this.bits[29];
		}
		set
		{
			this.bits[29] = value;
		}
	}

	public bool isShoreSand
	{
		get
		{
			return this.bits[30];
		}
		set
		{
			this.bits[30] = value;
		}
	}

	public bool isToggleWallPillar
	{
		get
		{
			return this.bits[31];
		}
		set
		{
			this.bits[31] = value;
		}
	}

	public bool hasWindow
	{
		get
		{
			return this.bits2[0];
		}
		set
		{
			this.bits2[0] = value;
		}
	}

	public bool isCurtainClosed
	{
		get
		{
			return this.bits2[1];
		}
		set
		{
			this.bits2[1] = value;
		}
	}

	public bool isSkyFloor
	{
		get
		{
			return this.bits2[2];
		}
		set
		{
			this.bits2[2] = value;
		}
	}

	public bool isClearArea
	{
		get
		{
			return this.bits2[3];
		}
		set
		{
			this.bits2[3] = value;
		}
	}

	public bool isObjDyed
	{
		get
		{
			return this.bits2[4];
		}
		set
		{
			this.bits2[4] = value;
		}
	}

	public bool HasObj
	{
		get
		{
			return this.obj > 0;
		}
	}

	public bool HasBlock
	{
		get
		{
			return this._block > 0;
		}
	}

	public bool HasFloor
	{
		get
		{
			return this._floor > 0;
		}
	}

	public bool HasRoof
	{
		get
		{
			return this.room != null && this.room.lot.idRoofStyle != 0;
		}
	}

	public bool HasFloorOrBlock
	{
		get
		{
			return this._block != 0 || this._floor > 0;
		}
	}

	public bool HasBridge
	{
		get
		{
			return this._bridge > 0;
		}
	}

	public bool HasLiquid
	{
		get
		{
			CellEffect cellEffect = this.effect;
			return cellEffect != null && cellEffect.IsLiquid;
		}
	}

	public bool HasFire
	{
		get
		{
			return this.effect != null && this.effect.IsFire;
		}
	}

	public bool IsNotWaterEdge
	{
		get
		{
			return this.HasFullBlock || Cell.floorList[(int)this._floor].tileType.IsWater || Cell.floorList[(int)this._floor].edge == 0;
		}
	}

	public bool IsTopWater
	{
		get
		{
			if (this._bridge != 0)
			{
				return Cell.floorList[(int)this._bridge].tileType.IsWater;
			}
			return Cell.floorList[(int)this._floor].tileType.IsWater;
		}
	}

	public bool IsTopWaterAndNoSnow
	{
		get
		{
			return this.IsTopWater && (!Cell.map.zone.IsSnowCovered || this.HasRoof || this.isClearSnow);
		}
	}

	public bool IsFloorWater
	{
		get
		{
			return Cell.floorList[(int)this._floor].tileType.IsWater;
		}
	}

	public bool IsFarmField
	{
		get
		{
			if (this._bridge != 0)
			{
				return this.sourceBridge.alias == "field";
			}
			return this.sourceFloor.alias == "field";
		}
	}

	public bool CanGrowWeed
	{
		get
		{
			if (this.obj != 0 || this.FirstThing != null || this.HasBlock || this.HasRoof)
			{
				return false;
			}
			if (this.IsFarmField)
			{
				return true;
			}
			if (this._bridge != 0)
			{
				return this.matBridge.category == "grass" && this.sourceBridge.tag.Contains("weed");
			}
			return this.matFloor.category == "grass" && this.sourceFloor.tag.Contains("weed");
		}
	}

	public bool IsBridgeWater
	{
		get
		{
			return this._bridge != 0 && Cell.floorList[(int)this._bridge].tileType.IsWater;
		}
	}

	public bool IsSnowTile
	{
		get
		{
			return Cell.map.zone.IsSnowCovered && !this.HasRoof && !this.IsTopWater && !this.isClearSnow && !this.isFloating && !this.isDeck;
		}
	}

	public bool IsDeepWater
	{
		get
		{
			return this.sourceSurface.tileType.IsDeepWater && !this.IsIceTile;
		}
	}

	public bool CanSuffocate()
	{
		if (!this.IsDeepWater)
		{
			return false;
		}
		if (this.detail != null && this.detail.things.Count > 0)
		{
			foreach (Thing thing in this.detail.things)
			{
				if (thing.IsInstalled && thing.isFloating)
				{
					return false;
				}
			}
			return true;
		}
		return true;
	}

	public bool IsIceTile
	{
		get
		{
			return Cell.map.zone.IsSnowCovered && !this.HasRoof && this.IsTopWater && !this.isClearSnow;
		}
	}

	public bool HasFullBlock
	{
		get
		{
			return Cell.blockList[(int)this._block].tileType.IsFullBlock;
		}
	}

	public bool HasFullBlockOrWallOrFence
	{
		get
		{
			return this.HasFullBlock || this.HasWallOrFence;
		}
	}

	public bool HasWallOrFence
	{
		get
		{
			return Cell.blockList[(int)this._block].tileType.IsWallOrFence;
		}
	}

	public bool HasWall
	{
		get
		{
			return Cell.blockList[(int)this._block].tileType.IsWall;
		}
	}

	public bool HasFence
	{
		get
		{
			return Cell.blockList[(int)this._block].tileType.IsFence;
		}
	}

	public bool HasRamp
	{
		get
		{
			return Cell.blockList[(int)this._block].tileType.IsRamp;
		}
	}

	public bool HasLadder
	{
		get
		{
			return Cell.blockList[(int)this._block].tileType.IsLadder;
		}
	}

	public bool HasRampOrLadder
	{
		get
		{
			return this.HasRamp || this.HasLadder;
		}
	}

	public bool HasSlope
	{
		get
		{
			return Cell.blockList[(int)this._block].tileType == TileType.Slope;
		}
	}

	public bool HasStairs
	{
		get
		{
			return Cell.blockList[(int)this._block].tileType == TileType.Stairs;
		}
	}

	public bool HasHalfBlock
	{
		get
		{
			return Cell.blockList[(int)this._block].tileType == TileType.HalfBlock;
		}
	}

	public bool HasBlockOrRamp
	{
		get
		{
			return Cell.blockList[(int)this._block].isBlockOrRamp;
		}
	}

	public bool UseLowBlock
	{
		get
		{
			return Cell.blockList[(int)this._block].tileType.UseLowBlock;
		}
	}

	public bool CastShadow
	{
		get
		{
			return Cell.blockList[(int)this._block].tileType.CastShadowSelf;
		}
	}

	public bool CastAmbientShadow
	{
		get
		{
			return Cell.blockList[(int)this._block].tileType.CastAmbientShadow;
		}
	}

	public bool IsRoomEdge
	{
		get
		{
			return this.Front.room != this.room || this.Right.room != this.room || this.FrontRight.room != this.room;
		}
	}

	public bool IsLotEdge
	{
		get
		{
			return this.Front.room == null || this.Right.room == null || this.FrontRight.room == null || this.Back.room == null || this.Left.room == null || (this.Front.room != null && (this.Front.room.data.atrium || !this.Front.HasRoof)) || (this.Right.room != null && (this.Right.room.data.atrium | !this.Right.HasRoof));
		}
	}

	public bool IsBlocked
	{
		get
		{
			return this.blocked;
		}
	}

	public bool HasObstacle()
	{
		return this.blocked || this._block != 0 || this.obj != 0 || this.hasDoor;
	}

	public bool CanBuildRamp(int dir)
	{
		return !this.HasRamp || this.blockDir == dir;
	}

	public int blockDir
	{
		get
		{
			return (int)(this._dirs % 4);
		}
		set
		{
			this._dirs = (byte)((int)this._dirs - this.blockDir + value);
		}
	}

	public int objDir
	{
		get
		{
			return (int)(this._dirs / 4 % 8);
		}
		set
		{
			this._dirs = (byte)((int)this._dirs - this.objDir * 4 + value % 8 * 4);
		}
	}

	public int floorDir
	{
		get
		{
			return (int)(this._dirs / 32);
		}
		set
		{
			this._dirs = (byte)((int)this._dirs - this.floorDir * 32 + value * 32);
		}
	}

	public bool IsSky
	{
		get
		{
			return this.sourceFloor.tileType == TileType.Sky && !this.HasBridge;
		}
	}

	public int liquidLv
	{
		get
		{
			if (this.effect != null && this.effect.LiquidAmount != 0)
			{
				CellEffect cellEffect = this.effect;
				return ((cellEffect != null) ? cellEffect.LiquidAmount : 0) / 3 + 1;
			}
			return 0;
		}
	}

	public int fireAmount
	{
		get
		{
			CellEffect cellEffect = this.effect;
			if (cellEffect == null)
			{
				return 0;
			}
			return cellEffect.FireAmount;
		}
	}

	public bool IsVoid
	{
		get
		{
			return this == Cell.Void;
		}
	}

	public Thing FirstThing
	{
		get
		{
			CellDetail cellDetail = this.detail;
			if (cellDetail == null || cellDetail.things.Count <= 0)
			{
				return null;
			}
			return this.detail.things[0];
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
			return (this.detail ?? CellDetail.Empty).things;
		}
	}

	public List<Chara> Charas
	{
		get
		{
			return (this.detail ?? CellDetail.Empty).charas;
		}
	}

	public int gatherCount
	{
		get
		{
			if (!Cell.map.gatherCounts.ContainsKey(this.index))
			{
				return 0;
			}
			return Cell.map.gatherCounts[this.index];
		}
		set
		{
			if (value == 0)
			{
				if (Cell.map.gatherCounts.ContainsKey(this.index))
				{
					Cell.map.gatherCounts.Remove(this.index);
					return;
				}
			}
			else
			{
				Cell.map.gatherCounts[this.index] = value;
			}
		}
	}

	public bool HasFloodBlock
	{
		get
		{
			return this.sourceBlock.tileType.IsFloodBlock || this.hasDoor;
		}
	}

	public SourceMaterial.Row matRoofBlock
	{
		get
		{
			return Cell.matList[(int)this._roofBlockMat];
		}
	}

	public SourceMaterial.Row matBlock
	{
		get
		{
			return Cell.matList[(int)this._blockMat];
		}
	}

	public SourceMaterial.Row matFloor
	{
		get
		{
			return Cell.matList[(int)this._floorMat];
		}
	}

	public SourceMaterial.Row matBridge
	{
		get
		{
			return Cell.matList[(int)this._bridgeMat];
		}
	}

	public SourceMaterial.Row matObj
	{
		get
		{
			return Cell.matList[(int)this.objMat];
		}
	}

	public SourceMaterial.Row matObj_fixed
	{
		get
		{
			if (!this.isObjDyed)
			{
				return Cell.matList[(int)this.objMat];
			}
			return this.sourceObj.DefaultMaterial;
		}
	}

	public SourceBlock.Row sourceRoofBlock
	{
		get
		{
			return Cell.blockList[(int)this._roofBlock];
		}
	}

	public SourceBlock.Row sourceBlock
	{
		get
		{
			return Cell.blockList[(int)this._block];
		}
	}

	public SourceFloor.Row sourceFloor
	{
		get
		{
			return Cell.floorList[(int)this._floor];
		}
	}

	public SourceFloor.Row sourceBridge
	{
		get
		{
			return Cell.floorList[(int)this._bridge];
		}
	}

	public SourceFloor.Row sourceSurface
	{
		get
		{
			if (this._bridge != 0)
			{
				return this.sourceBridge;
			}
			return this.sourceFloor;
		}
	}

	public SourceCellEffect.Row sourceEffect
	{
		get
		{
			CellEffect cellEffect = this.effect;
			return ((cellEffect != null) ? cellEffect.source : null) ?? Cell.effectList[0];
		}
	}

	public SourceObj.Row sourceObj
	{
		get
		{
			return Cell.objList[(int)this.obj];
		}
	}

	public BiomeProfile biome
	{
		get
		{
			if (!(this.sourceFloor.biome != null))
			{
				return EClass._zone.biome;
			}
			return this.sourceFloor.biome;
		}
	}

	public void Refresh()
	{
		Cell.<>c__DisplayClass311_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		Cell cell = (this.x > 0) ? Cell.cells[(int)(this.x - 1), (int)this.z] : Cell.Void;
		Cell cell2 = ((int)(this.x + 1) < Cell.Size) ? Cell.cells[(int)(this.x + 1), (int)this.z] : Cell.Void;
		Cell cell3 = (this.z > 0) ? Cell.cells[(int)this.x, (int)(this.z - 1)] : Cell.Void;
		Cell cell4 = ((int)(this.z + 1) < Cell.Size) ? Cell.cells[(int)this.x, (int)(this.z + 1)] : Cell.Void;
		Cell cell5 = (this.x > 0 && this.z > 0) ? Cell.cells[(int)(this.x - 1), (int)(this.z - 1)] : Cell.Void;
		Cell cell6 = ((int)(this.x + 1) < Cell.Size && this.z > 0) ? Cell.cells[(int)(this.x + 1), (int)(this.z - 1)] : Cell.Void;
		Cell cell7 = (this.x > 0 && (int)(this.z + 1) < Cell.Size) ? Cell.cells[(int)(this.x - 1), (int)(this.z + 1)] : Cell.Void;
		Cell cell8 = ((int)(this.x + 1) < Cell.Size && (int)(this.z + 1) < Cell.Size) ? Cell.cells[(int)(this.x + 1), (int)(this.z + 1)] : Cell.Void;
		Cell cell9 = (this.z > 1) ? Cell.cells[(int)this.x, (int)(this.z - 2)] : Cell.Void;
		Cell cell10 = ((int)(this.x + 2) < Cell.Size) ? Cell.cells[(int)(this.x + 2), (int)this.z] : Cell.Void;
		TileType tileType = this.sourceBlock.tileType;
		TileType tileType2 = this.sourceFloor.tileType;
		MapBounds bounds = Cell.map.bounds;
		bool flag = this._bridge > 0;
		this.outOfBounds = ((int)this.x < bounds.x || (int)this.z < bounds.z || (int)this.x > bounds.maxX || (int)this.z > bounds.maxZ);
		this.isSurrounded4d = (cell.HasFullBlock && cell2.HasFullBlock && cell3.HasFullBlock && cell4.HasFullBlock);
		this.isSurrounded = (this.isSurrounded4d && cell5.HasFullBlock && cell6.HasFullBlock && cell7.HasFullBlock && cell8.HasFullBlock && cell3.bridgeHeight == this.bridgeHeight && cell2.bridgeHeight == this.bridgeHeight && cell4.bridgeHeight == this.bridgeHeight && cell.bridgeHeight == this.bridgeHeight);
		this.isFloating = this.isForceFloat;
		this.hasDoor = false;
		this.hasDoorBoat = false;
		this.isDeck = false;
		this.isShoreSand = false;
		this.hasWindow = false;
		this.isCurtainClosed = false;
		this.isSkyFloor = (tileType2 != TileType.Sky && (cell2.sourceFloor.tileType == TileType.Sky || cell3.sourceFloor.tileType == TileType.Sky));
		Cell.openPath = false;
		Cell.openSight = (tileType.IsOpenSight || (cell3.hasWindow && !cell3.isCurtainClosed) || (cell2.hasWindow && !cell2.isCurtainClosed));
		this.blockSight = (tileType.IsBlockSight || ((Cell.objList[(int)this.obj].growth != null) ? Cell.objList[(int)this.obj].growth.BlockSight(this) : Cell.objList[(int)this.obj].tileType.IsBlockSight));
		this.blocked = (this.outOfBounds || tileType.IsBlockPass || (tileType2.IsBlockPass && !flag) || ((Cell.objList[(int)this.obj].growth != null) ? Cell.objList[(int)this.obj].growth.BlockPass(this) : Cell.objList[(int)this.obj].tileType.IsBlockPass) || this.impassable);
		this.isSlopeEdge = (this.height > cell2.height || this.height > cell3.height);
		if (flag && this.sourceBridge.tileType.ShowPillar)
		{
			this.isBridgeEdge = (this._bridge != cell2._bridge || this._bridge != cell3._bridge || this.bridgeHeight > cell2.bridgeHeight || this.bridgeHeight > cell3.bridgeHeight || this._bridge != cell._bridge || this._bridge != cell4._bridge || this.bridgeHeight > cell.bridgeHeight || this.bridgeHeight > cell4.bridgeHeight);
		}
		else
		{
			this.isBridgeEdge = false;
		}
		this.lotShade = false;
		if (this.room == null && !EClass._zone.IsSnowCovered)
		{
			if ((cell10.room != null && cell2.sourceBlock.tileType.CastShadowSelf) || (cell9.room != null && cell3.sourceBlock.tileType.CastShadowSelf))
			{
				this.lotShade = true;
			}
			else if ((cell10.Front.room != null && cell6.sourceBlock.tileType.CastShadowSelf) || (cell9.Right.room != null && cell6.sourceBlock.tileType.CastShadowSelf) || (cell6.FrontRight.room != null && cell6.Right.sourceBlock.tileType.CastShadowSelf))
			{
				this.lotShade = true;
			}
			else if (cell10.sourceBlock.tileType.CastShadowSelf && cell10.Right.room != null)
			{
				this.lotShade = true;
			}
		}
		this.isShadowed = (this.lotShade || this.sourceBlock.tileType.CastShadowSelf || cell3.sourceBlock.tileType.CastShadowBack || (!this.HasRoof && this._roofBlock > 0));
		this.castFloorShadow = (this.lotShade || (this.room == null && this.sourceBlock.tileType.CastShadowSelf));
		byte b = flag ? this.bridgeHeight : this.height;
		byte b2 = (cell3.bridgeHeight == 0) ? cell3.height : cell3.bridgeHeight;
		byte b3 = (cell2.bridgeHeight == 0) ? cell2.height : cell2.bridgeHeight;
		byte b4 = (cell9.bridgeHeight == 0) ? cell9.height : cell9.bridgeHeight;
		byte b5 = (cell10.bridgeHeight == 0) ? cell10.height : cell10.bridgeHeight;
		int num = 0;
		if ((int)(b3 - b) > num)
		{
			num = (int)(b3 - b);
		}
		if ((int)(b2 - b) > num)
		{
			num = (int)(b2 - b);
		}
		if ((int)(b5 - b - 4) > num)
		{
			num = (int)(b5 - b - 4);
		}
		if ((int)(b4 - b - 4) > num)
		{
			num = (int)(b4 - b - 4);
		}
		num -= 4;
		if (num > 15)
		{
			num = 15;
		}
		if (this.IsTopWater)
		{
			num /= 3;
		}
		else if (this.isShadowed)
		{
			num /= 2;
		}
		if (num <= 0)
		{
			num = 0;
		}
		this.shadowMod = (byte)num;
		this.baseWeight = 0;
		if (this.detail != null && this.detail.things.Count > 0)
		{
			for (int i = 0; i < this.detail.things.Count; i++)
			{
				Thing thing = this.detail.things[i];
				if (thing.IsInstalled)
				{
					if (thing.trait.WeightMod > this.baseWeight)
					{
						this.baseWeight = thing.trait.WeightMod;
					}
					if (!thing.isHidden)
					{
						if (thing.trait.IsFloating)
						{
							this.isFloating = true;
							if (this.IsFloorWater)
							{
								this.blocked = true;
							}
							else
							{
								this.isDeck = true;
							}
						}
						else
						{
							if (thing.trait.IsOpenPath)
							{
								this.blocked = false;
								this.hasDoorBoat = true;
								Cell.openPath = true;
								Cell.openSight = true;
							}
							if (thing.trait.IsBlockPath)
							{
								this.blocked = true;
							}
						}
						if (thing.trait.IsOpenSight)
						{
							Cell.openSight = true;
						}
						if (thing.trait is TraitDoor)
						{
							this.blocked = false;
							this.hasDoor = true;
							this.blockSight = true;
						}
						else if (thing.trait.IsBlockSight)
						{
							this.blockSight = true;
						}
						Trait trait = thing.trait;
						if (!(trait is TraitWindow))
						{
							if (trait is TraitCurtain)
							{
								this.isCurtainClosed = thing.isOn;
							}
						}
						else
						{
							this.hasWindow = true;
						}
					}
				}
			}
		}
		this.minHeight = (byte)((int)(10 + b) + ((this.IsTopWater && !this.isFloating) ? this.sourceFloor.tileType.FloorAltitude : 0));
		this.topHeight = this.minHeight + tileType.slopeHeight;
		this.weights[0] = ((Mathf.Abs((int)(cell3.minHeight - this.topHeight)) > 8 && Mathf.Abs((int)(cell3.topHeight - this.minHeight)) > 8) ? 0 : 1);
		this.weights[1] = ((Mathf.Abs((int)(cell2.minHeight - this.topHeight)) > 8 && Mathf.Abs((int)(cell2.topHeight - this.minHeight)) > 8) ? 0 : 1);
		this.weights[2] = ((Mathf.Abs((int)(cell4.minHeight - this.topHeight)) > 8 && Mathf.Abs((int)(cell4.topHeight - this.minHeight)) > 8) ? 0 : 1);
		this.weights[3] = ((Mathf.Abs((int)(cell.minHeight - this.topHeight)) > 8 && Mathf.Abs((int)(cell.topHeight - this.minHeight)) > 8) ? 0 : 1);
		if (this.IsDeepWater)
		{
			this.baseWeight += 100;
		}
		this.ignoreObjShadow = ((b2 > b && b2 - b > 6) || (b3 > b && b3 - b > 6));
		this.isWallEdge = (!this.blocked && cell2.sourceBlock.tileType.IsWallOrFence && cell2.blockDir != 1 && cell3.sourceBlock.tileType.IsWallOrFence && cell3.blockDir != 0);
		if (this.isWallEdge)
		{
			this.blockSight = (cell2.sourceBlock.tileType.IsBlockSight && cell3.sourceBlock.tileType.IsBlockSight);
			this.blocked = true;
			this.blockSight = true;
		}
		this.shadow = 0;
		if (!this.HasBlock)
		{
			if (cell.isSeen && cell.CastAmbientShadow && !cell.hasDoor && this.bridgeHeight == cell.bridgeHeight && !this.sourceFloor.ignoreTransition && (!cell.sourceBlock.tileType.IsWallOrFence || cell.blockDir != 0))
			{
				this.shadow += 1;
			}
			if (cell4.isSeen && cell4.CastAmbientShadow && !cell4.hasDoor && this.bridgeHeight == cell4.bridgeHeight && !this.sourceFloor.ignoreTransition && (!cell4.sourceBlock.tileType.IsWallOrFence || cell4.blockDir != 1))
			{
				this.shadow += 2;
			}
		}
		else
		{
			if (this.isFloating && (this.room != null || cell3.room != null || cell2.room != null || cell6.room != null))
			{
				this.isFloating = false;
			}
			if (this.sourceBlock.tileType.IsFence && !this.hasDoor && (this.blockDir != 0 || cell4.topHeight >= b - 3) && (this.blockDir != 1 || cell.topHeight >= b - 3))
			{
				this.shadow = (byte)(this.blockDir + 4);
			}
		}
		CS$<>8__locals1.isFloorWater = this.IsFloorWater;
		if (CS$<>8__locals1.isFloorWater)
		{
			this.shore = (byte)((((cell4.IsNotWaterEdge || (int)this.z == Cell.Size - 1 || this.height != cell4.height) ? 0 : 1) + ((cell2.IsNotWaterEdge || (int)this.x == Cell.Size - 1 || this.height != cell2.height) ? 0 : 2) + ((cell3.IsNotWaterEdge || this.z == 0 || this.height != cell3.height) ? 0 : 4) + ((cell.IsNotWaterEdge || this.x == 0 || this.height != cell.height) ? 0 : 8)) * 12);
			this.isShoreSand = (cell4.sourceFloor.isBeach || cell2.sourceFloor.isBeach || cell3.sourceFloor.isBeach || cell.sourceFloor.isBeach);
			if (this.shore != 0)
			{
				this.shore += (this.isShoreSand ? 2 : 3);
			}
		}
		else
		{
			this.shore = 0;
		}
		if (this.sourceFloor.autotile > 0)
		{
			this.autotile = (byte)(((!this.<Refresh>g__IsAutoTileEdge|311_0(cell4, ref CS$<>8__locals1) || (int)this.z == Cell.Size - 1) ? 0 : 1) + ((!this.<Refresh>g__IsAutoTileEdge|311_0(cell2, ref CS$<>8__locals1) || (int)this.x == Cell.Size - 1) ? 0 : 2) + ((!this.<Refresh>g__IsAutoTileEdge|311_0(cell3, ref CS$<>8__locals1) || this.z == 0) ? 0 : 4) + ((!this.<Refresh>g__IsAutoTileEdge|311_0(cell, ref CS$<>8__locals1) || this.x == 0) ? 0 : 8));
		}
		else
		{
			this.autotile = 0;
		}
		if (this._bridge != 0 && this.sourceBridge.autotile > 0)
		{
			this.autotileBridge = (byte)(((!this.<Refresh>g__IsBridgeAutoTileEdge|311_1(cell4, ref CS$<>8__locals1) || (int)this.z == Cell.Size - 1) ? 0 : 1) + ((!this.<Refresh>g__IsBridgeAutoTileEdge|311_1(cell2, ref CS$<>8__locals1) || (int)this.x == Cell.Size - 1) ? 0 : 2) + ((!this.<Refresh>g__IsBridgeAutoTileEdge|311_1(cell3, ref CS$<>8__locals1) || this.z == 0) ? 0 : 4) + ((!this.<Refresh>g__IsBridgeAutoTileEdge|311_1(cell, ref CS$<>8__locals1) || this.x == 0) ? 0 : 8));
		}
		else
		{
			this.autotileBridge = 0;
		}
		if (this.obj != 0 && this.sourceObj.autoTile)
		{
			this.autotileObj = (byte)(((!this.<Refresh>g__IsObjAutoTileEdge|311_2(cell4, ref CS$<>8__locals1) || (int)this.z == Cell.Size - 1) ? 0 : 1) + ((!this.<Refresh>g__IsObjAutoTileEdge|311_2(cell2, ref CS$<>8__locals1) || (int)this.x == Cell.Size - 1) ? 0 : 2) + ((!this.<Refresh>g__IsObjAutoTileEdge|311_2(cell3, ref CS$<>8__locals1) || this.z == 0) ? 0 : 4) + ((!this.<Refresh>g__IsObjAutoTileEdge|311_2(cell, ref CS$<>8__locals1) || this.x == 0) ? 0 : 8));
		}
		else
		{
			this.autotileObj = 0;
		}
		if (Cell.openSight)
		{
			this.blockSight = false;
		}
		if (Cell.openPath)
		{
			this.blocked = false;
		}
	}

	public void RotateBlock(int a)
	{
		int num = this.blockDir;
		int num2 = this.sourceBlock.tileType.IsWallOrFence ? 3 : this.sourceBlock._tiles.Length;
		num += a;
		if (num < 0)
		{
			num = num2 - 1;
		}
		if (num >= num2)
		{
			num = 0;
		}
		this.blockDir = num;
	}

	public void RotateFloor(int a)
	{
		int num = this.floorDir;
		num += a;
		if (num < 0)
		{
			num = this.sourceFloor._tiles.Length - 1;
		}
		if (num >= this.sourceFloor._tiles.Length)
		{
			num = 0;
		}
		this.floorDir = num;
	}

	public void RotateObj(bool reverse = false, bool useBlockDir = false)
	{
		int num = useBlockDir ? this.blockDir : this.objDir;
		if (reverse)
		{
			num--;
		}
		else
		{
			num++;
		}
		if (num < 0)
		{
			num = this.sourceObj._tiles.Length - 1;
		}
		if (num >= this.sourceObj._tiles.Length)
		{
			num = 0;
		}
		if (useBlockDir)
		{
			this.blockDir = num;
			return;
		}
		this.objDir = num;
	}

	public void RotateAll()
	{
		bool flag = EInput.isShiftDown || Input.GetMouseButton(1);
		PointTarget mouseTarget = Core.Instance.scene.mouseTarget;
		if (mouseTarget.card != null)
		{
			mouseTarget.card.Rotate(flag);
			return;
		}
		if (this.detail == null || this.detail.things.Count == 0)
		{
			if (this.HasBlock)
			{
				if (this.sourceBlock.tileType.CanRotate(false))
				{
					this.RotateBlock(flag ? -1 : 1);
				}
			}
			else if (this.HasObj)
			{
				if (this.sourceObj.tileType.CanRotate(false))
				{
					this.RotateObj(flag, this.sourceObj.tileType.IsUseBlockDir);
				}
			}
			else if (this.HasFloor)
			{
				this.RotateFloor(flag ? -1 : 1);
			}
		}
		this.GetPoint().RefreshNeighborTiles();
	}

	public void Reset()
	{
		this._block = (this._blockMat = (this._floor = (this._floorMat = (this.obj = (this.decal = (this.objVal = (this.objMat = 0)))))));
		this.bits.Bits = 0U;
		this.bits2.Bits = 0U;
	}

	public CellDetail GetOrCreateDetail()
	{
		if (this.detail == null)
		{
			this.detail = CellDetail.Spawn();
		}
		return this.detail;
	}

	public void TryDespawnDetail()
	{
		if (this.detail != null && this.detail.TryDespawn())
		{
			this.detail = null;
		}
	}

	public void AddCard(Card c)
	{
		this.GetOrCreateDetail();
		if (c.isChara)
		{
			this.detail.charas.Add(c.Chara);
			return;
		}
		this.detail.things.Add(c.Thing);
		c.Thing.stackOrder = this.detail.things.Count - 1;
		if (c.trait.ShouldRefreshTile || c.sourceCard.multisize)
		{
			this.GetPoint().RefreshNeighborTiles();
		}
	}

	public void RemoveCard(Card c)
	{
		if (c.isChara)
		{
			if (this.detail != null)
			{
				this.detail.charas.Remove(c.Chara);
			}
		}
		else
		{
			if (this.detail != null)
			{
				this.detail.things.Remove(c.Thing);
			}
			if (c.trait.ShouldRefreshTile || c.sourceCard.multisize)
			{
				this.GetPoint().RefreshNeighborTiles();
			}
		}
		this.TryDespawnDetail();
	}

	public string GetBlockName()
	{
		return Lang.Parse("blockName", this.matBlock.GetName(), this.sourceBlock.GetName(), null, null, null);
	}

	public string GetFloorName()
	{
		return Lang.Parse("blockName", this.matFloor.GetName(), this.sourceFloor.GetName(), null, null, null);
	}

	public string GetBridgeName()
	{
		return Lang.Parse("blockName", this.matBridge.GetName(), this.sourceBridge.GetName(), null, null, null);
	}

	public string GetObjName()
	{
		PlantData plantData = EClass._map.TryGetPlant(this);
		string text = this.sourceObj.GetName();
		if (this.sourceObj.tag.Contains("mat_name") && this.matObj.alias != "granite" && (this.sourceObj.id != 10 || !(this.matObj.alias == "crystal")))
		{
			text = "_of2".lang(this.matObj.GetName(), text, null, null, null);
		}
		if (plantData != null && plantData.size > 0)
		{
			text = Lang.GetList("plant_size")[plantData.size - 1] + text;
		}
		text = text.AddArticle();
		if (plantData != null)
		{
			if (plantData.seed != null)
			{
				if (!plantData.seed.c_refText.IsEmpty())
				{
					text = plantData.seed.c_refText;
				}
				if (plantData.seed.encLV > 0)
				{
					text = text + "+" + plantData.seed.encLV.ToString();
				}
			}
			if (plantData.fert != 0)
			{
				text += ((plantData.fert > 0) ? "fertilized" : "defertilized").lang();
			}
		}
		if (Cell.map.backerObjs.ContainsKey(this.index))
		{
			int key = Cell.map.backerObjs[this.index];
			SourceBacker.Row row = EClass.sources.backers.map.TryGetValue(key, null);
			if (row != null && EClass.core.config.backer.Show(row))
			{
				string name = row.Name;
				text = ((this.sourceObj.id == 82) ? "backerRemain".lang(text, name, null, null, null) : "backerTree".lang(text, name, null, null, null));
			}
		}
		return text;
	}

	public string GetLiquidName()
	{
		return Lang.Parse("liquidName", this.sourceEffect.GetName(), null, null, null, null);
	}

	public bool CanGrow(SourceObj.Row obj, VirtualDate date)
	{
		GrowSystem.cell = this;
		GrowSystem growth = obj.growth;
		return growth != null && growth.CanGrow(date);
	}

	public void TryGrow(VirtualDate date = null)
	{
		GrowSystem growth = this.growth;
		if (growth == null)
		{
			return;
		}
		growth.TryGrow(date);
	}

	public bool CanHarvest()
	{
		GrowSystem growth = this.growth;
		return growth != null && growth.CanHarvest();
	}

	public bool CanReapSeed()
	{
		GrowSystem growth = this.growth;
		return growth != null && growth.CanReapSeed();
	}

	public bool CanMakeStraw()
	{
		return this.sourceObj.alias == "wheat" && this.growth.IsLastStage();
	}

	public PlantData TryGetPlant()
	{
		return EClass._map.TryGetPlant(this);
	}

	public float GetSurfaceHeight()
	{
		if (this.detail == null)
		{
			return 0f;
		}
		bool isTopWater = this.IsTopWater;
		for (int i = 0; i < this.detail.things.Count; i++)
		{
			Thing thing = this.detail.things[i];
			if (thing.IsInstalled)
			{
				if (isTopWater)
				{
					if (thing.isFloating)
					{
						return thing.Pref.height + 0.1f;
					}
				}
				else if (thing.Pref.Surface)
				{
					return 0.1f * (float)thing.altitude + thing.Pref.height;
				}
			}
		}
		return 0f;
	}

	public bool HasZoneStairs(bool includeLocked = true)
	{
		if (this.detail == null)
		{
			return false;
		}
		foreach (Thing thing in this.detail.things)
		{
			if (thing.IsInstalled && (thing.trait is TraitStairs || (includeLocked && thing.trait is TraitStairsLocked)))
			{
				return true;
			}
		}
		return false;
	}

	public override bool IsPathBlocked(PathManager.MoveType moveType)
	{
		if (moveType == PathManager.MoveType.Default)
		{
			return this.blocked;
		}
		return this.blocked || (this.detail != null && this.detail.charas.Count > 0);
	}

	[CompilerGenerated]
	private bool <Refresh>g__IsAutoTileEdge|311_0(Cell cell, ref Cell.<>c__DisplayClass311_0 A_2)
	{
		return (cell.sourceFloor.autotilePriority <= this.sourceFloor.autotilePriority && (!this.sourceFloor.isBeach || !cell.sourceFloor.isBeach) && (cell._floor != this._floor || cell._floorMat != this._floorMat)) || (!A_2.isFloorWater && this.height != cell.height);
	}

	[CompilerGenerated]
	private bool <Refresh>g__IsBridgeAutoTileEdge|311_1(Cell cell, ref Cell.<>c__DisplayClass311_0 A_2)
	{
		return (cell.sourceBridge.autotilePriority <= this.sourceBridge.autotilePriority && (cell._bridge != this._bridge || cell._bridgeMat != this._bridgeMat)) || this.bridgeHeight != cell.bridgeHeight;
	}

	[CompilerGenerated]
	private bool <Refresh>g__IsObjAutoTileEdge|311_2(Cell cell, ref Cell.<>c__DisplayClass311_0 A_2)
	{
		return (cell.obj != this.obj && (this.obj != 31 || (cell.obj != 97 && cell.obj != 98))) || this.topHeight != cell.topHeight;
	}

	public static Map map;

	public static Cell Void = new Cell();

	public static List<SourceMaterial.Row> matList;

	public static List<SourceBlock.Row> blockList;

	public static List<SourceFloor.Row> floorList;

	public static List<SourceCellEffect.Row> effectList;

	public static List<SourceObj.Row> objList;

	public static int Size;

	public static Cell[,] cells;

	public const int DivAutotile = 12;

	public byte _dirs;

	public byte _block;

	public byte _blockMat;

	public byte _floor;

	public byte _floorMat;

	public byte obj;

	public byte objVal;

	public byte objMat;

	public byte decal;

	public byte _bridge;

	public byte _bridgeMat;

	public byte _roofBlock;

	public byte _roofBlockMat;

	public byte _roofBlockDir;

	public byte x;

	public byte z;

	public byte light;

	public byte autotile;

	public byte autotileBridge;

	public byte autotileObj;

	public byte shore;

	public byte shadow;

	public byte height;

	public byte bridgeHeight;

	public byte shadowMod;

	public byte topHeight;

	public byte minHeight;

	public byte bridgePillar;

	public byte highlight;

	public ushort lightR;

	public ushort lightG;

	public ushort lightB;

	public CellDetail detail;

	public CellEffect effect;

	public Critter critter;

	public BitArray32 bits;

	public BitArray32 bits2;

	public Room room;

	public bool pcSync;

	public const int DIV_LIQUID = 10;

	private static bool openSight;

	private static bool openPath;

	public const int DivStage = 30;
}
