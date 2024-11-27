using System;
using CreativeSpore.SuperTilemapEditor;
using UnityEngine;

public class TileMapElona : BaseTileMap
{
	public EloMap elomap
	{
		get
		{
			return EMono.scene.elomapActor.elomap;
		}
	}

	public override void Draw()
	{
		Zone zone = EMono._zone;
		float zoom = EMono.scene.camSupport.Zoom;
		SceneProfile profile = EMono.scene.profile;
		this.lightSetting = EMono.scene.profile.light;
		this.map = zone.map;
		this.Size = this.map.Size;
		this.SizeXZ = this.map.SizeXZ;
		this.isMining = (EMono.scene.actionMode == ActionMode.Mine);
		this.iconMode = EMono.scene.actionMode.cardIconMode;
		this.count = 0;
		this.totalFire = 0;
		this.pcX = EMono.pc.pos.x;
		this.pcZ = EMono.pc.pos.z;
		float num = this.lightSetting.nightRatioCurve.Evaluate(EMono.scene.timeRatio);
		this.modSublight1 = profile.global.modSublight1 * num;
		this.modSublight2 = profile.global.modSublight2 * num;
		this.pcMaxLight = EMono.player.lightPower * profile.global.fovModNonGradient * 0.8f;
		this.buildMode = EMono.scene.actionMode.IsBuildMode;
		this.subtleHighlightArea = (EMono.scene.actionMode.AreaHihlight == AreaHighlightMode.Build || EMono.scene.actionMode.AreaHihlight != AreaHighlightMode.Edit);
		this.highlightArea = (EMono.scene.actionMode.AreaHihlight == AreaHighlightMode.Edit || this.subtleHighlightArea);
		this.lightLimit = this.lightSetting.lightLimit.Evaluate(EMono.scene.timeRatio);
		this._lightMod = this.lightSetting.lightMod * this.lightSetting.lightModCurve.Evaluate(EMono.scene.timeRatio);
		this._baseBrightness = this.lightSetting.baseBrightness * this.lightSetting.baseBrightnessCurve.Evaluate(EMono.scene.timeRatio);
		Fov.nonGradientMod = profile.global.fovModNonGradient;
		this.shadowStrength = this.lightSetting.shadowCurve.Evaluate(EMono.scene.timeRatio);
		base.RefreshHeight();
		this.innerMode = ((this.buildMode || ActionMode.Select.ForceInnerBlockMode()) ? this.defaultInnerMode : (ActionMode.Bird.IsActive ? BaseTileMap.InnerMode.Height : this.defaultInnerMode));
		this.currentHeight = (int)EMono.pc.pos.cell.TopHeight;
		this.currentRoom = EMono.pc.pos.cell.room;
		this.map.rooms.Refresh();
		this.lowObj = false;
		if (this.usingHouseBoard || ActionMode.Bird.IsActive)
		{
			this.lowBlock = (this.hideRoomFog = (this.showFullWall = (this.hideHang = false)));
			this.showRoof = true;
		}
		else if (this.buildMode)
		{
			this.showRoof = EMono.game.config.showRoof;
			this.lowBlock = (!this.showRoof && !EMono.game.config.showWall);
			this.showFullWall = (!this.showRoof && EMono.game.config.showWall);
			this.hideRoomFog = true;
			this.hideHang = (!this.showRoof && !EMono.game.config.showWall);
		}
		else if (ActionMode.IsAdv)
		{
			if (EMono.pc.pos.cell.Front.UseLowBlock || EMono.pc.pos.cell.Right.UseLowBlock || EMono.pc.pos.cell.Front.Right.UseLowBlock || EMono.pc.pos.cell.UseLowBlock || EMono.pc.pos.cell.UseLowBlock)
			{
				if (!EMono.pc.IsMoving)
				{
					this.lowblockTimer = 0.1f;
				}
			}
			else
			{
				this.lowblockTimer = 0f;
			}
			this.lowBlock = (this.lowblockTimer > 0f);
			this.hideRoomFog = (this.currentRoom != null && this.currentRoom.lot.idRoofStyle != 0);
			this.x = EMono.pc.pos.x;
			this.z = EMono.pc.pos.z;
			this.showFullWall = this.hideRoomFog;
			this.showRoof = (!this.lowBlock && !this.hideRoomFog && EMono.game.config.noRoof);
			this.hideHang = (this.showFullWall || this.lowBlock);
			EMono.game.config.showRoof = !this.hideRoomFog;
		}
		else
		{
			this.lowBlock = (this.hideRoomFog = (this.showFullWall = (this.hideHang = false)));
			this.showRoof = true;
		}
		if (this.map.config.indoor)
		{
			this.showRoof = false;
		}
		int x = TilemapUtils.GetMouseGridX(this.elomap.fogmap, EMono.scene.cam) - this.elomap.minX;
		int z = TilemapUtils.GetMouseGridY(this.elomap.fogmap, EMono.scene.cam) - this.elomap.minY;
		WidgetDebug.output = x.ToString() + "/" + z.ToString() + "\n";
		base.HitPoint.Set(x, z);
		base.HitPoint.Clamp(false);
		this.z = 0;
		while (this.z < this.screen.height)
		{
			this.x = 0;
			while (this.x < this.screen.width)
			{
				this.cx = this.screen.scrollX + this.x;
				this.cz = this.screen.scrollY + this.z;
				if (this.cx >= 0 && this.cz >= 0 && this.cx < this.Size && this.cz < this.Size)
				{
					this.DrawTile();
				}
				this.x++;
			}
			this.z++;
		}
		EMono.scene.sfxFire.SetVolume(Mathf.Clamp(0.1f * (float)this.totalFire + ((this.totalFire != 0) ? 0.2f : 0f), 0f, 1f));
		Room currentRoom = this.currentRoom;
		int? num2;
		if (currentRoom == null)
		{
			num2 = null;
		}
		else
		{
			Lot lot = currentRoom.lot;
			num2 = ((lot != null) ? new int?(lot.idBGM) : null);
		}
		int? num3 = num2;
		int valueOrDefault = num3.GetValueOrDefault();
		if ((valueOrDefault != 0 && (EMono.Sound.currentPlaylist != EMono.Sound.plLot || EMono.Sound.plLot.list[0].data.id != valueOrDefault)) || (valueOrDefault == 0 && EMono.Sound.currentPlaylist == EMono.Sound.plLot))
		{
			EMono._zone.RefreshBGM();
		}
		if (this.currentRoom != this.lastRoom)
		{
			this.screen.RefreshWeather();
			this.lastRoom = this.currentRoom;
		}
	}

