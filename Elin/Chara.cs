using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Algorithms;
using Newtonsoft.Json;
using UnityEngine;

public class Chara : Card, IPathfindWalker
{
	public string Aka
	{
		get
		{
			return this._alias.IsEmpty(this.source.GetText("aka", true) ?? "");
		}
	}

	public string _alias
	{
		get
		{
			return this._strs[0];
		}
		set
		{
			this._strs[0] = value;
		}
	}

	public string idFaith
	{
		get
		{
			return this._strs[1];
		}
		set
		{
			this._strs[1] = value;
		}
	}

	public string idFaction
	{
		get
		{
			return this._strs[2];
		}
		set
		{
			this._strs[2] = value;
		}
	}

	public Zone currentZone
	{
		get
		{
			return RefZone.Get(this._cints[1]);
		}
		set
		{
			this._cints[1] = RefZone.Set(value);
		}
	}

	public Zone homeZone
	{
		get
		{
			return RefZone.Get(this._cints[2]);
		}
		set
		{
			this._cints[2] = RefZone.Set(value);
		}
	}

	public GoalListType goalListType
	{
		get
		{
			return this._cints[3].ToEnum<GoalListType>();
		}
		set
		{
			this._cints[3] = (int)value;
		}
	}

	public Hostility hostility
	{
		get
		{
			return this._cints[4].ToEnum<Hostility>();
		}
		set
		{
			this._cints[4] = (int)value;
		}
	}

	public int _affinity
	{
		get
		{
			return this._cints[5];
		}
		set
		{
			this._cints[5] = value;
		}
	}

	public Affinity affinity
	{
		get
		{
			return Affinity.Get(this);
		}
	}

	public int interest
	{
		get
		{
			return this._cints[6];
		}
		set
		{
			this._cints[6] = value;
		}
	}

	public int daysStarved
	{
		get
		{
			return this._cints[7];
		}
		set
		{
			this._cints[7] = value;
		}
	}

	public int _idTimeTable
	{
		get
		{
			return this._cints[9];
		}
		set
		{
			this._cints[9] = value;
		}
	}

	public int uidEditor
	{
		get
		{
			return this._cints[10];
		}
		set
		{
			this._cints[10] = value;
		}
	}

	public int _maxStamina
	{
		get
		{
			return this._cints[20];
		}
		set
		{
			this._cints[20] = value;
		}
	}

	public int corruption
	{
		get
		{
			return this._cints[21];
		}
		set
		{
			this._cints[21] = value;
		}
	}

	public bool isDead
	{
		get
		{
			return this._cbits1[0];
		}
		set
		{
			this._cbits1[0] = value;
		}
	}

	public bool isAlawysVisible
	{
		get
		{
			return this._cbits1[1];
		}
		set
		{
			this._cbits1[1] = value;
		}
	}

	public bool knowFav
	{
		get
		{
			return this._cbits1[3];
		}
		set
		{
			this._cbits1[3] = value;
		}
	}

	public CharaAbility ability
	{
		get
		{
			CharaAbility result;
			if ((result = this._ability) == null)
			{
				result = (this._ability = new CharaAbility(this));
			}
			return result;
		}
	}

	public Faction faction
	{
		get
		{
			Faction result;
			if ((result = this._faction) == null)
			{
				result = (this._faction = EClass.game.factions.dictAll.TryGetValue(this.idFaction, null));
			}
			return result;
		}
		set
		{
			this.idFaction = ((value != null) ? value.uid : null);
		}
	}

	public Religion faith
	{
		get
		{
			return EClass.game.religions.dictAll.TryGetValue(this.idFaith, null) ?? EClass.game.religions.dictAll["eyth"];
		}
		set
		{
			this.idFaith = ((value != null) ? value.id : null);
		}
	}

	public override CardRow sourceCard
	{
		get
		{
			return this.source;
		}
	}

	public override CardRow sourceRenderCard
	{
		get
		{
			if (this.pccData != null && !(this.source.renderData is RenderDataPcc))
			{
				return SourceChara.rowDefaultPCC;
			}
			return this.source;
		}
	}

	public SourceRace.Row race
	{
		get
		{
			SourceRace.Row result;
			if ((result = this._race) == null)
			{
				result = ((this._race = EClass.sources.races.map.TryGetValue(base.c_idRace.IsEmpty(this.source.race), null)) ?? EClass.sources.races.map["norland"]);
			}
			return result;
		}
	}

	public SourceJob.Row job
	{
		get
		{
			SourceJob.Row result;
			if ((result = this._job) == null)
			{
				result = ((this._job = EClass.sources.jobs.map.TryGetValue(base.c_idJob.IsEmpty(this.source.job), null)) ?? EClass.sources.jobs.map["none"]);
			}
			return result;
		}
	}

	public string idTimeTable
	{
		get
		{
			return Chara.IDTimeTable[this._idTimeTable];
		}
	}

	public Hostility OriginalHostility
	{
		get
		{
			if (EClass.pc != null && this.IsPCFaction)
			{
				return Hostility.Ally;
			}
			if (base.c_originalHostility != (Hostility)0)
			{
				return base.c_originalHostility;
			}
			if (!this.source.hostility.IsEmpty())
			{
				return this.source.hostility.ToEnum(true);
			}
			return Hostility.Enemy;
		}
	}

	public string IDPCCBodySet
	{
		get
		{
			if (this.source.idActor.Length <= 1)
			{
				return "female";
			}
			return this.source.idActor[1];
		}
	}

	public new TraitChara trait
	{
		get
		{
			return this.trait as TraitChara;
		}
		set
		{
			this.trait = value;
		}
	}

	public override string ToString()
	{
		string[] array = new string[9];
		array[0] = base.Name;
		array[1] = "(";
		int num = 2;
		AIAct aiact = this.ai;
		array[num] = ((aiact != null) ? aiact.ToString() : null);
		array[3] = ")";
		int num2 = 4;
		Point pos = this.pos;
		array[num2] = ((pos != null) ? pos.ToString() : null);
		array[5] = "/";
		array[6] = base.ExistsOnMap.ToString();
		array[7] = "/";
		array[8] = this.isDead.ToString();
		return string.Concat(array);
	}

	public string NameBraced
	{
		get
		{
			return this.GetName(NameStyle.Full, -1);
		}
	}

	public string NameTitled
	{
		get
		{
			return ((EClass.game.idDifficulty == 0) ? "" : ((EClass.game.idDifficulty == 1) ? "☆" : "★")) + this.NameBraced;
		}
	}

	public override string actorPrefab
	{
		get
		{
			if (this.IsPCC)
			{
				return "CharaActorPCC";
			}
			return "CharaActor";
		}
	}

	public override int PrefIndex
	{
		get
		{
			if (this.sourceCard._tiles.Length == 0 && this.renderer.replacer == null)
			{
				return base.dir;
			}
			if (base.dir != 1 && base.dir != 2)
			{
				return 0;
			}
			return 1;
		}
	}

	public override bool flipX
	{
		get
		{
			return base.dir == 1 || base.dir == 2;
		}
	}

	public override string AliasMaterialOnCreate
	{
		get
		{
			return this.race.material;
		}
	}

	public override bool IsAliveInCurrentZone
	{
		get
		{
			return !this.isDead && base.ExistsOnMap;
		}
	}

	public override bool IsDeadOrSleeping
	{
		get
		{
			return this.isDead || this.conSleep != null || this.conSuspend != null || this.isFainted;
		}
	}

	public override bool IsDisabled
	{
		get
		{
			return this.isDead || this.conSleep != null || this.isFainted || this.isParalyzed;
		}
	}

	public bool IsLevitating
	{
		get
		{
			if (this.ride != null)
			{
				return this.ride._isLevitating;
			}
			return this._isLevitating;
		}
	}

	public bool IsCriticallyWounded(bool includeRide = false)
	{
		if (this.host != null && !includeRide)
		{
			return false;
		}
		if (base.Evalue(1421) <= 0)
		{
			return base.hp < this.MaxHP / 5;
		}
		return base.hp + this.mana.value < (this.MaxHP + this.mana.max) / 5;
	}

	public override bool IsMoving
	{
		get
		{
			return this.idleTimer > 0f;
		}
	}

	public override bool IsGlobal
	{
		get
		{
			return this.global != null;
		}
	}

	public override bool IsPC
	{
		get
		{
			return this == EClass.player.chara;
		}
	}

	public override bool IsPCParty
	{
		get
		{
			return this.party != null && this.party.leader == EClass.pc;
		}
	}

	public override bool IsMinion
	{
		get
		{
			return this.master != null || base.c_uidMaster != 0;
		}
	}

	public override bool IsPCPartyMinion
	{
		get
		{
			return this.master != null && (this.master.IsPCParty || this.master.IsPCPartyMinion);
		}
	}

	public override bool IsPCFactionMinion
	{
		get
		{
			return this.master != null && (this.master.IsPCFaction || this.master.IsPCFactionMinion);
		}
	}

	public override bool IsPCFaction
	{
		get
		{
			return EClass.pc != null && this.faction == EClass.pc.faction;
		}
	}

	public override bool IsPCC
	{
		get
		{
			return this.pccData != null;
		}
	}

	public override bool isThing
	{
		get
		{
			return false;
		}
	}

	public override bool isChara
	{
		get
		{
			return true;
		}
	}

	public override bool HasHost
	{
		get
		{
			return this.host != null;
		}
	}

	public override bool isSynced
	{
		get
		{
			return this.renderer.isSynced || (this.host != null && this.host.renderer.isSynced);
		}
	}

	public override bool IsMultisize
	{
		get
		{
			return this.sourceCard.multisize;
		}
	}

	public override int MaxHP
	{
		get
		{
			return Mathf.Max(1, ((base.END * 2 + base.STR + base.WIL / 2) * Mathf.Min(base.LV, 25) / 25 + base.END + 10) * base.Evalue(60) / 100 * (int)((this.IsPCFaction ? ((Rarity)100) : ((Rarity)100 + (int)(base.rarity * (Rarity)300))) + (this.IsPC ? (EClass.player.lastEmptyAlly * base.Evalue(1646)) : 0)) / 100);
		}
	}

	public override int WeightLimit
	{
		get
		{
			return (base.STR * 500 + base.END * 250 + base.Evalue(207) * 2000) * (base.HasElement(1411, 1) ? 5 : 1) + 45000;
		}
	}

	public override int SelfWeight
	{
		get
		{
			return this.bio.weight * 1000;
		}
	}

	public int MaxSummon
	{
		get
		{
			return (int)(Mathf.Max((int)Mathf.Sqrt((float)base.CHA), 1) + base.Evalue(1647) + ((!base.IsPCFactionOrMinion) ? (base.rarity * (Rarity)5) : Rarity.Normal));
		}
	}

	public Element MainElement
	{
		get
		{
			if (base.c_idMainElement == 0)
			{
				return Element.Void;
			}
			return this.elements.GetElement(base.c_idMainElement);
		}
	}

	public override int DV
	{
		get
		{
			if (this.IsPCFaction)
			{
				return this.elements.Value(64) / (this.HasCondition<ConWeakness>() ? 2 : 1);
			}
			int num = base.LV;
			if (num > 50)
			{
				num = 50 + (num - 50) / 10;
			}
			return (num + this.elements.Value(64) * (100 + num + this.race.DV * 5) / 100) / (this.HasCondition<ConWeakness>() ? 2 : 1);
		}
	}

	public override int PV
	{
		get
		{
			if (this.IsPCFaction)
			{
				return this.elements.Value(65) / (this.HasCondition<ConWeakness>() ? 2 : 1);
			}
			int num = base.LV;
			if (num > 50)
			{
				num = 50 + (num - 50) / 10;
			}
			return (num + this.elements.Value(65) * (100 + num + this.race.PV * 5) / 100) / (this.HasCondition<ConWeakness>() ? 2 : 1);
		}
	}

	public bool CanOpenDoor
	{
		get
		{
			return base.INT >= 5 || (this.IsPCFaction && this.memberType == FactionMemberType.Default);
		}
	}

	public bool HasHigherGround(Card c)
	{
		if (c == null)
		{
			return false;
		}
		float num = this.renderer.position.y - this.pos.Position(0).y + (this._isLevitating ? 0.4f : 0f);
		float num2 = c.renderer.position.y - c.pos.Position(0).y + ((c.isChara && c.Chara._isLevitating) ? 0.4f : 0f);
		return num > num2 + 0.1f;
	}

	public bool CanSeeSimple(Point p)
	{
		return p.IsValid && !p.IsHidden && (!this.IsPC || (this.fov != null && this.fov.lastPoints.ContainsKey(p.index)));
	}

	public bool CanSee(Card c)
	{
		if (c == this)
		{
			return true;
		}
		if (!c.pos.IsValid)
		{
			return false;
		}
		if (c.isChara)
		{
			if (this.hasTelepathy && c.Chara.race.visibleWithTelepathy)
			{
				return true;
			}
			if (c.isHidden && c != this && !this.canSeeInvisible)
			{
				return false;
			}
		}
		return !this.IsPC || (this.fov != null && this.fov.lastPoints.ContainsKey(c.pos.index));
	}

	public bool CanSeeLos(Card c, int dist = -1, bool includeTelepathy = false)
	{
		return (c.isChara && includeTelepathy && this.hasTelepathy && c.Chara.race.visibleWithTelepathy) || ((!c.isHidden || this.canSeeInvisible) && this.CanSeeLos(c.pos, dist));
	}

	public bool CanSeeLos(Point p, int dist = -1)
	{
		if (dist == -1)
		{
			dist = this.pos.Distance(p);
		}
		if (dist > base.GetSightRadius())
		{
			return false;
		}
		if (this.IsPC)
		{
			return this.fov != null && this.fov.lastPoints.ContainsKey(p.index);
		}
		return Los.IsVisible(this.pos, p, null);
	}

	public bool HasAccess(Card c)
	{
		return this.HasAccess(c.pos);
	}

	public bool HasAccess(Point p)
	{
		if (!EClass._zone.IsPCFaction || p.cell.room == null || this.IsPC)
		{
			return true;
		}
		BaseArea.AccessType accessType = p.cell.room.data.accessType;
		if (accessType != BaseArea.AccessType.Resident)
		{
			return accessType != BaseArea.AccessType.Private;
		}
		return this.memberType == FactionMemberType.Default;
	}

	public Tactics tactics
	{
		get
		{
			Tactics result;
			if ((result = this._tactics) == null)
			{
				result = (this._tactics = new Tactics(this));
			}
			return result;
		}
	}

	public TimeTable.Span CurrentSpan
	{
		get
		{
			return TimeTable.GetSpan(this.idTimeTable, EClass.world.date.hour);
		}
	}

	public bool IsInActiveZone
	{
		get
		{
			return this.currentZone == EClass.game.activeZone;
		}
	}

	public bool IsLocalChara
	{
		get
		{
			return !this.IsGlobal && !base.isSubsetCard && this.homeZone == EClass._zone;
		}
	}

	public bool IsIdle
	{
		get
		{
			return !this.IsDeadOrSleeping && this.ai.Current.IsIdle;
		}
	}

	public bool IsInCombat
	{
		get
		{
			return this.ai is GoalCombat;
		}
	}

	public int DestDist
	{
		get
		{
			return this.tactics.DestDist;
		}
	}

	public bool HasNoGoal
	{
		get
		{
			return this.ai.IsNoGoal;
		}
	}

	public bool CanWitness
	{
		get
		{
			return this.race.IsHuman || this.race.IsFairy || this.race.IsGod || this.race.id == "mutant";
		}
	}

	public bool IsHuman
	{
		get
		{
			return this.race.tag.Contains("human");
		}
	}

	public bool IsHumanSpeak
	{
		get
		{
			return this.IsHuman || this.race.tag.Contains("humanSpeak");
		}
	}

	public bool IsMaid
	{
		get
		{
			return EClass.Branch != null && EClass.Branch.uidMaid == base.uid;
		}
	}

	public bool IsPrisoner
	{
		get
		{
			return false;
		}
	}

	public bool IsAdventurer
	{
		get
		{
			return this.global != null && this.faction != EClass.pc.faction && this.IsPCC;
		}
	}

	public bool IsEyth
	{
		get
		{
			return this.faith.id == "eyth";
		}
	}

	public bool CanSleep()
	{
		return EClass._zone.events.GetEvent<ZoneEventQuest>() == null && (EClass.debug.godMode || this.sleepiness.GetPhase() != 0 || this.stamina.GetPhase() <= 1);
	}

	public bool IsWealthy
	{
		get
		{
			return this.source.works.Contains("Rich") || this.source.hobbies.Contains("Rich");
		}
	}

	public FactionBranch homeBranch
	{
		get
		{
			Zone homeZone = this.homeZone;
			if (homeZone == null)
			{
				return null;
			}
			return homeZone.branch;
		}
	}

	public int MaxGene
	{
		get
		{
			return this.race.geneCap;
		}
	}

	protected override void OnSerializing()
	{
		if (this.enemy != null)
		{
			base.SetInt(55, this.enemy.uid);
		}
		this._cints[0] = (int)this._cbits1.Bits;
		List<BodySlot> slots = this.body.slots;
		this.rawSlots = new int[slots.Count];
		for (int i = 0; i < slots.Count; i++)
		{
			this.rawSlots[i] = slots[i].elementId;
		}
	}

	protected override void OnDeserialized()
	{
		this.isCreated = true;
		this._cbits1.Bits = (uint)this._cints[0];
		this.InitStats(true);
		this.body.SetOwner(this, true);
		this.elements.ApplyElementMap(base.uid, SourceValueType.Chara, this.job.elementMap, base.DefaultLV, false, false);
		this.elements.ApplyElementMap(base.uid, SourceValueType.Chara, this.race.elementMap, base.DefaultLV, false, false);
		if (this.global != null && this.global.goal != null)
		{
			this.global.goal.SetOwner(this);
		}
		if (this.IsPCC)
		{
			this.pccData.state = (base.isCensored ? PCCState.Naked : PCCState.Normal);
		}
		if (this.tempElements != null)
		{
			this.tempElements.SetParent(this);
		}
		this.UpdateAngle();
		this.RefreshFaithElement();
		this.Refresh(false);
		if (this.source.tag.Contains("boss"))
		{
			this.bossText = true;
		}
		this.sharedCheckTurn = EClass.rnd(200);
	}

	public override string GetName(NameStyle style, int num = -1)
	{
		if (base.isBackerContent && EClass.core.config.backer.Show(base.c_idBacker))
		{
			if (this.id == "follower" && !this.IsGlobal)
			{
				return "_follower".lang(EClass.sources.backers.map[base.c_idBacker].Name, this.faith.Name, null, null, null);
			}
			return EClass.sources.backers.map[base.c_idBacker].Name;
		}
		else
		{
			string text = base.c_altName ?? this.source.GetName(this, false);
			text = text.Replace("#ele4", this.MainElement.source.GetAltname(2)).Replace("#ele3", this.MainElement.source.GetAltname(1)).Replace("#ele2", this.MainElement.source.GetAltname(0)).Replace("#ele", this.MainElement.source.GetName().ToLower());
			if (base.c_bossType == BossType.Evolved)
			{
				text = "_evolved".lang(text.ToTitleCase(true), null, null, null, null);
			}
			this.trait.SetName(ref text);
			if (text.Length > 0 && char.IsLower(text[0]))
			{
				if (base.rarity >= Rarity.Legendary)
				{
					text = text.ToTitleCase(false);
				}
				else if (num != 0)
				{
					text = text.AddArticle();
				}
			}
			if (style == NameStyle.Simple)
			{
				return text;
			}
			int num2 = (base.rarity == Rarity.Mythical) ? 3 : ((base.rarity >= Rarity.Legendary) ? 2 : ((!this._alias.IsEmpty()) ? 1 : -1));
			if (this.trait is TraitAdventurer)
			{
				num2 = 1;
			}
			if (!this.Aka.IsEmpty())
			{
				text = ((num2 == 1) ? "_aka3" : ((num2 == -1) ? "_aka" : "_aka2")).lang((num2 == -1) ? this.Aka : this.Aka.ToTitleCase(true), text.Bracket(num2), null, null, null);
			}
			else
			{
				text = text.Bracket(num2);
			}
			string str = base.isSale ? "forSale".lang(Lang._currency(this.GetPrice(CurrencyType.Money, true, PriceType.PlayerShop, null), "money"), null, null, null, null) : "";
			return text + str;
		}
	}

	public override void ChangeRarity(Rarity r)
	{
		if (r == base.rarity)
		{
			return;
		}
		base.rarity = r;
		if (this.renderer != null && this.renderer.isSynced)
		{
			this.renderer.RefreshExtra();
		}
		base.hp = this.MaxHP;
	}

	public void SetFaction(Faction f)
	{
		this._faction = null;
		this.faction = f;
		this.hostility = this.faction.GetHostility();
	}

	public void SetHomeZone(Zone zone)
	{
		this.homeZone = zone;
		this.SetGlobal();
	}

	public void OnBanish()
	{
		if (!this.IsGlobal)
		{
			return;
		}
		this.memberType = FactionMemberType.Default;
		UniqueData c_uniqueData = base.c_uniqueData;
		if (c_uniqueData != null)
		{
			Debug.Log(string.Concat(new string[]
			{
				c_uniqueData.uidZone.ToString(),
				"/",
				EClass.game.spatials.map.ContainsKey(c_uniqueData.uidZone).ToString(),
				"/",
				c_uniqueData.x.ToString(),
				"/",
				c_uniqueData.y.ToString()
			}));
		}
		if (c_uniqueData != null && EClass.game.spatials.map.ContainsKey(c_uniqueData.uidZone))
		{
			this.MoveHome(EClass.game.spatials.map[c_uniqueData.uidZone] as Zone, c_uniqueData.x, c_uniqueData.y);
			return;
		}
		Zone zone = EClass.game.spatials.Find("somewhere");
		if (this.trait is TraitAdventurer)
		{
			zone = EClass.world.region.ListTowns().RandomItem<Zone>();
			this.SetHomeZone(zone);
		}
		this.MoveZone(zone, ZoneTransition.EnterState.RandomVisit);
	}

	public Chara SetGlobal(Zone _home, int x, int z)
	{
		this.SetGlobal();
		this.homeZone = _home;
		_home.AddCard(this, x, z);
		this.global.transition = new ZoneTransition
		{
			state = ZoneTransition.EnterState.Exact,
			x = x,
			z = z
		};
		this.orgPos = new Point(x, z);
		return this;
	}

	public Chara SetGlobal()
	{
		if (!this.IsGlobal)
		{
			EClass.game.cards.globalCharas.Add(this);
			this.global = new GlobalData();
			base.isSubsetCard = false;
			this.enemy = null;
			base.c_uidMaster = 0;
		}
		return this;
	}

	public void RemoveGlobal()
	{
		if (!this.IsGlobal || this.trait is TraitUniqueChara || base.IsUnique || EClass.game.cards.listAdv.Contains(this))
		{
			return;
		}
		this.global = null;
		EClass.game.cards.globalCharas.Remove(this);
	}

	public override void OnBeforeCreate()
	{
		if (this.source.job == "*r")
		{
			base.c_idJob = (from j in EClass.sources.jobs.rows
			where j.playable < 4
			select j).RandomItem<SourceJob.Row>().id;
		}
		if (this.bp.idJob != null)
		{
			base.c_idJob = this.bp.idJob;
		}
		if (this.bp.idRace != null)
		{
			base.c_idRace = this.bp.idRace;
		}
	}

	public override void OnCreate(int genLv)
	{
		if (this.source.tag.Contains("boss"))
		{
			this.bossText = true;
		}
		int num = (this.bp.lv != -999) ? this.bp.lv : base.LV;
		if (this.trait.AdvType == TraitChara.Adv_Type.Adv || this.trait.AdvType == TraitChara.Adv_Type.Adv_Fairy)
		{
			if (Chara.ListAdvRace.Count == 0)
			{
				Chara.ListAdvRace = (from a in EClass.sources.races.rows
				where a.playable <= 1 && a.id != "fairy"
				select a).ToList<SourceRace.Row>();
				Chara.ListAdvJob = (from a in EClass.sources.jobs.rows
				where a.playable <= 4
				select a).ToList<SourceJob.Row>();
			}
			if (this.trait.AdvType == TraitChara.Adv_Type.Adv)
			{
				base.c_idRace = Chara.ListAdvRace.RandomItem<SourceRace.Row>().id;
			}
			base.c_idJob = Chara.ListAdvJob.RandomItem<SourceJob.Row>().id;
			this._race = null;
			this._job = null;
			num = 10 + EClass.rnd(40);
		}
		this.bio = new Biography();
		this.bio.Generate(this);
		if (this.source.idActor.Length != 0 && this.source.idActor[0] == "pcc" && this.pccData == null)
		{
			this.pccData = PCCData.Create(this.IDPCCBodySet);
			if (this.source.idActor.Length > 2)
			{
				this.pccData.SetPart("body", this.IDPCCBodySet, this.source.idActor[2], null);
			}
			else
			{
				this.pccData.Randomize(this.IDPCCBodySet, null, true);
			}
		}
		if (this.source.mainElement.Length != 0)
		{
			List<Tuple<string, int, int>> list = new List<Tuple<string, int, int>>();
			string[] mainElement = this.source.mainElement;
			for (int i = 0; i < mainElement.Length; i++)
			{
				string[] array = mainElement[i].Split('/', StringSplitOptions.None);
				SourceElement.Row row = EClass.sources.elements.alias["ele" + array[0]];
				int num2 = this.source.LV * row.eleP / 100 + base.LV - this.source.LV;
				if (list.Count == 0 || num2 < genLv)
				{
					list.Add(new Tuple<string, int, int>(array[0], (array.Length > 1) ? int.Parse(array[1]) : 0, num2));
				}
			}
			Tuple<string, int, int> tuple = list.RandomItemWeighted((Tuple<string, int, int> a) => (float)(10000 / (100 + (genLv - a.Item3) * 25)));
			if (!this.bp.idEle.IsEmpty())
			{
				tuple = (from a in list
				where a.Item1 == this.bp.idEle
				select a).First<Tuple<string, int, int>>();
			}
			this.SetMainElement(tuple.Item1, (tuple.Item2 == 0) ? 10 : tuple.Item2, true);
			num = tuple.Item3;
		}
		if (this.source.name == "*r")
		{
			base.c_altName = NameGen.getRandomName();
		}
		if (this.source.GetText("aka", false) == "*r" || this.trait.UseRandomAlias)
		{
			this._alias = AliasGen.GetRandomAlias();
		}
		this.happiness = EClass.rnd(100);
		this.contribution = EClass.rnd(100);
		this.RerollHobby(true);
		this._idTimeTable = ((EClass.rnd(5) == 0) ? 1 : 0);
		this.ApplyRace(false);
		this.ApplyJob(false);
		if (num != this.source.LV)
		{
			base.SetLv(num);
		}
		if (base.LV > 5 && this.race.id == "mutant")
		{
			for (int j = 0; j < Mathf.Min(1 + base.LV / 5, 22); j++)
			{
				this.SetFeat(1644, j + 1, false);
			}
		}
		this.InitStats(false);
		this.body.SetOwner(this, false);
		this.hostility = this.OriginalHostility;
		if (this.race.EQ.Length != 0)
		{
			this.TryRestock(true);
		}
		string id = this.id;
		uint num3 = <PrivateImplementationDetails>.ComputeStringHash(id);
		if (num3 <= 2331752522U)
		{
			if (num3 != 108289031U)
			{
				if (num3 != 1020922669U)
				{
					if (num3 != 2331752522U)
					{
						goto IL_633;
					}
					if (!(id == "snail"))
					{
						goto IL_633;
					}
					base.idSkin = 5;
					goto IL_633;
				}
				else
				{
					if (!(id == "dodo"))
					{
						goto IL_633;
					}
					base.idSkin = EClass.rnd(4);
					goto IL_633;
				}
			}
			else if (!(id == "cat"))
			{
				goto IL_633;
			}
		}
		else if (num3 <= 3253821027U)
		{
			if (num3 != 2629043432U)
			{
				if (num3 != 3253821027U)
				{
					goto IL_633;
				}
				if (!(id == "sister_undead"))
				{
					goto IL_633;
				}
			}
			else
			{
				if (!(id == "olderyoungersister"))
				{
					goto IL_633;
				}
				base.idSkin = 1;
				goto IL_633;
			}
		}
		else if (num3 != 3352919697U)
		{
			if (num3 != 3865623817U)
			{
				goto IL_633;
			}
			if (!(id == "dog"))
			{
				goto IL_633;
			}
		}
		else
		{
			if (!(id == "putty_snow"))
			{
				goto IL_633;
			}
			if (EClass.rnd(100) == 0 || EClass.debug.enable)
			{
				base.idSkin = EClass.rnd(4);
				goto IL_633;
			}
			goto IL_633;
		}
		base.idSkin = EClass.rnd(this.sourceCard.tiles.Length);
		if (this.id == "sister_undead" && EClass.rnd(10) == 0)
		{
			SourceBacker.Row row2 = EClass.sources.backers.listSister.NextItem(ref BackerContent.indexSister);
			if (row2 != null && (!EClass.player.doneBackers.Contains(row2.id) || EClass.core.config.test.ignoreBackerDestoryFlag))
			{
				base.ApplyBacker(row2.id);
			}
		}
		IL_633:
		if (this.source.tag.Contains("random_color"))
		{
			base.DyeRandom();
		}
		this.SetAI(new NoGoal());
		if (!this.source.faith.IsEmpty())
		{
			this.SetFaith(this.source.faith);
		}
		else if (EClass.game.activeZone != null && EClass.game.activeZone.id == "foxtown")
		{
			this.SetFaith(EClass.game.religions.Trickery);
		}
		else if (EClass.game.activeZone != null && EClass.game.activeZone.id == "foxtown_nefu")
		{
			this.SetFaith(EClass.game.religions.MoonShadow);
		}
		else
		{
			this.SetFaith(EClass.game.religions.GetRandomReligion(true, EClass.rnd(10) == 0));
		}
		this._affinity = 0;
		this.interest = 100;
		this.CalculateMaxStamina();
		this.Refresh(false);
		this.stamina.value = this.stamina.max;
		this.mana.value = this.mana.max;
		this.isCreated = true;
	}

	public void SetFaith(string id)
	{
		this.SetFaith(EClass.game.religions.dictAll[id]);
	}

	public void SetFaith(Religion r)
	{
		this.faith = r;
		this.RefreshFaithElement();
	}

	public void HealAll()
	{
		this.Cure(CureType.Death, 100, BlessedState.Normal);
		base.hp = this.MaxHP;
		this.mana.value = this.mana.max;
		this.stamina.value = this.stamina.max;
		this.Refresh(false);
	}

	public void Refresh(bool calledRecursive = false)
	{
		if (this.ride != null)
		{
			this.ride.Refresh(true);
		}
		this.hasTelepathy = false;
		this.isWet = false;
		this._isLevitating = (base.HasElement(401, 1) || (this.ride != null && this.ride._isLevitating));
		this.canSeeInvisible = base.HasElement(416, 1);
		base.isHidden = base.HasElement(415, 1);
		foreach (Condition condition in this.conditions)
		{
			condition.OnRefresh();
		}
		if (this.isWet)
		{
			base.isHidden = false;
		}
		this.SetDirtySpeed();
		if (this.host != null && !calledRecursive)
		{
			this.host.Refresh(true);
		}
	}

	public Chara Duplicate()
	{
		Chara chara = CharaGen.Create(this.id, -1);
		chara.mana.value = this.mana.value;
		chara.stamina.value = this.stamina.value;
		foreach (KeyValuePair<int, Element> keyValuePair in this.elements.dict)
		{
			Element element = chara.elements.GetElement(keyValuePair.Key);
			if (element != null)
			{
				element.vBase = keyValuePair.Value.ValueWithoutLink - element.vSource;
			}
		}
		chara.SetFaith(this.faith);
		chara.bio = IO.DeepCopy<Biography>(this.bio);
		chara.hp = Mathf.Max(0, (int)((float)chara.MaxHP * ((float)base.hp / (float)this.MaxHP) * 0.99f));
		chara.LV = base.LV;
		chara.isCopy = true;
		if (base.HaveFur())
		{
			chara.c_fur = -1;
		}
		return chara;
	}

	public int GetBurden(Card t = null, int num = -1)
	{
		int num2 = (base.ChildrenWeight + ((t != null) ? ((num == -1) ? t.ChildrenAndSelfWeight : (t.SelfWeight * num)) : 0)) * 100 / this.WeightLimit;
		if (num2 < 0)
		{
			num2 = 1000;
		}
		if (EClass.debug.ignoreWeight && this.IsPC)
		{
			num2 = 0;
		}
		int num3 = (num2 < 100) ? 0 : ((num2 - 100) / 10 + 1);
		if (num3 > 9)
		{
			num3 = 9;
		}
		return num3;
	}

