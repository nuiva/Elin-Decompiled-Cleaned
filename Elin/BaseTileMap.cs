using System;
using System.Collections.Generic;
using NoiseSystem;
using UnityEngine;

public class BaseTileMap : EMono
{
	public Point HitPoint
	{
		get
		{
			return Scene.HitPoint;
		}
	}

	public void OnActivate(BaseGameScreen _screen)
	{
		this.screen = _screen;
		if (this.lightLookUp == null)
		{
			this.lightLookUp = new float[256];
			for (int i = 0; i < 256; i++)
			{
				this.lightLookUp[i] = EMono.scene.profile.global.lightLookupCurve.Evaluate((float)i / 255f);
			}
		}
		this.selector = this.screen.tileSelector;
		this.renderSetting = EMono.setting.render;
		this.zSetting = this.renderSetting.zSetting;
		RenderObject.syncList = EMono.scene.syncList;
		RenderObject.altitudeFix = this.altitudeFix.y;
		if (Rand.bytes == null)
		{
			Rand.InitBytes(1);
		}
		this.RefreshHeight();
	}

	public unsafe virtual void Draw()
	{
		Zone zone = EMono._zone;
		this.map = zone.map;
		this.Size = this.map.Size;
		this.SizeXZ = this.map.SizeXZ;
		this.isIndoor = EMono._map.IsIndoor;
		this.count = 0;
		this.totalFire = 0;
		this.pcX = EMono.pc.pos.x;
		this.pcZ = EMono.pc.pos.z;
		float zoom = EMono.scene.camSupport.Zoom;
		SceneProfile profile = EMono.scene.profile;
		this.lightSetting = profile.light;
		this.buildMode = EMono.scene.actionMode.IsBuildMode;
		if (ActionMode.ViewMap.IsActive)
		{
			this.buildMode = false;
		}
		this.cinemaMode = ActionMode.Cinema.IsActive;
		this.isMining = (EMono.scene.actionMode == ActionMode.Mine);
		this.iconMode = EMono.scene.actionMode.cardIconMode;
		this.showAllCards = this.buildMode;
		this.highlightCells = ActionMode.FlagCell.IsActive;
		this.subtleHighlightArea = (EMono.scene.actionMode.AreaHihlight != AreaHighlightMode.None && (EMono.scene.actionMode.AreaHihlight == AreaHighlightMode.Build || EMono.scene.actionMode.AreaHihlight != AreaHighlightMode.Edit));
		this.highlightArea = (EMono.scene.actionMode.AreaHihlight == AreaHighlightMode.Edit || this.subtleHighlightArea);
		this.nightRatio = profile.light.nightRatioCurve.Evaluate(EMono.scene.timeRatio);
		this._rightWallShade = (float)((int)(50f * this.rightWallShade) * 262144);
		this.alwaysLowblock = EMono._zone.AlwaysLowblock;
		this.lowWallObjAltitude = EMono.game.config.lowWallObjAltitude;
		this.showBorder = ((EMono.core.config.game.showBorder == 1 && EMono._zone is Zone_Field && !EMono._zone.IsPlayerFaction) || (EMono.core.config.game.showBorder == 2 && !EMono._zone.map.IsIndoor && !EMono._zone.IsSkyLevel && !EMono._zone.IsRegion));
		this.snowLimit = profile.global.snowLimit.Evaluate(EMono.scene.timeRatio);
		this.snowLight = profile.global.snowLight;
		this.snowColor = profile.global.snowColor;
		this.snowColor2 = profile.global.snowColor2;
		this.snowColorToken = (int)((float)profile.global.snowRGB.x * this.nightRatio) * 4096 + (int)((float)profile.global.snowRGB.y * this.nightRatio) * 64 + (int)((float)profile.global.snowRGB.z * this.nightRatio);
		this.isSnowCovered = zone.IsSnowCovered;
		if (EMono.game.config.reverseSnow && this.buildMode)
		{
			this.isSnowCovered = !this.isSnowCovered;
		}
		this.roofLightLimitMod = profile.global.roofLightLimitMod.Evaluate(EMono.scene.timeRatio);
		this.fogBounds = ((EMono._map.config.forceHideOutbounds && (!EMono.debug.godBuild || !this.buildMode)) || (EMono.core.config.game.dontRenderOutsideMap && !this.buildMode));
		this.heightLightMod = ((EMono._map.config.heightLightMod == 0f) ? 0.005f : EMono._map.config.heightLightMod);
		this.modSublight1 = profile.global.modSublight1;
		this.modSublight2 = profile.global.modSublight2 * this.nightRatio;
		this.pcMaxLight = EMono.player.lightPower * 2f * (float)(EMono.player.lightRadius - 2);
		this.fogBrightness = this.lightSetting.fogBrightness.Evaluate(EMono.scene.timeRatio);
		this.lightLimit = this.lightSetting.lightLimit.Evaluate(EMono.scene.timeRatio);
		this._lightMod = this.lightSetting.lightMod * this.lightSetting.lightModCurve.Evaluate(EMono.scene.timeRatio);
		this.destBrightness = this.lightSetting.baseBrightness * this.lightSetting.baseBrightnessCurve.Evaluate(EMono.scene.timeRatio);
		float num = this.destBrightness;
		this.destBrightness = this.destBrightness * this.lightSetting.dynamicBrightnessCurve2.Evaluate(EMono.scene.timeRatio) + this.lightSetting.dynamicBrightnessCurve.Evaluate(EMono.scene.timeRatio) * (float)EMono.pc.pos.cell.light + ((EMono.pc.pos.cell.light == 0) ? 0f : this.lightSetting.dynamicBrightnessLightBonus);
		if (this.destBrightness > num)
		{
			this.destBrightness = num;
		}
		if (!Mathf.Approximately(this._baseBrightness, this.destBrightness))
		{
			this._baseBrightness = Mathf.Lerp(this._baseBrightness, this.destBrightness, Core.delta * this.lightSetting.dynamicBrightnessSpeed);
		}
		if (this.activeCount == 0)
		{
			this._baseBrightness = this.destBrightness;
		}
		this.activeCount++;
		if (this.buildMode && EMono.game.config.buildLight)
		{
			this._baseBrightness = 0.7f;
		}
		Fov.nonGradientMod = profile.global.fovModNonGradient;
		this.shadowStrength = this.lightSetting.shadowCurve.Evaluate(EMono.scene.timeRatio);
		this.floorShadowStrength = this.lightSetting.shadowCurveFloor.Evaluate(EMono.scene.timeRatio);
		float num2 = Core.delta * (float)EMono.game.config.animeSpeed * 0.01f;
		this.floatTimer += num2;
		if (this.floatTimer > this.floatSpeed)
		{
			this.floatTimer -= this.floatSpeed;
			this.floatY += (float)this.floatV;
			if (this.floatY >= (float)this.maxFloat)
			{
				this.floatV *= -1;
			}
			if (this.floatY < 0f)
			{
				this.floatV *= -1;
			}
		}
		this.waterAnimeTimer += num2;
		if (this.waterAnimeTimer > 0.5f)
		{
			this.waterAnimeTimer = 0f;
			this.waterAnimeIndex++;
		}
		if (this.cinemaMode)
		{
			CinemaConfig cinemaConfig = EMono.player.cinemaConfig;
			profile = ActionMode.Cinema.profile;
			this.lightSetting = profile.light;
			this.lightLimit = this.lightSetting.lightLimit.Evaluate(EMono.scene.timeRatio);
			this._lightMod += this.lightSetting.lightMod * this.lightSetting.lightModCurve.Evaluate(EMono.scene.timeRatio) * (0.01f * (float)cinemaConfig.light);
			this.destBrightness += 0.01f * (float)cinemaConfig.brightness;
			EMono.scene.camSupport.grading.cinemaBrightness = 0.01f * (float)cinemaConfig.brightness;
			EMono.scene.camSupport.grading.SetGrading();
			this.heightLightMod = 0f;
		}
		this.map.rooms.Refresh();
		this.noSlopMode = (EInput.isAltDown && EInput.isShiftDown);
		this.RefreshHeight();
		this.innerMode = ((this.buildMode || !this.isIndoor) ? BaseTileMap.InnerMode.None : this.defaultInnerMode);
		if (EMono.pc.IsInActiveZone)
		{
			this.currentHeight = (int)EMono.pc.pos.cell.TopHeight;
			this.currentRoom = EMono.pc.pos.cell.room;
		}
		else
		{
			this.currentHeight = 0;
			this.currentRoom = null;
		}
		this.lowObj = false;
		this.defaultBlockHeight = this.map.config.blockHeight;
		this.noRoofMode = false;
		bool flag = !this.isIndoor || EMono._zone is Zone_Tent;
		if (this.usingHouseBoard || ActionMode.Bird.IsActive)
		{
			this.lowBlock = (this.hideRoomFog = (this.hideHang = false));
			this.showRoof = (this.showFullWall = flag);
			if (ActionMode.Bird.IsActive)
			{
				this.fogBounds = false;
			}
		}
		else if (this.buildMode)
		{
			this.defaultBlockHeight = 0f;
			if (this.HitPoint.IsValid)
			{
				this.currentRoom = this.HitPoint.cell.room;
			}
			this.hideRoomFog = true;
			this.showRoof = (flag && EMono.game.config.showRoof);
			this.showFullWall = (this.showRoof || EMono.game.config.showWall);
			this.lowBlock = !this.showFullWall;
			this.hideHang = !this.showFullWall;
			if (this.cinemaMode)
			{
				this.hideRoomFog = !this.showRoof;
			}
		}
		else if (ActionMode.IsAdv)
		{
			this.noRoofMode = (EMono.game.config.noRoof || this.screen.focusOption != null);
			if (EMono.pc.pos.cell.Front.UseLowBlock || EMono.pc.pos.cell.Right.UseLowBlock || EMono.pc.pos.cell.Front.Right.UseLowBlock || EMono.pc.pos.cell.UseLowBlock || (EMono.pc.pos.cell.Front.Right.isWallEdge && EMono.pc.pos.cell.Front.Right.Right.UseLowBlock))
			{
				if (!EMono.pc.IsMoving)
				{
					this.lowblockTimer = 0.1f;
				}
			}
			else if (!EInput.rightMouse.pressing)
			{
				this.lowblockTimer = 0f;
			}
			this.x = EMono.pc.pos.x;
			this.z = EMono.pc.pos.z;
			Room room = null;
			if (room != null)
			{
				this.currentRoom = room;
			}
			if (this.currentRoom != null)
			{
				this.currentRoom.data.visited = true;
			}
			if (room != null)
			{
				room.data.visited = true;
			}
			this.lowBlock = (this.lowblockTimer > 0f);
			this.hideRoomFog = (this.currentRoom != null && (this.currentRoom.HasRoof || this.isIndoor));
			if (this.hideRoomFog)
			{
				this.lowBlock = true;
			}
			if (this.noRoofMode && (this.currentRoom == null || this.currentRoom.lot.idRoofStyle == 0))
			{
				this.hideRoomFog = true;
				this.showRoof = (this.showFullWall = false);
			}
			else
			{
				this.showRoof = (this.showFullWall = (flag && !this.lowBlock && !this.hideRoomFog));
			}
			this.hideHang = this.lowBlock;
			EMono.game.config.showRoof = !this.hideRoomFog;
			if (BaseTileMap.forceShowHang)
			{
				this.hideHang = false;
				BaseTileMap.forceShowHang = false;
			}
		}
		else
		{
			this.lowBlock = (this.hideRoomFog = (this.hideHang = false));
			this.showRoof = (this.showFullWall = true);
		}
		Room room2 = this.currentRoom;
		this.currentLot = (((room2 != null) ? room2.lot : null) ?? null);
		Vector3 mposWorld = EInput.mposWorld;
		mposWorld.z = 0f;
		Vector3Int vector3Int = this.screen.grid.WorldToCell(mposWorld);
		this.mx = -vector3Int.y;
		this.mz = vector3Int.x - 1;
		this.HitPoint.Set(this.mx, this.mz);
		bool isAltDown = EInput.isAltDown;
		for (int i = this.maxColliderCheck; i >= 0; i--)
		{
			this.TestPoint.x = this.mx + i;
			this.TestPoint.z = this.mz - i;
			if (this.TestPoint.x >= 0 && this.TestPoint.x < this.Size && this.TestPoint.z >= 0 && this.TestPoint.z < this.Size)
			{
				this.mouseCollider.transform.position = (isAltDown ? (*this.TestPoint.Position((int)this.TestPoint.cell.height)) : (*this.TestPoint.Position())) + this.colliderFix;
				Physics2D.SyncTransforms();
				if (Physics2D.RaycastNonAlloc(mposWorld, Vector2.zero, this.rays) > 0)
				{
					this.HitPoint.Set(this.TestPoint);
					break;
				}
			}
			this.TestPoint.x = this.mx + i - 1;
			this.TestPoint.z = this.mz - i;
			if (this.TestPoint.x >= 0 && this.TestPoint.x < this.Size && this.TestPoint.z >= 0 && this.TestPoint.z < this.Size)
			{
				this.mouseCollider.transform.position = (isAltDown ? (*this.TestPoint.Position((int)this.TestPoint.cell.height)) : (*this.TestPoint.Position())) + this.colliderFix;
				Physics2D.SyncTransforms();
				if (Physics2D.RaycastNonAlloc(mposWorld, Vector2.zero, this.rays) > 0)
				{
					this.HitPoint.Set(this.TestPoint);
					break;
				}
			}
			this.TestPoint.x = this.mx + i;
			this.TestPoint.z = this.mz - i + 1;
			if (this.TestPoint.x >= 0 && this.TestPoint.x < this.Size && this.TestPoint.z >= 0 && this.TestPoint.z < this.Size)
			{
				this.mouseCollider.transform.position = (isAltDown ? (*this.TestPoint.Position((int)this.TestPoint.cell.height)) : (*this.TestPoint.Position())) + this.colliderFix;
				Physics2D.SyncTransforms();
				if (Physics2D.RaycastNonAlloc(mposWorld, Vector2.zero, this.rays) > 0)
				{
					this.HitPoint.Set(this.TestPoint);
					break;
				}
			}
		}
		this.HitPoint.Clamp(false);
		bool flag2 = EMono._zone.UseFog && !EMono.scene.bg.wall;
		this.z = 0;
		while (this.z < this.screen.height)
		{
			this.x = 0;
			while (this.x < this.screen.width)
			{
				this.cx = this.screen.scrollX - this.screen.scrollY + this.x - (this.z + 1) / 2;
				this.cz = this.screen.scrollY + this.screen.scrollX + this.x + this.z / 2;
				if (((this.cz < 0 && this.cx >= -this.cz && this.cx > 0 && this.cx < this.Size - this.cz) || (this.cx >= this.Size && this.cx < this.Size * 2 - this.cz - 1 && this.cz >= -this.cx && this.cz < this.Size - 1)) && EMono.scene.bg.wall)
				{
					this.param.x = (float)(this.cx + this.cz) * this.screen.tileAlign.x + this.edgeBlockFix.x;
					this.param.y = (float)(this.cz - this.cx) * this.screen.tileAlign.y + this.edgeBlockFix.y;
					this.param.z = 1000f + this.param.x * this.screen.tileWeight.x + this.param.y * this.screen.tileWeight.z + (float)this.z * this.edgeBlockFix.z;
					this.blockLight = 10485760f;
					this.pass = this.passInner;
					this.batch = this.pass.batches[this.pass.batchIdx];
					this.batch.matrices[this.pass.idx].m03 = this.param.x;
					this.batch.matrices[this.pass.idx].m13 = this.param.y;
					this.batch.matrices[this.pass.idx].m23 = this.param.z;
					this.batch.tiles[this.pass.idx] = (float)(4 + ((this.cx >= this.Size) ? 1 : 0));
					this.batch.colors[this.pass.idx] = this.blockLight;
					this.batch.matColors[this.pass.idx] = 104025f;
					this.pass.idx++;
					if (this.pass.idx == this.pass.batchSize)
					{
						this.pass.NextBatch();
					}
				}
				else if (this.cx < 0 || this.cz < 0 || this.cx >= this.Size || this.cz >= this.Size)
				{
					if (flag2)
					{
						this.param.x = (float)(this.cx + this.cz) * this.screen.tileAlign.x;
						this.param.y = (float)(this.cz - this.cx) * this.screen.tileAlign.y + (float)this.height * this._heightMod.y;
						this.param.z = 1000f + this.param.x * this.screen.tileWeight.x + this.param.y * this.screen.tileWeight.z + (float)this.height * this._heightMod.z;
						this.param.tile = 0f;
						this.rendererFov2.Draw(this.param);
					}
				}
				else
				{
					this.DrawTile();
				}
				this.x++;
			}
			this.z++;
		}
		if (this.showRoof)
		{
			using (List<Lot>.Enumerator enumerator = this.map.rooms.listLot.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Lot lot = enumerator.Current;
					if (lot.sync)
					{
						this.DrawRoof(lot);
						lot.sync = false;
						lot.light = 0f;
					}
				}
				goto IL_1589;
			}
		}
		foreach (Lot lot2 in this.map.rooms.listLot)
		{
			lot2.sync = false;
			lot2.light = 0f;
		}
		IL_1589:
		EMono.scene.sfxFire.SetVolume(Mathf.Clamp(0.1f * (float)this.totalFire + ((this.totalFire != 0) ? 0.2f : 0f), 0f, 1f));
		this.room = EMono.pc.pos.cell.room;
		Room room3 = this.room;
		int? num3;
		if (room3 == null)
		{
			num3 = null;
		}
		else
		{
			Lot lot3 = room3.lot;
			num3 = ((lot3 != null) ? new int?(lot3.idBGM) : null);
		}
		int? num4 = num3;
		int valueOrDefault = num4.GetValueOrDefault();
		if (valueOrDefault != 0)
		{
			if (EMono.Sound.currentPlaylist != EMono.Sound.plLot)
			{
				goto IL_1690;
			}
			BGMData data = EMono.Sound.plLot.list[0].data;
			if (data == null || data.id != valueOrDefault)
			{
				goto IL_1690;
			}
		}
		if (valueOrDefault != 0 || !(EMono.Sound.currentPlaylist == EMono.Sound.plLot))
		{
			goto IL_169A;
		}
		IL_1690:
		EMono._zone.RefreshBGM();
		IL_169A:
		if (this.room != this.lastRoom)
		{
			this.screen.RefreshWeather();
			this.lastRoom = this.room;
		}
		SoundManager.bgmVolumeMod = ((!LayerDrama.maxBGMVolume && !EMono._map.IsIndoor && this.room != null && !this.room.data.atrium && this.room.HasRoof) ? -0.6f : 0f);
		this.screenHighlight = BaseTileMap.ScreenHighlight.None;
	}

	public void RefreshHeight()
	{
		if (EMono.game == null)
		{
			return;
		}
		float num = ((this.buildMode && !EMono.game.config.slope) || this.noSlopMode) ? 0f : ((float)EMono.game.config.slopeMod * 0.01f);
		this._heightMod = this.heightMod;
		this._heightMod.x = num;
		this._heightMod.y = this._heightMod.y * num;
		this._heightMod.z = this._heightMod.z * num;
	}

	public virtual void DrawTile()
	{
		BaseTileMap.<>c__DisplayClass221_0 CS$<>8__locals1 = new BaseTileMap.<>c__DisplayClass221_0();
		CS$<>8__locals1.<>4__this = this;
		this.count++;
		this.index = this.cx + this.cz * this.Size;
		this.cell = (this.param.cell = this.map.cells[this.cx, this.cz]);
		this.detail = this.cell.detail;
		this.isSeen = this.cell.isSeen;
		this.room = this.cell.room;
		this.roof = this.cell.HasRoof;
		this.matBlock = this.cell.matBlock;
		this.sourceBlock = this.cell.sourceBlock;
		bool flag = this.cell.isFloating;
		this.snowed = (this.isSnowCovered && !this.roof && !this.cell.isClearSnow && !this.cell.isDeck && !flag);
		this._shadowStrength = (this.snowed ? (this.shadowStrength * 0.4f) : this.shadowStrength);
		this.floorDir = this.cell.floorDir;
		if (this.snowed && !this.cell.sourceFloor.ignoreSnow)
		{
			if (this.cell.IsFloorWater)
			{
				this.sourceFloor = FLOOR.sourceIce;
				this.matFloor = this.cell.matFloor;
			}
			else
			{
				if (this.cell.sourceObj.snowTile > 0)
				{
					this.sourceFloor = FLOOR.sourceSnow2;
					this.floorDir = this.cell.sourceObj.snowTile - 1;
				}
				else if (this.index % 3 == 0 && Rand.bytes[this.index % Rand.MaxBytes] < 8 && !this.cell.HasObj && this.cell.FirstThing == null)
				{
					this.sourceFloor = FLOOR.sourceSnow2;
					this.floorDir = (int)Rand.bytes[this.index % Rand.MaxBytes];
				}
				else
				{
					this.sourceFloor = FLOOR.sourceSnow;
				}
				this.matFloor = MATERIAL.sourceSnow;
			}
		}
		else
		{
			this.sourceFloor = this.cell.sourceFloor;
			this.matFloor = this.cell.matFloor;
		}
		bool isWater = this.sourceFloor.tileType.IsWater;
		this.light = (float)(this.cell.pcSync ? this.cell.light : (this.cell.light / 3 * 2));
		this._lowblock = this.lowBlock;
		this.height = (int)this.cell.height;
		this.hasBridge = (this.cell._bridge > 0);
		this.blockLight = this._lightMod * this.lightLookUp[(int)this.light] + this._baseBrightness + ((this.roof || this.cell.isShadowed) ? this._shadowStrength : 0f);
		if (this.hasBridge)
		{
			if ((int)this.cell.bridgeHeight < this.currentHeight)
			{
				this.blockLight -= this.heightLightMod * (float)(this.currentHeight - (int)this.cell.bridgeHeight);
			}
			if (this.snowed && !this.cell.sourceBridge.ignoreSnow)
			{
				if (this.cell.IsBridgeWater)
				{
					this.sourceBridge = FLOOR.sourceIce;
					this.matBridge = this.cell.matBridge;
				}
				else
				{
					this.sourceBridge = FLOOR.sourceSnow;
					this.matBridge = MATERIAL.sourceSnow;
				}
			}
			else
			{
				this.sourceBridge = this.cell.sourceBridge;
				this.matBridge = this.cell.matBridge;
			}
		}
		else if (this.height < this.currentHeight)
		{
			this.blockLight -= this.heightLightMod * (float)(this.currentHeight - this.height);
		}
		this.liquidLv = (this.param.liquidLv = (flag ? 0 : (((this.cell.liquidLv + (int)this.cell._bridge != 0) ? this.cell.sourceBridge.tileType.LiquidLV : this.sourceFloor.tileType.LiquidLV) * 10)));
		if (this.cell.shore != 0 && this.liquidLv > 20)
		{
			this.liquidLv = (this.param.liquidLv = 20);
		}
		if (this.liquidLv > 99)
		{
			this.liquidLv = (this.param.liquidLv = 99);
		}
		CellEffect effect = this.cell.effect;
		if (effect != null && effect.IsFire)
		{
			this.blockLight += 0.2f;
			this.totalFire++;
		}
		if (this.blockLight > this.lightLimit)
		{
			this.blockLight = this.lightLimit;
		}
		this.blockLight += this.shadowModStrength * (float)this.cell.shadowMod * this._heightMod.x * this._shadowStrength;
		if (this.room != null)
		{
			this.room.lot.sync = true;
			if (this.room.lot.light < this.blockLight)
			{
				this.room.lot.light = this.blockLight;
			}
		}
		if (this.currentLot != null && this.currentLot.idRoofStyle != 0)
		{
			bool flag2;
			if (this.cell.IsRoomEdge)
			{
				Room room = this.cell.Front.room;
				if (((room != null) ? room.lot : null) != this.currentLot)
				{
					Room room2 = this.cell.Right.room;
					if (((room2 != null) ? room2.lot : null) != this.currentLot)
					{
						Room room3 = this.cell.FrontRight.room;
						flag2 = (((room3 != null) ? room3.lot : null) == this.currentLot);
						goto IL_625;
					}
				}
				flag2 = true;
			}
			else
			{
				flag2 = false;
			}
			IL_625:
			bool flag3 = flag2;
			if (!this.buildMode)
			{
				if ((this.room != null && this.room.lot == this.currentLot) || flag3)
				{
					this.blockLight += this.lotLight;
				}
				else
				{
					this.blockLight += (this.isSnowCovered ? -0.02f : this.lotLight2);
				}
			}
		}
		if (this.cell.outOfBounds && !this.cinemaMode)
		{
			this.blockLight -= 0.1f;
			if (this.fogBounds)
			{
				this.isSeen = false;
			}
		}
		this.floorLight = this.blockLight;
		this.param.color = (this.blockLight = (float)((int)(this.blockLight * 50f) * 262144 + (int)(((this.cell.lightR >= 64) ? 63 : this.cell.lightR) * 4096) + (int)(((this.cell.lightG >= 64) ? 63 : this.cell.lightG) * 64) + (int)((this.cell.lightB >= 64) ? 63 : this.cell.lightB)));
		if (this.snowed)
		{
			this.floorLight += this.snowLight;
			if (this.floorLight > this.lightLimit * this.snowLimit)
			{
				this.floorLight = this.lightLimit * this.snowLimit;
			}
			this.floorLight = (float)((int)(this.floorLight * 50f) * 262144 + (int)((float)((this.cell.lightR >= 50) ? 50 : this.cell.lightR) * this.snowColor) * 4096 + (int)((float)((this.cell.lightG >= 50) ? 50 : this.cell.lightG) * this.snowColor) * 64 + (int)((float)((this.cell.lightB >= 50) ? 50 : this.cell.lightB) * this.snowColor) + this.snowColorToken);
		}
		else if (this.isSnowCovered && !this.roof)
		{
			this.floorLight = (float)((int)(this.floorLight * 50f) * 262144 + (int)((float)((this.cell.lightR >= 50) ? 50 : this.cell.lightR) * this.snowColor2) * 4096 + (int)((float)((this.cell.lightG >= 50) ? 50 : this.cell.lightG) * this.snowColor2) * 64 + (int)((float)((this.cell.lightB >= 50) ? 50 : this.cell.lightB) * this.snowColor2) + this.snowColorToken);
		}
		else
		{
			this.floorLight = this.blockLight;
		}
		bool flag4 = this.room != null && this.sourceBlock.tileType.CastShadowSelf && !this.cell.hasDoor;
		bool flag5 = this.room != null && this.showRoof && this.room.lot.idRoofStyle != 0 && !this.room.data.atrium && !this.sourceBlock.tileType.Invisible;
		if (flag5 && this.cell.hasDoor && this.cell.IsLotEdge)
		{
			flag5 = false;
		}
		if (flag4 || !this.isSeen || flag5)
		{
			this.floorLight += -3145728f;
		}
		if (this.cell.isWatered && !this.snowed)
		{
			this.floorLight += -2359296f;
		}
		this.param.snow = false;
		this.param.x = (float)(this.cx + this.cz) * this.screen.tileAlign.x;
		this.param.y = (float)(this.cz - this.cx) * this.screen.tileAlign.y + (float)this.height * this._heightMod.y;
		this.param.z = 1000f + this.param.x * this.screen.tileWeight.x + this.param.y * this.screen.tileWeight.z + (float)this.height * this._heightMod.z;
		if (flag)
		{
			this.param.y += 0.01f * this.floatY;
		}
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
		if (this.screenHighlight != BaseTileMap.ScreenHighlight.None && this.screenHighlight == BaseTileMap.ScreenHighlight.SunMap && Map.sunMap.Contains(this.index))
		{
			this.screen.guide.passGuideFloor.Add(this.param.x, this.param.y, this.param.z - 10f, 11f, 0f);
		}
		if (this.cell._roofBlock != 0 && (this.isSeen || !EMono._zone.UseFog) && this.showRoof && !this.lowBlock)
		{
			SourceBlock.Row row = Cell.blockList[(int)this.cell._roofBlock];
			SourceMaterial.Row row2 = Cell.matList[(int)this.cell._roofBlockMat];
			this.tileType = row.tileType;
			this.param.mat = row2;
			this.param.dir = (int)(this.cell._roofBlockDir % 4);
			this.param.snow = this.isSnowCovered;
			this.orgX = this.param.x;
			this.orgY = this.param.y;
			this.orgZ = this.param.z;
			this.SetRoofHeight(this.param, this.cell, this.cx, this.cz, 0, (int)(this.cell._roofBlockDir / 4), this.tileType.IsWallOrFence ? this.param.dir : -1, false);
			switch (this.tileType.blockRenderMode)
			{
			case BlockRenderMode.FullBlock:
				this.param.color -= (float)((int)(this._shadowStrength * 50f) * 262144);
				this.param.tile = (float)row._tiles[this.param.dir % row._tiles.Length];
				this.param.matColor = (float)((row.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref row2.matColor, row.colorMod));
				row.renderData.Draw(this.param);
				break;
			case BlockRenderMode.WallOrFence:
			{
				this._lowblock = true;
				int dir = this.param.dir;
				if (dir == 0 || dir == 2)
				{
					this.param.dir = 0;
					this._sourceBlock = row;
					this.tileType = this._sourceBlock.tileType;
					if (this._sourceBlock.useAltColor)
					{
						this.param.matColor = (float)((this._sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.param.mat.altColor, this._sourceBlock.colorMod));
					}
					else
					{
						this.param.matColor = (float)((this._sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.param.mat.matColor, this._sourceBlock.colorMod));
					}
					this.param.tile = (float)(this.tile = this._sourceBlock._tiles[0] + ((this._lowblock && !this.tileType.IsFence) ? 32 : 0));
					this._sourceBlock.renderData.Draw(this.param);
					this.param.z -= 0.01f;
				}
				if (dir == 1 || dir == 2)
				{
					this.param.dir = 1;
					this._sourceBlock = row;
					this.tileType = this._sourceBlock.tileType;
					if (this._sourceBlock.useAltColor)
					{
						this.param.matColor = (float)((this._sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.param.mat.altColor, this._sourceBlock.colorMod));
					}
					else
					{
						this.param.matColor = (float)((this._sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.param.mat.matColor, this._sourceBlock.colorMod));
					}
					this.param.tile = (float)(this.tile = -this._sourceBlock._tiles[0] + ((this._lowblock && !this.tileType.IsFence) ? -32 : 0));
					this._sourceBlock.renderData.Draw(this.param);
				}
				break;
			}
			case BlockRenderMode.Pillar:
			{
				RenderData renderData = row.renderData;
				this.param.tile = (float)row._tiles[this.param.dir % row._tiles.Length];
				this.param.matColor = (float)((row.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref row2.matColor, row.colorMod));
				renderData.Draw(this.param);
				this.param.tile = (float)renderData.idShadow;
				SourcePref shadowPref = renderData.shadowPref;
				int shadow = shadowPref.shadow;
				this.passShadow.AddShadow(this.param.x + renderData.offsetShadow.x, this.param.y + renderData.offsetShadow.y, this.param.z + renderData.offsetShadow.z, ShadowData.Instance.items[shadow], shadowPref, 0, this.param.snow);
				break;
			}
			case BlockRenderMode.HalfBlock:
				this._sourceBlock = ((row.id == 5) ? EMono.sources.blocks.rows[row2.defBlock] : row);
				this.param.tile = (float)this._sourceBlock._tiles[0];
				this.param.matColor = (float)((this._sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref row2.matColor, this._sourceBlock.colorMod));
				this.param.tile2 = this._sourceBlock.sourceAutoFloor._tiles[0];
				this.param.halfBlockColor = ((this._sourceBlock.sourceAutoFloor.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref row2.matColor, this._sourceBlock.sourceAutoFloor.colorMod));
				row.renderData.Draw(this.param);
				break;
			default:
				this.param.tile = (float)row._tiles[this.param.dir % row._tiles.Length];
				this.param.matColor = (float)((row.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref row2.matColor, row.colorMod));
				row.renderData.Draw(this.param);
				break;
			}
			this.param.x = this.orgX;
			this.param.y = this.orgY;
			this.param.z = this.orgZ;
			this.param.color = this.blockLight;
		}
		this.fogged = false;
		bool flag6 = this.cell.isSurrounded && this.innerMode != BaseTileMap.InnerMode.None && this.sourceBlock.tileType.IsFullBlock;
		if (!this.isSeen || flag6)
		{
			bool isRoomEdge = this.cell.IsRoomEdge;
			this.orgY = this.param.y;
			this.orgZ = this.param.z;
			this.param.color = (float)((int)(50f * (this._baseBrightness + this.fogBrightness)) * 262144);
			this.param.matColor = 104025f;
			if (this.hasBridge)
			{
				this.param.y = (float)(this.cz - this.cx) * this.screen.tileAlign.y + (float)this.cell.bridgeHeight * this._heightMod.y + this.ugFixBridgeBottom.x;
				this.param.z = 1000f + this.param.x * this.screen.tileWeight.x + this.param.y * this.screen.tileWeight.z + (float)this.cell.bridgeHeight * this._heightMod.z;
			}
			bool flag7 = (!this.isSeen && EMono._zone.UseFog) || flag6;
			if (flag7)
			{
				this.param.tile = 7f;
				this.rendererFogFloorSolid.Draw(this.param);
				this.param.tile = 0f;
				this.rendererFov2.Draw(this.param);
			}
			else if (this.cell.HasFloodBlock && isRoomEdge)
			{
				this.param.tile = 9f;
				this.rendererFogRoomWallSolid.Draw(this.param);
			}
			else
			{
				this.param.tile = 8f;
				this.rendererFogRoomSolid.Draw(this.param);
			}
			if ((this.cell.isSlopeEdge || this.hasBridge) && (flag7 || !isRoomEdge))
			{
				float num = (float)this.cell.TopHeight * this._heightMod.y;
				this.param.tile = 0f;
				int num2 = 0;
				while ((float)num2 < num / this.heightBlockSize)
				{
					this.param.y += this.ugFix.y;
					this.param.z += this.ugFix.z + this.slopeFixZ * (float)num2;
					if (flag7)
					{
						this.rendererFogBlockSolid.Draw(this.param);
					}
					else
					{
						this.rendererFogRoomBlockSolid.Draw(this.param);
					}
					num2++;
				}
			}
			this.param.y = this.orgY;
			this.param.z = this.orgZ;
			this.param.color = this.blockLight;
			if (flag7)
			{
				if (this.detail == null || !EMono.pc.hasTelepathy)
				{
					return;
				}
				goto IL_7A49;
			}
			else if (!isRoomEdge)
			{
				if (this.detail != null && EMono.pc.hasTelepathy)
				{
					goto IL_7A49;
				}
				if (this.noRoofMode || this.detail == null)
				{
					return;
				}
				this.fogged = true;
				goto IL_7A49;
			}
		}
		if (this.cell.isSlopeEdge)
		{
			float num3 = (float)this.height * this._heightMod.y;
			this.orgY = this.param.y;
			this.orgZ = this.param.z;
			this.param.dir = this.cell.blockDir;
			if (this.snowed)
			{
				this.param.color = this.floorLight;
			}
			SourceBlock.Row defBlock;
			if (this.sourceBlock.tileType.IsFullBlock)
			{
				defBlock = this.sourceBlock;
				this.param.mat = this.matBlock;
				this.param.tile = (float)this.sourceBlock._tiles[this.cell.blockDir % this.sourceBlock._tiles.Length];
				this.param.matColor = (float)((this.sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.matBlock.matColor, this.sourceBlock.colorMod));
			}
			else
			{
				defBlock = this.sourceFloor._defBlock;
				this.param.mat = this.matFloor;
				this.param.tile = (float)defBlock._tiles[this.cell.blockDir % defBlock._tiles.Length];
				if (defBlock.id != 1)
				{
					this.param.matColor = (float)((this.sourceFloor.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.matFloor.matColor, this.sourceFloor.colorMod));
				}
				else
				{
					this.param.matColor = 104025f;
				}
			}
			int num4 = 0;
			while ((float)num4 < num3 / this.heightBlockSize)
			{
				this.param.y += this.ugFix.y;
				this.param.z += this.ugFix.z + this.slopeFixZ * (float)num4;
				defBlock.renderData.Draw(this.param);
				if (this.cell.pcSync && EMono.player.lightPower > 0f)
				{
					float num5 = this.param.tile;
					this.param.tile = 0f;
					this.rendererFov.Draw(this.param);
					this.param.tile = num5;
				}
				num4++;
			}
			this.param.y = this.orgY;
			this.param.z = this.orgZ;
		}
		this.param.color = this.floorLight;
		if (!isWater && (this.cell.Front.sourceFloor.tileType.IsWater || this.cell.Right.sourceFloor.tileType.IsWater) && this.cell.sourceBlock.tileType.RenderWaterBlock && !flag)
		{
			this.orgY = this.param.y;
			this.orgZ = this.param.z;
			int num6;
			if (this.sourceBlock.tileType.IsFullBlock)
			{
				SourceBlock.Row defBlock2 = this.sourceBlock;
				num6 = this.sourceBlock._tiles[this.cell.blockDir % this.sourceBlock._tiles.Length];
			}
			else
			{
				SourceBlock.Row defBlock2 = this.sourceFloor._defBlock;
				num6 = defBlock2._tiles[this.cell.blockDir % defBlock2._tiles.Length];
			}
			if ((this.cell.Front.shore / 12 & 1) == 0 && this.cell.Front.sourceFloor.tileType.IsWater && (int)this.cell.Front.height <= this.height && this.cell.Front.sourceBlock.tileType.RenderWaterBlock)
			{
				this.param.y = (float)(this.cz - this.cx) * this.screen.tileAlign.y - (this.cell.Front.sourceFloor.tileType.IsDeepWater ? 0.6f : 0.4f) + (float)this.cell.Front.height * this._heightMod.y;
				this.param.z = 1000f + this.param.x * this.screen.tileWeight.x + this.param.y * this.screen.tileWeight.z;
				this.param.tile = (float)(num6 + (this.cell.Front.sourceFloor.tileType.IsDeepWater ? 0 : 3000000));
				this.rendererWaterBlock.Draw(this.param);
			}
			if ((this.cell.Right.shore / 12 & 8) == 0 && this.cell.Right.sourceFloor.tileType.IsWater && (int)this.cell.Right.height <= this.height && this.cell.Right.sourceBlock.tileType.RenderWaterBlock)
			{
				this.param.y = (float)(this.cz - this.cx) * this.screen.tileAlign.y - (this.cell.Right.sourceFloor.tileType.IsDeepWater ? 0.6f : 0.4f) + (float)this.cell.Right.height * this._heightMod.y;
				this.param.z = 1000f + this.param.x * this.screen.tileWeight.x + this.param.y * this.screen.tileWeight.z;
				this.param.tile = (float)(num6 + (this.cell.Right.sourceFloor.tileType.IsDeepWater ? 0 : 3000000));
				this.rendererWaterBlock.Draw(this.param);
			}
			this.param.y = this.orgY;
			this.param.z = this.orgZ;
		}
		if (this.showBorder && !this.cell.outOfBounds)
		{
			this.param.matColor = 104025f;
			if (this.cx == EMono._map.bounds.x)
			{
				this.renderBorder.Draw(this.param, 12 + (EMono.world.date.IsNight ? 4 : 0));
			}
			else if (this.cx == EMono._map.bounds.maxX)
			{
				this.renderBorder.Draw(this.param, 13 + (EMono.world.date.IsNight ? 4 : 0));
			}
			if (this.cz == EMono._map.bounds.z)
			{
				this.renderBorder.Draw(this.param, 14 + (EMono.world.date.IsNight ? 4 : 0));
			}
			else if (this.cz == EMono._map.bounds.maxZ)
			{
				this.renderBorder.Draw(this.param, 15 + (EMono.world.date.IsNight ? 4 : 0));
			}
		}
		if (this.cell.isSkyFloor || (this.detail != null && this.detail.anime != null && this.detail.anime.drawBlock))
		{
			this.orgY = this.param.y;
			this.orgZ = this.param.z;
			SourceBlock.Row defBlock3 = this.sourceFloor._defBlock;
			this.param.mat = this.matFloor;
			this.param.tile = (float)defBlock3._tiles[this.cell.blockDir % defBlock3._tiles.Length];
			if (defBlock3.id != 1)
			{
				this.param.matColor = (float)((this.sourceFloor.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.matFloor.matColor, this.sourceFloor.colorMod));
			}
			else
			{
				this.param.matColor = 104025f;
			}
			for (int i = 0; i < (this.cell.isSkyFloor ? EMono._map.config.skyBlockHeight : 1); i++)
			{
				this.param.y += this.ugFix.y;
				this.param.z += this.ugFix.z + this.slopeFixZ * (float)i;
				defBlock3.renderData.Draw(this.param);
			}
			this.param.y = this.orgY;
			this.param.z = this.orgZ;
		}
		if (!this.sourceFloor.tileType.IsSkipFloor)
		{
			if ((this.hasBridge && this.sourceBridge.tileType.CastShadowSelf) || this.cell.castFloorShadow)
			{
				this.floorLight2 = this._lightMod * this.light * 0.2f + this._baseBrightness + this._shadowStrength * this.floorShadowStrength * (isWater ? 0.7f : (this.hasBridge ? 1f : (0.6f * (1f - this.nightRatio))));
				if (this.snowed)
				{
					this.floorLight2 = (float)((int)((double)this.floorLight2 * 0.85 * 50.0) * 262144 + this.snowColorToken);
				}
				else
				{
					this.floorLight2 = (float)((int)(this.floorLight2 * 50f) * 262144 + (int)(((this.cell.lightR >= 64) ? 63 : this.cell.lightR) * 4096) + (int)(((this.cell.lightG >= 64) ? 63 : this.cell.lightG) * 64) + (int)((this.cell.lightB >= 64) ? 63 : this.cell.lightB));
				}
				this.param.color = this.floorLight2;
				if (this.cell.lotShade)
				{
					this.floorLight = this.floorLight2;
				}
			}
			this.floorMatColor = ((this.sourceFloor.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.matFloor.matColor, this.sourceFloor.colorMod));
			if (isWater && flag)
			{
				this.param.y -= 0.01f * this.floatY;
			}
			if (!this.sourceBlock.tileType.IsSkipFloor || this.sourceBlock.transparent || this.hasBridge || this.cell.hasDoor || this.cell.skipRender)
			{
				this.param.mat = this.matFloor;
				this.param.tile = (float)this.sourceFloor._tiles[this.floorDir % this.sourceFloor._tiles.Length];
				this.param.matColor = (float)this.floorMatColor;
				this.param.snow = this.snowed;
				if (this.cell.isDeck)
				{
					this.param.z += 1f;
					if (this.sourceFloor.renderData.subData)
					{
						this.sourceFloor.renderData.subData.Draw(this.param);
					}
					this.sourceFloor.renderData.Draw(this.param);
					this.param.z -= 1f;
				}
				else
				{
					if (this.sourceFloor.renderData.subData)
					{
						this.sourceFloor.renderData.subData.Draw(this.param);
					}
					this.sourceFloor.renderData.Draw(this.param);
				}
				int num7 = 0;
				if (this.isSnowCovered && this.sourceFloor == FLOOR.sourceSnow && !this.cell.hasDoor)
				{
					if (!this.cell.Right.IsSnowTile && this.cell.Right.topHeight == this.cell.topHeight)
					{
						num7++;
					}
					if (!this.cell.Front.IsSnowTile && this.cell.Front.topHeight == this.cell.topHeight)
					{
						num7 += 2;
					}
					if (num7 != 0)
					{
						this.param.tile = (float)(448 + num7 + 12);
						this.param.z -= 0.1f;
						this.sourceFloor.renderData.Draw(this.param);
						this.param.z += 0.1f;
					}
				}
				if (this.cell.shadow != 0 && !this.hasBridge && !this.cell.skipRender)
				{
					if (this.snowed)
					{
						if (this.sourceFloor == FLOOR.sourceSnow)
						{
							this.param.tile = (float)(448 + (int)this.cell.shadow + 8 + (this.cell.HasFence ? 4 : 0));
							this.param.z -= 0.01f;
							this.sourceFloor.renderData.Draw(this.param);
						}
					}
					else
					{
						this.pass = this.passEdge;
						this.batch = this.pass.batches[this.pass.batchIdx];
						this.batch.matrices[this.pass.idx].m03 = this.param.x + this.ambientShadowFix[(int)this.cell.shadow].x;
						this.batch.matrices[this.pass.idx].m13 = this.param.y + this.ambientShadowFix[(int)this.cell.shadow].y;
						this.batch.matrices[this.pass.idx].m23 = this.param.z + this.ambientShadowFix[(int)this.cell.shadow].z;
						this.batch.tiles[this.pass.idx] = (float)(448 + (int)this.cell.shadow);
						this.batch.colors[this.pass.idx] = this.param.color;
						this.batch.matColors[this.pass.idx] = 104025f;
						this.pass.idx++;
						if (this.pass.idx == this.pass.batchSize)
						{
							this.pass.NextBatch();
						}
					}
					if (!this.sourceFloor.ignoreTransition && !this.snowed)
					{
						Cell cell = this.cell.Back;
						if (cell.sourceBlock.transition[0] != -1 && cell.isSeen && !cell.hasDoor)
						{
							this.pass = this.passFloor;
							this.batch = this.pass.batches[this.pass.batchIdx];
							this.batch.matrices[this.pass.idx].m03 = this.param.x + this.transitionFix[0].x;
							this.batch.matrices[this.pass.idx].m13 = this.param.y + this.transitionFix[0].y;
							this.batch.matrices[this.pass.idx].m23 = this.param.z + this.transitionFix[0].z;
							this.batch.tiles[this.pass.idx] = (float)(480 + cell.sourceBlock.transition[0] + (int)Rand.bytes[this.index % Rand.MaxBytes] % cell.sourceBlock.transition[1]);
							this.batch.colors[this.pass.idx] = this.param.color;
							this.batch.matColors[this.pass.idx] = (float)BaseTileMap.GetColorInt(ref cell.matBlock.matColor, cell.sourceBlock.colorMod);
							this.pass.idx++;
							if (this.pass.idx == this.pass.batchSize)
							{
								this.pass.NextBatch();
							}
						}
						cell = this.cell.Left;
						if (cell.sourceBlock.transition[0] != -1 && cell.isSeen && !cell.hasDoor)
						{
							this.pass = this.passFloor;
							this.batch = this.pass.batches[this.pass.batchIdx];
							this.batch.matrices[this.pass.idx].m03 = this.param.x + this.transitionFix[1].x;
							this.batch.matrices[this.pass.idx].m13 = this.param.y + this.transitionFix[1].y;
							this.batch.matrices[this.pass.idx].m23 = this.param.z + this.transitionFix[1].z;
							this.batch.tiles[this.pass.idx] = (float)(512 + cell.sourceBlock.transition[0] + (int)Rand.bytes[this.index % Rand.MaxBytes] % cell.sourceBlock.transition[1]);
							this.batch.colors[this.pass.idx] = this.param.color;
							this.batch.matColors[this.pass.idx] = (float)BaseTileMap.GetColorInt(ref cell.matBlock.matColor, cell.sourceBlock.colorMod);
							this.pass.idx++;
							if (this.pass.idx == this.pass.batchSize)
							{
								this.pass.NextBatch();
							}
						}
					}
				}
				if (this.cell.autotile != 0 && this.sourceFloor.autotile != 0 && (!this.hasBridge || this.cell.bridgeHeight - this.cell.height > 3) && !this.cell.skipRender && num7 == 0)
				{
					this.pass = (isWater ? this.passAutoTileWater : this.passAutoTile);
					this.batch = this.pass.batches[this.pass.batchIdx];
					this.batch.matrices[this.pass.idx].m03 = this.param.x;
					this.batch.matrices[this.pass.idx].m13 = this.param.y;
					this.batch.matrices[this.pass.idx].m23 = this.param.z + ((this.hasBridge || this.cell._block != 0) ? 0.8f : 0f);
					this.batch.tiles[this.pass.idx] = (float)((26 + this.sourceFloor.autotile / 2) * 32 + this.sourceFloor.autotile % 2 * 16 + (int)this.cell.autotile);
					this.batch.colors[this.pass.idx] = this.param.color + (float)((int)(this.sourceFloor.autotileBrightness * 100f) * 262144);
					this.batch.matColors[this.pass.idx] = this.param.matColor;
					this.pass.idx++;
					if (this.pass.idx == this.pass.batchSize)
					{
						this.pass.NextBatch();
					}
				}
			}
			if (isWater)
			{
				int num8 = 12;
				int num9 = (int)this.cell.shore / num8;
				int num10 = (int)this.cell.shore % num8;
				bool isShoreSand = this.cell.isShoreSand;
				if (this.cell.shore != 0)
				{
					Cell cell2 = ((num9 & 1) != 0) ? this.cell.Back : (((num9 & 2) != 0) ? this.cell.Right : (((num9 & 4) != 0) ? this.cell.Front : this.cell.Left));
					if (isShoreSand && !cell2.sourceFloor.isBeach)
					{
						cell2 = (((num9 & 8) != 0) ? this.cell.Left : (((num9 & 4) != 0) ? this.cell.Front : (((num9 & 2) != 0) ? this.cell.Right : this.cell.Back)));
					}
					if (!cell2.IsSnowTile)
					{
						this.param.matColor = (float)BaseTileMap.GetColorInt(ref cell2.matFloor.matColor, cell2.sourceFloor.colorMod);
						if (isShoreSand)
						{
							this.pass = this.passShore;
							this.batch = this.pass.batches[this.pass.batchIdx];
							this.batch.matrices[this.pass.idx].m03 = this.param.x;
							this.batch.matrices[this.pass.idx].m13 = this.param.y;
							this.batch.matrices[this.pass.idx].m23 = this.param.z;
							this.batch.tiles[this.pass.idx] = (float)(768 + (int)this.cell.shore / num8);
							this.batch.colors[this.pass.idx] = this.param.color;
							this.batch.matColors[this.pass.idx] = this.param.matColor;
							this.pass.idx++;
							if (this.pass.idx == this.pass.batchSize)
							{
								this.pass.NextBatch();
							}
							num10 = 2;
						}
						else
						{
							num10 = cell2.sourceFloor.edge;
						}
						this.param.tile = (float)((24 + num10 / 2) * 32 + num10 % 2 * 16 + num9);
						this.rendererShore.Draw(this.param);
					}
				}
				if (this.cell.Back.isShoreSand && ((int)this.cell.Back.shore / num8 & 8) != 0 && this.cell.Left.isShoreSand && ((int)this.cell.Left.shore / num8 & 1) != 0)
				{
					this.param.tile = 785f;
					this.param.matColor = (float)BaseTileMap.GetColorInt(ref this.cell.BackLeft.matFloor.matColor, this.cell.BackLeft.sourceFloor.colorMod);
					this.passShore.Add(this.param);
					CS$<>8__locals1.<DrawTile>g__Draw|0(60);
				}
				if (this.cell.Back.isShoreSand && ((int)this.cell.Back.shore / num8 & 2) != 0 && this.cell.Right.isShoreSand && ((int)this.cell.Right.shore / num8 & 1) != 0)
				{
					this.param.tile = 786f;
					this.param.matColor = (float)BaseTileMap.GetColorInt(ref this.cell.BackRight.matFloor.matColor, this.cell.BackRight.sourceFloor.colorMod);
					this.passShore.Add(this.param);
					CS$<>8__locals1.<DrawTile>g__Draw|0(56);
				}
				if (this.cell.Front.isShoreSand && ((int)this.cell.Front.shore / num8 & 2) != 0 && this.cell.Right.isShoreSand && ((int)this.cell.Right.shore / num8 & 4) != 0)
				{
					this.param.tile = 787f;
					this.param.matColor = (float)BaseTileMap.GetColorInt(ref this.cell.FrontRight.matFloor.matColor, this.cell.FrontRight.sourceFloor.colorMod);
					this.passShore.Add(this.param);
					CS$<>8__locals1.<DrawTile>g__Draw|0(48);
				}
				if (this.cell.Front.isShoreSand && ((int)this.cell.Front.shore / num8 & 8) != 0 && this.cell.Left.isShoreSand && ((int)this.cell.Left.shore / num8 & 4) != 0)
				{
					this.param.tile = 788f;
					this.param.matColor = (float)BaseTileMap.GetColorInt(ref this.cell.FrontLeft.matFloor.matColor, this.cell.FrontLeft.sourceFloor.colorMod);
					this.passShore.Add(this.param);
					CS$<>8__locals1.<DrawTile>g__Draw|0(52);
				}
				if (this.cell._bridge != 0 && this.cell.isBridgeEdge && this.cell.bridgePillar != 255)
				{
					this.pass = this.passEdge;
					this.batch = this.pass.batches[this.pass.batchIdx];
					this.batch.matrices[this.pass.idx].m03 = this.param.x + this.waterEdgeBridgeFix.x;
					this.batch.matrices[this.pass.idx].m13 = this.param.y + this.waterEdgeBridgeFix.y;
					this.batch.matrices[this.pass.idx].m23 = this.param.z + this.waterEdgeBridgeFix.z;
					this.batch.tiles[this.pass.idx] = (float)(616 + this.waterAnimeIndex % 4);
					this.batch.colors[this.pass.idx] = this.param.color;
					this.batch.matColors[this.pass.idx] = 104025f;
					this.pass.idx++;
					if (this.pass.idx == this.pass.batchSize)
					{
						this.pass.NextBatch();
					}
				}
				bool flag8 = false;
				if (isShoreSand)
				{
					if ((num9 & 1) != 0)
					{
						if ((num9 & 8) != 0)
						{
							CS$<>8__locals1.<DrawTile>g__Draw|0(16);
							flag8 = true;
						}
						if ((num9 & 2) != 0)
						{
							CS$<>8__locals1.<DrawTile>g__Draw|0(20);
							flag8 = true;
						}
					}
					if ((num9 & 4) != 0)
					{
						if ((num9 & 8) != 0)
						{
							CS$<>8__locals1.<DrawTile>g__Draw|0(24);
							flag8 = true;
						}
						if ((num9 & 2) != 0)
						{
							CS$<>8__locals1.<DrawTile>g__Draw|0(28);
							flag8 = true;
						}
					}
					if (!flag8)
					{
						if (!this.cell.Front.sourceFloor.tileType.IsWater && !this.cell.Front.isDeck)
						{
							CS$<>8__locals1.<DrawTile>g__Draw|0(8);
						}
						if (!this.cell.Right.sourceFloor.tileType.IsWater && !this.cell.Right.isDeck)
						{
							CS$<>8__locals1.<DrawTile>g__Draw|0(12);
						}
					}
				}
				if (!flag8)
				{
					if (!this.cell.Back.sourceFloor.tileType.IsWater && !this.cell.Back.isDeck)
					{
						this.pass = this.passEdge;
						this.batch = this.pass.batches[this.pass.batchIdx];
						this.batch.tiles[this.pass.idx] = (float)(608 + this.waterAnimeIndex % 4);
						this.batch.matColors[this.pass.idx] = 104025f;
						if (((int)this.cell.shore / num8 & 1) != 0)
						{
							if (isShoreSand)
							{
								this.param.matColor = (float)BaseTileMap.GetColorInt(ref this.cell.Back.matFloor.matColor, this.cell.Back.sourceFloor.colorMod);
								this.batch.matrices[this.pass.idx].m03 = this.param.x + this.waterEdgeFixShoreSand.x;
								this.batch.matrices[this.pass.idx].m13 = this.param.y + this.waterEdgeFixShoreSand.y;
								this.batch.matrices[this.pass.idx].m23 = this.param.z + this.waterEdgeFixShoreSand.z;
								this.batch.tiles[this.pass.idx] = (float)(640 + this.seaAnimeIndexes[this.waterAnimeIndex % this.seaAnimeIndexes.Length]);
								this.batch.matColors[this.pass.idx] = this.param.matColor;
							}
							else
							{
								this.batch.matrices[this.pass.idx].m03 = this.param.x;
								this.batch.matrices[this.pass.idx].m13 = this.param.y;
								this.batch.matrices[this.pass.idx].m23 = this.param.z + this.waterEdgeFixShore.z;
							}
						}
						else
						{
							this.batch.matrices[this.pass.idx].m03 = this.param.x;
							this.batch.matrices[this.pass.idx].m13 = this.param.y;
							this.batch.matrices[this.pass.idx].m23 = this.param.z + this.waterEdgeFix.z;
							this.batch.tiles[this.pass.idx] += 12f;
						}
						this.batch.colors[this.pass.idx] = this.param.color;
						this.pass.idx++;
						if (this.pass.idx == this.pass.batchSize)
						{
							this.pass.NextBatch();
						}
					}
					if (!this.cell.Left.sourceFloor.tileType.IsWater && !this.cell.Left.isDeck)
					{
						this.pass = this.passEdge;
						this.batch = this.pass.batches[this.pass.batchIdx];
						this.batch.tiles[this.pass.idx] = (float)(612 + this.waterAnimeIndex % 4);
						this.batch.matColors[this.pass.idx] = 104025f;
						if (((int)this.cell.shore / num8 & 8) != 0)
						{
							if (isShoreSand)
							{
								this.param.matColor = (float)BaseTileMap.GetColorInt(ref this.cell.Left.matFloor.matColor, this.cell.Left.sourceFloor.colorMod);
								this.batch.matrices[this.pass.idx].m03 = this.param.x + this.waterEdgeFixShoreSand.x;
								this.batch.matrices[this.pass.idx].m13 = this.param.y + this.waterEdgeFixShoreSand.y;
								this.batch.matrices[this.pass.idx].m23 = this.param.z + this.waterEdgeFixShoreSand.z;
								this.batch.tiles[this.pass.idx] = (float)(644 + this.seaAnimeIndexes[this.waterAnimeIndex % this.seaAnimeIndexes.Length]);
								this.batch.matColors[this.pass.idx] = this.param.matColor;
							}
							else
							{
								this.batch.matrices[this.pass.idx].m03 = this.param.x;
								this.batch.matrices[this.pass.idx].m13 = this.param.y;
								this.batch.matrices[this.pass.idx].m23 = this.param.z + this.waterEdgeFixShore.z;
							}
						}
						else
						{
							this.batch.matrices[this.pass.idx].m03 = this.param.x;
							this.batch.matrices[this.pass.idx].m13 = this.param.y;
							this.batch.matrices[this.pass.idx].m23 = this.param.z + this.waterEdgeFix.z;
							this.batch.tiles[this.pass.idx] += 12f;
						}
						this.batch.colors[this.pass.idx] = this.param.color;
						this.pass.idx++;
						if (this.pass.idx == this.pass.batchSize)
						{
							this.pass.NextBatch();
						}
					}
				}
				if (flag)
				{
					this.param.y += 0.01f * this.floatY;
				}
			}
			if (flag)
			{
				this.param.z -= 1f;
			}
		}
		if (this.cell.skipRender)
		{
			if (this.cell.pcSync)
			{
				this.param.tile = 0f;
				this.rendererFov.Draw(this.param);
			}
			return;
		}
		if (this.hasBridge)
		{
			this.param.y = (float)(this.cz - this.cx) * this.screen.tileAlign.y + (float)this.cell.bridgeHeight * this._heightMod.y;
			this.param.z = 1000f + this.param.x * this.screen.tileWeight.x + this.param.y * this.screen.tileWeight.z + (float)this.cell.bridgeHeight * this._heightMod.z;
			if (flag)
			{
				this.param.y += 0.01f * this.floatY;
			}
			this.param.color = this.floorLight;
			this.param.mat = this.matBridge;
			this.floorMatColor = ((this.sourceBridge.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.matBridge.matColor, this.sourceBridge.colorMod));
			this.param.dir = this.cell.floorDir;
			this.param.tile = (float)this.sourceBridge._tiles[this.cell.floorDir % this.sourceBridge._tiles.Length];
			this.param.matColor = (float)this.floorMatColor;
			this.sourceBridge.renderData.Draw(this.param);
			if (this.cell.autotileBridge != 0 && this.sourceBridge.autotile != 0)
			{
				this.pass = this.passAutoTile;
				this.batch = this.pass.batches[this.pass.batchIdx];
				this.batch.matrices[this.pass.idx].m03 = this.param.x;
				this.batch.matrices[this.pass.idx].m13 = this.param.y;
				this.batch.matrices[this.pass.idx].m23 = this.param.z + ((this.cell._block != 0) ? 0.8f : 0f);
				this.batch.tiles[this.pass.idx] = (float)((26 + this.sourceBridge.autotile / 2) * 32 + this.sourceBridge.autotile % 2 * 16 + (int)this.cell.autotileBridge);
				this.batch.colors[this.pass.idx] = this.param.color + (float)((int)(this.sourceBridge.autotileBrightness * 100f) * 262144);
				this.batch.matColors[this.pass.idx] = this.param.matColor;
				this.pass.idx++;
				if (this.pass.idx == this.pass.batchSize)
				{
					this.pass.NextBatch();
				}
			}
			if (this.cell.shadow != 0)
			{
				if (this.sourceBridge == FLOOR.sourceSnow)
				{
					this.param.tile = (float)(448 + (int)this.cell.shadow + 8 + (this.cell.HasFence ? 4 : 0));
					this.param.z -= 0.01f;
					this.sourceBridge.renderData.Draw(this.param);
				}
				else
				{
					this.pass = this.passEdge;
					this.batch = this.pass.batches[this.pass.batchIdx];
					this.batch.matrices[this.pass.idx].m03 = this.param.x + this.ambientShadowFix[(int)this.cell.shadow].x;
					this.batch.matrices[this.pass.idx].m13 = this.param.y + this.ambientShadowFix[(int)this.cell.shadow].y;
					this.batch.matrices[this.pass.idx].m23 = this.param.z + this.ambientShadowFix[(int)this.cell.shadow].z;
					this.batch.tiles[this.pass.idx] = (float)(448 + (int)this.cell.shadow);
					this.batch.colors[this.pass.idx] = this.blockLight;
					this.batch.matColors[this.pass.idx] = 104025f;
					this.pass.idx++;
					if (this.pass.idx == this.pass.batchSize)
					{
						this.pass.NextBatch();
					}
				}
			}
			if (this.cell.isBridgeEdge && this.cell.bridgeHeight - this.cell.height >= 3 && this.cell.bridgePillar != 255 && !this.noSlopMode)
			{
				this.orgY = this.param.y;
				this.orgZ = this.param.z;
				this.param.y += this.bridgeFix.y;
				this.param.z += this.bridgeFix.z;
				this.param.dir = 0;
				SourceBlock.Row row3 = this.sourceBridge._bridgeBlock;
				float num11 = (float)(this.cell.bridgeHeight - this.cell.height) * this._heightMod.y;
				if (this.cell.sourceFloor.tileType == TileType.Sky)
				{
					num11 += (float)EMono._map.config.skyBlockHeight;
				}
				int num12 = (int)(num11 / this.heightBlockSize) + 2;
				if (this.cell.bridgePillar != 0)
				{
					row3 = EMono.sources.blocks.rows[(int)this.cell.bridgePillar];
					this.param.tile = (float)(row3._tiles[0] + ((num12 == 2) ? 32 : 0));
					this.param.mat = ((this.sourceBridge.DefaultMaterial == row3.DefaultMaterial) ? this.sourceBridge.DefaultMaterial : row3.DefaultMaterial);
					this.param.matColor = (float)((row3.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.param.mat.matColor, row3.colorMod));
				}
				else
				{
					this.param.mat = this.matBlock;
					this.param.tile = (float)(row3._tiles[0] + 32);
					this.param.matColor = (float)((row3.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.matBridge.matColor, row3.colorMod));
				}
				this.param.y += this.ugFixBridgeTop.y;
				this.param.z += this.ugFixBridgeTop.z;
				for (int j = 0; j < num12; j++)
				{
					if (j == num12 - 1)
					{
						this.param.y = (float)(this.cz - this.cx) * this.screen.tileAlign.y + (float)this.height * this._heightMod.y + this.ugFixBridgeBottom.y;
						this.param.z = 1000f + this.param.x * this.screen.tileWeight.x + this.param.y * this.screen.tileWeight.z + (float)this.height * this._heightMod.z + this.ugFixBridgeBottom.z;
					}
					else
					{
						this.param.y += this.ugFixBridge.y;
						this.param.z += this.ugFixBridge.z;
					}
					row3.renderData.Draw(this.param);
				}
				this.param.y = this.orgY;
				this.param.z = this.orgZ;
			}
		}
		if (!this.buildMode && this.cell.highlight != 0)
		{
			if (this.cell._block != 0)
			{
				this.screen.guide.DrawWall(this.cell.GetPoint(), EMono.Colors.blockColors.MapHighlight, true, 0f);
			}
			else
			{
				this.passGuideFloor.Add(this.cell.GetPoint(), (float)this.cell.highlight, 0f);
			}
		}
		this.param.color = this.blockLight;
		if (this.isSnowCovered && (this.sourceBlock.id != 0 || this.cell.hasDoor))
		{
			if (this.snowed || this.cell.isClearSnow)
			{
				if (this.cell.Front.HasRoof || this.cell.Right.HasRoof)
				{
					this.snowed = false;
				}
			}
			else if ((!this.cell.Front.HasRoof && !this.cell.Front.HasBlock) || (!this.cell.Right.HasRoof && !this.cell.Right.HasBlock))
			{
				this.snowed = true;
			}
		}
		int num13 = 0;
		if (this.sourceBlock.id != 0)
		{
			this.tileType = this.sourceBlock.tileType;
			this.roomHeight = 0f;
			int blockDir = this.cell.blockDir;
			bool flag9 = false;
			BaseTileMap.WallClipMode wallClipMode = this.wallClipMode;
			if (wallClipMode != BaseTileMap.WallClipMode.ByRoom)
			{
				if (wallClipMode == BaseTileMap.WallClipMode.ByLot)
				{
					if (this.defaultBlockHeight > 0f || this.isIndoor)
					{
						this._lowblock = (this.cx != 0 && this.cz != this.Size - 1 && ((!this.cell.Back.HasBlock && !this.cell.Back.isWallEdge) || (!this.cell.Left.HasBlock && !this.cell.Left.isWallEdge) || !this.cell.Back.Left.HasBlock));
						if (!this._lowblock)
						{
							this.roomHeight = this.defaultBlockHeight * EMono.setting.render.roomHeightMod;
							this.maxHeight = (float)(this.cz - this.cx) * this.screen.tileAlign.y + (float)this.cell.TopHeight * this._heightMod.y;
						}
					}
					else
					{
						if (this.showFullWall)
						{
							this._lowblock = (this.room != null);
							if (this._lowblock)
							{
								if (this.cell.Back.IsRoomEdge && this.cell.Right.IsRoomEdge && this.cell.Back.room == null && this.cell.Right.room == null)
								{
									Room room4 = this.cell.Right.Front.room;
									Lot lot = (room4 != null) ? room4.lot : null;
									Room room5 = this.room;
									if (lot == ((room5 != null) ? room5.lot : null))
									{
										this._lowblock = false;
									}
								}
							}
							else if (this.cell.Back.room != null)
							{
								Lot lot2 = this.cell.Back.room.lot;
								Room room6 = this.cell.Front.room ?? this.cell.Right.room;
								if (lot2 == ((room6 != null) ? room6.lot : null))
								{
									this._lowblock = true;
								}
							}
						}
						else
						{
							this._lowblock = this.lowBlock;
						}
						if (this.tileType.RepeatBlock)
						{
							Room room7;
							if ((room7 = this.room) == null && (room7 = this.cell.Front.room) == null)
							{
								room7 = (this.cell.Right.room ?? this.cell.FrontRight.room);
							}
							this.room = room7;
							if (this.room != null && (!this.noRoofMode || this.currentRoom != null) && (!this.showFullWall || this.currentRoom == null || this.room.lot == this.currentRoom.lot))
							{
								this.roomHeight = ((this._lowblock && !this.tileType.ForceRpeatBlock) ? 0f : this.room.lot.realHeight);
								this.maxHeight = (float)(this.cz - this.cx) * this.screen.tileAlign.y + (float)this.room.lot.mh * this._heightMod.y;
							}
						}
					}
				}
			}
			else if (this.tileType.RepeatBlock)
			{
				if (this.currentRoom == null || this.showFullWall)
				{
					Room room8;
					if ((room8 = this.room) == null && (room8 = this.cell.Front.room) == null)
					{
						room8 = (this.cell.Right.room ?? this.cell.FrontRight.room);
					}
					this.room = room8;
					this._lowblock = this.lowBlock;
				}
				else
				{
					if (this.room != this.cell.Front.room)
					{
						if (this.cell.Front.room != this.currentRoom)
						{
							Room room9 = this.room;
							if (((room9 != null) ? room9.lot : null) == this.currentLot)
							{
								goto IL_484C;
							}
							Room room10 = this.cell.Front.room;
							if (((room10 != null) ? room10.lot : null) != this.currentLot)
							{
								goto IL_484C;
							}
						}
						this.room = this.cell.Front.room;
						this._lowblock = (!this.cell.Front.lotWall && !this.cell.Front.fullWall);
						goto IL_4A64;
					}
					IL_484C:
					if (this.room != this.cell.Right.room)
					{
						if (this.cell.Right.room != this.currentRoom)
						{
							Room room11 = this.room;
							if (((room11 != null) ? room11.lot : null) == this.currentLot)
							{
								goto IL_4906;
							}
							Room room12 = this.cell.Right.room;
							if (((room12 != null) ? room12.lot : null) != this.currentLot)
							{
								goto IL_4906;
							}
						}
						this.room = this.cell.Right.room;
						this._lowblock = (!this.cell.Right.lotWall && !this.cell.Right.fullWall);
						goto IL_4A64;
					}
					IL_4906:
					if (this.tileType.IsFullBlock && this.room != this.cell.FrontRight.room)
					{
						if (this.cell.FrontRight.room != this.currentRoom)
						{
							Room room13 = this.room;
							if (((room13 != null) ? room13.lot : null) == this.currentLot)
							{
								goto IL_49D0;
							}
							Room room14 = this.cell.FrontRight.room;
							if (((room14 != null) ? room14.lot : null) != this.currentLot)
							{
								goto IL_49D0;
							}
						}
						this.room = this.cell.FrontRight.room;
						this._lowblock = (!this.cell.FrontRight.lotWall && !this.cell.FrontRight.fullWall);
						goto IL_4A64;
					}
					IL_49D0:
					Room room15;
					if ((room15 = this.room) == null && (room15 = this.cell.Front.room) == null)
					{
						room15 = (this.cell.Right.room ?? this.cell.FrontRight.room);
					}
					this.room = room15;
					this._lowblock = true;
					if (!this.tileType.IsFullBlock)
					{
						if (this.cell.lotWall)
						{
							this._lowblock = false;
						}
						else if (this.room == this.currentRoom)
						{
							this._lowblock = !this.cell.fullWall;
						}
					}
				}
				IL_4A64:
				flag9 = ((this.room != null && this.room.data.atrium) || (this.cell.room != null && this.cell.room.data.atrium));
				if (flag9)
				{
					this._lowblock = false;
				}
				if (this.room == null && this.alwaysLowblock)
				{
					this._lowblock = true;
					this.roomHeight = 0f;
				}
				if (this.room != null)
				{
					this.maxHeight = (float)(this.cz - this.cx) * this.screen.tileAlign.y + (float)this.room.lot.mh * this._heightMod.y;
					if (this.showRoof)
					{
						this.roomHeight = this.room.lot.realHeight;
					}
					else if ((this.noRoofMode && this.currentRoom == null) || (this._lowblock && !this.tileType.ForceRpeatBlock))
					{
						this.roomHeight = 0f;
					}
					else
					{
						int num14 = (this.room.data.maxHeight == 0) ? 2 : this.room.data.maxHeight;
						this.roomHeight = EMono.setting.render.roomHeightMod * (float)((this.room.lot.height < num14) ? this.room.lot.height : num14) + 0.01f * (float)this.room.lot.heightFix;
					}
				}
			}
			if (!this._lowblock && (double)this.roomHeight > 1.2 && this.tileType.RepeatBlock)
			{
				num13 = 1;
			}
			else if (this.lowBlock)
			{
				num13 = 2;
			}
			this.param.mat = this.matBlock;
			this.param.dir = this.cell.blockDir;
			this.param.snow = this.snowed;
			switch (this.tileType.blockRenderMode)
			{
			case BlockRenderMode.FullBlock:
			{
				bool invisible = this.sourceBlock.tileType.Invisible;
				if (!invisible || this.buildMode || ActionMode.Cinema.IsActive)
				{
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
							this.blockLight = this._baseBrightness + this.fogBrightness;
							this.param.color = (float)((int)(50f * this.blockLight) * 262144);
							this.param.matColor = 104025f;
							this.param.tile = (float)((this.buildMode ? 1 : 2) + ((this._lowblock || this.defaultBlockHeight > 0f) ? 3000000 : 0));
							this.rendererInnerBlock.Draw(this.param);
							return;
						}
					}
					if (this.snowed)
					{
						this.param.color = this.floorLight;
					}
					this.param.color -= (float)((int)(this._shadowStrength * 0.8f * 50f) * 262144);
					if (this.currentRoom != null && !this.showFullWall)
					{
						this._lowblock = true;
						this.roomHeight = 0f;
						if (this.cell.room != this.currentRoom && (this.cell.Front.room == this.currentRoom || this.cell.Right.room == this.currentRoom || this.cell.FrontRight.room == this.currentRoom) && (this.cell.Back.room != this.currentRoom || this.cell.Right.room != this.currentRoom) && (this.cell.Front.room != this.currentRoom || this.cell.Left.room != this.currentRoom))
						{
							this._lowblock = false;
						}
						if (!this._lowblock)
						{
							int num15 = (this.currentRoom.data.maxHeight == 0) ? 2 : this.currentRoom.data.maxHeight;
							this.roomHeight = EMono.setting.render.roomHeightMod * (float)((this.currentRoom.lot.height < num15) ? this.currentRoom.lot.height : num15) + 0.01f * (float)this.currentRoom.lot.heightFix;
						}
					}
					if (flag9)
					{
						this._lowblock = ((!this.cell.Front.HasFullBlock || !this.cell.Right.HasFullBlock) && (!this.cell.Front.HasFullBlock || !this.cell.Left.HasFullBlock) && (!this.cell.Back.HasFullBlock || !this.cell.Right.HasFullBlock) && (!this.cell.Back.HasFullBlock || !this.cell.Left.HasFullBlock));
						if (this._lowblock)
						{
							this.roomHeight = 0f;
						}
					}
					if (invisible)
					{
						this.roomHeight = 0f;
						this._lowblock = false;
					}
					if (this.cell.Things.Count > 0)
					{
						this._lowblock = false;
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
						RenderData renderData2 = this.sourceBlock.renderData;
						RenderParam p = this.param;
						float maxY = this.maxHeight;
						float num16 = this.roomHeight;
						GameSetting.RenderSetting renderSetting = this.renderSetting;
						bool hasDoor = this.cell.hasDoor;
						CellEffect effect2 = this.cell.effect;
						renderData2.DrawRepeatTo(p, maxY, num16, ref renderSetting.peakFixBlock, hasDoor, (effect2 != null) ? effect2.FireAmount : 0, true);
					}
					Room room16 = this.cell.Front.room ?? this.cell.room;
					if (room16 == null && this.cell.Right.room != null)
					{
						room16 = this.cell.Right.room;
					}
					if (!invisible && room16 != null)
					{
						if (room16.lot.idDeco != 0 && !this.cell.hasDoor)
						{
							this.param.tile = (float)EMono.sources.blocks.rows[0].ConvertTile(1000 + room16.lot.idDeco);
							this.param.matColor = (float)room16.lot.colDeco;
							float y = this.param.y;
							this.param.y += (float)room16.lot.decoFix * 0.01f;
							this.rendererWallDeco.Draw(this.param);
							this.param.y = y;
						}
						if (room16.lot.idDeco2 != 0 && this.roomHeight != 0f && (float)room16.lot.decoFix2 * 0.01f + this.heightLimitDeco < this.roomHeight + this.maxHeight - this.param.y)
						{
							this.param.tile = (float)EMono.sources.blocks.rows[0].ConvertTile(1000 + room16.lot.idDeco2);
							this.param.matColor = (float)room16.lot.colDeco2;
							float y2 = this.param.y;
							float num17 = this.param.z;
							this.param.y += (float)room16.lot.decoFix2 * 0.01f;
							this.param.z += (float)room16.lot.decoFix2 * 0.01f * this.heightModDeco;
							this.rendererWallDeco.Draw(this.param);
							this.param.y = y2;
							this.param.z = num17;
						}
					}
					room16 = (this.cell.Right.room ?? this.cell.room);
					if (room16 == null && this.cell.Front.room != null)
					{
						room16 = this.cell.Front.room;
					}
					if (!invisible && room16 != null)
					{
						if (room16.lot.idDeco != 0 && !this.cell.hasDoor)
						{
							this.param.tile = (float)(EMono.sources.blocks.rows[0].ConvertTile(1000 + room16.lot.idDeco) * -1);
							this.param.matColor = (float)room16.lot.colDeco;
							float y3 = this.param.y;
							this.param.y += (float)room16.lot.decoFix * 0.01f;
							this.rendererWallDeco.Draw(this.param);
							this.param.y = y3;
						}
						if (room16.lot.idDeco2 != 0 && this.roomHeight != 0f && (float)room16.lot.decoFix2 * 0.01f + this.heightLimitDeco < this.roomHeight + this.maxHeight - this.param.y)
						{
							this.param.tile = (float)(EMono.sources.blocks.rows[0].ConvertTile(1000 + room16.lot.idDeco2) * -1);
							this.param.matColor = (float)room16.lot.colDeco2;
							float y4 = this.param.y;
							float num18 = this.param.z;
							this.param.y += (float)room16.lot.decoFix2 * 0.01f;
							this.param.z += (float)room16.lot.decoFix2 * 0.01f * this.heightModDeco;
							this.rendererWallDeco.Draw(this.param);
							this.param.y = y4;
							this.param.z = num18;
						}
					}
				}
				break;
			}
			case BlockRenderMode.WallOrFence:
			{
				if (this.map.config.fullWallHeight)
				{
					this.showFullWall = true;
					this._lowblock = false;
				}
				this.orgY = this.param.y;
				this.orgZ = this.param.z;
				this.param.color = (this.tileType.IsFence ? (this.floorLight - (float)((int)(this._shadowStrength * 0.8f * 50f) * 262144)) : this.blockLight);
				bool flag10 = blockDir == 1 || this._lowblock || flag9;
				bool flag11 = blockDir == 0 || this._lowblock || flag9;
				if (!this.showFullWall && this.currentRoom != null)
				{
					if (!flag10)
					{
						if (this.currentRoom != this.cell.room)
						{
							if (this.cell.lotWall)
							{
								Room room17 = this.cell.room;
								if (((room17 != null) ? room17.lot : null) == this.currentLot && this.cell.Front.room != this.currentRoom)
								{
									goto IL_5A46;
								}
							}
							if (this.cell.Front.lotWall)
							{
								Room room18 = this.cell.Front.room;
								if (((room18 != null) ? room18.lot : null) == this.currentLot)
								{
									goto IL_5AE3;
								}
							}
							if (this.cell.Front.room != this.currentRoom)
							{
								flag10 = true;
								goto IL_5AE3;
							}
							goto IL_5AE3;
						}
						IL_5A46:
						if (!this.cell.IsRoomEdge || (this.cell.Front.room != this.cell.room && this.cell.FrontRight.room != this.cell.room))
						{
							flag10 = true;
						}
					}
					IL_5AE3:
					if (!flag11)
					{
						if (this.currentRoom != this.cell.room)
						{
							if (this.cell.lotWall)
							{
								Room room19 = this.cell.room;
								if (((room19 != null) ? room19.lot : null) == this.currentLot && this.cell.Right.room != this.currentRoom)
								{
									goto IL_5B44;
								}
							}
							if (this.cell.Right.lotWall)
							{
								Room room20 = this.cell.Right.room;
								if (((room20 != null) ? room20.lot : null) == this.currentLot)
								{
									goto IL_5BE1;
								}
							}
							if (this.cell.Right.room != this.currentRoom)
							{
								flag11 = true;
								goto IL_5BE1;
							}
							goto IL_5BE1;
						}
						IL_5B44:
						if (!this.cell.IsRoomEdge || (this.cell.Right.room != this.cell.room && this.cell.FrontRight.room != this.cell.room))
						{
							flag11 = true;
						}
					}
				}
				IL_5BE1:
				if (blockDir == 0 || blockDir == 2)
				{
					this.param.dir = 0;
					Room room21 = this.cell.Front.room ?? this.cell.room;
					if (room21 != null && this.tileType.IsWall)
					{
						if (room21.lot.idDeco != 0 && !this.cell.hasDoor)
						{
							this.param.tile = (float)EMono.sources.blocks.rows[0].ConvertTile(1000 + room21.lot.idDeco);
							this.param.matColor = (float)room21.lot.colDeco;
							this.param.y += (float)room21.lot.decoFix * 0.01f;
							this.rendererWallDeco.Draw(this.param);
							this.param.y = this.orgY;
						}
						if (room21.lot.idDeco2 != 0 && this.roomHeight != 0f && !flag10 && (float)room21.lot.decoFix2 * 0.01f + this.heightLimitDeco < this.roomHeight + this.maxHeight - this.param.y)
						{
							this.param.tile = (float)EMono.sources.blocks.rows[0].ConvertTile(1000 + room21.lot.idDeco2);
							this.param.matColor = (float)room21.lot.colDeco2;
							this.param.y += (float)room21.lot.decoFix2 * 0.01f;
							this.param.z += (float)room21.lot.decoFix2 * 0.01f * this.heightModDeco;
							this.rendererWallDeco.Draw(this.param);
							this.param.y = this.orgY;
							this.param.z = this.orgZ;
						}
					}
					Cell left = this.cell.Left;
					if (blockDir == 2 && left.sourceBlock.tileType.IsWallOrFence)
					{
						this._sourceBlock = left.sourceBlock;
						this.param.mat = left.matBlock;
					}
					else
					{
						this._sourceBlock = this.sourceBlock;
						this.param.mat = this.matBlock;
					}
					this.tileType = this._sourceBlock.tileType;
					this.param.tile = (float)(this.tile = this._sourceBlock._tiles[0] + ((flag10 && !this.tileType.IsFence) ? 32 : 0));
					if (this._sourceBlock.useAltColor)
					{
						this.param.matColor = (float)((this._sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.param.mat.altColor, this._sourceBlock.colorMod));
					}
					else
					{
						this.param.matColor = (float)((this._sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.param.mat.matColor, this._sourceBlock.colorMod));
					}
					if (this.roomHeight == 0f || flag10)
					{
						if (!this.cell.hasDoor)
						{
							this._sourceBlock.renderData.Draw(this.param);
						}
					}
					else
					{
						RenderData renderData3 = this._sourceBlock.renderData;
						RenderParam p2 = this.param;
						float maxY2 = this.maxHeight;
						float num19 = this.roomHeight;
						GameSetting.RenderSetting renderSetting2 = this.renderSetting;
						bool hasDoor2 = this.cell.hasDoor;
						CellEffect effect3 = this.cell.effect;
						renderData3.DrawRepeatTo(p2, maxY2, num19, ref renderSetting2.peakFix, hasDoor2, (effect3 != null) ? effect3.FireAmount : 0, false);
					}
					this.param.z += this.cornerWallFix2.z;
					if ((blockDir == 2 || (this.cell.Front.HasWallOrFence && this.cell.Front.blockDir != 0)) != this.cell.isToggleWallPillar)
					{
						if (this.cell.Back.IsSnowTile && this.cell.Right.IsSnowTile)
						{
							this.param.snow = true;
						}
						this.param.tile = (float)(this._sourceBlock._tiles[0] + ((flag10 && flag11 && !this.tileType.IsFence && !flag9) ? 32 : 0) + (this.tileType.IsFence ? 32 : 64));
						if (this.roomHeight == 0f || !this.tileType.RepeatBlock || (flag10 && flag11 && !flag9))
						{
							this._sourceBlock.renderData.Draw(this.param);
						}
						else
						{
							this._sourceBlock.renderData.DrawRepeatTo(this.param, this.maxHeight, this.roomHeight, ref this.renderSetting.peakFix, false, 0, false);
						}
					}
					if (!flag10 && !this.showRoof && this.cell.Left.HasWallOrFence && this.cell.Left.blockDir != 0 && !this.cell.isToggleWallPillar)
					{
						this.orgX = this.param.x;
						this.param.tile = (float)(this._sourceBlock._tiles[0] + ((flag10 && !this.tileType.IsFence && !flag9) ? 32 : 0) + (this.tileType.IsFence ? 32 : 64));
						this.param.x += this.cornerWallFix3.x;
						this.param.y += this.cornerWallFix3.y;
						this.param.z += this.cornerWallFix3.z;
						if (!flag9 && (this.roomHeight == 0f || flag10))
						{
							this._sourceBlock.renderData.Draw(this.param);
						}
						else
						{
							this._sourceBlock.renderData.DrawRepeatTo(this.param, this.maxHeight, this.roomHeight + this.cornerWallFix.y, ref this.renderSetting.peakFix, false, 0, false);
						}
						this.param.x = this.orgX;
					}
					else if (this.cell.FrontLeft.HasWallOrFence && this.cell.FrontLeft.blockDir != 0 && (!flag10 || !this.cell.Left.HasWall) && !this.cell.isToggleWallPillar)
					{
						this.orgX = this.param.x;
						this.param.tile = (float)(this._sourceBlock._tiles[0] + ((flag10 && !this.tileType.IsFence && !flag9) ? 32 : 0) + (this.tileType.IsFence ? 32 : 64));
						this.param.x += this.cornerWallFix.x;
						this.param.y += this.cornerWallFix.y;
						this.param.z += this.cornerWallFix.z;
						if (!flag9 && (this.roomHeight == 0f || flag10))
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
					Room room22 = this.cell.Right.room ?? this.cell.room;
					if (room22 != null && this.tileType.IsWall)
					{
						if (room22.lot.idDeco != 0 && !this.cell.hasDoor)
						{
							this.param.tile = (float)(-(float)EMono.sources.blocks.rows[0].ConvertTile(1000 + room22.lot.idDeco));
							this.param.matColor = (float)room22.lot.colDeco;
							this.param.y += (float)room22.lot.decoFix * 0.01f;
							this.rendererWallDeco.Draw(this.param);
							this.param.y = this.orgY;
						}
						if (room22.lot.idDeco2 != 0 && this.roomHeight != 0f && !flag11 && (float)room22.lot.decoFix2 * 0.01f + this.heightLimitDeco < this.roomHeight + this.maxHeight - this.param.y)
						{
							this.param.tile = (float)(-(float)EMono.sources.blocks.rows[0].ConvertTile(1000 + room22.lot.idDeco2));
							this.param.matColor = (float)room22.lot.colDeco2;
							this.param.y += (float)room22.lot.decoFix2 * 0.01f;
							this.param.z += (float)room22.lot.decoFix2 * 0.01f * this.heightModDeco;
							this.rendererWallDeco.Draw(this.param);
							this.param.y = this.orgY;
							this.param.z = this.orgZ;
						}
					}
					if (blockDir == 2 && this.cell.room == null && this.cell.Right.room != null)
					{
						Room room23 = this.cell.Right.room;
						this.maxHeight = (float)(this.cz - this.cx) * this.screen.tileAlign.y + (float)room23.lot.mh * this._heightMod.y;
						if (this.showRoof)
						{
							this.roomHeight = room23.lot.realHeight;
						}
						else if ((this.noRoofMode && this.currentRoom == null) || (this._lowblock && !this.tileType.ForceRpeatBlock))
						{
							this.roomHeight = 0f;
						}
						else
						{
							int num20 = (room23.data.maxHeight == 0) ? 2 : room23.data.maxHeight;
							this.roomHeight = EMono.setting.render.roomHeightMod * (float)((room23.lot.height < num20) ? room23.lot.height : num20) + 0.01f * (float)room23.lot.heightFix;
						}
					}
					Cell back = this.cell.Back;
					if (blockDir == 2 && back.sourceBlock.tileType.IsWallOrFence)
					{
						this._sourceBlock = back.sourceBlock;
						this.param.mat = back.matBlock;
					}
					else
					{
						this._sourceBlock = this.sourceBlock;
						this.param.mat = this.matBlock;
					}
					this.tileType = this._sourceBlock.tileType;
					this.param.tile = (float)(this.tile = -this._sourceBlock._tiles[0] + ((flag11 && !this.tileType.IsFence) ? -32 : 0));
					if (this._sourceBlock.useAltColor)
					{
						this.param.matColor = (float)((this._sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.param.mat.altColor, this._sourceBlock.colorMod));
					}
					else
					{
						this.param.matColor = (float)((this._sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.param.mat.matColor, this._sourceBlock.colorMod));
					}
					this.param.color += this._rightWallShade;
					if (this.roomHeight == 0f || flag11 || !this.tileType.RepeatBlock)
					{
						if (!this.cell.hasDoor)
						{
							this._sourceBlock.renderData.Draw(this.param);
						}
					}
					else
					{
						RenderData renderData4 = this._sourceBlock.renderData;
						RenderParam p3 = this.param;
						float maxY3 = this.maxHeight;
						float num21 = this.roomHeight;
						GameSetting.RenderSetting renderSetting3 = this.renderSetting;
						bool hasDoor3 = this.cell.hasDoor;
						CellEffect effect4 = this.cell.effect;
						renderData4.DrawRepeatTo(p3, maxY3, num21, ref renderSetting3.peakFix, hasDoor3, (effect4 != null) ? effect4.FireAmount : 0, false);
					}
					if ((this.cell.Right.HasWallOrFence && this.cell.Right.blockDir != 1) != this.cell.isToggleWallPillar && (blockDir != 2 || !this.cell.isToggleWallPillar))
					{
						if (this.cell.Left.IsSnowTile && this.cell.Front.IsSnowTile)
						{
							this.param.snow = true;
						}
						this.orgX = this.param.x;
						this.param.tile = (float)(this._sourceBlock._tiles[0] + ((flag11 && !this.tileType.IsFence && !flag9) ? 32 : 0) + (this.tileType.IsFence ? 32 : 64));
						if (!flag9 && (this.roomHeight == 0f || !this.tileType.RepeatBlock || flag11))
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
				this.param.y = this.orgY;
				this.param.z = this.orgZ;
				break;
			}
			case BlockRenderMode.Pillar:
			{
				RenderData renderData5 = this.sourceBlock.renderData;
				this.param.tile = (float)this.sourceBlock._tiles[this.cell.blockDir % this.sourceBlock._tiles.Length];
				this.param.matColor = (float)((this.sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.matBlock.matColor, this.sourceBlock.colorMod));
				int num22 = this.cell.objDir + ((this.cell.objDir >= 7) ? this.cell.objDir : 0) + 1;
				if (num22 == 0)
				{
					renderData5.Draw(this.param);
				}
				else
				{
					renderData5.DrawRepeat(this.param, num22, this.sourceBlock.tileType.RepeatSize, false);
				}
				this.param.tile = (float)renderData5.idShadow;
				SourcePref shadowPref2 = renderData5.shadowPref;
				int shadow2 = shadowPref2.shadow;
				this.passShadow.AddShadow(this.param.x + renderData5.offsetShadow.x, this.param.y + renderData5.offsetShadow.y, this.param.z + renderData5.offsetShadow.z, ShadowData.Instance.items[shadow2], shadowPref2, 0, this.param.snow);
				break;
			}
			case BlockRenderMode.HalfBlock:
				this.param.color = this.floorLight;
				this._sourceBlock = ((this.sourceBlock.id == 5) ? EMono.sources.blocks.rows[this.matBlock.defBlock] : this.sourceBlock);
				this.param.tile = (float)this._sourceBlock._tiles[0];
				this.param.matColor = (float)((this._sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.matBlock.matColor, this._sourceBlock.colorMod));
				this.param.tile2 = this._sourceBlock.sourceAutoFloor._tiles[0];
				this.param.halfBlockColor = ((this._sourceBlock.sourceAutoFloor.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref this.matBlock.matColor, this._sourceBlock.sourceAutoFloor.colorMod));
				this.sourceBlock.renderData.Draw(this.param);
				break;
			default:
				this.param.color = this.floorLight;
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
		if (this.cell.pcSync && EMono.player.lightPower > 0f && !this.cinemaMode)
		{
			bool flag12;
			if (this.cell.room == null && this.cell.IsRoomEdge && this.showRoof)
			{
				if (this.cell._block == 0 || !this.cell.sourceBlock.tileType.RepeatBlock)
				{
					Room room24 = this.cell.FrontRight.room;
					flag12 = (room24 != null && room24.HasRoof);
				}
				else
				{
					flag12 = true;
				}
			}
			else
			{
				flag12 = false;
			}
			if (!flag12 && (!this.showRoof || !this.roof || this.cell.room == null || this.cell.Front.room == null || this.cell.Right.room == null))
			{
				this.param.tile = (float)num13;
				this.rendererFov.Draw(this.param);
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
				this.param.matColor = (float)((this.cell.effect.color == 0) ? BaseTileMap.GetColorInt(ref defaultMaterial.matColor, sourceEffect.colorMod) : this.cell.effect.color);
				sourceEffect.renderData.Draw(this.param);
			}
			else
			{
				this.param.tile = (float)this.cell.effect.source._tiles[0];
				SourceCellEffect.Row sourceEffect2 = this.cell.sourceEffect;
				if (sourceEffect2.anime.Length != 0)
				{
					if (sourceEffect2.anime.Length > 2)
					{
						float num23 = Time.realtimeSinceStartup * 1000f / (float)sourceEffect2.anime[1] % (float)sourceEffect2.anime[2];
						if (num23 < (float)sourceEffect2.anime[0])
						{
							this.param.tile += num23;
						}
					}
					else
					{
						float num24 = Time.realtimeSinceStartup * 1000f / (float)sourceEffect2.anime[1] % (float)sourceEffect2.anime[0];
						this.param.tile += num24;
					}
				}
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
		this.param.color = this.floorLight;
		if (this.cell.critter != null)
		{
			Critter critter = this.cell.critter;
			int snowTile = critter.tile;
			if (this.snowed && critter.SnowTile != 0)
			{
				critter.x = 0.06f;
				critter.y = -0.06f;
				snowTile = critter.SnowTile;
			}
			else
			{
				critter.Update();
			}
			this.pass = this.passObjSS;
			this.batch = this.pass.batches[this.pass.batchIdx];
			this.batch.matrices[this.pass.idx].m03 = this.param.x + (float)((int)(critter.x * 100f)) * 0.01f;
			this.batch.matrices[this.pass.idx].m13 = this.param.y + (float)((int)(critter.y * 100f)) * 0.01f;
			this.batch.matrices[this.pass.idx].m23 = this.param.z;
			this.batch.tiles[this.pass.idx] = (float)(snowTile * (critter.reverse ? -1 : 1));
			this.batch.colors[this.pass.idx] = this.floorLight;
			this.pass.idx++;
			if (this.pass.idx == this.pass.batchSize)
			{
				this.pass.NextBatch();
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
		if (this.cell.obj != 0 && !this.cell.sourceObj.renderData.SkipOnMap)
		{
			SourceObj.Row sourceObj = this.cell.sourceObj;
			if (!this.snowed || sourceObj.snowTile <= 0)
			{
				this.param.snow = this.snowed;
				this.param.mat = this.cell.matObj;
				this.orgY = this.param.y;
				if (this.param.liquidLv > 0)
				{
					if (sourceObj.pref.Float)
					{
						this.param.y += 0.01f * this.floatY;
						if (this.liquidLv > 10)
						{
							this.liquidLv = TileType.FloorWaterShallow.LiquidLV * 10;
						}
						this.liquidLv -= (int)(this.floatY * 0.5f);
						this.param.liquidLv = this.liquidLv;
					}
					if (sourceObj.tileType.IsWaterTop)
					{
						this.param.liquidLv = 0;
					}
					else
					{
						this.param.liquidLv += sourceObj.pref.liquidMod;
						if (this.param.liquidLv < 1)
						{
							this.param.liquid = 1f;
						}
						else if (this.param.liquidLv > 99 + sourceObj.pref.liquidModMax)
						{
							this.param.liquidLv = 99 + sourceObj.pref.liquidModMax;
						}
					}
				}
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
					if (this.cell.autotileObj != 0)
					{
						this.param.tile = (float)(sourceObj._tiles[0] + (int)this.cell.autotileObj);
					}
					else if (sourceObj.tileType.IsUseBlockDir)
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
					this.orgY = this.param.y;
					this.orgZ = this.param.z;
					this.param.y += sourceObj.pref.y;
					this.param.z += sourceObj.pref.z;
					sourceObj.renderData.Draw(this.param);
					this.param.y = this.orgY;
					this.param.z = this.orgZ;
					int shadow3 = sourceObj.pref.shadow;
					if (shadow3 > 1 && !this.cell.ignoreObjShadow)
					{
						this.passShadow.AddShadow(this.param.x + sourceObj.renderData.offsetShadow.x, this.param.y + sourceObj.renderData.offsetShadow.y, this.param.z + sourceObj.renderData.offsetShadow.z, ShadowData.Instance.items[shadow3], sourceObj.pref, 0, this.param.snow);
					}
					this.param.y = this.orgY;
				}
			}
		}
		if (this.cell.decal != 0 && this.sourceFloor.tileType.AllowBlood)
		{
			this.passDecal.Add(this.param, (float)this.cell.decal, this.floorLight, 0f);
		}
		if (this.highlightCells)
		{
			switch (ActionMode.FlagCell.mode)
			{
			case AM_FlagCell.Mode.flagSnow:
				if (this.cell.isClearSnow)
				{
					this.passArea.Add(this.param, 34f, 0f, 0f);
				}
				break;
			case AM_FlagCell.Mode.flagFloat:
				if (this.cell.isForceFloat)
				{
					this.passArea.Add(this.param, 34f, 0f, 0f);
				}
				break;
			case AM_FlagCell.Mode.flagWallPillar:
				if (this.cell.isToggleWallPillar)
				{
					this.passArea.Add(this.param, 34f, 0f, 0f);
				}
				break;
			case AM_FlagCell.Mode.flagClear:
				if (this.cell.isClearArea)
				{
					this.passArea.Add(this.param, 34f, 0f, 0f);
				}
				break;
			}
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
			this.param.matColor = 104025f;
			this.renderFootmark.Draw(this.param);
		}
		IL_7A49:
		if (this.detail.things.Count == 0 && this.detail.charas.Count == 0)
		{
			return;
		}
		int num25 = 0;
		this.thingPos.x = 0f;
		this.thingPos.y = 0f;
		this.thingPos.z = 0f;
		this.freePos.x = (this.freePos.y = (this.freePos.z = 0f));
		if (this.cell.HasRamp)
		{
			Vector3 rampFix = this.sourceBlock.tileType.GetRampFix(this.cell.blockDir);
			this.param.x += rampFix.x;
			this.param.y += rampFix.y;
			this.param.z += rampFix.z;
			this.freePos.x = this.freePos.x + rampFix.x;
			this.freePos.y = this.freePos.y + rampFix.y;
			this.freePos.z = this.freePos.z + rampFix.z;
		}
		this.param.y += (flag ? 0f : ((this.cell._bridge != 0) ? this.cell.sourceBridge.tileType.FloorHeight : this.sourceFloor.tileType.FloorHeight));
		this.orgPos.x = (this.orgX = this.param.x);
		this.orgPos.y = (this.orgY = this.param.y);
		this.orgPos.z = (this.orgZ = this.param.z);
		if (flag && this.liquidLv > 0)
		{
			if (this.liquidLv > 10)
			{
				this.liquidLv = TileType.FloorWaterShallow.LiquidLV * 10;
			}
			this.liquidLv -= (int)(this.floatY * 0.5f);
			this.param.liquidLv = this.liquidLv;
			this.param.y -= TileType.FloorWaterShallow.FloorHeight;
		}
		Thing thing = null;
		CS$<>8__locals1.shadow = (this.liquidLv == 0);
		float num26 = 0f;
		float num27 = 0f;
		bool flag13 = false;
		float num28 = 0f;
		bool flag14 = false;
		float num29 = 0f;
		if (this.detail.things.Count > 0 && this.isSeen)
		{
			float max = this.zSetting.max1;
			float num30 = 0f;
			for (int k = 0; k < this.detail.things.Count; k++)
			{
				Thing t = this.detail.things[k];
				if ((!this.fogged || t.isRoofItem) && ((!t.isHidden && !t.trait.HideInAdv && !t.isMasked) || EMono.scene.actionMode.ShowMaskedThings) && (!t.isRoofItem || ((this.room != null || this.sourceBlock.tileType.IsFullBlock || EMono._zone.IsPCFaction) && (!this.lowBlock || this.showFullWall || this.room == null) && (!this.noRoofMode || this.currentRoom != null))) && (!flag5 || t.isRoofItem))
				{
					TileType tileType = t.trait.tileType;
					bool isInstalled = t.IsInstalled;
					SourcePref pref = t.Pref;
					if (!isInstalled && t.category.tileDummy != 0)
					{
						pref = this.rendererObjDummy.shadowPref;
					}
					float num31 = (tileType.UseMountHeight && isInstalled) ? 0f : ((pref.height < 0f) ? 0f : ((pref.height == 0f) ? 0.1f : pref.height));
					if (t.ignoreStackHeight)
					{
						this.thingPos.y = this.thingPos.y - num26;
					}
					CS$<>8__locals1.shadow = (this.thingPos.y < 0.16f && num29 < 0.16f);
					bool bypassShadow = pref.bypassShadow;
					this.param.shadowFix = -this.thingPos.y;
					this.param.liquidLv = ((this.thingPos.y + (float)t.altitude < 0.1f) ? this.liquidLv : 0);
					if (t.isRoofItem)
					{
						this.param.snow = (this.isSnowCovered && !this.cell.isClearSnow);
						this.SetRoofHeight(this.param, this.cell, this.cx, this.cz, 0, 0, -1, false);
						this._actorPos.x = this.param.x;
						this._actorPos.y = this.param.y;
						this._actorPos.z = this.param.z + num30;
						if (this.room != null)
						{
							this.param.color = (float)this.GetRoofLight(this.room.lot);
						}
						CS$<>8__locals1.shadow = false;
						this.param.liquidLv = 0;
					}
					else
					{
						this.param.snow = this.snowed;
						this._actorPos.x = this.orgX + num27;
						this._actorPos.y = this.orgY;
						this._actorPos.z = this.orgZ + num30 + this.thingPos.z;
						if (tileType.CanStack || !isInstalled)
						{
							if (((thing != null) ? thing.id : null) != t.id)
							{
								this._actorPos.x = this._actorPos.x + this.thingPos.x;
							}
							this._actorPos.y = this._actorPos.y + this.thingPos.y;
							if (t.trait.IgnoreLastStackHeight && (thing == null || !thing.trait.IgnoreLastStackHeight))
							{
								this._actorPos.y = this._actorPos.y - num26;
							}
							this._actorPos.z = this._actorPos.z + (this.renderSetting.thingZ + (float)k * -0.01f + this.zSetting.mod1 * this.thingPos.y);
						}
						if (isInstalled)
						{
							if (t.TileType.IsRamp)
							{
								Vector3 rampFix2 = t.TileType.GetRampFix(t.dir);
								this.orgX += rampFix2.x;
								this.orgY += rampFix2.y;
								this.orgZ += rampFix2.z;
								this.freePos.x = this.freePos.x + rampFix2.x;
								this.freePos.y = this.freePos.y + rampFix2.y;
								this.freePos.z = this.freePos.z + rampFix2.z;
								if (!this.cell.IsTopWater || t.altitude > 0)
								{
									num29 += rampFix2.y;
								}
								this.liquidLv -= (int)(rampFix2.y * 150f);
								if (this.liquidLv < 0)
								{
									this.liquidLv = 0;
								}
							}
							else if (!flag14 && t.trait.IsChangeFloorHeight)
							{
								this.orgY += num31 + (float)t.altitude * this.altitudeFix.y;
								this.orgZ += (float)t.altitude * this.altitudeFix.z;
								this.freePos.y = this.freePos.y + (num31 + (float)t.altitude * this.altitudeFix.y);
								if (!this.cell.IsTopWater || t.altitude > 0)
								{
									num29 += num31 + (float)t.altitude * this.altitudeFix.y;
								}
								this._actorPos.x = this._actorPos.x + pref.x * (float)(t.flipX ? -1 : 1);
								this._actorPos.z = this._actorPos.z + pref.z;
								this.thingPos.z = this.thingPos.z + pref.z;
								this.liquidLv -= (int)(num31 * 150f);
								if (this.liquidLv < 0)
								{
									this.liquidLv = 0;
								}
							}
							else
							{
								this.thingPos.y = this.thingPos.y + num31;
								this._actorPos.x = this._actorPos.x + pref.x * (float)(t.flipX ? -1 : 1);
								this._actorPos.z = this._actorPos.z + pref.z;
								if (pref.height >= 0f)
								{
									this.thingPos.z = this.thingPos.z + pref.z;
								}
							}
							if (!tileType.UseMountHeight && k > 1)
							{
								flag14 = true;
							}
						}
						else
						{
							this.thingPos.y = this.thingPos.y + num31;
							this._actorPos.x = this._actorPos.x + pref.x * (float)(t.flipX ? -1 : 1);
							this._actorPos.z = this._actorPos.z + pref.z;
							this.thingPos.z = this.thingPos.z + pref.z;
						}
						if (t.isFloating && isWater && !this.hasBridge && !flag)
						{
							flag = true;
							float num32 = (this.cell._bridge != 0) ? this.sourceBridge.tileType.FloorHeight : this.sourceFloor.tileType.FloorHeight;
							this.orgY += 0.01f * this.floatY - num32;
							num28 = num31;
							this._actorPos.y = this._actorPos.y + (0.01f * this.floatY - num32);
							if (this.liquidLv > 10)
							{
								this.liquidLv = TileType.FloorWaterShallow.LiquidLV * 10;
							}
							this.liquidLv -= (int)(this.floatY * 0.5f);
							if (this.liquidLv < 0)
							{
								this.liquidLv = 0;
							}
							this.param.liquidLv = this.liquidLv;
						}
						num26 = num31;
						if (t.sourceCard.multisize && !t.trait.IsGround)
						{
							num30 += this.zSetting.multiZ;
						}
						this.orgZ += t.renderer.data.stackZ;
						if (this.param.liquidLv > 0)
						{
							this.param.liquidLv += pref.liquidMod;
							if (this.param.liquidLv < 1)
							{
								this.param.liquidLv = 1;
							}
							else if (this.param.liquidLv > 99 + pref.liquidModMax)
							{
								this.param.liquidLv = 99 + pref.liquidModMax;
							}
						}
					}
					if (isInstalled && tileType.UseMountHeight)
					{
						if (tileType != TileType.Illumination || !this.cell.HasObj)
						{
							if (this.noRoofMode && this.currentRoom == null && t.altitude >= this.lowWallObjAltitude)
							{
								goto IL_8CF5;
							}
							if (this.hideHang)
							{
								Room room25 = this.cell.room;
								if (((room25 != null) ? room25.lot : null) != this.currentLot || (!this.cell.lotWall && this.cell.room != this.currentRoom))
								{
									Room room26 = (t.dir == 0) ? this.cell.Back.room : this.cell.Left.room;
									if (t.trait.AlwaysHideOnLowWall)
									{
										if (room26 == null)
										{
											goto IL_8CF5;
										}
										if (!room26.data.showWallItem)
										{
											goto IL_8CF5;
										}
									}
									else if (t.altitude >= this.lowWallObjAltitude)
									{
										goto IL_8CF5;
									}
								}
							}
						}
						if (tileType.UseHangZFix)
						{
							flag13 = true;
						}
						tileType.GetMountHeight(ref this._actorPos, Point.shared.Set(this.index), t.dir, t);
						CS$<>8__locals1.shadow = false;
						this.param.liquidLv = 0;
						if (t.freePos)
						{
							this._actorPos.x = this._actorPos.x + t.fx;
							this._actorPos.y = this._actorPos.y + t.fy;
						}
					}
					else
					{
						if (t.altitude != 0)
						{
							this._actorPos += this.altitudeFix * (float)t.altitude;
							if (t.altitude > 2 && ((this.cell.Back.room != null && this.cell.Back.IsRoomEdge) || (this.cell.Left.room != null && this.cell.Left.IsRoomEdge)) && this.hideHang)
							{
								Room room27 = this.cell.room;
								if (((room27 != null) ? room27.lot : null) != this.currentLot || (!this.cell.lotWall && this.cell.room != this.currentRoom))
								{
									goto IL_8CF5;
								}
							}
						}
						if (t.freePos)
						{
							this._actorPos.x = this.orgX + t.fx - this.freePos.x;
							this._actorPos.y = this.orgY + t.fy - this.freePos.y;
						}
						if (t.trait is TraitDoor && (t.trait as TraitDoor).IsOpen())
						{
							this._actorPos.z = this._actorPos.z + -0.5f;
						}
					}
					if (!t.sourceCard.multisize || (t.pos.x == this.cx && t.pos.z == this.cz))
					{
						if (this.iconMode != BaseTileMap.CardIconMode.None)
						{
							int num33 = 0;
							switch (this.iconMode)
							{
							case BaseTileMap.CardIconMode.Deconstruct:
								if (t.isDeconstructing)
								{
									num33 = 14;
								}
								break;
							case BaseTileMap.CardIconMode.State:
								if (t.placeState == PlaceState.installed)
								{
									num33 = 18;
								}
								break;
							case BaseTileMap.CardIconMode.Visibility:
								if (t.isMasked)
								{
									num33 = 17;
								}
								break;
							}
							if (t.isNPCProperty && !EMono.debug.godBuild)
							{
								num33 = 13;
							}
							if (num33 != 0)
							{
								this.passGuideBlock.Add(this._actorPos.x, this._actorPos.y, this._actorPos.z - 10f, (float)num33, 0f);
							}
						}
						t.SetRenderParam(this.param);
						if (this._lowblock && t.trait.UseLowblock && !this.cell.HasFullBlock)
						{
							this.param.tile += (float)((this.param.tile < 0f) ? -64 : 64);
						}
						if (t.trait is TraitTrolley)
						{
							AI_Trolley ai_Trolley = EMono.pc.ai as AI_Trolley;
							if (ai_Trolley != null && ai_Trolley.trolley.owner == t)
							{
								RenderParam _param = new RenderParam(this.param);
								EMono.core.actionsLateUpdate.Add(delegate
								{
									t.SetRenderParam(_param);
									CS$<>8__locals1.<>4__this._actorPos.x = EMono.pc.renderer.position.x;
									CS$<>8__locals1.<>4__this._actorPos.y = EMono.pc.renderer.position.y - pref.height;
									CS$<>8__locals1.<>4__this._actorPos.z = EMono.pc.renderer.position.z + 0.02f;
									t.renderer.Draw(_param, ref CS$<>8__locals1.<>4__this._actorPos, !t.noShadow && (CS$<>8__locals1.shadow || tileType.AlwaysShowShadow));
								});
								goto IL_8C6A;
							}
						}
						t.renderer.Draw(this.param, ref this._actorPos, !t.noShadow && (CS$<>8__locals1.shadow || tileType.AlwaysShowShadow));
					}
					IL_8C6A:
					if (isInstalled)
					{
						num27 += pref.stackX * (float)(t.flipX ? -1 : 1);
					}
					this.param.x = this.orgX;
					this.param.y = this.orgY;
					this.param.z = this.orgZ;
					this.param.color = this.floorLight;
					thing = t;
					if (pref.Float)
					{
						this.liquidLv = 0;
					}
				}
				IL_8CF5:;
			}
		}
		this.orgY += num28;
		if (this.detail.charas.Count > 0)
		{
			this.param.shadowFix = -num29;
			this.param.color += 1310720f;
			float max2 = this.zSetting.max2;
			for (int l = 0; l < this.detail.charas.Count; l++)
			{
				Chara chara = this.detail.charas[l];
				if (chara.host == null && (chara == EMono.pc || chara == LayerDrama.alwaysVisible || (!flag5 && !this.fogged && (this.showAllCards || EMono.player.CanSee(chara)))))
				{
					this._actorPos.x = this.orgX;
					this._actorPos.y = this.orgY;
					this._actorPos.z = this.orgZ;
					chara.SetRenderParam(this.param);
					bool isAliveInCurrentZone = chara.IsAliveInCurrentZone;
					if (chara.isRestrained)
					{
						TraitShackle restrainer = chara.GetRestrainer();
						if (restrainer != null)
						{
							Vector3 getRestrainPos = restrainer.GetRestrainPos;
							if (getRestrainPos != default(Vector3))
							{
								Vector3 position = restrainer.owner.renderer.position;
								float defCharaHeight = EMono.setting.render.defCharaHeight;
								float num34 = getRestrainPos.y + defCharaHeight - ((chara.Pref.height == 0f) ? defCharaHeight : chara.source.pref.height);
								this._actorPos.x = position.x + getRestrainPos.x * (float)((restrainer.owner.dir % 2 == 0) ? 1 : -1);
								this._actorPos.y = position.y + num34;
								this._actorPos.z = position.z + getRestrainPos.z;
								this.param.liquidLv = 0;
								this.param.shadowFix = this.orgY - this._actorPos.y;
								chara.renderer.SetFirst(true);
								chara.renderer.Draw(this.param, ref this._actorPos, true);
								this.param.shadowFix = 0f;
								goto IL_92B7;
							}
						}
					}
					if (!chara.sourceCard.multisize || (chara.pos.x == this.cx && chara.pos.z == this.cz))
					{
						if (chara.IsDeadOrSleeping && chara.IsPCC)
						{
							float num35 = chara.renderer.data.size.y * 0.3f;
							if (this.thingPos.y > max2)
							{
								this.thingPos.y = max2;
							}
							float num36 = this.thingPos.y + num35;
							float num37 = (float)l * -0.01f;
							if (num36 > this.zSetting.thresh1)
							{
								num37 = this.zSetting.mod1;
							}
							this._actorPos.x = this._actorPos.x + this.thingPos.x;
							this._actorPos.y = this._actorPos.y + this.thingPos.y;
							this._actorPos.z = this._actorPos.z + (this.renderSetting.laydownZ + num37);
							this.param.liquidLv = ((this.thingPos.y == 0f && this.liquidLv > 0) ? 90 : 0);
							this.thingPos.y = this.thingPos.y + num35 * 0.8f;
							chara.renderer.Draw(this.param, ref this._actorPos, this.liquidLv == 0);
						}
						else
						{
							this.param.liquidLv = this.liquidLv;
							if (this.param.liquidLv > 0)
							{
								this.param.liquidLv += chara.Pref.liquidMod;
								if (this.param.liquidLv < 1)
								{
									this.param.liquidLv = 1;
								}
								else if (this.param.liquidLv > 99 + chara.Pref.liquidModMax)
								{
									this.param.liquidLv = 99 + chara.Pref.liquidModMax;
								}
							}
							if (!chara.IsPC && !chara.renderer.IsMoving && this.detail.charas.Count > 1 && (this.detail.charas.Count != 2 || !this.detail.charas[0].IsDeadOrSleeping || !this.detail.charas[0].IsPCC))
							{
								this._actorPos += this.renderSetting.charaPos[1 + ((num25 < 4) ? num25 : 3)];
							}
							this._actorPos.z = this._actorPos.z + (0.01f * (float)l + this.renderSetting.charaZ);
							num25++;
							if (flag13)
							{
								this._actorPos.z = this._actorPos.z + chara.renderer.data.hangedFixZ;
							}
							chara.renderer.Draw(this.param, ref this._actorPos, this.liquidLv == 0);
						}
					}
					this.param.x = this.orgX;
					this.param.y = this.orgY;
					this.param.z = this.orgZ;
				}
				IL_92B7:;
			}
		}
	}

	public Vector3 GetThingPosition(Card tg, Point p)
	{
		Vector3 vector = Vector3.zero;
		float num = 0f;
		this.cell = p.cell;
		this.sourceFloor = this.cell.sourceFloor;
		if (!tg.TileType.UseMountHeight)
		{
			if (this.cell.isFloating && !this.cell.IsSnowTile)
			{
				vector.z -= 1f;
			}
			else if (!tg.sourceCard.multisize)
			{
				float num2 = (this.cell._bridge != 0) ? this.cell.sourceBridge.tileType.FloorHeight : this.sourceFloor.tileType.FloorHeight;
				vector.y += num2;
				vector.z -= num2 * this.heightMod.z;
			}
			if (this.cell.HasRamp)
			{
				Vector3 rampFix = this.cell.sourceBlock.tileType.GetRampFix(this.cell.blockDir);
				vector.x += rampFix.x;
				vector.y += rampFix.y;
				vector.z += rampFix.z;
			}
		}
		if (tg.sourceCard.multisize)
		{
			vector.z -= 1f;
		}
		vector.x += tg.Pref.x * (float)(tg.flipX ? -1 : 1);
		vector.z += tg.Pref.z;
		this.detail = this.cell.detail;
		if (tg.isChara)
		{
			return vector;
		}
		bool flag = false;
		if (tg.TileType.UseMountHeight && !EMono.scene.actionMode.IsRoofEditMode(tg))
		{
			flag = true;
		}
		else if (!tg.TileType.CanStack || EMono.scene.actionMode.IsRoofEditMode(tg))
		{
			if (tg.altitude != 0)
			{
				vector += this.altitudeFix * (float)tg.altitude;
			}
			flag = true;
		}
		if (EMono.scene.actionMode.IsRoofEditMode(tg))
		{
			return vector;
		}
		float num3 = 0f;
		if (this.detail != null && this.detail.things.Count > 0)
		{
			for (int i = 0; i < this.detail.things.Count; i++)
			{
				Thing thing = this.detail.things[i];
				SourcePref pref = thing.Pref;
				TileType tileType = thing.trait.tileType;
				float num4 = tileType.UseMountHeight ? 0f : ((pref.height == 0f) ? 0.1f : pref.height);
				if (thing.IsInstalled && thing != ActionMode.Inspect.target)
				{
					if (thing.TileType.IsRamp)
					{
						Vector3 rampFix2 = thing.TileType.GetRampFix(thing.dir);
						vector.x += rampFix2.x;
						vector.y += rampFix2.y;
						vector.z += rampFix2.z;
					}
					if (!flag && tileType.CanStack)
					{
						if (thing.ignoreStackHeight)
						{
							vector.y -= num3;
						}
						vector.y += num4;
						vector.x += pref.stackX * (float)(thing.flipX ? -1 : 1);
						vector.z += pref.z + thing.renderer.data.stackZ;
						if (!tileType.UseMountHeight && thing.altitude != 0)
						{
							vector += this.altitudeFix * (float)thing.altitude;
							num4 += this.altitudeFix.y * (float)thing.altitude;
						}
						num3 = num4;
						vector.z += this.renderSetting.thingZ + num + (float)i * -0.01f + this.zSetting.mod1 * vector.y;
						if (thing.sourceCard.multisize)
						{
							num += this.zSetting.multiZ;
						}
					}
				}
			}
		}
		if (flag)
		{
			return vector;
		}
		if (tg.ignoreStackHeight)
		{
			vector.y -= num3;
		}
		if (tg.altitude != 0)
		{
			vector += this.altitudeFix * (float)tg.altitude;
		}
		return vector;
	}

	public int GetApproximateBlocklight(Cell cell)
	{
		float num = this._baseBrightness + 0.05f;
		if (cell.IsSnowTile)
		{
			num = (float)((int)(num * 50f) * 262144 + (int)((float)((cell.lightR >= 50) ? 50 : cell.lightR) * this.snowColor) * 4096 + (int)((float)((cell.lightG >= 50) ? 50 : cell.lightG) * this.snowColor) * 64 + (int)((float)((cell.lightB >= 50) ? 50 : cell.lightB) * this.snowColor) + this.snowColorToken);
		}
		else
		{
			num = (float)((int)(num * 50f) * 262144 + (int)(((cell.lightR >= 64) ? 63 : cell.lightR) * 4096) + (int)(((cell.lightG >= 64) ? 63 : cell.lightG) * 64) + (int)((cell.lightB >= 64) ? 63 : cell.lightB));
		}
		return (int)num;
	}

	public int GetRoofLight(Lot lot)
	{
		float num = Mathf.Sqrt(lot.light) * this.roofLightMod;
		if (num > this.lightLimit * this.roofLightLimitMod)
		{
			num = this.lightLimit * this.roofLightLimitMod;
		}
		if (this.isSnowCovered)
		{
			num += this.roofLightSnow * (1f - this.nightRatio);
		}
		int num2 = (int)(num * 50f) * 262144;
		if (this.isSnowCovered)
		{
			num2 += this.snowColorToken;
		}
		return num2;
	}

	public void DrawRoof(Lot lot)
	{
		RoofStyle roofStyle = this.roofStyles[lot.idRoofStyle];
		if (roofStyle.type == RoofStyle.Type.None)
		{
			return;
		}
		bool reverse = lot.reverse;
		int num;
		int num2;
		int num3;
		int num4;
		if (reverse)
		{
			num = lot.z - roofStyle.h;
			num2 = lot.x - roofStyle.w;
			num3 = lot.mz + 1 + roofStyle.h;
			num4 = lot.mx + 1 + roofStyle.w;
			if (num2 > 1 && num > 0 && this.map.cells[num2 - 1, num].HasFullBlock)
			{
				num2--;
			}
			if (num3 < this.Size && num4 < this.Size && this.map.cells[num4 - 1, num3].HasFullBlock)
			{
				num3++;
			}
		}
		else
		{
			num = lot.x - roofStyle.w;
			num2 = lot.z - roofStyle.h;
			num3 = lot.mx + 1 + roofStyle.w;
			num4 = lot.mz + 1 + roofStyle.h;
			if (num2 > 0 && num > 1 && this.map.cells[num - 1, num2].HasFullBlock)
			{
				num--;
			}
			if (num3 < this.Size && num4 < this.Size && this.map.cells[num3 - 1, num4].HasFullBlock)
			{
				num4++;
			}
		}
		bool flag = roofStyle.wing && lot.height > 1;
		if (flag)
		{
			num2--;
			num4++;
		}
		int num5 = num4 + (reverse ? roofStyle.w : roofStyle.h) * 2 - num2;
		int idRoofTile = lot.idRoofTile;
		int num6 = lot.idBlock;
		int num7 = num6;
		if (num6 >= EMono.sources.blocks.rows.Count)
		{
			num6 = EMono.sources.blocks.rows.Count - 1;
		}
		if (num7 >= EMono.sources.floors.rows.Count)
		{
			num7 = EMono.sources.floors.rows.Count - 1;
		}
		int num8 = lot.idRamp;
		if (num8 >= EMono.sources.blocks.rows.Count)
		{
			num8 = EMono.sources.blocks.rows.Count - 1;
		}
		bool flag2 = false;
		int num9 = num5 / 2 - roofStyle.flatW;
		int num10 = num5 / 2 + roofStyle.flatW + ((num5 % 2 == 0) ? 0 : 1);
		SourceBlock.Row row = roofStyle.useDefBlock ? this.cell.sourceFloor._defBlock : EMono.sources.blocks.rows[num6];
		int num11 = 0;
		int num12 = flag ? -1 : 0;
		int num13 = 0;
		Vector3 vector = lot.fullblock ? roofStyle.posFixBlock : roofStyle.posFix;
		switch (roofStyle.type)
		{
		case RoofStyle.Type.Default:
			flag2 = (num5 % 2 == 1 && roofStyle.flatW == 0);
			break;
		case RoofStyle.Type.Flat:
		case RoofStyle.Type.FlatFloor:
			num9 = roofStyle.flatW;
			num10 = num5 - roofStyle.flatW;
			if (num9 == 0)
			{
				num13 = 1;
			}
			if (roofStyle.type != RoofStyle.Type.FlatFloor)
			{
				num--;
				num3++;
				num2--;
				num4++;
			}
			break;
		case RoofStyle.Type.Triangle:
			num9 = 999;
			num10 = 999;
			break;
		}
		this.cz = num2;
		while (this.cz < num4)
		{
			this.cx = num;
			while (this.cx < num3)
			{
				if (this.cx >= 0 && this.cz >= 0 && this.cx < this.Size && this.cz < this.Size)
				{
					int num14;
					int num15;
					if (reverse)
					{
						num14 = this.cz;
						num15 = this.cx;
						this.cell = this.map.cells[num14, num15];
						if (roofStyle.wing && this.cz == num4 - 1 && this.cell.Right.Right.room != null && this.cell.Right.Right.room.lot != lot)
						{
							goto IL_1068;
						}
					}
					else
					{
						num14 = this.cx;
						num15 = this.cz;
						this.cell = this.map.cells[num14, num15];
						if (roofStyle.wing && this.cz == 0 && this.cell.Front.Front.room != null && this.cell.Front.Front.room.lot != lot)
						{
							goto IL_1068;
						}
					}
					int num16 = num15 - num14;
					this.room = this.cell.room;
					if (this.room == null || this.room.lot == lot)
					{
						bool flag3 = false;
						if (roofStyle.type == RoofStyle.Type.Flat)
						{
							if (reverse)
							{
								if (this.cell.HasFullBlock && this.cell.room == null)
								{
									Room room = this.cell.Left.room;
									if (((room != null) ? room.lot : null) == lot && this.cell.Right.room != null)
									{
										num13 = 1;
										flag3 = true;
									}
									else
									{
										Room room2 = this.cell.Front.room;
										if (((room2 != null) ? room2.lot : null) == lot)
										{
											Room room3 = this.cell.FrontRight.room;
											num13 = ((((room3 != null) ? room3.lot : null) == lot) ? 1 : 2);
											flag3 = true;
										}
										else
										{
											Room room4 = this.cell.Right.room;
											if (((room4 != null) ? room4.lot : null) != lot)
											{
												Room room5 = this.cell.FrontRight.room;
												if (((room5 != null) ? room5.lot : null) != lot)
												{
													goto IL_7EB;
												}
											}
											num13 = 0;
											flag3 = true;
										}
									}
								}
								else if (this.cell.Left.room == null || this.cell.Left.room.lot != lot)
								{
									num13 = (this.cell.Left.HasFullBlock ? 1 : 0);
								}
								else if (this.cell.Right.room == null || this.cell.Right.room.lot != lot)
								{
									num13 = ((this.cell.HasFullBlock && this.cell.Right.HasFullBlock && this.cell.Right.room != null) ? 1 : 2);
								}
								else
								{
									num13 = 1;
								}
							}
							else if (this.cell.HasFullBlock && this.cell.room == null)
							{
								Room room6 = this.cell.Right.room;
								if (((room6 != null) ? room6.lot : null) == lot)
								{
									Room room7 = this.cell.FrontRight.room;
									num13 = ((((room7 != null) ? room7.lot : null) == lot) ? 1 : 0);
									flag3 = true;
								}
								else
								{
									Room room8 = this.cell.Front.room;
									if (((room8 != null) ? room8.lot : null) != lot)
									{
										Room room9 = this.cell.FrontRight.room;
										if (((room9 != null) ? room9.lot : null) != lot)
										{
											goto IL_7EB;
										}
									}
									num13 = 2;
									flag3 = true;
								}
							}
							else if (this.cell.Front.room == null || this.cell.Front.room.lot != lot)
							{
								num13 = ((this.cell.HasFullBlock && this.cell.Front.HasFullBlock && this.cell.Front.room != null) ? 1 : 0);
							}
							else if (this.cell.Back.room == null || this.cell.Back.room.lot != lot)
							{
								num13 = (this.cell.Back.HasFullBlock ? 1 : 2);
							}
							else
							{
								num13 = 1;
							}
							IL_7EB:
							num12 = 0;
						}
						if (this.room != null || roofStyle.coverLot || flag3)
						{
							this.index = this.cx + this.cz * this.Size;
							this.height = (int)this.cell.TopHeight;
							bool flag4 = this.isSnowCovered && !this.cell.isClearSnow;
							float num17 = (float)num16 * this.screen.tileAlign.y + (float)lot.mh * this._heightMod.y + lot.realHeight + this.roofFix.y + vector.y;
							float num18 = 1000f + this.param.x * this.screen.tileWeight.x + (float)lot.mh * this._heightMod.z + lot.realHeight * this.roofFix3.z + this.roofFix.z + vector.z;
							if (lot.height == 1 && lot.heightFix < 20)
							{
								num17 += roofStyle.lowRoofFix.y;
								num18 += roofStyle.lowRoofFix.z;
							}
							this.param.x = (float)(this.cx + this.cz) * this.screen.tileAlign.x + this.roofFix.x + (float)num12 * this.roofFix2.x + vector.x * (float)(reverse ? 1 : -1);
							this.param.y = num17 + (float)num12 * this.roofFix2.y;
							this.param.z = num18 + this.param.y * this.screen.tileWeight.z + (float)num12 * this.roofFix2.z;
							this.param.color = (float)this.GetRoofLight(lot);
							this.param.snow = (idRoofTile == 0 && flag4);
							this.param.shadowFix = 0f;
							if (num13 == 1)
							{
								SourceMaterial.Row mat = this.matBlock;
								RenderRow renderRow;
								if (roofStyle.type == RoofStyle.Type.FlatFloor)
								{
									if (this.cell.HasFullBlock && this.cell.IsRoomEdge)
									{
										goto IL_1068;
									}
									renderRow = EMono.sources.floors.rows[num7];
									renderRow.SetRenderParam(this.param, mat, 0);
									this.param.matColor = (float)lot.colRoof;
								}
								else
								{
									renderRow = row;
									renderRow.SetRenderParam(this.param, mat, 0);
									this.param.matColor = (float)lot.colBlock;
								}
								renderRow.renderData.Draw(this.param);
								if (idRoofTile != 0)
								{
									renderRow = EMono.sources.blocks.rows[EMono.sources.objs.rows[idRoofTile].idRoof];
									int num19 = (reverse ? 1 : 0) + (flag2 ? 0 : 2);
									renderRow.SetRenderParam(this.param, MATERIAL.sourceGold, num19);
									this.param.matColor = (float)lot.colRoof;
									if (roofStyle.type == RoofStyle.Type.FlatFloor)
									{
										this.param.x += roofStyle.posFixBlock.x;
										this.param.y += roofStyle.posFixBlock.y;
										this.param.z += roofStyle.posFixBlock.z;
									}
									if (!flag2)
									{
										this.param.z += 0.5f;
									}
									if (flag4)
									{
										this.param.matColor = 104025f;
										if (roofStyle.type != RoofStyle.Type.FlatFloor)
										{
											this.param.z += roofStyle.snowZ;
										}
										this.param.tile = (float)(renderRow.renderData.ConvertTile(renderRow.snowTile) + num19);
										renderRow.renderData.Draw(this.param);
									}
									else
									{
										renderRow.renderData.Draw(this.param);
									}
								}
								else if (flag4 && roofStyle.type == RoofStyle.Type.FlatFloor)
								{
									this.param.matColor = 104025f;
									this.param.tile = 10f;
									this.param.x += roofStyle.snowFix.x;
									this.param.y += roofStyle.snowFix.y;
									this.param.z += roofStyle.snowZ + roofStyle.snowFix.z;
									renderRow.renderData.Draw(this.param);
								}
							}
							else
							{
								if (idRoofTile != 0)
								{
									int num20 = reverse ? ((num13 == 0) ? 3 : 1) : ((num13 == 0) ? 0 : 2);
									if (lot.altRoof && !flag2 && (roofStyle.type == RoofStyle.Type.Default || roofStyle.type == RoofStyle.Type.DefaultNoTop))
									{
										this.param.shadowFix = (float)(num20 + 1);
									}
									RenderRow renderRow = EMono.sources.objs.rows[idRoofTile];
									renderRow.SetRenderParam(this.param, MATERIAL.sourceGold, num20);
									this.param.matColor = (float)lot.colRoof;
									if (flag4)
									{
										this.param.matColor = 104025f;
										this.param.z += roofStyle.snowZ;
										this.param.tile = (float)(renderRow.renderData.ConvertTile(renderRow.snowTile) + num20 + (lot.altRoof ? 8 : 0));
										renderRow.renderData.Draw(this.param);
									}
									else
									{
										this.param.tile += (float)(lot.altRoof ? 8 : 0);
										renderRow.renderData.Draw(this.param);
									}
									this.param.shadowFix = 0f;
								}
								if (num12 >= 0)
								{
									this.param.y += this.roofRampFix.y;
									this.param.z += this.roofRampFix.z;
									RenderRow renderRow = EMono.sources.blocks.rows[num8];
									renderRow.SetRenderParam(this.param, MATERIAL.sourceGold, reverse ? ((num13 == 0) ? 3 : 1) : ((num13 == 0) ? 0 : 2));
									this.param.matColor = (float)lot.colBlock;
									renderRow.renderData.Draw(this.param);
								}
							}
							CellEffect effect = this.cell.effect;
							if (effect != null && effect.FireAmount > 0)
							{
								this.rendererEffect.Draw(this.param, this.cell.effect.FireAmount);
							}
							if (num12 >= 1)
							{
								if (roofStyle.type != RoofStyle.Type.Flat)
								{
									this.param.snow = false;
								}
								for (int i = 0; i < num12; i++)
								{
									this.param.x = (float)(this.cx + this.cz) * this.screen.tileAlign.x + this.roofFix.x + (float)i * this.roofFix2.x + vector.x * (float)(reverse ? 1 : -1);
									this.param.y = num17 + (float)i * this.roofFix2.y;
									this.param.z = num18 + this.param.y * this.screen.tileWeight.z + (float)i * this.roofFix2.z;
									RenderRow renderRow = row;
									renderRow.SetRenderParam(this.param, MATERIAL.sourceGold, 0);
									this.param.matColor = (float)lot.colBlock;
									renderRow.renderData.Draw(this.param);
									this.index++;
									CellEffect effect2 = this.cell.effect;
									if (effect2 != null && effect2.FireAmount > 0 && Rand.bytes[this.index % Rand.MaxBytes] % 3 == 0)
									{
										this.rendererEffect.Draw(this.param, this.cell.effect.FireAmount);
									}
								}
							}
						}
					}
				}
				IL_1068:
				this.cx++;
			}
			num11++;
			if (roofStyle.type != RoofStyle.Type.Flat)
			{
				if (num11 == num9)
				{
					num13 = 1;
				}
				if (num11 == num10)
				{
					num13 = 2;
					num12++;
				}
				num12 += ((num13 == 0) ? 1 : ((num13 == 1) ? 0 : -1));
			}
			this.cz++;
		}
	}

	public static int GetColorInt(ref Color matColor, int p)
	{
		if (p == 0)
		{
			return 104025;
		}
		return p * 262144 + (int)(matColor.r * 50f) * 4096 + (int)(matColor.g * 50f) * 64 + (int)(matColor.b * 50f);
	}

	public void SetRoofHeight(MeshPassParam _param, Cell _cell, int _cx, int _cz, int h = 0, int altitude = 0, int dirWall = -1, bool ignoreAltitudeY = false)
	{
		Room room = _cell.room;
		if (room == null && dirWall != -1)
		{
			if (dirWall == 0 && _cell.Front.room != null)
			{
				room = _cell.Front.room;
				_cell = _cell.Front;
			}
			else if (_cell.Right.room != null)
			{
				room = _cell.Right.room;
				_cell = _cell.Right;
			}
		}
		if (room != null)
		{
			Lot lot = room.lot;
			RoofStyle roofStyle = this.roofStyles[lot.idRoofStyle];
			float num = (float)(_cz - _cx) * this.screen.tileAlign.y + (float)lot.mh * this._heightMod.y + lot.realHeight + this.roofFix.y;
			float num2 = 1000f + this.param.x * this.screen.tileWeight.x + (float)lot.mh * this._heightMod.z + lot.realHeight * this.roofFix3.z + this.roofFix.z + roofStyle.posFix.z;
			if (lot.height == 1)
			{
				num += roofStyle.lowRoofFix.y;
				num2 += roofStyle.lowRoofFix.z;
			}
			_param.x = (float)(_cx + _cz) * this.screen.tileAlign.x + this.roofFix.x + (float)h * this.roofFix2.x;
			_param.y = num + (float)h * this.roofFix2.y;
			_param.z = num2 + _param.y * this.screen.tileWeight.z + (float)h * this.roofFix2.z + this.heightModRoofBlock.y;
		}
		if (!ignoreAltitudeY || room != null)
		{
			_param.y += (float)altitude * this._heightMod.y;
		}
		_param.z += (float)altitude * this.heightModRoofBlock.z;
	}

	public static bool forceShowHang;

	public int count;

	[Header("Mouse Hit")]
	public Collider2D mouseCollider;

	public Vector3 colliderFix;

	public int maxColliderCheck;

	[Header("Height")]
	public float heightBlockSize;

	public float slopeFixZ;

	public float heightModDeco;

	public float heightLimitDeco;

	public Vector3 heightMod;

	public Vector3 ugFix;

	public Vector3 ugFixBridge;

	public Vector3 ugFixBridgeBottom;

	public Vector3 ugFixBridgeTop;

	public Vector3 heightModRoofBlock;

	[Header("Room")]
	public float rightWallShade;

	public Vector3 roofRampFix;

	public Vector3 roofFix;

	public Vector3 roofFix2;

	public Vector3 roofFix3;

	public float roofLightMod;

	public float lotLight;

	public float lotLight2;

	public float roofLightSnow;

	[Header("Other")]
	public BaseTileMap.WallClipMode wallClipMode;

	public BaseTileMap.InnerMode defaultInnerMode;

	public Vector3 edgeBlockFix;

	public Vector3 bridgeFix;

	public Vector3 cornerWallFix;

	public Vector3 cornerWallFix2;

	public Vector3 cornerWallFix3;

	public Vector3 altitudeFix;

	public Vector3 waterEdgeFix;

	public Vector3 waterEdgeBridgeFix;

	public Vector3 waterEdgeFixShore;

	public Vector3 waterEdgeFixShoreSand;

	public Vector3[] ambientShadowFix;

	public Vector3[] transitionFix;

	public Vector3[] wallHangFix;

	public Vector3[] waterEdgeBlockFix;

	public int fogTile;

	public float floatSpeed;

	public float shadowModStrength;

	public int maxFloat;

	public int[] seaAnimeIndexes;

	[Header("References")]
	public NoiseLayer layerGroundLights;

	public MeshPass passShadow;

	public MeshPass passLiquid;

	public MeshPass passGuideBlock;

	public MeshPass passGuideFloor;

	public MeshPass passArea;

	public MeshPass passRamp;

	public MeshPass passFloor;

	public MeshPass passBlock;

	public MeshPass passObjS;

	public MeshPass passObjSS;

	public MeshPass passObj;

	public MeshPass passObjL;

	public MeshPass passDecal;

	public MeshPass passRoof;

	public MeshPass passBlockEx;

	public MeshPass passFloorEx;

	public MeshPass passFloorWater;

	public MeshPass passInner;

	public MeshPass passFog;

	public MeshPass passFov;

	public MeshPass passEdge;

	public MeshPass passAutoTile;

	public MeshPass passAutoTileWater;

	public MeshPass passBlockMarker;

	public MeshPass passFloorMarker;

	public MeshPass passWaterBlock;

	public MeshPass passIcon;

	public MeshPass passChara;

	public MeshPass passCharaL;

	public MeshPass passCharaLL;

	public MeshPass passShore;

	public RenderData renderFootmark;

	public RenderData rendererBlockMarker;

	public RenderData rendererFloorMarker;

	public RenderData rendererInnerBlock;

	public RenderData rendererFov;

	public RenderData rendererFov2;

	public RenderData rendererShore;

	public RenderData renderBorder;

	public RenderData rendererFogBlockSolid;

	public RenderData rendererFogFloorSolid;

	public RenderData rendererFogRoomSolid;

	public RenderData rendererFogRoomBlockSolid;

	public RenderData rendererFogRoomWallSolid;

	public RenderData rendererWallDeco;

	public RenderData rendererWaterBlock;

	public RenderDataObjDummy rendererObjDummy;

	public RenderDataEffect rendererEffect;

	public Point TestPoint = new Point();

	protected BaseTileMap.InnerMode innerMode;

	[NonSerialized]
	public int Size;

	[NonSerialized]
	public int SizeXZ;

	[NonSerialized]
	public int mx;

	[NonSerialized]
	public int mz;

	[NonSerialized]
	public int x;

	[NonSerialized]
	public int z;

	[NonSerialized]
	public int cx;

	[NonSerialized]
	public int cz;

	[NonSerialized]
	public int activeCount;

	[NonSerialized]
	public int floatV = 1;

	[NonSerialized]
	public byte[] groundLights;

	[NonSerialized]
	public bool lowBlock;

	[NonSerialized]
	public bool lowObj;

	[NonSerialized]
	public bool highlightArea;

	[NonSerialized]
	public bool subtleHighlightArea;

	[NonSerialized]
	public bool hideRoomFog;

	[NonSerialized]
	public bool showRoof;

	[NonSerialized]
	public bool showFullWall;

	[NonSerialized]
	public bool hideHang;

	[NonSerialized]
	public bool usingHouseBoard;

	[NonSerialized]
	public bool noRoofMode;

	[NonSerialized]
	public bool fogged;

	[NonSerialized]
	public float[] lightLookUp;

	[NonSerialized]
	public float _lightMod;

	[NonSerialized]
	public float _baseBrightness;

	[NonSerialized]
	public float lowblockTimer;

	[NonSerialized]
	public float heightLightMod;

	[NonSerialized]
	public float _rightWallShade;

	[NonSerialized]
	public float roofLightLimitMod;

	[NonSerialized]
	public float floatY;

	[NonSerialized]
	public float floorShadowStrength;

	[NonSerialized]
	public Vector3 _heightMod;

	[NonSerialized]
	public BaseTileMap.ScreenHighlight screenHighlight;

	protected RaycastHit2D[] rays = new RaycastHit2D[1];

	protected BaseTileMap.CardIconMode iconMode;

	protected bool isMining;

	protected bool buildMode;

	protected bool hasBridge;

	protected bool _lowblock;

	protected bool isIndoor;

	public new BaseGameScreen screen;

	protected Map map;

	protected MeshPass pass;

	protected BaseTileSelector selector;

	protected GameSetting.RenderSetting renderSetting;

	protected GameSetting.RenderSetting.ZSetting zSetting;

	protected int liquidLv;

	protected int index;

	protected int totalFire;

	protected int snowColorToken;

	protected int waterAnimeIndex;

	protected int lowWallObjAltitude;

	protected SourceMaterial.Row matBlock;

	protected SourceMaterial.Row matFloor;

	protected SourceMaterial.Row matBridge;

	protected float blockLight;

	protected float floorLight;

	protected float floorLight2;

	protected float light;

	protected float pcMaxLight;

	protected float orgX;

	protected float orgY;

	protected float orgZ;

	protected float roomHeight;

	protected float maxHeight;

	protected float snowLight;

	protected float waterAnimeTimer;

	protected float floatTimer;

	protected float destBrightness;

	protected float lightLimit;

	protected float modSublight1;

	protected float modSublight2;

	protected float shadowStrength;

	protected float _shadowStrength;

	protected float fogBrightness;

	protected float defaultBlockHeight;

	protected float snowLimit;

	protected float snowColor;

	protected float snowColor2;

	protected float nightRatio;

	protected RenderParam param = new RenderParam();

	protected MeshBatch batch;

	protected Vector3 _actorPos;

	protected Vector3 freePos;

	protected int tile;

	protected int floorMatColor;

	protected int height;

	protected int currentHeight;

	protected int pcX;

	protected int pcZ;

	protected int floorDir;

	protected bool roof;

	protected bool isSeen;

	protected bool showAllCards;

	protected bool fogBounds;

	protected bool snowed;

	protected bool isSnowCovered;

	protected bool highlightCells;

	protected bool cinemaMode;

	protected bool alwaysLowblock;

	protected bool showBorder;

	protected Vector3 thingPos;

	protected Vector3 orgPos;

	protected Cell cell;

	protected CellDetail detail;

	protected SourceBlock.Row sourceBlock;

	protected SourceFloor.Row sourceFloor;

	protected SourceFloor.Row sourceBridge;

	protected Room currentRoom;

	protected Room lastRoom;

	protected Room room;

	protected Lot currentLot;

	protected SourceBlock.Row _sourceBlock;

	protected TileType tileType;

	protected SceneLightProfile lightSetting;

	private bool noSlopMode;

	public RoofStyle[] roofStyles;

	public const int DefColor = 104025;

	public const int BlocklightToken = 262144;

	public const int BlocklightMTP = 50;

	public enum WallClipMode
	{
		ByRoom,
		ByLot
	}

	public enum InnerMode
	{
		None,
		InnerBlock,
		Height,
		BuildMode
	}

	public enum CardIconMode
	{
		None,
		Inspect,
		Deconstruct,
		State,
		Visibility
	}

	public enum ScreenHighlight
	{
		None,
		SunMap
	}
}
