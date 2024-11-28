using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CreativeSpore.SuperTilemapEditor;
using UnityEngine;
using UnityEngine.UI;

public class Scene : EMono
{
	public EloMap elomap
	{
		get
		{
			return this.elomapActor.GetEloMap();
		}
	}

	private void Awake()
	{
		base.InvokeRepeating("RefreshActorEx", 0.5f, 0.5f);
	}

	public void TryWarnMacScreen()
	{
		if (EMono.core.config.ignoreParallelsWarning)
		{
			return;
		}
		if (SystemInfo.graphicsDeviceName.Contains("Parallels Display"))
		{
			Dialog.TryWarn("warn_parallels", delegate
			{
				Application.OpenURL(Lang.isJP ? "https://ylvania.org/elin_dev.html" : "https://ylvania.org/elin_dev_e.html");
			}, false);
		}
	}

	public void TryWarnLinuxMod()
	{
		if (EMono.core.config.ignoreLinuxModWarning)
		{
			return;
		}
		Dialog.TryWarn("warn_linuxMod", delegate
		{
			Application.OpenURL(Lang.isJP ? "https://ylvania.org/elin_dev.html" : "https://ylvania.org/elin_dev_e.html");
		}, false);
	}

	public void Init(Scene.Mode newMode)
	{
		Debug.Log("Scene.Init:" + newMode.ToString());
		EMono.ui.RemoveLayers(false);
		BuildMenu.Deactivate();
		ScreenFlash.Reset();
		this.mode = newMode;
		switch (newMode)
		{
		case Scene.Mode.None:
			if (EMono.game != null)
			{
				EMono.game.Kill();
			}
			EMono.ui.RemoveLayers(false);
			this.screenNoMap.Activate();
			break;
		case Scene.Mode.Title:
			SoundManager.bgmVolumeMod = (SoundManager.bgmDumpMod = 0f);
			if (EMono.game != null)
			{
				EMono.game.Kill();
			}
			EMono.ui.AddLayer<LayerTitle>();
			if (!Scene.isAnnounced)
			{
				Scene.isAnnounced = true;
				EMono.ui.AddLayer("LayerAnnounce").SetOnKill(new Action(this.TryWarnMacScreen));
			}
			else
			{
				this.TryWarnMacScreen();
			}
			ActionMode.Title.Activate(true, false);
			this.screenNoMap.Activate();
			break;
		case Scene.Mode.StartGame:
			EMono.ui.RemoveLayer<LayerTitle>();
			EMono.ui.ShowCover(0f, 1f, null, default(Color));
			this.Init(Scene.Mode.Zone);
			break;
		case Scene.Mode.Zone:
			EMono.player.target = null;
			UIBook.str_pc = EMono.pc.NameSimple;
			if (EMono.player.zone == null)
			{
				EMono.player.zone = (EMono.pc.currentZone = EMono.pc.homeZone);
			}
			EMono.core.config.game.ignoreWarnCrime = (EMono.core.config.game.ignoreWarnMana = (EMono.core.config.game.ignoreWarnDisassemble = false));
			EMono.game.updater.Reset();
			CellDetail.count = 0;
			Scene.skipAnime = true;
			EMono.player.baseActTime = EMono.setting.defaultActPace;
			EMono.player.Agent.renderer.isSynced = true;
			EMono.player.Agent.currentZone = EMono.player.zone;
			Point._screen = (EMono.player.zone.IsRegion ? this.screenElona : this.screenElin);
			EMono.player.zone.Activate();
			EMono.player.RefreshEmptyAlly();
			this.UpdateTimeRatio();
			EMono.world.weather.RefreshWeather();
			EMono.screen.Deactivate();
			Popper.scale = (EMono._zone.IsRegion ? new Vector3(1.7f, 1.7f, 1f) : Vector3.one);
			if (!EMono.pc.IsInActiveZone)
			{
				ActionMode.View.Activate(true, false);
			}
			else if (EMono.player.zone is Region)
			{
				ActionMode.Region.Activate(true, false);
			}
			else
			{
				ActionMode.Adv.Activate(true, false);
			}
			if (EMono.game.altCraft)
			{
				if (EMono._zone.IsRegion)
				{
					if (EMono.ui.layerFloat.GetLayer<LayerCraftFloat>(false))
					{
						EMono.ui.layerFloat.RemoveLayer<LayerCraftFloat>();
					}
				}
				else if (!EMono.ui.layerFloat.GetLayer<LayerCraftFloat>(false))
				{
					EMono.ui.layerFloat.AddLayer<LayerCraftFloat>();
				}
			}
			EMono.screen.tileMap.activeCount = 0;
			EMono.screen.SetZoom(EMono.screen.TargetZoom);
			EMono.player.RefreshCurrentHotItem();
			EMono.screen.FocusPC();
			EMono.game.updater.recipe.Build(EMono.pc.pos, RecipeUpdater.Mode.Passive);
			WidgetDate.Refresh();
			WidgetMenuPanel.OnChangeMode();
			EMono.player.hotbars.ResetHotbar(2);
			if (EMono.ui.hud.imageCover.gameObject.activeSelf && !EMono.player.simulatingZone)
			{
				EMono.ui.HideCover(4f, null);
			}
			if (!EMono.core.debug.ignoreAutoSave && EMono.core.config.game.autoSave && EMono.game.countLoadedMaps > 3 && !EMono.player.simulatingZone)
			{
				EMono.game.Save(false, null, false);
			}
			foreach (Thing thing in EMono._map.things)
			{
				if (thing.IsInstalled)
				{
					thing.trait.TryToggle();
				}
			}
			EMono.core.config.ApplyGrading();
			if (EMono.player.onStartZone != null)
			{
				EMono.player.onStartZone();
				EMono.player.onStartZone = null;
			}
			if (ActionMode.Adv.IsActive && (EInput.leftMouse.pressing || EInput.axis != Vector2.zero))
			{
				ActionMode.Adv.SetPressedAction(EInput.leftMouse);
				ActionMode.Adv.RefreshArrow();
				ActionMode.Adv.SetManualMove();
			}
			if (EMono.pc.IsAliveInCurrentZone)
			{
				EMono.pc.RecalculateFOV();
			}
			EMono.player.flags.OnEnterZone();
			this.flock.SetSpawnType(EMono._zone.FlockType);
			if (!EMono.player.simulatingZone)
			{
				if (EMono._zone == EMono.pc.homeZone)
				{
					EMono.pc.faith.Revelation("welcome", 100);
				}
				if (EMono._zone.Boss != null && EMono._zone.Boss.ExistsOnMap)
				{
					Msg.Say("beware", EMono._zone.Name, EMono._zone.Boss.NameBraced, null, null);
				}
				EMono.game.quests.OnEnterZone();
			}
			EInput.keyWait.Consume();
			if (EMono.player.questTracker && !EMono.ui.widgets.GetWidget("QuestTracker"))
			{
				EMono.ui.widgets.Activate("QuestTracker");
			}
			if (EMono._zone is Zone_Town && !EMono._zone.isMapSaved)
			{
				EMono.game.Save(false, null, false);
			}
			break;
		}
		this.etherBlossom.SetActive(this.mode == Scene.Mode.Zone && EMono._zone is Zone_WindRest);
	}

