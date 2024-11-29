using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAbility : UIButton, IMouseHint
{
	public void SetAct(Chara _chara, Element e)
	{
		this.source = e.source;
		this.chara = _chara;
		this.act = ACT.Create(this.source);
		if (this.act == null)
		{
			Debug.Log(this.source.alias);
		}
		Sprite iconType = this.act.TargetType.IconType;
		this.imageType.SetActive(iconType);
		this.imageType.sprite = iconType;
		this.act.SetImage(this.icon);
		if (EClass.game.altAbility)
		{
			this.textStock.SetActive(e is Spell);
			this.textStock.text = (e.vPotential.ToString() ?? "");
			this.onRightClick = delegate()
			{
				if (EClass.ui.IsActive)
				{
					SE.BeepSmall();
					return;
				}
				this.Use();
			};
		}
		else
		{
			this.mainText.SetText(e.Name);
		}
		base.SetTooltip("note", delegate(UITooltip t)
		{
			e.WriteNote(t.note, this.chara.elements, delegate(UINote n)
			{
				e._WriteNote(t.note, this.chara, this.act);
			});
		}, true);
		this.RefreshFavIcon();
	}

	public void OnDrag(PointerEventData data)
	{
		if (this.dragParent == null)
		{
			return;
		}
		if (data.button != PointerEventData.InputButton.Left)
		{
			return;
		}
		if (!this.dragged)
		{
			this.dragged = true;
			this.dragParent.OnStartDrag(this);
			base.OnPointerUpOnDrag(data);
			return;
		}
		this.dragParent.OnDrag(this);
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		if (this.dragged)
		{
			this.dragged = false;
			this.dragParent.OnEndDrag(this, false);
			return;
		}
		base.OnPointerUp(eventData);
	}

	public void Use()
	{
		if (!EClass.pc.HasNoGoal)
		{
			SE.BeepSmall();
			return;
		}
		if (this.CanAutoUse(this.act))
		{
			if (ButtonAbility.TryUse(this.act, null, null, null, true, true) && EClass.pc.ai.IsNoGoal)
			{
				EClass.player.EndTurn(true);
				return;
			}
		}
		else
		{
			this.HoldAbility();
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
			ButtonAbility.hotkeyTimer = 0f;
			Debug.Log(EInput.GetHotkey());
		}
		if (act.HaveLongPressAction)
		{
			if ((mouse && EInput.rightMouse.pressedLong) || (!mouse && ButtonAbility.hotkeyTimer >= 0.45f))
			{
				EInput.rightMouse.Consume();
				flag = true;
				ButtonAbility.hotkeyTimer = 1f;
			}
			if ((mouse && EInput.rightMouse.pressing) || (!mouse && EInput.GetHotkey() != -1 && ButtonAbility.hotkeyTimer < 1f))
			{
				ButtonAbility.hotkeyTimer += Core.delta;
				EClass.core.actionsNextFrame.Add(delegate
				{
					ButtonAbility.TryUse(act, tg, pos, catalyst, false, mouse);
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
		if (flag && ButtonAbility.SpecialHoldAction(act))
		{
			EClass.player.EndTurn(true);
		}
		else if (EClass.pc.UseAbility(act.source.alias, tg, pos, flag))
		{
			EClass.player.EndTurn(true);
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
				if (t.IsIdentified)
				{
					return;
				}
				if (EClass.pc.mana.value < e.GetCost(EClass.pc).cost && !first)
				{
					stop = true;
				}
				if (e.vPotential <= 0)
				{
					stop = true;
				}
				if (stop)
				{
					return;
				}
				if (t.rarity >= Rarity.Mythical && e.id == 8230)
				{
					return;
				}
				EClass.pc.UseAbility(act.source.alias, t, EClass.pc.pos, false);
				int count = count;
				count++;
				first = false;
			}, true);
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
		EClass.player.SetCurrentHotItem(new HotItemAct(this.source));
		SE.SelectHotitem();
	}

	public bool CanAutoUse(Act _act)
	{
		return _act.TargetType.CanSelectSelf && (!EClass._zone.IsRegion || !_act.LocalAct);
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
		return this.CanAutoUse(ACT.Create(this.source));
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
			this.ToggleFav();
		}
		EInput.middleMouse.pressedLongAction = delegate()
		{
			if (EClass.ui.AllowInventoryInteractions)
			{
				this.HoldAbility();
				EInput.Consume(false, 1);
			}
		};
	}

	public void ToggleFav()
	{
		if (EClass.player.favAbility.Contains(this.source.id))
		{
			EClass.player.favAbility.Remove(this.source.id);
			SE.Tab();
		}
		else
		{
			EClass.player.favAbility.Add(this.source.id);
			SE.Tab();
		}
		this.RefreshFavIcon();
		LayerAbility.Instance.list.Redraw();
	}

	public void RefreshFavIcon()
	{
		bool enable = EClass.player.favAbility.Contains(this.source.id);
		this.imageFav.SetActive(enable);
	}

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
}
