using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class QuestDeliver : QuestDestZone
{
	public virtual bool ConsumeGoods
	{
		get
		{
			return true;
		}
	}

	public SourceThing.Row sourceThing
	{
		get
		{
			return EClass.sources.things.map[this.idThing.IsEmpty("generator_snowman")];
		}
	}

	public override string NameDeliver
	{
		get
		{
			return this.sourceThing.GetName();
		}
	}

	public override string RefDrama2
	{
		get
		{
			return this.NameDeliver;
		}
	}

	public override int KarmaOnFail
	{
		get
		{
			if (!this.IsDeliver)
			{
				return base.KarmaOnFail;
			}
			return -15;
		}
	}

	public override Quest.DifficultyType difficultyType
	{
		get
		{
			return Quest.DifficultyType.Deliver;
		}
	}

	public override bool ForbidTeleport
	{
		get
		{
			return true;
		}
	}

	public override int GetExtraMoney()
	{
		if (base.DestZone == null)
		{
			return 0;
		}
		return base.DestZone.Dist(base.ClientZone) * 6;
	}

	public override void OnInit()
	{
		this.num = this.GetDestNum();
		this.SetIdThing();
	}

	public virtual int GetDestNum()
	{
		return 1;
	}

	public virtual void SetIdThing()
	{
		CardRow cardRow;
		do
		{
			SourceCategory.Row r = this.GetDeliverCat();
			cardRow = SpawnListThing.Get("cat_" + r.id, (SourceThing.Row s) => EClass.sources.categories.map[s.category].IsChildOf(r.id)).Select(-1, -1);
		}
		while (cardRow == null);
		this.idThing = cardRow.id;
	}

	private SourceCategory.Row GetDeliverCat()
	{
		if (QuestDeliver._listDeliver.Count == 0)
		{
			foreach (SourceCategory.Row row in EClass.sources.categories.rows)
			{
				if (row.deliver > 0)
				{
					QuestDeliver._listDeliver.Add(row);
				}
			}
		}
		return QuestDeliver._listDeliver.RandomItem<SourceCategory.Row>();
	}

	public override void OnStart()
	{
		if (this.IsDeliver)
		{
			Thing thing = ThingGen.Create(this.idThing, -1, -1);
			thing.Identify(false, IDTSource.SuperiorIdentify);
			Msg.Say("get_quest_item");
			EClass.pc.Pick(thing, true, true);
		}
	}

	public virtual bool IsDestThing(Thing t)
	{
		if (t.parentCard != null && (t.parentCard.c_lockLv != 0 || t.parentCard.trait is TraitChestMerchant))
		{
			return false;
		}
		if (!t.c_isImportant && t.Num >= this.num && t.IsIdentified && t.things.Count == 0 && !t.isEquipped)
		{
			if (t.id == this.idThing)
			{
				return true;
			}
			if (t.c_altName.IsEmpty())
			{
				string name = this.sourceThing.GetName();
				if (t.source.GetName() == name || t.GetName(NameStyle.Simple, 1) == name)
				{
					return true;
				}
			}
		}
		return false;
	}

	public List<Thing> ListDestThing()
	{
		List<Thing> list = EClass.pc.things.List((Thing t) => this.IsDestThing(t), false);
		if (!this.IsDeliver && EClass._zone.IsPCFaction)
		{
			foreach (Thing thing in EClass._map.props.stocked.things)
			{
				if (this.IsDestThing(thing))
				{
					list.Add(thing);
				}
			}
		}
		return list;
	}

	public override Thing GetDestThing()
	{
		List<Thing> list = this.ListDestThing();
		if (list.Count <= 0)
		{
			return null;
		}
		return list[0];
	}

	public override bool IsDeliverTarget(Chara c)
	{
		if (this.IsDeliver)
		{
			return EClass._zone == base.DestZone && c.uid == this.uidTarget;
		}
		return c.quest != null && c.quest.uid == this.uid;
	}

	public override bool CanDeliverToClient(Chara c)
	{
		if (this.GetDestThing() == null && !EClass.debug.autoAdvanceQuest)
		{
			return false;
		}
		if (this.IsDeliver)
		{
			return EClass._zone == base.DestZone && c.uid == this.uidTarget;
		}
		Chara chara = this.person.chara;
		int? num = (chara != null) ? new int?(chara.uid) : null;
		int uid = c.uid;
		return (num.GetValueOrDefault() == uid & num != null) || (c.quest != null && c.quest.uid == this.uid);
	}

	public override bool CanDeliverToBox(Thing t)
	{
		return false;
	}

	public override bool Deliver(Chara c, Thing t = null)
	{
		if (t == null)
		{
			t = this.GetDestThing();
			if (t == null && EClass.debug.autoAdvanceQuest)
			{
				Debug.Log("[error] creating " + this.idThing);
				t = ThingGen.Create(this.idThing, -1, -1);
			}
		}
		if (t != null)
		{
			Thing thing = t.Split(this.num);
			this.bonusMoney += this.GetBonus(thing);
			Msg.Say("deliverItem", thing, null, null, null);
			if (this.ConsumeGoods)
			{
				thing.Destroy();
			}
			else
			{
				c.Pick(thing, true, true);
			}
			EClass.game.quests.Complete(this);
			c.quest = null;
			return true;
		}
		return false;
	}

	public virtual int GetBonus(Thing t)
	{
		return 0;
	}

	public override string GetTextProgress()
	{
		string text = (this.GetDestThing() != null) ? "supplyInInv".lang().TagColor(FontColor.Good, null) : "supplyNotInInv".lang();
		if (this.IsDeliver)
		{
			string @ref = (base.DestZone.dictCitizen.TryGetValue(this.uidTarget, null) ?? "???") + " (" + base.DestZone.Name + ")";
			return "progressDeliver".lang(this.sourceThing.GetName(this.num), @ref, text, null, null);
		}
		return "progressSupply".lang(this.sourceThing.GetName(this.num) + Lang.space + this.TextExtra2.IsEmpty(""), text, null, null, null);
	}

	public override void OnEnterZone()
	{
		if (!this.IsDeliver)
		{
			return;
		}
		if (EClass._zone == base.DestZone)
		{
			Chara chara = EClass._map.charas.Find((Chara c) => c.uid == this.uidTarget);
			if (chara == null)
			{
				chara = EClass._map.deadCharas.Find((Chara c) => c.uid == this.uidTarget);
			}
			if (chara == null)
			{
				Msg.Say("deliver_funny");
				base.Complete();
			}
		}
	}

	[JsonProperty]
	public string idThing;

	[JsonProperty]
	public int num;

	private static List<SourceCategory.Row> _listDeliver = new List<SourceCategory.Row>();
}
