using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Religion : EClass
{
	public virtual string id
	{
		get
		{
			return "";
		}
	}

	public virtual bool IsAvailable
	{
		get
		{
			return false;
		}
	}

	public virtual SourceElement.Row GetFeat(int i)
	{
		return EClass.sources.elements.alias["featGod_" + this.id + i.ToString()];
	}

	public string Name
	{
		get
		{
			return this.source.GetName();
		}
	}

	public SourceReligion.Row source
	{
		get
		{
			SourceReligion.Row result;
			if ((result = this._source) == null)
			{
				result = (this._source = EClass.sources.religions.map[this.id]);
			}
			return result;
		}
	}

	public string NameShort
	{
		get
		{
			return this.source.GetTextArray("name2")[1];
		}
	}

	public string NameDomain
	{
		get
		{
			return this.source.GetTextArray("name2")[0];
		}
	}

	public string TextType
	{
		get
		{
			return ("sub_" + this.source.type).lang();
		}
	}

	public string TextGodGender
	{
		get
		{
			return this.source.GetText("textType", false);
		}
	}

	public string TextMood
	{
		get
		{
			return this.GetTextTemper(-99999);
		}
	}

	public bool IsEyth
	{
		get
		{
			return this.id == "eyth";
		}
	}

	public bool IsEhekatl
	{
		get
		{
			return this.id == "luck";
		}
	}

	public bool IsOpatos
	{
		get
		{
			return this.id == "earth";
		}
	}

	public virtual bool IsMinorGod
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanJoin
	{
		get
		{
			return true;
		}
	}

	public void Init()
	{
		this.relation = this.source.relation;
	}

	public void OnLoad()
	{
	}

	public void OnAdvanceDay()
	{
	}

	public Sprite GetSprite()
	{
		return ResourceCache.Load<Sprite>("Media/Graphics/Image/Faction/" + this.source.id) ?? ResourceCache.Load<Sprite>("Media/Graphics/Image/Faction/eyth");
	}

	public void SetTextRelation(UIText text)
	{
		if (this.relation > 100)
		{
			text.SetText("reFriend".lang(), FontColor.Good);
			return;
		}
		if (this.relation < -100)
		{
			text.SetText("reEnemy".lang(), FontColor.Bad);
			return;
		}
		text.SetText("reNone".lang(), FontColor.Passive);
	}

	public string GetTextBenefit()
	{
		string text = "";
		for (int i = 0; i < this.source.elements.Length; i += 2)
		{
			if (i != 0)
			{
				text = text + Lang.words.comma.ToString() + Lang.space;
			}
			text += EClass.sources.elements.map[this.source.elements[i]].GetName();
		}
		return this.source.GetText("textBenefit", false) + (this.IsEyth ? "" : "textBenefit".lang(text, null, null, null, null));
	}

	public string GetTextTemper(int _temper = -99999)
	{
		if (this.IsEyth)
		{
			return "-";
		}
		if (_temper == -99999)
		{
			_temper = this.mood;
		}
		string[] list = Lang.GetList("temper");
		if (_temper <= -85)
		{
			return list[0].ToTitleCase(false).TagColor(FontColor.Bad, null);
		}
		if (_temper <= -45)
		{
			return list[1].ToTitleCase(false).TagColor(FontColor.Bad, null);
		}
		if (_temper <= -15)
		{
			return list[2].ToTitleCase(false);
		}
		if (_temper < 15)
		{
			return list[3].ToTitleCase(false);
		}
		if (_temper < 45)
		{
			return list[4].ToTitleCase(false);
		}
		if (_temper < 85)
		{
			return list[5].ToTitleCase(false).TagColor(FontColor.Great, null);
		}
		return list[6].ToTitleCase(false).TagColor(FontColor.Good, null);
	}

	public void Revelation(string idTalk, int chance = 100)
	{
		if (this.IsEyth || EClass.rnd(100) > chance)
		{
			return;
		}
		this.Talk(idTalk, EClass.pc, null);
	}

	public void Talk(string idTalk, Card c = null, Card agent = null)
	{
		if (this.IsEyth)
		{
			return;
		}
		Msg.SetColor(Msg.colors.TalkGod);
		Msg.Say("<i>" + this.GetGodTalk(idTalk) + " </i>", c ?? EClass.pc, null, null, null);
	}

	public string GetGodTalk(string suffix)
	{
		return EClass.sources.dataGodTalk.GetText(this.id, suffix).Split(Environment.NewLine.ToCharArray()).RandomItem<string>();
	}

	public int GetOfferingValue(Thing t, int num = -1)
	{
		if (num == -1)
		{
			num = t.Num;
		}
		int num2 = 0;
		if (t.source._origin == "meat")
		{
			num2 = Mathf.Clamp(t.SelfWeight / 10, 1, 1000);
			if (t.refCard == null)
			{
				num2 /= 10;
			}
		}
		else
		{
			foreach (string text in this.source.cat_offer)
			{
				if (t.category.IsChildOf(text))
				{
					num2 = Mathf.Clamp(t.SelfWeight / 10, 50, 1000);
					num2 *= EClass.sources.categories.map[text].offer / 100;
					break;
				}
			}
		}
		if (num2 == 0)
		{
			return 0;
		}
		if (t.IsDecayed)
		{
			num2 /= 10;
		}
		num2 *= (t.LV * 2 + 100) / 100;
		return Mathf.Max(num2, 1) * num;
	}

	public bool TryGetGift()
	{
		if (this.IsEyth || this.source.rewards.Length == 0)
		{
			return false;
		}
		Point point = EClass.pc.pos.GetNearestPoint(false, false, false, false) ?? EClass.pc.pos;
		int num = EClass.pc.Evalue(85);
		if (this.giftRank == 0 && (num >= 15 || EClass.debug.enable))
		{
			this.Talk("pet", null, null);
			Chara chara = CharaGen.Create(this.source.rewards[0], -1);
			EClass._zone.AddCard(chara, point);
			chara.MakeAlly(true);
			chara.PlayEffect("aura_heaven", true, 0f, default(Vector3));
			this.giftRank = 1;
			return true;
		}
		if (this.source.rewards.Length >= 2 && this.giftRank == 1 && (num >= 30 || EClass.debug.enable))
		{
			this.Talk("gift", null, null);
			string[] array = this.source.rewards[1].Split('|', StringSplitOptions.None);
			foreach (string text in array)
			{
				Religion.Reforge(text, point, text == array[0]);
			}
			this.giftRank = 2;
			return true;
		}
		return false;
	}

	public bool IsValidArtifact(string id)
	{
		if (this.giftRank < 2)
		{
			return false;
		}
		uint num = <PrivateImplementationDetails>.ComputeStringHash(id);
		if (num <= 1934600421U)
		{
			if (num <= 1352297338U)
			{
				if (num != 850586461U)
				{
					if (num != 968766664U)
					{
						if (num == 1352297338U)
						{
							if (id == "pole_holy")
							{
								return this == EClass.game.religions.Healing;
							}
						}
					}
					else if (id == "sword_muramasa2")
					{
						return this == EClass.game.religions.MoonShadow;
					}
				}
				else if (id == "luckydagger")
				{
					return this == EClass.game.religions.Luck;
				}
			}
			else if (num != 1564759233U)
			{
				if (num != 1755407712U)
				{
					if (num == 1934600421U)
					{
						if (id == "warmonger")
						{
							return this == EClass.game.religions.Strife;
						}
					}
				}
				else if (id == "scythe_kumi")
				{
					return this == EClass.game.religions.Harvest;
				}
			}
			else if (id == "gun_mani")
			{
				return this == EClass.game.religions.Machine;
			}
		}
		else if (num <= 2931008955U)
		{
			if (num != 2039965765U)
			{
				if (num != 2376119426U)
				{
					if (num == 2931008955U)
					{
						if (id == "kogitsunemaru")
						{
							return this == EClass.game.religions.Trickery;
						}
					}
				}
				else if (id == "staff_element")
				{
					return this == EClass.game.religions.Element;
				}
			}
			else if (id == "windbow")
			{
				return this == EClass.game.religions.Wind;
			}
		}
		else if (num != 3457783642U)
		{
			if (num != 3896459095U)
			{
				if (num == 3950410875U)
				{
					if (id == "cloak_mani")
					{
						return this == EClass.game.religions.Machine;
					}
				}
			}
			else if (id == "blunt_earth")
			{
				return this == EClass.game.religions.Earth;
			}
		}
		else if (id == "shirt_wind")
		{
			return this == EClass.game.religions.Wind;
		}
		return false;
	}

	public static void Reforge(string id, Point pos = null, bool first = true)
	{
		if (pos == null)
		{
			pos = EClass.pc.pos.Copy();
		}
		pos.Set(pos.GetNearestPoint(false, false, false, true) ?? pos);
		Thing thing = ThingGen.Create(id, -1, -1);
		foreach (Element element in thing.elements.dict.Values)
		{
			if (element.id != 66 && element.id != 67 && element.id != 64 && element.id != 65 && element.id != 92)
			{
				uint num = <PrivateImplementationDetails>.ComputeStringHash(id);
				if (num <= 1934600421U)
				{
					if (num <= 1352297338U)
					{
						if (num != 850586461U)
						{
							if (num != 968766664U)
							{
								if (num == 1352297338U)
								{
									if (id == "pole_holy")
									{
										if (element.id == 60 || element.id == 461 || element.id == 423)
										{
											element.vExp = -1;
										}
										thing.c_idDeity = EClass.game.religions.Healing.id;
									}
								}
							}
							else if (id == "sword_muramasa2")
							{
								if (element.id == 401 || element.id == 916 || element.id == 661)
								{
									element.vExp = -1;
								}
								thing.c_idDeity = EClass.game.religions.MoonShadow.id;
							}
						}
						else if (id == "luckydagger")
						{
							if (element.id != 426)
							{
								element.vExp = -1;
							}
							thing.c_idDeity = EClass.game.religions.Luck.id;
						}
					}
					else if (num != 1564759233U)
					{
						if (num != 1755407712U)
						{
							if (num == 1934600421U)
							{
								if (id == "warmonger")
								{
									if (element.id == 423 || element.id == 463 || element.id == 460 || element.id == 464 || element.id == 465)
									{
										element.vExp = -1;
									}
									thing.c_idDeity = EClass.game.religions.Strife.id;
								}
							}
						}
						else if (id == "scythe_kumi")
						{
							if (element.id == 6650 || element.id == 480 || element.id == 959 || element.id == 428 || element.id == 640 || element.id == 665)
							{
								element.vExp = -1;
							}
							thing.c_idDeity = EClass.game.religions.Harvest.id;
						}
					}
					else if (id == "gun_mani")
					{
						thing.c_idDeity = EClass.game.religions.Machine.id;
					}
				}
				else if (num <= 2931008955U)
				{
					if (num != 2039965765U)
					{
						if (num != 2376119426U)
						{
							if (num == 2931008955U)
							{
								if (id == "kogitsunemaru")
								{
									if (element.id != 656)
									{
										element.vExp = -1;
									}
									thing.c_idDeity = EClass.game.religions.Trickery.id;
								}
							}
						}
						else if (id == "staff_element")
						{
							if (element.id == 411 || (element is Resistance && element.id != 959))
							{
								element.vExp = -1;
							}
							thing.c_idDeity = EClass.game.religions.Element.id;
						}
					}
					else if (id == "windbow")
					{
						thing.c_idDeity = EClass.game.religions.Wind.id;
					}
				}
				else if (num != 3457783642U)
				{
					if (num != 3896459095U)
					{
						if (num == 3950410875U)
						{
							if (id == "cloak_mani")
							{
								if (element.id == 427 || element.id == 957 || element.id == 105 || element.id == 466 || element.id == 664)
								{
									element.vExp = -1;
								}
								thing.c_idDeity = EClass.game.religions.Machine.id;
							}
						}
					}
					else if (id == "blunt_earth")
					{
						if (element.id == 70 || element.id == 55 || element.id == 56 || element.id == 954 || element.id == 423 || element.id == 421)
						{
							element.vExp = -1;
						}
						thing.c_idDeity = EClass.game.religions.Earth.id;
					}
				}
				else if (id == "shirt_wind")
				{
					if (!(element is Resistance) && element.id != 226 && element.id != 152 && element.id != 77)
					{
						element.vExp = -1;
					}
					thing.c_idDeity = EClass.game.religions.Wind.id;
				}
			}
		}
		EClass._zone.AddCard(thing, pos);
		pos.PlayEffect("aura_heaven");
		if (first)
		{
			pos.PlaySound("godbless", true, 1f, true);
		}
	}

	public virtual void OnBecomeBranchFaith()
	{
	}

	public void JoinFaith(Chara c)
	{
		if (!c.IsPC)
		{
			c.faith = this;
			c.RefreshFaithElement();
			EClass.Sound.Play("worship");
			Msg.Say("changeFaith", c, this.Name, null, null);
			return;
		}
		if (c.faith != this)
		{
			c.faith.LeaveFaith(c);
		}
		EClass.pc.c_daysWithGod = 0;
		Msg.Say("worship", this.Name, null, null, null);
		this.Talk("worship", c, null);
		EClass.Sound.Play("worship");
		c.PlayEffect("aura_heaven", true, 0f, default(Vector3));
		c.faith = this;
		c.elements.SetBase(85, 0, 0);
		this.OnJoinFaith();
		if (this.IsEyth)
		{
			this.mood = 0;
		}
		else
		{
			this.mood = 50;
		}
		c.RefreshFaithElement();
		if (!c.HasElement(306, 1))
		{
			c.elements.Learn(306, 1);
		}
		if (c.IsPC)
		{
			EClass.pc.faction.charaElements.OnJoinFaith();
		}
	}

	public void LeaveFaith(Chara c)
	{
		if (this.IsEyth)
		{
			return;
		}
		if (c.IsPC)
		{
			Msg.Say("worship2");
			this.Punish(c);
		}
		if (c.IsPC)
		{
			EClass.pc.faction.charaElements.OnLeaveFaith();
		}
		this.OnLeaveFaith();
		c.RefreshFaithElement();
	}

	public void Punish(Chara c)
	{
		this.Talk("wrath", null, null);
		if (c.Evalue(1228) > 0)
		{
			c.SayNothingHappans();
			return;
		}
		c.hp = 1;
		c.mana.value = 1;
		c.stamina.value = 1;
		if (c.HasCondition<ConWrath>())
		{
			Religion.recentWrath = this;
			c.DamageHP(999999, AttackSource.Wrath, null);
			Religion.recentWrath = null;
			return;
		}
		Thing thing = ThingGen.Create("punish_ball", -1, -1);
		int num = 0;
		using (List<Religion>.Enumerator enumerator = EClass.game.religions.list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.giftRank > 0)
				{
					num++;
				}
			}
		}
		if (num >= 4)
		{
			thing.idSkin = 1;
		}
		thing.ChangeWeight(EClass.pc.WeightLimit / 4 + 1000);
		c.AddThing(thing, true, -1, -1);
		c.AddCondition<ConWrath>(2000 + (c.IsPC ? (EClass.pc.c_daysWithGod * 20) : 0), false);
	}

	public void PunishTakeOver(Chara c)
	{
		this.Talk("takeoverFail", null, null);
		if (c.Evalue(1228) > 0)
		{
			c.SayNothingHappans();
			return;
		}
		c.hp /= 2;
		if (c.mana.value > 0)
		{
			c.mana.value = c.mana.value / 2;
		}
		if (c.stamina.value > 0)
		{
			c.stamina.value = c.stamina.value / 2;
		}
		if (c.HasCondition<ConWrath>())
		{
			Religion.recentWrath = this;
			c.DamageHP(999999, AttackSource.Wrath, null);
			Religion.recentWrath = null;
			return;
		}
		Thing thing = ThingGen.Create("punish_ball", -1, -1);
		thing.c_weight = EClass.pc.WeightLimit / 4 + 1000;
		thing.isWeightChanged = true;
		thing.SetDirtyWeight();
		c.AddThing(thing, true, -1, -1);
		c.AddCondition<ConWrath>(200, false);
	}

	public virtual void OnJoinFaith()
	{
	}

	public virtual void OnLeaveFaith()
	{
	}

	public void OnChangeHour()
	{
		if (this.IsEyth)
		{
			this.mood = 0;
			return;
		}
		this.mood = EClass.rnd(200) - 100;
	}

	[JsonProperty]
	public int relation;

	[JsonProperty]
	public int giftRank;

	[JsonProperty]
	public int mood;

	public static Religion recentWrath;

	public SourceReligion.Row _source;
}
