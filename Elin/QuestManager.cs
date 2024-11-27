using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class QuestManager : EClass
{
	public QuestMain Main
	{
		get
		{
			return this.Get<QuestMain>();
		}
	}

	public new QuestHome Home
	{
		get
		{
			return this.Get<QuestHome>();
		}
	}

	public Quest Add(string id, string idGlobalChara = null)
	{
		Quest quest = Quest.Create(id, null, null);
		if (idGlobalChara.IsEmpty())
		{
			idGlobalChara = quest.source.drama[0];
		}
		quest.SetClient(EClass.game.cards.globalCharas.Find(idGlobalChara), false);
		EClass.game.quests.globalList.Add(quest);
		return quest;
	}

	public Quest Start(string id, string idGlobalChara)
	{
		Quest q = Quest.Create(id, null, null).SetClient(EClass.game.cards.globalCharas.Find(idGlobalChara), false);
		return this.Start(q);
	}

	public Quest Start(string id, Chara c = null, bool assignQuest = true)
	{
		return this.Start(Quest.Create(id, null, null).SetClient(c, assignQuest));
	}

	public Quest Start(Quest q)
	{
		this.list.Insert(0, q);
		q.Start();
		if (EClass.core.IsGameStarted)
		{
			q.UpdateJournal();
			if (EClass.player.questTracker)
			{
				WidgetQuestTracker.Show();
			}
		}
		return q;
	}

	public void Remove(Quest q)
	{
		if (!this.list.Remove(q))
		{
			string[] array = new string[10];
			array[0] = "exception: Failed to remove quest:";
			int num = 1;
			Quest quest = q;
			array[num] = ((quest != null) ? quest.ToString() : null);
			array[2] = "/";
			array[3] = q.uid.ToString();
			array[4] = "/";
			array[5] = q.source.id;
			array[6] = "/";
			array[7] = q.deadline.ToString();
			array[8] = "/";
			array[9] = q.Hours.ToString();
			Debug.Log(string.Concat(array));
			q = this.Get(q.uid);
			Debug.Log(q);
			if (q != null && q.IsRandomQuest)
			{
				this.list.Remove(q);
			}
		}
	}

	public void RemoveGlobal(Quest q)
	{
		this.globalList.Remove(q);
	}

	public void RemoveAll(Chara c)
	{
		foreach (Quest q in (from a in this.list
		where a.chara == c
		select a).ToList<Quest>())
		{
			this.Remove(q);
		}
		foreach (Quest q2 in (from a in this.globalList
		where a.chara == c
		select a).ToList<Quest>())
		{
			this.RemoveGlobal(q2);
		}
	}

	public void Complete(Quest q)
	{
		q.Complete();
	}

	public bool OnShowDialog(Chara c)
	{
		foreach (Quest quest in this.list)
		{
			if (quest.person.chara == c && (quest.CanUpdateOnTalk(c) || (quest.CanAutoAdvance && EClass.debug.autoAdvanceQuest)))
			{
				return quest.UpdateOnTalk();
			}
		}
		return false;
	}

	public void OnAdvanceHour()
	{
		this.list.ForeachReverse(delegate(Quest q)
		{
			if (q.IsExpired)
			{
				Msg.Say("questExpired", q.GetTitle(), null, null, null);
				q.Fail();
			}
		});
	}

	public bool IsStarted<T>() where T : Quest
	{
		return this.GetPhase<T>() != -1;
	}

	public int GetPhase<T>() where T : Quest
	{
		if (this.completedTypes.Contains(typeof(T).ToString()))
		{
			return 999;
		}
		foreach (Quest quest in this.list)
		{
			if (quest is T)
			{
				return quest.phase;
			}
		}
		return -1;
	}

	public T Get<T>() where T : Quest
	{
		foreach (Quest quest in this.list)
		{
			if (quest is T)
			{
				return quest as T;
			}
		}
		return default(T);
	}

	public Quest Get(string id)
	{
		foreach (Quest quest in this.list)
		{
			if (quest.GetType().ToString() == id || quest.id == id)
			{
				return quest;
			}
		}
		return null;
	}

	public Quest Get(int uid)
	{
		foreach (Quest quest in this.list)
		{
			if (quest.uid == uid)
			{
				return quest;
			}
		}
		return null;
	}

	public Quest GetGlobal(string id)
	{
		foreach (Quest quest in this.globalList)
		{
			if (quest.GetType().ToString() == id || quest.id == id)
			{
				return quest;
			}
		}
		return null;
	}

	public bool IsCompleted(string id)
	{
		return this.completedIDs.Contains(id);
	}

	public int CountNew()
	{
		int num = 0;
		using (List<Quest>.Enumerator enumerator = this.list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.isNew)
				{
					num++;
				}
			}
		}
		return num;
	}

	public int CountRandomQuest()
	{
		int num = 0;
		using (List<Quest>.Enumerator enumerator = this.list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current is QuestRandom)
				{
					num++;
				}
			}
		}
		return num;
	}

	public void UpdateJournal()
	{
		SE.WriteJournal();
		Msg.Say("journalUpdate");
	}

	public bool IsDeliverTarget(Chara c)
	{
		using (List<Quest>.Enumerator enumerator = this.list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsDeliverTarget(c))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void AddQuestAsh()
	{
		Chara c = EClass.game.cards.globalCharas.Find("ashland");
		QuestCraft questCraft = Quest.Create("ash1", null, null) as QuestCraft;
		questCraft.SetClient(c, true);
		questCraft.req1.Add(new QuestCraft.Req("rock", 1));
		questCraft.req1.Add(new QuestCraft.Req("branch", 1));
		questCraft.req2.Add(new QuestCraft.Req("axe", 1));
	}

	public void AddQuestFiama()
	{
		Chara c = EClass.game.cards.globalCharas.Find("fiama");
		QuestCraft questCraft = Quest.Create("fiama1", null, null) as QuestCraft;
		questCraft.SetClient(c, true);
		questCraft.req1.Add(new QuestCraft.Req("crim", 3));
		questCraft.req2.Add(new QuestCraft.Req("hotcrim", 1));
	}

	public bool HasFarAwayEscort(bool execute = false)
	{
		bool has = false;
		if (EClass._zone.IsTown || EClass._zone.IsPCFaction || EClass._zone.HasLaw || EClass._zone.IsInstance)
		{
			return false;
		}
		EClass._map.charas.ForeachReverse(delegate(Chara m)
		{
			if (!m.IsEscorted())
			{
				return;
			}
			if (EClass.pc.Dist(m) < 5)
			{
				return;
			}
			has = true;
			if (execute)
			{
				m.Destroy();
			}
		});
		if (has & execute)
		{
			this.OnEnterZone();
		}
		return has;
	}

	public void OnEnterZone()
	{
		this.list.ForeachReverse(delegate(Quest q)
		{
			q.OnEnterZone();
		});
	}

	public const int MaxRandomQuest = 5;

	[JsonProperty]
	public List<Quest> list = new List<Quest>();

	[JsonProperty]
	public List<Quest> globalList = new List<Quest>();

	[JsonProperty]
	public HashSet<string> completedIDs = new HashSet<string>();

	[JsonProperty]
	public HashSet<string> completedTypes = new HashSet<string>();

	[JsonProperty]
	public int uid;
}