	public override void DrawTile()
	{
		this.count++;
		this.index = this.cx + this.cz * this.Size;
		this.cell = (this.param.cell = this.map.cells[this.cx, this.cz]);
		this.detail = this.cell.detail;
		this.isSeen = this.cell.isSeen;
		this.roof = this.cell.HasRoof;
		this.matBlock = this.cell.matBlock;
		this.matFloor = this.cell.matFloor;
		this.sourceBlock = this.cell.sourceBlock;
		this.sourceFloor = this.cell.sourceFloor;
		this.light = (float)this.cell.light;
		float num = Fov.DistanceFloat(this.cx, this.cz, this.pcX, this.pcZ);
		if (num < 4f)
		{
			num = 4f;
		}
		if (this.light < this.pcMaxLight && num < (float)(EMono.player.lightRadius - 1))
		{
			float num2 = (this.light + this.pcMaxLight) / 2f * this.modSublight1 / num;
			if (num2 > this.light)
			{
				this.light = num2;
			}
		}
		this._lowblock = this.lowBlock;
		this.liquidLv = (this.param.liquidLv = ((this.cell.liquidLv + (int)this.cell._bridge != 0) ? this.cell.sourceBridge.tileType.LiquidLV : this.sourceFloor.tileType.LiquidLV) * 10);
		if (this.liquidLv > 99)
		{
			this.liquidLv = (this.param.liquidLv = 99);
		}
		this.height = (int)this.cell.height;
		this.hasBridge = (this.cell._bridge > 0);
		if (this.cell.room != null)
		{
			this.cell.room.lot.sync = true;
		}
		this.blockLight = this._lightMod * this.lightLookUp[(int)this.light] + this._baseBrightness + ((this.roof || this.cell.isShadowed) ? this.shadowStrength : 0f);
		CellEffect effect = this.cell.effect;
		if (effect != null && effect.FireAmount > 0)
		{
			this.blockLight += 0.2f;
			this.totalFire++;
		}
		if (this.blockLight > this.lightLimit)
		{
			this.blockLight = this.lightLimit;
		}
		this.blockLight -= 0.025f * (float)this.cell.shadowMod * this._heightMod.x;
		this.param.color = (this.blockLight = (float)((int)(this.blockLight * 50f) * 262144 + (int)(((this.cell.lightR >= 64) ? 63 : this.cell.lightR) * 4096) + (int)(((this.cell.lightG >= 64) ? 63 : this.cell.lightG) * 64) + (int)((this.cell.lightB >= 64) ? 63 : this.cell.lightB)));
		this.param.x = (float)this.cx * this.screen.tileAlign.x;
		this.param.y = (float)this.cz * this.screen.tileAlign.y + (float)this.height * this._heightMod.y;
		this.param.z = 1000f + this.param.x * this.screen.tileWeight.x + this.param.y * this.screen.tileWeight.z + (float)this.height * this._heightMod.z;
		if (this.detail != null)
		{
			TransAnime anime = this.detail.anime;
			if (anime != null && anime.animeBlock)
			{
				TransAnime anime2 = this.detail.anime;
				this.param.x += anime2.v.x;
				this.param.y += anime2.v.y;
				this.param.z += anime2.v.z;
			}
			if (this.detail.designation != null)
			{
				this.detail.designation.Draw(this.cx, this.cz, this.param);
			}
		}
		if (this.cell.isSlopeEdge)
		{
			float num3 = (float)this.height * this._heightMod.y;
			this.orgY = this.param.y;
			this.orgZ = this.param.z;
			this.param.dir = this.cell.blockDir;
			if (this.cell.isSurrounded)
			{
				this.param.matColor = 0f;
				this.param.color = (float)(262144 * (int)((this._baseBrightness + this.fogBrightness) * 50f));
				this.param.tile = 0f;
				int num4 = 0;
				while ((float)num4 < num3 / this.heightBlockSize)
				{
					this.param.y += this.ugFix.y;
					this.param.z += this.ugFix.z + this.slopeFixZ * (float)num4;
					this.rendererInnerBlock.Draw(this.param);
					num4++;
				}
			}
			else
			{
				SourceBlock.Row row;
				if (this.sourceBlock.tileType.IsFullBlock)
				{
					row = this.sourceBlock;
					this.param.mat = this.matBlock;
					this.param.tile = (float)this.sourceBlock._tiles[this.cell.blockDir % this.sourceBlock._tiles.Length];
					this.param.matColor = (float)((this.sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.matBlock.matColor, this.sourceBlock.colorMod));
				}
				else
				{
					row = this.sourceFloor._defBlock;
					this.param.mat = this.matFloor;
					this.param.tile = (float)row._tiles[this.cell.blockDir % row._tiles.Length];
					if (row.id != 1)
					{
						this.param.matColor = (float)((this.sourceFloor.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.matFloor.matColor, this.sourceFloor.colorMod));
					}
					else
					{
						this.param.matColor = 104025f;
					}
				}
				int num5 = 0;
				while ((float)num5 < num3 / this.heightBlockSize)
				{
					this.param.y += this.ugFix.y;
					this.param.z += this.ugFix.z + this.slopeFixZ * (float)num5;
					row.renderData.Draw(this.param);
					num5++;
				}
			}
			this.param.y = this.orgY;
			this.param.z = this.orgZ;
		}
		if (this.sourceBlock.id != 0)
		{
			this.tileType = this.sourceBlock.tileType;
			this.room = this.cell.room;
			this._lowblock = (this.showFullWall ? (this.room != null) : this.lowBlock);
			if (this.tileType.RepeatBlock)
			{
				Room room;
				if ((room = this.room) == null && (room = this.cell.Front.room) == null)
				{
					room = (this.cell.Right.room ?? this.cell.FrontRight.room);
				}
				this.room = room;
				if (this.room != null)
				{
					this.roomHeight = ((this._lowblock && !this.tileType.ForceRpeatBlock) ? 0f : this.room.lot.realHeight);
					this.maxHeight = (float)(this.cz - this.cx) * this.screen.tileAlign.y + (float)this.room.lot.mh * this._heightMod.y;
				}
				else
				{
					this.roomHeight = 0f;
				}
			}
			else
			{
				this.roomHeight = 0f;
			}
			this.param.mat = this.matBlock;
			this.param.dir = this.cell.blockDir;
			switch (this.tileType.blockRenderMode)
			{
			case BlockRenderMode.FullBlock:
				if (this.cell.isSurrounded)
				{
					switch (this.innerMode)
					{
					case BaseTileMap.InnerMode.None:
					case BaseTileMap.InnerMode.Height:
						this.param.color = this.blockLight;
						break;
					case BaseTileMap.InnerMode.InnerBlock:
					case BaseTileMap.InnerMode.BuildMode:
						this.param.matColor = 104025f;
						this.param.color = (float)(262144 * (int)((this._baseBrightness + this.fogBrightness) * 50f));
						this.param.tile = (float)(this._lowblock ? 3000000 : 0);
						this.rendererInnerBlock.Draw(this.param);
						return;
					}
				}
				this.param.tile = (float)(this.sourceBlock._tiles[this.cell.blockDir % this.sourceBlock._tiles.Length] + (this._lowblock ? 3000000 : 0));
				this.param.matColor = (float)((this.sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.matBlock.matColor, this.sourceBlock.colorMod));
				if (this.roomHeight == 0f)
				{
					if (!this.cell.hasDoor)
					{
						this.sourceBlock.renderData.Draw(this.param);
					}
				}
				else
				{
					RenderData renderData = this.sourceBlock.renderData;
					RenderParam param = this.param;
					float maxHeight = this.maxHeight;
					float roomHeight = this.roomHeight;
					GameSetting.RenderSetting renderSetting = this.renderSetting;
					bool hasDoor = this.cell.hasDoor;
					CellEffect effect2 = this.cell.effect;
					renderData.DrawRepeatTo(param, maxHeight, roomHeight, ref renderSetting.peakFixBlock, hasDoor, (effect2 != null) ? effect2.FireAmount : 0, false);
				}
				break;
			case BlockRenderMode.WallOrFence:
			{
				this.orgY = this.param.y;
				this.orgZ = this.param.z;
				int blockDir = this.cell.blockDir;
				if (blockDir == 0 || blockDir == 2)
				{
					this.param.dir = 0;
					if (blockDir == 2 && this.cell.Left.sourceBlock.tileType.IsWallOrFence)
					{
						Cell left = this.cell.Left;
						this._sourceBlock = left.sourceBlock;
						if (this._sourceBlock.useAltColor)
						{
							this.param.matColor = (float)((this._sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.param.mat.altColor, this._sourceBlock.colorMod));
						}
						else
						{
							this.param.matColor = (float)((this._sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.param.mat.matColor, this._sourceBlock.colorMod));
						}
					}
					else
					{
						this._sourceBlock = this.sourceBlock;
						if (this._sourceBlock.useAltColor)
						{
							this.param.matColor = (float)((this._sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.param.mat.altColor, this._sourceBlock.colorMod));
						}
						else
						{
							this.param.matColor = (float)((this._sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.param.mat.matColor, this._sourceBlock.colorMod));
						}
					}
					this.param.tile = (float)(this.tile = this._sourceBlock._tiles[0] + (this._lowblock ? 1000000 : 0));
					if (this.roomHeight == 0f || !this.tileType.RepeatBlock)
					{
						if (!this.cell.hasDoor)
						{
							this._sourceBlock.renderData.Draw(this.param);
						}
					}
					else
					{
						RenderData renderData2 = this._sourceBlock.renderData;
						RenderParam param2 = this.param;
						float maxHeight2 = this.maxHeight;
						float roomHeight2 = this.roomHeight;
						GameSetting.RenderSetting renderSetting2 = this.renderSetting;
						bool hasDoor2 = this.cell.hasDoor;
						CellEffect effect3 = this.cell.effect;
						renderData2.DrawRepeatTo(param2, maxHeight2, roomHeight2, ref renderSetting2.peakFix, hasDoor2, (effect3 != null) ? effect3.FireAmount : 0, false);
					}
					this.param.z -= 0.01f;
					if (blockDir == 2 || (this.cell.Front.HasWallOrFence && this.cell.Front.blockDir != 0))
					{
						this.param.tile = (float)(this.tile + 16);
						if (this.roomHeight == 0f || !this.tileType.RepeatBlock)
						{
							this._sourceBlock.renderData.Draw(this.param);
						}
						else
						{
							this._sourceBlock.renderData.DrawRepeatTo(this.param, this.maxHeight, this.roomHeight, ref this.renderSetting.peakFix, false, 0, false);
						}
					}
					if (this.cell.Left.Front.HasWallOrFence && this.cell.Left.Front.blockDir == 1)
					{
						this.orgX = this.param.x;
						this.param.tile = (float)(this.tile + 16);
						this.param.x += this.cornerWallFix.x;
						this.param.y += this.cornerWallFix.y;
						this.param.z += this.cornerWallFix.z;
						if (this.roomHeight == 0f || !this.tileType.RepeatBlock)
						{
							this._sourceBlock.renderData.Draw(this.param);
						}
						else
						{
							this._sourceBlock.renderData.DrawRepeatTo(this.param, this.maxHeight, this.roomHeight + this.cornerWallFix.y, ref this.renderSetting.peakFix, false, 0, false);
						}
						this.param.x = this.orgX;
					}
				}
				if (blockDir == 1 || blockDir == 2)
				{
					this.param.y = this.orgY;
					this.param.z = this.orgZ;
					this.param.dir = 1;
					if (blockDir == 2 && this.cell.Back.sourceBlock.tileType.IsWallOrFence)
					{
						Cell back = this.cell.Back;
						this._sourceBlock = back.sourceBlock;
						if (this._sourceBlock.useAltColor)
						{
							this.param.matColor = (float)((this._sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.param.mat.altColor, this._sourceBlock.colorMod));
						}
						else
						{
							this.param.matColor = (float)((this._sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.param.mat.matColor, this._sourceBlock.colorMod));
						}
					}
					else
					{
						this._sourceBlock = this.sourceBlock;
						if (this._sourceBlock.useAltColor)
						{
							this.param.matColor = (float)((this._sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.param.mat.altColor, this._sourceBlock.colorMod));
						}
						else
						{
							this.param.matColor = (float)((this._sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.param.mat.matColor, this._sourceBlock.colorMod));
						}
					}
					this.param.tile = (float)(this.tile = -this._sourceBlock._tiles[0] + (this._lowblock ? -1000000 : 0));
					if (this.roomHeight == 0f || !this.tileType.RepeatBlock)
					{
						if (!this.cell.hasDoor)
						{
							this._sourceBlock.renderData.Draw(this.param);
						}
					}
					else
					{
						RenderData renderData3 = this._sourceBlock.renderData;
						RenderParam param3 = this.param;
						float maxHeight3 = this.maxHeight;
						float roomHeight3 = this.roomHeight;
						GameSetting.RenderSetting renderSetting3 = this.renderSetting;
						bool hasDoor3 = this.cell.hasDoor;
						CellEffect effect4 = this.cell.effect;
						renderData3.DrawRepeatTo(param3, maxHeight3, roomHeight3, ref renderSetting3.peakFix, hasDoor3, (effect4 != null) ? effect4.FireAmount : 0, false);
					}
					if (this.cell.Right.HasWallOrFence && this.cell.Right.blockDir != 1)
					{
						this.orgX = this.param.x;
						this.param.tile = (float)(-(float)this.tile + 16);
						if (this.roomHeight == 0f || !this.tileType.RepeatBlock)
						{
							this._sourceBlock.renderData.Draw(this.param);
						}
						else
						{
							this._sourceBlock.renderData.DrawRepeatTo(this.param, this.maxHeight, this.roomHeight, ref this.renderSetting.peakFix, false, 0, false);
						}
						this.param.x = this.orgX;
					}
				}
				if (this.cell.room != null && !this.hideRoomFog && !this.cell.hasDoor)
				{
					return;
				}
				this.param.y = this.orgY;
				this.param.z = this.orgZ;
				break;
			}
			case BlockRenderMode.Pillar:
			{
				RenderData renderData4 = this.sourceBlock.renderData;
				this.param.tile = (float)this.sourceBlock._tiles[this.cell.blockDir % this.sourceBlock._tiles.Length];
				this.param.matColor = (float)((this.sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.matBlock.matColor, this.sourceBlock.colorMod));
				int num6 = this.cell.objDir * 2 + 1;
				if (num6 == 0)
				{
					renderData4.Draw(this.param);
				}
				else
				{
					renderData4.DrawRepeat(this.param, num6, this.sourceBlock.tileType.RepeatSize, false);
				}
				this.param.tile = (float)renderData4.idShadow;
				SourcePref shadowPref = renderData4.shadowPref;
				int shadow = shadowPref.shadow;
				this.passShadow.AddShadow(this.param.x + renderData4.offsetShadow.x, this.param.y + renderData4.offsetShadow.y, this.param.z + renderData4.offsetShadow.z, ShadowData.Instance.items[shadow], shadowPref, 0, false);
				break;
			}
			case BlockRenderMode.HalfBlock:
				this._sourceBlock = ((this.sourceBlock.id == 5) ? EMono.sources.blocks.rows[this.matBlock.defBlock] : this.sourceBlock);
				this.param.tile = (float)this._sourceBlock._tiles[0];
				this.param.matColor = (float)((this._sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.matBlock.matColor, this._sourceBlock.colorMod));
				this.param.tile2 = this._sourceBlock.sourceAutoFloor._tiles[0];
				this.param.halfBlockColor = ((this._sourceBlock.sourceAutoFloor.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.matBlock.matColor, this._sourceBlock.sourceAutoFloor.colorMod));
				this.sourceBlock.renderData.Draw(this.param);
				break;
			default:
				this.param.tile = (float)(this.sourceBlock._tiles[this.cell.blockDir % this.sourceBlock._tiles.Length] + ((this._lowblock && this.tileType.UseLowWallTiles) ? 3000000 : 0));
				this.param.matColor = (float)((this.sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.matBlock.matColor, this.sourceBlock.colorMod));
				if (this.roomHeight == 0f)
				{
					this.sourceBlock.renderData.Draw(this.param);
				}
				else
				{
					this.sourceBlock.renderData.DrawRepeatTo(this.param, this.maxHeight, this.roomHeight, ref this.renderSetting.peakFixBlock, false, 0, false);
				}
				break;
			}
		}
		if (this.cell.effect != null)
		{
			if (this.cell.effect.IsLiquid)
			{
				SourceCellEffect.Row sourceEffect = this.cell.sourceEffect;
				SourceMaterial.Row defaultMaterial = sourceEffect.DefaultMaterial;
				this.tile = (int)(4 + Rand.bytes[this.index % Rand.MaxBytes] % 4);
				this.param.tile = (float)(this.tile + this.cell.sourceEffect._tiles[0]);
				this.param.mat = defaultMaterial;
				this.param.matColor = (float)BaseTileMap.GetColorInt(ref defaultMaterial.matColor, sourceEffect.colorMod);
				sourceEffect.renderData.Draw(this.param);
			}
			else
			{
				this.param.tile = (float)this.cell.effect.source._tiles[0];
				if (this.cell.effect.IsFire)
				{
					this.rendererEffect.Draw(this.param, 0);
				}
				else
				{
					this.cell.effect.source.renderData.Draw(this.param);
				}
			}
		}
		if (this.detail != null)
		{
			TransAnime anime3 = this.detail.anime;
			if (anime3 != null && !anime3.animeBlock)
			{
				TransAnime anime4 = this.detail.anime;
				this.param.x += anime4.v.x;
				this.param.y += anime4.v.y;
				this.param.z += anime4.v.z;
			}
		}
		if (this.cell.obj != 0)
		{
			SourceObj.Row sourceObj = this.cell.sourceObj;
			this.param.mat = this.cell.matObj;
			if (sourceObj.useAltColor)
			{
				this.param.matColor = (float)((sourceObj.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.param.mat.altColor, sourceObj.colorMod));
			}
			else
			{
				this.param.matColor = (float)((sourceObj.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.param.mat.matColor, sourceObj.colorMod));
			}
			if (sourceObj.HasGrowth)
			{
				this.cell.growth.OnRenderTileMap(this.param);
			}
			else
			{
				if (sourceObj.tileType.IsUseBlockDir)
				{
					this.param.tile = (float)sourceObj._tiles[this.cell.blockDir % sourceObj._tiles.Length];
				}
				else
				{
					this.param.tile = (float)sourceObj._tiles[this.cell.objDir % sourceObj._tiles.Length];
				}
				if (this._lowblock && sourceObj.tileType.IsSkipLowBlock)
				{
					this.param.tile += (float)(((this.param.tile > 0f) ? 1 : -1) * 3000000);
				}
				sourceObj.renderData.Draw(this.param);
				int shadow2 = sourceObj.pref.shadow;
				if (shadow2 > 1 && !this.cell.ignoreObjShadow)
				{
					this.passShadow.AddShadow(this.param.x + sourceObj.renderData.offsetShadow.x, this.param.y + sourceObj.renderData.offsetShadow.y, this.param.z + sourceObj.renderData.offsetShadow.z, ShadowData.Instance.items[shadow2], sourceObj.pref, 0, false);
				}
			}
		}
		if (this.cell.decal != 0)
		{
			this.passDecal.Add(this.param, (float)this.cell.decal, this.blockLight, 0f);
		}
		if (this.detail == null)
		{
			return;
		}
		if (this.highlightArea && this.detail.area != null)
		{
			this.passArea.Add(this.param, (float)(this.detail.area.GetTile(this.index) - (this.subtleHighlightArea ? 0 : 1)), 0f, 0f);
		}
		if (this.detail.footmark != null && this.sourceFloor.id != 0)
		{
			this.param.tile = (float)this.detail.footmark.tile;
			this.param.mat = this.matFloor;
			this.param.matColor = (float)this.floorMatColor;
			this.renderFootmark.Draw(this.param);
		}
		if (this.detail.things.Count == 0 && this.detail.charas.Count == 0)
		{
			return;
		}
		int num7 = 0;
		this.thingPos.x = 0f;
		this.thingPos.y = 0f;
		this.param.y += ((this.cell._bridge != 0) ? this.cell.sourceBridge.tileType.FloorHeight : this.sourceFloor.tileType.FloorHeight);
		this.orgX = this.param.x;
		this.orgY = this.param.y;
		this.orgZ = this.param.z;
		Thing thing = null;
		bool drawShadow = this.liquidLv == 0;
		if (this.detail.things.Count > 0)
		{
			float max = this.zSetting.max1;
			int i = 0;
			while (i < this.detail.things.Count)
			{
				Thing thing2 = this.detail.things[i];
				this._actorPos.x = this.orgX;
				this._actorPos.y = this.orgY;
				this._actorPos.z = this.orgZ;
				SourcePref pref = thing2.sourceCard.pref;
				TileType tileType = thing2.trait.tileType;
				float num8 = tileType.UseMountHeight ? 0f : ((pref.height == 0f) ? 0.1f : pref.height);
				if (tileType.CanStack || !thing2.IsInstalled)
				{
					if (((thing != null) ? thing.id : null) != thing2.id)
					{
						this._actorPos.x = this._actorPos.x + this.thingPos.x;
					}
					this._actorPos.y = this._actorPos.y + this.thingPos.y;
					this._actorPos.z = this._actorPos.z + (this.renderSetting.thingZ + (float)i * -0.01f + this.zSetting.mod1 * this.thingPos.y);
				}
				if (!thing2.IsInstalled || !tileType.UseMountHeight)
				{
					drawShadow = (this.thingPos.y == 0f && this.liquidLv == 0);
					goto IL_1DD1;
				}
				if (!this.hideHang || this.cell.room != null || thing2.altitude <= 3)
				{
					thing2.TileType.GetMountHeight(ref this._actorPos, Point.shared.Set(this.index), thing2.dir, thing2);
					drawShadow = false;
					goto IL_1DD1;
				}
				IL_1F27:
				i++;
				continue;
				IL_1DD1:
				this.param.liquidLv = ((i == 0) ? this.liquidLv : 0);
				this.thingPos.y = this.thingPos.y + num8;
				if (!thing2.sourceCard.multisize || (thing2.pos.x == this.cx && thing2.pos.z == this.cz))
				{
					if (this.iconMode != BaseTileMap.CardIconMode.None)
					{
						int num9 = 0;
						switch (this.iconMode)
						{
						case BaseTileMap.CardIconMode.Deconstruct:
							if (thing2.isDeconstructing)
							{
								num9 = 14;
							}
							break;
						case BaseTileMap.CardIconMode.State:
							if (thing2.placeState == PlaceState.installed)
							{
								num9 = 18;
							}
							break;
						}
						if (thing2.isNPCProperty)
						{
							num9 = 13;
						}
						if (num9 != 0)
						{
							this.passGuideBlock.Add(this._actorPos.x, this._actorPos.y, this._actorPos.z - 10f, (float)num9, 0f);
						}
					}
					thing2.SetRenderParam(this.param);
					thing2.renderer.Draw(this.param, ref this._actorPos, drawShadow);
				}
				this.param.x = this.orgX;
				this.param.y = this.orgY;
				this.param.z = this.orgZ;
				thing = thing2;
				goto IL_1F27;
			}
		}
		if (this.detail.charas.Count > 0)
		{
			this.param.shadowFix = 0f;
			this.param.color += 1310720f;
			float max2 = this.zSetting.max2;
			bool flag = EMono.pc.fov != null && EMono.pc.fov.lastPoints.ContainsKey(this.index);
			for (int j = 0; j < this.detail.charas.Count; j++)
			{
				Chara chara = this.detail.charas[j];
				if (chara.host == null)
				{
					if (!flag)
					{
						Party party = chara.party;
						if (((party != null) ? party.leader : null) != EMono.pc && !chara.isDead)
						{
							goto IL_221B;
						}
					}
					this._actorPos.x = this.orgX;
					this._actorPos.y = this.orgY;
					this._actorPos.z = this.orgZ;
					chara.SetRenderParam(this.param);
					bool isAliveInCurrentZone = chara.IsAliveInCurrentZone;
					if (chara.IsDeadOrSleeping && chara.IsPCC)
					{
						float num10 = chara.renderer.data.size.y * 0.3f;
						if (this.thingPos.y > max2)
						{
							this.thingPos.y = max2;
						}
						float num11 = this.thingPos.y + num10;
						float num12 = (float)j * -0.01f;
						if (num11 > this.zSetting.thresh1)
						{
							num12 = this.zSetting.mod1;
						}
						this._actorPos.x = this._actorPos.x + this.thingPos.x;
						this._actorPos.y = this._actorPos.y + this.thingPos.y;
						this._actorPos.z = this._actorPos.z + (this.renderSetting.laydownZ + num12);
						this.param.liquidLv = ((this.thingPos.y == 0f && this.liquidLv > 0) ? 90 : 0);
						this.thingPos.y = this.thingPos.y + num10 * 0.8f;
						chara.renderer.Draw(this.param, ref this._actorPos, this.liquidLv == 0);
					}
					else
					{
						this.param.liquidLv = this.liquidLv;
						this._actorPos.z = this._actorPos.z + (0.01f * (float)j + this.renderSetting.charaZ);
						num7++;
						chara.renderer.Draw(this.param, ref this._actorPos, this.liquidLv == 0);
					}
					this.param.x = this.orgX;
					this.param.y = this.orgY;
					this.param.z = this.orgZ;
				}
				IL_221B:;
			}
		}
	}
}