	public void OnKillGame()
	{
		this.actionMode.Deactivate();
		this.mouseTarget.Clear();
		this.hideBalloon = false;
		this.actionMode = null;
		PCC.PurgeCache();
		this.Clear();
		this.flock.Reset();
		this.elomapActor.OnKillGame();
	}

	public void Clear()
	{
		foreach (ISyncScreen syncScreen in this.syncList)
		{
			syncScreen.OnLeaveScreen();
		}
		this.syncList.Clear();
		EffectManager.Instance.KillAll();
		this.ClearActorEx();
		this.DestroyPrefabs();
	}

	public unsafe void OnUpdate()
	{
		SoundManager.speed = (EMono.core.IsGameStarted ? Mathf.Clamp(this.actionMode.gameSpeed * 0.75f, 1f, 2f) : 1f);
		UIButton.UpdateButtons();
		EMono.ui.RefreshActiveState();
		bool isShiftDown = EInput.isShiftDown;
		if (UIDropdown.activeInstance)
		{
			EInput.UpdateOnlyAxis();
		}
		else
		{
			EInput.Update();
		}
		CoreConfig.InputSetting input = EMono.core.config.input;
		if (EInput.middleMouse.pressing)
		{
			if (input.middleClick == CoreConfig.GameFunc.EmuShift || input.middlePressLong == CoreConfig.GameFunc.EmuShift)
			{
				EInput.isShiftDown = true;
			}
			if (input.middleClick == CoreConfig.GameFunc.EmuAlt || input.middlePressLong == CoreConfig.GameFunc.EmuAlt)
			{
				EInput.isAltDown = true;
			}
		}
		if (EInput.mouse3.pressing)
		{
			if (input.mouse3Click == CoreConfig.GameFunc.EmuShift || input.mouse3PressLong == CoreConfig.GameFunc.EmuShift)
			{
				EInput.isShiftDown = true;
			}
			if (input.mouse3Click == CoreConfig.GameFunc.EmuAlt || input.mouse3PressLong == CoreConfig.GameFunc.EmuAlt)
			{
				EInput.isAltDown = true;
			}
		}
		if (EInput.mouse4.pressing)
		{
			if (input.mouse4Click == CoreConfig.GameFunc.EmuShift || input.mouse4PressLong == CoreConfig.GameFunc.EmuShift)
			{
				EInput.isShiftDown = true;
			}
			if (input.mouse4Click == CoreConfig.GameFunc.EmuAlt || input.mouse4PressLong == CoreConfig.GameFunc.EmuAlt)
			{
				EInput.isAltDown = true;
			}
		}
		EInput.hasShiftChanged = (EInput.isShiftDown != isShiftDown);
		Layer.cancelKeyDown = EInput.isCancel;
		Layer.rightClicked = EInput.rightMouse.down;
		int num = EInput.wheel;
		if (num != 0)
		{
			if (EMono.ui.isPointerOverUI)
			{
				if (!EMono.ui.GetLayer<LayerConfig>(false))
				{
					UIDropdown componentOf = InputModuleEX.GetComponentOf<UIDropdown>();
					if (componentOf && !UIDropdown.activeInstance)
					{
						if (num < 0)
						{
							componentOf.Next();
						}
						else if (num > 0)
						{
							componentOf.Prev();
						}
						num = (EInput.wheel = 0);
					}
					Slider componentOf2 = InputModuleEX.GetComponentOf<Slider>();
					if (componentOf2 && EMono.ui.GetLayer<LayerEditPCC>(false))
					{
						SE.Tab();
						componentOf2.value += (componentOf2.wholeNumbers ? ((float)num) : (0.1f * (float)num));
						num = (EInput.wheel = 0);
					}
				}
				if (!DropdownGrid.IsActive)
				{
					DropdownGrid componentOf3 = InputModuleEX.GetComponentOf<DropdownGrid>();
					if (componentOf3)
					{
						if (num < 0)
						{
							componentOf3.Next();
						}
						else if (num > 0)
						{
							componentOf3.Prev();
						}
						num = (EInput.wheel = 0);
					}
				}
				if (EMono.ui.contextMenu.isActive)
				{
					Slider componentOf4 = InputModuleEX.GetComponentOf<Slider>();
					if (componentOf4)
					{
						componentOf4.value += (componentOf4.wholeNumbers ? ((float)num) : (0.1f * (float)num));
						num = (EInput.wheel = 0);
						SE.Tab();
					}
				}
				UIScrollView componentOf5 = InputModuleEX.GetComponentOf<UIScrollView>();
				if (componentOf5)
				{
					num = (EInput.wheel = 0);
					if (componentOf5.verticalScrollbar.isActiveAndEnabled)
					{
						EMono.ui.HideMouseHint(0.2f);
					}
				}
				UIButton componentOf6 = InputModuleEX.GetComponentOf<UIButton>();
				if (componentOf6 && componentOf6.onInputWheel != null)
				{
					componentOf6.onInputWheel(num);
					num = (EInput.wheel = 0);
				}
			}
			if (num != 0)
			{
				Layer layer = EMono.ui.IsActive ? EMono.ui.GetTopLayer() : LayerAbility.Instance;
				if (layer != null && layer.windows.Count > 0)
				{
					Window window = layer.windows[0];
					if (InputModuleEX.IsPointerChildOf(window.rectTab) && !DropdownGrid.IsActive && window.NextTab(num))
					{
						EInput.wheel = 0;
					}
				}
			}
		}
		if ((EInput.leftMouse.down || EInput.rightMouse.down) && WidgetMainText.Instance && WidgetMainText.Instance.box.isShowingLog && !InputModuleEX.IsPointerChildOf(WidgetMainText.Instance))
		{
			WidgetMainText.Instance._ToggleLog();
			EInput.Consume(false, 1);
		}
		Scene.Mode mode = this.mode;
		if (mode != Scene.Mode.Title)
		{
			if (mode != Scene.Mode.Zone)
			{
				this.UpdateCursor();
				EMono.ui.UpdateInput();
			}
			else if (EMono.core.IsGameStarted)
			{
				if (EMono._zone.dirtyElectricity)
				{
					EMono._zone.RefreshElectricity();
				}
				if (EMono.player.target != null && EMono.player.target.isSynced)
				{
					this.srTarget.enabled = true;
					Vector3 position = *EMono.player.target.pos.PositionCenter();
					position.y += EMono.player.target.renderer.data.size.y;
					position.z -= 10f;
					this.srTarget.transform.parent.position = position;
				}
				else
				{
					this.srTarget.enabled = false;
				}
				EMono.player.pickupDelay -= Core.delta;
				if (EMono.player.pickupDelay < 0f)
				{
					EMono.player.pickupDelay = 0f;
				}
				this.actionMode.OnBeforeUpdate();
				this.paused = (EMono.ui.IsPauseGame || this.actionMode.ShouldPauseGame || Game.IsWaiting);
				EMono.player.RefreshEmptyAlly();
				if (!EMono.pc.isDead)
				{
					if (AI_PlayMusic.keepPlaying && EInput.skipFrame <= 0 && Input.anyKeyDown && !Input.GetKey(KeyCode.Tab) && !Input.GetKey(KeyCode.LeftAlt))
					{
						AI_PlayMusic.CancelKeepPlaying();
					}
					this.actionMode.UpdateInput();
				}
				else
				{
					EMono.ui.UpdateInput();
				}
				if (EMono.pc.currentZone != EMono.game.activeZone && EMono.pc.global.transition != null)
				{
					EMono.player.flags.OnLeaveZone();
					if (!LayerDrama.IsActive())
					{
						EMono.player.MoveZone(EMono.pc.currentZone);
					}
				}
				if (EMono.pc.isDead && !EMono.player.deathDialog)
				{
					EMono.player.deathDialog = true;
					EMono.player.returnInfo = null;
					Msg.Say("diebye");
					EMono.Sound.Play("dead_pc2");
					EMono.Sound.Play("dead_pc");
					string[] list = Lang.GetList("lastWords");
					string lastWord = list.RandomItem<string>();
					if (EMono.game.Difficulty.deleteGameOnDeath)
					{
						GameIO.DeleteGame(Game.id, true);
					}
					EMono.ui.CloseLayers();
					if (UIContextMenu.Current)
					{
						UIContextMenu.Current.Hide();
					}
					EInput.haltInput = false;
					Dialog.InputName("dialogLastword", lastWord, delegate(bool cancel, string text)
					{
						if (!cancel)
						{
							lastWord = text;
						}
						Player player = EMono.player;
						player.deathMsg = player.deathMsg + Lang.space + lastWord.Bracket(1);
						if (EMono.core.config.net.enable && EMono.core.config.net.sendEvent)
						{
							Net.SendChat(EMono.pc.NameTitled, EMono.player.deathMsg, ChatCategory.Dead, Lang.langCode);
						}
						Debug.Log(lastWord);
						Msg.SetColor(Msg.colors.Talk);
						Msg.Say(lastWord.Bracket(1));
						List<string> list2 = new List<string>();
						Zone lastTown = EMono.game.spatials.Find(EMono.player.uidLastTown);
						bool addTownRevive = lastTown != null && !EMono._zone.IsInstance;
						if (EMono.game.Difficulty.allowRevive)
						{
							if (addTownRevive)
							{
								list2.Add("pc_revive_town".lang(lastTown.Name, null, null, null, null));
							}
							list2.Add("pc_revive");
							list2.Add("pc_gameover");
						}
						else
						{
							list2.Add("pc_gameover");
						}
						Debug.Log(list2.Count);
						Func<int, string, bool> <>9__3;
						EMono.ui.AddLayer<LayerCover>().SetDuration(1f, delegate
						{
							EMono.ui.CloseLayers();
							string langDetail = "pc_deathChoice".lang();
							ICollection<string> list2 = list2;
							Func<string, string> getString = (string j) => j;
							Func<int, string, bool> onSelect;
							if ((onSelect = <>9__3) == null)
							{
								onSelect = (<>9__3 = delegate(int c, string d)
								{
									EMono.player.deathDialog = false;
									if (EMono.game.Difficulty.allowRevive && (c == 0 || (addTownRevive && c == 1)))
									{
										EMono.pc.MakeGrave(lastWord);
										EMono.pc.Revive(null, false);
										EMono.pc.RecalculateFOV();
										Zone zone = EMono.player.spawnZone ?? EMono.pc.homeZone;
										if (addTownRevive && c == 0)
										{
											zone = lastTown;
										}
										if (EMono.game.activeZone == zone)
										{
											EMono.pc.Teleport(EMono._zone.GetSpawnPos(EMono.pc, ZoneTransition.EnterState.Return), true, true);
										}
										else
										{
											EMono.player.deathZoneMove = true;
											EMono.pc.MoveZone(zone, ZoneTransition.EnterState.Return);
										}
										EMono.screen.FocusPC();
										Msg.Say("crawlup");
										if (EMono.player.stats.death == 0)
										{
											Tutorial.Reserve("death", null);
										}
										EMono.player.stats.death++;
									}
									else
									{
										EMono.game.GotoTitle(false);
									}
									return true;
								});
							}
							Dialog.List<string>(langDetail, list2, getString, onSelect, false);
						});
					}, Dialog.InputType.Default);
				}
				this.UpdateCursor();
				this.UpdateTimeRatio();
				EMono.Sound.UpdateBGM();
				EMono.game.updater.Update();
				if (WidgetHotbar.dirtyCurrentItem)
				{
					EMono.player.RefreshCurrentHotItem();
				}
				if (WidgetEquip.dirty)
				{
					WidgetEquip.Redraw();
				}
				if (EMono.player.wasDirtyWeight)
				{
					foreach (Chara chara in EMono.pc.party.members)
					{
						chara.CalcBurden();
					}
					EMono.player.wasDirtyWeight = false;
				}
				if (!EMono.ui.IsActive)
				{
					EMono.core.screen.tileSelector.OnUpdate();
				}
				else if (EMono.ui.mouseInfo.gameObject.activeSelf)
				{
					EMono.ui.mouseInfo.SetText("");
				}
				EMono.screen.Draw();
				this.actionMode.OnAfterUpdate();
				if (EMono.player.lastTransition != null && !EMono.player.simulatingZone)
				{
					if (!EMono.ui.GetLayer<LayerDrama>(false))
					{
						ZoneTransition.EnterState state = EMono.player.lastTransition.state;
						if (state != ZoneTransition.EnterState.Encounter)
						{
							switch (state)
							{
							case ZoneTransition.EnterState.PortalReturn:
							case ZoneTransition.EnterState.Return:
							case ZoneTransition.EnterState.Teleport:
							case ZoneTransition.EnterState.Moongate:
								EMono.pc.PlayEffect("teleport", true, 0f, default(Vector3));
								EMono.pc.PlaySound("return", 1f, true);
								goto IL_922;
							case ZoneTransition.EnterState.Elevator:
								EMono.pc.PlaySound("elevator", 1f, true);
								goto IL_922;
							case ZoneTransition.EnterState.Fall:
								EMono.pc.PlaySound("fall", 1f, true);
								EMono.pc.PlayAnime(AnimeID.FallSky, false);
								EMono.pc.DamageHP((20 + EMono.rnd(30) + ((EMono.player.lastTransition.lastZone is Zone_Casino) ? 1000 : 0)) / (EMono.pc.IsLevitating ? 10 : 1), AttackSource.Fall, null);
								goto IL_922;
							}
							SE.MoveZone();
						}
					}
					IL_922:
					bool flag = (EMono.rnd(5) == 0 && EMono.pc.burden.GetPhase() >= 3) || EMono.pc.burden.GetPhase() >= 4;
					if (EMono.player.lastTransition.lastZone is Region)
					{
						flag = false;
					}
					if (flag)
					{
						EMono.pc.Stumble(100);
					}
					EMono.player.lastTransition = null;
					return;
				}
			}
			return;
		}
		if (EMono.scene.elomapActor.IsActive)
		{
			this.timeRatio = 0f;
			EMono.screen.UpdateShaders(0f);
		}
		this.godray.SetActive(false);
		this.UpdateCursor();
		EMono.ui.UpdateInput();
	}

