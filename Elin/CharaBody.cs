using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CharaBody : EClass
{
	public int[] rawSlots
	{
		get
		{
			return this.owner.rawSlots;
		}
	}

	public void SetOwner(Chara chara, bool deserialized = false)
	{
		this.owner = chara;
		if (deserialized)
		{
			if (this.rawSlots != null)
			{
				for (int i = 0; i < this.rawSlots.Length; i++)
				{
					this.AddBodyPart(this.rawSlots[i], null);
				}
			}
			foreach (Thing thing in this.owner.things)
			{
				if (thing.isEquipped)
				{
					int num = thing.c_equippedSlot - 1;
					if (num < 0 || num >= this.slots.Count)
					{
						thing.c_equippedSlot = 0;
					}
					else
					{
						this.slots[num].thing = thing;
						thing.elements.SetParent(this.owner);
					}
				}
			}
		}
	}

	public void Unequip(Thing thing, bool refresh = true)
	{
		if (!thing.isEquipped)
		{
			return;
		}
		this.Unequip(this.slots[thing.c_equippedSlot - 1], refresh);
	}

	public void UnequipAll(int idSlot)
	{
		foreach (BodySlot bodySlot in this.slots)
		{
			if (bodySlot.elementId == idSlot)
			{
				this.Unequip(bodySlot, true);
			}
		}
	}

	public void Unequip(BodySlot slot, bool refresh = true)
	{
		if (slot.thing == null)
		{
			return;
		}
		Thing thing = slot.thing;
		if (EClass.pc != null)
		{
			EClass.pc.faction.charaElements.OnUnequip(this.owner, thing);
		}
		thing.elements.SetParent(null);
		thing.c_equippedSlot = 0;
		slot.thing = null;
		if (this.owner.IsPC)
		{
			LayerChara.Refresh();
			LayerInventory.SetDirty(thing);
			WidgetEquip.SetDirty();
		}
		if (slot.elementId == 45 && EClass.core.IsGameStarted)
		{
			this.owner.RecalculateFOV();
		}
		if (refresh && this.owner.isCreated && EClass.core.IsGameStarted)
		{
			this.owner.Refresh(false);
			if (slot.elementId == 37 && this.owner.HasElement(1209, 1))
			{
				this.owner.Say("tail_free", this.owner, null, null);
			}
		}
	}

	public bool IsEquippable(Thing thing, BodySlot slot, bool text = true)
	{
		CharaBody.<>c__DisplayClass11_0 CS$<>8__locals1;
		CS$<>8__locals1.text = text;
		CS$<>8__locals1.slot = slot;
		if (CS$<>8__locals1.slot == null)
		{
			return false;
		}
		int elementId = CS$<>8__locals1.slot.elementId;
		if (elementId != 31)
		{
			if (elementId != 33)
			{
				if (elementId == 39)
				{
					if (this.owner.HasElement(1552, 1))
					{
						return CharaBody.<IsEquippable>g__CannotEquip|11_0(ref CS$<>8__locals1);
					}
				}
			}
			else if (this.owner.HasElement(1554, 1))
			{
				return CharaBody.<IsEquippable>g__CannotEquip|11_0(ref CS$<>8__locals1);
			}
		}
		else if (this.owner.HasElement(1555, 1))
		{
			return CharaBody.<IsEquippable>g__CannotEquip|11_0(ref CS$<>8__locals1);
		}
		if (this.IsTooHeavyToEquip(thing))
		{
			if (EClass.core.IsGameStarted && this.owner.IsPC && CS$<>8__locals1.text)
			{
				Msg.Say("tooHeavyToEquip", thing, null, null, null);
			}
			return false;
		}
		return true;
	}

	public bool IsTooHeavyToEquip(Thing thing)
	{
		return this.owner.HasElement(1204, 1) && thing.ChildrenAndSelfWeight > 1000 && thing.category.slot != 44;
	}

	public void UnqeuipIfTooHeavy(Thing t)
	{
		if (t.isEquipped)
		{
			BodySlot bodySlot = this.slots[t.c_equippedSlot - 1];
			if (this.IsTooHeavyToEquip(t))
			{
				this.owner.Say("tooHeavyToEquip", t, null, null);
				this.Unequip(t, true);
			}
		}
		if (!this.owner.IsPC && !t.isEquipped)
		{
			this.owner.TryEquip(t, false);
		}
	}

	public bool Equip(Thing thing, BodySlot slot = null, bool msg = true)
	{
		BodySlot bodySlot;
		if ((bodySlot = slot) == null)
		{
			bodySlot = (this.GetSlot(thing.category.slot, true, false) ?? this.GetSlot(thing.category.slot, false, false));
		}
		slot = bodySlot;
		if (slot == this.slotMainHand || slot == this.slotOffHand)
		{
			this.owner.combatCount = 10;
		}
		if (slot == null || slot.elementId != thing.category.slot)
		{
			string[] array = new string[6];
			array[0] = "could not equip:";
			array[1] = ((thing != null) ? thing.ToString() : null);
			array[2] = "/";
			int num = 3;
			BodySlot bodySlot2 = slot;
			array[num] = ((bodySlot2 != null) ? bodySlot2.ToString() : null);
			array[4] = "/";
			int num2 = 5;
			Chara chara = this.owner;
			array[num2] = ((chara != null) ? chara.ToString() : null);
			Debug.LogWarning(string.Concat(array));
			return false;
		}
		if (!this.IsEquippable(thing, slot, true))
		{
			return false;
		}
		if (slot.thing != null)
		{
			if (slot.thing == thing)
			{
				this.Unequip(slot, true);
				return false;
			}
			this.Unequip(slot, false);
		}
		this.Unequip(thing, false);
		if (thing.parent != this.owner)
		{
			if (msg && this.owner.IsPC && thing.parent is Thing)
			{
				Msg.Say("movedToEquip", thing, thing.parent as Thing, null, null);
			}
			this.owner.AddCard(thing);
		}
		slot.thing = thing;
		thing.c_equippedSlot = slot.index + 1;
		thing.elements.SetParent(this.owner);
		if (EClass.pc != null)
		{
			EClass.pc.faction.charaElements.OnEquip(this.owner, thing);
		}
		this.owner.SetTempHand(0, 0);
		if (this.owner.IsPC)
		{
			LayerChara.Refresh();
			LayerInventory.SetDirty(thing);
			WidgetEquip.SetDirty();
		}
		if (slot.elementId == 45 && EClass.core.IsGameStarted)
		{
			this.owner.RecalculateFOV();
		}
		if (this.owner.isCreated && EClass.core.IsGameStarted)
		{
			this.owner.Refresh(false);
		}
		if (this.owner.isCreated)
		{
			if (thing.Evalue(656) > 0)
			{
				thing.blessedState = BlessedState.Cursed;
			}
			if (thing.blessedState <= BlessedState.Cursed)
			{
				this.owner.Say("equipCursed", this.owner, null, null);
				this.owner.PlaySound("curse3", 1f, true);
			}
		}
		if (slot.elementId == 37 && this.owner.HasElement(1209, 1))
		{
			this.owner.Say("tail_covered", this.owner, null, null);
		}
		return true;
	}

	public void AddBodyPart(int ele, Thing thing = null)
	{
		BodySlot item = new BodySlot
		{
			elementId = ele,
			thing = thing,
			index = this.slots.Count
		};
		if (ele == 35)
		{
			if (this.slotMainHand == null)
			{
				this.slotMainHand = item;
			}
			else if (this.slotOffHand == null)
			{
				this.slotOffHand = item;
			}
		}
		if (ele == 41)
		{
			this.slotRange = item;
		}
		this.slots.Add(item);
	}

	public void RefreshBodyParts()
	{
		foreach (BodySlot bodySlot in this.slots)
		{
			int elementId = bodySlot.elementId;
			if (elementId == 35)
			{
				if (this.slotMainHand == null)
				{
					this.slotMainHand = bodySlot;
				}
				else if (this.slotOffHand == null)
				{
					this.slotOffHand = bodySlot;
				}
			}
			if (elementId == 41)
			{
				this.slotRange = bodySlot;
			}
		}
	}

	public void RemoveBodyPart(int ele)
	{
		int num = this.slots.FindIndex((BodySlot a) => a.elementId == ele);
		if (num != -1)
		{
			BodySlot bodySlot = this.slots[num];
			if (bodySlot.thing != null)
			{
				this.Unequip(bodySlot, true);
			}
			if (this.slotMainHand == bodySlot)
			{
				this.slotMainHand = null;
			}
			if (this.slotOffHand == bodySlot)
			{
				this.slotOffHand = null;
			}
			if (this.slotRange == bodySlot)
			{
				this.slotRange = null;
			}
			this.slots.RemoveAt(num);
		}
	}

	public BodySlot GetSlot(Thing t, bool onlyEmpty = false, bool secondSlot = false)
	{
		BodySlot slot = this.GetSlot(t.category.slot, true, secondSlot);
		if (slot != null)
		{
			return slot;
		}
		if (!onlyEmpty)
		{
			return this.GetSlot(t.category.slot, false, secondSlot);
		}
		return null;
	}

	public BodySlot GetSlot(int elementId, bool onlyEmpty = true, bool secondSlot = false)
	{
		bool flag = true;
		foreach (BodySlot bodySlot in this.slots)
		{
			if (elementId == bodySlot.elementId && (!onlyEmpty || bodySlot.thing == null))
			{
				if (!secondSlot || !flag)
				{
					return bodySlot;
				}
				flag = false;
			}
		}
		return null;
	}

	public Thing GetEquippedThing(int elementId)
	{
		foreach (BodySlot bodySlot in this.slots)
		{
			if (bodySlot.elementId == elementId && bodySlot.thing != null)
			{
				return bodySlot.thing;
			}
		}
		return null;
	}

	public int GetWeight(bool armorOnly = false)
	{
		int num = 0;
		foreach (BodySlot bodySlot in this.slots)
		{
			if (bodySlot.thing != null && (!armorOnly || (bodySlot.elementId != 44 && bodySlot.elementId != 45)))
			{
				num += bodySlot.thing.ChildrenAndSelfWeight;
			}
		}
		return num;
	}

	public int GetAttackIndex(Thing t)
	{
		int num = 0;
		foreach (BodySlot bodySlot in this.slots)
		{
			if (bodySlot.thing != null && bodySlot.elementId == 35 && bodySlot.thing.source.offense.Length >= 2)
			{
				if (bodySlot.thing == t)
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	public int GetMeleeDistance()
	{
		int num = this.owner.Evalue(666);
		if (num == 0)
		{
			return 1;
		}
		num = 0;
		foreach (BodySlot bodySlot in this.slots)
		{
			if (bodySlot.elementId == 35 && bodySlot.thing != null && bodySlot.thing.Evalue(666) > num)
			{
				num = bodySlot.thing.Evalue(666);
			}
		}
		return 1 + num;
	}

	public AttackStyle GetAttackStyle()
	{
		bool flag = false;
		int num = 0;
		foreach (BodySlot bodySlot in this.slots)
		{
			if (bodySlot.elementId == 35 && bodySlot.thing != null)
			{
				if (bodySlot.thing.IsMeleeWeapon)
				{
					num++;
				}
				else
				{
					flag = true;
				}
			}
		}
		if (num > 1)
		{
			return AttackStyle.TwoWield;
		}
		if (flag)
		{
			return AttackStyle.Shield;
		}
		if (num == 1)
		{
			return AttackStyle.TwoHand;
		}
		return AttackStyle.Default;
	}

	public int GetAttackStyleElement(AttackStyle style)
	{
		if (style == AttackStyle.TwoHand)
		{
			return 130;
		}
		if (style != AttackStyle.TwoWield)
		{
			return 0;
		}
		return 131;
	}

	public int GetSortVal(BodySlot slot)
	{
		int num = slot.element.sort * 10;
		if (slot.elementId == 35)
		{
			num += this.owner.body.slots.IndexOf(slot);
		}
		return -num;
	}

	public void SetBodyIndexText(BodySlot b, UIText t)
	{
		if (!t)
		{
			return;
		}
		if (b.elementId == 35)
		{
			if (b.indexHnd == 0)
			{
				int num = 0;
				foreach (BodySlot bodySlot in this.slots)
				{
					if (bodySlot.elementId == 35)
					{
						num++;
					}
					if (b == bodySlot)
					{
						break;
					}
				}
				b.indexHnd = num;
			}
			t.SetText(b.indexHnd.ToString() ?? "");
			t.SetActive(true);
			return;
		}
		t.SetActive(false);
	}

	[CompilerGenerated]
	internal static bool <IsEquippable>g__CannotEquip|11_0(ref CharaBody.<>c__DisplayClass11_0 A_0)
	{
		if (A_0.text)
		{
			Msg.Say("cannnotEquip", A_0.slot.element.GetName().ToLower(), null, null, null);
		}
		return false;
	}

	public Chara owner;

	public List<BodySlot> slots = new List<BodySlot>();

	public BodySlot slotMainHand;

	public BodySlot slotOffHand;

	public BodySlot slotRange;
}
