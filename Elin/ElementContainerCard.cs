using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ElementContainerCard : ElementContainer
{
	public override Card Card
	{
		get
		{
			return this.owner;
		}
	}

	public override Chara Chara
	{
		get
		{
			return this.owner.Chara;
		}
	}

	public override bool IsMeleeWeapon
	{
		get
		{
			return this.Card.category.slot == 35 && this.Card.category.skill != 0;
		}
	}

	public void SetOwner(Card c, bool applyFeat)
	{
		this.owner = c;
		SourceValueType sourceValueType = (c.IsEquipmentOrRanged && c.rarity < Rarity.Artifact) ? SourceValueType.EquipmentRandom : SourceValueType.Fixed;
		base.ApplyElementMap(c.uid, sourceValueType, c.sourceCard.elementMap, (sourceValueType == SourceValueType.EquipmentRandom) ? c.LV : 1, false, applyFeat);
	}

	public override void OnLearn(int ele)
	{
		if (this.owner.IsPCFaction)
		{
			SE.DingSkill2();
		}
		Msg.Say("learnSkill", this.Card, EClass.sources.elements.map[ele].GetName(), null, null);
		this.CheckSkillActions();
	}

	public void CheckSkillActions()
	{
		if (this.owner.IsPC)
		{
			this.<CheckSkillActions>g__TryLearn|9_0(6011, 281, 0);
			this.<CheckSkillActions>g__TryLearn|9_0(6018, 226, 0);
			this.<CheckSkillActions>g__TryLearn|9_0(6019, 227, 0);
			this.<CheckSkillActions>g__TryLearn|9_0(6700, 1649, 0);
			this.<CheckSkillActions>g__TryLearn|9_0(6720, 1657, 0);
			this.<CheckSkillActions>g__TryLearn|9_0(6450, 132, 5);
		}
	}

	public override void OnTrain(int ele)
	{
		Msg.Say("trainSkill", this.Card, EClass.sources.elements.map[ele].GetName(), null, null);
	}

	public override void OnModTempPotential(Element e, int v, int threshMsg)
	{
		if (threshMsg != 0 && Mathf.Abs(v) < threshMsg)
		{
			return;
		}
		string lang = (v > 0) ? "potentialInc" : "potentialDec";
		if (this.owner.IsPCFaction && v > 0)
		{
			this.owner.PlaySound("ding_potential", 1f, true);
		}
		Msg.SetColor((v > 0) ? "positive" : "negative");
		this.owner.Say(lang, this.owner, e.Name.ToLower(), null);
	}

	public override void OnLevelUp(Element e, int lastValue)
	{
		if (!e.ShowMsgOnValueChanged)
		{
			return;
		}
		if (this.owner.IsPC)
		{
			if (e.id == 287)
			{
				EClass.player.flags.canComupWithFoodRecipe = true;
			}
			SE.DingSkill2();
		}
		if (this.owner.isChara)
		{
			if (this.owner.Chara.IsPCFaction)
			{
				if (!VirtualDate.IsActive)
				{
					if (this.owner.Chara.IsInActiveZone)
					{
						Msg.SetColor(Msg.colors.Ding);
						string text = e.source.GetText("textInc", true);
						if (!text.IsEmpty())
						{
							this.owner.Say(text, this.owner, null, null);
						}
						else
						{
							this.owner.Say("ding_skill", this.owner, e.Name, null);
						}
						this.owner.pos.TalkWitnesses(this.owner.Chara, "ding_other", 4, WitnessType.ally, null, 4 + EClass.pc.party.members.Count);
					}
					if (this.owner.IsPCParty)
					{
						WidgetPopText.Say("popDing".lang(this.owner.IsPC ? "" : this.owner.Name, e.Name, lastValue.ToString() ?? "", e.ValueWithoutLink.ToString() ?? "", null), FontColor.Good, null);
					}
				}
				if (this.owner.Chara.homeBranch != null)
				{
					this.owner.Chara.homeBranch.Log("bDing", this.Card, e.Name, lastValue.ToString() ?? "", e.ValueWithoutLink.ToString() ?? "");
				}
			}
			if (e is Skill)
			{
				this.owner.AddExp(10 + EClass.rnd(5));
			}
			this.owner.Chara.CalculateMaxStamina();
		}
		this.CheckSkillActions();
	}

	public override void OnLevelDown(Element e, int lastValue)
	{
		if (!e.ShowMsgOnValueChanged)
		{
			return;
		}
		bool isPC = this.owner.IsPC;
		if (this.owner.isChara)
		{
			if (this.owner.Chara.IsPCFaction)
			{
				Msg.SetColor(Msg.colors.Negative);
				string text = e.source.GetText("textDec", true);
				if (!text.IsEmpty())
				{
					Msg.Say(text, this.owner, null, null, null);
				}
				else
				{
					Msg.Say("dec_skill", this.owner, e.Name, null, null);
				}
			}
			this.owner.Chara.CalculateMaxStamina();
		}
	}

	public override int ValueBonus(Element e)
	{
		if (EClass.game == null)
		{
			return 0;
		}
		int num = 0;
		if (this.owner.IsPCFactionOrMinion)
		{
			Element element = EClass.pc.faction.charaElements.GetElement(e.id);
			if (element != null)
			{
				num += element.Value;
			}
			if (this.owner.IsPCParty)
			{
				int id = e.id;
				if (id == 70 || id - 72 <= 1)
				{
					int num2 = 0;
					foreach (Chara chara in EClass.pc.party.members)
					{
						if (chara.Evalue(1419) > 0)
						{
							num2 += chara.Evalue(1419);
						}
					}
					if (num2 > 0)
					{
						int num3 = 0;
						using (List<Chara>.Enumerator enumerator = EClass._map.charas.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								if (enumerator.Current.IsHostile(EClass.pc))
								{
									num3++;
								}
							}
						}
						if (num3 > 0)
						{
							num += Mathf.Max(1, (e.ValueWithoutLink + e.vLink) * (int)Mathf.Clamp(4f + Mathf.Sqrt((float)num3) * (float)num2 * 2f, 5f, 30f) / 100);
						}
					}
				}
			}
			if (e.id == 78)
			{
				num += EClass.player.CountKeyItem("lucky_coin") * 2;
				if (this.Chara != null && this.Chara.Evalue(663) > 0)
				{
					num = num * 2 + (e.ValueWithoutLink + e.vLink);
				}
			}
			if (this.owner.Chara.race.IsMachine || this.owner.id == "android")
			{
				int num4 = this.owner.Evalue(664);
				if (num4 > 0)
				{
					int id = e.id;
					if (id - 64 > 1)
					{
						if (id == 79)
						{
							num += (e.ValueWithoutLink + e.vLink) * num4 / 100;
						}
					}
					else
					{
						num += (e.ValueWithoutLink + e.vLink) * num4 / 2 / 100;
					}
				}
			}
		}
		if (!e.source.aliasMtp.IsEmpty())
		{
			int num5 = this.owner.Evalue(e.source.aliasMtp);
			if (num5 != 0)
			{
				num += (e.ValueWithoutLink + e.vLink) * num5 / 100;
			}
		}
		return num;
	}

	[CompilerGenerated]
	private void <CheckSkillActions>g__TryLearn|9_0(int eleAction, int reqEle, int lqlv)
	{
		if (base.HasBase(eleAction) || !base.HasBase(reqEle) || base.GetElement(reqEle).ValueWithoutLink < lqlv)
		{
			return;
		}
		base.SetBase(eleAction, 1, 0);
		if (!EClass.core.IsGameStarted)
		{
			return;
		}
		if (this.owner.IsPC)
		{
			LayerAbility.Redraw();
		}
		this.owner.Say("learnSkill", this.Card, EClass.sources.elements.map[eleAction].GetName(), null);
	}

	public Card owner;
}