	public void OnLateUpdate()
	{
		this.DrawPass();
		CursorSystem.Instance.Draw();
	}

	public void UpdateCursor()
	{
		CursorSystem.position = EInput.mposWorld;
		CursorSystem.position.z = -100f;
		if (EMono.core.IsGameStarted)
		{
			CursorSystem.posOrigin = EMono.screen.position.SetZ(-100f);
			this.actionMode.OnUpdateCursor();
			return;
		}
		CursorSystem.SetCursor(null, 0);
	}

	public void UpdateTimeRatio()
	{
		float num = (EMono._map.config.hour != -1) ? ((float)EMono._map.config.hour) : ((float)EMono.world.date.hour + (float)EMono.world.date.min / 60f);
		if (num > 12f)
		{
			num = 24f - num;
		}
		this.timeRatio = Mathf.Clamp(num * 100f / 24f * 0.01f + EMono.setting.dayRatioMod, 0f, 1f);
	}

	public void OnChangeHour()
	{
		this.UpdateTimeRatio();
		this.firefly.OnChangeHour();
		this.fireflyNight.OnChangeHour();
		this.star.OnChangeHour();
		if (EMono.screen.pcOrbit)
		{
			EMono.screen.pcOrbit.OnChangeMin();
		}
		EMono.screen.RefreshSky();
		EMono.screen.OnChangeHour();
		foreach (Transform transform in EMono.ui.rectDynamic.GetComponentsInDirectChildren(true))
		{
			if (transform.gameObject.tag != "IgnoreDestroy" && !transform.GetComponent<TC>())
			{
				UnityEngine.Object.Destroy(transform.gameObject);
			}
		}
	}

