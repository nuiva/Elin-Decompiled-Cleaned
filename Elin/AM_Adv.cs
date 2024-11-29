using System;
using System.Collections.Generic;
using UnityEngine;

public class AM_Adv : AM_BaseGameMode
{
	public PointTarget mouseTarget
	{
		get
		{
			return EClass.scene.mouseTarget;
		}
	}

	public bool autorun
	{
		get
		{
			return EClass.core.config.input.autorun;
		}
	}

	public bool ShouldHideTile
	{
		get
		{
			return EClass.ui.layers.Count > 0 || (EClass.pc.renderer.IsMoving && !EInput.rightMouse.pressing) || EClass.pc.ai.Current is AI_Goto || this.cursorMove || EClass.ui.isPointerOverUI || EInput.axis != Vector2.zero;
		}
	}

	public override float gameSpeed
	{
		get
		{
			if (!this.ShouldPauseGame)
			{
				return ActionMode.GameSpeeds[EClass.game.gameSpeedIndex] * ((AM_Adv.turbo != 0f) ? AM_Adv.turbo : 1f);
			}
			return 1f;
		}
	}

	public override bool FixFocus
	{
		get
		{
			return !this.zoomOut && (EClass.core.config.test.alwaysFixCamera || EInput.leftMouse.pressing || !EInput.rightMouse.pressing || AM_Adv.actCount <= 0 || (EClass.pc.ai != Chara._NoGoalRepeat && (!(EClass.pc.ai is TaskDesignation) || !(EClass.pc.ai as TaskDesignation).isRepeated)));
		}
	}

	public override bool ShouldPauseGame
	{
		get
		{
			return EClass.core.config.game.autopause && EClass.pc.HasNoGoal;
		}
	}

	public override bool AllowWheelZoom
	{
		get
		{
			return false;
		}
	}

	public override float TargetZoom
	{
		get
		{
			if (!this.zoomOut2)
			{
				return 0.01f * (float)EClass.game.config.defaultZoom;
			}
			return 0.01f * (float)EClass.game.config.zoomedZoom;
		}
	}

	public override bool HighlightWall(Point p)
	{
		return EClass.pc.held != null && TaskMine.CanMine(p, EClass.pc.held);
	}

	public unsafe override void OnActivate()
	{
		if (WidgetMouseover.Instance)
		{
			WidgetMouseover.Instance.Hide(false);
		}
		ActionMode.DefaultMode = this;
		this.pressedAction.Init(null);
		EClass.pc.ai.Cancel();
		EClass.screen.tileMap.RefreshHeight();
		(EClass.pc.renderer as CharaRenderer).first = true;
		EClass.pc.renderer.SetFirst(true, *EClass.pc.pos.PositionCenter());
		EClass.screen.FocusPC();
		EClass.screen.RefreshPosition();
	}

	public override void OnDeactivate()
	{
		this.EndTurbo();
	}

