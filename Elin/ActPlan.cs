using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ActPlan : EClass
{
	public class List : List<Item>
	{
		public void Add(Act a, string s = "")
		{
			Add(new Item
			{
				act = a
			});
		}
	}

	public class Item
	{
		public Act act;

		public Card tc;

		public Point pos;

		public Chara cc => EClass.pc;

		public bool HideHint
		{
			get
			{
				if (!EClass.pc.isBlind)
				{
					if (tc != null && tc.isChara)
					{
						return !EClass.pc.CanSee(tc);
					}
					return false;
				}
				return true;
			}
		}

		public string GetText(bool showName)
		{
			return act.GetText() + ((showName && act.GetTextSmall(tc) != null) ? ("\n<size=28>" + act.GetTextSmall(tc) + "</size>") : "");
		}

		public string GetTextContext(bool showName)
		{
			return act.GetText() + ((showName && tc != null && !HideHint) ? ("<size=13> (" + tc.Name + ")</size>") : "");
		}

		public bool Perform(bool repeated = false)
		{
			if (AM_Adv.actCount == 0 && !Dialog.warned)
			{
				warning = true;
				Chara _CC = Act.CC;
				Card _TC = Act.TC;
				Point _TP = new Point(Act.TP);
				if (EClass._zone.IsCrime(EClass.pc, act) && act.ID != "actContainer")
				{
					Dialog.TryWarnCrime(delegate
					{
						Act.CC = _CC;
						Act.TC = _TC;
						Act.TP.Set(_TP);
						if (Perform())
						{
							EClass.player.EndTurn();
						}
					});
					return false;
				}
				if (act is TaskHarvest && (act as TaskHarvest).mode == BaseTaskHarvest.HarvestType.Disassemble)
				{
					Dialog.TryWarnDisassemble(delegate
					{
						Act.CC = _CC;
						Act.TC = _TC;
						Act.TP.Set(_TP);
						if (Perform())
						{
							EClass.player.EndTurn();
						}
					});
					return false;
				}
			}
			warning = false;
			int num = cc.pos.Distance(pos);
			bool flag = num == 1 && cc.CanInteractTo(pos);
			AIAct aIAct = act as AIAct;
			if (act.IsAct)
			{
				if (act.PerformDistance != -1 && (num > act.PerformDistance || (num == 1 && !flag)))
				{
					cc.SetAIImmediate(new DynamicAIAct(act.GetText(), () => act.Perform(cc, tc, pos))
					{
						pos = pos.Copy()
					});
					return false;
				}
				bool num2 = act.Perform(cc, tc, pos);
				if (num2 && !EClass.pc.HasNoGoal)
				{
					ActionMode.Adv.SetTurbo();
				}
				if (num2)
				{
					return EClass.pc.HasNoGoal;
				}
				return false;
			}
			if (repeated)
			{
				if (cc.ai.GetType() == act.GetType() && cc.ai.IsRunning)
				{
					return false;
				}
				aIAct.Reset();
				if (!act.CanPerform())
				{
					cc.SetAI(Chara._NoGoalRepeat);
					return false;
				}
				if (aIAct is Task task)
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
			cc.SetAIImmediate(aIAct);
			ActionMode.Adv.SetTurbo(aIAct.UseTurbo ? (-1) : 0);
			return false;
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

	public List list = new List();

	public int dist;

	public Thing tool;

	public List<Thing> listPick = new List<Thing>();

	public Act lastAct;

	public bool IsSelf => dist == 0;

	public bool IsSelfOrNeighbor => _canInteractNeighbor;

	public bool IsNeighborBlocked
	{
		get
		{
			if (!_canInteractNeighbor)
			{
				return dist == 1;
			}
			return false;
		}
	}

	public bool canRepeat
	{
		get
		{
			if (list.Count == 1)
			{
				return list[0].act.CanPressRepeat;
			}
			return false;
		}
	}

	public Chara cc => EClass.pc;

	public CursorInfo CursorIcon
	{
		get
		{
			if (list.Count != 0)
			{
				if (list.Count <= 1)
				{
					return list[0].act.GetCursorIcon(list[0].tc);
				}
				return CursorSystem.Notice;
			}
			return null;
		}
	}

	public bool WillEndTurn
	{
		get
		{
			if (list.Count > 0)
			{
				return list[0].act.WillEndTurn;
			}
			return false;
		}
	}

	public bool HideRightInfo
	{
		get
		{
			if (list.Count > 0)
			{
				return list[0].act.HideRightInfo;
			}
			return false;
		}
	}

	public bool HasAct => list.Count > 0;

	public bool ShowAct
	{
		get
		{
			if (HasAct)
			{
				if (lastAct != list[0].act)
				{
					return list[0].act.ShowMouseHint(list[0].tc);
				}
				return false;
			}
			return false;
		}
	}

	public bool HasMultipleTargets
	{
		get
		{
			if (list.Count > 1)
			{
				return list[0].tc != list.LastItem().tc;
			}
			return false;
		}
	}

	public string GetText(bool showName)
	{
		if (list.Count == 0)
		{
			return "";
		}
		if (list.Count == 1)
		{
			return list[0].GetText(showName);
		}
		return "+" + list.Count + ((HasMultipleTargets || list[0].tc == null) ? ("\n<size=28>(" + "multipleTargets".lang() + ")</size>") : ("\n<size=28>" + list[0].tc.Name + list[0].tc.GetExtraName() + "</size>"));
	}

	public Func<bool> GetAction()
	{
		if (list.Count == 0)
		{
			return () => false;
		}
		if (list.Count > 1)
		{
			return delegate
			{
				if (pos.Equals(EClass.pc.pos) && ActWait.SearchMedal(EClass.pc, pos))
				{
					return false;
				}
				ShowContextMenu();
				return false;
			};
		}
		Item item = list[0];
		return delegate
		{
			if (performed && !item.act.CanPressRepeat)
			{
				return false;
			}
			performed = true;
			lastAct = item.act;
			return item.Perform(performed);
		};
	}

	public void ShowContextMenu()
	{
		UIContextMenu uIContextMenu = EClass.ui.CreateContextMenuInteraction();
		int num = 1;
		_ = showOrder;
		foreach (Item i in list)
		{
			string textContext = i.GetTextContext(HasMultipleTargets);
			textContext = textContext.Replace("\n", " ").Replace(Environment.NewLine, " ");
			uIContextMenu.AddButton(textContext, delegate
			{
				performed = true;
				if (i.Perform())
				{
					EClass.player.EndTurn();
				}
			});
			num++;
			if (num >= 21)
			{
				break;
			}
		}
		uIContextMenu.Show();
		EClass.ui.hud.HideMouseInfo();
	}

	public bool TrySetAct(string lang, Func<bool> onPerform, Card tc, CursorInfo cursor = null, int dist = 1, bool isHostileAct = false, bool localAct = true, bool canRepeat = false)
	{
		return TrySetAct(new DynamicAct(lang, onPerform)
		{
			id = lang,
			dist = dist,
			isHostileAct = isHostileAct,
			localAct = localAct,
			cursor = ((cursor == CursorSystem.Arrow) ? null : cursor),
			canRepeat = () => canRepeat
		}, tc);
	}

	public bool TrySetAct(string lang, Func<bool> onPerform, CursorInfo cursor = null, int dist = 1)
	{
		return TrySetAct(new DynamicAct(lang, onPerform)
		{
			id = lang,
			dist = dist,
			cursor = ((cursor == CursorSystem.Arrow) ? null : cursor)
		});
	}

	public bool TrySetAct(Act _act, Card _tc = null)
	{
		if (!ignoreAdddCondition && !_act.CanPerform(cc, _tc, pos))
		{
			return false;
		}
		Item item = new Item
		{
			act = _act,
			tc = _tc,
			pos = pos.Copy()
		};
		if (_tc != null && _tc.isChara)
		{
			int num = -1;
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].tc == _tc)
				{
					num = i;
				}
			}
			if (num != -1)
			{
				list.Insert(num + 1, item);
			}
			else
			{
				list.Add(item);
			}
		}
		else
		{
			list.Add(item);
		}
		return true;
	}

	public void Clear()
	{
		list.Clear();
		pos.IsValid = false;
	}

	public void Update(PointTarget target)
	{
		if (warning)
		{
			return;
		}
		_Update(target);
		if (!HasAct)
		{
			return;
		}
		Color effectColor = ((list.Count == 1) ? list[0].act.GetActPlanColor() : EClass.Colors.colorAct);
		if (input == ActInput.LeftMouse || input == ActInput.Key)
		{
			EClass.ui.hud.textLeft.SetText(GetText(showName: true));
			Outline[] components = EClass.ui.hud.textLeft.GetComponents<Outline>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].effectColor = effectColor;
			}
		}
		else if (input == ActInput.RightMouse)
		{
			EClass.player.currentHotItem.SetImage(EClass.ui.hud.imageRight);
			EClass.ui.hud.imageRight.rectTransform.localScale = new Vector3(2f, 2f, 1f);
			EClass.ui.hud.textRight.SetText(GetText(showName: true));
			Outline[] components = EClass.ui.hud.textRight.GetComponents<Outline>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].effectColor = effectColor;
			}
		}
	}

	public void _Update(PointTarget target)
	{
		showOrder = false;
		performed = false;
		list.Clear();
		listPick.Clear();
		if (!pos.Equals(target.pos))
		{
			lastAct = null;
		}
		pos.Set(target.pos);
		dist = cc.pos.Distance(pos);
		if (!pos.IsValid || EClass.pc.isDead)
		{
			return;
		}
		Point _pos = new Point(pos);
		_ = target.pos.cell;
		List<Card> items = _pos.ListCards();
		bool isKey = input == ActInput.Key;
		altAction = (EInput.isShiftDown && !EInput.isAltDown && !isKey) || input == ActInput.AllAction;
		_canInteractNeighbor = dist == 0 || (dist == 1 && cc.CanInteractTo(_pos));
		if (EClass.pc.isBlind && !_pos.Equals(EClass.pc.pos) && !isKey && input != 0)
		{
			return;
		}
		if (isKey || input == ActInput.LeftMouse || input == ActInput.AllAction)
		{
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
			if (!isKey && _pos.Equals(cc.pos) && EClass._zone.IsRegion)
			{
				TrySetAct("actNewZone", delegate
				{
					EClass.player.EnterLocalZone();
					return false;
				}, CursorSystem.MoveZone);
				if (input == ActInput.AllAction)
				{
					TrySetAct("LayerTravel", delegate
					{
						EClass.ui.AddLayer<LayerTravel>();
						return false;
					}, null, CursorSystem.MoveZone, 1, isHostileAct: false, localAct: false);
				}
			}
			items.ForeachReverse(delegate(Card _c)
			{
				Chara c = _c.Chara;
				if (c != null && !c.IsPC && EClass.pc.CanSee(c))
				{
					int num = c.Dist(EClass.pc);
					if (num <= 1 || !EClass.pc.isBlind)
					{
						if (!EClass.pc.isBlind && !c.IsHostile() && (input == ActInput.AllAction || !(c.IsPCParty || c.IsMinion || isKey)) && (input == ActInput.AllAction || !c.IsNeutral() || c.quest != null || EClass.game.quests.IsDeliverTarget(c)) && c.isSynced && num <= 2)
						{
							bool flag = !c.HasCondition<ConSuspend>() && (!c.isRestrained || !c.IsPCFaction);
							if (EClass._zone.instance is ZoneInstanceMusic && !c.IsPCFactionOrMinion)
							{
								flag = false;
							}
							if (flag || altAction)
							{
								if (EClass.pc.HasElement(1216) && c.HasCondition<ConSleep>())
								{
									TrySetAct(new AI_Fuck
									{
										target = c,
										succubus = true
									}, c);
								}
								TrySetAct(ACT.Chat, c);
							}
						}
						if (c.host != EClass.pc)
						{
							if (c.IsRestrainedResident)
							{
								TrySetAct(new AI_PracticeDummy
								{
									target = c
								});
							}
							else if ((c.IsHostile() || altAction || c.isRestrained) && c.IsAliveInCurrentZone)
							{
								TrySetAct(ACT.Melee, c);
							}
						}
						if (c.IsPCPartyMinion && !c.Chara.IsEscorted() && altAction)
						{
							TrySetAct("ActBanishSummon", delegate
							{
								EClass.pc.Say("summon_vanish", c);
								c.pos.PlayEffect("vanish");
								c.pos.PlaySound("vanish");
								c.pos.PlayEffect("teleport");
								c.Destroy();
								return true;
							}, c, null, 99);
						}
					}
				}
			});
			if (_pos.IsHidden || !IsSelfOrNeighbor)
			{
				return;
			}
			items.ForeachReverse(delegate(Card _c)
			{
				Chara c2 = _c.Chara;
				if (c2 != null)
				{
					bool flag2 = EClass.pc.CanSee(c2);
					if (flag2)
					{
						if (input == ActInput.LeftMouse && c2.IsPCFaction && !c2.IsPC && pos.FindThing<TraitHitchingPost>() != null)
						{
							Chara ride = c2;
							List<string> list = EClass.core.pccs.sets["ride"].map["body"].map.Keys.ToList();
							int index = list.IndexOf(ride.c_idRidePCC);
							if (index == -1)
							{
								index = 0;
							}
							TrySetAct("ActChangeRideSkin", delegate
							{
								UIContextMenu uIContextMenu = EClass.ui.CreateContextMenuInteraction();
								uIContextMenu.AddSlider("rideSkin", (float a) => list[(int)a].Split('-')[0] ?? "", index, delegate(float a)
								{
									ride.c_idRidePCC = list[(int)a];
									ride._CreateRenderer();
								}, 0f, list.Count - 1, isInt: true, hideOther: false);
								uIContextMenu.Show();
								return false;
							}, c2);
						}
						if (!c2.IsPC && ((c2.IsPCFaction && !c2.IsDisabled) || EClass.debug.enable) && input == ActInput.AllAction)
						{
							TrySetAct("actTrade", delegate
							{
								LayerInventory.CreateContainer(c2);
								return false;
							}, c2);
						}
						if (c2.host != null && EClass.pc.held != null && altAction)
						{
							bool flag3 = true;
							if ((EClass.pc.held.trait is TraitThrown || EClass.pc.held.trait.IsTool) && !HotItemHeld.disableTool)
							{
								flag3 = false;
							}
							if (!c2.IsDisabled && flag3 && c2.CanAcceptGift(EClass.pc, EClass.pc.held))
							{
								string lang = "actGive";
								if (c2.Evalue(1232) > 0 && EClass.pc.held.trait is TraitDrinkMilkMother)
								{
									lang = "actMilk";
								}
								TrySetAct(lang, delegate
								{
									if (!c2.IsValidGiftWeight(EClass.pc.held, 1))
									{
										c2.Talk("tooHeavy");
										return true;
									}
									if (EClass.core.config.game.confirmGive)
									{
										Dialog.YesNo("dialogGive".lang(EClass.pc.held.GetName(NameStyle.Full, 1)), func);
									}
									else
									{
										func();
									}
									return true;
								}, c2);
							}
						}
					}
					if (input == ActInput.AllAction && EClass.pc.held != null && EClass.pc.held.trait is TraitDrink)
					{
						TrySetAct(c2.IsPC ? "actPour" : "ActThrow", delegate
						{
							ActThrow.Throw(EClass.pc, c2.pos, c2, EClass.pc.held.Split(1));
							return true;
						}, (c2.host != null) ? c2 : EClass.pc.held);
					}
					if (!c2.IsPC && c2.host == null && ((!EClass.pc.isBlind && flag2) || input != ActInput.AllAction))
					{
						if (c2.isRestrained && (input == ActInput.AllAction || (!c2.IsRestrainedResident && !c2.IsHostile())))
						{
							TrySetAct("ActUnrestrain", delegate
							{
								c2.TryUnrestrain(force: true, EClass.pc);
								return true;
							}, c2);
						}
						if (!EClass.pc.isBlind && flag2 && input == ActInput.AllAction)
						{
							TrySetAct(ACT.Kick, c2);
							if (c2.IsMofuable)
							{
								TrySetAct("ActCuddle", delegate
								{
									EClass.pc.Cuddle(c2);
									return true;
								}, c2);
							}
							if (EClass.debug.showExtra)
							{
								TrySetAct("inspect", delegate
								{
									c2.Inspect();
									return false;
								}, c2);
							}
						}
					}
				}
				else if (_c.isThing)
				{
					if (!EClass.pc.isBlind)
					{
						Thing t = _c.Thing;
						if (input == ActInput.AllAction)
						{
							if ((EClass.debug.enable || EClass.player.HasKeyItem("license_illumination")) && t.LightData != null)
							{
								if (t.c_lightColor != 0)
								{
									TrySetAct("actClearLight", delegate
									{
										t.c_lightColor = 0;
										t.RecalculateFOV();
										t.renderer.GetTC<TCExtra>()?.RefreshColor();
										return false;
									}, t);
								}
								TrySetAct("actSetLight", delegate
								{
									Color lightColor = t.LightColor;
									EClass.ui.AddLayer<LayerColorPicker>().SetColor(lightColor, lightColor, delegate(PickerState state, Color _c)
									{
										t.c_lightColor = (byte)Mathf.Clamp(_c.r * 32f, 1f, 31f) * 1024 + (byte)Mathf.Clamp(_c.g * 32f, 1f, 31f) * 32 + (byte)Mathf.Clamp(_c.b * 32f, 1f, 31f);
										t.RecalculateFOV();
										t.renderer.GetTC<TCExtra>()?.RefreshColor();
									});
									return false;
								}, t);
							}
							if (EClass.debug.enable && pos.cell.IsTopWater)
							{
								TrySetAct("(debug) Toggle Float", delegate
								{
									t.isFloating = !t.isFloating;
									return false;
								}, t);
							}
							if (!EClass._zone.IsUserZone || !t.isNPCProperty)
							{
								if (t.trait.CanEat(EClass.pc))
								{
									TrySetAct(new AI_Eat
									{
										target = t
									}, t);
								}
								if (t.trait.CanDrink(EClass.pc))
								{
									TrySetAct(new AI_Drink
									{
										target = t
									}, t);
								}
								if (t.trait.CanRead(EClass.pc))
								{
									TrySetAct(new AI_Read
									{
										target = t
									}, t);
								}
								if (t.trait.IsBlendBase)
								{
									TrySetAct("invBlend", delegate
									{
										LayerDragGrid.Create(new InvOwnerBlend(t));
										return true;
									}, t);
								}
							}
						}
						if (isKey)
						{
							_ = t.trait.CanBeAttacked;
						}
						else
						{
							if (t.placeState == PlaceState.roaming && (_pos.cell.blocked || t.ignoreAutoPick || altAction || input == ActInput.AllAction || _pos.Equals(EClass.pc.pos)) && EClass.pc.CanPick(t))
							{
								listPick.Add(t);
							}
							if (t.IsInstalled)
							{
								t.trait.TrySetToggleAct(this);
								t.trait.TrySetAct(this);
							}
						}
					}
				}
				void func()
				{
					EClass.pc.GiveGift(c2, EClass.pc.SplitHeld(1) as Thing);
				}
			});
			if (listPick.Count > 0)
			{
				if (listPick.Count == 1)
				{
					Thing _t = listPick[0];
					if (!EClass._zone.IsRegion || (!_t.ignoreAutoPick && _t.pos.Equals(EClass.pc.pos)) || altAction)
					{
						TrySetAct("actPickOne", delegate
						{
							EClass.pc.Pick(_t);
							return true;
						}, listPick[0], CursorSystem.Hand, 1, isHostileAct: false, localAct: false);
					}
				}
				else
				{
					IList<Card> _cards = items.Copy();
					TrySetAct("actPickAll", delegate
					{
						foreach (Card item in _cards)
						{
							if (item.isThing && item.placeState == PlaceState.roaming)
							{
								EClass.pc.Pick(item.Thing);
							}
						}
						return true;
					}, null, CursorSystem.Hand, 1, isHostileAct: false, localAct: false);
				}
			}
			if (input == ActInput.AllAction && pos.IsSky)
			{
				TrySetAct("actSkyJump", delegate
				{
					EClass.pc.FallFromZone();
					return false;
				});
			}
			if (!_pos.Equals(cc.pos))
			{
				return;
			}
			if (cc.held != null && !cc.held.IsHotItem)
			{
				TrySetAct("actPick", delegate
				{
					_ = cc.held;
					cc.PickHeld(msg: true);
					return false;
				}, cc.held, CursorSystem.Inventory, 1, isHostileAct: false, localAct: false);
			}
			else if (!HasAct && !cc.isRestrained)
			{
				TrySetAct(ACT.Wait);
			}
			if (EClass.pc.party.members.Count > 1)
			{
				showOrder = true;
			}
			if (input == ActInput.AllAction && EClass.pc.held != null && !EClass.pc.held.trait.CanOnlyCarry)
			{
				TrySetAct("actDrop", delegate
				{
					EClass.pc.DropThing(EClass.pc.held.Thing);
					return true;
				});
			}
			if (cc.isRestrained)
			{
				TrySetAct("ActUnrestrain", delegate
				{
					cc.TryUnrestrain(force: true, EClass.pc);
					return true;
				}, cc);
			}
		}
		else
		{
			if (input != ActInput.RightMouse)
			{
				return;
			}
			if (pos.cell.outOfBounds || EClass.ui.IsDragging)
			{
				return;
			}
			HotItem hotItem = EClass.player.currentHotItem;
			if (!hotItem.IsGameAction)
			{
				TrySetAct(hotItem.Name, delegate
				{
					hotItem.OnClick(hotItem.button, hotItem.hotbar);
					return false;
				}, null, -1);
			}
			else
			{
				hotItem.TrySetAct(this);
			}
			bool flag4 = EClass.game.config.autoCombat.enable && EClass.scene.mouseTarget.TargetChara != null;
			if (hotItem.Thing != null && hotItem.Thing.trait.DisableAutoCombat)
			{
				flag4 = false;
			}
			if (flag4)
			{
				Chara targetChara = EClass.scene.mouseTarget.TargetChara;
				bool flag5 = true;
				if (targetChara.hostility >= Hostility.Friend)
				{
					flag5 = false;
				}
				if (targetChara.hostility == Hostility.Neutral && EClass.game.config.autoCombat.bDontAutoAttackNeutral)
				{
					flag5 = false;
				}
				if (list.Count >= 2)
				{
					flag5 = false;
				}
				if (list.Count == 1)
				{
					if (targetChara.hostility >= Hostility.Neutral)
					{
						flag5 = false;
					}
					if (!list[0].act.ShowAuto)
					{
						flag5 = false;
					}
					if (EClass.player.currentHotItem is HotItemNoItem && targetChara.hostility <= Hostility.Enemy)
					{
						flag5 = true;
					}
				}
				if (flag5)
				{
					list.Clear();
					TrySetAct(new GoalAutoCombat(targetChara));
				}
			}
			if (list.Count == 0 && !EClass.core.config.test.toolNoPick)
			{
				HotItemNoItem._TrySetAct(this);
			}
			if ((bool)WidgetCurrentTool.Instance)
			{
				WidgetCurrentTool.Instance.placer.Refresh();
			}
		}
	}
}