	public void RefreshBG()
	{
		this.bg = EMono.setting.render.bgs[EMono._map.config.bg];
		if (EMono._map.config.bg == MapBG.Sky && EMono._zone.IsSnowZone)
		{
			this.bg = EMono.setting.render.bgs[MapBG.Snow];
		}
		this.skyBG.SetActive(this.bg.skyBox);
		this.skyBG.sharedMaterial = this.bg.mat;
		this.skyPlane.SetActive(this.bg.plane);
		this.skyPlane.sharedMaterial = this.bg.mat;
		this.cam.clearFlags = ((!EMono.core.config.graphic.alwaysClearCamera && this.bg.skyBox) ? CameraClearFlags.Depth : CameraClearFlags.Color);
	}

	public void FocusAndPause(Point pos, int a)
	{
		if (a >= 1)
		{
			EMono.core.screen.Focus(pos);
		}
		if (a >= 2)
		{
			ActionMode.Sim.Pause(false);
		}
	}

	public void ApplyZoneConfig()
	{
		if (!EMono.core.IsGameStarted)
		{
			return;
		}
		Scene.<>c__DisplayClass73_0 CS$<>8__locals1;
		CS$<>8__locals1.conf = EMono._map.config;
		LiquidProfile.Apply(CS$<>8__locals1.conf.idLiquid);
		RefractionProfile.Apply(CS$<>8__locals1.conf.idRefraction);
		BaseTileMap tileMap = this.screenElin.tileMap;
		Scene.<ApplyZoneConfig>g__SetOcean|73_0(tileMap.passLiquid.mat, ref CS$<>8__locals1);
		Scene.<ApplyZoneConfig>g__SetOcean|73_0(tileMap.passFloorWater.mat, ref CS$<>8__locals1);
		Scene.<ApplyZoneConfig>g__SetOcean|73_0(tileMap.passAutoTileWater.mat, ref CS$<>8__locals1);
		ScreenGrading grading = this.camSupport.grading;
		ScreenGradingProfile.Lut lut = grading.lut;
		grading.profile.funcOverlay = (() => EMono.scene.profile.overlay);
		if (CS$<>8__locals1.conf.idLut == "None")
		{
			lut.tex = null;
		}
		else
		{
			lut.tex = Resources.Load<Texture2D>("Scene/Profile/Lut/" + CS$<>8__locals1.conf.idLut);
			lut.Brightness = CS$<>8__locals1.conf.lutBrightness;
			lut.Saturation = CS$<>8__locals1.conf.lutSaturation;
			lut.Contrast = CS$<>8__locals1.conf.lutContrast;
			lut.blend = CS$<>8__locals1.conf.lutBlend;
		}
		grading.SetGrading();
	}