	public void CalcBurden()
	{
		int num = base.ChildrenWeight * 100 / this.WeightLimit;
		if (num < 0)
		{
			num = 1000;
		}
		if (EClass.debug.ignoreWeight && this.IsPC)
		{
			num = 0;
		}
		this.burden.Set(num);
		this.SetDirtySpeed();
	}

	public void Stumble(int mtp = 100)
	{
		bool flag = EClass._map.FindThing((Thing t) => t.IsInstalled && t.pos.Equals(EClass.pc.pos) && t.trait is TraitStairsUp) != null;
		base.Say(flag ? "dmgBurdenStairs" : "dmgBurdenFallDown", this, null, null);
		int num = this.MaxHP;
		if (base.Evalue(1421) > 0)
		{
			num = this.mana.max;
		}
		int num2 = (num * (base.ChildrenWeight * 100 / this.WeightLimit) / (flag ? 100 : 200) + 1) * mtp / 100;
		if (base.hp <= 0)
		{
			num2 *= 2;
		}
		base.DamageHP(num2, flag ? AttackSource.BurdenStairs : AttackSource.BurdenFallDown, null);
	}

	public int Speed
	{
		get
		{
			if (this.dirtySpeed)
			{
				this.RefreshSpeed(null);
			}
			return this._Speed;
		}
	}

	public void SetDirtySpeed()
	{
		this.dirtySpeed = true;
		if (this.host != null)
		{
			this.host.SetDirtySpeed();
		}
	}

	public void RefreshSpeed(Element.BonusInfo info = null)
	{
		if (this.ride != null && !this.ride.IsDeadOrSleeping)
		{
			this.ride.RefreshSpeed(null);
			this._Speed = this.ride._Speed;
		}
		else if (this.host != null)
		{
			if (this.host.ride == this)
			{
				this._Speed = base.Evalue(79);
				this._Speed = this._Speed * 100 / Mathf.Clamp(100 + this._Speed * (this.race.tag.Contains("noRide") ? 5 : 1) - base.STR - this.host.EvalueRiding() * 2 - (this.race.tag.Contains("ride") ? 50 : 0), 100, 1000);
			}
			else
			{
				this._Speed = (base.Evalue(79) + this.host.Evalue(79)) / 2;
			}
		}
		else
		{
			this._Speed = base.Evalue(79) + base.Evalue(407) / 2;
		}
		BodySlot slot = this.body.GetSlot(37, false, false);
		if (((slot != null) ? slot.thing : null) != null && base.HasElement(1209, 1))
		{
			this._Speed -= 25;
		}
		int num = 100;
		if (this.parasite != null)
		{
			this._Speed = this._Speed * 100 / Mathf.Clamp(120 + this.parasite.LV * 2 - base.STR - base.Evalue(227) * 2, 100, 1000);
		}
		if (this.IsPCFaction)
		{
			switch (this.burden.GetPhase())
			{
			case 1:
				num -= 10;
				if (info != null)
				{
					info.AddFix(-10, this.burden.GetPhaseStr());
				}
				break;
			case 2:
				num -= 20;
				if (info != null)
				{
					info.AddFix(-20, this.burden.GetPhaseStr());
				}
				break;
			case 3:
				num -= 30;
				if (info != null)
				{
					info.AddFix(-30, this.burden.GetPhaseStr());
				}
				break;
			case 4:
				num -= (this.IsPC ? 50 : 100);
				if (info != null)
				{
					info.AddFix(this.IsPC ? -50 : -100, this.burden.GetPhaseStr());
				}
				break;
			}
			if (this.IsPC)
			{
				int phase = this.stamina.GetPhase();
				if (phase != 0)
				{
					if (phase == 1)
					{
						num -= 10;
						if (info != null)
						{
							info.AddFix(-10, this.stamina.GetPhaseStr());
						}
					}
				}
				else
				{
					num -= 20;
					if (info != null)
					{
						info.AddFix(-20, this.stamina.GetPhaseStr());
					}
				}
				phase = this.sleepiness.GetPhase();
				if (phase != 2)
				{
					if (phase == 3)
					{
						num -= 20;
						if (info != null)
						{
							info.AddFix(-20, this.sleepiness.GetPhaseStr());
						}
					}
				}
				else
				{
					num -= 10;
					if (info != null)
					{
						info.AddFix(-10, this.sleepiness.GetPhaseStr());
					}
				}
				switch (this.hunger.GetPhase())
				{
				case 3:
				case 4:
					num -= 10;
					if (info != null)
					{
						info.AddFix(-10, this.hunger.GetPhaseStr());
					}
					break;
				case 5:
					num -= 30;
					if (info != null)
					{
						info.AddFix(-30, this.hunger.GetPhaseStr());
					}
					break;
				}
				num += EClass.player.lastEmptyAlly * base.Evalue(1646);
				if (info != null)
				{
					info.AddFix(EClass.player.lastEmptyAlly * base.Evalue(1646), EClass.sources.elements.map[1646].GetName());
				}
			}
			if (this.IsPCParty && EClass.player.lastEmptyAlly < 0)
			{
				num += EClass.player.lastEmptyAlly * 10 - 10;
				if (info != null)
				{
					info.AddFix(EClass.player.lastEmptyAlly * 10 - 10, "exceedParty".lang());
				}
			}
		}
		if (this.HasCondition<ConGravity>())
		{
			num -= 30;
			if (info != null)
			{
				info.AddFix(-30, this.GetCondition<ConGravity>().Name);
			}
		}
		this._Speed = this._Speed * num / 100;
		if (this._Speed < 10)
		{
			this._Speed = 10;
		}
		this.dirtySpeed = false;
	}

	public void CalculateMaxStamina()
	{
		int num = base.END;
		foreach (Element element in this.elements.dict.Values)
		{
			if (element.source.category == "skill")
			{
				if (this.IsPC)
				{
					num += Mathf.Max(element.vBase, 0);
				}
				else
				{
					num += Mathf.Max(element.ValueWithoutLink, 0);
				}
			}
		}
		int num2 = EClass.curve(num, 30, 10, 60);
		if (num2 < 10)
		{
			num2 = 10;
		}
		this._maxStamina = num2 + 15;
	}

	public override void ApplyEditorTags(EditorTag tag)
	{
		switch (tag)
		{
		case EditorTag.HostilityNeutral:
			this.hostility = (base.c_originalHostility = Hostility.Neutral);
			break;
		case EditorTag.HostilityFriend:
			this.hostility = (base.c_originalHostility = Hostility.Friend);
			break;
		case EditorTag.HostilityEnemy:
			this.hostility = (base.c_originalHostility = Hostility.Enemy);
			break;
		default:
			if (tag != EditorTag.Male)
			{
				if (tag == EditorTag.Female)
				{
					this.bio.SetGender(1);
					base.c_idPortrait = Portrait.GetRandomPortrait(1, this.GetIdPortraitCat());
				}
			}
			else
			{
				this.bio.SetGender(2);
				base.c_idPortrait = Portrait.GetRandomPortrait(2, this.GetIdPortraitCat());
			}
			break;
		}
		base.ApplyEditorTags(tag);
	}

	public override void SetSource()
	{
		this.source = EClass.sources.charas.map.TryGetValue(this.id, null);
		if (this.source == null)
		{
			Debug.LogWarning("Chara " + this.id + " not found");
			this.id = "begger";
			this.source = EClass.sources.charas.map[this.id];
		}
		this.path.walker = this;
	}

	public void SetMainElement(string id, int v = 0, bool elemental = false)
	{
		if (!id.StartsWith("ele"))
		{
			id = "ele" + id;
		}
		this.SetMainElement(EClass.sources.elements.alias[id].id, v, elemental);
	}

	public void SetMainElement(int id, int v = 0, bool elemental = false)
	{
		if (base.c_idMainElement != 0)
		{
			this.elements.SetBase(base.c_idMainElement, 0, 0);
			this.elements.ModBase(EClass.sources.elements.alias[EClass.sources.elements.map[base.c_idMainElement].aliasRef].id, -20);
			base.c_idMainElement = 0;
		}
		if (id == 0)
		{
			return;
		}
		SourceElement.Row row = EClass.sources.elements.map[id];
		base.c_idMainElement = id;
		this.elements.ModBase(id, (v == 0) ? 10 : v);
		this.elements.ModBase(EClass.sources.elements.alias[row.aliasRef].id, 20);
		if (elemental)
		{
			base.isElemental = true;
			this._colorInt = 0;
			Color colorSprite = EClass.setting.elements[this.MainElement.source.alias].colorSprite;
			base.c_lightColor = (int)((byte)Mathf.Clamp(colorSprite.r * 3f, 1f, 31f)) * 1024 + (int)((byte)Mathf.Clamp(colorSprite.g * 3f, 1f, 31f) * 32) + (int)((byte)Mathf.Clamp(colorSprite.b * 3f, 1f, 31f));
		}
		this._ability = null;
	}

	public void ApplyJob(bool remove = false)
	{
		this.elements.ApplyElementMap(base.uid, SourceValueType.Chara, this.job.elementMap, base.DefaultLV, remove, true);
		if (this.IsPCC)
		{
			EClass.game.uniforms.Apply(this.pccData, this.job.id, base.IsMale, true);
		}
	}

	public void ChangeJob(string idNew)
	{
		this.ApplyJob(true);
		base.c_idJob = idNew;
		this._job = null;
		this.ApplyJob(false);
		if (this.IsPCC)
		{
			PCC.Get(this.pccData).Build(false);
		}
	}

