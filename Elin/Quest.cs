using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Quest : EClass
{
	public static Quest Create(string _id, string _idPerson = null, Chara c = null)
	{
		Quest quest = ClassCache.Create<Quest>(EClass.sources.quests.map[_id].type.IsEmpty("Quest"), "Elin");
		quest.id = _id;
		quest.person = new Person(_idPerson, null);
		QuestDestZone questDestZone = quest as QuestDestZone;
		if (questDestZone != null && questDestZone.IsDeliver)
		{
			Zone zone = Quest.ListDeliver().RandomItem<Zone>();
			questDestZone.SetDest(zone, zone.dictCitizen.Keys.RandomItem<int>());
		}
		if (c != null)
		{
			quest.SetClient(c, true);
		}
		quest.Init();
		return quest;
	}

	public static List<Zone> ListDeliver()
	{
		List<Zone> list = new List<Zone>();
		foreach (Zone zone in EClass.game.spatials.Zones)
		{
			if (zone != EClass._zone && zone.IsTown && !zone.IsClosed && !zone.IsPCFaction && zone.dictCitizen.Count > 0 && zone.CanBeDeliverDestination)
			{
				list.Add(zone);
			}
		}
		return list;
	}

	public Zone ClientZone
	{
		get
		{
			Zone result;
			if (this.chara == null || !this.chara.IsGlobal || this.chara.currentZone == null)
			{
				if ((result = RefZone.Get(this.uidClientZone)) == null)
				{
					if (this.source.idZone.IsEmpty())
					{
						return EClass.game.StartZone;
					}
					return EClass.game.spatials.Find(this.source.idZone);
				}
			}
			else
			{
				result = this.chara.currentZone;
			}
			return result;
		}
	}

	public Chara chara
	{
		get
		{
			return this.person.chara;
		}
	}

	public SourceQuest.Row source
	{
		get
		{
			return EClass.sources.quests.map[this.idSource];
		}
	}

	public bool Confetti
	{
		get
		{
			return false;
		}
	}

	public bool IsExpired
	{
		get
		{
			return (this.deadline > 0 && this.Hours < 0) || this.isComplete;
		}
	}

	public int Hours
	{
		get
		{
			if (this.deadline != 0)
			{
				return EClass.world.date.GetRemainingHours(this.deadline);
			}
			return -1;
		}
	}

	public string TextDeadline
	{
		get
		{
			if (this.deadline != 0)
			{
				return Date.GetText((this.Hours < 0) ? 0 : this.Hours);
			}
			return "dateDayVoid".lang();
		}
	}

	public virtual bool HasDLC
	{
		get
		{
			return true;
		}
	}

	public virtual string idSource
	{
		get
		{
			return this.id;
		}
	}

	public virtual string RewardSuffix
	{
		get
		{
			return "";
		}
	}

	public virtual bool FameContent
	{
		get
		{
			return false;
		}
	}

	public virtual string TextExtra
	{
		get
		{
			return null;
		}
	}

	public virtual string TextExtra2
	{
		get
		{
			if (!this.FameContent)
			{
				return null;
			}
			return "dangerLv".lang(this.DangerLv.ToString() ?? "", null, null, null, null);
		}
	}

	public virtual int DangerLv
	{
		get
		{
			return this.dangerLv;
		}
	}

	public virtual int AffinityGain
	{
		get
		{
			return 20;
		}
	}

	public virtual int BaseMoney
	{
		get
		{
			return this.source.money;
		}
	}

	public virtual int KarmaOnFail
	{
		get
		{
			return 0;
		}
	}

	public virtual bool CanAbandon
	{
		get
		{
			return this.IsRandomQuest;
		}
	}

	public virtual int FameOnComplete
	{
		get
		{
			return 0;
		}
	}

	public virtual int RangeDeadLine
	{
		get
		{
			return 0;
		}
	}

	public virtual bool UseInstanceZone
	{
		get
		{
			return false;
		}
	}

	public virtual bool ForbidTeleport
	{
		get
		{
			return false;
		}
	}

	public virtual bool RequireClientInSameZone
	{
		get
		{
			return true;
		}
	}

	public virtual Quest.DifficultyType difficultyType
	{
		get
		{
			return Quest.DifficultyType.Default;
		}
	}

	public virtual Thing GetDestThing()
	{
		return null;
	}

	public virtual int GetExtraMoney()
	{
		return 0;
	}

	public virtual int GetRewardPlat(int money)
	{
		return 1 + EClass.rnd(2) + EClass.rndHalf((int)Mathf.Sqrt((float)(money / 200)));
	}

	public virtual Chara DestChara
	{
		get
		{
			return this.chara;
		}
	}

	public virtual string RefDrama1
	{
		get
		{
			if (this.task == null)
			{
				return "";
			}
			return this.task.RefDrama1;
		}
	}

	public virtual string RefDrama2
	{
		get
		{
			if (this.task == null)
			{
				return "";
			}
			return this.task.RefDrama2;
		}
	}

	public virtual string RefDrama3
	{
		get
		{
			if (this.task == null)
			{
				return "";
			}
			return this.task.RefDrama3;
		}
	}

	public virtual string RefDrama4
	{
		get
		{
			return "";
		}
	}

	public virtual string TitlePrefix
	{
		get
		{
			return "";
		}
	}

	public override bool Equals(object obj)
	{
		int num = this.uid;
		Quest quest = obj as Quest;
		int? num2 = (quest != null) ? new int?(quest.uid) : null;
		return num == num2.GetValueOrDefault() & num2 != null;
	}

	public virtual bool IsVisibleOnQuestBoard()
	{
		return (!this.RequireClientInSameZone || (this.chara != null && this.chara.IsAliveInCurrentZone && this.chara.conSuspend == null)) && EClass.world.date.GetRaw(0) >= this.startDate && this.CanStartQuest() && ((!this.chara.IsPCFaction && this is QuestRandom) || EClass._zone.IsPCFaction) && (this.chara == null || !(this.chara.trait is TraitLoytel) || this.chara.currentZone == null || this.chara.currentZone.IsPCFaction);
	}

	public virtual bool CanStartQuest()
	{
		return true;
	}

	public virtual bool CanUpdateOnTalk(Chara c)
	{
		return false;
	}

	public virtual bool CanAutoAdvance
	{
		get
		{
			return true;
		}
	}

	public virtual bool IsRandomQuest
	{
		get
		{
			return false;
		}
	}

	public virtual Zone CreateInstanceZone(Chara c)
	{
		return null;
	}

	public virtual bool UpdateOnTalk()
	{
		this.NextPhase();
		this.OnClickQuest();
		return true;
	}

	public void Init()
	{
		if (this.RangeDeadLine != 0)
		{
			this.deadline = EClass.world.date.GetRaw(0) + (36 + EClass.rnd(12)) * 60 + EClass.rnd(this.RangeDeadLine) * 1440;
		}
		this.uid = EClass.game.quests.uid;
		EClass.game.quests.uid++;
		this.isNew = true;
		this.track = true;
		switch (this.difficultyType)
		{
		case Quest.DifficultyType.Deliver:
		case Quest.DifficultyType.Supply:
		case Quest.DifficultyType.Escort:
			this.difficulty = Mathf.Clamp(6 - this.Hours / 48, 1, 6);
			goto IL_160;
		case Quest.DifficultyType.Bulk:
			this.difficulty = Mathf.Clamp(EClass.rnd(EClass.rnd(((EClass._zone.branch != null) ? EClass._zone.branch.lv : 1) + 3)) + 1, 1, 6);
			goto IL_160;
		case Quest.DifficultyType.Farm:
			this.difficulty = 1 + EClass.rnd(3 + EClass.pc.Evalue(250) / 10);
			goto IL_160;
		case Quest.DifficultyType.Music:
			this.difficulty = 1 + EClass.rnd(3 + EClass.pc.Evalue(241) / 10);
			goto IL_160;
		}
		this.difficulty = 3 + EClass.rnd(EClass.rnd(4)) - EClass.rnd(4);
		IL_160:
		this.difficulty = Mathf.Clamp(this.difficulty, 1, 7);
		this.OnInit();
		if (this.task != null)
		{
			this.task.OnInit();
		}
		int fameLv = EClass.pc.FameLv;
		this.dangerLv = Mathf.Max(fameLv + EClass.rnd(fameLv / 10 + 5) - EClass.rnd(fameLv / 10 + 5), 1);
		this.rewardMoney = (this.BaseMoney + EClass.rnd(this.BaseMoney / 4) + this.GetExtraMoney()) * (55 + this.difficulty * 15) / 100;
	}

	public virtual void OnInit()
	{
	}

	public Quest SetClient(Chara c, bool assignQuest = true)
	{
		if (c == null)
		{
			this.person = new Person();
			return this;
		}
		if (assignQuest)
		{
			c.quest = this;
		}
		if (c.currentZone != null)
		{
			this.uidClientZone = c.currentZone.uid;
		}
		this.person = new Person(c);
		return this;
	}

	public void SetTask(QuestTask _task)
	{
		this.task = _task;
		this.task.SetOwner(this);
	}

	public void Start()
	{
		this.OnStart();
		if (this.task != null)
		{
			this.task.OnStart();
		}
	}

	public virtual void OnStart()
	{
	}

	public virtual void OnEnterZone()
	{
	}

	public virtual void OnClickQuest()
	{
		if (!this.source.drama.IsEmpty())
		{
			LayerDrama.currentQuest = this;
			SoundManager.ForceBGM();
			this.chara.ShowDialog(this.source.drama[0], (this.source.drama.Length > 1) ? this.source.drama[1] : ("quest_" + this.source.id), "");
			return;
		}
		LayerDrama.currentQuest = this;
		LayerDrama.forceJump = "_quest";
		this.chara.ShowDialog();
	}

	public void Fail()
	{
		EClass.Sound.Play("questFail");
		if (!(this is QuestTrackCraft))
		{
			EClass.player.ModFame(-10 - (int)((float)EClass.player.fame * 0.05f));
			EClass.player.ModKarma(this.KarmaOnFail);
		}
		EClass.game.quests.Remove(this);
		if (this.chara != null && this.chara.quest != null && this.chara.quest.uid == this.uid)
		{
			this.chara.quest = null;
		}
		if (this.ClientZone != null)
		{
			this.ClientZone.completedQuests.Add(this.uid);
		}
		this.OnFail();
	}

	public virtual void OnFail()
	{
	}

	public virtual void ShowCompleteText()
	{
		EClass.Sound.Play("questComplete");
		Msg.Say("completeQuest", this.GetTitle(), null, null, null);
	}

	public void Complete()
	{
		this.OnBeforeComplete();
		EClass.game.quests.Remove(this);
		EClass.game.quests.completedIDs.Add(this.id);
		EClass.game.quests.completedTypes.Add(base.GetType().ToString());
		this.ShowCompleteText();
		this.OnDropReward();
		if (this.Confetti)
		{
			EClass.Sound.Play("confetti");
			ScreenEffect.Play("Firework");
		}
		if (this.chara != null && this.chara.quest != null && this.chara.quest.uid == this.uid)
		{
			this.chara.quest = null;
		}
		if (this.DestChara != null && this.DestChara.IsAliveInCurrentZone && this.AffinityGain != 0)
		{
			this.DestChara.ModAffinity(EClass.pc, this.AffinityGain, true);
		}
		if (this.ClientZone != null)
		{
			this.ClientZone.completedQuests.Add(this.uid);
		}
		this.OnComplete();
		EClass.player.ModKarma(1);
		this.isComplete = true;
	}

	public virtual void OnBeforeComplete()
	{
	}

	public virtual void OnDropReward()
	{
	}

	public Thing DropReward(string id)
	{
		return this.DropReward(ThingGen.Create(id, -1, -1));
	}

	public Thing DropReward(Thing t)
	{
		return EClass.player.DropReward(t, false);
	}

	public virtual void OnComplete()
	{
	}

	public void CompleteTask()
	{
		this.OnCompleteTask();
		this.task = null;
	}

	public virtual void OnCompleteTask()
	{
		EClass.game.quests.Complete(this);
	}

	public virtual void OnGiveItem(Chara c, Thing t)
	{
		if (this.task != null)
		{
			this.task.OnGiveItem(c, t);
			if (this.task.IsComplete())
			{
				this.CompleteTask();
			}
		}
	}

	public virtual void OnKillChara(Chara c)
	{
		if (this.task != null)
		{
			this.task.OnKillChara(c);
			if (this.task.IsComplete())
			{
				this.CompleteTask();
			}
		}
	}

	public virtual void OnModKarma(int a)
	{
		if (this.task != null)
		{
			this.task.OnModKarma(a);
			if (this.task.IsComplete())
			{
				this.CompleteTask();
			}
		}
	}

	public void NextPhase()
	{
		this.ChangePhase(this.phase + 1);
	}

	public void ChangePhase(int a)
	{
		this.phase = a;
		this.UpdateJournal();
		this.OnChangePhase(a);
	}

	public virtual void OnChangePhase(int a)
	{
	}

	public string Parse(string s)
	{
		if (s.IsEmpty())
		{
			return s;
		}
		GameLang.refDrama1 = this.RefDrama1.TagColor(FontColor.QuestObjective, null);
		GameLang.refDrama2 = this.RefDrama2.TagColor(FontColor.QuestObjective, null);
		GameLang.refDrama3 = this.RefDrama3.TagColor(FontColor.QuestObjective, null);
		GameLang.refDrama4 = this.RefDrama4.TagColor(FontColor.QuestObjective, null);
		Rand.UseSeed(this.uid, delegate
		{
			s = GameLang.ConvertDrama(s, this.chara);
			if (this.chara != null)
			{
				s = this.chara.ApplyTone(s, false);
				return;
			}
			s = Card.ApplyTone(this.chara, ref s, this.person.tones, this.person.gender, false);
		});
		return s;
	}

	public virtual string GetTitle()
	{
		string s = "";
		Rand.UseSeed(this.uid, delegate
		{
			s = this.source.GetText("name", false).Split(Environment.NewLine.ToCharArray()).RandomItem<string>();
		});
		return s;
	}

	public virtual string GetTrackerText()
	{
		string text = this.GetTextProgress();
		if (text.IsEmpty() && !this.IsRandomQuest)
		{
			text = this.GetDetail(true);
		}
		return text;
	}

	public virtual string GetDetail(bool onJournal = false)
	{
		string text = this.Parse(this.GetDetailText(onJournal));
		if (onJournal)
		{
			string textProgress = this.GetTextProgress();
			if (!textProgress.IsEmpty())
			{
				text = text + "\n\n" + textProgress;
			}
		}
		if (this.task != null)
		{
			this.task.OnGetDetail(ref text, onJournal);
		}
		text = GameLang.Convert(text);
		return text;
	}

	public virtual string GetDetailText(bool onJournal = false)
	{
		string s = "";
		Rand.UseSeed(this.uid, delegate
		{
			s = this.source.GetText("detail", false).Split(Environment.NewLine.ToCharArray()).RandomItem<string>();
		});
		return s.ToTitleCase(false);
	}

	public virtual string GetTextProgress()
	{
		if (this.task != null)
		{
			return this.task.GetTextProgress();
		}
		return "";
	}

	public string GetRewardText()
	{
		string result = "-";
		if (this.rewardMoney != 0 || this is QuestRandom)
		{
			result = ("qReward" + this.RewardSuffix).lang(this.rewardMoney.ToString() ?? "", null, null, null, null);
		}
		return result;
	}

	public virtual string GetTalkProgress()
	{
		return this.Parse(this.source.GetText("talkProgress", true));
	}

	public virtual string GetTalkComplete()
	{
		return this.Parse(this.source.GetText("talkComplete", true));
	}

	public void UpdateJournal()
	{
		SE.WriteJournal();
		Msg.Say("journalUpdate2", this.GetTitle(), null, null, null);
	}

	public virtual bool IsDeliverTarget(Chara c)
	{
		return false;
	}

	public virtual bool CanDeliverToClient(Chara c)
	{
		return false;
	}

	public virtual bool CanDeliverToBox(Thing t)
	{
		return false;
	}

	public virtual bool Deliver(Chara c, Thing t = null)
	{
		return false;
	}

	public virtual string NameDeliver
	{
		get
		{
			return "";
		}
	}

	public const int PhaseComplete = 999;

	[JsonProperty]
	public string id;

	[JsonProperty]
	public int uid;

	[JsonProperty]
	public int uidClientZone;

	[JsonProperty]
	public int phase;

	[JsonProperty]
	public int lv;

	[JsonProperty]
	public int deadline;

	[JsonProperty]
	public int difficulty;

	[JsonProperty]
	public int rewardMoney;

	[JsonProperty]
	public int bonusMoney;

	[JsonProperty]
	public int startDate;

	[JsonProperty]
	public int dangerLv;

	[JsonProperty]
	public bool isNew;

	[JsonProperty]
	public bool isComplete;

	[JsonProperty]
	public bool track;

	[JsonProperty]
	public Person person;

	[JsonProperty]
	public QuestTask task;

	public RefChara refChara = new RefChara();

	public enum DifficultyType
	{
		Default,
		Deliver,
		Supply,
		Escort,
		Bulk,
		Meal,
		Farm,
		Music
	}

	public enum SubReward
	{
		plat,
		gacha_coin,
		money2,
		ticket_furniture
	}
}