	public void OnToggle()
	{
		SE.ClickGeneral();
		WidgetSystemIndicator.Refresh();
		WidgetHotbar.RefreshHighlights();
	}

	public void ToggleShowRoof()
	{
		EMono.game.config.showRoof = !EMono.game.config.showRoof;
		this.OnToggle();
	}

	public void ToggleLight()
	{
		EMono.game.config.buildLight = !EMono.game.config.buildLight;
		this.OnToggle();
	}

	public void ToggleSnapFreePos()
	{
		if (!EMono.core.config.game.freePos)
		{
			SE.Beep();
			return;
		}
		EMono.game.config.snapFreePos = !EMono.game.config.snapFreePos;
		this.OnToggle();
	}

	public void ToggleFreePos()
	{
		if (!EMono.core.config.game.freePos)
		{
			SE.Beep();
			return;
		}
		EMono.game.config.freePos = !EMono.game.config.freePos;
		this.OnToggle();
	}

	public void ToggleShowWall()
	{
		EMono.game.config.showWall = !EMono.game.config.showWall;
		this.OnToggle();
	}

	public void ToggleRoof()
	{
		EMono.game.config.noRoof = !EMono.game.config.noRoof;
		this.OnToggle();
	}

	public void ToggleSlope()
	{
		EMono.game.config.slope = !EMono.game.config.slope;
		this.OnToggle();
		Scene.skipAnime = true;
	}