	private int ParseBodySlot(string s)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(s);
		if (num <= 2451230377U)
		{
			if (num <= 656231480U)
			{
				if (num != 44370790U)
				{
					if (num == 656231480U)
					{
						if (s == "頭")
						{
							return 30;
						}
					}
				}
				else if (s == "指")
				{
					return 36;
				}
			}
			else if (num != 1212265771U)
			{
				if (num == 2451230377U)
				{
					if (s == "首")
					{
						return 31;
					}
				}
			}
			else if (s == "背")
			{
				return 33;
			}
		}
		else if (num <= 3467005066U)
		{
			if (num != 3039685058U)
			{
				if (num == 3467005066U)
				{
					if (s == "手")
					{
						return 35;
					}
				}
			}
			else if (s == "足")
			{
				return 39;
			}
		}
		else if (num != 3477141168U)
		{
			if (num != 3596583458U)
			{
				if (num == 4097913071U)
				{
					if (s == "腰")
					{
						return 37;
					}
				}
			}
			else if (s == "体")
			{
				return 32;
			}
		}
		else if (s == "腕")
		{
			return 34;
		}
		return -1;
	}

	public void AddRandomBodyPart(bool msg = false)
	{
		int num = new int[]
		{
			30,
			31,
			33,
			35,
			35,
			36,
			34,
			37,
			39
		}.RandomItem<int>();
		this.body.AddBodyPart(num, null);
		if (msg)
		{
			base.Say("gain_bodyparts", this, Element.Get(num).GetName().ToLower(), null);
			base.PlaySound("offering", 1f, true);
		}
	}

	public void ApplyRace(bool remove = false)
	{
		foreach (string s in this.race.figure.Split('|', StringSplitOptions.None))
		{
			int num = this.ParseBodySlot(s);
			if (num != -1)
			{
				if (remove)
				{
					this.body.RemoveBodyPart(num);
				}
				else
				{
					this.body.AddBodyPart(num, null);
				}
			}
		}
		this.elements.ApplyElementMap(base.uid, SourceValueType.Chara, this.race.elementMap, base.DefaultLV, remove, true);
	}

	public void ChangeRace(string idNew)
	{
		this.ApplyRace(true);
		base.c_idRace = idNew;
		this._race = null;
		this.ApplyRace(false);
		base.ChangeMaterial(this.race.material);
	}

	public void MakePartyMemeber()
	{
		this._MakeAlly();
		EClass.pc.party.AddMemeber(this);
	}

	public void MakeAlly(bool msg = true)
	{
		if (this.IsLocalChara && !base.IsUnique)
		{
			Debug.Log("Creating Replacement NPC for:" + ((this != null) ? this.ToString() : null));
			EClass._map.deadCharas.Add(this.CreateReplacement());
		}
		this._MakeAlly();
		if (msg)
		{
			EClass.pc.Say("hire", this, null, null);
			EClass.Sound.Play("good");
		}
		EClass.pc.party.AddMemeber(this);
		if (EClass._zone.IsTown)
		{
			EClass._zone.RefreshListCitizen();
		}
	}

	public void _MakeAlly()
	{
		if (EClass.pc.homeBranch != null)
		{
			EClass.pc.homeBranch.AddMemeber(this);
		}
		else
		{
			this.SetGlobal();
			this.SetFaction(EClass.Home);
		}
		this.hostility = (base.c_originalHostility = Hostility.Ally);
		this.orgPos = null;
		base.c_summonDuration = 0;
		base.isSummon = false;
		this.ReleaseMinion();
		base.SetInt(32, 0);
		this.Refresh(false);
	}

	public bool CanBeTempAlly(Chara c)
	{
		return !this.IsPCFaction && !this.IsGlobal && !this.IsMinion && !this.IsMultisize && EClass._zone.CountMinions(c) <= c.MaxSummon && base.rarity < Rarity.Legendary && !base.HasElement(1222, 1);
	}

	public void MakeMinion(Chara _master, MinionType type = MinionType.Default)
	{
		this.ReleaseMinion();
		this.hostility = (base.c_originalHostility = (_master.IsPCFaction ? Hostility.Ally : _master.hostility));
		base.c_uidMaster = _master.uid;
		base.c_minionType = type;
		this.master = _master;
		this.Refresh(false);
	}

	public void ReleaseMinion()
	{
		Debug.Log("released:" + ((this != null) ? this.ToString() : null));
		base.c_uidMaster = 0;
		this.master = null;
		this.enemy = null;
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.enemy == this)
			{
				chara.SetEnemy(null);
				chara.ai.Cancel();
			}
		}
		this.ai.Cancel();
		this.Refresh(false);
	}

	public void SetSummon(int duration)
	{
		base.c_summonDuration = duration;
		base.isSummon = true;
	}

	public Chara FindMaster()
	{
		if (this.IsMinion)
		{
			this.master = EClass._map.FindChara(base.c_uidMaster);
		}
		return this.master;
	}

	public bool IsEscorted()
	{
		if (!this.IsPCPartyMinion)
		{
			return false;
		}
		foreach (Quest quest in EClass.game.quests.list)
		{
			QuestEscort questEscort = quest as QuestEscort;
			if (questEscort != null && questEscort.uidChara == base.uid)
			{
				return true;
			}
		}
		return false;
	}

	public bool CanDestroyPath()
	{
		return this.IsMultisize || (base.rarity >= Rarity.Superior && !base.IsPCFactionOrMinion && this.ai is GoalCombat && !EClass._zone.IsPCFaction && !(EClass._zone is Zone_Town));
	}

	public bool CanMoveTo(Point p, bool allowDestroyPath = true)
	{
		if (!p.IsValid)
		{
			return false;
		}
		int num = (p.z < this.pos.z) ? 0 : ((p.x > this.pos.x) ? 1 : ((p.z > this.pos.z) ? 2 : 3));
		if (allowDestroyPath && this.CanDestroyPath())
		{
			if (!p.IsInBounds)
			{
				return false;
			}
		}
		else
		{
			if (EClass._map.cells[p.x, p.z].blocked || EClass._map.cells[this.pos.x, this.pos.z].weights[num] == 0)
			{
				return false;
			}
			if (p.x != this.pos.x && p.z != this.pos.z)
			{
				Cell[,] cells = EClass._map.cells;
				int x = p.x;
				int z = this.pos.z;
				int num2 = (z < this.pos.z) ? 0 : ((x > this.pos.x) ? 1 : ((z > this.pos.z) ? 2 : 3));
				if (cells[this.pos.x, this.pos.z].weights[num2] == 0)
				{
					return false;
				}
				if (cells[x, z].blocked)
				{
					return false;
				}
				num2 = ((z < p.z) ? 0 : ((x > p.x) ? 1 : ((z > p.z) ? 2 : 3)));
				if (cells[p.x, p.z].weights[num2] == 0)
				{
					return false;
				}
				x = this.pos.x;
				z = p.z;
				num2 = ((z < this.pos.z) ? 0 : ((x > this.pos.x) ? 1 : ((z > this.pos.z) ? 2 : 3)));
				if (cells[this.pos.x, this.pos.z].weights[num2] == 0)
				{
					return false;
				}
				if (cells[x, z].blocked)
				{
					return false;
				}
				num2 = ((z < p.z) ? 0 : ((x > p.x) ? 1 : ((z > p.z) ? 2 : 3)));
				if (cells[p.x, p.z].weights[num2] == 0)
				{
					return false;
				}
			}
		}
		if (this.IsPC)
		{
			if (this.IsEnemyOnPath(p, true))
			{
				return false;
			}
		}
		else if (p.HasChara && !this.IsMultisize && !this.CanReplace(p.FirstChara))
		{
			return false;
		}
		return true;
	}

	public bool IsEnemyOnPath(Point p, bool cancelAI = true)
	{
		if (!this.currentZone.IsRegion && p.IsValid)
		{
			CellDetail detail = p.detail;
			if (detail != null && detail.charas.Count > 0)
			{
				foreach (Chara chara in p.detail.charas)
				{
					if (chara.IsHostile(this) || !chara.trait.CanBePushed)
					{
						if (cancelAI && EClass.pc.ai is GoalManualMove)
						{
							EClass.pc.ai.Cancel();
						}
						return true;
					}
				}
				return false;
			}
		}
		return false;
	}

	public bool CanInteractTo(Card c)
	{
		return this.CanInteractTo(c.pos);
	}

	public bool CanInteractTo(Point p)
	{
		if (!p.IsValid)
		{
			return false;
		}
		if (EClass._map.cells[p.x, p.z].blocked)
		{
			return true;
		}
		int num = (p.z < this.pos.z) ? 0 : ((p.x > this.pos.x) ? 1 : ((p.z > this.pos.z) ? 2 : 3));
		if (EClass._map.cells[this.pos.x, this.pos.z].weights[num] == 0)
		{
			return false;
		}
		if (p.x != this.pos.x && p.z != this.pos.z)
		{
			Cell[,] cells = EClass._map.cells;
			int x = p.x;
			int z = this.pos.z;
			int num2 = (z < this.pos.z) ? 0 : ((x > this.pos.x) ? 1 : ((z > this.pos.z) ? 2 : 3));
			if (cells[this.pos.x, this.pos.z].weights[num2] == 0)
			{
				return false;
			}
			num2 = ((z < p.z) ? 0 : ((x > p.x) ? 1 : ((z > p.z) ? 2 : 3)));
			if (cells[p.x, p.z].weights[num2] == 0)
			{
				return false;
			}
			x = this.pos.x;
			z = p.z;
			num2 = ((z < this.pos.z) ? 0 : ((x > this.pos.x) ? 1 : ((z > this.pos.z) ? 2 : 3)));
			if (cells[this.pos.x, this.pos.z].weights[num2] == 0)
			{
				return false;
			}
			num2 = ((z < p.z) ? 0 : ((x > p.x) ? 1 : ((z > p.z) ? 2 : 3)));
			if (cells[p.x, p.z].weights[num2] == 0)
			{
				return false;
			}
		}
		return true;
	}

	public Point GetFirstStep(Point newPoint, PathManager.MoveType moveType = PathManager.MoveType.Default)
	{
		return PathManager.Instance.GetFirstStep(this.pos, newPoint, this, this.IsPC ? 40 : 10, this.IsMultisize ? PathManager.MoveType.Default : moveType);
	}

	public bool MoveRandom()
	{
		Point randomNeighbor = this.pos.GetRandomNeighbor();
		return !randomNeighbor.Equals(this.pos) && !randomNeighbor.HasChara && this.HasAccess(randomNeighbor) && this.TryMove(randomNeighbor, true) == Card.MoveResult.Success;
	}

	public bool MoveNeighborDefinitely()
	{
		List<Point> list = new List<Point>();
		this.pos.ForeachNeighbor(delegate(Point p)
		{
			list.Add(p.Copy());
		}, true);
		list.Shuffle<Point>();
		foreach (Point point in list)
		{
			if (!point.Equals(this.pos) && !point.HasChara && this.TryMove(point, true) == Card.MoveResult.Success)
			{
				return true;
			}
		}
		return false;
	}

	public void MoveByForce(Point newPoint, Card c = null, bool checkWall = false)
	{
		if (newPoint.sourceBlock.tileType.IsBlockPass)
		{
			return;
		}
		if (checkWall && (base.Dist(newPoint) > 1 || !this.CanMoveTo(newPoint, false)))
		{
			return;
		}
		if (this._Move(newPoint, Card.MoveType.Force) == Card.MoveResult.Success && this.ai.Current.CancelWhenMoved)
		{
			this.ai.Current.TryCancel(c);
		}
	}

	public Card.MoveResult TryMoveTowards(Point p)
	{
		if (p.Equals(this.pos))
		{
			return Card.MoveResult.Success;
		}
		if (this.IsPC && EClass.player.TooHeavyToMove())
		{
			return Card.MoveResult.Fail;
		}
		Chara._sharedPos.Set(p);
		if (this.CanDestroyPath() && this.TryMove(this.pos.GetPointTowards(Chara._sharedPos), true) == Card.MoveResult.Success)
		{
			return Card.MoveResult.Success;
		}
		int num = this.pos.Distance(p);
		PathProgress pathProgress = PathManager.Instance.RequestPathImmediate(this.pos, p, this, PathManager.MoveType.Default, num + 4, 1);
		if (pathProgress.HasPath && pathProgress.nodes.Count < num + 4)
		{
			PathFinderNode pathFinderNode = pathProgress.nodes[pathProgress.nodes.Count - 1];
			if (this.TryMove(new Point(pathFinderNode.X, pathFinderNode.Z), true) == Card.MoveResult.Success)
			{
				return Card.MoveResult.Success;
			}
		}
		pathProgress = PathManager.Instance.RequestPathImmediate(this.pos, p, this, PathManager.MoveType.Combat, num + 4, 1);
		if (pathProgress.HasPath && pathProgress.nodes.Count < num + 4)
		{
			PathFinderNode pathFinderNode2 = pathProgress.nodes[pathProgress.nodes.Count - 1];
			if (this.TryMove(new Point(pathFinderNode2.X, pathFinderNode2.Z), true) == Card.MoveResult.Success)
			{
				return Card.MoveResult.Success;
			}
		}
		pathProgress = PathManager.Instance.RequestPathImmediate(this.pos, p, this, PathManager.MoveType.Default, num + 25, 2);
		if (pathProgress.HasPath && pathProgress.nodes.Count < num + 25)
		{
			PathFinderNode pathFinderNode3 = pathProgress.nodes[pathProgress.nodes.Count - 1];
			if (this.TryMove(new Point(pathFinderNode3.X, pathFinderNode3.Z), true) == Card.MoveResult.Success)
			{
				return Card.MoveResult.Success;
			}
		}
		pathProgress = PathManager.Instance.RequestPathImmediate(this.pos, p, this, PathManager.MoveType.Combat, num + 25, 2);
		if (pathProgress.HasPath && pathProgress.nodes.Count < num + 25)
		{
			PathFinderNode pathFinderNode4 = pathProgress.nodes[pathProgress.nodes.Count - 1];
			if (this.TryMove(new Point(pathFinderNode4.X, pathFinderNode4.Z), true) == Card.MoveResult.Success)
			{
				return Card.MoveResult.Success;
			}
		}
		return Card.MoveResult.Fail;
	}

	public Card.MoveResult TryMoveFrom(Point p)
	{
		if (this.IsPC && EClass.player.TooHeavyToMove())
		{
			return Card.MoveResult.Fail;
		}
		Point point = p.Copy();
		int num = p.x - this.pos.x;
		int num2 = p.z - this.pos.z;
		if (num > 1)
		{
			num = 1;
		}
		else if (num < -1)
		{
			num = -1;
		}
		if (num2 > 1)
		{
			num2 = 1;
		}
		else if (num2 < -1)
		{
			num2 = -1;
		}
		if (num == 0 && num2 == 0)
		{
			num = EClass.rnd(3) - 1;
			num2 = EClass.rnd(3) - 1;
		}
		point.Set(this.pos);
		point.x -= num;
		point.z -= num2;
		if (point.IsValid && !point.HasChara)
		{
			return this.TryMove(point, false);
		}
		return Card.MoveResult.Fail;
	}

	public Card.MoveResult TryMove(Point newPoint, bool allowDestroyPath = true)
	{
		using (List<Condition>.Enumerator enumerator = this.conditions.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.TryMove(newPoint))
				{
					return Card.MoveResult.Fail;
				}
			}
		}
		if (base.isRestrained)
		{
			if (!this.IsPC)
			{
				return Card.MoveResult.Fail;
			}
			base.isRestrained = false;
		}
		if (!this.CanMoveTo(newPoint, allowDestroyPath))
		{
			return Card.MoveResult.Fail;
		}
		return this._Move(newPoint, Card.MoveType.Walk);
	}

	public override Card.MoveResult _Move(Point newPoint, Card.MoveType type = Card.MoveType.Walk)
	{
		if (this.isDead)
		{
			return Card.MoveResult.Fail;
		}
		if (this.IsPC)
		{
			float num = EClass.setting.defaultActPace;
			switch (this.burden.GetPhase())
			{
			case 3:
				num *= 1.5f;
				break;
			case 4:
				num *= 2f;
				break;
			}
			if (this.currentZone.IsRegion)
			{
				int num2 = 30;
				if (!EClass.pc.HasElement(408, 1))
				{
					switch (EClass.world.weather.CurrentCondition)
					{
					case Weather.Condition.Rain:
						num2 += 5;
						break;
					case Weather.Condition.RainHeavy:
						num2 += 10;
						num *= 1.5f;
						break;
					case Weather.Condition.Snow:
						num2 += 10;
						break;
					case Weather.Condition.SnowHeavy:
						num2 += 15;
						num *= 1.5f;
						break;
					}
				}
				if (newPoint.matFloor.id == 48)
				{
					num2 += 20;
					num *= 1.8f;
				}
				num2 = num2 * 100 / (100 + base.Evalue(240) + base.Evalue(407) * 5);
				EClass.world.date.AdvanceMin(num2 * 6);
				EClass.player.lastZonePos = null;
				EClass.player.distanceTravel++;
				int hp = base.hp;
				for (int i = 0; i < num2 * 4; i++)
				{
					EClass.pc.party.members.ForeachReverse(delegate(Chara m)
					{
						if (!m.isDead)
						{
							m.TickConditions();
						}
					});
					if (base.hp < this.MaxHP / 5 && base.hp < hp && !EClass.player.regionMoveWarned)
					{
						EClass.player.regionMoveWarned = true;
						Msg.Say("regionAbortMove");
						EInput.Consume(true, 1);
						this.SetAI(new NoGoal());
						return Card.MoveResult.Fail;
					}
				}
				if (newPoint.cell.CanSuffocate())
				{
					this.AddCondition<ConSuffocation>((EClass.pc.Evalue(200) > 0) ? (2000 / (100 + base.Evalue(200) * 10)) : 30, false);
					ConSuffocation condition = this.GetCondition<ConSuffocation>();
					int num3 = (condition != null) ? condition.GetPhase() : 0;
					if (num3 >= 2)
					{
						base.DamageHP(EClass.rndHalf(10 + this.MaxHP / 5), AttackSource.Condition, null);
					}
					if (!this.isDead && !base.HasElement(429, 1))
					{
						base.ModExp(200, 8 + num3 * 12);
					}
				}
				EClass.player.regionMoveWarned = false;
				if (this.isDead)
				{
					return Card.MoveResult.Fail;
				}
			}
			if (num > EClass.setting.defaultActPace * 3f)
			{
				num = EClass.setting.defaultActPace * 3f;
			}
			this.actTime = num;
		}
		Chara chara = (this.ride == null) ? this : this.ride;
		if ((!EClass._zone.IsRegion || chara.IsPC) && ((chara.isConfused && EClass.rnd(2) == 0) || (chara.isDrunk && EClass.rnd(this.IsIdle ? 2 : 8) == 0 && !chara.HasElement(1215, 1))) && newPoint.Distance(this.pos) <= 1)
		{
			Point randomNeighbor = this.pos.GetRandomNeighbor();
			if (this.CanMoveTo(randomNeighbor, false))
			{
				newPoint = randomNeighbor;
				if (this.isDrunk)
				{
					base.Talk("drunk", null, null, false);
				}
			}
		}
		if (newPoint.x != this.pos.x || newPoint.z != this.pos.z)
		{
			this.LookAt(newPoint);
		}
		CellEffect effect = base.Cell.effect;
		if (effect != null && effect.id == 7)
		{
			CellEffect effect2 = base.Cell.effect;
			if (this.race.height < 500 && !this.race.tag.Contains("webfree") && EClass.rnd(effect2.power + 25) > EClass.rnd(base.STR + base.DEX + 1))
			{
				base.Say("abWeb_caught", this, null, null);
				base.PlaySound("web", 1f, true);
				effect2.power = effect2.power * 3 / 4;
				this.renderer.PlayAnime(AnimeID.Shiver, default(Vector3), false);
				return Card.MoveResult.Fail;
			}
			base.Say("abWeb_pass", this, null, null);
			EClass._map.SetEffect((int)base.Cell.x, (int)base.Cell.z, null);
		}
		if (this.IsPC)
		{
			if (EClass._zone.IsRegion)
			{
				this.actTime *= EClass.setting.render.anime.regionSpeed;
			}
			else if ((newPoint.x > this.pos.x && newPoint.z > this.pos.z) || (newPoint.x < this.pos.x && newPoint.z < this.pos.z))
			{
				this.actTime += this.actTime * EClass.setting.render.anime.diagonalSpeed;
			}
		}
		if (newPoint.cell.hasDoor)
		{
			foreach (Thing thing in this.pos.Things)
			{
				TraitDoor traitDoor = thing.trait as TraitDoor;
				if (traitDoor != null && traitDoor.owner.c_lockLv > 0)
				{
					if (base.INT < 10)
					{
						return Card.MoveResult.Fail;
					}
					traitDoor.TryOpenLock(this, true);
					return Card.MoveResult.Door;
				}
			}
		}
		Cell cell = newPoint.cell;
		Cell cell2 = this.pos.cell;
		bool flag = cell.HasLiquid && !this.IsLevitating;
		bool hasBridge = cell.HasBridge;
		bool hasRamp = cell.HasRamp;
		bool flag2 = EClass._zone.IsSnowCovered && !cell.HasRoof && !cell.isClearSnow;
		TileRow tileRow = hasRamp ? cell.sourceBlock : (hasBridge ? cell.sourceBridge : cell.sourceFloor);
		SourceMaterial.Row row = hasRamp ? cell.matBlock : (hasBridge ? cell.matBridge : cell.matFloor);
		bool flag3 = cell.IsTopWater && !cell.isFloating;
		if (!EClass._zone.IsRegion)
		{
			if (cell.hasDoorBoat)
			{
				tileRow = FLOOR.sourceWood;
				row = MATERIAL.sourceOak;
				flag3 = false;
			}
			else if (flag2 && !tileRow.ignoreSnow)
			{
				if (tileRow.tileType.IsWater)
				{
					tileRow = FLOOR.sourceIce;
					row = MATERIAL.sourceIce;
				}
				else
				{
					tileRow = FLOOR.sourceSnow;
					row = MATERIAL.sourceSnow;
				}
				flag3 = false;
			}
		}
		if ((this.pos.sourceFloor.isBeach || cell2.IsSnowTile) && !this.pos.HasObj)
		{
			EClass._map.SetFoormark(this.pos, 1, (int)Util.GetAngle((float)(this.pos.x - newPoint.x), (float)(this.pos.z - newPoint.z)), cell2.IsSnowTile ? 312 : 304);
		}
		if (this.isSynced)
		{
			string text = (flag || flag3) ? "water" : tileRow.soundFoot.IsEmpty(row.soundFoot.IsEmpty("default"));
			if (cell.obj != 0 && cell.sourceObj.tileType.IsPlayFootSound && !cell.matObj.soundFoot.IsEmpty())
			{
				text = cell.matObj.soundFoot;
			}
			if (!text.IsEmpty())
			{
				SoundManager.altLastData = this.IsPC;
				base.PlaySound("Footstep/" + text, this.IsPC ? 1f : 0.9f, true);
			}
			if (!flag3)
			{
				Scene scene = EClass.scene;
				PCOrbit pcOrbit = EClass.screen.pcOrbit;
				bool flag4 = scene.actionMode.gameSpeed > 1f;
				scene.psFoot.transform.position = this.renderer.position + pcOrbit.footPos;
				scene.psFoot.startColor = row.matColor;
				scene.psFoot.Emit(pcOrbit.emitFoot * (flag4 ? 2 : 1));
				if (flag4 && this.IsPC)
				{
					scene.psSmoke.transform.position = this.renderer.position + pcOrbit.smokePos;
					scene.psSmoke.Emit(pcOrbit.emitSmoke);
				}
			}
			if (flag || flag3)
			{
				Effect.Get("ripple").Play(0.4f * this.actTime * EClass.scene.actionMode.gameSpeed, newPoint, 0f, null, null);
			}
		}
		this.lastPos.Set(this.pos);
		if (type != Card.MoveType.Force)
		{
			if (newPoint.HasChara && this.ai.Current.PushChara)
			{
				this.TryPush(newPoint);
			}
			if (newPoint.HasChara && newPoint.Charas.Count == 1)
			{
				Chara chara2 = newPoint.Charas[0];
				if (this.CanReplace(chara2))
				{
					chara2.MoveByForce(this.lastPos, this, false);
					if (chara.IsPC)
					{
						base.Say("replace_pc", chara, chara2, null, null);
					}
				}
			}
		}
		if (cell.hasDoor)
		{
			foreach (Thing thing2 in newPoint.Things)
			{
				TraitDoor traitDoor2 = thing2.trait as TraitDoor;
				if (traitDoor2 != null)
				{
					traitDoor2.TryOpen(this);
				}
			}
		}
		EClass._zone.map.MoveCard(newPoint, this);
		this.SyncRide();
		if (this.IsPC && EClass._zone.PetFollow && (!EClass._zone.KeepAllyDistance || !EClass.game.config.tactics.allyKeepDistance) && !EClass._zone.IsRegion)
		{
			foreach (Chara chara3 in EClass.pc.party.members)
			{
				if (chara3.isLeashed && !chara3.IsPC && chara3.host == null && !chara3.IsDisabled && !chara3.HasCondition<ConEntangle>() && !chara3.IsInCombat && chara3.Dist(EClass.pc) > 1)
				{
					chara3.TryMoveTowards(EClass.pc.pos);
				}
			}
		}
		if (EClass.core.config.test.animeFramePCC == 0 && this.isSynced && this.renderer.hasActor && this.renderer.actor.isPCC)
		{
			this.renderer.NextFrame();
		}
		if (this.IsPC)
		{
			base.PlaySound("Footstep/Extra/pcfootstep", 1f, true);
			if (this.pos.HasThing)
			{
				foreach (Card card in this.pos.ListCards(false))
				{
					if (card.isThing && card.placeState == PlaceState.roaming && !card.ignoreAutoPick)
					{
						if (EClass.core.config.game.advancedMenu)
						{
							Window.SaveData dataPick = EClass.player.dataPick;
							ContainerFlag containerFlag = card.category.GetRoot().id.ToEnum(true);
							if (containerFlag == ContainerFlag.none)
							{
								containerFlag = ContainerFlag.other;
							}
							if ((!dataPick.noRotten || !card.IsDecayed) && (!dataPick.onlyRottable || card.trait.Decay != 0) && (!dataPick.userFilter || dataPick.IsFilterPass(card.GetName(NameStyle.Full, 1))))
							{
								if (dataPick.advDistribution)
								{
									using (HashSet<int>.Enumerator enumerator4 = dataPick.cats.GetEnumerator())
									{
										while (enumerator4.MoveNext())
										{
											int num4 = enumerator4.Current;
											if (card.category.uid == num4)
											{
												this.Pick(card.Thing, true, true);
											}
										}
										continue;
									}
								}
								if (!dataPick.flag.HasFlag(containerFlag))
								{
									this.Pick(card.Thing, true, true);
								}
							}
						}
						else
						{
							this.Pick(card.Thing, true, true);
						}
					}
				}
			}
			if (EClass._zone.IsRegion)
			{
				EloMap.Cell cell3 = EClass.scene.elomap.GetCell(EClass.pc.pos);
				if (((cell3 != null) ? cell3.zone : null) != null && !cell3.zone.HiddenInRegionMap && (!(cell3.zone is Zone_Field) || cell3.zone.children.Count > 0 || cell3.zone.IsPCFaction))
				{
					Msg.Say((!cell3.zone.source.GetText("textFlavor", false).IsEmpty()) ? cell3.zone.source.GetText("textFlavor", false) : (cell3.zone.ShowDangerLv ? "seeZoneDanger" : "seeZone"), cell3.zone.Name, cell3.zone.DangerLv.ToString() ?? "", null, null);
				}
				if (this.pos.matFloor.alias == "snow" && EClass.rnd(3) == 0)
				{
					Msg.SetColor(Msg.colors.Ono);
					Msg.Say(Lang.GetList("walk_snow").RandomItem<string>());
				}
				else if (EClass.world.weather.CurrentCondition == Weather.Condition.RainHeavy && EClass.rnd(3) == 0)
				{
					Msg.SetColor(Msg.colors.Ono);
					Msg.Say(Lang.GetList("walk_storm").RandomItem<string>());
				}
			}
			ActWait.Search(EClass.pc, false);
		}
		if (this.IsPCC)
		{
			int num5 = Mathf.Abs((int)(cell2.topHeight - cell.topHeight));
			if ((num5 >= 3 && this.lastPos.sourceBlock.tileType.slopeHeight == 0 && newPoint.sourceBlock.tileType.slopeHeight == 0) || cell2.hasDoorBoat || cell.hasDoorBoat)
			{
				this.renderer.PlayAnime((cell2.topHeight >= cell.topHeight) ? AnimeID.JumpDown : ((num5 >= 6) ? AnimeID.Jump : AnimeID.JumpSmall), default(Vector3), false);
			}
			else
			{
				float surfaceHeight = cell2.GetSurfaceHeight();
				float surfaceHeight2 = cell.GetSurfaceHeight();
				num5 = (int)Mathf.Abs((surfaceHeight - surfaceHeight2) * 100f);
				if (num5 >= 15)
				{
					this.renderer.PlayAnime((surfaceHeight >= surfaceHeight2) ? AnimeID.JumpDown : ((num5 >= 40) ? AnimeID.Jump : AnimeID.JumpSmall), default(Vector3), false);
				}
			}
		}
		this.lastPos.Things.ForeachReverse(delegate(Thing t)
		{
			t.trait.OnSteppedOut(this);
		});
		if (!this.IsPC)
		{
			this.pos.Things.ForeachReverse(delegate(Thing t)
			{
				t.trait.OnStepped(this);
			});
		}
		if (this.CanDestroyPath())
		{
			this.DestroyPath(this.pos);
		}
		if (this.IsPC)
		{
			if (this.renderer.anime == null && this.renderer.replacer != null)
			{
				this.renderer.PlayAnime(AnimeID.Hop, default(Vector3), false);
			}
			if (EClass.player.flags.isShoesOff)
			{
				if (!FLOOR.IsTatami(this.pos.cell.sourceSurface.id) && this.pos.cell.room == null)
				{
					EClass.player.flags.isShoesOff = false;
					EClass.pc.Say("shoes_on", EClass.pc, null, null);
					EClass.pc.SetPCCState(PCCState.Normal);
				}
			}
			else if (FLOOR.IsTatami(this.pos.cell.sourceSurface.id) && this.pos.cell.room != null)
			{
				EClass.player.flags.isShoesOff = true;
				EClass.pc.Say("shoes_off", EClass.pc, null, null);
				EClass.pc.SetPCCState(PCCState.ShoesOff);
			}
		}
		this.hasMovedThisTurn = true;
		return Card.MoveResult.Success;
	}

	public void DestroyPath(Point pos)
	{
		bool broke = false;
		pos.ForeachMultiSize(base.W, base.H, delegate(Point _p, bool main)
		{
			if (!_p.IsValid)
			{
				return;
			}
			if (_p.HasBlock)
			{
				EClass._map.MineBlock(_p, false, this);
				if (_p.HasObj)
				{
					EClass._map.MineObj(_p, null, this);
				}
				broke = true;
			}
			if (_p.HasObj && _p.IsBlocked)
			{
				EClass._map.MineObj(_p, null, this);
				broke = true;
			}
			_p.Things.ForeachReverse(delegate(Thing t)
			{
				if (t.IsInstalled && (t.trait.IsBlockPath || t.trait.IsDoor))
				{
					if (t.isNPCProperty && t.trait.CanBeDestroyed)
					{
						t.Destroy();
						return;
					}
					t.SetPlaceState(PlaceState.roaming, false);
				}
			});
		});
		if (broke)
		{
			Msg.Say("stomp");
			Shaker.ShakeCam("stomp", 1f);
		}
	}

	public void TryPush(Point point)
	{
		point.Charas.ForeachReverse(delegate(Chara c)
		{
			if (c.ai.IsMoveAI || c.IsPC || !c.trait.CanBePushed || c == this || c.noMove || (EClass._zone.IsRegion && !c.IsPCFactionOrMinion))
			{
				return;
			}
			List<Point> list = new List<Point>();
			for (int i = point.x - 1; i <= point.x + 1; i++)
			{
				for (int j = point.z - 1; j <= point.z + 1; j++)
				{
					if (i != point.x || j != point.z)
					{
						Point point2 = new Point(i, j);
						if (point2.IsValid && !point2.HasChara && !point2.IsBlocked && !point2.cell.hasDoor && !point2.IsBlockByHeight(point))
						{
							list.Add(point2);
						}
					}
				}
			}
			if (list.Count > 0)
			{
				if (list.Count > 1)
				{
					list.ForeachReverse(delegate(Point p)
					{
						if (p.Equals(new Point(point.x + point.x - this.pos.x, point.z + point.z - this.pos.z)))
						{
							list.Remove(p);
						}
					});
				}
				Point newPoint = list.RandomItem<Point>();
				if (this.IsPC)
				{
					this.Say("displace", this, c, null, null);
					this.PlaySound("push", 1f, true);
				}
				else if (c.isSynced)
				{
					c.PlayEffect("push", true, 0f, default(Vector3));
				}
				c.MoveByForce(newPoint, this, true);
				if (this.IsPC && !c.IsPCParty && !c.IsUnique && c.IsHuman && EClass.rnd(5) == 0)
				{
					c.Talk("pushed", null, null, false);
				}
			}
		});
	}

	public bool CanReplace(Chara c)
	{
		if (c.IsMultisize || !c.trait.CanBePushed || c.noMove || this.IsHostile(c) || this.IsMinion || (EClass._zone.IsRegion && !c.IsPCFactionOrMinion))
		{
			return false;
		}
		if (this.IsPC)
		{
			return true;
		}
		if (c.IsPC || c.pos.Equals(EClass.pc.pos) || c.host != null)
		{
			return false;
		}
		if (!this.IsHostile(c))
		{
			if (c.c_uidMaster != 0 || c.isSummon || base.IsPowerful || this.IsEscorted())
			{
				return true;
			}
			if (this.DestDist < c.DestDist)
			{
				return true;
			}
			if (this.IsPCParty && !c.IsPCParty)
			{
				return true;
			}
			if (this.IsPCFaction && !c.IsPCParty)
			{
				return true;
			}
		}
		return false;
	}

	public void MoveZone(string alias)
	{
		this.MoveZone(EClass.game.spatials.Find(alias), ZoneTransition.EnterState.Auto);
	}

	public void MoveZone(Zone z, ZoneTransition.EnterState state = ZoneTransition.EnterState.Auto)
	{
		this.MoveZone(z, new ZoneTransition
		{
			state = state
		});
	}

	public void MoveZone(Zone z, ZoneTransition transition)
	{
		if (z == this.currentZone)
		{
			return;
		}
		if (this.HasCondition<ConInvulnerable>())
		{
			this.RemoveCondition<ConInvulnerable>();
		}
		if (this.IsPC)
		{
			EClass.player.nextZone = z;
			if (this.IsInActiveZone && !EClass.player.simulatingZone)
			{
				if (this.held != null && this.held.trait.CanOnlyCarry)
				{
					this.DropHeld(null);
				}
				if (z.instance == null && this.currentZone.instance == null && !EClass.player.deathZoneMove && !EClass.pc.isDead && (z.IsPCFaction || z.WillAutoSave) && z.GetTopZone() != EClass._zone.GetTopZone())
				{
					if (EClass.player.returnInfo != null)
					{
						EClass.player.returnInfo.turns += 5;
					}
					if (!EClass.debug.ignoreAutoSave)
					{
						EClass.game.Save(true, null, false);
					}
				}
				EClass.player.deathZoneMove = false;
			}
			this.currentZone.events.OnLeaveZone();
			if (this.currentZone.instance != null)
			{
				ZoneInstance instance = this.currentZone.instance;
				z = EClass.game.spatials.Find(instance.uidZone);
				transition = new ZoneTransition
				{
					state = instance.ReturnState,
					x = instance.x,
					z = instance.z
				};
				instance.OnLeaveZone();
				Debug.Log(z);
			}
			EInput.Consume(true, 1);
			EClass.player.uidLastZone = this.currentZone.uid;
			if (!EClass.player.simulatingZone)
			{
				if (this.currentZone.IsRegion)
				{
					Msg.Say("enterZone", z.Name, null, null, null);
				}
				else
				{
					if (z.IsRegion)
					{
						Msg.Say("leaveZone", this.currentZone.Name, null, null, null);
					}
					else if (z.id != this.currentZone.id)
					{
						Msg.Say("enterZone", z.Name, null, null, null);
					}
					EClass.player.lastZonePos = this.pos.Copy();
				}
				EClass.player.lastTransition = transition;
			}
			foreach (Chara t in (from c in EClass._map.charas
			where c.IsPCPartyMinion && c.master != EClass.pc
			select c).ToList<Chara>())
			{
				EClass._zone.RemoveCard(t);
			}
			EClass.player.listSummon = (from c in EClass._map.charas
			where c.c_uidMaster != 0 && c.FindMaster() == EClass.pc && c.c_minionType == MinionType.Default
			select c).ToList<Chara>();
			foreach (Chara t2 in EClass.player.listSummon)
			{
				EClass._zone.RemoveCard(t2);
			}
		}
		if (this.party != null && this.party.leader == this)
		{
			foreach (Chara chara in this.party.members)
			{
				if (chara != this && !chara.isDead && chara.parent is Zone)
				{
					chara.MoveZone(z, ZoneTransition.EnterState.Auto);
				}
			}
		}
		if (this.global == null)
		{
			Debug.Log(base.Name);
			return;
		}
		ZoneTransition zoneTransition = transition;
		Zone currentZone = this.currentZone;
		zoneTransition.uidLastZone = ((currentZone != null) ? currentZone.uid : 0);
		this.global.transition = transition;
		if (z.IsActiveZone)
		{
			Point spawnPos = z.GetSpawnPos(this, ZoneTransition.EnterState.Auto);
			this.global.transition = null;
			if (this.IsPC)
			{
				EClass.player.lastTransition = null;
			}
			z.AddCard(this, spawnPos);
			if (this.IsBranchMember())
			{
				EClass._map.rooms.AssignCharas();
				return;
			}
		}
		else
		{
			z.AddCard(this);
		}
	}

	public void MoveHome(string id, int x = -1, int z = -1)
	{
		this.MoveHome(EClass.game.world.FindZone(id), x, z);
	}

	public void MoveHome(Zone zone, int x = -1, int z = -1)
	{
		if (this.isDead)
		{
			this.Revive(null, false);
		}
		else
		{
			this.Cure(CureType.Death, 100, BlessedState.Normal);
		}
		this.CureCondition<ConSuspend>(99999);
		if (this.IsPCParty)
		{
			EClass.pc.party.RemoveMember(this);
		}
		FactionBranch homeBranch = this.homeBranch;
		if (x == -1)
		{
			x = 50;
		}
		if (z == -1)
		{
			z = 50;
		}
		Point point = new Point(x, z);
		if (zone.IsActiveZone)
		{
			point = point.GetNearestPoint(false, false, true, false);
		}
		zone.AddCard(this, point);
		this.SetHomeZone(zone);
		this.global.transition = new ZoneTransition
		{
			state = ZoneTransition.EnterState.Dead,
			x = point.x,
			z = point.z
		};
		this.orgPos = new Point(x, z);
		if (homeBranch != null)
		{
			this.RefreshWorkElements(null);
			homeBranch.policies.Validate();
		}
	}

	public void FallFromZone()
	{
		Msg.Say("skyFall", EClass.pc, EClass._zone.Name, null, null);
		Zone zone = EClass._zone.isExternalZone ? null : EClass._zone.GetTopZone().FindZone(EClass._zone.lv - 1);
		zone = (zone ?? EClass.world.region);
		this.MoveZone(zone ?? EClass.world.region, new ZoneTransition
		{
			state = ZoneTransition.EnterState.Fall,
			x = this.pos.x,
			z = this.pos.z
		});
	}

	public override void SetDir(int d)
	{
		base.dir = d;
		this.UpdateAngle();
		this.renderer.RefreshSprite();
	}

	public override void Rotate(bool reverse = false)
	{
		if (this.renderer.hasActor)
		{
			base.dir = (base.dir + (reverse ? -1 : 1)).Clamp(0, 3, true);
		}
		else
		{
			base.dir = ((base.dir == 0) ? 1 : 0);
		}
		this.UpdateAngle();
		this.renderer.RefreshSprite();
	}

	public override void LookAt(Card c)
	{
		this.LookAt(c.pos);
	}

	public override void LookAt(Point p)
	{
		this.angle = Util.GetAngle((float)(p.x - this.pos.x), (float)(p.z - this.pos.z));
		if (EClass._zone.IsRegion)
		{
			if (this.angle > 100f && this.angle < 170f)
			{
				base.dir = 2;
			}
			else if (this.angle > 170f && this.angle < 190f)
			{
				base.dir = 0;
			}
			else if (this.angle > 190f || (this.angle < -10f && this.angle > -100f))
			{
				base.dir = 3;
			}
			else
			{
				base.dir = 1;
			}
			this.angle -= 45f;
		}
		else if (this.angle > 170f && this.angle < 235f)
		{
			base.dir = 0;
		}
		else if (this.angle > 80f && this.angle < 145f)
		{
			base.dir = 1;
		}
		else if (this.angle > -100f && this.angle < -35f)
		{
			base.dir = 3;
		}
		else if (this.angle > -10f && this.angle < 55f)
		{
			base.dir = 2;
		}
		this.renderer.RefreshSprite();
	}

	public void UpdateAngle()
	{
		if (this.IsPCC)
		{
			if (base.dir == 0)
			{
				this.angle = 225f;
				return;
			}
			if (base.dir == 1)
			{
				this.angle = 135f;
				return;
			}
			if (base.dir == 2)
			{
				this.angle = 45f;
				return;
			}
			if (base.dir == 3)
			{
				this.angle = -45f;
				return;
			}
		}
		else
		{
			if (base.dir == 0)
			{
				this.angle = 165f;
				return;
			}
			if (base.dir == 1)
			{
				this.angle = 300f;
				return;
			}
			if (base.dir == 2)
			{
				this.angle = 0f;
				return;
			}
			if (base.dir == 3)
			{
				this.angle = 120f;
			}
		}
	}

	public int GetCurrentDir()
	{
		Debug.Log(this.angle);
		if (this.renderer.hasActor)
		{
			return this.renderer.actor.currentDir;
		}
		if (this.angle == 0f || this.angle == 45f || this.angle == 90f)
		{
			return 2;
		}
		if (this.angle == -135f || this.angle == 180f || this.angle == -90f)
		{
			return 1;
		}
		if (this.angle == 135f)
		{
			return 0;
		}
		return 3;
	}

	public void UpdateSight()
	{
		int num = 4;
		for (int i = -num; i < num + 1; i++)
		{
			for (int j = -num; j < num + 1; j++)
			{
				Chara.shared.Set(this.pos.x + i, this.pos.z + j);
				if (!Chara.shared.IsValid || Chara.shared.cell.isSeen || i < -1 || i > 1 || j >= -1)
				{
				}
			}
		}
	}

	public bool WillConsumeTurn()
	{
		for (int i = this.conditions.Count - 1; i >= 0; i--)
		{
			if (this.conditions[i].ConsumeTurn)
			{
				return true;
			}
		}
		return false;
	}

	public void TickConditions()
	{
		if (this._cooldowns != null)
		{
			this.TickCooldown();
		}
		this.turn++;
		Chara.consumeTurn = false;
		Chara.preventRegen = false;
		this.emoIcon = Emo2.none;
		if (base.isSummon)
		{
			int c_summonDuration = base.c_summonDuration;
			base.c_summonDuration = c_summonDuration - 1;
			if (base.c_summonDuration <= 0)
			{
				this.Die(null, null, AttackSource.None);
				return;
			}
		}
		if (EClass.world.weather.IsRaining && !EClass._map.IsIndoor && !this.pos.cell.HasRoof)
		{
			this.AddCondition<ConWet>(20, false);
		}
		switch (this.turn % 50)
		{
		case 0:
			this.happiness = (this.hunger.value + this.stamina.value + this.depression.value + this.bladder.value + this.hygiene.value) / 5;
			break;
		case 1:
			if (!this.IsPC || !EClass.debug.godMode)
			{
				if (EClass.rnd(2) == 0)
				{
					this.sleepiness.Mod(1);
				}
				if (EClass.rnd(3) == 0)
				{
					this.hunger.Mod(1);
				}
				if (this.IsPC && (this.sleepiness.GetPhase() != 0 || this.stamina.GetPhase() <= 1))
				{
					Tutorial.Play("sleep");
				}
			}
			break;
		case 2:
			if (this.parasite != null)
			{
				base.ModExp(227, (EClass._zone.IsRegion ? 5 : 40) * 100 / Mathf.Max(100, 100 + (this.elements.Base(227) - this.parasite.LV) * 25));
			}
			if (this.ride != null)
			{
				base.ModExp(226, (EClass._zone.IsRegion ? 5 : 40) * 100 / Mathf.Max(100, 100 + (this.elements.Base(226) - this.ride.LV) * 25));
			}
			break;
		}
		if (this.turn % 500 == 0)
		{
			this.DiminishTempElements(1);
		}
		if (this.IsPCParty)
		{
			if (this.dirtyWeight)
			{
				this.CalcBurden();
			}
			int phase = this.burden.GetPhase();
			int phase2 = this.hunger.GetPhase();
			if (phase2 >= 4)
			{
				Chara.preventRegen = true;
			}
			if (EClass.rnd(EClass._zone.IsRegion ? 100 : 30) == 0 && phase >= 3)
			{
				base.Say("dmgBurden", this, null, null);
				base.DamageHP(this.MaxHP * (base.ChildrenWeight * 100 / this.WeightLimit) / 1000 + 1, AttackSource.Burden, null);
				if (this.isDead)
				{
					return;
				}
			}
			if (EClass.rnd(12) == 0)
			{
				if (this.IsPC)
				{
					if (phase > 0)
					{
						base.ModExp(207, 1 + phase * phase);
					}
				}
				else
				{
					base.ModExp(207, 4);
				}
			}
			if (this.IsPC)
			{
				if (phase2 >= 5 && !(this.ai is AI_Eat) && EClass.rnd(5) == 0)
				{
					base.DamageHP(1 + EClass.rnd(2) + this.MaxHP / 50, AttackSource.Hunger, null);
				}
				if (this.isDead)
				{
					return;
				}
				phase2 = this.stamina.GetPhase();
				if (phase2 <= 0)
				{
					Chara.preventRegen = true;
				}
				if (this.currentZone.IsRegion && EClass.world.weather.CurrentCondition == Weather.Condition.RainHeavy && !EClass.pc.HasElement(408, 1))
				{
					if (EClass.rnd(100) == 0 && !this.isConfused)
					{
						Msg.Say("rain_confuse");
						this.AddCondition<ConConfuse>(500, false);
					}
					if (EClass.rnd(300) == 0 && !this.isBlind)
					{
						Msg.Say("rain_confuse");
						this.AddCondition<ConBlind>(200, false);
					}
				}
				if (this.turn % (2000 * (100 + base.Evalue(412) * 2) / (100 + base.Evalue(409) * 10)) == 0)
				{
					this.ModCorruption(1);
				}
			}
		}
		if (!this.IsPC)
		{
			int num = base.Evalue(409);
			if (num > 0 && this.turn % 2000 * (100 + base.Evalue(412) * 2) / (100 + num * 10) == 0)
			{
				this.ModCorruption(1);
			}
		}
		for (int i = this.conditions.Count - 1; i >= 0; i--)
		{
			if (i < this.conditions.Count)
			{
				Condition condition = this.conditions[i];
				if (!condition.TimeBased)
				{
					condition.Tick();
				}
				if (!condition.IsKilled)
				{
					if (condition.ConsumeTurn)
					{
						Chara.consumeTurn = true;
					}
					if (condition.PreventRegen)
					{
						Chara.preventRegen = true;
					}
					if (condition.EmoIcon != Emo2.none && condition.EmoIcon > this.emoIcon)
					{
						this.emoIcon = condition.EmoIcon;
					}
				}
				if (this.isDead)
				{
					return;
				}
			}
		}
		if (!Chara.preventRegen)
		{
			if (EClass.rnd(25) == 0 && base.hp < this.MaxHP)
			{
				this.HealHP(EClass.rnd(base.Evalue(300) / 3 + 1) + 1, HealSource.None);
				this.elements.ModExp(300, 8, false);
			}
			if (EClass.rnd(8) == 0 && this.mana.value < this.mana.max)
			{
				this.mana.Mod(EClass.rnd(base.Evalue(301) / 2 + 1) + 1);
				this.elements.ModExp(301, 8, false);
			}
			if (EClass.rnd(20) == 0 && !this.IsPC && this.stamina.value < this.stamina.max)
			{
				this.stamina.Mod(EClass.rnd(5) + 1);
			}
		}
	}

	public void SyncRide()
	{
		if (this.host != null)
		{
			this.host.SyncRide();
		}
		if (this.ride != null)
		{
			this.SyncRide(this.ride);
		}
		if (this.parasite != null)
		{
			this.SyncRide(this.parasite);
		}
	}

	public void SyncRide(Chara c)
	{
		if (!c.pos.Equals(this.pos))
		{
			if (!this.pos.IsValid)
			{
				string str = "exception: pos is not valid:";
				Point pos = this.pos;
				Debug.LogError(str + ((pos != null) ? pos.ToString() : null) + "/" + ((this != null) ? this.ToString() : null));
				this.pos = new Point();
			}
			EClass._map.MoveCard(this.pos, c);
		}
	}

	public override void Tick()
	{
		Chara.<>c__DisplayClass337_0 CS$<>8__locals1 = new Chara.<>c__DisplayClass337_0();
		CS$<>8__locals1.<>4__this = this;
		this.SyncRide();
		this.combatCount--;
		if (this.IsPC)
		{
			if (this.hasMovedThisTurn)
			{
				this.pos.Things.ForeachReverse(delegate(Thing t)
				{
					t.trait.OnStepped(CS$<>8__locals1.<>4__this);
				});
				if (this.isDead)
				{
					return;
				}
				this.hasMovedThisTurn = false;
				if (EClass.player.haltMove)
				{
					EClass.player.haltMove = false;
					ActionMode.Adv.TryCancelInteraction(false);
					EInput.Consume(1);
					return;
				}
				if (EClass._zone.IsRegion)
				{
					if (EClass.game.config.preference.autoEat)
					{
						foreach (Chara chara in EClass.pc.party.members)
						{
							if (chara.hunger.value > 65)
							{
								chara.InstantEat(null, false);
							}
						}
					}
					Chara chara2 = null;
					EloMap.Cell cell = EClass.scene.elomap.GetCell(EClass.pc.pos);
					if (cell != null && (cell.zone == null || (cell.zone is Zone_Field && !cell.zone.IsPCFaction)))
					{
						foreach (Chara chara3 in EClass._map.charas)
						{
							if (!chara3.IsPCFactionOrMinion && !chara3.IsGlobal && chara3.pos.Equals(EClass.pc.pos))
							{
								chara2 = chara3;
								break;
							}
						}
					}
					if (chara2 != null)
					{
						if (!EClass.pc.HasCondition<ConDrawMonster>())
						{
							EClass.player.safeTravel = 5 + EClass.rnd(5);
						}
						EClass._zone.RemoveCard(chara2);
						Msg.Say("encounter");
						EClass.player.EnterLocalZone(true, chara2);
					}
					else if (EClass.player.safeTravel <= 0)
					{
						if (cell != null && cell.zone == null && !EClass.debug.ignoreEncounter)
						{
							EloMap.TileInfo tileInfo = EClass.scene.elomap.GetTileInfo(EClass.pc.pos.eloX, EClass.pc.pos.eloY);
							if (!tileInfo.shore)
							{
								bool flag = EClass.pc.HasCondition<ConWardMonster>();
								bool flag2 = EClass.pc.HasCondition<ConDrawMonster>();
								bool flag3 = EClass.game.quests.Get<QuestEscort>() != null;
								int num = tileInfo.isRoad ? 22 : 12;
								if (flag3)
								{
									num = (tileInfo.isRoad ? 16 : 10);
								}
								if (flag)
								{
									num *= (flag3 ? 2 : 20);
								}
								if (flag2)
								{
									num /= 2;
								}
								if (EClass.rnd(num) == 0)
								{
									Msg.Say("encounter");
									if (!flag2)
									{
										EClass.player.safeTravel = 5 + EClass.rnd(5);
									}
									EClass.player.EnterLocalZone(true, null);
								}
							}
						}
					}
					else
					{
						EClass.player.safeTravel--;
					}
				}
			}
			EClass.player.pickupDelay = 0f;
			if (EClass.player.returnInfo != null)
			{
				EClass.player.returnInfo.turns--;
				if (EClass.player.returnInfo.turns <= 0)
				{
					if (EClass.pc.burden.GetPhase() != 4 || EClass.debug.ignoreWeight)
					{
						int uidDest = EClass.player.returnInfo.uidDest;
						Zone zone = null;
						if (uidDest != 0)
						{
							zone = (EClass.game.spatials.map.TryGetValue(uidDest, null) as Zone);
						}
						if (zone == null || zone.destryoed)
						{
							zone = EClass.world.region;
						}
						if (zone == EClass.game.activeZone || EClass.game.activeZone.IsRegion)
						{
							Msg.Say("returnFail");
						}
						else
						{
							Msg.Say("returnComplete");
							EClass.player.uidLastTravelZone = 0;
							EClass.pc.MoveZone(zone, ZoneTransition.EnterState.Return);
							EClass.player.lastZonePos = null;
						}
						EClass.player.returnInfo = null;
						return;
					}
					EClass.player.returnInfo = null;
					Msg.Say("returnOverweight");
				}
			}
			if ((this.HasNoGoal || !this.ai.IsRunning) && !this.WillConsumeTurn())
			{
				this.SetAI(Chara._NoGoalPC);
				return;
			}
			EClass.player.stats.turns++;
			if (EClass.core.config.game.alwaysUpdateRecipe)
			{
				RecipeUpdater.dirty = true;
			}
			this.actTime = EClass.player.baseActTime;
		}
		else
		{
			this.actTime = EClass.player.baseActTime * Mathf.Max(0.1f, (float)EClass.pc.Speed / (float)this.Speed);
			this.hasMovedThisTurn = false;
		}
		this.TickConditions();
		if (!this.IsAliveInCurrentZone)
		{
			return;
		}
		this.renderer.RefreshStateIcon();
		if (this.host != null && !Chara.consumeTurn)
		{
			if (this.host.ride == this && ((this.host.hasMovedThisTurn && this.IsInCombat) || (this.enemy != null && this.turn % 3 != 0)))
			{
				Chara.consumeTurn = true;
			}
			if (this.host.parasite == this && this.enemy != null && EClass.rnd(10) > EClass.rnd(this.host.Evalue(227) + 10))
			{
				if (base.Dist(this.enemy) < 3 && EClass.rnd(2) == 0)
				{
					base.Say("parasite_fail", this, this.host, null, null);
					if (EClass.rnd(2) == 0 && base.GetInt(106, null) == 0)
					{
						base.Talk("parasite_fail", null, null, false);
					}
				}
				Chara.consumeTurn = true;
			}
		}
		if (Chara.consumeTurn)
		{
			if (this.IsPC)
			{
				ActionMode.Adv.SetTurbo(-1);
			}
		}
		else
		{
			if (base.isRestrained)
			{
				base.TryUnrestrain(false, null);
			}
			if (this.enemy != null)
			{
				if (!this.enemy.IsAliveInCurrentZone || EClass._zone.IsRegion)
				{
					this.enemy = null;
				}
				else if (!this.IsPC && ((!(this.ai is GoalCombat) && !(this.ai is AI_Trolley)) || !this.ai.IsRunning))
				{
					this.SetAIAggro();
				}
			}
			if (this.HasNoGoal || !this.ai.IsRunning)
			{
				this.ChooseNewGoal();
			}
			this.ai.Tick();
		}
		CS$<>8__locals1.cell = base.Cell;
		if (CS$<>8__locals1.cell.IsTopWaterAndNoSnow && !CS$<>8__locals1.cell.isFloating)
		{
			this.AddCondition<ConWet>(50, false);
		}
		if (this.IsPC && !EClass._zone.IsRegion && CS$<>8__locals1.cell.CanSuffocate())
		{
			this.AddCondition<ConSuffocation>(800 / (100 + base.Evalue(200) * 10), false);
		}
		if (CS$<>8__locals1.cell.effect != null)
		{
			Chara.<>c__DisplayClass337_1 CS$<>8__locals2;
			CS$<>8__locals2.e = CS$<>8__locals1.cell.effect;
			switch (CS$<>8__locals2.e.id)
			{
			case 1:
			case 2:
			case 4:
				if (this.IsLevitating)
				{
					base.Say("levitating", null, null);
				}
				else
				{
					this.AddCondition<ConWet>(50, false);
					CS$<>8__locals1.<Tick>g__ProcEffect|2(ref CS$<>8__locals2);
				}
				break;
			case 3:
				base.PlaySound("fire_step", 1f, true);
				this.AddCondition<ConBurning>(30, false);
				break;
			case 5:
				if (!this.isWet)
				{
					base.PlaySound("bubble", 1f, true);
					this.AddCondition<ConWet>(30, false);
					CS$<>8__locals1.<Tick>g__ProcEffect|2(ref CS$<>8__locals2);
				}
				break;
			case 6:
				if (this.hasMovedThisTurn)
				{
					base.Say("abMistOfDarkness_step", this, null, null);
				}
				break;
			}
		}
		if (this.IsPC)
		{
			if (EClass.player.currentHotItem.Thing != null)
			{
				EClass.player.currentHotItem.Thing.trait.OnTickHeld();
			}
			EClass.screen.OnEndPlayerTurn();
		}
	}

	public bool CanLift(Card c)
	{
		return true;
	}

	public bool CanAutoPick(Card c)
	{
		return true;
	}

	public bool CanPick(Card c)
	{
		if (c.isDestroyed)
		{
			return false;
		}
		Card rootCard = c.GetRootCard();
		return !rootCard.isDestroyed && (!rootCard.ExistsOnMap || rootCard.pos.Distance(this.pos) <= 1) && (rootCard == this || !this.things.IsFull(c.Thing, true, true));
	}

	public void PickOrDrop(Point p, string idThing, int idMat = -1, int num = 1, bool msg = true)
	{
		if (num == 0)
		{
			return;
		}
		this.PickOrDrop(p, ThingGen.Create(idThing, idMat, -1).SetNum(num), msg);
	}

	public void PickOrDrop(Point p, Thing t, bool msg = true)
	{
		if (this.things.GetDest(t, true).IsValid)
		{
			this.Pick(t, msg, true);
			return;
		}
		EClass._zone.AddCard(t, p);
	}

	public Thing Pick(Thing t, bool msg = true, bool tryStack = true)
	{
		if (t.trait is TraitCard && t.isNew && EClass.game.config.autoCollectCard)
		{
			ContentCodex.Collect(t);
			return t;
		}
		if (t.parent == this)
		{
			return t;
		}
		ThingContainer.DestData dest = this.things.GetDest(t, tryStack);
		if (!dest.IsValid)
		{
			if (t.parent != EClass._zone)
			{
				if (this.IsPC)
				{
					base.Say("backpack_full_drop", t, null, null);
					SE.Drop();
				}
				return EClass._zone.AddCard(t, this.pos).Thing;
			}
			if (this.IsPC)
			{
				base.Say("backpack_full", t, null, null);
			}
			return t;
		}
		else
		{
			if (dest.stack != null)
			{
				if (msg)
				{
					base.PlaySound("pick_thing", 1f, true);
					base.Say("pick_thing", this, t, null, null);
				}
				t.TryStackTo(dest.stack);
				return dest.stack;
			}
			this.TryAbsorbRod(t);
			if (t.trait is TraitPotion && t.id != "1165" && !t.source.tag.Contains("neg") && EClass.rnd(2) == 0 && base.HasElement(1565, 1))
			{
				string id = (from a in EClass.sources.things.rows
				where a._origin == "potion" && a.tag.Contains("neg")
				select a).ToList<SourceThing.Row>().RandomItemWeighted((SourceThing.Row a) => (float)a.chance).id;
				base.Say("poisonDrip", this, null, null);
				int num = t.Num;
				t.Destroy();
				t = ThingGen.Create(id, -1, -1).SetNum(num);
			}
			if (msg)
			{
				base.PlaySound("pick_thing", 1f, true);
				base.Say("pick_thing", this, t, null, null);
			}
			this.TryReservePickupTutorial(t);
			return dest.container.AddThing(t, tryStack, -1, -1);
		}
	}

	public void TryAbsorbRod(Thing t)
	{
		if (!this.IsPC)
		{
			return;
		}
		if (t.trait is TraitRod && t.c_charges > 0 && base.HasElement(1564, 1))
		{
			base.Say("absorbRod", this, t, null, null);
			TraitRod rod = t.trait as TraitRod;
			bool flag = false;
			if (rod.source != null)
			{
				IEnumerable<SourceElement.Row> rows = EClass.sources.elements.rows;
				Func<SourceElement.Row, bool> predicate;
				Func<SourceElement.Row, bool> <>9__0;
				if ((predicate = <>9__0) == null)
				{
					predicate = (<>9__0 = ((SourceElement.Row a) => a.id == rod.source.id));
				}
				using (IEnumerator<SourceElement.Row> enumerator = rows.Where(predicate).GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						SourceElement.Row row = enumerator.Current;
						if (this.IsPC)
						{
							this.GainAbility(row.id, t.c_charges * 100);
							flag = true;
						}
					}
				}
			}
			if (!flag)
			{
				this.mana.Mod(-50 * t.c_charges);
			}
			t.c_charges = 0;
			LayerInventory.SetDirty(t);
		}
	}

	public void TryReservePickupTutorial(Thing t)
	{
		if (t.id == "axe")
		{
			Tutorial.Reserve("tool", null);
		}
		if (t.category.id == "mushroom")
		{
			Tutorial.Reserve("food", null);
		}
		if (t.category.id == "herb")
		{
			Tutorial.Reserve("herb", null);
		}
		if (t.id == "pasture")
		{
			Tutorial.Reserve("pasture", null);
		}
		if (t.id == "log")
		{
			Tutorial.Reserve("process", null);
		}
	}

	public void TryPickGroundItem()
	{
		foreach (Card card in this.pos.ListCards(false))
		{
			if (!this.IsPC)
			{
				AI_UseCrafter ai_UseCrafter = EClass.pc.ai as AI_UseCrafter;
				if (ai_UseCrafter != null && ai_UseCrafter.ings.Contains(card))
				{
					continue;
				}
			}
			if (card.isThing && card.placeState == PlaceState.roaming && this.CanPick(card))
			{
				Thing thing = this.Pick(card.Thing, true, true);
				if (thing != null && !this.IsPC)
				{
					thing.isNPCProperty = true;
				}
				break;
			}
		}
	}

	public void TryPutShared(Thing t, List<Thing> containers = null, bool dropIfFail = true)
	{
		if (EClass._zone.TryAddThingInSharedContainer(t, containers, true, false, null, true))
		{
			return;
		}
		if (dropIfFail)
		{
			EClass._zone.AddCard(t, this.pos);
		}
	}

	public bool TryHoldCard(Card t, int num = -1, bool pickHeld = false)
	{
		if (this.held == t)
		{
			return true;
		}
		if (t.isDestroyed || t.Num <= 0)
		{
			return false;
		}
		if (!this.CanPick(t))
		{
			if (t.parent == null)
			{
				EClass._zone.AddCard(t, this.pos);
			}
			return false;
		}
		this.HoldCard(t, num);
		return true;
	}

	public void HoldCard(Card t, int num = -1)
	{
		if (this.held == t || t.isDestroyed || t.Num <= 0)
		{
			return;
		}
		if (this.IsPC && t.isNPCProperty)
		{
			t.isNPCProperty = false;
			EClass.player.ModKarma(-1);
			this.pos.TryWitnessCrime(this, null, 4, null);
		}
		this.PickHeld(false);
		if (t.isChara)
		{
			if (t.IsAliveInCurrentZone)
			{
				t.ShowEmo(Emo.love, 0f, true);
			}
			EClass.player.altHeldPos = (t.renderer.data.ForceAltHeldPosition || EClass.rnd(2) == 0);
		}
		else
		{
			if (num == -1 || num > t.Num)
			{
				num = t.Num;
			}
			if (num < t.Num)
			{
				t = t.Split(num);
			}
		}
		if (t.GetRootCard() != this)
		{
			t = this.Pick(t.Thing, false, true);
			if (t.GetRootCard() != this)
			{
				return;
			}
		}
		this.held = t;
		if (this.held.GetLightRadius() > 0)
		{
			base.RecalculateFOV();
		}
		if (this.IsPC)
		{
			LayerInventory.SetDirty(t.Thing);
			WidgetHotbar.dirtyCurrentItem = true;
		}
	}

	public void PickHeld(bool msg = false)
	{
		if (this.held == null)
		{
			return;
		}
		Card card = this.held;
		if (this.IsPC && this.held.invY == 1)
		{
			WidgetHotbar.dirtyCurrentItem = true;
			LayerInventory.SetDirty(this.held.Thing);
			this.held = null;
			return;
		}
		if (this.held.isChara)
		{
			this.DropHeld(null);
			return;
		}
		if (this.IsPC && !this.held.IsHotItem && this.held.trait.CanOnlyCarry)
		{
			base.Say("canOnlyCarry", this.held, null, null);
			this.DropHeld(null);
			return;
		}
		bool flag = this.held != this.things.TryStack(this.held.Thing, -1, -1);
		if (!flag && this.things.IsOverflowing())
		{
			if (this.IsPC)
			{
				base.Say("backpack_full_drop", this.held, null, null);
				SE.Drop();
			}
			this.DropHeld(null);
			return;
		}
		if (msg)
		{
			base.PlaySound("pick_thing", 1f, true);
			base.Say("pick_held", this, card, null, null);
			if (this.IsPC && card.id == "statue_weird")
			{
				base.Say("statue_pick", null, null);
			}
		}
		if (this.IsPC)
		{
			WidgetHotbar.dirtyCurrentItem = true;
			if (!flag)
			{
				LayerInventory.SetDirty(this.held.Thing);
				if (this.held.GetRootCard() != EClass.pc)
				{
					this.Pick(this.held.Thing, false, true);
				}
			}
		}
		this.held = null;
	}

	public Card SplitHeld(int a)
	{
		return this.held.Split(a);
	}

	public Card DropHeld(Point dropPos = null)
	{
		if (this.held == null)
		{
			return null;
		}
		if (this.IsPC)
		{
			WidgetHotbar.dirtyCurrentItem = true;
			LayerInventory.SetDirty(this.held.Thing);
		}
		Card card = EClass._zone.AddCard(this.held, dropPos ?? this.pos);
		card.OnLand();
		if (card.trait.CanOnlyCarry)
		{
			card.SetPlaceState(PlaceState.installed, false);
		}
		return card;
	}

	public void DropThing(Thing t, int num = -1)
	{
		if (t.c_isImportant)
		{
			Msg.Say("markedImportant");
			return;
		}
		if (!t.trait.CanBeDropped)
		{
			Msg.Say("cantDrop", t, null, null, null);
			return;
		}
		if (t.trait is TraitAbility)
		{
			SE.Trash();
			t.Destroy();
			return;
		}
		Msg.Say("dropItem", t.Name, null, null, null);
		t.ignoreAutoPick = true;
		base.PlaySound("drop", 1f, true);
		EClass._zone.AddCard(t, this.pos);
	}

	public AttackStyle GetFavAttackStyle()
	{
		int num = base.Evalue(131);
		int num2 = base.Evalue(130);
		int num3 = base.Evalue(123);
		if (num > num2 && num > num3)
		{
			return AttackStyle.TwoWield;
		}
		if (num2 > num && num2 > num3)
		{
			return AttackStyle.TwoHand;
		}
		if (num3 > num && num3 > num2)
		{
			return AttackStyle.Shield;
		}
		return AttackStyle.Default;
	}

	public Element GetFavWeaponSkill()
	{
		return this.elements.ListElements((Element e) => e.source.categorySub == "weapon" && !e.HasTag("ranged"), null).FindMax((Element a) => a.Value);
	}

	public Element GetFavArmorSkill()
	{
		if (this.elements.Value(122) > this.elements.Value(120))
		{
			return this.elements.GetElement(122);
		}
		return this.elements.GetElement(120);
	}

	public void TryRestock(bool onCreate)
	{
		Chara.isOnCreate = onCreate;
		if (onCreate || (!this.IsPCFaction && (base.IsUnique || this.trait is TraitAdventurer || this.trait is TraitGuard)))
		{
			this.RestockEquip((!EClass.core.IsGameStarted || !(EClass._zone is Zone_Music)) && onCreate);
		}
		this.RestockInventory(onCreate);
	}

	public void RestockEquip(bool onCreate)
	{
		string equip = this.source.equip;
		if (equip.IsEmpty())
		{
			equip = this.job.equip;
		}
		string id = this.id;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(id);
		if (num <= 1337759161U)
		{
			if (num <= 646060618U)
			{
				if (num != 511800731U)
				{
					if (num != 613794712U)
					{
						if (num != 646060618U)
						{
							goto IL_490;
						}
						if (!(id == "seeker"))
						{
							goto IL_490;
						}
						if (onCreate)
						{
							this.EQ_ID("helm_seeker", -1, Rarity.Random);
						}
						this.EQ_ID("robe_pope", -1, Rarity.Random);
						this.EQ_ID("sword_katana", -1, Rarity.Random);
						this.EQ_ID("staff", -1, Rarity.Random);
						this.EQ_ID("sword_katana", -1, Rarity.Random);
						if (onCreate)
						{
							this.EQ_ID("boots_seven", -1, Rarity.Random);
						}
						if (onCreate)
						{
							for (int i = 0; i < 20; i++)
							{
								base.AddThing(ThingGen.CreateFromCategory("book", 50), true, -1, -1);
							}
							this.EQ_Item("panty", 1);
							base.AddThing("plat", -1).SetNum(6);
							goto IL_490;
						}
						goto IL_490;
					}
					else
					{
						if (!(id == "adv_ivory"))
						{
							goto IL_490;
						}
						this.EQ_ID("dagger", -1, Rarity.Legendary);
						base.AddThing("60", -1);
						goto IL_490;
					}
				}
				else
				{
					if (!(id == "adv_gaki"))
					{
						goto IL_490;
					}
					if (onCreate)
					{
						this.EQ_ID("dagger_gaki", -1, Rarity.Random);
					}
					if (onCreate)
					{
						this.EQ_ID("dagger_ninto", -1, Rarity.Random);
						goto IL_490;
					}
					goto IL_490;
				}
			}
			else if (num <= 1184812370U)
			{
				if (num != 1052270088U)
				{
					if (num != 1184812370U)
					{
						goto IL_490;
					}
					if (!(id == "ashland"))
					{
						goto IL_490;
					}
					if (onCreate)
					{
						base.AddThing("guitar_ash", -1);
						goto IL_490;
					}
					goto IL_490;
				}
				else
				{
					if (!(id == "swordkeeper"))
					{
						goto IL_490;
					}
					if (onCreate)
					{
						this.EQ_ID("EtherDagger", -1, Rarity.Random);
						goto IL_490;
					}
					goto IL_490;
				}
			}
			else if (num != 1188679840U)
			{
				if (num != 1337759161U)
				{
					goto IL_490;
				}
				if (!(id == "adv_verna"))
				{
					goto IL_490;
				}
				if (onCreate)
				{
					this.EQ_ID("staff_long", -1, Rarity.Legendary);
				}
				if (onCreate)
				{
					this.EQ_ID("cloak_wing", -1, Rarity.Mythical);
					goto IL_490;
				}
				goto IL_490;
			}
			else if (!(id == "loytel"))
			{
				goto IL_490;
			}
		}
		else if (num <= 2936881100U)
		{
			if (num <= 1491226603U)
			{
				if (num != 1488976522U)
				{
					if (num != 1491226603U)
					{
						goto IL_490;
					}
					if (!(id == "adv_mesherada"))
					{
						goto IL_490;
					}
					if (onCreate)
					{
						this.EQ_ID("dagger_hathaway", -1, Rarity.Random);
						goto IL_490;
					}
					goto IL_490;
				}
				else if (!(id == "quru"))
				{
					goto IL_490;
				}
			}
			else if (num != 1808495106U)
			{
				if (num != 2936881100U)
				{
					goto IL_490;
				}
				if (!(id == "shojo"))
				{
					goto IL_490;
				}
			}
			else if (!(id == "kettle"))
			{
				goto IL_490;
			}
		}
		else if (num <= 3538562634U)
		{
			if (num != 3099445666U)
			{
				if (num != 3538562634U)
				{
					goto IL_490;
				}
				if (!(id == "adv_wini"))
				{
					goto IL_490;
				}
				if (onCreate)
				{
					this.EQ_ID("staff_Cat", -1, Rarity.Mythical);
				}
				base.AddCard(ThingGen.CreateSpellbook(9150, 1));
				if (onCreate)
				{
					base.AddThing("1071", -1);
					goto IL_490;
				}
				goto IL_490;
			}
			else
			{
				if (!(id == "big_sister"))
				{
					goto IL_490;
				}
				if (onCreate)
				{
					this.EQ_ID("sword_muramasa", -1, Rarity.Random);
					goto IL_490;
				}
				goto IL_490;
			}
		}
		else if (num != 3590419257U)
		{
			if (num != 4273125121U)
			{
				goto IL_490;
			}
			if (!(id == "adv_kiria"))
			{
				goto IL_490;
			}
			if (onCreate)
			{
				this.EQ_ID("sword_zephir", -1, Rarity.Random);
				goto IL_490;
			}
			goto IL_490;
		}
		else
		{
			if (!(id == "ephrond"))
			{
				goto IL_490;
			}
			if (onCreate)
			{
				base.AddThing("guitar_efrond", -1);
				goto IL_490;
			}
			goto IL_490;
		}
		this.EQ_ID("staff_long", 1, Rarity.Random);
		this.EQ_CAT("head");
		this.EQ_CAT("torso");
		this.EQ_CAT("arm");
		return;
		IL_490:
		if (!(equip == "archer"))
		{
			if (!(equip == "inquisitor") && !(equip == "gunner"))
			{
				if (!(equip == "predator") && !(equip == "none"))
				{
				}
			}
			else if (onCreate || !this.TryEquipRanged())
			{
				this.EQ_CAT("gun");
			}
		}
		else if (onCreate || !this.TryEquipRanged())
		{
			this.EQ_CAT((EClass.rnd(4) == 0) ? "crossbow" : "bow");
		}
		int num2 = (base.rarity >= Rarity.Mythical) ? (base.LV * 3) : ((base.rarity >= Rarity.Legendary) ? (base.LV * 2) : base.LV);
		if (this.trait is TraitAdventurer)
		{
			num2 *= 3;
		}
		if (this.race.id == "asura")
		{
			for (int j = 0; j < 4; j++)
			{
				this.EQ_CAT(this.job.weapon.RandomItem<string>());
			}
		}
		for (int k = 0; k < ((this.race.id == "mutant") ? (2 + base.LV / 30) : 1); k++)
		{
			if (!this.job.weapon.IsEmpty())
			{
				if (this.race.id == "mutant" || (this.body.slotMainHand != null && this.body.slotMainHand.thing == null))
				{
					this.EQ_CAT(this.job.weapon.RandomItem<string>());
				}
				if (this.race.id == "mutant" || (base.Evalue(131) > 0 && EClass.rnd(2) == 0))
				{
					this.EQ_CAT(this.job.weapon.RandomItem<string>());
				}
			}
			this.EQ_CAT("torso");
			if (EClass.rnd(num2) > 5)
			{
				this.EQ_CAT("arm");
			}
			if (EClass.rnd(num2) > 10)
			{
				this.EQ_CAT("head");
			}
			if (EClass.rnd(num2) > 15)
			{
				this.EQ_CAT("back");
			}
			if (EClass.rnd(num2) > 20)
			{
				this.EQ_CAT("ring");
			}
			if (EClass.rnd(num2) > 25)
			{
				this.EQ_CAT("amulet");
			}
			if (EClass.rnd(num2) > 30)
			{
				this.EQ_CAT("foot");
			}
			if (EClass.rnd(num2) > 35)
			{
				this.EQ_CAT("waist");
			}
			if (EClass.rnd(num2) > 40)
			{
				this.EQ_CAT("ring");
			}
		}
		if (this.trait is TraitBard)
		{
			base.AddThing(ThingGen.Create("lute", -1, -1), true, -1, -1);
		}
	}

	public void RestockInventory(bool onCreate)
	{
		string id = this.id;
		if (id == "fiama")
		{
			this.<RestockInventory>g__Restock|360_0("book_story", 1);
			return;
		}
		if (id == "rock_thrower")
		{
			this.<RestockInventory>g__Restock|360_0("stone", 10 + EClass.rnd(10));
			return;
		}
		if (!(id == "giant"))
		{
			if (!(id == "begger"))
			{
				if (!(id == "farris"))
				{
					return;
				}
				this.<RestockInventory>g__Restock|360_0("lute", 1);
			}
			return;
		}
		this.<RestockInventory>g__Restock|360_0("rock", 2 + EClass.rnd(10));
	}

	private void SetEQQuality()
	{
		CardBlueprint.Set(CardBlueprint.CharaGenEQ);
		Rarity rarity = Rarity.Normal;
		int num = (base.LV >= 1000) ? 7 : ((base.LV >= 500) ? 5 : ((base.LV >= 250) ? 3 : ((base.LV >= 100) ? 2 : ((base.LV >= 50) ? 1 : 0))));
		Rarity rarity2 = base.rarity;
		if (this.id == "big_sister")
		{
			num = (Chara.isOnCreate ? 8 : 4);
		}
		if (!Chara.isOnCreate && EClass.rnd(10) != 0)
		{
			num /= 2;
		}
		if (rarity2 == Rarity.Superior && EClass.rnd(10) <= num)
		{
			rarity = Rarity.Superior;
		}
		else if (rarity2 == Rarity.Legendary)
		{
			rarity = ((EClass.rnd(10) <= num) ? Rarity.Legendary : ((EClass.rnd(5) <= num) ? Rarity.Superior : Rarity.Normal));
		}
		else if (rarity2 >= Rarity.Mythical)
		{
			rarity = ((EClass.rnd(30) <= num) ? Rarity.Mythical : ((EClass.rnd(10) <= num) ? Rarity.Legendary : Rarity.Superior));
		}
		if (rarity == Rarity.Normal && EClass.rnd(1000) == 0)
		{
			rarity = Rarity.Legendary;
		}
		CardBlueprint.current.rarity = rarity;
	}

	public Thing EQ_ID(string s, int mat = -1, Rarity r = Rarity.Random)
	{
		this.SetEQQuality();
		if (r != Rarity.Random)
		{
			CardBlueprint.current.rarity = r;
		}
		Thing thing = ThingGen.Create(s, mat, base.LV);
		if (this.body.GetSlot(thing, true, false) == null)
		{
			return thing;
		}
		base.AddThing(thing, true, -1, -1);
		if (!this.body.Equip(thing, null, true))
		{
			thing.Destroy();
		}
		return thing;
	}

	public void EQ_CAT(string s)
	{
		int slot = EClass.sources.categories.map[s].slot;
		BodySlot bodySlot = (slot == 0) ? null : this.body.GetSlot(slot, true, false);
		if (slot != 0 && bodySlot == null)
		{
			return;
		}
		if (slot == 37 && base.HasElement(1209, 1))
		{
			return;
		}
		this.SetEQQuality();
		Thing thing = ThingGen.CreateFromCategory(s, base.LV);
		base.AddThing(thing, true, -1, -1);
		if (bodySlot != null && !this.body.Equip(thing, bodySlot, true))
		{
			thing.Destroy();
		}
	}

	public void EQ_Item(string s, int num = 1)
	{
		this.SetEQQuality();
		Thing t = ThingGen.Create(s, -1, base.LV).SetNum(num);
		base.AddThing(t, true, -1, -1);
	}

	public void Drink(Card t)
	{
		base.Say("drink", this, t.Duplicate(1), null, null);
		base.Say("quaff", null, null);
		base.PlaySound("drink", 1f, true);
		this.hunger.Mod(-2);
		t.ModNum(-1, true);
		t.trait.OnDrink(this);
		bool isPC = this.IsPC;
	}

	public void GetRevived()
	{
		this.Revive(EClass.pc.pos.GetNearestPoint(false, false, true, false), true);
		if (!this.IsPCFaction)
		{
			return;
		}
		if (!this.IsPC && !this.trait.CanJoinPartyResident)
		{
			if (this.homeZone != null && EClass._zone != this.homeZone)
			{
				Msg.Say("returnHome", this, this.homeZone.Name, null, null);
				this.MoveZone(this.homeZone, ZoneTransition.EnterState.Auto);
			}
			return;
		}
		if (!EClass._zone.IsPCFaction || this.homeBranch != EClass.Branch || base.GetInt(103, null) != 0)
		{
			EClass.pc.party.AddMemeber(this);
		}
	}

	public void Revive(Point p = null, bool msg = false)
	{
		if (!this.isDead)
		{
			return;
		}
		this.isDead = false;
		base.hp = this.MaxHP / 3;
		this.mana.value = 0;
		this.stamina.value = 0;
		this.hunger.value = 30;
		this.sleepiness.value = 0;
		this.hostility = this.OriginalHostility;
		if (this.IsPC)
		{
			if (EClass.player.preventDeathPenalty)
			{
				Msg.Say("noDeathPenalty2", this, null, null, null);
			}
			else if (EClass.player.stats.days <= 90 && !EClass.debug.enable)
			{
				Msg.Say("noDeathPenalty", this, null, null, null);
			}
			else
			{
				EClass.pc.ApplyDeathPenalty();
			}
			List<Thing> dropList = new List<Thing>();
			EClass.pc.things.Foreach(delegate(Thing t)
			{
				if (!t.IsContainer && t.SelfWeight > EClass.pc.WeightLimit && t.trait.CanBeDropped && (t.parentCard.c_lockLv == 0 || t.parentCard.trait is TraitChestPractice))
				{
					t.ignoreAutoPick = true;
					dropList.Add(t);
				}
			}, false);
			foreach (Thing thing in dropList)
			{
				EClass._zone.AddCard(thing, EClass.pc.pos);
				Msg.Say("backpack_full_drop", thing, null, null, null);
			}
			EClass.player.preventDeathPenalty = false;
		}
		if (this.IsPCFaction && this.homeBranch != null)
		{
			this.homeBranch.Log("bRevive", this, null, null, null);
		}
		if (p == null)
		{
			return;
		}
		if (!p.IsInBounds)
		{
			p.Set(EClass._map.GetCenterPos().GetNearestPoint(false, true, true, false) ?? EClass._map.GetCenterPos());
		}
		EClass._zone.AddCard(this, p);
		if (msg)
		{
			SE.Play("revive");
			Msg.Say("revive", this, null, null, null);
			base.PlayEffect("revive", true, 0f, default(Vector3));
		}
	}

	public void MakeGrave(string lastword)
	{
		if (EClass._zone.IsRegion)
		{
			return;
		}
		List<string> list = new List<string>
		{
			"930",
			"931",
			"947",
			"948",
			"949",
			"950",
			"951",
			"952"
		};
		if (this.IsPC && EClass.rnd(2) == 0 && EClass.pc.things.Find("letter_will", -1, -1) != null)
		{
			list = new List<string>
			{
				"944",
				"946",
				"backerGrave",
				"backerGrave2"
			};
		}
		Thing thing = ThingGen.Create(list.RandomItem<string>(), -1, -1);
		thing.MakeRefFrom(this, null);
		if (!lastword.IsEmpty())
		{
			thing.c_note = lastword;
		}
		thing.isModified = true;
		EClass._zone.AddCard(thing, this.pos).Install();
	}

	public void ApplyDeathPenalty()
	{
		if (this.IsPC)
		{
			int currency = base.GetCurrency("money");
			if (currency > 0)
			{
				int num = currency / 3 + EClass.rnd(currency / 3 + 1);
				if (num <= 0)
				{
					num = 1;
				}
				Msg.Say("panaltyMoney", this, Lang._currency(num, false, 14), null, null);
				base.ModCurrency(-num, "money");
				EClass._zone.AddCard(ThingGen.CreateCurrency(num, "money"), EClass.pc.pos);
				foreach (Element element in EClass.pc.elements.dict.Values)
				{
					if (EClass.rnd(5) == 0 && element.IsMainAttribute)
					{
						EClass.pc.elements.ModExp(element.id, -500, false);
					}
				}
			}
		}
	}

	public void Vomit()
	{
		int c_vomit = base.c_vomit;
		base.c_vomit = c_vomit + 1;
		base.Say("vomit", this, null, null);
		base.PlaySound("vomit", 1f, true);
		if (!EClass._zone.IsRegion)
		{
			Thing thing = ThingGen.Create("731", -1, -1);
			if (!EClass._zone.IsPCFaction)
			{
				thing.MakeRefFrom(this, null);
			}
			EClass._zone.AddCard(thing, this.pos);
		}
		if (this.HasCondition<ConAnorexia>())
		{
			base.ModExp(70, -50);
			base.ModExp(71, -75);
			base.ModExp(77, -100);
		}
		else if (base.c_vomit > 10)
		{
			this.AddCondition<ConAnorexia>(100, false);
		}
		this.AddCondition<ConDim>(100, false);
		if (this.HasCondition<ConAnorexia>())
		{
			this.ModWeight(-1 * (1 + EClass.rnd(5)), false);
		}
		if (this.hunger.GetPhase() >= 4)
		{
			base.DamageHP(9999, AttackSource.Hunger, null);
		}
		this.hunger.Mod(30);
	}

	public override void Die(Element e = null, Card origin = null, AttackSource attackSource = AttackSource.None)
	{
		this.combatCount = 0;
		if (this.isDead || this.host != null)
		{
			return;
		}
		bool isInActiveZone = this.IsInActiveZone;
		if (isInActiveZone)
		{
			if (this.IsPC)
			{
				EClass._zone.ResetHostility();
			}
			if (base.isSummon)
			{
				base.Say("summon_vanish", this, null, null);
				this.pos.PlayEffect("vanish");
				this.pos.PlaySound("vanish", true, 1f, true);
				base.Destroy();
				return;
			}
			if (attackSource == AttackSource.DeathSentense)
			{
				if (this.trait is TraitLittleOne)
				{
					this.MakeEgg(true, 1, true);
				}
				base.PlayEffect("revive", true, 0f, default(Vector3));
				base.PlaySound("chime_angel", 1f, true);
			}
			else
			{
				Effect.Get("blood").Play((this.parent is Chara) ? (this.parent as Chara).pos : this.pos, 0f, null, null).SetParticleColor(EClass.Colors.matColors[base.material.alias].main).Emit(50);
				base.AddBlood(2 + EClass.rnd(2), -1);
				base.PlaySound(base.material.GetSoundDead(this.source), 1f, true);
			}
			this.renderer.RefreshSprite();
			this.renderer.RefreshStateIcon();
			base.ClearFOV();
		}
		string text = "";
		"dead_in".langGame(EClass._zone.Name, null, null, null);
		string text2 = (origin == null) ? "" : origin.GetName(NameStyle.Full, -1);
		if (LangGame.Has("dead_" + attackSource.ToString()))
		{
			text = "dead_" + attackSource.ToString();
		}
		else
		{
			if (e != Element.Void && e != null)
			{
				text = "dead_" + e.source.alias;
			}
			if (text == "" || !LangGame.Has(text))
			{
				text = "dead";
			}
		}
		if (this.IsPC)
		{
			EClass._zone.isDeathLocation = true;
			string s = (origin == null) ? text : "dead_by";
			Msg.thirdPerson1.Set(EClass.pc, false);
			if (attackSource == AttackSource.Wrath)
			{
				text2 = Religion.recentWrath.NameShort;
			}
			EClass.player.deathMsg = GameLang.Parse(s.langGame(), true, EClass.pc.NameBraced, "dead_in".langGame(EClass._zone.Name, null, null, null), text2, null);
			Debug.Log(EClass.player.deathMsg);
			ZoneInstanceRandomQuest zoneInstanceRandomQuest = EClass._zone.instance as ZoneInstanceRandomQuest;
			if (zoneInstanceRandomQuest != null)
			{
				zoneInstanceRandomQuest.status = ZoneInstance.Status.Fail;
			}
			AI_PlayMusic.keepPlaying = false;
		}
		if (isInActiveZone)
		{
			if (attackSource == AttackSource.DeathSentense)
			{
				Msg.Say("goto_heaven", this, null, null, null);
			}
			else
			{
				if (origin == null || !origin.isSynced || (attackSource != AttackSource.Melee && attackSource != AttackSource.Range))
				{
					Msg.Say(text, this, "", text2, null);
				}
				string text3 = this.TalkTopic("dead");
				if (!text3.IsEmpty())
				{
					text3 = text3.StripBrackets();
				}
				bool flag = base.rarity >= Rarity.Legendary && !this.IsPCFaction;
				if (!this.IsPC && flag)
				{
					this.MakeGrave(text3);
				}
				Msg.SetColor();
				base.SpawnLoot(origin);
			}
			if (this.held != null && this.held.trait.CanOnlyCarry)
			{
				this.DropHeld(null);
			}
		}
		if (this.IsPCFaction)
		{
			if (this.homeBranch != null)
			{
				this.homeBranch.Log(text, this, "", null, null);
			}
			WidgetPopText.Say("popDead".lang(base.Name, null, null, null, null), FontColor.Bad, null);
			if (!this.IsPC)
			{
				if (EClass.player.stats.allyDeath == 0)
				{
					Tutorial.Reserve("death_pet", null);
				}
				EClass.player.stats.allyDeath++;
			}
		}
		if (this.id == "mandrake")
		{
			base.Say("a_scream", this, null, null);
			ActEffect.ProcAt(EffectId.Scream, base.LV * 3 + 200, BlessedState.Normal, this, this, this.pos, true, default(ActRef));
		}
		this.daysStarved = 0;
		this.isDead = true;
		this.enemy = null;
		this._cooldowns = null;
		base.isSale = false;
		EClass._map.props.sales.Remove(this);
		this.Cure(CureType.Death, 100, BlessedState.Normal);
		this.SetAI(new NoGoal());
		this.TryDropBossLoot();
		if (isInActiveZone && EClass._zone.HasLaw && this.IsHuman && this.OriginalHostility >= Hostility.Neutral)
		{
			this.pos.TalkWitnesses((origin != null) ? origin.Chara : null, "witness", 3, WitnessType.crime, (Chara c) => !c.IsPCParty && !c.IsUnique, 3);
		}
		if (this.IsPC)
		{
			EClass.player.returnInfo = null;
			EClass.player.uidLastTravelZone = 0;
			foreach (Chara chara in EClass._map.charas)
			{
				if (chara.IsHostile())
				{
					chara.hostility = chara.OriginalHostility;
				}
				if (chara.enemy == EClass.pc)
				{
					chara.enemy = null;
				}
			}
			if (EClass.pc.things.Find("letter_will", -1, -1) != null && EClass.rnd(10) == 0)
			{
				EClass.player.preventDeathPenalty = true;
			}
		}
		else
		{
			bool flag2 = this.currentZone.IsActiveZone && this.IsLocalChara && !this.currentZone.IsPCFaction;
			if (this.currentZone is Zone_LittleGarden && this.id == "littleOne")
			{
				flag2 = false;
			}
			if (flag2)
			{
				EClass._map.deadCharas.Add(this);
			}
			this.currentZone.RemoveCard(this);
		}
		string id;
		if ((origin != null && origin.IsPCParty) || this.IsPCFaction)
		{
			int a = -10;
			if (this.IsPCFaction && !this.IsPCParty && (origin == null || !origin.IsPCParty))
			{
				a = -5;
			}
			this.ModAffinity(EClass.pc, a, false);
			id = this.id;
			if (!(id == "quru"))
			{
				if (id == "corgon")
				{
					Chara chara2 = EClass.game.cards.globalCharas.Find("quru");
					if (chara2 != null)
					{
						chara2.ModAffinity(EClass.pc, -20, false);
					}
				}
			}
			else
			{
				Chara chara3 = EClass.game.cards.globalCharas.Find("corgon");
				if (chara3 != null)
				{
					chara3.ModAffinity(EClass.pc, -20, false);
				}
			}
		}
		if (origin != null)
		{
			if (origin.IsPCParty || origin.IsPCPartyMinion)
			{
				int num = 0;
				if (this.OriginalHostility >= Hostility.Friend && this.IsHuman && !this.IsPCFaction && !this.IsPCFactionMinion)
				{
					num = -5;
				}
				else if (this.race.id == "cat" && this.OriginalHostility >= Hostility.Neutral)
				{
					EClass.pc.Say("killcat", null, null);
					num = -1;
				}
				if (EClass.pc.party.HasElement(1563) && num < 0)
				{
					num = num * 30 / 100;
				}
				if (num != 0)
				{
					EClass.player.ModKarma(num);
				}
			}
			if (origin == EClass.pc)
			{
				EClass.pc.faith.Revelation("kill", 10);
			}
			else if (origin.IsPCFaction)
			{
				origin.Chara.ModAffinity(EClass.pc, 1, false);
				origin.Chara.ShowEmo(Emo.love, 0f, true);
			}
		}
		if (base.sourceBacker != null && origin != null && origin.IsPCParty)
		{
			EClass.player.doneBackers.Add(base.sourceBacker.id);
		}
		base.SetInt(103, this.IsPCParty ? 1 : 0);
		if (this.IsPCParty)
		{
			if (!this.IsPC)
			{
				EClass.pc.party.RemoveMember(this);
				EClass.pc.Say("allyDead", null, null);
				if (EClass.game.config.autoCombat.abortOnAllyDead && EClass.player.TryAbortAutoCombat())
				{
					Msg.Say("abort_allyDead");
				}
			}
		}
		else if (EClass.game.config.autoCombat.abortOnEnemyDead && EClass.player.TryAbortAutoCombat())
		{
			Msg.Say("abort_enemyDead");
		}
		id = this.id;
		if (!(id == "littleOne"))
		{
			if (id == "big_daddy")
			{
				if (!this.IsPCFaction)
				{
					Chara t = CharaGen.Create("littleOne", -1);
					EClass._zone.AddCard(t, this.pos.Copy());
					Msg.Say("little_pop");
				}
			}
		}
		else if (attackSource != AttackSource.DeathSentense)
		{
			EClass.player.flags.little_killed = true;
			EClass.player.little_dead++;
		}
		if (attackSource == AttackSource.Finish && origin != null && origin.Evalue(665) > 0)
		{
			Chara chara4 = CharaGen.CreateFromFilter("c_plant", base.LV, -1);
			EClass._zone.AddCard(chara4, this.pos.Copy());
			if (chara4.LV < base.LV)
			{
				chara4.SetLv(base.LV);
			}
			chara4.MakeMinion((origin.IsPCParty || origin.IsPCPartyMinion) ? EClass.pc : origin.Chara, MinionType.Friend);
			Msg.Say("plant_pop", this, chara4, null, null);
		}
		foreach (ZoneEvent zoneEvent in EClass._zone.events.list)
		{
			zoneEvent.OnCharaDie(this);
		}
	}

	public void TryDropBossLoot()
	{
		if (this.IsPCFaction || this.IsPCFactionMinion)
		{
			return;
		}
		int num = 0;
		bool flag = false;
		bool flag2 = false;
		Point point = this.pos.GetNearestPoint(true, false, false, true) ?? this.pos;
		TreasureType type = TreasureType.BossQuest;
		if (EClass._zone.Boss == this)
		{
			type = TreasureType.BossNefia;
			num = 2 + EClass.rnd(2);
			flag2 = (flag = true);
			EClass._zone.Boss = null;
			Msg.Say("boss_win", EClass._zone.Name, null, null, null);
			if (EClass._zone is Zone_Void && (!EClass.player.flags.KilledBossInVoid || EClass.debug.enable))
			{
				Msg.Say("boss_win_void", EClass._zone.Name, null, null, null);
				EClass.player.flags.KilledBossInVoid = true;
			}
			if (EClass._zone.IsNefia)
			{
				EClass._zone.GetTopZone().isConquered = true;
				EClass.Sound.StopBGM(2f, false);
				SE.Play("Jingle/fanfare2");
				EClass._zone.SetBGM(114, true);
			}
			EClass.player.ModFame(EClass.rndHalf(30 + EClass._zone.DangerLv * 2));
			EClass.player.ModKarma(5);
		}
		string id = this.id;
		if (!(id == "vernis_boss"))
		{
			if (!(id == "melilith_boss"))
			{
				if (!(id == "isygarad"))
				{
					if (id == "swordkeeper")
					{
						num = 10;
						flag = true;
						SE.Play("kill_boss");
						SoundManager.ForceBGM();
						LayerDrama.Activate("_event", "event", "swordkeeper_defeat", null, null, "");
					}
				}
				else
				{
					num = 5;
					flag2 = (flag = true);
					QuestExploration questExploration = EClass.game.quests.Get<QuestExploration>();
					if (questExploration != null)
					{
						SE.Play("kill_boss");
						questExploration.ChangePhase(3);
						EClass.Sound.StopBGM(3f, false);
						EClass._zone.SetBGM(1, false);
					}
				}
			}
			else
			{
				num = 5;
				flag2 = (flag = true);
				EClass.Sound.StopBGM(3f, false);
				EClass._zone.SetBGM(1, false);
			}
		}
		else
		{
			num = 5;
			flag2 = (flag = true);
			EClass.Sound.StopBGM(3f, false);
			EClass._zone.SetBGM(1, false);
			if (EClass.game.quests.GetPhase<QuestVernis>() == 8)
			{
				EClass.game.quests.Get<QuestVernis>().UpdateOnTalk();
			}
		}
		if (flag)
		{
			SE.Play("kill_boss");
		}
		if (num != 0)
		{
			EClass.player.willAutoSave = true;
			Thing thing = ThingGen.CreateTreasure("chest_boss", base.LV, type);
			point.SetBlock(0, 0);
			point.SetObj(0, 1, 0);
			EClass._zone.AddCard(thing, point).Install();
			ThingGen.TryLickChest(thing);
		}
		if (flag2)
		{
			EClass._zone.AddCard(ThingGen.CreateScroll(8221, 1), this.pos);
		}
	}

	public void Kick(Point p, bool ignoreSelf = false)
	{
		foreach (Chara t in p.ListCharas())
		{
			this.Kick(t, ignoreSelf, true);
		}
	}

	public void Kick(Chara t, bool ignoreSelf = false, bool karmaLoss = true)
	{
		if (!this.IsAliveInCurrentZone)
		{
			return;
		}
		if (t.IsPC)
		{
			ActionMode.Adv.ClearPlans();
		}
		if (t.host != null)
		{
			return;
		}
		if (t == this)
		{
			if (!ignoreSelf)
			{
				Debug.Log(t.pos.GetNearestPoint(false, true, true, false));
				if (this.TryMove(t.pos.GetNearestPoint(false, true, true, false), true) != Card.MoveResult.Success)
				{
					t.MoveImmediate(this.pos.GetNearestPoint(false, true, true, false) ?? t.pos, true, true);
					return;
				}
			}
		}
		else
		{
			base.Say("kick", this, t, null, null);
			base.PlaySound("kick", 1f, true);
			if ((t.conSuspend == null || t.conSuspend.uidMachine != 0) && t.trait.CanBePushed && (!t.IsHostile() || EClass.rnd(2) == 0) && !t.noMove && !t.isRestrained)
			{
				t.MoveByForce(t.pos.GetNearestPoint(false, false, true, true), this, !t.pos.IsBlocked);
			}
			if (t.conSleep != null)
			{
				t.conSleep.Kill(false);
			}
			if (this.IsPC && t.IsFriendOrAbove() && !t.IsPCFactionOrMinion && karmaLoss)
			{
				EClass.player.ModKarma(-1);
			}
			t.PlayEffect("kick", true, 0f, default(Vector3));
		}
	}

	public bool UseAbility(string idAct, Card tc = null, Point pos = null, bool pt = false)
	{
		Element element = this.elements.GetElement(idAct);
		return this.UseAbility(((element != null) ? element.act : null) ?? ACT.Create(idAct), tc, pos, pt);
	}

	public bool UseAbility(Act a, Card tc = null, Point pos = null, bool pt = false)
	{
		Chara.<>c__DisplayClass377_0 CS$<>8__locals1 = new Chara.<>c__DisplayClass377_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.tc = tc;
		CS$<>8__locals1.a = a;
		CS$<>8__locals1.pos = pos;
		CS$<>8__locals1.pt = pt;
		if (!this.IsPC && this.HasCooldown(CS$<>8__locals1.a.id))
		{
			return false;
		}
		int num = 1;
		Act.Cost cost = CS$<>8__locals1.a.GetCost(this);
		CS$<>8__locals1.a.GetPower(this);
		CS$<>8__locals1.n = 1;
		int num2 = 0;
		if (this.IsPC && this.HasCondition<StanceManaCost>())
		{
			num2 = base.Evalue(1657);
		}
		Chara._pts.Clear();
		if (CS$<>8__locals1.a.TargetType.ForceParty)
		{
			CS$<>8__locals1.pt = true;
		}
		if (CS$<>8__locals1.pt)
		{
			CS$<>8__locals1.n = 0;
			CS$<>8__locals1.<UseAbility>g__ForeachParty|0(delegate(Chara c)
			{
				int n = CS$<>8__locals1.n;
				CS$<>8__locals1.n = n + 1;
			});
		}
		if (CS$<>8__locals1.a is Spell && this.IsPC && CS$<>8__locals1.a.vPotential < CS$<>8__locals1.n)
		{
			CS$<>8__locals1.n = 1;
			Chara._pts.Clear();
			Chara._pts.Add(this);
			CS$<>8__locals1.pt = false;
		}
		int num3 = 100;
		if (!CS$<>8__locals1.a.TargetType.ForceParty && CS$<>8__locals1.n > 1)
		{
			num3 = (this.IsPC ? (CS$<>8__locals1.n * 100) : (50 + CS$<>8__locals1.n * 50));
		}
		int num4 = cost.cost * num3 / 100;
		if (cost.type == Act.CostType.MP && base.Evalue(483) > 0)
		{
			num4 = num4 * 100 / (100 + (int)Mathf.Sqrt((float)(base.Evalue(483) * 10)) * 3);
		}
		if (CS$<>8__locals1.n == 0)
		{
			if (this.IsPC)
			{
				Msg.SayNothingHappen();
			}
			return false;
		}
		if (!this.IsPC && cost.type == Act.CostType.MP && this.mana.value < 0 && (EClass.rnd(4) != 0 || this.IsPCFaction || (base.IsPowerful && this.mana.value < -20)))
		{
			return false;
		}
		if (this.IsPC)
		{
			if (!Dialog.warned && cost.type == Act.CostType.MP && cost.cost > 0 && this.mana.value < num4 && !EClass.debug.godMode)
			{
				ActPlan.warning = true;
				Dialog.TryWarnMana(delegate
				{
					if (CS$<>8__locals1.<>4__this.UseAbility(CS$<>8__locals1.a, CS$<>8__locals1.tc, CS$<>8__locals1.pos, CS$<>8__locals1.pt))
					{
						EClass.player.EndTurn(true);
					}
				});
				return false;
			}
			EClass.ui.CloseLayers();
		}
		if ((this.isConfused && EClass.rnd(4) == 0) || (this.isBlind && (CS$<>8__locals1.pt || (CS$<>8__locals1.pos != null && !CS$<>8__locals1.pos.Equals(this.pos)) || (CS$<>8__locals1.tc != null && CS$<>8__locals1.tc.pos != null && !CS$<>8__locals1.tc.pos.Equals(this.pos))) && EClass.rnd(2) == 0))
		{
			base.Say("shakeHead", this, null, null);
			return true;
		}
		if (CS$<>8__locals1.tc != null && CS$<>8__locals1.tc != this)
		{
			this.LookAt(CS$<>8__locals1.tc.pos);
		}
		if (CS$<>8__locals1.pos != null && !this.pos.Equals(CS$<>8__locals1.pos))
		{
			this.LookAt(CS$<>8__locals1.pos);
		}
		if (CS$<>8__locals1.a.CanRapidFire && base.HasElement(1648, 1))
		{
			num = 1 + base.Evalue(1648);
		}
		if (this.IsPC && cost.cost > 0 && CS$<>8__locals1.a.Value == 0)
		{
			Msg.SayNothingHappen();
			return false;
		}
		if (CS$<>8__locals1.a is Spell)
		{
			string s = this.isConfused ? "_cast_confuse" : (this.HasCondition<ConDim>() ? "_cast_dim" : "");
			if (!CS$<>8__locals1.a.source.tag.Contains("useHand"))
			{
				base.Say(this.race.castStyle.IsEmpty("cast"), this, CS$<>8__locals1.a.source.GetName().ToLower(), s.lang());
			}
			if (this.IsPC)
			{
				int num5 = (CS$<>8__locals1.n + 1) / 2;
				if (CS$<>8__locals1.a.vPotential < CS$<>8__locals1.n)
				{
					Msg.Say("noSpellStock");
					EInput.Consume(false, 1);
					return false;
				}
				if (num2 > 0 && CS$<>8__locals1.a.vPotential >= CS$<>8__locals1.n * 2)
				{
					CS$<>8__locals1.a.vPotential -= CS$<>8__locals1.n * 2;
					num4 = num4 * (100 - num2 * 20) / 100;
				}
				else
				{
					CS$<>8__locals1.a.vPotential -= CS$<>8__locals1.n;
				}
				LayerAbility.SetDirty(CS$<>8__locals1.a);
			}
		}
		else if (CS$<>8__locals1.a.source.langAct.Length != 0 && CS$<>8__locals1.tc != null)
		{
			string text = CS$<>8__locals1.a.source.langAct[0];
			string text2 = (CS$<>8__locals1.a.source.langAct.Length >= 2) ? CS$<>8__locals1.a.source.langAct[1] : "";
			if (text == "spell_hand")
			{
				string[] list = Lang.GetList("attack" + this.race.meleeStyle.IsEmpty("Touch"));
				string @ref = text2.lang(list[4], null, null, null, null);
				base.Say(CS$<>8__locals1.tc.IsPCParty ? "cast_hand_ally" : "cast_hand", this, CS$<>8__locals1.tc, @ref, CS$<>8__locals1.tc.IsPCParty ? list[1] : list[2]);
			}
			else
			{
				base.Say(text, this, CS$<>8__locals1.tc, text2.IsEmpty() ? "" : text2.lang(), null);
			}
			if (CS$<>8__locals1.a.source.id == 6630)
			{
				base.Talk("insult_" + (base.IsMale ? "m" : "f"), null, null, false);
			}
		}
		Act.CostType type = cost.type;
		if (type != Act.CostType.MP)
		{
			if (type == Act.CostType.SP)
			{
				this.stamina.Mod(-num4);
			}
		}
		else
		{
			if (base.Evalue(1421) >= 2 && base.hp <= this.MaxHP / (9 - base.Evalue(1421) * 2))
			{
				num4 /= 2;
			}
			base.PlayEffect("cast", true, 0f, default(Vector3));
			this.mana.Mod(-num4);
			if (this.isDead)
			{
				return true;
			}
			this.elements.ModExp(304, Mathf.Clamp(num4 * 2, 1, 200), false);
		}
		if (CS$<>8__locals1.a is Spell && this.GetCondition<ConSilence>() != null)
		{
			base.Say("cast_silence", this, null, null);
			return true;
		}
		if (this.isDead)
		{
			return true;
		}
		int spellExp = this.elements.GetSpellExp(this, CS$<>8__locals1.a, num3);
		if (EClass.rnd(100) >= this.CalcCastingChance(CS$<>8__locals1.a, CS$<>8__locals1.n) && !EClass.debug.godMode)
		{
			base.Say("fizzle", this, null, null);
			base.PlayEffect("fizzle", true, 0f, default(Vector3));
			base.PlaySound("fizzle", 1f, true);
			if (cost.cost > 0 && CS$<>8__locals1.a.source.lvFactor > 0)
			{
				base.ModExp(CS$<>8__locals1.a.id, spellExp / 5);
			}
			this.RemoveCondition<ConInvisibility>();
			return true;
		}
		bool flag = true;
		if (CS$<>8__locals1.pt)
		{
			Act.forcePt = true;
			CS$<>8__locals1.<UseAbility>g__ForeachParty|0(delegate(Chara c)
			{
				CS$<>8__locals1.a.Perform(CS$<>8__locals1.<>4__this, c, c.pos);
			});
			Act.forcePt = false;
		}
		else
		{
			int num6 = 0;
			while (num6 < num && (CS$<>8__locals1.a.TargetType == TargetType.SelfParty || CS$<>8__locals1.tc == null || CS$<>8__locals1.tc.IsAliveInCurrentZone))
			{
				ActEffect.RapidCount = num6;
				ActEffect.RapidDelay = CS$<>8__locals1.a.RapidDelay;
				flag = CS$<>8__locals1.a.Perform(this, CS$<>8__locals1.tc, CS$<>8__locals1.pos);
				num6++;
			}
		}
		if (flag && !this.isDead && cost.cost > 0 && CS$<>8__locals1.a.source.lvFactor > 0)
		{
			base.ModExp(CS$<>8__locals1.a.id, spellExp);
		}
		ActEffect.RapidCount = 0;
		if (!this.IsPC && CS$<>8__locals1.a.source.cooldown > 0)
		{
			this.AddCooldown(CS$<>8__locals1.a.id, CS$<>8__locals1.a.source.cooldown);
		}
		if (flag && !CS$<>8__locals1.a.source.tag.Contains("keepInvisi") && EClass.rnd(2) == 0)
		{
			this.RemoveCondition<ConInvisibility>();
		}
		return flag;
	}

	public int EvalueRiding()
	{
		if (this.ride != null && this.ride.HasCondition<ConTransmuteBroom>() && base.HasElement(1417, 1))
		{
			return 25 + base.Evalue(226) * 125 / 100;
		}
		return base.Evalue(226);
	}

	public int CalcCastingChance(Element e, int num = 1)
	{
		if (!(e is Spell))
		{
			return 100;
		}
		if (!this.IsPC)
		{
			int num2 = 95;
			if (this.host != null)
			{
				if (this.host.ride == this)
				{
					return num2 * 100 / (100 + 300 / Mathf.Max(5, 10 + this.host.EvalueRiding()));
				}
				if (this.host.parasite == this)
				{
					return num2 * 100 / (100 + 300 / Mathf.Max(5, 10 + this.host.Evalue(227)));
				}
			}
			return num2;
		}
		int num3 = base.Evalue(304);
		if (!this.IsPCFaction)
		{
			num3 = Mathf.Max(num3, base.LV + 5);
		}
		int num4 = 0;
		bool flag = this.GetArmorSkill() == 122;
		AttackStyle attackStyle = this.body.GetAttackStyle();
		int num5;
		if (flag)
		{
			num5 = 20 - base.Evalue(122) / 5;
			num4 += 10 - base.Evalue(1654) * 4;
		}
		else
		{
			num5 = 10 - base.Evalue(120) / 5;
		}
		if (num5 < 5)
		{
			num5 = 5;
		}
		if (this.ride != null)
		{
			num5 += 5;
		}
		if (this.parasite != null)
		{
			num5 += 10;
		}
		if (attackStyle == AttackStyle.TwoWield)
		{
			num5 += 5;
		}
		if (attackStyle == AttackStyle.Shield)
		{
			num5 += 5;
			num4 += 10 - base.Evalue(1654) * 4;
		}
		if (this.isConfused)
		{
			num5 += 10000;
		}
		if (this.HasCondition<ConDim>())
		{
			num5 += ((base.Evalue(1654) >= 3) ? 1500 : 2500);
		}
		if (num > 1)
		{
			num5 += 5 * num;
		}
		if (num4 < 0)
		{
			num4 = 0;
		}
		return Mathf.Clamp(100 + e.Value - 10 - e.source.LV * e.source.cost[0] * num5 / (10 + num3 * 10), 0, 100 - num4);
	}

	public void DoAI(int wait, Action onPerform)
	{
		this.SetAI(new DynamicAIAct("", delegate()
		{
			onPerform();
			return true;
		}, false)
		{
			wait = wait
		});
	}

	public bool IsMofuable
	{
		get
		{
			return this.race.tag.Contains("mofu");
		}
	}

	public void Cuddle(Chara c, bool headpat = false)
	{
		base.Talk("goodBoy", null, null, false);
		base.Say(headpat ? "headpat" : "cuddle", this, c, null, null);
		c.ShowEmo(Emo.love, 0f, true);
		if (EClass.rnd(this.IsPC ? 100 : 5000) == 0)
		{
			c.MakeEgg(true, 1, true);
		}
		if (headpat && this != c)
		{
			if (c.interest > 0)
			{
				c.ModAffinity(EClass.pc, 1 + EClass.rnd(3), true);
				c.interest -= 20 + EClass.rnd(10);
			}
			if (this.faith == EClass.game.religions.MoonShadow && c.IsPCParty)
			{
				foreach (Chara chara in this.party.members)
				{
					if (!chara.IsPC && this.CanSeeLos(chara, -1, false))
					{
						chara.AddCondition<ConEuphoric>(100 + base.Evalue(6904) * 5, false);
					}
				}
			}
		}
	}

	public Chara SetEnemy(Chara c = null)
	{
		this.enemy = c;
		if (c != null)
		{
			this.calmCheckTurn = 10 + EClass.rnd(30);
		}
		return c;
	}

	public void TrySetEnemy(Chara c)
	{
		if (this.IsPC && EClass.game.config.autoCombat.bDontChangeTarget)
		{
			return;
		}
		if (this.enemy != null && (EClass.rnd(5) != 0 || base.Dist(c) > 1))
		{
			return;
		}
		if (((this.IsPCFaction || this.IsPCFactionMinion) && (c.IsPCFaction || c.IsPCFactionMinion)) || (this.hostility == Hostility.Enemy && c.hostility == Hostility.Enemy))
		{
			return;
		}
		if (c.IsPC && this.hostility >= Hostility.Neutral)
		{
			return;
		}
		this.SetEnemy(c);
	}

	private void GoHostile(Card _tg)
	{
		if (this.enemy == null && !this.IsPC)
		{
			if (base.GetInt(106, null) == 0)
			{
				this.TalkTopic("aggro");
			}
			if (this.OriginalHostility != Hostility.Enemy)
			{
				base.ShowEmo(Emo.angry, 0f, true);
			}
			this.SetEnemy(_tg.Chara);
		}
		if (!this.IsPCFaction && !this.IsPCFactionMinion && (_tg.IsPCFaction || _tg.IsPCFactionMinion))
		{
			if (this.hostility >= Hostility.Neutral)
			{
				base.Say("angry", this, null, null);
			}
			this.hostility = Hostility.Enemy;
		}
	}

	public void DoHostileAction(Card _tg, bool immediate = false)
	{
		if (_tg == null || !_tg.isChara)
		{
			return;
		}
		Chara chara = _tg.Chara;
		if (chara.IsPC)
		{
			EClass.pc.combatCount = 10;
		}
		if (!chara.IsAliveInCurrentZone || !this.IsAliveInCurrentZone || chara == this)
		{
			return;
		}
		if ((this.IsPCFaction || this.IsPCFactionMinion) && (chara.IsPCFaction || chara.IsPCFactionMinion))
		{
			chara.Say("frown", this, chara, null, null);
			return;
		}
		if (EClass._zone.IsRegion)
		{
			return;
		}
		if (this.IsPC)
		{
			if (chara.IsFriendOrAbove() && !immediate)
			{
				chara.Say("frown", this, chara, null, null);
				chara.ShowEmo(Emo.sad, 0f, true);
				chara.hostility = Hostility.Neutral;
				return;
			}
			if (!chara.IsPCFaction && chara.hostility >= Hostility.Neutral && !EClass._zone.IsPCFaction)
			{
				bool flag = chara.id == "fanatic";
				if (EClass.rnd(4) == 0 || flag)
				{
					chara.Say("callHelp", chara, null, null);
					chara.CallHelp(this, flag);
				}
			}
			if (chara.hostility > Hostility.Enemy)
			{
				goto IL_1C5;
			}
			using (List<Chara>.Enumerator enumerator = EClass.pc.party.members.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Chara chara2 = enumerator.Current;
					if (chara2 != EClass.pc && chara2.enemy == null)
					{
						chara2.SetEnemy(chara);
					}
				}
				goto IL_1C5;
			}
		}
		if (chara.IsPC && this.hostility <= Hostility.Enemy)
		{
			foreach (Chara chara3 in EClass.pc.party.members)
			{
				if (chara3 != EClass.pc && chara3.enemy == null)
				{
					chara3.SetEnemy(this);
				}
			}
		}
		IL_1C5:
		if (chara.calmCheckTurn <= 0 || this.IsPC)
		{
			chara.calmCheckTurn = (this.IsPC ? (20 + EClass.rnd(30)) : (10 + EClass.rnd(10)));
		}
		if (this.hostility == Hostility.Enemy && chara.hostility == Hostility.Enemy)
		{
			return;
		}
		this.GoHostile(chara);
		chara.GoHostile(this);
		if (base.isHidden && !chara.CanSee(this) && !chara.IsDisabled && !chara.IsPCParty && !chara.IsPCPartyMinion && EClass.rnd(6) == 0)
		{
			Thing t = ThingGen.Create("49", -1, -1);
			ActThrow.Throw(chara, this.pos, t, ThrowMethod.Default, 0f);
		}
	}

	public void CallHelp(Chara tg, bool fanatic = false)
	{
		foreach (Chara chara in EClass._map.charas)
		{
			if (!chara.IsPCFaction && chara.OriginalHostility == this.OriginalHostility && (fanatic || base.Dist(chara) <= 6) && (EClass.rnd(3) != 0 || fanatic))
			{
				chara.GoHostile(tg);
			}
		}
	}

	public bool FindNewEnemy()
	{
		if (EClass._zone.isPeace && base.IsPCFactionOrMinion)
		{
			return false;
		}
		if (this.enemy != null && !this.enemy.IsAliveInCurrentZone)
		{
			this.enemy = null;
		}
		if (this.enemy != null)
		{
			return false;
		}
		bool flag = this.enemy != null || this.ai is GoalCombat;
		int num = (base.PER + base.Evalue(210) * 2) * (flag ? 2 : 1);
		bool flag2 = this.IsPCParty && !this.IsPC && EClass.game.config.tactics.dontWander;
		bool flag3 = !this.IsPCParty;
		for (int i = 0; i < EClass._map.charas.Count; i++)
		{
			Chara chara = EClass._map.charas[i];
			if (chara != this && this.IsHostile(chara) && this.CanSee(chara))
			{
				int num2 = base.Dist(chara);
				int num3 = base.GetSightRadius() + (flag ? 1 : 0);
				if (num2 <= num3)
				{
					if (flag3 && EClass.rnd(chara.Evalue(152) + 5) * (100 + num2 * num2 * 10) / 100 > EClass.rnd(num))
					{
						if (this == this.pos.FirstChara)
						{
							chara.ModExp(152, Mathf.Clamp((num - chara.Evalue(152)) / 2, 1, Mathf.Max(30 - this.stealthSeen * 2, 1)));
						}
						this.stealthSeen++;
					}
					else if (Los.IsVisible(this.pos.x, chara.pos.x, this.pos.z, chara.pos.z, null, true) && (!flag2 || EClass.pc.isBlind || EClass.pc.CanSeeLos(chara, -1, false)))
					{
						if (this.IsPCFaction)
						{
							AI_Shear ai_Shear = EClass.pc.ai as AI_Shear;
							if (ai_Shear != null && ai_Shear.target == chara)
							{
								goto IL_217;
							}
						}
						this.DoHostileAction(chara, false);
						this.enemy = chara;
						return true;
					}
				}
			}
			IL_217:;
		}
		return false;
	}

	public bool FindNearestNewEnemy()
	{
		for (int i = 0; i < EClass._map.charas.Count; i++)
		{
			Chara chara = EClass._map.charas[i];
			if (chara != this && chara != this.enemy && this.IsHostile(chara) && base.Dist(chara) <= 1 && this.CanInteractTo(chara.pos))
			{
				this.DoHostileAction(chara, false);
				this.enemy = chara;
				return true;
			}
		}
		return false;
	}

	public bool IsHostile()
	{
		return this.hostility <= Hostility.Enemy;
	}

	public bool IsHostile(Chara c)
	{
		if (c == null)
		{
			return false;
		}
		if (base.IsPCFactionOrMinion)
		{
			if ((c == EClass.pc.enemy && !c.IsPCFactionOrMinion) || c.hostility <= Hostility.Enemy)
			{
				return true;
			}
		}
		else
		{
			if (this.trait is TraitGuard && c.IsPCParty && EClass.player.IsCriminal && EClass._zone.instance == null)
			{
				return true;
			}
			if (this.OriginalHostility >= Hostility.Friend)
			{
				if (c.hostility <= Hostility.Enemy && c.OriginalHostility == Hostility.Enemy)
				{
					return true;
				}
			}
			else if (this.OriginalHostility <= Hostility.Enemy && (c.IsPCFactionOrMinion || (c.OriginalHostility != Hostility.Enemy && c.hostility >= Hostility.Friend)))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsNeutral()
	{
		return this.hostility == Hostility.Neutral;
	}

	public bool IsNeutralOrAbove()
	{
		return this.hostility >= Hostility.Neutral;
	}

	public bool IsBranchMember()
	{
		return this.faction == EClass.Home && this.homeZone == EClass.game.activeZone;
	}

	public bool IsHomeMember()
	{
		return this.faction == EClass.Home;
	}

	public bool IsInHomeZone()
	{
		return EClass.game.activeZone == this.currentZone;
	}

	public bool IsInSpot<T>() where T : TraitSpot
	{
		foreach (T t in EClass._map.props.installed.traits.List<T>(null))
		{
			foreach (Point obj in t.ListPoints(null, true))
			{
				if (this.pos.Equals(obj))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool IsGuest()
	{
		return this.memberType == FactionMemberType.Guest;
	}

	public bool IsFriendOrAbove()
	{
		return this.hostility >= Hostility.Friend;
	}

	public bool IsFriendOrAbove(Chara c)
	{
		if (base.IsPCFactionOrMinion || this.IsFriendOrAbove())
		{
			if (c.IsPCFactionOrMinion || c.IsFriendOrAbove())
			{
				return true;
			}
		}
		else if (this.IsHostile() && c.IsHostile())
		{
			return true;
		}
		return this.race == c.race;
	}

	public override CardRenderer _CreateRenderer()
	{
		CharaRenderer charaRenderer = new CharaRenderer();
		if (this.source.moveAnime == "hop")
		{
			charaRenderer.hopCurve = EClass.setting.render.anime.hop;
		}
		if (this.host != null)
		{
			charaRenderer.pccData = PCCData.Create("ride");
			string text = base.c_idRidePCC.IsEmpty(EClass.core.pccs.sets["ride"].map["body"].map.Keys.First<string>());
			charaRenderer.pccData.SetPart("body", "ride", text, null);
			charaRenderer.pccData.ride = EClass.core.pccs.GetRideData(text);
		}
		else
		{
			foreach (Condition condition in this.conditions)
			{
				RendererReplacer rendererReplacer = condition.GetRendererReplacer();
				if (rendererReplacer != null)
				{
					charaRenderer.replacer = rendererReplacer;
					charaRenderer.data = rendererReplacer.data;
					break;
				}
			}
			if (charaRenderer.replacer == null)
			{
				charaRenderer.pccData = this.pccData;
			}
		}
		this.renderer = charaRenderer;
		this.renderer.SetOwner(this);
		return charaRenderer;
	}

	public void SetPCCState(PCCState state)
	{
		if (this.IsPCC)
		{
			PCC.Get(this.pccData).Build(state, false);
		}
	}

	public override Sprite GetSprite(int dir = 0)
	{
		if (this.IsPCC)
		{
			PCC pcc = PCC.Get(this.pccData);
			pcc.Build(false);
			return pcc.variation.idle[0, 0];
		}
		return this.sourceCard.GetSprite((this.sourceCard._tiles.Length > 1) ? ((base.idSkin != 0 || this.source.staticSkin) ? base.idSkin : (base.uid % this.sourceCard._tiles.Length / 2 * 2 + (base.IsMale ? 0 : 1))) : 0, 0, false);
	}

	public void SetTempHand(int right = 0, int left = 0)
	{
		if (!this.IsPC)
		{
			return;
		}
		if (this.IsPC)
		{
			return;
		}
		this.pccData.tempRight = EClass.scene.screenElin.renderTempEQ.ConvertTile(right);
		this.pccData.tempLeft = EClass.scene.screenElin.renderTempEQ.ConvertTile(left);
	}

	public override SubPassData GetSubPassData()
	{
		if (this.IsPCC && (this.IsDeadOrSleeping || (!EClass.player.altHeldPos && this.parent is Chara)))
		{
			return EClass.setting.pass.subDeadPCC;
		}
		return SubPassData.Default;
	}

	public override void SetRenderParam(RenderParam p)
	{
		p.mat = base.material;
		p.matColor = (float)base.colorInt;
		if (!this.renderer.usePass)
		{
			return;
		}
		if (this.renderer.replacer != null)
		{
			p.tile = (float)(this.renderer.replacer.tile * (this.flipX ? -1 : 1));
		}
		else if (this.source._tiles_snow.Length != 0 && EClass._zone.IsSnowCovered)
		{
			if (this.source._tiles_snow.Length > 1)
			{
				int num = (base.idSkin != 0 || this.source.staticSkin) ? base.idSkin : (base.uid % this.source._tiles_snow.Length / 2 * 2 + (base.IsMale ? 0 : 1));
				p.tile = (float)(this.source._tiles_snow[(num < this.source._tiles_snow.Length) ? num : 0] * (this.flipX ? -1 : 1));
			}
			else
			{
				p.tile = (float)(this.source._tiles_snow[0] * (this.flipX ? -1 : 1));
			}
		}
		else if (this.sourceCard._tiles.Length > 1)
		{
			int num2 = (base.idSkin != 0 || this.source.staticSkin) ? base.idSkin : (base.uid % this.sourceCard._tiles.Length / 2 * 2 + (base.IsMale ? 0 : 1));
			p.tile = (float)(this.sourceCard._tiles[(num2 < this.sourceCard._tiles.Length) ? num2 : 0] * (this.flipX ? -1 : 1));
		}
		else
		{
			p.tile = (float)(this.sourceCard._tiles[0] * (this.flipX ? -1 : 1));
		}
		p.dir = base.dir;
	}

	public override string GetHoverText()
	{
		string text = base.Name;
		if (this.IsFriendOrAbove())
		{
			text = text.TagColor(EClass.Colors.colorFriend);
		}
		else if (this.IsHostile())
		{
			text = text.TagColor(EClass.Colors.colorHostile);
		}
		int num = 2;
		int lv = EClass.pc.LV;
		if (base.LV >= lv * 5)
		{
			num = 0;
		}
		else if (base.LV >= lv * 2)
		{
			num = 1;
		}
		else if (base.LV <= lv / 4)
		{
			num = 4;
		}
		else if (base.LV <= lv / 2)
		{
			num = 3;
		}
		string text2 = Lang.GetList("lvComparison")[num];
		text2 = (" (" + text2 + ") ").TagSize(14).TagColor(EClass.Colors.gradientLVComparison.Evaluate(0.25f * (float)num));
		string s = this.IsFriendOrAbove() ? "HostilityAlly" : (this.IsNeutral() ? "HostilityNeutral" : "HostilityEnemy");
		s = (" (" + s.lang() + ") ").TagSize(14);
		if (!EClass.pc.IsMoving)
		{
			if (EClass.pc.HasHigherGround(this))
			{
				text2 += "lowerGround".lang();
			}
			else if (this.HasHigherGround(EClass.pc))
			{
				text2 += "higherGround".lang();
			}
		}
		if (base.Evalue(1232) > 0)
		{
			text2 = "milkBaby".lang().TagSize(14) + text2;
		}
		if (Guild.Fighter.ShowBounty(this) && Guild.Fighter.HasBounty(this))
		{
			text2 = "hasBounty".lang().TagSize(14) + text2;
		}
		if (EClass.pc.HasElement(481, 1))
		{
			text2 += ("( " + this.faith.Name + ")").TagSize(14);
		}
		return text + text2;
	}

	public override string GetHoverText2()
	{
		string str = "";
		if (this.knowFav)
		{
			str += Environment.NewLine;
			str = str + "<size=14>" + "favgift".lang(this.GetFavCat().GetName().ToLower(), this.GetFavFood().GetName(), null, null, null) + "</size>";
		}
		string text = "";
		if (EClass.debug.showExtra)
		{
			text += Environment.NewLine;
			text = string.Concat(new string[]
			{
				text,
				"Lv:",
				base.LV.ToString(),
				"  HP:",
				base.hp.ToString(),
				"/",
				this.MaxHP.ToString(),
				"  MP:",
				this.mana.value.ToString(),
				"/",
				this.mana.max.ToString(),
				"  DV:",
				this.DV.ToString(),
				"  PV:",
				this.PV.ToString(),
				"  Hunger:",
				this.hunger.value.ToString()
			});
			text += Environment.NewLine;
			string[] array = new string[7];
			array[0] = text;
			array[1] = "Global:";
			array[2] = this.IsGlobal.ToString();
			array[3] = "  AI:";
			int num = 4;
			AIAct aiact = this.ai;
			array[num] = ((aiact != null) ? aiact.ToString() : null);
			array[5] = " ";
			int num2 = 6;
			string tactics = this.source.tactics;
			SourceTactics.Row row = EClass.sources.tactics.map.TryGetValue(this.id, null);
			string defaultStr;
			if ((defaultStr = ((row != null) ? row.id : null)) == null)
			{
				SourceTactics.Row row2 = EClass.sources.tactics.map.TryGetValue(this.job.id, null);
				defaultStr = (((row2 != null) ? row2.id : null) ?? "predator");
			}
			array[num2] = tactics.IsEmpty(defaultStr);
			text = string.Concat(array);
			text += Environment.NewLine;
			string[] array2 = new string[7];
			array2[0] = text;
			array2[1] = base.uid.ToString();
			array2[2] = this.IsMinion.ToString();
			array2[3] = "/";
			array2[4] = base.c_uidMaster.ToString();
			array2[5] = "/";
			int num3 = 6;
			Chara chara = this.master;
			array2[num3] = ((chara != null) ? chara.ToString() : null);
			text = string.Concat(array2);
		}
		string text2 = "";
		IEnumerable<BaseStats> first = this.conditions;
		IEnumerable<BaseStats> second;
		if (!this.IsPCFaction)
		{
			second = new BaseStats[0];
		}
		else
		{
			BaseStats[] array3 = new BaseStats[2];
			array3[0] = this.hunger;
			second = array3;
			array3[1] = this.stamina;
		}
		IEnumerable<BaseStats> enumerable = first.Concat(second);
		if (enumerable.Count<BaseStats>() > 0)
		{
			str = "";
			text2 += Environment.NewLine;
			text2 += "<size=14>";
			foreach (BaseStats baseStats in enumerable)
			{
				string text3 = baseStats.GetPhaseStr();
				if (!text3.IsEmpty() && !(text3 == "#"))
				{
					Color c = Color.white;
					string group = baseStats.source.group;
					if (!(group == "Bad") && !(group == "Debuff") && !(group == "Disease"))
					{
						if (group == "Buff")
						{
							c = EClass.Colors.colorBuff;
						}
					}
					else
					{
						c = EClass.Colors.colorDebuff;
					}
					if (EClass.debug.showExtra)
					{
						text3 = text3 + "(" + baseStats.GetValue().ToString() + ")";
						if (this.resistCon != null && this.resistCon.ContainsKey(baseStats.id))
						{
							text3 = text3 + "{" + this.resistCon[baseStats.id].ToString() + "}";
						}
					}
					text2 = text2 + text3.TagColor(c) + ", ";
				}
			}
			text2 = text2.TrimEnd(", ".ToCharArray()) + "</size>";
		}
		return str + text + text2;
	}

	public string GetTopicText(string topic = "calm")
	{
		string key = this.source.idText.IsEmpty(this.id);
		if (this.id == "littleOne" && EClass._zone is Zone_LittleGarden)
		{
			key = "littleOne2";
		}
		SourceCharaText.Row row = EClass.sources.charaText.map.TryGetValue(key, null);
		if (row == null)
		{
			return null;
		}
		string text = row.GetText(topic, true);
		if (text.IsEmpty())
		{
			return null;
		}
		if (text.StartsWith("@"))
		{
			row = EClass.sources.charaText.map.TryGetValue(text.Replace("@", ""), null);
			if (row == null)
			{
				return null;
			}
			text = row.GetText(topic, true);
			if (text.IsEmpty())
			{
				return null;
			}
		}
		return text.Split(Environment.NewLine.ToCharArray()).RandomItem<string>();
	}

	public unsafe string TalkTopic(string topic = "calm")
	{
		if (this.host == null && !this.IsInActiveZone)
		{
			return null;
		}
		if (!this.isSynced && (this.host == null || !this.host.isSynced) && topic != "dead")
		{
			return null;
		}
		if (this.IsPCParty)
		{
			int num = EClass.pc.party.members.Count - 1;
			if (!(topic == "calm"))
			{
				if (!(topic == "aggro"))
				{
					if (!(topic == "kill"))
					{
						if (topic == "fov")
						{
							return null;
						}
					}
					else if (EClass.rnd(num * 3) != 0)
					{
						return null;
					}
				}
				else if (EClass.rnd(num * 10) != 0)
				{
					return null;
				}
			}
			else if (EClass.rnd(num * 5) != 0)
			{
				return null;
			}
		}
		string text = this.GetTopicText(topic);
		if (text.IsEmpty())
		{
			return null;
		}
		string text2 = "_bracketTalk".lang();
		bool flag = text.StartsWith("*");
		bool flag2 = text.StartsWith("(");
		bool flag3 = text.StartsWith(text2) || (text.Length > 0 && text[0] == text2[0]) || text[0] == '“';
		text = base.ApplyTone(text, false);
		text = text.Replace("~", "*");
		Msg.SetColor(flag2 ? Msg.colors.Thinking : (flag3 ? Msg.colors.Talk : Msg.colors.Ono));
		Msg.Say(text.Replace("&", ""));
		if (topic == "dead")
		{
			EClass.ui.popGame.PopText(base.ApplyNewLine(text.StripBrackets()), null, "PopTextDead", default(Color), *this.pos.Position() + EClass.setting.render.tc.textPosDead, 0f);
		}
		else if (flag || flag3 || flag2)
		{
			(this.host ?? this).renderer.Say(base.ApplyNewLine(text.StripBrackets()), default(Color), this.IsPCParty ? 0.6f : 0f);
		}
		return text;
	}

	public override Sprite GetImageSprite()
	{
		return this.GetSprite(0);
	}

	public void ChangeMemberType(FactionMemberType type)
	{
		this.memberType = type;
	}

	public void ShowDialog()
	{
		Zone_Nymelle zone_Nymelle = EClass._zone as Zone_Nymelle;
		if (this.IsDeadOrSleeping)
		{
			this.ShowDialog("_chara", "sleep", "");
			return;
		}
		if (base.isRestrained)
		{
			this.ShowDialog("_chara", "strain", "");
			return;
		}
		if (EClass.pc.isHidden && !this.CanSee(EClass.pc))
		{
			this.ShowDialog("_chara", "invisible", "");
			return;
		}
		if (this.IsEscorted())
		{
			this.ShowDialog("_chara", "escort", "");
			return;
		}
		if (EClass._zone is Zone_Music)
		{
			this.ShowDialog("_chara", "party", "");
			return;
		}
		if (LayerDrama.forceJump == null && EClass.game.quests.OnShowDialog(this))
		{
			return;
		}
		string id = this.id;
		if (!(id == "loytel"))
		{
			if (!(id == "farris"))
			{
				if (!(id == "ashland"))
				{
					if (!(id == "fiama"))
					{
						if (id == "big_sister")
						{
							if (EClass.player.flags.little_saved)
							{
								this.ShowDialog("big_sister", "little_saved", "");
								EClass.player.flags.little_saved = false;
								return;
							}
							if (EClass.player.flags.little_killed)
							{
								this.ShowDialog("big_sister", "little_dead", "");
								EClass.player.flags.little_killed = false;
								return;
							}
						}
					}
					else
					{
						if (zone_Nymelle != null && zone_Nymelle.IsCrystalLv)
						{
							SoundManager.ForceBGM();
							LayerDrama.ActivateMain("mono", "nymelle_crystal", null, null, "");
							return;
						}
						if (!EClass.player.EnableDreamStory)
						{
							this.ShowDialog("fiama", "main", "");
							return;
						}
						if (!EClass.player.flags.fiamaFirstDream && EClass.player.flags.storyFiama >= 10)
						{
							EClass.player.flags.fiamaFirstDream = true;
							this.ShowDialog("fiama", "firstDream", "");
							return;
						}
						if (!EClass.player.flags.fiamaStoryBookGiven && EClass.player.flags.storyFiama >= 30)
						{
							this.ShowDialog("fiama", "giveStoryBook", "").SetOnKill(delegate
							{
								EClass.player.flags.fiamaStoryBookGiven = true;
								EClass.player.DropReward(ThingGen.Create("book_story", -1, -1), false);
							});
							return;
						}
						this.ShowDialog("fiama", "main", "");
						return;
					}
				}
				else
				{
					if (zone_Nymelle != null && zone_Nymelle.IsCrystalLv)
					{
						SoundManager.ForceBGM();
						LayerDrama.ActivateMain("mono", "nymelle_crystal", null, null, "");
						return;
					}
					this.ShowDialog("ashland", "main", "");
					return;
				}
			}
			else
			{
				if (EClass._zone.id == "startVillage" || EClass._zone.id == "startVillage3")
				{
					this.ShowDialog("_chara", "main", "");
					return;
				}
				int phase = EClass.game.quests.GetPhase<QuestExploration>();
				if (phase == -1)
				{
					this.ShowDialog("farris", "nymelle_noQuest", "");
					return;
				}
				if (phase == 0)
				{
					this.ShowDialog("farris", "nymelle_first", "");
					return;
				}
				if (phase == 1)
				{
					this.ShowDialog("farris", "home_first", "");
					return;
				}
				this.ShowDialog("_chara", "main", "");
				return;
			}
		}
		else
		{
			if (EClass.player.flags.loytelEscaped)
			{
				EClass.game.quests.Get("pre_debt_runaway").Complete();
				EClass.player.flags.loytelEscaped = false;
				EClass.game.quests.Add("debt", "loytel");
				this.ShowDialog("loytel", "loytelEscaped", "");
				return;
			}
			QuestDebt questDebt = EClass.game.quests.Get<QuestDebt>();
			if (questDebt != null && questDebt.paid)
			{
				questDebt.stage++;
				if (questDebt.stage > 6)
				{
					questDebt.stage = 1;
				}
				this.ShowDialog("loytel", "debt" + questDebt.stage.ToString(), "");
				return;
			}
		}
		if (this.trait is TraitGuildDoorman)
		{
			string tag = (this.trait is TraitDoorman_Fighter) ? "fighter" : ((this.trait is TraitDoorman_Mage) ? "mage" : "thief");
			this.ShowDialog("guild_doorman", "main", tag);
			return;
		}
		if (this.trait is TraitGuildClerk)
		{
			string tag2 = (this.trait is TraitClerk_Fighter) ? "fighter" : ((this.trait is TraitClerk_Mage) ? "mage" : "thief");
			this.ShowDialog("guild_clerk", "main", tag2);
			return;
		}
		if (File.Exists(CorePath.DramaData + this.id + ".xlsx"))
		{
			this.ShowDialog(this.id, "main", "");
			return;
		}
		this.ShowDialog("_chara", "main", "");
	}

	public LayerDrama ShowDialog(string book, string step = "main", string tag = "")
	{
		return this._ShowDialog(book, null, step, tag);
	}

	private LayerDrama _ShowDialog(string book, string sheet, string step = "main", string tag = "")
	{
		EClass.Sound.Play("pop_drama");
		if (book == "_chara" && this.IsPC)
		{
			step = "pc";
		}
		return LayerDrama.Activate(book, sheet, step, this, null, tag);
	}

	public Point GetDestination()
	{
		return (this.ai.IsRunning ? this.ai.GetDestination() : this.pos).Copy();
	}

	public int GetHireCost()
	{
		return base.LV / 2 + 4;
	}

	public int GetHappiness()
	{
		int num = 50;
		if (this.FindBed() != null)
		{
			num += 50;
		}
		return num;
	}

	public string GetTextHappiness()
	{
		return this.GetHappiness().ToString() ?? "";
	}

	public string GetActionText()
	{
		string result = "?????";
		if (this.ai != null)
		{
			result = this.ai.GetCurrentActionText();
		}
		return result;
	}

	public override void WriteNote(UINote n, Action<UINote> onWriteNote = null, IInspect.NoteMode mode = IInspect.NoteMode.Default, Recipe recipe = null)
	{
		n.Clear();
		UIItem uiitem = n.AddHeaderCard(base.Name.ToTitleCase(false), null);
		this.SetImage(uiitem.image2);
		uiitem.text2.SetText(this.race.GetText("name", false).ToTitleCase(true) + " " + this.job.GetText("name", false).ToTitleCase(true));
		n.AddText("", FontColor.DontChange);
		n.Build();
	}

	public override void SetSortVal(UIList.SortMode m, CurrencyType currency = CurrencyType.Money)
	{
		if (m <= UIList.SortMode.ByRace)
		{
			if (m == UIList.SortMode.ByJob)
			{
				this.sortVal = this.job._index * 10000 + this.sourceCard._index;
				return;
			}
			if (m == UIList.SortMode.ByRace)
			{
				this.sortVal = this.race._index * 10000 * (this.IsHuman ? -1 : 1) + this.sourceCard._index;
				return;
			}
		}
		else
		{
			if (m == UIList.SortMode.ByWorkk)
			{
				return;
			}
			if (m == UIList.SortMode.ByFeat)
			{
				this.sortVal = -this.GetTotalFeat();
				return;
			}
		}
		this.sortVal = this.sourceCard._index * (this.IsHuman ? -1 : 1);
	}

	public void ClearBed(Map map = null)
	{
		if (map == null)
		{
			map = EClass._map;
		}
		foreach (Card card in map.props.installed.all.Values)
		{
			TraitBed traitBed = card.trait as TraitBed;
			if (traitBed != null && traitBed.IsHolder(this))
			{
				traitBed.RemoveHolder(this);
			}
		}
	}

	public TraitBed FindBed()
	{
		foreach (Card card in EClass._map.props.installed.all.Values)
		{
			TraitBed traitBed = card.trait as TraitBed;
			if (traitBed != null && traitBed.IsHolder(this))
			{
				return traitBed;
			}
		}
		return null;
	}

	public TraitBed TryAssignBed()
	{
		if (this.memberType == FactionMemberType.Livestock || (!this.IsPCFaction && !this.IsGuest()))
		{
			return null;
		}
		foreach (Card card in EClass._map.props.installed.all.Values)
		{
			TraitBed traitBed = card.trait as TraitBed;
			if (traitBed != null && traitBed.CanAssign(this))
			{
				traitBed.AddHolder(this);
				Msg.Say("claimBed", this, null, null, null);
				return traitBed;
			}
		}
		return null;
	}

	public void TryPutSharedItems(IEnumerable<Thing> containers, bool msg = true)
	{
		if (base.GetInt(113, null) != 0)
		{
			return;
		}
		Chara._ListItems.Clear();
		Thing bestRangedWeapon = this.GetBestRangedWeapon();
		foreach (Thing thing in this.things)
		{
			if (!thing.IsAmmo)
			{
				if (thing.category.slot != 0 && !thing.isEquipped && !thing.HasTag(CTAG.gift))
				{
					Chara._ListItems.Add(thing);
				}
				else if (thing.IsRangedWeapon && thing.category.slot == 0 && bestRangedWeapon != thing)
				{
					Chara._ListItems.Add(thing);
				}
			}
		}
		if (Chara._ListItems.Count == 0)
		{
			return;
		}
		List<Thing> containers2 = containers.ToList<Thing>();
		foreach (Thing t in Chara._ListItems)
		{
			EClass._zone.TryAddThingInSharedContainer(t, containers2, true, true, this, true);
		}
	}

	public void TryPutSharedItems(bool msg = true)
	{
		this.TryPutSharedItems(EClass._map.props.installed.containers, true);
	}

	public void TryTakeSharedItems(bool msg = true)
	{
		this.TryTakeSharedItems(EClass._map.props.installed.containers, true, true);
	}

	public void TryTakeSharedItems(IEnumerable<Thing> containers, bool msg = true, bool shouldEat = true)
	{
		if (base.isSummon)
		{
			return;
		}
		int num = 2;
		int num2 = 2;
		bool flag = base.GetInt(113, null) == 0;
		int num3 = 2;
		int num4 = 2;
		int num5 = 2;
		foreach (Thing thing in this.things)
		{
			if (this.CanEat(thing, shouldEat))
			{
				num -= thing.Num;
			}
			if (thing.trait.GetHealAction(this) != null)
			{
				num2 -= thing.Num;
			}
			if (thing.id == "polish_powder")
			{
				num3 -= thing.Num;
			}
			if (thing.trait is TraitBlanketColdproof)
			{
				num4 -= thing.Num;
			}
			if (thing.trait is TraitBlanketFireproof)
			{
				num5 -= thing.Num;
			}
		}
		Chara._ListItems.Clear();
		foreach (Thing thing2 in containers)
		{
			if (thing2.IsSharedContainer)
			{
				foreach (Thing thing3 in thing2.things)
				{
					if (!thing3.c_isImportant)
					{
						if (num3 > 0 && thing3.id == "polish_powder")
						{
							Chara._ListItems.Add(thing3);
							num3 -= thing3.Num;
						}
						else if (num4 > 0 && thing3.trait is TraitBlanketColdproof)
						{
							Chara._ListItems.Add(thing3);
							num4 -= thing3.Num;
						}
						else if (num5 > 0 && thing3.trait is TraitBlanketFireproof)
						{
							Chara._ListItems.Add(thing3);
							num5 -= thing3.Num;
						}
						else if (num > 0 && this.CanEat(thing3, shouldEat))
						{
							Chara._ListItems.Add(thing3);
							num -= thing3.Num;
						}
						else if (num2 > 0 && thing3.trait.GetHealAction(this) != null)
						{
							Chara._ListItems.Add(thing3);
							num2 -= thing3.Num;
						}
						else if (flag && thing3.IsEquipmentOrRanged && !thing3.HasTag(CTAG.gift) && this.ShouldEquip(thing3, true))
						{
							Chara._ListItems.Add(thing3);
						}
					}
				}
			}
		}
		if (Chara._ListItems.Count == 0)
		{
			return;
		}
		Chara._ListItems.ForeachReverse(delegate(Thing t)
		{
			if (!t.IsEquipmentOrRanged)
			{
				return;
			}
			bool flag3 = false;
			int slot = t.category.slot;
			int equipValue = t.GetEquipValue();
			foreach (Thing thing7 in Chara._ListItems)
			{
				if (thing7.category.slot == slot && thing7.GetEquipValue() > equipValue)
				{
					flag3 = true;
					break;
				}
			}
			if (flag3)
			{
				Chara._ListItems.Remove(t);
			}
		});
		bool flag2 = false;
		foreach (Thing thing4 in Chara._ListItems)
		{
			Thing thing5 = thing4;
			if (this.things.IsFull(thing5, true, true))
			{
				break;
			}
			Thing thing6 = thing4.parent as Thing;
			if (thing5.Num > 2)
			{
				thing5 = thing5.Split(2);
			}
			if (msg)
			{
				base.Say("takeSharedItem", this, thing5, thing6.GetName(NameStyle.Full, -1), null);
			}
			base.AddCard(thing5);
			if (this.ShouldEquip(thing5, true) && thing5.category.slot != 0)
			{
				this.TryEquip(thing5, true);
				flag2 = true;
			}
		}
		if (flag2 && flag)
		{
			this.TryPutSharedItems(containers, true);
		}
	}

	public void InstantEat(Thing t = null, bool sound = true)
	{
		if (t == null)
		{
			t = this.things.Find((Thing a) => this.CanEat(a, true), true);
		}
		if (t == null)
		{
			t = this.things.Find((Thing a) => this.CanEat(a, false), true);
		}
		if (t == null)
		{
			return;
		}
		base.Say("eat_start", this, t.Duplicate(1), null, null);
		if (sound)
		{
			base.PlaySound("eat", 1f, true);
		}
		FoodEffect.Proc(this, t);
		t.ModNum(-1, true);
	}

	public bool CanEat(Thing t, bool shouldEat = false)
	{
		return (!t.IsDecayed || base.HasElement(480, 1)) && (!shouldEat || t.trait is TraitFoodPrepared) && (!t.IsNegativeGift && !t.HasTag(CTAG.ignoreUse) && !t.isEquipped) && t.trait.CanEat(this);
	}

	public bool ShouldEquip(Thing t, bool useFav = false)
	{
		if (t.IsRangedWeapon && t.category.slot == 0)
		{
			if (!this.CanEquipRanged(t))
			{
				return false;
			}
			int num = 0;
			foreach (Thing thing in this.things)
			{
				if (thing.IsRangedWeapon)
				{
					if (thing.category.slot != 0 && thing.isEquipped)
					{
						return false;
					}
					if (this.CanEquipRanged(thing) && thing.GetEquipValue() > num)
					{
						num = thing.GetEquipValue();
					}
				}
			}
			return t.GetEquipValue() > num;
		}
		else
		{
			BodySlot bodySlot = this.body.GetSlot(t, false, false);
			if (bodySlot == null)
			{
				return false;
			}
			if (useFav)
			{
				switch (this.GetFavAttackStyle())
				{
				case AttackStyle.Default:
				case AttackStyle.TwoHand:
					if (t.IsMeleeWeapon)
					{
						bodySlot = this.body.slotMainHand;
					}
					else if (bodySlot.elementId == 35)
					{
						return false;
					}
					break;
				case AttackStyle.TwoWield:
					if (bodySlot.elementId == 35 && !t.IsMeleeWeapon)
					{
						return false;
					}
					break;
				case AttackStyle.Shield:
					if (t.IsMeleeWeapon)
					{
						bodySlot = this.body.slotMainHand;
					}
					else if (bodySlot.elementId == 35 && t.IsMeleeWeapon)
					{
						return false;
					}
					break;
				}
			}
			return this.body.IsEquippable(t, bodySlot, false) && (bodySlot.thing == null || (bodySlot.thing.blessedState > BlessedState.Cursed && bodySlot.thing.GetEquipValue() < t.GetEquipValue())) && !t.HasTag(CTAG.gift);
		}
	}

	public bool TryEquip(Thing t, bool useFav = false)
	{
		if (!this.ShouldEquip(t, useFav))
		{
			return false;
		}
		if (t.category.slot == 0)
		{
			return false;
		}
		if (useFav)
		{
			BodySlot slot = this.body.GetSlot(t, false, false);
			AttackStyle favAttackStyle = this.GetFavAttackStyle();
			if (favAttackStyle > AttackStyle.TwoHand)
			{
				if (favAttackStyle == AttackStyle.Shield)
				{
					if (t.IsMeleeWeapon)
					{
						slot = this.body.slotMainHand;
					}
				}
			}
			else if (t.IsMeleeWeapon)
			{
				slot = this.body.slotMainHand;
			}
			this.body.Equip(t, slot, true);
		}
		else
		{
			this.body.Equip(t, null, true);
		}
		base.Say("equip", this, t, null, null);
		return true;
	}

	public bool CanEquipRanged(Thing t)
	{
		return !this.body.IsTooHeavyToEquip(t);
	}

	public Thing TryGetThrowable()
	{
		Chara.<>c__DisplayClass438_0 CS$<>8__locals1 = new Chara.<>c__DisplayClass438_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.dest = null;
		if (!this.IsPC)
		{
			this.things.Foreach(delegate(Thing t)
			{
				if (t.HasTag(CTAG.throwWeapon) || (!CS$<>8__locals1.<>4__this.IsPCFactionOrMinion && t.HasTag(CTAG.throwWeaponEnemy)))
				{
					CS$<>8__locals1.dest = t;
					return;
				}
			}, true);
			return CS$<>8__locals1.dest;
		}
		if (EClass.game.config.autoCombat.bUseHotBar)
		{
			return CS$<>8__locals1.<TryGetThrowable>g__FindThrowable|0(true);
		}
		return null;
	}

	public Thing FindAmmo(Thing weapon)
	{
		TraitToolRange ranged = weapon.trait as TraitToolRange;
		Thing thing = this.IsPC ? EClass.pc.things.Find<TraitQuiver>() : null;
		if (thing != null)
		{
			Thing thing2 = thing.things.Find((Thing t) => ranged.IsAmmo(t), true);
			if (thing2 != null)
			{
				return thing2;
			}
		}
		return this.things.Find((Thing t) => ranged.IsAmmo(t), true);
	}

	public Thing GetBestRangedWeapon()
	{
		Thing result = null;
		int num = 0;
		foreach (Thing thing in this.things)
		{
			if (thing.IsRangedWeapon && this.CanEquipRanged(thing) && thing.GetEquipValue() > num)
			{
				num = thing.GetEquipValue();
				result = thing;
			}
		}
		return result;
	}

	public bool TryEquipRanged()
	{
		if (this.IsPC)
		{
			Thing thing = EClass.player.currentHotItem.Thing;
			if (((thing != null) ? thing.trait : null) is TraitToolRange && this.CanEquipRanged(thing))
			{
				this.ranged = thing;
				return true;
			}
			return false;
		}
		else
		{
			if (this.ranged != null && this.ranged.parent == this)
			{
				return true;
			}
			this.ranged = this.GetBestRangedWeapon();
			return this.ranged != null;
		}
	}

	public override int GetArmorSkill()
	{
		if (this.body.GetWeight(true) <= 30000)
		{
			return 120;
		}
		return 122;
	}

	public bool TryUse(Thing t)
	{
		if (t.id == "338")
		{
			Thing thing = this.things.Find((Thing a) => a.IsEquipmentOrRanged && !a.isAcidproof, true);
			if (thing != null)
			{
				base.Say("dip", this, thing, t.GetName(NameStyle.Full, 1), null);
				SE.Change();
				t.trait.OnBlend(thing, this);
				return true;
			}
			return false;
		}
		else
		{
			if (t.IsNegativeGift || t.source.HasTag(CTAG.ignoreUse))
			{
				return false;
			}
			if (t.trait.CanEat(this) && this.hunger.GetPhase() > ((this.IsPCFaction || this.IsPCFactionMinion) ? 2 : 0))
			{
				this.SetAIImmediate(new AI_Eat
				{
					target = t
				});
				return true;
			}
			if (t.trait.CanDrink(this))
			{
				this.Drink(t);
				return true;
			}
			if (t.trait.CanRead(this))
			{
				this.SetAIImmediate(new AI_Read
				{
					target = t
				});
				return true;
			}
			return false;
		}
	}

	public Room FindRoom()
	{
		TraitBed traitBed = this.FindBed();
		if (traitBed == null)
		{
			return null;
		}
		return traitBed.owner.pos.cell.room;
	}

	public void ModAffinity(Chara c, int a, bool show = true)
	{
		if (c == this)
		{
			return;
		}
		if (this.IsPC)
		{
			c.ModAffinity(EClass.pc, a, show);
			return;
		}
		if (c.IsPC)
		{
			a = this.affinity.Mod(a);
		}
		bool flag = a > 0;
		if (show)
		{
			if (a == 0)
			{
				base.Say("affinityNone", this, c, null, null);
				return;
			}
			base.ShowEmo(flag ? Emo.love : Emo.angry, 0f, true);
			c.ShowEmo(flag ? Emo.love : Emo.sad, 0f, true);
			base.Say(flag ? "affinityPlus" : "affinityMinus", this, c, null, null);
		}
	}

	public bool TryIdentify(Thing t, int count = 1, bool show = true)
	{
		int num = base.Evalue(289);
		if (num == 0)
		{
			return false;
		}
		int lv = t.LV;
		if (EClass.rnd(num * num + 5) > EClass.rnd(lv * lv) * 20)
		{
			t.Identify(show, (num >= 20) ? IDTSource.SkillHigh : IDTSource.Skill);
			int num2 = 50;
			if (lv > num)
			{
				num2 += (lv - num) * 10;
			}
			this.elements.ModExp(289, Mathf.Min(num2, 1000), false);
			return true;
		}
		return false;
	}

	public Chara CreateReplacement()
	{
		Chara chara = CharaGen.Create(this.id, -1);
		chara.c_originalHostility = base.c_originalHostility;
		if (chara.c_originalHostility != (Hostility)0)
		{
			chara.hostility = chara.c_originalHostility;
		}
		if (this.orgPos != null)
		{
			chara.orgPos = this.orgPos.Copy();
		}
		chara.pos = this.pos.Copy();
		chara.isImported = true;
		chara.c_editorTags = base.c_editorTags;
		chara.c_editorTraitVal = base.c_editorTraitVal;
		chara.c_idTrait = base.c_idTrait;
		chara.homeZone = this.homeZone;
		return chara;
	}

	public SourceThing.Row GetFavFood()
	{
		if (Chara._listFavFood.Count == 0)
		{
			foreach (SourceThing.Row row in EClass.sources.things.rows)
			{
				if (row._origin == "dish" && row.value != 0)
				{
					Chara._listFavFood.Add(row);
				}
			}
		}
		SourceThing.Row r = null;
		Rand.UseSeed(base.uid + EClass.game.seed, delegate
		{
			r = Chara._listFavFood.RandomItem<SourceThing.Row>();
		});
		return r;
	}

	public SourceCategory.Row GetFavCat()
	{
		SourceCategory.Row r = null;
		if (Chara._listFavCat.Count == 0)
		{
			foreach (SourceCategory.Row row in EClass.sources.categories.rows)
			{
				if (row.gift > 0)
				{
					Chara._listFavCat.Add(row);
				}
			}
		}
		Rand.UseSeed(base.uid + EClass.game.seed, delegate
		{
			r = Chara._listFavCat.RandomItem<SourceCategory.Row>();
		});
		return r;
	}

	public int GetTotalFeat()
	{
		int num = 0;
		if (base.c_upgrades != null)
		{
			num += base.c_upgrades.spent;
		}
		if (base.c_genes != null)
		{
			num += base.c_genes.GetTotalCost();
		}
		return num + base.feat;
	}

	public bool CanInsult()
	{
		using (List<ActList.Item>.Enumerator enumerator = this.ability.list.items.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.act.id == 6630)
				{
					return true;
				}
			}
		}
		return false;
	}

	public string GetIdPortraitCat()
	{
		string id = this.race.id;
		if ((id == "mifu" || id == "nefu") && EClass.rnd(2) == 0)
		{
			return "foxfolk";
		}
		if (this.trait is TraitGuard)
		{
			return "guard";
		}
		return "";
	}

	public string GetIdPortrait()
	{
		if (this.id == "olderyoungersister")
		{
			if (base.idSkin != 2)
			{
				return "UN_olderyoungersister";
			}
			return "UN_olderyoungersister_alt";
		}
		else
		{
			if (Portrait.allIds.Contains("UN_" + this.id + ".png"))
			{
				return "UN_" + this.id;
			}
			return base.c_idPortrait;
		}
	}

	public Thing GiveBirth(Thing t, bool effect)
	{
		EClass.player.forceTalk = true;
		base.Talk("giveBirth", null, null, false);
		EClass._zone.TryAddThing(t, this.pos, false);
		if (effect)
		{
			base.PlayEffect("revive", true, 0f, default(Vector3));
			base.PlaySound("egg", 1f, true);
			base.PlayAnime(AnimeID.Shiver, false);
			this.AddCondition<ConDim>(200, false);
		}
		return t;
	}

	public Thing MakeGene(DNA.Type? type = null)
	{
		return DNA.GenerateGene(this, type);
	}

	public Thing MakeBraineCell()
	{
		return DNA.GenerateGene(this, new DNA.Type?(DNA.Type.Brain));
	}

	public Thing MakeMilk(bool effect = true, int num = 1, bool addToZone = true)
	{
		Thing thing = ThingGen.Create("milk", -1, -1).SetNum(num);
		thing.MakeRefFrom(this, null);
		int num2 = base.LV - this.source.LV;
		if (EClass._zone.IsUserZone && !base.IsPCFactionOrMinion)
		{
			num2 = 0;
		}
		if (num2 >= 10)
		{
			thing.SetEncLv(num2 / 10);
		}
		if (!addToZone)
		{
			return thing;
		}
		return this.GiveBirth(thing, effect);
	}

	public Thing MakeEgg(bool effect = true, int num = 1, bool addToZone = true)
	{
		Thing thing = ThingGen.Create((EClass.rnd(EClass.debug.enable ? 1 : 20) == 0) ? "egg_fertilized" : "_egg", -1, -1).SetNum(num);
		thing.MakeFoodFrom(this);
		thing.c_idMainElement = base.c_idMainElement;
		if (!addToZone)
		{
			return thing;
		}
		return this.GiveBirth(thing, effect);
	}

	public void OnInsulted()
	{
		if (this.isDead)
		{
			return;
		}
		if (base.HasElement(1231, 1))
		{
			base.Talk("insulted", null, null, false);
			this.AddCondition<ConEuphoric>(100, false);
			return;
		}
		if (EClass.rnd(20) == 0)
		{
			this.SetFeat(1231, 1, true);
		}
	}

	public bool IsValidGiftWeight(Card t, int num = -1)
	{
		int num2 = base.HasElement(1411, 1) ? 3 : 1;
		return this.GetBurden(t, num) < num2;
	}

	public bool CanAcceptItem(Card t, int num = -1)
	{
		return EClass.debug.enable || (this.IsValidGiftWeight(t, num) && !t.c_isImportant && ((!t.category.IsChildOf("furniture") && !t.category.IsChildOf("junk")) || base.HasElement(1411, 1)));
	}

	public bool CanAcceptGift(Chara c, Card t)
	{
		return !this.things.IsFull(0) && !t.c_isImportant && (t.id == "1084" || t.trait is TraitBookSecret || (!t.trait.CanOnlyCarry && t.trait.CanBeDestroyed && !t.trait.CanExtendBuild && t.rarity != Rarity.Artifact && !t.IsContainer));
	}

	public void GiveGift(Chara c, Thing t)
	{
		if (c.IsHostile() || c.IsDeadOrSleeping)
		{
			Msg.Say("affinityNone", c, this, null, null);
			return;
		}
		if (t.IsCursed && t.IsEquipmentOrRanged && c.HasElement(1414, 1))
		{
			bool flag = t.blessedState == BlessedState.Doomed;
			int num = 200 + t.LV * 3;
			if (t.rarity == Rarity.Legendary)
			{
				num *= 3;
			}
			if (t.rarity >= Rarity.Mythical)
			{
				num *= 5;
			}
			if (flag)
			{
				num *= 2;
			}
			EClass.pc.PlayEffect("identify", true, 0f, default(Vector3));
			EClass.pc.PlaySound("identify", 1f, true);
			c.PlayEffect("mutation", true, 0f, default(Vector3));
			c.Say("draw_curse", c, t, null, null);
			t.Destroy();
			List<Element> list = new List<Element>();
			foreach (Element element in EClass.pc.elements.dict.Values)
			{
				if (element is Spell)
				{
					list.Add(element);
				}
			}
			if (list.Count == 0)
			{
				EClass.pc.SayNothingHappans();
				return;
			}
			Element element2 = list.RandomItem<Element>();
			EClass.pc.ModExp(element2.id, num);
			EClass.pc.Say("draw_curse2", EClass.pc, element2.Name, null);
			return;
		}
		else
		{
			if ((t.id == "lovepotion" || t.id == "dreambug") && !Application.isEditor)
			{
				this.GiveLovePotion(c, t);
				return;
			}
			if (t.trait is TraitErohon && c.id == t.c_idRefName)
			{
				c.OnGiveErohon(t);
				return;
			}
			if (!(t.trait is TraitTicketMassage))
			{
				if (t.id == "flyer")
				{
					this.stamina.Mod(-1);
					if (c.things.Find((Thing a) => a.id == "flyer", true) != null)
					{
						c.Talk("flyer_miss", null, null, false);
						this.DoHostileAction(c, false);
						return;
					}
					if (EClass.rnd(20) != 0 && c.CHA > EClass.rnd(base.CHA + base.Evalue(291) * 3 + 10))
					{
						Msg.Say("affinityNone", c, this, null, null);
						t.Destroy();
						this.elements.ModExp(291, 10, false);
						return;
					}
					this.elements.ModExp(291, 50, false);
				}
				if (t.id == "statue_weird")
				{
					EClass.pc.Say("statue_sell", null, null);
				}
				t.isGifted = true;
				c.nextUse = c.affinity.OnGift(t);
				if (!t.isDestroyed)
				{
					EClass.game.quests.list.ForeachReverse(delegate(Quest q)
					{
						q.OnGiveItem(c, t);
					});
					if (c.TryEquip(t, false))
					{
						c.nextUse = null;
					}
				}
				return;
			}
			t.ModNum(-1, true);
			c.Talk("ticket", null, null, false);
			string id = t.id;
			if (id == "ticket_massage")
			{
				c.ModAffinity(EClass.pc, 10, true);
				EClass.pc.SetAI(new AI_Massage
				{
					target = c
				});
				return;
			}
			if (id == "ticket_armpillow")
			{
				c.ModAffinity(EClass.pc, 20, true);
				EClass.pc.AddCondition<ConSleep>(300, true);
				c.SetAI(new AI_ArmPillow
				{
					target = EClass.pc
				});
				return;
			}
			if (!(id == "ticket_champagne"))
			{
				return;
			}
			c.ModAffinity(EClass.pc, 10, true);
			c.AddCondition<ConChampagne>(100, false);
			return;
		}
	}

	public void OnGiveErohon(Thing t)
	{
		base.Say("give_erohon", this, null, null);
		this.AddCondition<ConParalyze>(50, true);
		this.AddCondition<ConConfuse>(50, true);
		this.AddCondition<ConFear>(1000, true);
		this.ModAffinity(EClass.pc, 100, true);
		t.Destroy();
		base.Talk("pervert", null, null, false);
	}

	public void GiveLovePotion(Chara c, Thing t)
	{
		c.Say("give_love", c, t, null, null);
		c.PlaySound(t.material.GetSoundDead(null), 1f, true);
		c.ShowEmo(Emo.angry, 0f, true);
		c.ModAffinity(EClass.pc, -20, false);
		c.Talk("pervert", null, null, false);
		t.Destroy();
	}

	public void RequestProtection(Chara attacker, Action<Chara> action)
	{
		if (this.HasCondition<StanceTaunt>() || base.isRestrained || attacker == this)
		{
			return;
		}
		if (this.host != null && this.host.isRestrained)
		{
			return;
		}
		if (this.IsPCFaction && attacker.IsPCFaction)
		{
			return;
		}
		bool flag = false;
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara != attacker && chara.enemy != this && chara != this && chara.host == null && !chara.IsDisabled && chara.IsFriendOrAbove(this) && chara.conSuspend == null && (!chara.IsPCParty || this.IsPCParty) && (!this.IsPCFaction || chara.IsPCFaction))
			{
				bool flag2 = chara.HasElement(1225, 1);
				if ((flag2 || (!flag && EClass.rnd(2) != 0 && chara.HasCondition<StanceTaunt>())) && !chara.HasCooldown(1225))
				{
					int num = Mathf.Max(chara.Evalue(1649), chara.IsPC ? 0 : (flag2 ? 3 : 1));
					int num2 = base.Dist(chara);
					if (num2 <= 25)
					{
						if (num2 > num || !chara.CanSeeLos(this.pos, num2))
						{
							if (!flag2)
							{
								continue;
							}
							if (base.Dist(chara) < 5)
							{
								chara.GoHostile(attacker);
								chara.SetEnemy(attacker);
								attacker.SetEnemy(chara);
								continue;
							}
							Point nearestPoint = this.pos.GetNearestPoint(false, false, true, true);
							if (!nearestPoint.IsValid)
							{
								continue;
							}
							chara.Teleport(nearestPoint, false, false);
							chara.AddCooldown(1225, 10);
							num2 = base.Dist(chara);
							base.Say("intercept_loyal", chara, this, null, null);
							chara.SetEnemy(attacker);
							attacker.SetEnemy(chara);
							if (num2 > num || !chara.CanSeeLos(this.pos, num2))
							{
								continue;
							}
						}
						if (!flag && !base.HasElement(1225, 1))
						{
							base.Say("intercept", chara, this, null, null);
							if (EClass.rnd(10) == 0)
							{
								chara.Talk("intercept", base.NameSimple, null, false);
							}
							if (attacker.enemy == this)
							{
								attacker.SetEnemy(chara);
							}
							action(chara);
							flag = true;
						}
					}
				}
			}
		}
	}

	public bool ShouldThrowAway(Thing t, ClearInventoryType type)
	{
		if (this.IsPCFaction)
		{
			if (t.IsFood && t.category.id != "fish" && !t.IsDecayed)
			{
				return false;
			}
			if (t.trait.GetHealAction(this) != null)
			{
				return false;
			}
			if (t.IsThrownWeapon || t.IsRangedWeapon || t.IsAmmo)
			{
				return false;
			}
			if (t.trait is TraitBlanket || t.trait is TraitItemProc || t.trait is TraitBookSkill)
			{
				return false;
			}
		}
		if (t.isEquipped || t.rarity >= Rarity.Legendary || !t.trait.CanBeDestroyed)
		{
			return false;
		}
		if (this.trait is TraitBard && t.trait is TraitToolMusic)
		{
			return false;
		}
		if (t.trait is TraitCurrency)
		{
			return false;
		}
		if (type != ClearInventoryType.Purge)
		{
			if (type == ClearInventoryType.SellAtTown && !t.isGifted && !t.isNPCProperty)
			{
				return false;
			}
		}
		else
		{
			string id = t.category.id;
			if (!(id == "fish"))
			{
				if (id == "junk" || id == "garbage")
				{
					if (EClass.rnd(3) != 0)
					{
						return true;
					}
				}
			}
			else if (EClass.rnd(3) == 0)
			{
				return true;
			}
			if (t.id == "flyer")
			{
				return true;
			}
			if (!t.IsDecayed && EClass.rnd(3) == 0)
			{
				return false;
			}
			if (t.IsRangedWeapon && !this.things.IsFull(0))
			{
				return false;
			}
		}
		return true;
	}

	public void ClearInventory(ClearInventoryType type)
	{
		int num = 0;
		for (int i = this.things.Count - 1; i >= 0; i--)
		{
			Thing thing = this.things[i];
			if (this.ShouldThrowAway(thing, type))
			{
				num += thing.GetPrice(CurrencyType.Money, true, PriceType.Default, this) * thing.Num;
				thing.Destroy();
			}
		}
		if (num > 0)
		{
			base.ModCurrency(num, "money");
			if (type == ClearInventoryType.SellAtTown)
			{
				Msg.Say("party_sell", this, num.ToString() ?? "", null, null);
				base.PlaySound("pay", 1f, true);
			}
		}
	}

	public void ResetUpgrade()
	{
		CharaUpgrade c_upgrades = base.c_upgrades;
	}

	public void TryUpgrade(bool msg = true)
	{
		if (!EClass.debug.enable)
		{
			return;
		}
		if (this.IsPC || !this.IsGlobal || !this.IsPCFaction)
		{
			return;
		}
		for (int i = 0; i < 100; i++)
		{
			if (base.feat == 0)
			{
				return;
			}
			if (base.c_upgrades == null)
			{
				base.c_upgrades = new CharaUpgrade();
			}
			if (base.c_upgrades.halt)
			{
				return;
			}
			Rand.SetSeed(base.uid + base.c_upgrades.count);
			int num = EClass.rnd(100);
			int num2 = 0;
			int num3 = 1;
			int num4 = 0;
			bool flag = false;
			IEnumerable<SourceElement.Row> ie = from a in EClass.sources.elements.rows
			where !this.elements.Has(a) && a.category == "skill" && !a.tag.Contains("noPet")
			select a;
			List<Element> list = this.ListAvailabeFeats(true);
			if (num >= 90 && list.Count > 0)
			{
				Element element = list.RandomItem<Element>();
				num2 = element.id;
				num4 = element.CostLearn;
			}
			else if (num >= 60 && ie.Any<SourceElement.Row>())
			{
				num2 = ie.RandomItem<SourceElement.Row>().id;
				num4 = 3;
			}
			else
			{
				num2 = Element.List_MainAttributesMajor.RandomItem<int>();
				num4 = 1;
				num3 = 2;
				flag = true;
			}
			Rand.SetSeed(-1);
			if (num4 > base.feat)
			{
				return;
			}
			base.feat -= num4;
			base.c_upgrades.count++;
			base.c_upgrades.spent += num4;
			bool flag2 = false;
			if (flag)
			{
				foreach (CharaUpgrade.Item item in base.c_upgrades.items)
				{
					if (item.idEle == num2)
					{
						item.value += num3;
						item.cost += num4;
						flag2 = true;
						break;
					}
				}
			}
			if (!flag2)
			{
				base.c_upgrades.items.Add(new CharaUpgrade.Item
				{
					idEle = num2,
					value = num3,
					cost = num4
				});
			}
			SourceElement.Row row = EClass.sources.elements.map[num2];
			if (row.category == "feat")
			{
				this.SetFeat(num2, this.elements.ValueWithoutLink(num2) + 1, true);
			}
			else if (this.elements.ValueWithoutLink(row.id) == 0)
			{
				this.elements.Learn(row.id, 1);
			}
			else
			{
				this.elements.ModBase(num2, num3);
			}
		}
	}

	public void AddCooldown(int idEle, int turns = 0)
	{
		if (this._cooldowns == null)
		{
			this._cooldowns = new List<int>();
		}
		if (turns != 0)
		{
			this._cooldowns.Add(idEle * 1000 + turns);
			return;
		}
		SourceElement.Row row = EClass.sources.elements.map[idEle];
		if (row.cooldown > 0)
		{
			this._cooldowns.Add(idEle * 1000 + row.cooldown);
		}
	}

	public bool HasCooldown(int idEle)
	{
		if (this._cooldowns != null)
		{
			for (int i = 0; i < this._cooldowns.Count; i++)
			{
				if (this._cooldowns[i] / 1000 == idEle)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void TickCooldown()
	{
		for (int i = this._cooldowns.Count - 1; i >= 0; i--)
		{
			if (this._cooldowns[i] % 1000 == 1)
			{
				this._cooldowns.RemoveAt(i);
			}
			else
			{
				List<int> cooldowns = this._cooldowns;
				int index = i;
				int num = cooldowns[index];
				cooldowns[index] = num - 1;
			}
		}
		if (this._cooldowns.Count == 0)
		{
			this._cooldowns = null;
		}
	}

	public void ChooseNewGoal()
	{
		if (this.IsPC && EClass.AdvMode)
		{
			this.SetNoGoal();
			return;
		}
		if (this.IsPCParty || base.noMove)
		{
			this.SetAI(new GoalIdle());
			return;
		}
		if ((this.IsHomeMember() && this.IsInHomeZone()) || this.IsGuest())
		{
			Goal goalFromTimeTable = this.GetGoalFromTimeTable(EClass.world.date.hour);
			if (goalFromTimeTable != null)
			{
				this.SetAI(goalFromTimeTable);
				if (goalFromTimeTable is GoalWork)
				{
					goalFromTimeTable.Tick();
				}
				return;
			}
		}
		if (this.goalList.index == -2)
		{
			this.goalList.Refresh(this, this.goalListType);
		}
		this.SetAI(this.goalList.Next());
	}

	public Goal GetGoalFromTimeTable(int hour)
	{
		if (this.IsPC)
		{
			return null;
		}
		switch (TimeTable.GetSpan(this.idTimeTable, hour))
		{
		case TimeTable.Span.Free:
			if (this.IsGuest())
			{
				return new GoalIdle();
			}
			return this.GetGoalHobby();
		case TimeTable.Span.Eat:
			return new GoalIdle();
		case TimeTable.Span.Work:
			if (this.IsGuest())
			{
				return new GoalIdle();
			}
			return this.GetGoalWork();
		case TimeTable.Span.Sleep:
			if (this.sleepiness.value > 10 || EClass._zone.isSimulating)
			{
				return new GoalSleep();
			}
			break;
		}
		return null;
	}

	public Goal GetGoalWork()
	{
		if (this.IsPrisoner)
		{
			return new GoalIdle();
		}
		if (this.memberType == FactionMemberType.Livestock)
		{
			return new GoalGraze();
		}
		return new GoalWork();
	}

	public Goal GetGoalHobby()
	{
		if (this.IsPrisoner)
		{
			return new GoalIdle();
		}
		if (this.memberType == FactionMemberType.Livestock)
		{
			return new GoalGraze();
		}
		return new GoalHobby();
	}

	public void SetAIIdle()
	{
	}

	public void SetAIAggro()
	{
		this.SetAI(new GoalCombat());
	}

	public AIAct SetNoGoal()
	{
		return this.SetAI(Chara._NoGoalPC);
	}

	public AIAct SetAI(AIAct g)
	{
		if (this.IsPC)
		{
			EClass.player.queues.OnSetGoal(g);
		}
		if (this.ai.IsRunning)
		{
			if (this.ai == g && this.ai.IsNoGoal)
			{
				return g;
			}
			this.ai.Cancel();
			if (this.ai == g)
			{
				string str = "goal is g:";
				AIAct aiact = this.ai;
				Debug.Log(str + ((aiact != null) ? aiact.ToString() : null) + "/" + ((this != null) ? this.ToString() : null));
				return g;
			}
		}
		if (this.HasCondition<ConWait>())
		{
			this.RemoveCondition<ConWait>();
		}
		this.ai = g;
		this.ai.SetOwner(this);
		if (this.IsPC)
		{
			this.renderer.RefreshStateIcon();
		}
		return g;
	}

	public void SetAIImmediate(AIAct g)
	{
		bool hasNoGoal = this.HasNoGoal;
		this.SetAI(g);
		if (EClass.scene.actionMode == ActionMode.Sim && EClass.scene.paused)
		{
			return;
		}
		if (hasNoGoal && !(this.renderer as CharaRenderer).IsMoving)
		{
			this.Tick();
		}
	}

	public List<Hobby> ListWorks(bool useMemberType = true)
	{
		Chara.listHobby.Clear();
		if (useMemberType && this.memberType == FactionMemberType.Livestock)
		{
			Chara.listHobby.Add(new Hobby
			{
				id = 45
			});
		}
		else
		{
			if (this._works == null)
			{
				this.RerollHobby(true);
			}
			foreach (int id in this._works)
			{
				Chara.listHobby.Add(new Hobby
				{
					id = id
				});
			}
		}
		return Chara.listHobby;
	}

	public List<Hobby> ListHobbies(bool useMemberType = true)
	{
		Chara.listHobby.Clear();
		if (!useMemberType || this.memberType != FactionMemberType.Livestock)
		{
			if (this._hobbies == null)
			{
				this.RerollHobby(true);
			}
			foreach (int id in this._hobbies)
			{
				Chara.listHobby.Add(new Hobby
				{
					id = id
				});
			}
		}
		return Chara.listHobby;
	}

	public Hobby GetWork(string id)
	{
		foreach (Hobby hobby in this.ListWorks(true))
		{
			if (hobby.source.alias == id)
			{
				return hobby;
			}
		}
		foreach (Hobby hobby2 in this.ListHobbies(true))
		{
			if (hobby2.source.alias == id)
			{
				return hobby2;
			}
		}
		return null;
	}

	public ElementContainer baseWorkElements
	{
		get
		{
			if (this._baseWorkElements == null)
			{
				this._baseWorkElements = new ElementContainer();
				foreach (Hobby h in this.ListHobbies(true))
				{
					this.<get_baseWorkElements>g__Build|494_0(h);
				}
				foreach (Hobby h2 in this.ListWorks(true))
				{
					this.<get_baseWorkElements>g__Build|494_0(h2);
				}
			}
			return this._baseWorkElements;
		}
	}

	public void RefreshWorkElements(ElementContainer parent = null)
	{
		if (this.workElements != null)
		{
			this.workElements.SetParent(null);
		}
		this.workElements = null;
		if (this.IsPCParty || this.homeBranch == null || this.homeBranch.owner == null)
		{
			return;
		}
		foreach (Hobby h in this.ListHobbies(true))
		{
			this.<RefreshWorkElements>g__TryAdd|495_0(h);
		}
		foreach (Hobby h2 in this.ListWorks(true))
		{
			this.<RefreshWorkElements>g__TryAdd|495_0(h2);
		}
		if (this.workElements != null)
		{
			this.workElements.SetParent(parent);
		}
	}

	public string GetTextHobby(bool simple = false)
	{
		string text = simple ? "" : ("hobby".lang() + ":");
		foreach (Hobby hobby in this.ListHobbies(true))
		{
			text = text + " " + hobby.Name.TagColor((hobby.GetEfficiency(this) > 0) ? FontColor.Good : FontColor.Warning, null) + ",";
		}
		return text.TrimEnd(',');
	}

	public string GetTextWork(bool simple = false)
	{
		string text = simple ? "" : ("work".lang() + ":");
		foreach (Hobby hobby in this.ListWorks(true))
		{
			text = text + " " + hobby.Name.TagColor((hobby.GetEfficiency(this) > 0) ? FontColor.Good : FontColor.Warning, null) + ",";
		}
		return text.TrimEnd(',');
	}

	public void AddHobby(int id)
	{
		using (List<int>.Enumerator enumerator = this._hobbies.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == id)
				{
					return;
				}
			}
		}
		this._hobbies.Add(id);
	}

	public void AddWork(int id)
	{
		using (List<int>.Enumerator enumerator = this._works.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == id)
				{
					return;
				}
			}
		}
		this._works.Add(id);
	}

	public void RerollHobby(bool extraSlotChance = true)
	{
		if (this._hobbies != null && this._works != null)
		{
			this._hobbies.Clear();
			this._works.Clear();
		}
		else
		{
			this._hobbies = new List<int>();
			this._works = new List<int>();
		}
		if (this.source.hobbies.IsEmpty())
		{
			this.AddHobby(EClass.sources.hobbies.listHobbies.RandomItem<SourceHobby.Row>().id);
		}
		else
		{
			foreach (string key in this.source.hobbies)
			{
				this.AddHobby(EClass.sources.hobbies.alias[key].id);
			}
		}
		if (this.source.works.IsEmpty())
		{
			this.AddWork(EClass.sources.hobbies.listWorks.RandomItem<SourceHobby.Row>().id);
		}
		else
		{
			foreach (string key2 in this.source.works)
			{
				this.AddWork(EClass.sources.hobbies.alias[key2].id);
			}
		}
		this.GetWorkSummary().Reset();
	}

	public WorkSummary GetWorkSummary()
	{
		if (this._workSummary == null)
		{
			this._workSummary = new WorkSummary();
		}
		return this._workSummary;
	}

	public void TickWork(VirtualDate date)
	{
		TimeTable.Span span = TimeTable.GetSpan(this.idTimeTable, date.hour);
		if (span != TimeTable.Span.Work && span != TimeTable.Span.Free)
		{
			return;
		}
		WorkSummary workSummary = this.GetWorkSummary();
		if (span == TimeTable.Span.Work)
		{
			if (workSummary.work != null)
			{
				this.PerformWork(workSummary.work, false, date.IsRealTime);
				return;
			}
		}
		else if (workSummary.hobbies.Count > 0)
		{
			this.PerformWork(workSummary.hobbies.RandomItem<WorkSession>(), true, date.IsRealTime);
		}
	}

	public bool TryWorkOutside(SourceHobby.Row sourceWork)
	{
		if (EClass.world.date.IsExpired(base.GetInt(51, null)))
		{
			Expedition expedition = Expedition.Create(this, sourceWork.expedition.ToEnum(true));
			base.SetInt(51, EClass.world.date.GetRaw(0) + 60 * (expedition.hours + 24));
			this.homeBranch.expeditions.Add(expedition);
			return true;
		}
		return false;
	}

	public void PerformWork(WorkSession session, bool isHobby = false, bool IsRealTime = false)
	{
		Chara.<>c__DisplayClass506_0 CS$<>8__locals1;
		CS$<>8__locals1.session = session;
		Hobby hobby = new Hobby();
		hobby.id = CS$<>8__locals1.session.id;
		WorkSummary workSummary = this._workSummary;
		hobby.GetAI(this).OnPerformWork(IsRealTime);
		if (!isHobby)
		{
			workSummary.progress += EClass.rnd(5) + 5;
		}
		int num = Chara.<PerformWork>g__PerformWork|506_0(hobby, 0, isHobby, ref CS$<>8__locals1);
		int num2 = Chara.<PerformWork>g__PerformWork|506_0(hobby, 1, isHobby, ref CS$<>8__locals1);
		int num3 = Chara.<PerformWork>g__PerformWork|506_0(hobby, 2, isHobby, ref CS$<>8__locals1);
		int num4 = Chara.<PerformWork>g__PerformWork|506_0(hobby, 3, isHobby, ref CS$<>8__locals1);
		workSummary.money += num;
		workSummary.food += num2;
		workSummary.knowledge += num3;
		workSummary.material += num4;
	}

	public void ValidateWorks()
	{
		Chara._goalWork.FindWork(this, false);
		Chara._goalHobby.ValidateHobby(this);
	}

	public Stats hunger
	{
		get
		{
			return Stats.Hunger.Set(this._cints, 10, this);
		}
	}

	public Stats burden
	{
		get
		{
			return Stats.Burden.Set(this._cints, 11, this);
		}
	}

	public Stats stamina
	{
		get
		{
			return Stats.Stamina.Set(this._cints, 12, this);
		}
	}

	public Stats depression
	{
		get
		{
			return Stats.Depression.Set(this._cints, 13, this);
		}
	}

	public Stats bladder
	{
		get
		{
			return Stats.Bladder.Set(this._cints, 14, this);
		}
	}

	public Stats hygiene
	{
		get
		{
			return Stats.Hygiene.Set(this._cints, 15, this);
		}
	}

	public Stats mana
	{
		get
		{
			return Stats.Mana.Set(this._cints, 16, this);
		}
	}

	public Stats sleepiness
	{
		get
		{
			return Stats.Sleepiness.Set(this._cints, 17, this);
		}
	}

	public Stats SAN
	{
		get
		{
			return Stats.SAN.Set(this._cints, 17, this);
		}
	}

	public void InitStats(bool onDeserialize = false)
	{
		if (!onDeserialize)
		{
			this._cints[10] = 20;
			this._cints[11] = 70;
			this._cints[13] = 70;
			this._cints[14] = 70;
			this._cints[15] = 70;
			this._cints[17] = 0;
		}
		foreach (Condition condition in this.conditions)
		{
			condition.SetOwner(this, onDeserialize);
		}
	}

	public Condition AddCondition<T>(int p = 100, bool force = false) where T : Condition
	{
		return this.AddCondition(typeof(T).Name, p, force);
	}

	public Condition AddCondition(string id, int p = 100, bool force = false)
	{
		return this.AddCondition(Condition.Create(id, p, null), force);
	}

	public Condition AddCondition(Condition c, bool force = false)
	{
		c.owner = this;
		if (!(c is ConBurning))
		{
			if (c is ConBleed)
			{
				if (base.ResistLv(964) >= 3)
				{
					return null;
				}
			}
		}
		else if (base.ResistLv(950) >= 3)
		{
			return null;
		}
		if (c.GainResistFactor > 0 && this.CanGainConResist)
		{
			if (c.GainResistFactor >= 400)
			{
				c.power /= 2;
			}
			this.ResistCon(c);
			if (c.power <= 0)
			{
				return null;
			}
		}
		if (!force)
		{
			if (c.source.negate.Length != 0)
			{
				foreach (string id in c.source.negate)
				{
					if (base.HasElement(id, 1))
					{
						return null;
					}
				}
			}
			Element defenseAttribute = c.GetDefenseAttribute(this);
			if (defenseAttribute != null)
			{
				c.power = 100 * c.power / (100 + defenseAttribute.Value);
			}
			if (c.source.resistance.Length != 0)
			{
				int num = base.ResistLv(EClass.sources.elements.alias[c.source.resistance[0]].id);
				if (num > 0)
				{
					c.power /= num * num + 1;
					if (c.power <= 40)
					{
						return null;
					}
				}
			}
			c.power = c.EvaluatePower(c.power);
			if (c.power <= 0)
			{
				return null;
			}
		}
		for (int j = 0; j < this.conditions.Count; j++)
		{
			if (this.conditions[j].id == c.id)
			{
				if (c.Type == ConditionType.Stance || c.IsToggle)
				{
					this.conditions[j].Kill(false);
					return null;
				}
				if (this.conditions[j].CanStack(c))
				{
					if (this.conditions[j].WillOverride)
					{
						this.conditions[j].Kill(true);
						goto IL_243;
					}
					if (this.CanGainConResist)
					{
						this.AddResistCon(c);
					}
					this.conditions[j].OnStacked(c.power);
					this.conditions[j].OnStartOrStack();
					this.conditions[j].PlayEffect();
				}
				if (!this.conditions[j].AllowMultipleInstance)
				{
					return null;
				}
			}
			IL_243:;
		}
		using (List<Condition>.Enumerator enumerator = this.conditions.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.TryNullify(c))
				{
					return null;
				}
			}
		}
		int num2 = c.EvaluateTurn(c.power);
		if (num2 == 0)
		{
			return null;
		}
		c.value = num2;
		this.conditions.Add(c);
		if (this.CanGainConResist)
		{
			this.AddResistCon(c);
		}
		c.SetOwner(this, false);
		c.Start();
		this.SetDirtySpeed();
		if (c.ShouldRefresh)
		{
			this.Refresh(false);
		}
		if (this.IsPC && c.ConsumeTurn && !EClass.pc.isRestrained)
		{
			EClass.player.EndTurn(true);
		}
		if (c.SyncRide && (this.ride != null || this.parasite != null))
		{
			if (this.ride != null)
			{
				this.ride.AddCondition(Condition.Create(c.source.alias, c.power, null), false);
			}
			if (this.parasite != null)
			{
				this.parasite.AddCondition(Condition.Create(c.source.alias, c.power, null), false);
			}
		}
		return c;
	}

	public override bool HasCondition<T>()
	{
		for (int i = 0; i < this.conditions.Count; i++)
		{
			if (this.conditions[i] is T)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasCondition(string alias)
	{
		for (int i = 0; i < this.conditions.Count; i++)
		{
			if (this.conditions[i].source.alias == alias)
			{
				return true;
			}
		}
		return false;
	}

	public Element GetBuffStats(string alias)
	{
		return this.GetBuffStats(EClass.sources.elements.alias[alias].id);
	}

	public Element GetBuffStats(int ele)
	{
		for (int i = 0; i < this.conditions.Count; i++)
		{
			ConBuffStats conBuffStats = this.conditions[i] as ConBuffStats;
			if (conBuffStats != null && conBuffStats.refVal == ele)
			{
				return conBuffStats.elements.GetElement(ele);
			}
		}
		return null;
	}

	public void CureCondition<T>(int v = 99999) where T : Condition
	{
		T condition = this.GetCondition<T>();
		if (condition == null)
		{
			return;
		}
		ref T ptr = ref condition;
		ptr.value -= v;
		if (condition.value <= 0)
		{
			condition.Kill(false);
		}
	}

	public T GetCondition<T>() where T : Condition
	{
		for (int i = 0; i < this.conditions.Count; i++)
		{
			if (this.conditions[i] is T)
			{
				return this.conditions[i] as T;
			}
		}
		return default(T);
	}

	public void RemoveCondition<T>() where T : Condition
	{
		for (int i = this.conditions.Count - 1; i >= 0; i--)
		{
			if (this.conditions[i] is T)
			{
				this.conditions[i].Kill(false);
				return;
			}
		}
	}

	public void CureHost(CureType type, int p = 100, BlessedState state = BlessedState.Normal)
	{
		if (this.parasite != null)
		{
			this.parasite.Cure(type, p, state);
		}
		if (this.ride != null)
		{
			this.ride.Cure(type, p, state);
		}
		this.Cure(type, p, state);
	}

	public void Cure(CureType type, int p = 100, BlessedState state = BlessedState.Normal)
	{
		bool flag = state == BlessedState.Blessed;
		switch (type)
		{
		case CureType.Heal:
		case CureType.Prayer:
			this.CureCondition<ConFear>(99999);
			this.CureCondition<ConBlind>(2 * p / 100 + 5);
			this.CureCondition<ConPoison>(5 * p / 100 + 5);
			this.CureCondition<ConConfuse>(10 * p / 100 + 10);
			this.CureCondition<ConDim>(p / 100 + 5);
			this.CureCondition<ConBleed>(2 * p / 100 + 10);
			if (flag)
			{
				this.SAN.Mod(-5);
				return;
			}
			break;
		case CureType.HealComplete:
		case CureType.Death:
		case CureType.Jure:
		case CureType.Boss:
			this.CureTempElements(p * 100, true, true);
			for (int i = this.conditions.Count - 1; i >= 0; i--)
			{
				Condition condition = this.conditions[i];
				if (condition.Type == ConditionType.Bad || condition.Type == ConditionType.Debuff || condition.Type == ConditionType.Disease)
				{
					condition.Kill(false);
				}
				else if (type == CureType.Death && condition.isPerfume)
				{
					condition.Kill(false);
				}
			}
			this.CureCondition<ConWait>(99999);
			this.CureCondition<ConSleep>(99999);
			if (type == CureType.Death)
			{
				this.hunger.value = 30;
			}
			if (type == CureType.Jure)
			{
				this.SAN.Mod(-999);
				if (base.HasElement(1206, 1))
				{
					this.SetFeat(1206, 0, true);
				}
			}
			break;
		case CureType.Sleep:
		{
			for (int j = this.conditions.Count - 1; j >= 0; j--)
			{
				Condition condition2 = this.conditions[j];
				if (!(condition2 is ConSleep) && !(condition2 is ConFaint))
				{
					if (condition2.isPerfume)
					{
						condition2.Mod(-1, true);
					}
					else if (condition2.Type == ConditionType.Bad || condition2.Type == ConditionType.Debuff)
					{
						condition2.Kill(false);
					}
				}
			}
			this.CureCondition<ConWait>(99999);
			this.CureCondition<ConDisease>((EClass.rnd(20) + 10) * p / 100);
			bool flag2 = this.HasCondition<ConAnorexia>();
			base.c_vomit -= (flag2 ? 3 : 2) * p / 100;
			if (base.c_vomit < 0)
			{
				base.c_vomit = 0;
				if (flag2)
				{
					this.RemoveCondition<ConAnorexia>();
					return;
				}
			}
			break;
		}
		case CureType.CureBody:
			this.CureCondition<ConBlind>(5 * p / 100 + 15);
			this.CureCondition<ConPoison>(10 * p / 100 + 15);
			this.CureCondition<ConBleed>(5 * p / 100 + 20);
			this.CureTempElements(p, true, false);
			return;
		case CureType.CureMind:
			this.CureCondition<ConFear>(99999);
			this.CureCondition<ConDim>(3 * p / 100 + 10);
			this.CureTempElements(p, false, true);
			return;
		default:
			return;
		}
	}

	public bool TryNeckHunt(Chara TC, int power, bool harvest = false)
	{
		if (TC == null || TC.HasCondition<ConInvulnerable>() || TC.Evalue(1421) > 0 || !TC.ExistsOnMap)
		{
			return false;
		}
		if (TC.hp > TC.MaxHP * Mathf.Min(5 + (int)Mathf.Sqrt((float)power), harvest ? 35 : 25) / 100)
		{
			return false;
		}
		if (TC.HasElement(427, 1))
		{
			return false;
		}
		base.PlaySound("hit_finish", 1f, true);
		base.Say("finish", null, null);
		base.Say("finish2", this, TC, null, null);
		TC.DamageHP(TC.MaxHP, AttackSource.Finish, this);
		return false;
	}

	public bool CanGainConResist
	{
		get
		{
			return base.rarity >= Rarity.Legendary && !this.IsPCFaction;
		}
	}

	public void AddResistCon(Condition con)
	{
		if (con.power <= 0 || con.GainResistFactor <= 0)
		{
			return;
		}
		int id = con.id;
		if (this.resistCon == null)
		{
			this.resistCon = new Dictionary<int, int>();
		}
		if (this.resistCon.ContainsKey(id))
		{
			Dictionary<int, int> dictionary = this.resistCon;
			int key = id;
			dictionary[key] += con.power * con.GainResistFactor / 100;
			return;
		}
		this.resistCon[id] = con.power * con.GainResistFactor / 100;
	}

	public void ResistCon(Condition con)
	{
		if (con.power <= 0 || this.resistCon == null)
		{
			return;
		}
		int a = this.resistCon.TryGetValue(con.id, 0);
		if (1000 < EClass.rnd(a))
		{
			con.power = 0;
			return;
		}
		if (500 < EClass.rnd(a))
		{
			con.power /= 5;
			return;
		}
		if (200 < EClass.rnd(a))
		{
			con.power /= 2;
		}
	}

	public void Sleep(Thing bed = null, Thing pillow = null, bool pickup = false, ItemPosition posBed = null, ItemPosition posPillow = null)
	{
		this.AddCondition(Condition.Create<ConSleep>(100, delegate(ConSleep con)
		{
			con.pcSleep = 15;
			con.pcBed = bed;
			con.pcPillow = pillow;
			con.pickup = pickup;
			con.posBed = posBed;
			con.posPillow = posPillow;
		}), true);
	}

	public void OnSleep(Thing bed = null, int days = 1)
	{
		TraitPillow traitPillow = this.pos.FindThing<TraitPillow>();
		int num = (bed == null) ? 20 : bed.Power;
		if (traitPillow != null)
		{
			num += traitPillow.owner.Power / 2;
		}
		num += base.Evalue(750) * 5;
		this.OnSleep(num, days);
	}

	public void OnSleep(int power, int days = 1)
	{
		if (days < 1)
		{
			days = 1;
		}
		int num = power * days;
		if (this.stamina.value < 0)
		{
			this.stamina.Set(1);
		}
		this.HealHP(num, HealSource.None);
		this.stamina.Mod(10 + 25 * num / 100 * (100 + this.elements.GetFeatRef(1642, 0)) / 100);
		this.mana.Mod(num);
		if (this.IsPCFaction)
		{
			this.hunger.Mod(20);
		}
		this.sleepiness.Set(0);
		this.interest = 100;
		this.Cure(CureType.Sleep, power, BlessedState.Normal);
	}

	public void ModHeight(int a)
	{
		int num = this.bio.height;
		num = num * (100 + a) / 100 + ((a > 0) ? 1 : -1);
		if (num < 1)
		{
			num = 1;
		}
		if (num == this.bio.height)
		{
			return;
		}
		this.bio.height = num;
		base.Say((a > 0) ? "height_gain" : "height_lose", this, null, null);
	}

	public void ModWeight(int a, bool ignoreLimit = false)
	{
		if (a == 0)
		{
			return;
		}
		int num = this.bio.weight;
		int height = this.bio.height;
		int num2 = height * height * 18 / 25000;
		int num3 = height * height * 24 / 10000;
		if (!ignoreLimit && (num <= num2 || num >= num3))
		{
			return;
		}
		num = num * (100 + a) / 100 + ((a > 0) ? 1 : -1);
		if (num < 1)
		{
			num = 1;
		}
		if (num == this.bio.weight)
		{
			return;
		}
		this.bio.weight = num;
		base.Say((a > 0) ? "weight_gain" : "weight_lose", this, null, null);
	}

	public void ModCorruption(int a)
	{
		if (a > 0 && base.ResistLv(962) > 0 && EClass.rnd(base.ResistLv(962) + 1) != 0)
		{
			return;
		}
		int num = (this.corruption + a) / 100 - this.corruption / 100;
		int num2 = 0;
		while (num2 < Mathf.Abs(num) && this.MutateRandom((num > 0) ? 1 : -1, 100, true, BlessedState.Normal))
		{
			num2++;
		}
		this.corruption += a;
		int num3 = 0;
		foreach (Element element in this.elements.dict.Values)
		{
			if (element.source.category == "ether")
			{
				num3 += element.Value;
			}
		}
		if (num3 > 0 && this.IsPC)
		{
			Tutorial.Reserve("ether", null);
		}
		this.corruption = num3 * 100 + this.corruption % 100;
	}

	public List<Element> ListAvailabeFeats(bool pet = false)
	{
		List<Element> list = new List<Element>();
		foreach (SourceElement.Row row in from a in EClass.sources.elements.rows
		where a.@group == "FEAT" && a.cost[0] != -1 && !a.categorySub.IsEmpty()
		select a)
		{
			Feat feat = this.elements.GetOrCreateElement(row.id) as Feat;
			int num = (feat.ValueWithoutLink > 0) ? (feat.ValueWithoutLink + 1) : 1;
			if (num <= feat.source.max && !feat.HasTag("class") && !feat.HasTag("hidden") && !feat.HasTag("innate") && (!pet || !feat.HasTag("noPet")) && feat.IsAvailable(this.elements, feat.Value))
			{
				list.Add(Element.Create(feat.id, num) as Feat);
			}
		}
		return list;
	}

	public void SetFeat(int id, int value = 1, bool msg = false)
	{
		Feat feat = this.elements.GetElement(id) as Feat;
		int num = 0;
		if (feat != null && feat.Value > 0)
		{
			if (value == feat.Value)
			{
				return;
			}
			num = feat.Value;
			feat.Apply(-feat.Value, this.elements, false);
		}
		if (value > 0)
		{
			feat = (this.elements.SetBase(id, value - ((feat != null) ? feat.vSource : 0), 0) as Feat);
			feat.Apply(feat.Value, this.elements, false);
		}
		else
		{
			this.elements.Remove(id);
		}
		if (EClass.core.IsGameStarted)
		{
			this.Refresh(false);
			this.CalculateMaxStamina();
		}
		if (msg)
		{
			if (feat.source.textInc.IsEmpty())
			{
				base.PlaySound("ding_skill", 1f, true);
				Msg.SetColor(Msg.colors.Ding);
				base.Say("gainFeat", this, feat.FullName, null);
			}
			else
			{
				bool flag = value < num;
				if (feat.source.tag.Contains("neg"))
				{
					flag = !flag;
				}
				base.PlaySound("mutation", 1f, true);
				Msg.SetColor(flag ? Msg.colors.Negative : Msg.colors.Ding);
				base.Say((value > num) ? feat.source.GetText("textInc", false) : feat.source.GetText("textDec", false), this, feat.FullName, null);
			}
			this.elements.CheckSkillActions();
		}
	}

	public bool MutateRandom(int vec = 0, int tries = 100, bool ether = false, BlessedState state = BlessedState.Normal)
	{
		if (!ether && vec >= 0 && base.HasElement(406, 1) && EClass.rnd(5) != 0)
		{
			base.Say("resistMutation", this, null, null);
			return false;
		}
		IEnumerable<SourceElement.Row> ie = from a in EClass.sources.elements.rows
		where a.category == (ether ? "ether" : "mutation")
		select a;
		for (int i = 0; i < tries; i++)
		{
			SourceElement.Row row = ie.RandomItem<SourceElement.Row>();
			if (((i == 0 && vec < 0) & ether) && base.c_corruptionHistory != null && base.c_corruptionHistory.Count > 0)
			{
				row = EClass.sources.elements.map[base.c_corruptionHistory.LastItem<int>()];
				base.c_corruptionHistory.RemoveAt(base.c_corruptionHistory.Count - 1);
				if (base.c_corruptionHistory.Count == 0)
				{
					base.c_corruptionHistory = null;
				}
			}
			Element element = this.elements.GetElement(row.id);
			int num = 1;
			if ((vec <= 0 || ((row.id != 1563 || this.corruption >= 300) && (row.id != 1562 || this.corruption >= 1000 || !base.IsPowerful))) && (vec >= 0 || (element != null && element.Value > 0)) && (vec <= 0 || element == null || element.Value < row.max))
			{
				if (element == null && !row.aliasParent.IsEmpty() && this.elements.Has(row.aliasParent))
				{
					row = EClass.sources.elements.alias[row.aliasParent];
					element = this.elements.GetElement(row.id);
				}
				bool flag = row.tag.Contains("neg");
				if (vec > 0)
				{
					if (state >= BlessedState.Blessed && flag)
					{
						goto IL_44F;
					}
					if (state <= BlessedState.Cursed && !flag)
					{
						goto IL_44F;
					}
				}
				else if (vec < 0 && ((state >= BlessedState.Blessed && !flag) || (state <= BlessedState.Cursed && flag)))
				{
					goto IL_44F;
				}
				bool flag2 = true;
				if (element != null)
				{
					num = element.Value + ((vec != 0) ? vec : ((EClass.rnd(2) == 0) ? 1 : -1));
					if (num > element.source.max)
					{
						num = element.source.max - 1;
					}
					flag = ((flag && num > element.Value) || (!flag && num < element.Value));
					flag2 = (num > element.Value);
					if (vec > 0 && !flag2)
					{
						goto IL_44F;
					}
				}
				base.Say(flag2 ? "mutation_gain" : "mutation_loose", this, null, null);
				this.SetFeat(row.id, num, false);
				if (flag2 & ether)
				{
					if (base.c_corruptionHistory == null)
					{
						base.c_corruptionHistory = new List<int>();
					}
					base.c_corruptionHistory.Add(row.id);
					if (this.IsPCFaction)
					{
						Element element2 = this.elements.GetElement(row.id);
						WidgetPopText.Say("popEther".lang(element2.Name, base.Name, null, null, null), FontColor.Default, null);
					}
					if (this.IsPC && !EClass.player.flags.gotEtherDisease)
					{
						EClass.player.flags.gotEtherDisease = true;
						Thing thing = ThingGen.Create("parchment", -1, -1);
						thing.SetStr(53, "letter_ether");
						Thing thing2 = ThingGen.Create("1165", -1, -1);
						thing2.SetBlessedState(BlessedState.Normal);
						Thing p = ThingGen.CreateParcel(null, new Thing[]
						{
							thing2,
							thing
						});
						EClass.world.SendPackage(p);
					}
				}
				if (EClass.core.IsGameStarted && this.pos != null)
				{
					base.PlaySound(ether ? "mutation_ether" : "mutation", 1f, true);
					base.PlayEffect("mutation", true, 0f, default(Vector3));
					Msg.SetColor(flag ? Msg.colors.MutateBad : Msg.colors.MutateGood);
					base.Say(row.GetText(flag ? "textDec" : "textInc", true) ?? row.alias, this, null, null);
				}
				return true;
			}
			IL_44F:;
		}
		base.Say("nothingHappens", null, null);
		return false;
	}

	public void GainAbility(int ele, int mtp = 100)
	{
		Element orCreateElement = this.elements.GetOrCreateElement(ele);
		if (orCreateElement.ValueWithoutLink == 0)
		{
			this.elements.ModBase(orCreateElement.id, 1);
		}
		if (orCreateElement is Spell)
		{
			int num = mtp * orCreateElement.source.charge * (100 + base.Evalue(307) + (base.HasElement(307, 1) ? 20 : 0)) / 100 / 100;
			if (orCreateElement.source.charge == 1)
			{
				num = 1;
			}
			orCreateElement.vPotential += Mathf.Max(1, num / 2 + EClass.rnd(num / 2 + 1));
		}
		base.Say("spell_gain", this, orCreateElement.Name, null);
		LayerAbility.SetDirty(orCreateElement);
	}

	public bool TryNullifyCurse()
	{
		if (this.IsPCParty)
		{
			using (List<Chara>.Enumerator enumerator = EClass.pc.party.members.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Chara chara = enumerator.Current;
					if (chara.HasElement(1641, 1) && EClass.rnd(3) != 0)
					{
						Msg.Say("curse_nullify", chara, null, null, null);
						return true;
					}
				}
				goto IL_8F;
			}
		}
		if (base.HasElement(1641, 1) && EClass.rnd(3) != 0)
		{
			base.Say("curse_nullify", this, null, null);
			return true;
		}
		IL_8F:
		if (this.GetCondition<ConHolyVeil>() != null && EClass.rnd(5) != 0)
		{
			base.Say("curse_nullify", this, null, null);
			return true;
		}
		return false;
	}

	public int GetPietyValue()
	{
		if (base._IsPC)
		{
			return 10 + (int)(Mathf.Sqrt((float)base.c_daysWithGod) * 2f + (float)base.Evalue(85)) / 2;
		}
		return 10 + (int)(Mathf.Sqrt((float)base.LV) * 5f + (float)base.Evalue(306)) / 2;
	}

	public void RefreshFaithElement()
	{
		if (this.faithElements != null)
		{
			this.faithElements.SetParent(null);
		}
		this.faithElements = new ElementContainer();
		if (this.idFaith == "eyth" && !base.HasElement(1228, 1))
		{
			return;
		}
		SourceReligion.Row row = EClass.sources.religions.map.TryGetValue(this.idFaith, null);
		if (row == null)
		{
			return;
		}
		this.faithElements = new ElementContainer();
		SourceElement.Row row2 = EClass.sources.elements.alias.TryGetValue("featGod_" + row.id + "1", null);
		if (row2 != null)
		{
			this.faithElements.SetBase(row2.id, 1, 0);
		}
		if (!this.HasCondition<ConExcommunication>())
		{
			int[] elements = row.elements;
			int num = this.GetPietyValue() * (120 + base.Evalue(1407) * 15) / 100;
			for (int i = 0; i < elements.Length; i += 2)
			{
				int num2 = elements[i + 1] * num / 50;
				if (elements[i] == 79)
				{
					num2 = EClass.curve(num2, 100, 20, 60);
				}
				if (num2 >= 20 && elements[i] >= 950 && elements[i] < 970)
				{
					num2 = 20;
				}
				this.faithElements.SetBase(elements[i], Mathf.Max(num2, 1), 0);
			}
		}
		this.faithElements.SetParent(this);
	}

	public void ModTempElement(int ele, int a, bool naturalDecay = false)
	{
		if (a < 0 && !naturalDecay)
		{
			SourceElement.Row row = EClass.sources.elements.alias["sustain_" + EClass.sources.elements.map[ele].alias];
			if (base.HasElement((row != null) ? row.id : 0, 1))
			{
				return;
			}
		}
		if (this.tempElements == null)
		{
			this.tempElements = new ElementContainer();
			this.tempElements.SetParent(this);
		}
		if (a > 0 && this.tempElements.Base(ele) > a)
		{
			a = a * 100 / (200 + (this.tempElements.Base(ele) - a) * 10);
		}
		Element element = this.tempElements.ModBase(ele, a);
		if (element.vBase == 0)
		{
			this.tempElements.Remove(element.id);
			if (this.tempElements.dict.Count == 0)
			{
				this.tempElements = null;
			}
		}
	}

	public void DamageTempElements(int p, bool body, bool mind)
	{
		if (body)
		{
			this.DamageTempElement(Element.List_Body.RandomItem<int>(), p);
		}
		if (mind)
		{
			this.DamageTempElement(Element.List_Mind.RandomItem<int>(), p);
		}
	}

	public void DamageTempElement(int ele, int p)
	{
		this.ModTempElement(ele, -(p / 100 + EClass.rnd(p / 100 + 1) + 1), false);
	}

	public void EnhanceTempElements(int p, bool body, bool mind)
	{
		if (body)
		{
			this.EnhanceTempElement(Element.List_Body.RandomItem<int>(), p);
		}
		if (mind)
		{
			this.EnhanceTempElement(Element.List_Mind.RandomItem<int>(), p);
		}
	}

	public void EnhanceTempElement(int ele, int p)
	{
		this.ModTempElement(ele, p / 100 + EClass.rnd(p / 100 + 1), false);
	}

	public void DiminishTempElements(int a = 1)
	{
		if (this.tempElements == null)
		{
			return;
		}
		foreach (Element element in this.tempElements.dict.Values.ToList<Element>())
		{
			if (element.vBase > 0)
			{
				this.ModTempElement(element.id, -Mathf.Min(a, element.vBase), true);
			}
		}
	}

	public void CureTempElements(int p, bool body, bool mind)
	{
		Chara.<>c__DisplayClass565_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.p = p;
		if (this.tempElements == null)
		{
			return;
		}
		if (body)
		{
			this.<CureTempElements>g__Cure|565_0(Element.List_Body, ref CS$<>8__locals1);
		}
		if (mind)
		{
			this.<CureTempElements>g__Cure|565_0(Element.List_Mind, ref CS$<>8__locals1);
		}
	}

	[CompilerGenerated]
	private void <RestockInventory>g__Restock|360_0(string id, int num)
	{
		if (this.things.Find(id, -1, -1) == null)
		{
			base.AddCard(ThingGen.Create(id, -1, -1).SetNum(num));
		}
	}

	[CompilerGenerated]
	private void <get_baseWorkElements>g__Build|494_0(Hobby h)
	{
		if (h.source.elements.IsEmpty())
		{
			return;
		}
		for (int i = 0; i < h.source.elements.Length; i += 2)
		{
			int ele = h.source.elements[i];
			int v = h.source.elements[i + 1];
			this._baseWorkElements.ModBase(ele, v);
		}
	}

	[CompilerGenerated]
	private void <RefreshWorkElements>g__TryAdd|495_0(Hobby h)
	{
		if (h.source.elements.IsEmpty())
		{
			return;
		}
		if (this.workElements == null)
		{
			this.workElements = new ElementContainer();
		}
		int i = 0;
		while (i < h.source.elements.Length)
		{
			int num = h.source.elements[i];
			int num2 = h.source.elements[i + 1];
			int num3 = 100;
			if (num == 2115 || num == 2207)
			{
				goto IL_79;
			}
			num3 = h.GetEfficiency(this) * this.homeBranch.efficiency / 100;
			if (num3 > 0)
			{
				goto IL_79;
			}
			IL_AD:
			i += 2;
			continue;
			IL_79:
			this.workElements.ModBase(num, (num2 < 0) ? (num2 / 10) : Mathf.Max(1, h.source.elements[i + 1] * num3 / 1000));
			goto IL_AD;
		}
	}

	[CompilerGenerated]
	internal static int <PerformWork>g__PerformWork|506_0(Hobby work, int idx, bool isHobby, ref Chara.<>c__DisplayClass506_0 A_3)
	{
		if (idx >= work.source.resources.Length)
		{
			return 0;
		}
		int num = work.source.resources[idx];
		int num2 = num;
		num = Rand.Range(num * (100 - work.source.resources[idx]) / 100, num);
		num = num * (isHobby ? 50 : 100) * A_3.session.efficiency / 10000;
		if (num2 > 0 && num <= 0)
		{
			num = 1;
		}
		return num;
	}

	[CompilerGenerated]
	private void <CureTempElements>g__Cure|565_0(int[] eles, ref Chara.<>c__DisplayClass565_0 A_2)
	{
		foreach (int num in eles)
		{
			if (this.tempElements == null)
			{
				return;
			}
			Element element = this.tempElements.GetElement(num);
			if (element != null && element.vBase < 0)
			{
				this.ModTempElement(num, Mathf.Clamp(A_2.p / 20 + EClass.rnd(A_2.p / 20), 1, -element.vBase), false);
			}
		}
	}

	private static Point shared = new Point();

	private static List<Hobby> listHobby = new List<Hobby>();

	public static string[] IDTimeTable = new string[]
	{
		"default",
		"owl"
	};

	[JsonProperty]
	public int contribution;

	[JsonProperty]
	public Point orgPos;

	[JsonProperty]
	public Quest quest;

	[JsonProperty]
	public Chara ride;

	[JsonProperty]
	public Chara parasite;

	[JsonProperty]
	public Chara host;

	[JsonProperty]
	public ElementContainer tempElements;

	public ElementContainer faithElements;

	public ElementContainer workElements;

	[JsonProperty(PropertyName = "T1")]
	public Party party;

	[JsonProperty(PropertyName = "T2")]
	public FactionMemberType memberType;

	[JsonProperty(PropertyName = "T3")]
	public List<int> _hobbies;

	[JsonProperty(PropertyName = "T4")]
	public List<int> _works;

	[JsonProperty(PropertyName = "T5")]
	public WorkSummary _workSummary;

	[JsonProperty(PropertyName = "T6")]
	public List<int> _cooldowns;

	[JsonProperty(PropertyName = "T7")]
	public List<int> _listAbility;

	[JsonProperty(PropertyName = "1")]
	public PCCData pccData;

	[JsonProperty(PropertyName = "2")]
	public Card held;

	[JsonProperty(PropertyName = "3")]
	public int[] rawSlots;

	[JsonProperty(PropertyName = "4")]
	public GlobalData global;

	[JsonProperty(PropertyName = "8")]
	public string[] _strs = new string[5];

	[JsonProperty(PropertyName = "9")]
	public int[] _cints = new int[30];

	public BitArray32 _cbits1;

	public Chara enemy;

	public Chara master;

	public Point lastPos = new Point();

	public PathProgress path = new PathProgress();

	public CharaBody body = new CharaBody();

	public CharaAbility _ability;

	public Thing ranged;

	public Thing nextUse;

	public ConSleep conSleep;

	public ConSuspend conSuspend;

	public Emo2 emoIcon;

	public int happiness;

	public int turnLastSeen = -100;

	public int idleActTimer;

	public int combatCount;

	public int calmCheckTurn;

	public int sharedCheckTurn;

	public float idleTimer;

	public bool isDrunk;

	public bool isConfused;

	public bool isFainted;

	public bool isBlind;

	public bool isParalyzed;

	public bool _isLevitating;

	public bool isCreated;

	public bool canSeeInvisible;

	public bool hasTelepathy;

	public bool isWet;

	public bool bossText;

	private Faction _faction;

	public SourceChara.Row source;

	public SourceRace.Row _race;

	public SourceJob.Row _job;

	public Tactics _tactics;

	public static List<SourceRace.Row> ListAdvRace = new List<SourceRace.Row>();

	public static List<SourceJob.Row> ListAdvJob = new List<SourceJob.Row>();

	public bool dirtySpeed = true;

	private int _Speed;

	private static Point _sharedPos = new Point();

	private bool hasMovedThisTurn;

	public float actTime = 0.3f;

	public static bool consumeTurn;

	public static bool preventRegen;

	public static bool isOnCreate;

	private static List<Chara> _pts = new List<Chara>();

	public int stealthSeen;

	private static List<Thing> _ListItems = new List<Thing>();

	private static List<SourceThing.Row> _listFavFood = new List<SourceThing.Row>();

	private static List<SourceCategory.Row> _listFavCat = new List<SourceCategory.Row>();

	public static NoGoal _NoGoalPC = new NoGoal();

	public static NoGoal _NoGoalRepeat = new NoGoal();

	public GoalList goalList = new GoalList();

	public AIAct ai = new NoGoal();

	public ElementContainer _baseWorkElements;

	private static GoalWork _goalWork = new GoalWork();

	private static GoalHobby _goalHobby = new GoalHobby();

	[JsonProperty]
	public List<Condition> conditions = new List<Condition>();

	[JsonProperty]
	public Dictionary<int, int> resistCon;
}