	public unsafe override void OnRenderTile(Point point, HitResult result, int dir)
	{
		if (this.ShouldHideTile)
		{
			return;
		}
		EClass.player.currentHotItem.OnRenderTile(point, result, dir);
		base.OnRenderTile(point, result, dir);
		if (EClass.game.config.highlightArea && point.Installed != null && (EClass._zone.IsPCFaction || EClass._zone is Zone_Tent))
		{
			point.Installed.trait.OnRenderTile(point, result, dir);
		}
		if (EClass.core.config.game.highlightEnemy)
		{
			using (List<Chara>.Enumerator enumerator = point.ListCharas().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsHostile(EClass.pc))
					{
						using (List<Chara>.Enumerator enumerator2 = EClass._map.charas.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								Chara chara = enumerator2.Current;
								if (chara.isSynced && chara.IsHostile(EClass.pc) && !chara.IsMultisize)
								{
									Vector3 vector = *chara.pos.Position();
									EClass.screen.guide.passGuideFloor.Add(vector.x, vector.y, vector.z - 0.01f, 23f, 0.3f);
								}
							}
							break;
						}
					}
				}
			}
		}
	}

	public override int GetDefaultTile(Point p)
	{
		if (!p.IsSync)
		{
			return 30;
		}
		return 0;
	}

	public override void OnBeforeUpdate()
	{
		if (EClass.pc.renderer.IsMoving)
		{
			this.isMoving = true;
			return;
		}
		this.isMoving = false;
	}

	public unsafe override void OnAfterUpdate()
	{
		Vector3 vector = this.mouseTarget.pos.IsValid ? (*this.mouseTarget.pos.PositionAuto()) : (*EClass.pc.pos.PositionAuto());
		vector = Camera.main.WorldToScreenPoint(vector);
		vector.z = 0f;
		vector += EClass.ui.hud.transRightPos * Mathf.Min(0f, EClass.screen.Zoom - 1f);
		EClass.ui.hud.transRight.position = vector;
		EClass.player.MarkMapHighlights();
	}

	public override void OnUpdateCursor()
	{
		this.RefreshArrow();
		CursorInfo info = (EClass.ui.layers.Count == 0 && !EClass.ui.isPointerOverUI) ? CursorSystem.Instance.arrows[this.arrowIndex] : null;
		CursorSystem.leftIcon = null;
		bool flag = (EClass.pc.ai.IsRunning || AM_Adv.turbo != 0f || this.pressedAction.action != null) && !EClass.pc.HasNoGoal && !(EClass.pc.ai is GoalManualMove);
		if (flag && !EClass.ui.isPointerOverUI)
		{
			CursorSystem.leftIcon = CursorSystem.IconGear;
			CursorSystem.leftIconAngle = (float)((int)(this.gearAngle / 45f) * 45);
		}
		if (!this.cursorMove && (this.mouseTarget.hasTargetChanged || !this.mouseTarget.pos.Equals(this.planLeft.pos) || this.planLeft.performed || this.planRight.performed))
		{
			this.updatePlans = true;
		}
		if (!this.ShouldHideTile)
		{
			if (this.planLeft.HasAct)
			{
				info = this.planLeft.CursorIcon;
			}
			else if (this.planRight.HasAct)
			{
				info = this.planRight.CursorIcon;
			}
		}
		this.gearAngle += Core.gameDelta * 200f;
		CursorSystem.SetCursor(info, 0);
		if ((this.updatePlans && !flag) || EClass.pc.IsDisabled)
		{
			this.UpdatePlans();
		}
		if (CursorSystem.ignoreCount < 0)
		{
			this.UpdateLangWheel();
			EClass.ui.hud.transRight.SetActive(true);
			bool flag2 = !this.ShouldHideTile && !this.pressedAction.hideRightInfo && !EClass.ui.contextMenu.isActive;
			EClass.ui.hud.textLeft.SetActive(flag2 && this.planLeft.ShowAct);
			EClass.ui.hud.textRight.SetActive(flag2 && this.planRight.ShowAct);
			EClass.ui.hud.textMiddle.SetActive(flag2 && this.textMiddle != null);
			EClass.ui.hud.textWheel.SetActive(flag2 && this.textWheel != null);
			if (EClass.ui.hud.textLeft.gameObject.activeSelf)
			{
				EClass.ui.hud.textLeft.rectTransform.anchoredPosition = EClass.ui.hud.leftTextPos * Mathf.Max(1f, EClass.screen.Zoom) / EClass.core.uiScale;
			}
			if (EClass.ui.hud.textRight.gameObject.activeSelf)
			{
				EClass.ui.hud.textRight.rectTransform.anchoredPosition = EClass.ui.hud.rightTextPos * Mathf.Max(1f, EClass.screen.Zoom) / EClass.core.uiScale;
			}
			if (EClass.ui.hud.textMiddle.gameObject.activeSelf)
			{
				EClass.ui.hud.textMiddle.rectTransform.anchoredPosition = EClass.ui.hud.wheelTextPos * Mathf.Max(1f, EClass.screen.Zoom) / EClass.core.uiScale;
			}
			if (EClass.ui.hud.textWheel.gameObject.activeSelf)
			{
				EClass.ui.hud.textWheel.rectTransform.anchoredPosition = (EClass.ui.hud.textMiddle.gameObject.activeSelf ? EClass.ui.hud.wheelTextPos2 : EClass.ui.hud.wheelTextPos) * Mathf.Max(1f, EClass.screen.Zoom) / EClass.core.uiScale;
			}
			EClass.ui.hud.imageRight.SetActive(flag2 && this.planRight.HasAct);
		}
	}

	public void UpdatePlans()
	{
		this.planLeft.Update(this.mouseTarget);
		this.planRight.Update(this.mouseTarget);
		this.updatePlans = false;
	}

	public void UpdateLangWheel()
	{
		this.textWheel = null;
		this.textMiddle = null;
		if (this.planRight.HasAct)
		{
			if (EClass.scene.mouseTarget.pos.Distance(EClass.pc.pos) <= 1 && HotItemHeld.CanChangeHeightByWheel())
			{
				this.textWheel = "textWheel_changeHeight".lang();
			}
			else if (HotItemHeld.CanRotate())
			{
				this.textMiddle = "textMiddle_rotate".lang(EInput.keys.mouseMiddle.key.ToString() ?? "", null, null, null, null);
			}
			EClass.ui.hud.textWheel.SetText(this.textWheel.IsEmpty(""));
			EClass.ui.hud.textMiddle.SetText(this.textMiddle.IsEmpty(""));
		}
	}

	public void ClearPlans()
	{
		this.planLeft.Clear();
		this.planRight.Clear();
		this.updatePlans = true;
	}

	public void OnBecomeNoGoal()
	{
		EClass.player.renderThing = null;
		if (EClass.player.hotItemToRestore != null)
		{
			if (EClass.player.currentHotItem != EClass.player.hotItemToRestore)
			{
				EClass.player.SetCurrentHotItem(null);
			}
			EClass.player.hotItemToRestore = null;
		}
		if (!EClass.core.config.game.alwaysUpdateRecipe)
		{
			EClass.game.updater.recipe.Build(EClass.pc.pos, RecipeUpdater.Mode.Passive);
		}
		if (EClass.rnd(30) == 0 && EClass.pc.HasElement(1559, 1))
		{
			List<Thing> list = EClass.pc.things.List((Thing a) => a.trait is TraitPotion, false);
			if (list.Count > 0)
			{
				EClass.pc.Drink(list.RandomItem<Thing>());
				EClass.player.EndTurn(true);
			}
		}
		if ((int)(Time.timeSinceLevelLoad / 60f / 60f) > Player.realHour)
		{
			EClass.player.OnAdvanceRealHour();
		}
		Tutorial.TryPlayReserve();
	}

	public override void _OnUpdateInput()
	{
		if (EClass.debug.enable)
		{
			EClass.player.flags.debugEnabled = true;
		}
		if (EClass.player.willAutoSave)
		{
			EClass.game.Save(true, null, false);
			EClass.player.willAutoSave = false;
		}
		if (RecipeUpdater.dirty)
		{
			RecipeUpdater.dirty = false;
			EClass.game.updater.recipe.Build(EClass.pc.pos, RecipeUpdater.Mode.Passive);
		}
		if (!EInput.rightMouse.pressing)
		{
			AM_Adv.actCount = 0;
		}
		if (EInput.rightMouse.pressing && EInput.rightMouse.pressedLong && !EInput.leftMouse.down && EClass.pc.ai is TaskDesignation && (EClass.pc.ai as TaskDesignation).isRepeated)
		{
			if (!EInput.rightMouse.pressing || !EInput.leftMouse.pressing)
			{
				this.rightMouseTimer = 2.5f;
			}
		}
		else
		{
			this.rightMouseTimer -= Core.delta;
		}
		EClass.player.waitingInput = false;
		if (EClass.pc.ai is GoalAutoCombat && EClass.pc.ai.IsRunning)
		{
			if (!EClass.debug.enable && (EClass.pc.IsCriticallyWounded(false) || (EClass.game.config.autoCombat.abortOnHalfHP && ((EClass.pc.Evalue(1421) > 0) ? (EClass.pc.hp + EClass.pc.mana.value) : EClass.pc.hp) < EClass.player.autoCombatStartHP / 2)))
			{
				Msg.Say("abort_lowHP");
				EClass.pc.ai.Cancel();
			}
			else if (!EClass.debug.enable && EClass.game.config.autoCombat.abortOnAllyDying && EClass.pc.party.IsCriticallyWounded(false))
			{
				Msg.Say("abort_allyDying");
				EClass.pc.ai.Cancel();
			}
			else if (EClass.game.config.autoCombat.abortOnItemLoss && this.itemLost > 0)
			{
				Msg.Say("abort_itemLoss");
				EClass.pc.ai.Cancel();
			}
		}
		this.itemLost = 0;
		if (EClass.pc.HasNoGoal)
		{
			if (EClass.pc.WillConsumeTurn())
			{
				EClass.player.EndTurn(true);
				EClass.player.invlunerable = false;
				return;
			}
			if (EClass.player.lastTurn != EClass.pc.turn)
			{
				EClass.player.lastTurn = EClass.pc.turn;
				EClass.player.invlunerable = false;
				this.OnBecomeNoGoal();
				if (!EClass.pc.HasNoGoal)
				{
					return;
				}
			}
			EClass.player.waitingInput = true;
			if (AI_PlayMusic.keepPlaying)
			{
				Thing playingTool = AI_PlayMusic.playingTool;
				if (EInput.IsAnyKeyDown(true, true) || (playingTool.GetRootCard() != EClass.pc && (!playingTool.ExistsOnMap || playingTool.Dist(EClass.pc) > 1)))
				{
					AI_PlayMusic.CancelKeepPlaying();
					return;
				}
				UISong instance = UISong.Instance;
				if (!instance || instance.ratio > 0.85f)
				{
					EClass.pc.SetAIImmediate(new AI_PlayMusic
					{
						tool = playingTool
					});
					return;
				}
			}
		}
		if (EClass.player.returnInfo != null && EClass.player.returnInfo.askDest)
		{
			EClass.player.returnInfo.askDest = false;
			int uidDest = 0;
			List<Zone> list = EClass.game.spatials.ListReturnLocations();
			if (list == null || list.Count == 0)
			{
				EClass.player.returnInfo = null;
				Msg.Say("returnAbort");
				return;
			}
			EClass.ui.AddLayer<LayerList>().SetList2<Zone>(list, (Zone a) => a.NameWithDangerLevel, delegate(Zone a, ItemGeneral b)
			{
				uidDest = a.uid;
				if (a is Zone_Dungeon)
				{
					uidDest = a.FindDeepestZone().uid;
				}
				EClass.player.returnInfo = new Player.ReturnInfo
				{
					turns = EClass.rnd(10) + 10,
					uidDest = uidDest
				};
				if (EClass.debug.instaReturn)
				{
					EClass.player.returnInfo.turns = 1;
					EClass.player.EndTurn(true);
				}
			}, delegate(Zone a, ItemGeneral b)
			{
				string lang = (a is Zone_Dungeon) ? a.TextDeepestLv : "surface".lang();
				b.SetSubText(lang, 200, FontColor.Default, TextAnchor.MiddleRight);
				b.Build();
				b.button1.mainText.rectTransform.sizeDelta = new Vector2(350f, 20f);
			}, true).SetSize(500f, -1f).SetOnKill(delegate
			{
				if (uidDest == 0)
				{
					EClass.player.returnInfo = null;
					Msg.Say("returnAbort");
				}
			}).SetTitles("wReturn", null);
			return;
		}
		else
		{
			if (EClass.player.haltMove)
			{
				EClass.player.haltMove = false;
				this.TryCancelInteraction(false);
				EInput.Consume(1);
				return;
			}
			if (this.keepWalking && (EInput.leftMouse.down || EInput.rightMouse.down))
			{
				this.keepWalking = false;
				EInput.Consume(1);
				return;
			}
			if (EClass.player.showShippingResult && !EClass.ui.IsActive)
			{
				EClass.player.showShippingResult = false;
				EClass.ui.AddLayer<LayerShippingResult>().Show();
			}
			if (this.pressedAction.IsPressing() || this.keepWalking)
			{
				this.pressedAction.timer += Core.delta;
				if (this.pressedAction.action != null)
				{
					if (this.pressedAction.canTurbo && ((this.autorun && this.pressedAction.timer > 0.45f) || Input.GetKey(KeyCode.LeftShift)))
					{
						this.SetTurbo(-1);
					}
					if (!this.pressedAction.waitForTurn || this.CanAct())
					{
						if (this.pressedAction.count == 1 && !this.pressedAction.ignoreCount)
						{
							this.pressedAction.timerRepeat += Core.delta;
							if (this.pressedAction.timerRepeat < 0.3f)
							{
								return;
							}
						}
						this.pressedAction.count++;
						if (this.pressedAction.action())
						{
							if (this.pressedAction.willEndTurn)
							{
								EClass.player.EndTurn(false);
								if (!this.pressedAction.repeat)
								{
									this.pressedAction.action = null;
									return;
								}
							}
						}
						else if (!this.pressedAction.repeat)
						{
							this.pressedAction.action = null;
							return;
						}
					}
				}
				else if ((this.autorun && this.pressedAction.timer > 0.5f) || Input.GetKey(KeyCode.LeftShift))
				{
					this.SetTurbo(-1);
				}
				return;
			}
			if (this.pressedAction.button != null || EInput.hasShiftChanged)
			{
				this.updatePlans = true;
			}
			this.cursorMove = false;
			EClass.player.nextMove = Vector2.zero;
			if (EClass.pc.HasNoGoal)
			{
				this.EndTurbo();
			}
			this.pressedAction.button = null;
			this.pressedAction.axis = Vector2.zero;
			this.pressedAction.count = 0;
			this.pressedAction.timerRepeat = 0f;
			if (EClass.player.willEndTurn)
			{
				EClass.player.willEndTurn = false;
				EClass.player.EndTurn(true);
				return;
			}
			if (EInput.axis != Vector2.zero)
			{
				this.AxisMove();
				return;
			}
			this.timerStartRunning = 0f;
			this.startedRun = false;
			if (this.movedByKey && EClass.pc.ai is GoalManualMove)
			{
				EClass.pc.ai.Cancel();
			}
			this.movedByKey = false;
			CoreConfig.InputSetting input = EClass.core.config.input;
			if (EInput.leftMouse.down)
			{
				if (this.TryCancelInteraction(true))
				{
					return;
				}
				CursorSystem.ignoreCount = 5;
				this.SetPressedAction(EInput.leftMouse);
			}
			if (EInput.rightMouse.down)
			{
				if (this.TryCancelInteraction(true))
				{
					return;
				}
				CursorSystem.ignoreCount = 5;
				this.SetPressedAction(EInput.rightMouse);
			}
			if (EInput.middleMouse.pressing || EInput.isShiftDown)
			{
				if (HotItemHeld.CanChangeHeightByWheel())
				{
					if (EInput.wheel != 0)
					{
						ActionMode.Build.ModAltitude(EInput.wheel);
						SE.Tab();
						WidgetCurrentTool.Instance.placer.Refresh();
						EInput.wheel = 0;
					}
					if (EInput.middleMouse.pressedTimer > EInput.middleMouse.clickDuration)
					{
						EInput.middleMouse.pressedTimer = 100f;
					}
				}
				else if (EClass.scene.mouseTarget.CanCycle())
				{
					if (EInput.wheel != 0)
					{
						EClass.scene.mouseTarget.CycleTarget(EInput.wheel);
						EInput.wheel = 0;
					}
					if (EInput.middleMouse.pressedTimer > EInput.middleMouse.clickDuration)
					{
						EInput.middleMouse.pressedTimer = 100f;
					}
				}
			}
			if ((EInput.middleMouse.down || EInput.middleMouse.clicked || EInput.middleMouse.pressedLong) && !EClass.ui.contextMenu.isActive)
			{
				if (EInput.middleMouse.clicked)
				{
					if (HotItemHeld.CanRotate())
					{
						HotItemHeld.taskBuild.recipe.Rotate();
						SE.Rotate();
					}
					else
					{
						base.DoFunc(input.middleClick);
					}
				}
				else if (EInput.middleMouse.pressedLong)
				{
					base.DoFunc(input.middlePressLong);
				}
			}
			if (EInput.mouse3.clicked)
			{
				base.DoFunc(input.mouse3Click);
			}
			else if (EInput.mouse3.pressedLong)
			{
				base.DoFunc(input.mouse3PressLong);
			}
			if (EInput.mouse4.clicked)
			{
				if (this.zoomOut)
				{
					this.ToggleZoom();
				}
				else
				{
					base.DoFunc(input.mouse4Click);
				}
			}
			else if (EInput.mouse4.pressedLong)
			{
				base.DoFunc(input.mouse4PressLong);
			}
			if (EInput.wheel != 0)
			{
				if (EClass.scene.mouseTarget.pos.Distance(EClass.pc.pos) <= 1 && HotItemHeld.CanChangeHeightByWheel())
				{
					if (EInput.wheel != 0)
					{
						ActionMode.Build.ModAltitude(EInput.wheel);
						SE.Tab();
						WidgetCurrentTool.Instance.placer.Refresh();
						EInput.wheel = 0;
					}
				}
				else
				{
					WidgetCurrentTool.Instance.ModSelected(-EInput.wheel);
					this.UpdatePlans();
				}
			}
			if (EClass.pc.HasNoGoal && EClass.player.currentHotItem.LookAtMouse)
			{
				EClass.pc.LookAt(this.planLeft.pos);
			}
			if (Input.GetKey(KeyCode.LeftControl))
			{
				if (Input.GetKeyDown(KeyCode.F))
				{
					Widget widget = EClass.ui.widgets.Toggle("Search");
					if (widget == null)
					{
						return;
					}
					widget.SoundActivate();
				}
				return;
			}
			EAction action = EInput.action;
			switch (action)
			{
			case EAction.Wait:
				if (EClass.debug.boradcast)
				{
					EClass.debug.BroadcastNext();
					return;
				}
				if (!EClass.pc.HasNoGoal)
				{
					if (EClass.pc.ai.CanManualCancel())
					{
						EClass.pc.ai.Cancel();
						return;
					}
				}
				else
				{
					if (EClass._zone.IsRegion)
					{
						EClass.player.EnterLocalZone(false, null);
						return;
					}
					TraitNewZone traitNewZone = EClass.pc.pos.FindThing<TraitNewZone>();
					if (traitNewZone != null && traitNewZone.CanAutoEnter())
					{
						traitNewZone.MoveZone(false);
						EInput.WaitReleaseKey();
						return;
					}
					ACT.Wait.Perform(EClass.pc, null, null);
					EClass.player.EndTurn(true);
					return;
				}
				break;
			case EAction.Interact:
			case EAction.Pick:
				break;
			case EAction.Search:
			{
				Widget widget2 = EClass.ui.widgets.Toggle("Search");
				if (widget2 == null)
				{
					return;
				}
				widget2.SoundActivate();
				return;
			}
			case EAction.Fire:
				if (!EClass.player.HasValidRangedTarget() || EInput.isShiftDown)
				{
					EClass.player.TargetRanged();
					if (EClass.player.HasValidRangedTarget())
					{
						SE.Play("lock_on");
					}
					if (EInput.keyFire.IsRepeating)
					{
						EInput.keyFire.Consume();
						return;
					}
				}
				else if (EClass.pc.HasNoGoal)
				{
					Thing thing = EClass.player.currentHotItem.Thing;
					bool reloading = EClass.pc.HasCondition<ConReload>();
					if (thing == null || !thing.CanAutoFire(EClass.pc, EClass.player.target, reloading))
					{
						foreach (UIList.ButtonPair buttonPair in WidgetCurrentTool.Instance.list.buttons)
						{
							Thing thing2 = buttonPair.obj as Thing;
							if (thing2 != null && thing2.CanAutoFire(EClass.pc, EClass.player.target, reloading))
							{
								thing = thing2;
								break;
							}
						}
					}
					if (thing == null || !thing.CanAutoFire(EClass.pc, EClass.player.target, reloading))
					{
						thing = EClass.pc.things.Find((Thing _t) => _t.isEquipped && _t.CanAutoFire(EClass.pc, EClass.player.target, reloading), true);
					}
					if (thing != null && thing.CanAutoFire(EClass.pc, EClass.player.target, reloading))
					{
						if (thing.HasTag(CTAG.throwWeapon))
						{
							Point pos = EClass.player.target.pos;
							if (ActThrow.CanThrow(EClass.pc, thing, EClass.player.target, null))
							{
								ACT.Throw.target = thing;
								ACT.Throw.Perform(EClass.pc, EClass.player.target, EClass.player.target.pos);
								EClass.player.EndTurn(false);
								this.SetTurbo(-1);
								return;
							}
						}
						else
						{
							TraitAbility traitAbility = thing.trait as TraitAbility;
							if (traitAbility != null)
							{
								if (traitAbility.act.CanPerform(EClass.pc, EClass.player.target, EClass.player.target.pos) && EClass.pc.UseAbility(traitAbility.act.source.alias, EClass.player.target, EClass.player.target.pos, false))
								{
									EClass.player.EndTurn(false);
									return;
								}
							}
							else if (thing.trait is TraitToolRange)
							{
								EClass.pc.ranged = thing;
								if (ACT.Ranged.CanPerform(EClass.pc, EClass.player.target, null))
								{
									EClass.player.renderThing = thing;
									if (ACT.Ranged.Perform(EClass.pc, EClass.player.target, null))
									{
										EClass.player.EndTurn(false);
										this.SetTurbo(-1);
									}
								}
							}
						}
					}
				}
				break;
			default:
				switch (action)
				{
				case EAction.Report:
					if (!EClass.debug.enable)
					{
						EClass.ui.ToggleFeedback();
						return;
					}
					break;
				case EAction.QuickSave:
					if (EClass.debug.enable || EClass.game.Difficulty.allowManualSave)
					{
						if (!EClass.pc.HasNoGoal)
						{
							SE.Beep();
							return;
						}
						EClass.game.Save(false, null, false);
						return;
					}
					break;
				case EAction.QuickLoad:
					if (!EClass.debug.enable && !EClass.game.Difficulty.allowManualSave)
					{
						SE.Beep();
						return;
					}
					EClass.core.WaitForEndOfFrame(delegate
					{
						string id = Game.id;
						EClass.scene.Init(Scene.Mode.None);
						Game.Load(id);
					});
					return;
				case EAction.AutoCombat:
					if (EClass.scene.mouseTarget.isValid && EClass.player.CanAcceptInput())
					{
						List<Chara> list2 = EClass.scene.mouseTarget.pos.ListCharas();
						list2.Sort((Chara a, Chara b) => a.hostility - b.hostility);
						foreach (Chara chara in list2)
						{
							if (chara.hostility < Hostility.Friend)
							{
								EClass.pc.SetAIImmediate(new GoalAutoCombat(chara));
								return;
							}
						}
						Msg.Say("noTargetFound");
						return;
					}
					break;
				case EAction.EmptyHand:
					if (WidgetCurrentTool.Instance)
					{
						WidgetCurrentTool.Instance.Select(-1, false);
						return;
					}
					break;
				case EAction.SwitchHotbar:
				case EAction.Examine:
				case EAction.GetAll:
				case EAction.CancelUI:
				case EAction.Mute:
					break;
				case EAction.Dump:
					TaskDump.TryPerform();
					return;
				case EAction.Meditate:
					if (EClass.pc.HasNoGoal)
					{
						EClass.pc.UseAbility("AI_Meditate", EClass.pc, null, false);
						return;
					}
					break;
				default:
					return;
				}
				break;
			}
			return;
		}
	}

	public void ShowAllAction()
	{
		this.planAll.Update(this.mouseTarget);
		if (this.planAll.HasAct)
		{
			this.planAll.ShowContextMenu();
			return;
		}
		if (EClass._zone.IsRegion)
		{
			EClass.ui.ToggleLayer<LayerTravel>(null);
			return;
		}
		if (EClass.debug.godBuild)
		{
			Thing lastThing = this.planAll.pos.LastThing;
			if (lastThing != null && EClass.pc.pos.Distance(this.planAll.pos) > 1 && lastThing.W == lastThing.H)
			{
				lastThing.Rotate(false);
				SE.Rotate();
				return;
			}
		}
		SE.BeepSmall();
	}

	public void SetTurbo(int mtp = -1)
	{
		if (mtp == -1)
		{
			if (AM_Adv.turbo <= EClass.setting.defaultTurbo)
			{
				AM_Adv.turbo = EClass.setting.defaultTurbo;
				return;
			}
		}
		else
		{
			AM_Adv.turbo = (float)mtp;
		}
	}

	public void EndTurbo()
	{
		AM_Adv.turbo = 0f;
	}

	public void ToggleZoom()
	{
		EClass.screen.focusPos = null;
		this.zoomOut2 = !this.zoomOut2;
		SE.Play(this.zoomOut2 ? "zoomOut" : "zoomIn");
		EClass.ui.widgets.OnChangeActionMode();
		this.ClearPlans();
	}

	public bool TryCancelInteraction(bool sound = true)
	{
		if (!EClass.pc.HasNoGoal && EClass.pc.ai.IsRunning)
		{
			if (EClass.pc.ai.CanManualCancel())
			{
				EClass.pc.ai.Cancel();
				if (sound)
				{
					SE.CancelAction();
				}
			}
			EInput.Consume(true, 1);
			return true;
		}
		return false;
	}

	public bool CanAct()
	{
		return EClass.pc.HasNoGoal;
	}

	public void AxisMove()
	{
		Vector2 vector = (EClass.core.config.input.altKeyAxis && !EClass._zone.IsRegion) ? Util.ConvertAxis(EInput.axis) : EInput.axis;
		if (!this.startedRun)
		{
			if ((this.autorun && this.timerStartRunning >= 0.7f) || Input.GetKey(KeyCode.LeftShift))
			{
				this.SetTurbo(-1);
				this.startedRun = true;
			}
		}
		else if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			this.SetTurbo((AM_Adv.turbo == 0f) ? -1 : 0);
		}
		if (EClass.core.config.test.alwaysRun)
		{
			this.SetTurbo(Input.GetKey(KeyCode.LeftShift) ? 0 : -1);
		}
		this.timerStartRunning += Core.delta;
		EClass.player.nextMove.x = 0f;
		EClass.player.nextMove.y = 0f;
		Point point = EClass.pc.pos.Copy();
		point.x += (int)vector.x;
		point.z += (int)vector.y;
		if (EClass.pc.HasNoGoal && !point.IsValid && EClass.player.CanExitBorder(point))
		{
			EClass.player.ExitBorder(null);
			return;
		}
		this.axisTarget.Update(point);
		this.pressedAction.Init(Vector3.zero);
		this.planKeyboard.Update(this.axisTarget);
		if (!this.planKeyboard.HasAct)
		{
			EClass.player.nextMove.x = vector.x;
			EClass.player.nextMove.y = vector.y;
			if (!(EClass.pc.ai is GoalManualMove) && EClass.pc.HasNoGoal && GoalManualMove.CanMove())
			{
				this.SetManualMove();
			}
			this.movedByKey = true;
			return;
		}
		if (this.planKeyboard.list[0].act.ResetAxis && EClass.pc.ai is GoalManualMove)
		{
			EInput.forbidAxis = EInput.axis;
			EClass.pc.ai.Cancel();
			return;
		}
		this.pressedAction.axis = EInput.axis;
		this.pressedAction.SetAction(this.planKeyboard.GetAction(), true, this.planKeyboard.WillEndTurn, true);
	}

	public void SetManualMove()
	{
		if (EClass.player.TooHeavyToMove())
		{
			return;
		}
		this.SetTurbo(EInput.isShiftDown ? -1 : 0);
		EClass.pc.SetAIImmediate(new GoalManualMove());
		this.cursorMove = true;
	}

	public void SetPressedAction(ButtonState button)
	{
		if (this.updatePlans)
		{
			this.UpdatePlans();
		}
		bool flag = button == EInput.leftMouse;
		this.pressedAction.Init(button);
		if (this.isMouseOnMap)
		{
			if (flag)
			{
				if (this.planLeft.HasAct)
				{
					this.pressedAction.SetPlan(this.planLeft);
					return;
				}
			}
			else
			{
				if (this.planRight.HasAct)
				{
					this.pressedAction.SetPlan(this.planRight);
					return;
				}
				SE.Play("noAction");
				return;
			}
		}
		if (flag)
		{
			Point pos = (!this.isMouseOnMap) ? null : base.hit.Copy();
			if (pos != null)
			{
				if (EClass.pc.pos.Equals(pos))
				{
					return;
				}
				if (EClass.pc.pos.Distance(pos) == 1 && pos.cell.blocked)
				{
					return;
				}
			}
			this.planLeft.Update(this.mouseTarget);
			if (this.planLeft.HasAct)
			{
				return;
			}
			this.clickPos = Input.mousePosition;
			this.pressedAction.SetAction(() => this.PressedActionMove(pos), false, false, false);
			this.pressedAction.repeat = this.zoomOut;
			this.pressedAction.ignoreCount = true;
		}
	}

	public bool PressedActionMove(Point pos)
	{
		if (EClass.player.TooHeavyToMove())
		{
			return false;
		}
		if (pos != null)
		{
			if (EInput.leftMouse.clicked)
			{
				if (EClass.pc.pos.Equals(pos) || !pos.IsValid)
				{
					EClass.player.nextMove = Vector2.zero;
					EClass.pc.ai.Cancel();
				}
				else if (PathManager.Instance.RequestPathImmediate(EClass.pc.pos, pos, EClass.pc, PathManager.MoveType.Default, -1, 0).HasPath && (this.zoomOut || (!pos.Equals(GoalManualMove.lastPoint) && !pos.Equals(GoalManualMove.lastlastPoint))))
				{
					if (!EClass.pc.IsEnemyOnPath(pos, true))
					{
						EClass.pc.SetAIImmediate(new AI_Goto(pos, 0, false, false));
					}
				}
				else
				{
					EClass.player.nextMove = Vector2.zero;
					EClass.pc.ai.Cancel();
				}
				return false;
			}
			if (EInput.rightMouse.down && EClass.core.config.input.autowalk)
			{
				this.keepWalking = true;
			}
		}
		if (this.autorun)
		{
			if (Vector2.Distance(this.posOrigin, this.posArrow) > EClass.core.config.game.runDistance)
			{
				this.SetTurbo(-1);
			}
			else if (AM_Adv.turbo != 0f && !Input.GetKey(KeyCode.LeftShift) && !EClass.core.config.input.keepRunning && !this.zoomOut && Vector2.Distance(this.posOrigin, this.posArrow) < 1.2f)
			{
				this.EndTurbo();
			}
		}
		if (pos != null && !this.cursorMove)
		{
			Vector2Int v = new Vector2Int(pos.x - EClass.pc.pos.x, pos.z - EClass.pc.pos.z);
			if (Mathf.Abs(v.x) > 1 || Mathf.Abs(v.y) > 1)
			{
				int num = Mathf.Max(Mathf.Abs(v.x), Mathf.Abs(v.y));
				v.x /= num;
				v.y /= num;
			}
			EClass.player.nextMove = v;
			Point.shared.Set(EClass.pc.pos.x + v.x, EClass.pc.pos.z + v.y);
		}
		else
		{
			EClass.player.nextMove = this.vArrow;
			Point.shared.Set(EClass.pc.pos.x + (int)this.vArrow.x, EClass.pc.pos.z + (int)this.vArrow.y);
		}
		if (EClass.pc.IsEnemyOnPath(Point.shared, true))
		{
			if (EClass.pc.ai is GoalManualMove)
			{
				EClass.pc.ai.Cancel();
			}
			return true;
		}
		if (Point.shared.IsInBounds)
		{
			if (!(EClass.pc.ai is GoalManualMove))
			{
				if (EClass.pc.HasNoGoal)
				{
					if (GoalManualMove.CanMove())
					{
						this.SetTurbo(EInput.isShiftDown ? -1 : 0);
						EClass.pc.SetAIImmediate(new GoalManualMove());
					}
					this.cursorMove = true;
				}
			}
			else
			{
				this.cursorMove = true;
			}
			return true;
		}
		if (EClass.pc.HasNoGoal && EClass.player.CanExitBorder(Point.shared))
		{
			EClass.player.ExitBorder(null);
			return false;
		}
		if (EClass.pc.ai is GoalManualMove)
		{
			EClass.pc.ai.Cancel();
		}
		return true;
	}

	public unsafe virtual void RefreshArrow()
	{
		bool flag = this.zoomOut || EInput.rightMouse.pressedLong;
		if (flag && this.mouseTarget.pos.Equals(EClass.pc.pos))
		{
			this.vArrow = Vector2.zero;
			EClass.player.nextMove = this.vArrow;
			return;
		}
		this.posOrigin = (flag ? (*EClass.pc.pos.PositionCenter()) : CursorSystem.posOrigin);
		this.posArrow = CursorSystem.position;
		float num;
		if (this.cursorMove || EClass.pc.pos.Distance(base.hit) > 1)
		{
			num = Util.GetAngle(this.posArrow.x - this.posOrigin.x, this.posArrow.y - this.posOrigin.y) + 90f + 22.5f;
		}
		else
		{
			num = Util.GetAngle((float)(EClass.pc.pos.x - base.hit.x), (float)(EClass.pc.pos.z - base.hit.z)) - 22.5f;
			if (num < 0f)
			{
				num = 360f + num;
			}
		}
		if (WidgetUnityChan.Instance)
		{
			WidgetUnityChan.Instance.Refresh(num);
		}
		if (this.clickPos != Vector3.zero)
		{
			if (Vector3.Distance(Input.mousePosition, this.clickPos) < EClass.core.config.game.angleMargin)
			{
				return;
			}
			this.clickPos = Vector3.zero;
		}
		this.vArrow = Vector2.zero;
		int _angle = 0;
		Action<int, int, int, int> action = delegate(int x, int y, int i, int a)
		{
			this.vArrow.x = (float)x;
			this.vArrow.y = (float)y;
			this.arrowIndex = i;
			_angle = -a;
		};
		if (num < 45f || num >= 360f)
		{
			action(-1, -1, 0, 0);
			return;
		}
		if (num < 90f)
		{
			action(-1, 0, 1, 35);
			return;
		}
		if (num < 135f)
		{
			action(-1, 1, 2, 90);
			return;
		}
		if (num < 180f)
		{
			action(0, 1, 3, 145);
			return;
		}
		if (num < 225f)
		{
			action(1, 1, 4, 180);
			return;
		}
		if (num < 270f)
		{
			action(1, 0, 5, 215);
			return;
		}
		if (num < 315f)
		{
			action(1, -1, 6, 270);
			return;
		}
		action(0, -1, 7, 325);
	}

	public static float turbo;

	public static int actCount;

	public bool zoomOut;

	public bool zoomOut2;

	public bool movedByKey;

	public int itemLost;

	protected int arrowIndex;

	protected float timerStartRunning;

	protected bool cursorMove;

	protected bool keepWalking;

	protected Vector3 posOrigin;

	protected Vector3 posArrow;

	protected Vector2 vArrow;

	public AM_Adv.PressedAction pressedAction = new AM_Adv.PressedAction();

	public PointTarget axisTarget = new PointTarget
	{
		mouse = false
	};

	public ActPlan planLeft = new ActPlan
	{
		input = ActInput.LeftMouse
	};

	public ActPlan planRight = new ActPlan
	{
		input = ActInput.RightMouse
	};

	public ActPlan planKeyboard = new ActPlan
	{
		input = ActInput.Key
	};

	public ActPlan planAll = new ActPlan
	{
		input = ActInput.AllAction
	};

	public string textWheel;

	public string textMiddle;

	private float gearAngle;

	protected bool updatePlans;

	protected bool isMoving;

	private Vector3 lastCamPos;

	public float rightMouseTimer;

	private bool startedRun;

	protected Vector3 clickPos;

	public class PressedAction
	{
		public void Init(Vector3 _axis)
		{
			this.button = null;
			this.action = null;
			this.plan = null;
			this.timer = 0f;
			this.axis = _axis;
		}

		public void Init(ButtonState _button = null)
		{
			this.button = _button;
			this.action = null;
			this.plan = null;
			this.timer = 0f;
			this.axis = Vector2.zero;
		}

		public void SetAction(Func<bool> _action = null, bool _canTurbo = true, bool _willEndTurn = true, bool _waitForTurn = true)
		{
			this.plan = null;
			this.action = _action;
			this.canTurbo = _canTurbo;
			this.willEndTurn = _willEndTurn;
			this.waitForTurn = _waitForTurn;
			this.hideRightInfo = false;
			this.repeat = false;
			this.ignoreCount = false;
			this.act = null;
		}

		public void SetPlan(ActPlan _plan)
		{
			this.plan = _plan;
			this.action = this.plan.GetAction();
			this.canTurbo = true;
			this.willEndTurn = this.plan.WillEndTurn;
			this.waitForTurn = true;
			this.hideRightInfo = this.plan.HideRightInfo;
			this.repeat = this.plan.canRepeat;
			this.ignoreCount = false;
			this.act = ((this.plan.list.Count == 1) ? this.plan.list[0].act : null);
		}

		public bool IsPressing()
		{
			if (this.button != null)
			{
				return this.button.down || this.button.pressing || this.button.clicked;
			}
			return this.axis != Vector2.zero && EInput.axis == this.axis;
		}

		public ButtonState button;

		public bool canTurbo;

		public bool willEndTurn;

		public bool waitForTurn;

		public bool hideRightInfo;

		public bool repeat;

		public bool ignoreCount;

		public Func<bool> action;

		public Act act;

		public ActPlan plan;

		public float timer;

		public float timerRepeat;

		public Vector2 axis;

		public int count;
	}
}