	public void ToggleTilt()
	{
		if (EMono._zone.IsRegion)
		{
			EMono.game.config.tiltRegion = !EMono.game.config.tiltRegion;
		}
		else
		{
			EMono.game.config.tilt = !EMono.game.config.tilt;
		}
		EMono.screen.RefreshTilt();
		this.OnToggle();
	}

	public void ToggleHighlightArea()
	{
		EMono.game.config.highlightArea = !EMono.game.config.highlightArea;
		this.OnToggle();
	}

	public void ToggleBirdView(bool sound = true)
	{
		SE.ClickGeneral();
		if (EMono.scene.actionMode == ActionMode.Bird)
		{
			EMono.scene.actionMode.Deactivate();
			return;
		}
		ActionMode.Bird.Activate(true, false);
	}

	public void ToggleBalloon()
	{
		this.hideBalloon = !this.hideBalloon;
		EMono.ui.ShowBalloon(!this.hideBalloon);
		SE.ClickGeneral();
		WidgetSystemIndicator.Refresh();
	}

	public void ToggleMuteBGM()
	{
		EMono.Sound.ToggleMute(EMono.core.config.sound.volumeBGM);
		SE.ClickGeneral();
		WidgetSystemIndicator.Refresh();
		if (!EMono.Sound.muteBGM)
		{
			EMono.core.config.ApplyVolume();
		}
	}

