using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class HotItemEQSet : HotAction
{
	public override string Id
	{
		get
		{
			return "EQSet";
		}
	}

	public override bool CanChangeIconColor
	{
		get
		{
			return true;
		}
	}

	public override string TextTip
	{
		get
		{
			string text = this.text.IsEmpty("EQSet".lang()) + base.TextHotkey() + Environment.NewLine;
			using (List<int>.Enumerator enumerator = this.ids.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int i = enumerator.Current;
					Thing thing = EClass.pc.things.Find((Thing t) => t.uid == i, true);
					text += ((thing == null) ? "missingEQ".lang() : thing.Name);
					text += Environment.NewLine;
				}
			}
			return text.TrimEnd(Environment.NewLine.ToCharArray());
		}
	}

	public HotItemEQSet Register()
	{
		this.ids.Clear();
		foreach (BodySlot bodySlot in EClass.pc.body.slots)
		{
			if (bodySlot.elementId != 44 && bodySlot.thing != null)
			{
				this.ids.Add(bodySlot.thing.uid);
			}
		}
		SE.Equip();
		return this;
	}

	public override void Perform()
	{
		for (int j = 0; j < 2; j++)
		{
			Dictionary<int, Thing> dictionary = new Dictionary<int, Thing>();
			foreach (BodySlot bodySlot in EClass.pc.body.slots)
			{
				if (bodySlot.elementId != 44 && bodySlot.thing != null && bodySlot.thing.blessedState >= BlessedState.Normal)
				{
					dictionary.Add(bodySlot.thing.c_equippedSlot - 1, bodySlot.thing);
					EClass.pc.body.Unequip(bodySlot.thing, true);
				}
			}
			Card card = null;
			using (List<int>.Enumerator enumerator2 = this.ids.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					int i = enumerator2.Current;
					Thing thing = EClass.pc.things.Find((Thing t) => t.uid == i && !t.isEquipped, true);
					if (thing != null)
					{
						BodySlot slot = EClass.pc.body.GetSlot(thing, true, false);
						if (slot != null && thing != slot.thing)
						{
							Card parentCard = thing.parentCard;
							int invX = thing.invX;
							int invY = thing.invY;
							EClass.pc.body.Equip(thing, slot, false);
							if (dictionary.ContainsKey(slot.index))
							{
								Thing thing2 = dictionary[slot.index];
								if (thing2.uid != thing.uid)
								{
									parentCard.AddThing(thing2, true, -1, -1);
									card = parentCard;
									thing2.invX = invX;
									thing2.invY = invY;
									dictionary.Remove(slot.index);
								}
							}
						}
					}
				}
			}
			if (card != null)
			{
				foreach (Thing thing3 in dictionary.Values)
				{
					if (!thing3.isEquipped && !card.things.IsFull(thing3, true, true))
					{
						card.AddThing(thing3, true, -1, -1);
					}
				}
			}
		}
		SE.Equip();
	}

	public override void OnShowContextMenu(UIContextMenu m)
	{
		base.OnShowContextMenu(m);
		m.AddButton("updateEQ", delegate()
		{
			this.Register();
		}, true);
	}

	[JsonProperty]
	public List<int> ids = new List<int>();

	public class Item
	{
		public Card container;

		public int invX;

		public int invY;
	}
}
