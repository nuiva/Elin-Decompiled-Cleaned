using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ActPlan : EClass
{
	public bool IsSelf
	{
		get
		{
			return this.dist == 0;
		}
	}

	public bool IsSelfOrNeighbor
	{
		get
		{
			return this._canInteractNeighbor;
		}
	}

	public bool IsNeighborBlocked
	{
		get
		{
			return !this._canInteractNeighbor && this.dist == 1;
		}
	}

	public bool canRepeat
	{
		get
		{
			return this.list.Count == 1 && this.list[0].act.CanPressRepeat;
		}
	}

	public Chara cc
	{
		get
		{
			return EClass.pc;
		}
	}

	public string GetText(bool showName)
	{
		if (this.list.Count == 0)
		{
			return "";
		}
		if (this.list.Count == 1)
		{
			return this.list[0].GetText(showName);
		}
		return "+" + this.list.Count.ToString() + ((this.HasMultipleTargets || this.list[0].tc == null) ? ("\n<size=28>(" + "multipleTargets".lang() + ")</size>") : ("\n<size=28>" + this.list[0].tc.Name + this.list[0].tc.GetExtraName() + "</size>"));
	}

	public CursorInfo CursorIcon
	{
		get
		{
			if (this.list.Count == 0)
			{
				return null;
			}
			if (this.list.Count <= 1)
			{
				return this.list[0].act.GetCursorIcon(this.list[0].tc);
			}
			return CursorSystem.Notice;
		}
	}

	public bool WillEndTurn
	{
		get
		{
			return this.list.Count > 0 && this.list[0].act.WillEndTurn;
		}
	}

	public bool HideRightInfo
	{
		get
		{
			return this.list.Count > 0 && this.list[0].act.HideRightInfo;
		}
	}

	public bool HasAct
	{
		get
		{
			return this.list.Count > 0;
		}
	}

	public bool ShowAct
	{
		get
		{
			return this.HasAct && this.lastAct != this.list[0].act && this.list[0].act.ShowMouseHint(this.list[0].tc);
		}
	}

	public bool HasMultipleTargets
	{
		get
		{
			return this.list.Count > 1 && this.list[0].tc != this.list.LastItem<ActPlan.Item>().tc;
		}
	}

	public Func<bool> GetAction()
	{
		if (this.list.Count == 0)
		{
			return () => false;
		}
		if (this.list.Count > 1)
		{
			return delegate()
			{
				if (this.pos.Equals(EClass.pc.pos) && ActWait.SearchMedal(EClass.pc, this.pos))
				{
					return false;
				}
				this.ShowContextMenu();
				return false;
			};
		}
		ActPlan.Item item = this.list[0];
		return delegate()
		{
			if (this.performed && !item.act.CanPressRepeat)
			{
				return false;
			}
			this.performed = true;
			this.lastAct = item.act;
			return item.Perform(this.performed);
		};
	}

	public void ShowContextMenu()
	{
		UIContextMenu uicontextMenu = EClass.ui.CreateContextMenuInteraction();
		int num = 1;
		bool flag = this.showOrder;
		using (List<ActPlan.Item>.Enumerator enumerator = this.list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				ActPlan.Item i = enumerator.Current;
				string text = i.GetTextContext(this.HasMultipleTargets);
				text = text.Replace("\n", " ").Replace(Environment.NewLine, " ");
				uicontextMenu.AddButton(text, delegate()
				{
					this.performed = true;
					if (i.Perform(false))
					{
						EClass.player.EndTurn(true);
					}
				}, true);
				num++;
				if (num >= 21)
				{
					break;
				}
			}
		}
		uicontextMenu.Show();
		EClass.ui.hud.HideMouseInfo();
	}

	public bool TrySetAct(string lang, Func<bool> onPerform, Card tc, CursorInfo cursor = null, int dist = 1, bool isHostileAct = false, bool localAct = true, bool canRepeat = false)
	{
		return this.TrySetAct(new DynamicAct(lang, onPerform, false)
		{
			id = lang,
			dist = dist,
			isHostileAct = isHostileAct,
			localAct = localAct,
			cursor = ((cursor == CursorSystem.Arrow) ? null : cursor),
			canRepeat = (() => canRepeat)
		}, tc);
	}

	public bool TrySetAct(string lang, Func<bool> onPerform, CursorInfo cursor = null, int dist = 1)
	{
		return this.TrySetAct(new DynamicAct(lang, onPerform, false)
		{
			id = lang,
			dist = dist,
			cursor = ((cursor == CursorSystem.Arrow) ? null : cursor)
		}, null);
	}

	public bool TrySetAct(Act _act, Card _tc = null)
	{
		if (!this.ignoreAdddCondition && !_act.CanPerform(this.cc, _tc, this.pos))
		{
			return false;
		}
		ActPlan.Item item = new ActPlan.Item
		{
			act = _act,
			tc = _tc,
			pos = this.pos.Copy()
		};
		if (_tc != null && _tc.isChara)
		{
			int num = -1;
			for (int i = 0; i < this.list.Count; i++)
			{
				if (this.list[i].tc == _tc)
				{
					num = i;
				}
			}
			if (num != -1)
			{
				this.list.Insert(num + 1, item);
			}
			else
			{
				this.list.Add(item);
			}
		}
		else
		{
			this.list.Add(item);
		}
		return true;
	}

	public void Clear()
	{
		this.list.Clear();
		this.pos.IsValid = false;
	}

	public void Update(PointTarget target)
	{
		if (ActPlan.warning)
		{
			return;
		}
		this._Update(target);
		if (this.HasAct)
		{
			Color effectColor = (this.list.Count == 1) ? this.list[0].act.GetActPlanColor() : EClass.Colors.colorAct;
			if (this.input == ActInput.LeftMouse || this.input == ActInput.Key)
			{
				EClass.ui.hud.textLeft.SetText(this.GetText(true));
				Outline[] components = EClass.ui.hud.textLeft.GetComponents<Outline>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].effectColor = effectColor;
				}
				return;
			}
			if (this.input == ActInput.RightMouse)
			{
				EClass.player.currentHotItem.SetImage(EClass.ui.hud.imageRight);
				EClass.ui.hud.imageRight.rectTransform.localScale = new Vector3(2f, 2f, 1f);
				EClass.ui.hud.textRight.SetText(this.GetText(true));
				Outline[] components = EClass.ui.hud.textRight.GetComponents<Outline>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].effectColor = effectColor;
				}
			}
		}
	}

	public void _Update(PointTarget target)
	{
		this.showOrder = false;
		this.performed = false;
		this.list.Clear();
		this.listPick.Clear();
		if (!this.pos.Equals(target.pos))
		{
			this.lastAct = null;
		}
		this.pos.Set(target.pos);
		this.dist = this.cc.pos.Distance(this.pos);
		if (!this.pos.IsValid || EClass.pc.isDead)
		{
			return;
		}
		Point _pos = new Point(this.pos);
		Cell cell = target.pos.cell;
		List<Card> items = _pos.ListCards(false);
		bool isKey = this.input == ActInput.Key;
		this.altAction = ((EInput.isShiftDown && !EInput.isAltDown && !isKey) || this.input == ActInput.AllAction);
		this._canInteractNeighbor = (this.dist == 0 || (this.dist == 1 && this.cc.CanInteractTo(_pos)));
		if (EClass.pc.isBlind && !_pos.Equals(EClass.pc.pos) && !isKey && this.input != ActInput.LeftMouse)
		{
			return;
		}
		if (!isKey && this.input != ActInput.LeftMouse && this.input != ActInput.AllAction)
		{
			if (this.input == ActInput.RightMouse)
			{
				if (this.pos.cell.outOfBounds || EClass.ui.IsDragging)
				{
					return;
				}
				HotItem hotItem = EClass.player.currentHotItem;
				if (!hotItem.IsGameAction)
				{
					this.TrySetAct(hotItem.Name, delegate()
					{
						hotItem.OnClick(hotItem.button, hotItem.hotbar);
						return false;
					}, null, -1);
				}
				else
				{
					hotItem.TrySetAct(this);
				}
				bool flag = EClass.game.config.autoCombat.enable && EClass.scene.mouseTarget.TargetChara != null;
				if (hotItem.Thing != null && hotItem.Thing.trait.DisableAutoCombat)
				{
					flag = false;
				}
				if (flag)
				{
					Chara targetChara = EClass.scene.mouseTarget.TargetChara;
					bool flag2 = true;
					if (targetChara.hostility >= Hostility.Friend)
					{
						flag2 = false;
					}
					if (targetChara.hostility == Hostility.Neutral && EClass.game.config.autoCombat.bDontAutoAttackNeutral)
					{
						flag2 = false;
					}
					if (this.list.Count >= 2)
					{
						flag2 = false;
					}
					if (this.list.Count == 1)
					{
						if (targetChara.hostility >= Hostility.Neutral)
						{
							flag2 = false;
						}
						if (!this.list[0].act.ShowAuto)
						{
							flag2 = false;
						}
						if (EClass.player.currentHotItem is HotItemNoItem && targetChara.hostility <= Hostility.Enemy)
						{
							flag2 = true;
						}
					}
					if (flag2)
					{
						this.list.Clear();
						this.TrySetAct(new GoalAutoCombat(targetChara), null);
					}
				}
				if (this.list.Count == 0 && !EClass.core.config.test.toolNoPick)
				{
					HotItemNoItem._TrySetAct(this);
				}
				if (WidgetCurrentTool.Instance)
				{
					WidgetCurrentTool.Instance.placer.Refresh();
				}
			}
			return;
		}
		if (EClass.ui.IsDragging)
		{
			return;
		}
		if (_pos.cell.outOfBounds)
		{
			if (EClass.player.CanExitBorder(_pos))
			{
				EClass.player.ExitBorder(this);
			}
			return;
		}
		if (!isKey && _pos.Equals(this.cc.pos) && EClass._zone.IsRegion)
		{
			this.TrySetAct("actNewZone", delegate()
			{
				EClass.player.EnterLocalZone(false, null);
				return false;
			}, CursorSystem.MoveZone, 1);
			if (this.input == ActInput.AllAction)
			{
				this.TrySetAct("LayerTravel", delegate()
				{
					EClass.ui.AddLayer<LayerTravel>();
					return false;
				}, null, CursorSystem.MoveZone, 1, false, false, false);
			}
		}
		items.ForeachReverse(delegate(Card _c)
		{
			Chara chara = _c.Chara;
			if (chara == null || chara.IsPC || !EClass.pc.CanSee(chara))
			{
				return;
			}
			int num = chara.Dist(EClass.pc);
			if (num > 1 && EClass.pc.isBlind)
			{
				return;
			}
			if (!EClass.pc.isBlind && !chara.IsHostile() && (this.input == ActInput.AllAction || !((chara.IsPCParty || chara.IsMinion) | isKey)) && (this.input == ActInput.AllAction || !chara.IsNeutral() || chara.quest != null || EClass.game.quests.IsDeliverTarget(chara)) && chara.isSynced && num <= 2)
			{
				bool flag3 = !chara.HasCondition<ConSuspend>() && (!chara.isRestrained || !chara.IsPCFaction);
				if (EClass._zone.instance is ZoneInstanceMusic && !chara.IsPCFactionOrMinion)
				{
					flag3 = false;
				}
				if (flag3 || this.altAction)
				{
					if (EClass.pc.HasElement(1216, 1) && chara.HasCondition<ConSleep>())
					{
						this.TrySetAct(new AI_Fuck
						{
							target = chara,
							succubus = true
						}, chara);
					}
					this.TrySetAct(ACT.Chat, chara);
				}
			}
			if (chara.host != EClass.pc)
			{
				if (chara.IsRestrainedResident)
				{
					this.TrySetAct(new AI_PracticeDummy
					{
						target = chara
					}, null);
					return;
				}
				if ((chara.IsHostile() || this.altAction || chara.isRestrained) && chara.IsAliveInCurrentZone)
				{
					this.TrySetAct(ACT.Melee, chara);
				}
			}
		});
		if (_pos.IsHidden || !this.IsSelfOrNeighbor)
		{
			return;
		}
		items.ForeachReverse(delegate(Card _c)
		{
			Chara c = _c.Chara;
			if (c != null)
			{
				bool flag3 = EClass.pc.CanSee(c);
				if (flag3)
				{
					if (this.input == ActInput.LeftMouse && c.IsPCFaction && !c.IsPC && this.pos.FindThing<TraitHitchingPost>() != null)
					{
						Chara ride = c;
						List<string> list = EClass.core.pccs.sets["ride"].map["body"].map.Keys.ToList<string>();
						int index = list.IndexOf(ride.c_idRidePCC);
						if (index == -1)
						{
							index = 0;
						}
						Func<float, string> <>9__15;
						Action<float> <>9__16;
						this.TrySetAct("ActChangeRideSkin", delegate()
						{
							UIContextMenu uicontextMenu = EClass.ui.CreateContextMenuInteraction();
							string text = "rideSkin";
							Func<float, string> textFunc;
							if ((textFunc = <>9__15) == null)
							{
								textFunc = (<>9__15 = ((float a) => list[(int)a].Split('-', StringSplitOptions.None)[0] ?? ""));
							}
							float value = (float)index;
							Action<float> action;
							if ((action = <>9__16) == null)
							{
								action = (<>9__16 = delegate(float a)
								{
									ride.c_idRidePCC = list[(int)a];
									ride._CreateRenderer();
								});
							}
							uicontextMenu.AddSlider(text, textFunc, value, action, 0f, (float)(list.Count - 1), true, false, false);
							uicontextMenu.Show();
							return false;
						}, c, null, 1, false, true, false);
					}
					if (!c.IsPC && ((c.IsPCFaction && !c.IsDisabled) || EClass.debug.enable) && this.input == ActInput.AllAction)
					{
						this.TrySetAct("actTrade", delegate()
						{
							LayerInventory.CreateContainer(c);
							return false;
						}, c, null, 1, false, true, false);
					}
					if (c.host != null && EClass.pc.held != null && this.altAction)
					{
						bool flag4 = true;
						if ((EClass.pc.held.trait is TraitThrown || EClass.pc.held.trait.IsTool) && !HotItemHeld.disableTool)
						{
							flag4 = false;
						}
						if (!c.IsDisabled && flag4 && c.CanAcceptGift(EClass.pc, EClass.pc.held))
						{
							string lang = "actGive";
							if (c.Evalue(1232) > 0 && EClass.pc.held.trait is TraitDrinkMilkMother)
							{
								lang = "actMilk";
							}
							this.TrySetAct(lang, delegate()
							{
								if (!c.IsValidGiftWeight(EClass.pc.held, 1))
								{
									c.Talk("tooHeavy", null, null, false);
									return true;
								}
								if (EClass.core.config.game.confirmGive)
								{
									Dialog.YesNo("dialogGive".lang(EClass.pc.held.GetName(NameStyle.Full, 1), null, null, null, null), new Action(base.<_Update>g__func|18), null, "yes", "no");
								}
								else
								{
									base.<_Update>g__func|18();
								}
								return true;
							}, c, null, 1, false, true, false);
						}
					}
				}
				if (this.input == ActInput.AllAction && EClass.pc.held != null && EClass.pc.held.trait is TraitDrink)
				{
					this.TrySetAct(c.IsPC ? "actPour" : "ActThrow", delegate()
					{
						ActThrow.Throw(EClass.pc, c.pos, c, EClass.pc.held.Split(1), ThrowMethod.Default);
						return true;
					}, (c.host != null) ? c : EClass.pc.held, null, 1, false, true, false);
				}
				if (c.IsPC || c.host != null)
				{
					return;
				}
				if ((EClass.pc.isBlind || !flag3) && this.input == ActInput.AllAction)
				{
					return;
				}
				if (c.isRestrained && (this.input == ActInput.AllAction || (!c.IsRestrainedResident && !c.IsHostile())))
				{
					this.TrySetAct("ActUnrestrain", delegate()
					{
						c.TryUnrestrain(true, EClass.pc);
						return true;
					}, c, null, 1, false, true, false);
				}
				if (EClass.pc.isBlind || !flag3)
				{
					return;
				}
				if (this.input == ActInput.AllAction)
				{
					this.TrySetAct(ACT.Kick, c);
					if (c.IsMofuable)
					{
						this.TrySetAct("ActCuddle", delegate()
						{
							EClass.pc.Cuddle(c, false);
							return true;
						}, c, null, 1, false, true, false);
					}
					if (EClass.debug.showExtra)
					{
						this.TrySetAct("inspect", delegate()
						{
							c.Inspect();
							return false;
						}, c, null, 1, false, true, false);
					}
					if (c.IsPCPartyMinion && !c.Chara.IsEscorted())
					{
						this.TrySetAct("ActBanishSummon", delegate()
						{
							EClass.pc.Say("summon_vanish", c, null, null);
							c.pos.PlayEffect("vanish");
							c.pos.PlaySound("vanish", true, 1f, true);
							c.pos.PlayEffect("teleport");
							c.Destroy();
							return true;
						}, c, null, 1, false, true, false);
						return;
					}
				}
			}
			else if (_c.isThing)
			{
				if (EClass.pc.isBlind)
				{
					return;
				}
				Thing t = _c.Thing;
				if (this.input == ActInput.AllAction)
				{
					if (EClass.debug.enable)
					{
						if (t.LightData != null)
						{
							if (t.c_lightColor != 0)
							{
								this.TrySetAct("(debug) Clear Light", delegate()
								{
									t.c_lightColor = 0;
									t.RecalculateFOV();
									TCExtra tc = t.renderer.GetTC<TCExtra>();
									if (tc != null)
									{
										tc.RefreshColor();
									}
									return false;
								}, t, null, 1, false, true, false);
							}
							Action<PickerState, Color> <>9__23;
							this.TrySetAct("(debug) Set Light", delegate()
							{
								Color lightColor = t.LightColor;
								LayerColorPicker layerColorPicker = EClass.ui.AddLayer<LayerColorPicker>();
								Color startColor = lightColor;
								Color resetColor = lightColor;
								Action<PickerState, Color> onChangeColor;
								if ((onChangeColor = <>9__23) == null)
								{
									onChangeColor = (<>9__23 = delegate(PickerState state, Color _c)
									{
										t.c_lightColor = (int)((byte)Mathf.Clamp(_c.r * 32f, 1f, 31f)) * 1024 + (int)((byte)Mathf.Clamp(_c.g * 32f, 1f, 31f) * 32) + (int)((byte)Mathf.Clamp(_c.b * 32f, 1f, 31f));
										t.RecalculateFOV();
										TCExtra tc = t.renderer.GetTC<TCExtra>();
										if (tc == null)
										{
											return;
										}
										tc.RefreshColor();
									});
								}
								layerColorPicker.SetColor(startColor, resetColor, onChangeColor);
								return false;
							}, t, null, 1, false, true, false);
						}
						if (this.pos.cell.IsTopWater)
						{
							this.TrySetAct("(debug) Toggle Float", delegate()
							{
								t.isFloating = !t.isFloating;
								return false;
							}, t, null, 1, false, true, false);
						}
					}
					if (!EClass._zone.IsUserZone || !t.isNPCProperty)
					{
						if (t.trait.CanEat(EClass.pc))
						{
							this.TrySetAct(new AI_Eat
							{
								target = t
							}, t);
						}
						if (t.trait.CanDrink(EClass.pc))
						{
							this.TrySetAct(new AI_Drink
							{
								target = t
							}, t);
						}
						if (t.trait.CanRead(EClass.pc))
						{
							this.TrySetAct(new AI_Read
							{
								target = t
							}, t);
						}
						if (t.trait.IsBlendBase)
						{
							this.TrySetAct("invBlend", delegate()
							{
								LayerDragGrid.Create(new InvOwnerBlend(t, null, CurrencyType.None), false);
								return true;
							}, t, null, 1, false, true, false);
						}
					}
				}
				if (isKey)
				{
					bool canBeAttacked = t.trait.CanBeAttacked;
					return;
				}
				if (t.placeState == PlaceState.roaming && (_pos.cell.blocked || t.ignoreAutoPick || this.altAction || this.input == ActInput.AllAction || _pos.Equals(EClass.pc.pos)) && EClass.pc.CanPick(t))
				{
					this.listPick.Add(t);
				}
				if (t.IsInstalled)
				{
					t.trait.TrySetToggleAct(this);
					t.trait.TrySetAct(this);
				}
			}
		});
		if (this.listPick.Count > 0)
		{
			if (this.listPick.Count == 1)
			{
				Thing _t = this.listPick[0];
				if (!EClass._zone.IsRegion || (!_t.ignoreAutoPick && _t.pos.Equals(EClass.pc.pos)) || this.altAction)
				{
					this.TrySetAct("actPickOne", delegate()
					{
						EClass.pc.Pick(_t, true, true);
						return true;
					}, this.listPick[0], CursorSystem.Hand, 1, false, false, false);
				}
			}
			else
			{
				IList<Card> _cards = items.Copy<Card>();
				this.TrySetAct("actPickAll", delegate()
				{
					foreach (Card card in _cards)
					{
						if (card.isThing && card.placeState == PlaceState.roaming)
						{
							EClass.pc.Pick(card.Thing, true, true);
						}
					}
					return true;
				}, null, CursorSystem.Hand, 1, false, false, false);
			}
		}
		if (this.input == ActInput.AllAction && this.pos.IsSky)
		{
			this.TrySetAct("actSkyJump", delegate()
			{
				EClass.pc.FallFromZone();
				return false;
			}, null, 1);
		}
		if (_pos.Equals(this.cc.pos))
		{
			if (this.cc.held != null && !this.cc.held.IsHotItem)
			{
				this.TrySetAct("actPick", delegate()
				{
					Card held = this.cc.held;
					this.cc.PickHeld(true);
					return false;
				}, this.cc.held, CursorSystem.Inventory, 1, false, false, false);
			}
			else if (!this.HasAct && !this.cc.isRestrained)
			{
				this.TrySetAct(ACT.Wait, null);
			}
			if (EClass.pc.party.members.Count > 1)
			{
				this.showOrder = true;
			}
			if (this.input == ActInput.AllAction && EClass.pc.held != null)
			{
				this.TrySetAct("actDrop", delegate()
				{
					EClass.pc.DropThing(EClass.pc.held.Thing, -1);
					return true;
				}, null, 1);
			}
			if (this.cc.isRestrained)
			{
				this.TrySetAct("ActUnrestrain", delegate()
				{
					this.cc.TryUnrestrain(true, EClass.pc);
					return true;
				}, this.cc, null, 1, false, true, false);
			}
		}
	}

	public static bool warning;

	public Point pos = new Point();

	public ActInput input;

	public bool performed;

	public bool altAction;

	public bool ignoreAdddCondition;

	private bool _canInteractNeighbor;

	private bool showOrder;

	public ActPlan.List list = new ActPlan.List();

	public int dist;

	public Thing tool;

	public List<Thing> listPick = new List<Thing>();

	public Act lastAct;

	public class List : List<ActPlan.Item>
	{
		public void Add(Act a, string s = "")
		{
			base.Add(new ActPlan.Item
			{
				act = a
			});
		}
	}

	public class Item
	{
		public Chara cc
		{
			get
			{
				return EClass.pc;
			}
		}

		public bool HideHint
		{
			get
			{
				return EClass.pc.isBlind || (this.tc != null && this.tc.isChara && !EClass.pc.CanSee(this.tc));
			}
		}

		public string GetText(bool showName)
		{
			return this.act.GetText("") + ((showName && this.act.GetTextSmall(this.tc) != null) ? ("\n<size=28>" + this.act.GetTextSmall(this.tc) + "</size>") : "");
		}

		public string GetTextContext(bool showName)
		{
			return this.act.GetText("") + ((showName && this.tc != null && !this.HideHint) ? ("<size=13> (" + this.tc.Name + ")</size>") : "");
		}

		public bool Perform(bool repeated = false)
		{
			if (AM_Adv.actCount == 0 && !Dialog.warned)
			{
				ActPlan.warning = true;
				Chara _CC = Act.CC;
				Card _TC = Act.TC;
				Point _TP = new Point(Act.TP);
				if (EClass._zone.IsCrime(EClass.pc, this.act) && this.act.ID != "actContainer")
				{
					Dialog.TryWarnCrime(delegate
					{
						Act.CC = _CC;
						Act.TC = _TC;
						Act.TP.Set(_TP);
						if (this.Perform(false))
						{
							EClass.player.EndTurn(true);
						}
					});
					return false;
				}
				if (this.act is TaskHarvest && (this.act as TaskHarvest).mode == BaseTaskHarvest.HarvestType.Disassemble)
				{
					Dialog.TryWarnDisassemble(delegate
					{
						Act.CC = _CC;
						Act.TC = _TC;
						Act.TP.Set(_TP);
						if (this.Perform(false))
						{
							EClass.player.EndTurn(true);
						}
					});
					return false;
				}
			}
			ActPlan.warning = false;
			int num = this.cc.pos.Distance(this.pos);
			bool flag = num == 1 && this.cc.CanInteractTo(this.pos);
			AIAct aiact = this.act as AIAct;
			if (!this.act.IsAct)
			{
				if (repeated)
				{
					if (this.cc.ai.GetType() == this.act.GetType() && this.cc.ai.IsRunning)
					{
						return false;
					}
					aiact.Reset();
					if (!this.act.CanPerform())
					{
						this.cc.SetAI(Chara._NoGoalRepeat);
						return false;
					}
					Task task = aiact as Task;
					if (task != null)
					{
						task.isDestroyed = false;
						TaskPoint taskPoint = task as TaskPoint;
						if (EClass.scene.mouseTarget.isValid && taskPoint != null)
						{
							taskPoint.isRepeated = true;
							taskPoint.pos = EClass.scene.mouseTarget.pos.Copy();
						}
					}
				}
				this.cc.SetAIImmediate(aiact);
				ActionMode.Adv.SetTurbo(aiact.UseTurbo ? -1 : 0);
				return false;
			}
			if (this.act.PerformDistance != -1 && (num > this.act.PerformDistance || (num == 1 && !flag)))
			{
				this.cc.SetAIImmediate(new DynamicAIAct(this.act.GetText(""), () => this.act.Perform(this.cc, this.tc, this.pos), false)
				{
					pos = this.pos.Copy()
				});
				return false;
			}
			bool flag2 = this.act.Perform(this.cc, this.tc, this.pos);
			if (flag2 && !EClass.pc.HasNoGoal)
			{
				ActionMode.Adv.SetTurbo(-1);
			}
			return flag2 && EClass.pc.HasNoGoal;
		}

		public Act act;

		public Card tc;

		public Point pos;
	}
}