	public void RebuildActorEx()
	{
		foreach (Thing thing in EMono._map.things)
		{
			if (!thing.trait.IDActorEx.IsEmpty())
			{
				this.AddActorEx(thing, null);
			}
		}
	}

	public void RefreshActorEx()
	{
		SoundManager.bgmDumpMod = 0f;
		foreach (ActorEx actorEx in this.dictActorEx.Values)
		{
			actorEx.Refresh();
		}
	}

	public void ClearActorEx()
	{
		foreach (ActorEx actorEx in this.dictActorEx.Values)
		{
			actorEx.Kill();
		}
		this.dictActorEx.Clear();
	}

	public void AddActorEx(Card c, Action<ActorEx> onBeforeSetOwner = null)
	{
		ActorEx actorEx = this.dictActorEx.TryGetValue(c, null);
		if (actorEx != null)
		{
			return;
		}
		actorEx = Util.Instantiate<ActorEx>("Scene/Render/Actor/Ex/" + c.trait.IDActorEx, null);
		if (onBeforeSetOwner != null)
		{
			onBeforeSetOwner(actorEx);
		}
		actorEx.SetOwner(c);
		this.dictActorEx.Add(c, actorEx);
	}

	public void RemoveActorEx(Card c)
	{
		ActorEx actorEx = this.dictActorEx.TryGetValue(c, null);
		if (actorEx == null)
		{
			return;
		}
		actorEx.Kill();
		this.dictActorEx.Remove(c);
	}

