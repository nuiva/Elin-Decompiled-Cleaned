using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAbility : UIButton, IMouseHint
{
	public SourceElement.Row source;

	public IDragParent dragParent;

	public UIText textStock;

	public Image imageType;

	public Image imageFav;

	public Transform attach;

	public Transform transFav;

	public Chara chara;

	public Act act;

	[NonSerialized]
	public bool dragged;

	public static float hotkeyTimer;

	public void SetAct(Chara _chara, Element e)
	{
		source = e.source;
		chara = _chara;
		act = ACT.Create(source);
		if (act == null)
		{
			Debug.Log(source.alias);
		}
		Sprite iconType = act.TargetType.IconType;
		imageType.SetActive(iconType);
		imageType.sprite = iconType;
		act.SetImage(icon);
		if (EClass.game.altAbility)
		{
			textStock.SetActive(e is Spell);
			textStock.text = e.vPotential.ToString() ?? "";
			onRightClick = delegate
			{
				if (EClass.ui.IsActive)
				{
					SE.BeepSmall();
				}
				else
				{
					Use();
				}
			};
		}
		else
		{
			mainText.SetText(e.Name);
		}
		SetTooltip("note", delegate(UITooltip t)
		{
			e.WriteNote(t.note, chara.elements, delegate
			{
				e._WriteNote(t.note, chara, act);
			});
		});
		RefreshFavIcon();
	}

	public void OnDrag(PointerEventData data)
	{
		if (dragParent != null && data.button == PointerEventData.InputButton.Left)
		{
			if (!dragged)
			{
				dragged = true;
				dragParent.OnStartDrag(this);
				OnPointerUpOnDrag(data);
			}
			else
			{
				dragParent.OnDrag(this);
			}
		}
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		if (dragged)
		{
			dragged = false;
			dragParent.OnEndDrag(this);
		}
		else
		{
			base.OnPointerUp(eventData);
		}
	}

	public void Use()
	{
		if (!EClass.pc.HasNoGoal)
		{
			SE.BeepSmall();
		}
		else if (CanAutoUse(act))
		{
			if (TryUse(act) && EClass.pc.ai.IsNoGoal)
			{
				EClass.player.EndTurn();
			}
		}
		else
		{
			HoldAbility();
		}
	}

	public static bool TryUse(Act act, Card tg = null, Point pos = null, Card catalyst = null, bool first = true, bool mouse = true)
	{
		bool flag = false;
		if (tg == null)
		{
			tg = EClass.pc;
		}
		if (pos == null)
		{
			pos = EClass.pc.pos.Copy();
		}
		if (first && EInput.GetHotkey() != -1)
		{
			mouse = false;
			hotkeyTimer = 0f;
			Debug.Log(EInput.GetHotkey());
		}
		if (act.HaveLongPressAction)
		{
			if ((mouse && EInput.rightMouse.pressedLong) || (!mouse && hotkeyTimer >= 0.45f))
			{
				EInput.rightMouse.Consume();
				flag = true;
				hotkeyTimer = 1f;
			}
			if ((mouse && EInput.rightMouse.pressing) || (!mouse && EInput.GetHotkey() != -1 && hotkeyTimer < 1f))
			{
				hotkeyTimer += Core.delta;
				EClass.core.actionsNextFrame.Add(delegate
				{
					TryUse(act, tg, pos, catalyst, first: false, mouse);
				});
				if (first)
				{
					EClass.core.actionsNextFrame.Add(delegate
					{
						EInput.rightMouse.down = false;
						EInput.rightMouse.consumed = false;
						EInput.rightMouse.pressing = true;
						EInput.rightMouse.usedMouse = true;
					});
				}
				return false;
			}
		}
		if (flag && SpecialHoldAction(act))
		{
			EClass.player.EndTurn();
		}
		else if (EClass.pc.UseAbility(act.source.alias, tg, pos, flag))
		{
			EClass.player.EndTurn();
		}
		if (catalyst != null)
		{
			LayerInventory.SetDirty(catalyst.Thing);
		}
		return false;
	}

	public static bool SpecialHoldAction(Act act)
	{
		Act e = EClass.pc.elements.GetElement(act.id) as Act;
		if (e == null)
		{
			return false;
		}
		int id = e.id;
		if (id == 8230 || id == 8232)
		{
			bool stop = false;
			bool first = true;
			int count = 0;
			EClass.pc.things.Foreach(delegate(Thing t)
			{
				if (!t.IsIdentified)
				{
					if (EClass.pc.mana.value < e.GetCost(EClass.pc).cost && !first)
					{
						stop = true;
					}
					if (e.vPotential <= 0)
					{
						stop = true;
					}
					if (!stop && (t.rarity < Rarity.Mythical || e.id != 8230))
					{
						EClass.pc.UseAbility(act.source.alias, t, EClass.pc.pos);
						count++;
						first = false;
					}
				}
			});
			if (count == 0)
			{
				Msg.Say("identify_nothing");
			}
			return true;
		}
		return false;
	}

	public void HoldAbility()
	{
		EClass.player.SetCurrentHotItem(new HotItemAct(source));
		SE.SelectHotitem();
	}

	public bool CanAutoUse(Act _act)
	{
		if (_act.TargetType.CanSelectSelf)
		{
			if (EClass._zone.IsRegion)
			{
				return !_act.LocalAct;
			}
			return true;
		}
		return false;
	}

	public bool ShowMouseHintLeft()
	{
		return false;
	}

	public string GetTextMouseHintLeft()
	{
		return "";
	}

	public bool ShowMouseHintRight()
	{
		return CanAutoUse(ACT.Create(source));
	}

	public string GetTextMouseHintRight()
	{
		return "actUse".lang();
	}

	public override bool CanMiddleClick()
	{
		return EClass.ui.AllowInventoryInteractions;
	}

	public override void OnMiddleClick(bool forceClick)
	{
		if (EInput.middleMouse.clicked)
		{
			ToggleFav();
		}
		EInput.middleMouse.pressedLongAction = delegate
		{
			if (EClass.ui.AllowInventoryInteractions)
			{
				HoldAbility();
				EInput.Consume();
			}
		};
	}

	public void ToggleFav()
	{
		if (EClass.player.favAbility.Contains(source.id))
		{
			EClass.player.favAbility.Remove(source.id);
			SE.Tab();
		}
		else
		{
			EClass.player.favAbility.Add(source.id);
			SE.Tab();
		}
		RefreshFavIcon();
		LayerAbility.Instance.list.Redraw();
	}

	public void RefreshFavIcon()
	{
		bool enable = EClass.player.favAbility.Contains(source.id);
		imageFav.SetActive(enable);
	}
}
