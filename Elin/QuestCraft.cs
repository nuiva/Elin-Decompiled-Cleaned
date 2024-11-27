using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class QuestCraft : Quest
{
	public bool ConsumeGoods
	{
		get
		{
			return false;
		}
	}

	public List<QuestCraft.Req> CurrentReq
	{
		get
		{
			if (this.progress != 0)
			{
				return this.req2;
			}
			return this.req1;
		}
	}

	public bool hasSecondReq
	{
		get
		{
			return this.req2.Count > 0;
		}
	}

	public override bool CanDeliverToClient(Chara c)
	{
		if (c.quest != this)
		{
			return false;
		}
		bool result = true;
		foreach (QuestCraft.Req req in this.CurrentReq)
		{
			Thing thing = EClass.pc.things.Find(req.idThing, -1, -1);
			if (thing == null || thing.Num < req.num)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	public override bool Deliver(Chara c, Thing t = null)
	{
		if (!this.CanDeliverToClient(c))
		{
			return false;
		}
		bool consumeGoods = this.ConsumeGoods;
		if (this.progress == 0 && this.hasSecondReq)
		{
			base.UpdateJournal();
		}
		else
		{
			EClass.game.quests.Complete(this);
		}
		this.OnProgressComplete();
		return true;
	}

	public void OnProgressComplete()
	{
		if (this.progress == 0)
		{
			string id = this.id;
			if (!(id == "fiama1"))
			{
				if (id == "ash1")
				{
					EClass._zone.AddCard(ThingGen.Create("string", -1, -1), EClass.pc.pos);
				}
			}
			else
			{
				EClass._zone.AddCard(ThingGen.Create("water", -1, -1), EClass.pc.pos);
			}
		}
		this.progress++;
	}

	public override void OnDropReward()
	{
		string id = this.id;
		if (id == "fiama1")
		{
			base.DropReward(ThingGen.Create("crimAle", -1, -1));
			return;
		}
		if (!(id == "ash1"))
		{
			return;
		}
		base.DropReward(ThingGen.Create("crimAle", -1, -1));
	}

	public override void OnComplete()
	{
		string id = this.id;
		if (id == "ash1")
		{
			QuestCraft questCraft = Quest.Create("ash2", null, null) as QuestCraft;
			questCraft.SetClient(base.chara, true);
			questCraft.req1.Add(new QuestCraft.Req("torch_held", 1));
			return;
		}
		if (!(id == "fiama1"))
		{
			return;
		}
		QuestCraft questCraft2 = Quest.Create("fiama2", null, null) as QuestCraft;
		questCraft2.SetClient(base.chara, true);
		questCraft2.req1.Add(new QuestCraft.Req("torch_held", 1));
	}

	public override string GetTextProgress()
	{
		string text = "";
		foreach (QuestCraft.Req req in this.CurrentReq)
		{
			text = text + (text.IsEmpty() ? "" : Environment.NewLine) + "progressShowSupply".lang(EClass.sources.things.map[req.idThing].GetName(req.num), null, null, null, null);
		}
		return text;
	}

	public override string GetDetailText(bool onJournal = false)
	{
		return base.source.GetDetail().Split('|', StringSplitOptions.None)[this.progress];
	}

	public override string GetTalkProgress()
	{
		return this.GetDetailText(false);
	}

	public override string GetTalkComplete()
	{
		if (this.progress != 1 || !this.hasSecondReq)
		{
			return base.Parse(base.source.GetText("talkComplete", false));
		}
		return this.GetDetailText(false);
	}

	[JsonProperty]
	public List<QuestCraft.Req> req1 = new List<QuestCraft.Req>();

	[JsonProperty]
	public List<QuestCraft.Req> req2 = new List<QuestCraft.Req>();

	[JsonProperty]
	public int progress;

	public class Req
	{
		public Req()
		{
		}

		public Req(string id, int n)
		{
			this.idThing = id;
			this.num = n;
		}

		[JsonProperty]
		public string idThing;

		[JsonProperty]
		public int num;
	}
}