	public void InitPass()
	{
		MeshPass[] array = this.passes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Init();
		}
		foreach (ModItem<Sprite> modItem in MOD.sprites.list)
		{
			Sprite sprite = SpriteSheet.Get(modItem.id);
			if (sprite)
			{
				Sprite @object = modItem.GetObject(new SpriteLoadOption
				{
					pivot = new Vector2(sprite.pivot.x / (sprite.bounds.size.x * 100f), sprite.pivot.y / (sprite.bounds.size.y * 100f))
				});
				SpriteSheet.dict[modItem.id] = @object;
			}
		}
	}

	public void DrawPass()
	{
		if (this.cam.enabled)
		{
			MeshPass[] array = this.passes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Draw();
			}
		}
		else
		{
			MeshPass[] array = this.passes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].DrawEmpty();
			}
		}
		this.RefreshSync();
		if (EMono.screen.guide)
		{
			EMono.screen.guide.OnDrawPass();
		}
	}

	public void RefreshSync()
	{
		for (int i = this.syncList.Count - 1; i >= 0; i--)
		{
			ISyncScreen syncScreen = this.syncList[i];
			if (syncScreen.Sync != this.syncFrame)
			{
				syncScreen.OnLeaveScreen();
				this.syncList.RemoveAt(i);
			}
		}
		this.syncFrame += 1L;
		RenderObject.syncFrame = this.syncFrame;
	}

	public Transform LoadPrefab(string id)
	{
		Transform transform = Util.Instantiate<Transform>("Scene/Prefab/" + id, null);
		transform.name = id;
		this.loadedPrefabs.Add(transform.gameObject);
		return transform;
	}

	public void DestroyPrefabs()
	{
		foreach (GameObject obj in this.loadedPrefabs)
		{
			UnityEngine.Object.Destroy(obj);
		}
		this.loadedPrefabs.Clear();
	}

	public void DestroyPrefab(string id)
	{
		GameObject gameObject = GameObject.Find(id);
		if (gameObject)
		{
			UnityEngine.Object.DestroyImmediate(gameObject);
		}
	}

	[CompilerGenerated]
	internal static void <ApplyZoneConfig>g__SetOcean|73_0(Material m, ref Scene.<>c__DisplayClass73_0 A_1)
	{
		m.SetFloat("_GradientWater", EMono.core.config.graphic.gradientWater ? EMono.core.config.graphic.gradientWaterLevel : 0f);
		if (A_1.conf.seaDir != 0)
		{
			m.EnableKeyword("WAVE_ON");
			Vector4[] array = new Vector4[]
			{
				new Vector4(-2f, -1f, -1f, 1f),
				new Vector4(1f, 0f, 1f, 1f),
				new Vector4(2f, 1f, -1f, 1f),
				new Vector4(-1f, 0f, 1f, 1f)
			};
			m.SetVector("_WaveDir", array[A_1.conf.seaDir - 1]);
			return;
		}
		m.DisableKeyword("WAVE_ON");
	}

	public Vector2[] test;

	public static bool isAnnounced;

	public static Point HitPoint = new Point();

	public static Point ClickPoint = new Point();

	public static bool skipAnime;

	public Transform transFocus;

	public Transform transBlizzard;

	public Transform transAudio;

	public Vector3 posAudioListener;

	public AudioListener audioListener;

	public Camera cam;

	public Camera camTreasure;

	public CameraSupport camSupport;

	public VFX firefly;

	public VFX fireflyNight;

	public VFX star;

	public ParticleSystem rain;

	public ParticleSystem snow;

	public ParticleSystem ether;

	public ParticleSystem godray;

	public ParticleSystem godrayDust;

	public ParticleSystem blossom;

	public ParticleSystem etherBlossom;

	public ParticleSystem[] blossoms;

	public ParticleSystem[] blizzards;

	public FlockController flock;

	public MeshRenderer skyBG;

	public MeshRenderer skyPlane;

	public SceneProfile profile;

	public CameraFilterPack_Atmosphere_Rain filterRain;

	public ActionMode actionMode;

	public SoundSource sfxRain;

	public SoundSource sfxWind;

	public SoundSource sfxSea;

	public SoundSource sfxFire;

	public SoundSource sfxEther;

	public GameScreen screenElin;

	public GameScreenElona screenElona;

	public GameScreenNoMap screenNoMap;

	public PopperManager popper;

	public EloMapActor elomapActor;

	public SpriteRenderer srTarget;

	public Tileset tileset;

	public ParticleSystem psFoot;

	public ParticleSystem psSmoke;

	public ParticleSystem psRainSplash;

	public ParticleSystem psRainSplashWater;

	public MeshPass[] passes;

	[NonSerialized]
	public GameSetting.RenderSetting.MapBGSetting bg;

	[NonSerialized]
	public Scene.Mode mode;

	[NonSerialized]
	public float timeRatio;

	[NonSerialized]
	public List<GameObject> loadedPrefabs = new List<GameObject>();

	public PointTarget mouseTarget = new PointTarget();

	public List<ISyncScreen> syncList = new List<ISyncScreen>();

	[NonSerialized]
	public bool paused;

	public long syncFrame;

	public Dictionary<Card, ActorEx> dictActorEx = new Dictionary<Card, ActorEx>();

	[NonSerialized]
	public bool hideBalloon;

	public enum Mode
	{
		None,
		Title,
		StartGame,
		Zone
	}
}
