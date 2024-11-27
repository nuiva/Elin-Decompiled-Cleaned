using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Thing : Card
{
	public bool isEquipped
	{
		get
		{
			return base.c_equippedSlot != 0;
		}
	}

	public bool IsMeleeWithAmmo
	{
		get
		{
			return this.trait is TraitToolRange && this.isEquipped;
		}
	}

	public int range
	{
		get
		{
			return this.source.range;
		}
	}

	public int Penetration
	{
		get
		{
			if (this.source.substats.Length == 0)
			{
				return 0;
			}
			return this.source.substats[0];
		}
	}

	public override bool isThing
	{
		get
		{
			return true;
		}
	}

	public override CardRow sourceCard
	{
		get
		{
			return this.source;
		}
	}

	public override SourcePref Pref
	{
		get
		{
			if (this.source.origin == null || this.source.pref.UsePref)
			{
				return this.source.pref;
			}
			return this.source.origin.pref;
		}
	}

	public override int SelfWeight
	{
		get
		{
			if (!base.IsUnique)
			{
				return (base.isWeightChanged ? base.c_weight : this.source.weight) * base.material.weight / 100;
			}
			if (!base.isWeightChanged)
			{
				return this.source.weight;
			}
			return base.c_weight;
		}
	}

	public override int[] Tiles
	{
		get
		{
			return this.sourceCard._tiles;
		}
	}

	public bool CanAutoFire(Chara c, Card tg, bool reloading = false)
	{
		if (base.GetRootCard() != c)
		{
			return false;
		}
		if (base.HasTag(CTAG.throwWeapon))
		{
			return true;
		}
		if (!this.trait.CanAutofire)
		{
			return false;
		}
		if (this.trait is TraitToolRange)
		{
			if ((c.IsPCFaction && c.body.IsTooHeavyToEquip(this)) || reloading)
			{
				return false;
			}
		}
		else if (this.trait is TraitAbility && c.IsPC)
		{
			Act act = (this.trait as TraitAbility).act;
			Element element = c.elements.GetElement(act.id);
			if (act is Spell && (element == null || element.vPotential == 0))
			{
				return false;
			}
		}
		return true;
	}

	public int GetEfficiency()
	{
		return (int)(50 + base.LV * 10 + base.encLV * 10 + base.rarity * (Rarity)10 + (int)(base.blessedState * (BlessedState)10));
	}

	public override void SetSource()
	{
		this.source = EClass.sources.things.map.TryGetValue(this.id, null);
		if (this.source != null && this.source.isOrigin)
		{
			this.source = (EClass.sources.cards.firstVariations[this.id] as SourceThing.Row);
			this.id = this.source.id;
		}
		if (this.source == null)
		{
			Debug.LogWarning("Thing " + this.id + " not found");
			this.id = "1101";
			this.source = EClass.sources.things.map[this.id];
		}
	}

	public override void OnCreate(int genLv)
	{
		if (this.bp.blesstedState != null)
		{
			this.SetBlessedState(this.bp.blesstedState.GetValueOrDefault());
		}
		else if (base.category.ignoreBless == 0 && this.bp.rarity == Rarity.Random && base.rarity != Rarity.Artifact)
		{
			if (EClass.rnd(25) == 0)
			{
				this.SetBlessedState(BlessedState.Blessed);
			}
			else if (EClass.rnd(25) == 0)
			{
				this.SetBlessedState(BlessedState.Cursed);
			}
			else if (EClass.rnd(50) == 0 && base.category.slot != 0)
			{
				this.SetBlessedState(BlessedState.Doomed);
			}
		}
		if (!EClass.debug.autoIdentify && (!this.source.unknown_JP.IsEmpty() || (base.category.slot != 0 && base.rarity >= Rarity.Superior)))
		{
			base.c_IDTState = 5;
		}
		string id = this.id;
		if (id == "bill_tax" || id == "bill")
		{
			base.c_bill = 100 + EClass.rnd(100);
		}
		if (base.category.slot != 0)
		{
			int num = 0;
			if (base.rarity == Rarity.Superior)
			{
				num = EClass.rnd(3);
			}
			else if (base.rarity == Rarity.Legendary)
			{
				num = EClass.rnd(4) + 2;
			}
			else if (base.rarity == Rarity.Mythical)
			{
				num = EClass.rnd(3) + 5;
			}
			else if (base.rarity >= Rarity.Artifact)
			{
				num = EClass.rnd(2) + 1;
			}
			if (num > 0 && !base.HasTag(CTAG.godArtifact))
			{
				for (int i = 0; i < num; i++)
				{
					this.AddEnchant(genLv);
				}
			}
		}
		if (base.IsRangedWeapon && !this.IsMeleeWithAmmo)
		{
			if (base.HasTag(CTAG.godArtifact))
			{
				base.AddSocket();
				base.AddSocket();
			}
			else
			{
				int num2 = 1;
				int num3 = (EClass.rnd(10) == 0) ? 1 : 0;
				if (base.rarity == Rarity.Superior)
				{
					num2 = 2 + num3;
				}
				else if (base.rarity == Rarity.Legendary)
				{
					num2 = EClass.rnd(2) + 3 + num3;
				}
				else if (base.rarity == Rarity.Mythical)
				{
					num2 = EClass.rnd(2) + 4 + num3;
				}
				else if (base.rarity >= Rarity.Artifact)
				{
					num2 = EClass.rnd(2) + 1;
				}
				if (num2 > 0)
				{
					for (int j = 0; j < num2; j++)
					{
						base.AddSocket();
					}
					for (int k = 0; k < EClass.rnd(num2 + 1); k++)
					{
						Tuple<SourceElement.Row, int> enchant = Thing.GetEnchant(genLv, (SourceElement.Row r) => r.tag.Contains("modRanged"), false);
						if (enchant != null && InvOwnerMod.IsValidMod(this, enchant.Item1))
						{
							base.ApplySocket(enchant.Item1.id, enchant.Item2, null);
						}
					}
				}
			}
		}
		if ((this.bp.rarity != Rarity.Normal || this.bp.qualityBonus != 0) && base.rarity < Rarity.Artifact && base.category.tag.Contains("enc"))
		{
			int num4 = 0;
			if (EClass.rnd(6) == 0)
			{
				if (this.bp.qualityBonus == 0)
				{
					num4 = EClass.rnd(EClass.rnd(11) + 1);
					if (num4 == 1 && EClass.rnd(3) != 0)
					{
						num4 = 0;
					}
				}
				else if (this.bp.qualityBonus < 0)
				{
					if (EClass.rnd(3) == 0)
					{
						num4 = 1;
					}
				}
				else if (this.bp.qualityBonus >= 10)
				{
					num4 = Mathf.Min(this.bp.qualityBonus / 10 + 2, 7) + EClass.rnd(EClass.rnd(5) + 1);
				}
			}
			if (num4 > 0)
			{
				base.SetEncLv(Mathf.Min(num4, 12));
			}
		}
		if (base.HasTag(CTAG.randomSkin))
		{
			base.idSkin = EClass.rnd(this.source.skins.Length + 1);
		}
	}

	public override void ApplyMaterialElements(bool remove)
	{
		Chara chara = null;
		if (EClass.core.IsGameStarted && this.isEquipped)
		{
			Card rootCard = base.GetRootCard();
			chara = ((rootCard != null) ? rootCard.Chara : null);
			if (chara != null)
			{
				this.elements.SetParent(null);
			}
		}
		this.elements.ApplyMaterialElementMap(this, remove);
		if (chara != null)
		{
			this.elements.SetParent(chara);
		}
	}

	public override void ApplyMaterial(bool remove = false)
	{
		Thing.<>c__DisplayClass27_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		if (this.source.HasTag(CTAG.replica))
		{
			base.isReplica = true;
		}
		if (remove)
		{
			this.ApplyMaterialElements(true);
			base.isAcidproof = (base.isFireproof = false);
			return;
		}
		CS$<>8__locals1.pvSet = false;
		CS$<>8__locals1.dmgSet = false;
		CS$<>8__locals1.hitSet = false;
		if (this.sourceCard.quality == 4)
		{
			if (this.source.offense.Length != 0)
			{
				base.c_diceDim = this.source.offense[1];
			}
			if (this.source.offense.Length > 2)
			{
				this.<ApplyMaterial>g__SetBase|27_0(66, this.source.offense[2], ref CS$<>8__locals1);
			}
			if (this.source.offense.Length > 3)
			{
				this.<ApplyMaterial>g__SetBase|27_0(67, this.source.offense[3], ref CS$<>8__locals1);
			}
			if (this.source.defense.Length != 0)
			{
				this.<ApplyMaterial>g__SetBase|27_0(64, this.source.defense[0], ref CS$<>8__locals1);
			}
			if (this.source.defense.Length > 1)
			{
				this.<ApplyMaterial>g__SetBase|27_0(65, this.source.defense[1], ref CS$<>8__locals1);
			}
		}
		else
		{
			int num = 120;
			bool flag = !base.IsAmmo;
			if (base.rarity <= Rarity.Crude)
			{
				num = 150;
			}
			else if (base.rarity == Rarity.Superior)
			{
				num = 100;
			}
			else if (base.rarity >= Rarity.Legendary)
			{
				num = 80;
			}
			if (this.source.offense.Length != 0)
			{
				base.c_diceDim = this.source.offense[1] * base.material.dice / (num + (flag ? EClass.rnd(25) : 0));
			}
			if (this.source.offense.Length > 2)
			{
				this.<ApplyMaterial>g__SetBase|27_0(66, this.source.offense[2] * base.material.atk * 9 / (num - (flag ? EClass.rnd(30) : 0)), ref CS$<>8__locals1);
			}
			if (this.source.offense.Length > 3)
			{
				this.<ApplyMaterial>g__SetBase|27_0(67, this.source.offense[3] * base.material.dmg * 5 / (num - (flag ? EClass.rnd(30) : 0)), ref CS$<>8__locals1);
			}
			if (this.source.defense.Length != 0)
			{
				this.<ApplyMaterial>g__SetBase|27_0(64, this.source.defense[0] * base.material.dv * 7 / (num - (flag ? EClass.rnd(30) : 0)), ref CS$<>8__locals1);
			}
			if (this.source.defense.Length > 1)
			{
				this.<ApplyMaterial>g__SetBase|27_0(65, this.source.defense[1] * base.material.pv * 9 / (num - (flag ? EClass.rnd(30) : 0)), ref CS$<>8__locals1);
			}
		}
		if (base.isReplica)
		{
			if (this.source.offense.Length != 0)
			{
				base.c_diceDim = Mathf.Max(this.source.offense[1] / 3, 1);
			}
			if (this.source.offense.Length > 2)
			{
				this.<ApplyMaterial>g__SetBase|27_0(66, this.source.offense[2] / 3, ref CS$<>8__locals1);
			}
			if (this.source.offense.Length > 3)
			{
				this.<ApplyMaterial>g__SetBase|27_0(67, this.source.offense[3] / 3, ref CS$<>8__locals1);
			}
			if (this.source.defense.Length != 0)
			{
				this.<ApplyMaterial>g__SetBase|27_0(64, this.source.defense[0] / 3, ref CS$<>8__locals1);
			}
			if (this.source.defense.Length > 1)
			{
				this.<ApplyMaterial>g__SetBase|27_0(65, this.source.defense[1] / 3, ref CS$<>8__locals1);
			}
		}
		if (base.IsEquipmentOrRanged || base.IsAmmo)
		{
			if (base.IsWeapon || base.IsAmmo)
			{
				if (CS$<>8__locals1.dmgSet)
				{
					this.elements.ModBase(67, base.encLV + ((base.blessedState == BlessedState.Blessed) ? 1 : 0));
				}
			}
			else if (CS$<>8__locals1.pvSet)
			{
				this.elements.ModBase(65, (base.encLV + ((base.blessedState == BlessedState.Blessed) ? 1 : 0)) * 2);
			}
		}
		if (this.sockets != null)
		{
			for (int i = 0; i < this.sockets.Count; i++)
			{
				int num2 = this.sockets[i];
				int num3 = num2 / 100;
				if (num3 == 67 & CS$<>8__locals1.dmgSet)
				{
					this.elements.ModBase(67, num2 % 100);
				}
				if (num3 == 66 & CS$<>8__locals1.hitSet)
				{
					this.elements.ModBase(66, num2 % 100);
				}
				if (num3 == 65 & CS$<>8__locals1.pvSet)
				{
					this.elements.ModBase(65, num2 % 100);
				}
			}
		}
		if (base.material == null || base.material.elements == null)
		{
			string[] array = new string[5];
			array[0] = base.idMaterial.ToString();
			array[1] = "/";
			int num4 = 2;
			SourceMaterial.Row material = base.material;
			array[num4] = ((material != null) ? material.name : null);
			array[3] = "/";
			int num5 = 4;
			SourceMaterial.Row material2 = base.material;
			int[] array2 = (material2 != null) ? material2.elements : null;
			array[num5] = ((array2 != null) ? array2.ToString() : null);
			Debug.Log(string.Concat(array));
		}
		this.ApplyMaterialElements(false);
		foreach (string a in base.material.bits)
		{
			if (!(a == "fire"))
			{
				if (a == "acid")
				{
					base.isAcidproof = true;
				}
			}
			else
			{
				base.isFireproof = true;
			}
		}
		if (base.rarity >= Rarity.Artifact)
		{
			base.isAcidproof = (base.isFireproof = true);
		}
		this._colorInt = 0;
	}

	public override string GetName(NameStyle style, int _num = -1)
	{
		int num = (_num == -1) ? base.Num : _num;
		string text = "";
		string text2 = "";
		string text3 = "";
		string str = "";
		string sig = "";
		string text4 = "";
		string text5 = this.source.GetText("unit", false);
		ArticleStyle style2 = (style == NameStyle.FullNoArticle) ? ArticleStyle.None : ArticleStyle.Default;
		bool flag = base.IsIdentified || this.source.unknown.IsEmpty();
		bool isEquipmentOrRanged = base.IsEquipmentOrRanged;
		bool flag2 = Lang.setting.nameStyle == 0;
		if (flag)
		{
			if (base.c_idRefCard.IsEmpty() && !base.c_altName.IsEmpty())
			{
				text = base.c_altName;
			}
			else
			{
				string[] array = this.trait.GetName().Split(',', StringSplitOptions.None);
				text = array[0];
				if (array.Length > 1)
				{
					text5 = array[1];
				}
			}
			if (text.IsEmpty())
			{
				text = this.id;
			}
			if (isEquipmentOrRanged && base.IsIdentified && base.rarity >= Rarity.Legendary)
			{
				if (base.rarity != Rarity.Artifact && !base.material.GetTextArray("altName").IsEmpty())
				{
					text = base.material.GetTextArray("altName")[0] + Lang.space + text;
				}
			}
			else
			{
				if (this.source.naming == "m" || (this.source.naming == "ms" && base.material != this.source.DefaultMaterial))
				{
					if (isEquipmentOrRanged)
					{
						string[] textArray = base.material.GetTextArray("altName");
						if (textArray != null && textArray.Length >= 2)
						{
							text = base.material.GetTextArray("altName")[1] + Lang.space + text;
							goto IL_1F2;
						}
					}
					text = "_of2".lang(base.material.GetName(), text, null, null, null);
				}
				IL_1F2:
				if (this.source.naming == "ma")
				{
					text = base.material.GetName();
				}
				if (base.qualityTier > 0)
				{
					text = Lang.GetList("quality_general")[Mathf.Clamp(base.qualityTier, 0, 3)] + text;
				}
			}
		}
		else
		{
			text = "unknown";
			string idUnknown = this.source.GetText("unknown", false);
			if (idUnknown.StartsWith("#"))
			{
				Rand.UseSeed(EClass.game.seed + (this.trait.CanStack ? this.sourceCard._index : base.uid) + base.refVal, delegate
				{
					idUnknown = Lang.GetList(idUnknown.Remove(0, 1)).RandomItem<string>();
				});
			}
			text = idUnknown;
		}
		if (!base.c_idRefCard.IsEmpty() && this.trait.RefCardName != RefCardName.None)
		{
			string text6 = base.c_altName.IsEmpty(EClass.sources.cards.map[base.c_idRefCard].GetName());
			if (!base.c_idRefCard2.IsEmpty())
			{
				text6 = "_and".lang(text6, base.c_altName2.IsEmpty(EClass.sources.cards.map[base.c_idRefCard2].GetName()), null, null, null);
			}
			if (text6 == "*r")
			{
				string text7 = EClass.sources.cards.map[base.c_idRefCard].GetText("aka", false);
				if (!text7.IsEmpty())
				{
					text = "_of".lang(text7, text, null, null, null);
				}
			}
			else if (!this.source.name2.IsEmpty())
			{
				text = this.source.GetTextArray("name2")[0].Replace("#1", text6);
			}
			else if (this.source.naming.Contains("last"))
			{
				text = text + Lang.space + text6;
			}
			else if (this.source.naming.Contains("first"))
			{
				text = text6 + Lang.space + text;
			}
			else if (this.source.naming.Contains("of"))
			{
				text = "_of".lang(text6, text, null, null, null);
			}
			else
			{
				text = (text5.IsEmpty() ? "_of3" : "_of2").lang(text6, text, null, null, null);
			}
		}
		if (base.c_bill != 0)
		{
			text = "_of".lang(Lang._currency(base.c_bill, true, 0), text, null, null, null);
		}
		this.trait.SetName(ref text);
		if (style == NameStyle.Simple)
		{
			return text;
		}
		if (style == NameStyle.Ref)
		{
			return text;
		}
		if (!base.c_refText.IsEmpty())
		{
			text = "_named".lang(base.c_refText, text, null, null, null);
		}
		if (base.IsIdentified)
		{
			int hit = base.HIT;
			int dmg = base.DMG;
			if ((base.IsMeleeWeapon || base.IsRangedWeapon || base.IsAmmo || hit != 0 || dmg != 0) && this.source.offense.Length != 0)
			{
				string text8 = "";
				if (this.source.offense[0] != 0)
				{
					text8 = text8 + this.source.offense[0].ToString() + "d" + base.c_diceDim.ToString();
				}
				if (dmg != 0)
				{
					text8 += ((base.IsMeleeWeapon || base.IsRangedWeapon || base.IsAmmo) ? dmg.ToText(true) : (dmg.ToString() ?? ""));
				}
				if (hit != 0)
				{
					text8 = text8 + ((dmg != 0 || this.source.offense[0] != 0) ? ", " : "") + hit.ToString();
				}
				text2 = text2 + " (" + text8.IsEmpty(" - ") + ") ";
			}
			int dv = this.DV;
			int pv = this.PV;
			if (dv != 0 || pv != 0)
			{
				text2 += " [";
				text2 = text2 + dv.ToString() + ", " + pv.ToString();
				text2 += "] ";
			}
			if (this.trait.HasCharges && this.trait.ShowCharges)
			{
				text2 = text2 + " " + "itemCharges".lang(base.c_charges.ToString() ?? "", null, null, null, null);
			}
		}
		else if (base.c_IDTState == 3 || base.c_IDTState == 1)
		{
			text2 = "(" + base.TextRarity.ToTitleCase(false) + ")";
		}
		if (base.IsDecayed)
		{
			text = "rotten".lang() + text;
		}
		else if (base.IsRotting)
		{
			text = "rotting".lang() + text;
		}
		if (base.IsIdentified)
		{
			if (base.blessedState != BlessedState.Normal)
			{
				str = ("bs" + base.blessedState.ToString()).lang();
			}
			Rarity rarity = base.rarity;
			if (rarity - Rarity.Legendary > 1)
			{
				if (rarity == Rarity.Artifact)
				{
					style2 = ArticleStyle.None;
					text3 = "★";
					text = (isEquipmentOrRanged ? text.Bracket(3) : text);
				}
			}
			else
			{
				style2 = ArticleStyle.The;
				text3 = "☆";
				if (isEquipmentOrRanged)
				{
					Rand.UseSeed(base.uid + EClass.game.seed, delegate
					{
						sig = AliasGen.GetRandomAlias().Bracket((this.rarity == Rarity.Mythical) ? 3 : 2);
					});
					sig = Lang.space + sig;
				}
			}
		}
		if (base.encLV != 0)
		{
			if (base.category.tag.Contains("enc"))
			{
				if (base.c_altName.IsEmpty())
				{
					string[] list = Lang.GetList("quality_furniture");
					text = "_qualityFurniture".lang(list[Mathf.Clamp(base.encLV - 1, 0, list.Length - 1)], text, null, null, null);
				}
			}
			else
			{
				sig = sig + Lang.space + ((base.encLV > 0) ? ("+" + base.encLV.ToString()) : (base.encLV.ToString() ?? ""));
			}
		}
		if (base.c_lockLv != 0 && base.c_revealLock)
		{
			sig = sig + Lang.space + "+" + base.c_lockLv.ToString();
		}
		if (base.isLostProperty)
		{
			text = "_lostproperty".lang(text, null, null, null, null);
		}
		if (this.trait is TraitEquipItem && EClass.player.eqBait == this && EClass.player.eqBait.GetRootCard() == EClass.pc)
		{
			text4 += "equippedItem".lang();
		}
		if (!base.c_note.IsEmpty() && (!base.isBackerContent || EClass.core.config.backer.Show(base.c_note)))
		{
			string text9 = base.c_note;
			if (text9.StartsWith('@'))
			{
				LangNote.Row row = Lang.Note.map.TryGetValue(text9.TrimStart('@'), null);
				text9 = (((row != null) ? row.GetText("text", false) : null) ?? base.c_note);
			}
			string text10 = base.category.IsChildOf("book") ? "_written" : "_engraved";
			if (this.id == "grave_dagger1" || this.id == "grave_dagger2")
			{
				text10 = "_daggerGrave";
			}
			if (text9.Contains("_bracketLeft".lang()))
			{
				text = (text10 + "Alt").lang(text9, text, null, null, null);
			}
			else
			{
				text = text10.lang(text9, text, null, null, null);
			}
		}
		if (flag2)
		{
			if (num > 1)
			{
				text = "_unit".lang(num.ToFormat() ?? "", str + text, text5, null, null);
			}
			else
			{
				text = str + text;
			}
		}
		else if (this.trait is TraitAbility)
		{
			text = text.ToTitleCase(true);
		}
		else if (text5.IsEmpty() || (!base.IsIdentified && !this.source.unknown.IsEmpty()))
		{
			text = (str + text).AddArticle(num, style2, this.source.name);
		}
		else
		{
			text = "_unit".lang((num == 1) ? "" : (num.ToFormat() ?? ""), text, (str + text5).AddArticle(num, style2, this.source.unit), null, null);
		}
		if (base.rarity >= Rarity.Legendary)
		{
			text = text.ToTitleCase(true);
		}
		string text11 = (base.isSale && this.things.Count > 0) ? "forSale2".lang() : ((base.isSale || (base.parentThing != null && base.parentThing.isSale && TraitSalesTag.CanTagSale(this, true))) ? "forSale".lang(Lang._currency(this.GetPrice(CurrencyType.Money, true, PriceType.PlayerShop, null), "money"), null, null, null, null) : "");
		if (this.trait is TraitSalesTag && base.isOn && !base.GetStr(11, null).IsEmpty())
		{
			text11 += "soldOut".lang(EClass.sources.categories.map[base.GetStr(11, null)].GetName(), null, null, null, null);
		}
		if (base.GetInt(101, null) != 0)
		{
			text4 = "_limitedStock".lang(text4, null, null, null, null);
		}
		return string.Concat(new string[]
		{
			text4,
			text3,
			text,
			sig,
			text2,
			text11
		});
	}

	public override string GetHoverText()
	{
		string text = "";
		text = text + " <size=14>(" + Lang._weight(base.ChildrenAndSelfWeight, true, 0) + ")</size> ";
		if (EClass.debug.showExtra)
		{
			text += Environment.NewLine;
			string[] array = new string[15];
			array[0] = text;
			array[1] = "id:";
			array[2] = this.id;
			array[3] = "  tile:";
			array[4] = this.source.idRenderData;
			array[5] = "/";
			int num = 6;
			object obj = (this.source.tiles.Length != 0) ? this.source.tiles[0] : "-";
			array[num] = ((obj != null) ? obj.ToString() : null);
			array[7] = " num:";
			array[8] = base.Num.ToString();
			array[9] = " lv:";
			array[10] = base.LV.ToString();
			array[11] = " enc:";
			array[12] = base.encLV.ToString();
			array[13] = " / ";
			array[14] = base.material.alias;
			text = string.Concat(array);
		}
		string hoverText = this.trait.GetHoverText();
		if (!hoverText.IsEmpty())
		{
			text = text + Environment.NewLine + hoverText;
		}
		return base.GetHoverText() + text;
	}

	public override string GetExtraName()
	{
		string text = "";
		if (this.trait.ShowChildrenNumber && base.c_lockLv == 0)
		{
			if (this.things.Count > 0)
			{
				text += "childCount".lang(this.things.Count.ToString() ?? "", null, null, null, null);
			}
			else if (this.trait.CanOpenContainer)
			{
				text += "empty".lang();
			}
		}
		if ((this.trait is TraitRoomPlate || this.trait is TraitHouseBoard) && this.pos.IsValid)
		{
			Room room = this.pos.cell.room;
			if (EClass.debug.enable && room != null && room.data.group != 0)
			{
				text = text + " #" + room.data.group.ToString();
			}
		}
		return text;
	}

	public List<Element> ListLimitedValidTraits(bool limit)
	{
		List<Element> list = new List<Element>();
		if (base.ShowFoodEnc)
		{
			foreach (Element element in this.elements.dict.Values)
			{
				if (element.IsFoodTraitMain && element.Value > 0)
				{
					list.Add(element);
				}
			}
			list.Sort((Element a, Element b) => ElementContainer.GetSortVal(b) - ElementContainer.GetSortVal(a));
			if (limit && list.Count > 5)
			{
				int num = list.Count - 5;
				for (int i = 0; i < num; i++)
				{
					list.RemoveAt(list.Count - 1);
				}
			}
		}
		return list;
	}

	public List<Element> ListValidTraits(bool isCraft, bool limit)
	{
		List<Element> list = this.ListLimitedValidTraits(limit);
		bool showFoodEnc = base.ShowFoodEnc;
		bool flag = EClass.pc.HasElement(1650, 1);
		if (showFoodEnc)
		{
			foreach (Element element in this.elements.dict.Values)
			{
				if (element.IsFoodTrait && !list.Contains(element) && (isCraft || flag || element.IsFoodTraitMain) && (!element.IsFoodTraitMain || element.Value < 0))
				{
					list.Add(element);
				}
			}
		}
		foreach (Element element2 in this.elements.dict.Values)
		{
			if ((isCraft || flag || ((!element2.IsFoodTrait || element2.IsFoodTraitMain) && (!showFoodEnc || !element2.IsTrait || element2.Value >= 0))) && !list.Contains(element2) && (element2.IsTrait || (element2.IsFoodTrait && !element2.IsFoodTraitMain)))
			{
				list.Add(element2);
			}
		}
		return list;
	}

	public override void WriteNote(UINote n, Action<UINote> onWriteNote = null, IInspect.NoteMode mode = IInspect.NoteMode.Default, Recipe recipe = null)
	{
		i.Clear();
		TraitAbility traitAbility = this.trait as TraitAbility;
		bool showEQStats = base.IsEquipmentOrRanged || base.IsAmmo;
		bool flag = mode == IInspect.NoteMode.Product;
		bool flag2 = base.IsIdentified || flag;
		string text = base.Name;
		if (base.rarity == Rarity.Legendary || base.rarity == Rarity.Mythical)
		{
			string text2 = text.Contains("『") ? "『" : (text.Contains("《") ? "《" : "");
			if (text2 != "")
			{
				string[] array = text.Split(text2, StringSplitOptions.None);
				text = array[0] + Environment.NewLine + text2 + array[1];
			}
		}
		if (flag)
		{
			text = recipe.GetName();
		}
		if (mode != IInspect.NoteMode.Recipe)
		{
			UIItem uiitem = i.AddHeaderCard(text, null);
			this.SetImage(uiitem.image2);
			uiitem.image2.Rect().pivot = new Vector2(0.5f, 0.5f);
			string text3 = base.Num.ToFormat() ?? "";
			string text4 = (Mathf.Ceil(0.01f * (float)base.ChildrenAndSelfWeight) * 0.1f).ToString("F1") + "s";
			if (this.things.Count > 0)
			{
				text3 = text3 + " (" + this.things.Count.ToString() + ")";
			}
			if (base.ChildrenAndSelfWeight != this.SelfWeight)
			{
				text4 = text4 + " (" + (Mathf.Ceil(0.01f * (float)this.SelfWeight) * 0.1f).ToString("F1") + "s)";
			}
			text = "_quantity".lang(text3 ?? "", text4, null, null, null);
			if (flag && recipe != null && LayerCraft.Instance)
			{
				text = text + "  " + "_recipe_lv".lang(recipe.RecipeLv.ToString() ?? "", null, null, null, null);
			}
			uiitem.text2.SetText(text);
			if (showEQStats && flag2)
			{
				if (!flag)
				{
					text = "";
					if (this.DV != 0 || this.PV != 0 || base.HIT != 0 || base.DMG != 0 || this.Penetration != 0)
					{
						if (base.DMG != 0)
						{
							text = string.Concat(new string[]
							{
								text,
								"DMG".lang(),
								(base.DMG > 0) ? "+" : "",
								base.DMG.ToString(),
								", "
							});
						}
						if (base.HIT != 0)
						{
							text = string.Concat(new string[]
							{
								text,
								"HIT".lang(),
								(base.HIT > 0) ? "+" : "",
								base.HIT.ToString(),
								", "
							});
						}
						if (this.DV != 0)
						{
							text = string.Concat(new string[]
							{
								text,
								"DV".lang(),
								(this.DV > 0) ? "+" : "",
								this.DV.ToString(),
								", "
							});
						}
						if (this.PV != 0)
						{
							text = string.Concat(new string[]
							{
								text,
								"PV".lang(),
								(this.PV > 0) ? "+" : "",
								this.PV.ToString(),
								", "
							});
						}
						if (this.Penetration != 0)
						{
							text = string.Concat(new string[]
							{
								text,
								"PEN".lang(),
								(this.Penetration > 0) ? "+" : "",
								this.Penetration.ToString(),
								"%, "
							});
						}
						text = text.TrimEnd(' ').TrimEnd(',');
					}
					if (!text.IsEmpty())
					{
						i.AddText("NoteText_eqstats", text, FontColor.DontChange);
					}
				}
				TraitToolRange traitToolRange = this.trait as TraitToolRange;
				if (traitToolRange != null)
				{
					i.AddText("NoteText_eqstats", "tip_range".lang(traitToolRange.BestDist.ToString() ?? "", null, null, null, null), FontColor.DontChange);
				}
			}
			else
			{
				string text5 = "";
				if (EClass.debug.showExtra)
				{
					int totalQuality = base.GetTotalQuality(true);
					int totalQuality2 = base.GetTotalQuality(false);
					text5 = string.Concat(new string[]
					{
						text5,
						"Lv. ",
						base.LV.ToString(),
						" TQ. ",
						base.GetTotalQuality(true).ToString(),
						(totalQuality == totalQuality2) ? "" : (" (" + totalQuality2.ToString() + ")")
					});
				}
				if (base.HasElement(10, 1))
				{
					text5 = text5 + (text5.IsEmpty() ? "" : "  ") + "_nutrition".lang(base.Evalue(10).ToFormat() ?? "", null, null, null, null);
				}
				if ((base.category.IsChildOf("resource") || this.trait.IsTool) && !(this.trait is TraitAbility))
				{
					text5 = text5 + (text5.IsEmpty() ? "" : "  ") + "_hardness".lang(base.material.hardness.ToString() ?? "", null, null, null, null);
				}
				if (flag && recipe != null && LayerCraft.Instance)
				{
					text5 = text5 + (text5.IsEmpty() ? "" : "  ") + "_max_quality".lang(recipe.GetQualityBonus().ToString() ?? "", null, null, null, null);
				}
				if (!text5.IsEmpty())
				{
					i.AddText("NoteText_eqstats", text5, FontColor.DontChange);
				}
			}
			string detail = this.GetDetail();
			if (!detail.IsEmpty())
			{
				LayoutElement component = i.AddText("NoteText_flavor", detail, FontColor.DontChange).GetComponent<LayoutElement>();
				if (flag)
				{
					component.preferredWidth = 400f;
				}
				i.Space(8, 1);
			}
		}
		if (this.trait is TraitBookPlan)
		{
			TraitBookPlan traitBookPlan = this.trait as TraitBookPlan;
			i.AddText("NoteText_flavor", traitBookPlan.source.GetDetail(), FontColor.DontChange);
			i.Space(8, 1);
		}
		if (traitAbility != null)
		{
			i.Space(8, 1);
			Act act = traitAbility.CreateAct();
			Element orCreateElement = EClass.pc.elements.GetOrCreateElement(act.source.id);
			orCreateElement._WriteNote(i, EClass.pc.elements, null, false, false);
			orCreateElement._WriteNote(i, EClass.pc, act);
			return;
		}
		if (EClass.debug.showExtra)
		{
			string[] array2 = new string[8];
			array2[0] = "(id:";
			array2[1] = this.id;
			array2[2] = " tile:";
			int num = 3;
			string text6 = this.source.tiles.IsEmpty() ? "-" : this.source.tiles[0];
			array2[num] = ((text6 != null) ? text6.ToString() : null);
			array2[4] = ") lv:";
			array2[5] = base.LV.ToString();
			array2[6] = " price:";
			array2[7] = this.GetPrice(CurrencyType.Money, false, PriceType.Default, null).ToString();
			i.AddText(string.Concat(array2), FontColor.DontChange);
		}
		Card rootCard = base.GetRootCard();
		if (rootCard != null && rootCard != EClass.pc && rootCard != this && rootCard.ExistsOnMap)
		{
			Thing thing = this.parent as Thing;
			if (!(((thing != null) ? thing.trait : null) is TraitChestMerchant))
			{
				i.AddText("isChildOf".lang(base.GetRootCard().Name, null, null, null, null), FontColor.ItemName);
			}
		}
		if (flag2)
		{
			i.AddText("isMadeOf".lang(base.material.GetText("name", false), base.material.hardness.ToString() ?? "", null, null, null), FontColor.DontChange);
		}
		i.AddText("isCategorized".lang(base.category.GetText("name", false), null, null, null, null), FontColor.DontChange);
		if (base.category.skill != 0)
		{
			int key = base.category.skill;
			int key2 = 132;
			if (base.IsRangedWeapon)
			{
				key2 = 133;
			}
			if (this.trait is TraitToolRangeCane)
			{
				key2 = 304;
			}
			if (base.Evalue(482) > 0)
			{
				key = 305;
				key2 = 304;
			}
			i.AddText("isUseSkill".lang(EClass.sources.elements.map[key].GetName().ToTitleCase(true), EClass.sources.elements.map[key2].GetName().ToTitleCase(true), null, null, null), FontColor.DontChange);
		}
		if (base.IsContainer)
		{
			i.AddText("isContainer".lang(this.things.MaxCapacity.ToString() ?? "", null, null, null, null), FontColor.DontChange);
		}
		if (base.c_lockLv != 0)
		{
			i.AddText((base.c_lockedHard ? "isLockedHard" : "isLocked").lang(base.c_lockLv.ToString() ?? "", null, null, null, null), FontColor.Warning);
		}
		if (base.isCrafted && recipe == null)
		{
			i.AddText("isCrafted".lang(), FontColor.DontChange);
		}
		if (this.trait.Decay > 0)
		{
			string s2;
			if (base.IsDecayed)
			{
				s2 = "isRotten";
			}
			else if (base.IsRotting)
			{
				s2 = "isRotting";
			}
			else if (base.IsFresn)
			{
				s2 = "isFresh";
			}
			else
			{
				s2 = "isNotFresh";
			}
			i.AddText(s2.lang(), FontColor.DontChange);
		}
		if (base.isDyed)
		{
			i.AddText("isDyed".lang(), FontColor.DontChange);
		}
		if (base.IsEquipment)
		{
			text = "isEquipable".lang(Element.Get(base.category.slot).GetText("name", false), null, null, null, null);
			i.AddText(text, FontColor.DontChange);
		}
		if (base.isFireproof)
		{
			i.AddText("isFreproof", FontColor.DontChange);
		}
		if (base.isAcidproof)
		{
			i.AddText("isAcidproof", FontColor.DontChange);
		}
		if (this.trait.Electricity > 0)
		{
			i.AddText("isGenerateElectricity".lang(this.trait.Electricity.ToString() ?? "", null, null, null, null), FontColor.DontChange);
		}
		if (this.trait.Electricity < 0)
		{
			i.AddText("isConsumeElectricity".lang(Mathf.Abs(this.trait.Electricity).ToString() ?? "", null, null, null, null), FontColor.DontChange);
		}
		if (base.IsUnique)
		{
			i.AddText("isPrecious", FontColor.DontChange);
		}
		if (base.isCopy)
		{
			i.AddText("isCopy", FontColor.DontChange);
		}
		if (!this.trait.CanBeDestroyed)
		{
			i.AddText("isIndestructable", FontColor.DontChange);
		}
		if (base.GetInt(107, null) > 0)
		{
			i.AddText("isLicked", FontColor.DontChange);
		}
		if (!base.c_idDeity.IsEmpty())
		{
			Religion religion = EClass.game.religions.Find(base.c_idDeity) ?? EClass.game.religions.Eyth;
			i.AddText("isDeity".lang(religion.Name, null, null, null, null), FontColor.Myth);
		}
		if (base.isGifted && base.GetRoot() != EClass.pc)
		{
			i.AddText("isGifted", FontColor.Ether);
		}
		if (base.isNPCProperty)
		{
			i.AddText("isNPCProperty", FontColor.Ether);
		}
		if (base.c_priceFix != 0)
		{
			i.AddText(((base.c_priceFix > 0) ? "isPriceUp" : "isPriceDown").lang(Mathf.Abs(base.c_priceFix).ToString() ?? "", null, null, null, null), FontColor.Ether);
		}
		if (base.noSell)
		{
			i.AddText("isNoSell", FontColor.Ether);
		}
		if (base.isStolen)
		{
			i.AddText("isStolen", FontColor.Ether);
		}
		if (base.c_isImportant)
		{
			i.AddText("isMarkedImportant", FontColor.Ether);
		}
		if (base.GetInt(25, null) != 0)
		{
			i.AddText("isDangerLv".lang((base.GetInt(25, null) + 1).ToString() ?? "", (EClass.pc.FameLv + 10).ToString() ?? "", null, null, null), FontColor.DontChange);
		}
		FontColor color = FontColor.Util;
		if (this.trait is TraitTool && !(this.trait is TraitToolRange))
		{
			if (base.HasElement(220, 1))
			{
				i.AddText("canMine".lang(), color);
			}
			if (base.HasElement(225, 1))
			{
				i.AddText("canLumberjack".lang(), color);
				i.AddText("canLumberjack2".lang(), color);
			}
			if (base.HasElement(230, 1))
			{
				i.AddText("canDig", color);
			}
			if (base.HasElement(286, 1))
			{
				i.AddText("canFarm", color);
			}
			if (base.HasElement(245, 1))
			{
				i.AddText("canFish", color);
			}
			if (base.HasElement(237, 1))
			{
				i.AddText("canTame", color);
			}
		}
		if (this.trait is TraitToolMusic)
		{
			i.AddText("canPlayMusic".lang(), color);
		}
		if (Lang.Has("hint_" + this.trait.ToString()))
		{
			i.AddText("hint_" + this.trait.ToString(), FontColor.Util);
		}
		if (Lang.Has("hint_" + this.trait.ToString() + "2"))
		{
			i.AddText("hint_" + this.trait.ToString() + "2", FontColor.Util);
		}
		if (base.HasTag(CTAG.tourism))
		{
			i.AddText("isTourism", FontColor.Util);
		}
		string langPlaceType = base.TileType.LangPlaceType;
		if (langPlaceType == "place_Door" || langPlaceType == "place_WallMount")
		{
			i.AddText(base.TileType.LangPlaceType + "_hint".lang(), FontColor.Util);
		}
		if (this.trait.IsHomeItem)
		{
			i.AddText("isHomeItem".lang(), FontColor.Util);
		}
		if (base.HasTag(CTAG.throwWeapon))
		{
			i.AddText("isThrowWeapon", FontColor.DontChange);
		}
		if (EClass.debug.showExtra && base.HasTag(CTAG.throwWeaponEnemy))
		{
			i.AddText("isThrowWeaponEnemy", FontColor.DontChange);
		}
		if (base.HasElement(10, 1))
		{
			i.AddText("isEdible", FontColor.DontChange);
		}
		if (base.HasTag(CTAG.rareResource))
		{
			i.AddText("isRareResource", FontColor.Great);
		}
		TraitBed traitBed = this.trait as TraitBed;
		if (traitBed != null)
		{
			i.AddText("isBed".lang(traitBed.MaxHolders.ToString() ?? "", null, null, null, null), FontColor.DontChange);
		}
		bool flag3 = base.IsEquipmentOrRanged || base.IsAmmo || base.IsThrownWeapon;
		if (flag2)
		{
			if (flag3)
			{
				Element element = this.elements.GetElement(653);
				if (element != null)
				{
					i.AddText("isAlive".lang(element.vBase.ToString() ?? "", (element.vExp / 10).ToString() ?? "", (element.ExpToNext / 10).ToString() ?? "", null, null), FontColor.Great);
				}
				string[] rangedSubCats = new string[]
				{
					"eleConvert",
					"eleAttack"
				};
				this.elements.AddNote(i, (Element e) => (!(this.trait is TraitToolRange) || this.category.slot != 0 || e is Ability || rangedSubCats.Contains(e.source.categorySub) || e.HasTag("modRanged")) && !e.IsTrait && (!(e.source.categorySub == "eleAttack") || this.IsWeapon || this.IsRangedWeapon || this.IsAmmo || this.IsThrownWeapon) && (!showEQStats || (e.id != 64 && e.id != 65 && e.id != 66 && e.id != 67)), null, ElementContainer.NoteMode.Default, false, null, null);
			}
			if (this.sockets == null)
			{
				goto IL_11FC;
			}
			using (List<int>.Enumerator enumerator = this.sockets.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int num2 = enumerator.Current;
					i.AddText((num2 == 0) ? "emptySocket".lang() : "socket".lang(EClass.sources.elements.map[num2 / 100].GetName(), (num2 % 100).ToString() ?? "", null, null, null), FontColor.Gray);
				}
				goto IL_11FC;
			}
		}
		i.AddText("isUnidentified".lang(), FontColor.Flavor);
		if (base.c_IDTState == 1)
		{
			i.AddText("isUnidentified2".lang(), FontColor.Flavor);
		}
		IL_11FC:
		this.trait.WriteNote(i, flag2);
		if (flag2 && !flag3)
		{
			bool infoMode = mode == IInspect.NoteMode.Info;
			List<Element> list = this.ListValidTraits(false, !infoMode);
			List<Element> list2 = this.ListValidTraits(false, false);
			if (list2.Count - list.Count <= 1)
			{
				list = list2;
			}
			this.elements.AddNote(i, (Element e) => list.Contains(e), null, ElementContainer.NoteMode.Trait, false, delegate(Element e, string s)
			{
				string text8 = s;
				string text9 = e.source.GetText("textExtra", false);
				if (!text9.IsEmpty())
				{
					string text10 = "";
					if (e.id == 2 && mode == IInspect.NoteMode.Product)
					{
						int num3 = recipe.GetQualityBonus() / 10;
						if (num3 >= 0)
						{
							num3++;
						}
						text10 = "qualityLimit".lang(num3.ToString() ?? "", null, null, null, null);
					}
					int num4 = e.Value / 10;
					if (e.Value >= 0)
					{
						num4++;
					}
					else
					{
						num4--;
					}
					text9 = string.Concat(new string[]
					{
						"Lv.",
						num4.ToString(),
						text10,
						" ",
						text9
					});
					if (infoMode && e.IsFoodTraitMain)
					{
						text9 += "traitAdditive".lang();
					}
					text8 += (" <size=12>" + text9 + "</size>").TagColor(FontColor.Passive, null);
				}
				return text8;
			}, delegate(UINote n, Element e)
			{
			});
			if (base.ShowFoodEnc && EClass.pc.HasElement(1650, 1))
			{
				if (FoodEffect.IsHumanFlesh(this))
				{
					i.AddText("foodHuman".lang(), FontColor.Ether);
				}
				if (FoodEffect.IsUndeadFlesh(this))
				{
					i.AddText("foodUndead".lang(), FontColor.Ether);
				}
			}
			if (list.Count != list2.Count)
			{
				i.AddText("traitOther".lang((list2.Count - list.Count).ToString() ?? "", null, null, null, null), FontColor.DontChange);
			}
			if (mode == IInspect.NoteMode.Product && base.HasTag(CTAG.dish_bonus))
			{
				i.AddHeader("HeaderAdditionalTrait", "additional_trait", null);
				this.source.model.elements.AddNote(i, (Element e) => e.IsFoodTraitMain, null, ElementContainer.NoteMode.Trait, false, delegate(Element e, string s)
				{
					string text8 = s;
					string text9 = e.source.GetText("textExtra", false);
					if (!text9.IsEmpty())
					{
						string text10 = "";
						int num3 = e.Value / 10;
						if (e.Value >= 0)
						{
							num3++;
						}
						else
						{
							num3--;
						}
						text9 = string.Concat(new string[]
						{
							"Lv.",
							num3.ToString(),
							text10,
							" ",
							text9
						});
						if (infoMode && e.IsFoodTraitMain)
						{
							text9 += "traitAdditive".lang();
						}
						text8 += (" <size=12>" + text9 + "</size>").TagColor(FontColor.Passive, null);
					}
					return text8;
				}, null);
			}
		}
		if (EClass.debug.showExtra)
		{
			i.AddText("decay:" + base.decay.ToString(), FontColor.DontChange);
		}
		if (this.id == "statue_weird")
		{
			i.AddText("weirdStatue", FontColor.DontChange);
		}
		if (base.isReplica)
		{
			i.AddText("isReplica", FontColor.Passive);
		}
		if (flag2)
		{
			Chara chara = base.GetRootCard() as Chara;
			Card parentCard = base.parentCard;
			if (((parentCard != null) ? parentCard.trait : null) is TraitChestMerchant)
			{
				chara = null;
			}
			if (base.c_equippedSlot != 0 && base.category.slot == 35 && chara != null)
			{
				Thing.AddAttackEvaluation(i, chara, this);
			}
			if (base.IsThrownWeapon || base.IsRangedWeapon || (base.IsMeleeWeapon && base.c_equippedSlot == 0))
			{
				i.AddHeader("headerAttackEval", null);
				AttackProcess.Current.Prepare(chara ?? EClass.pc, this, null, null, 0, base.IsThrownWeapon);
				string text7 = AttackProcess.Current.GetText();
				text7 = text7.TagColor(() => true, null);
				i.AddText(text7, FontColor.DontChange);
			}
		}
		if (base.ammoData != null)
		{
			i.AddHeader("headerAttackAmmo", null);
			i.AddText(base.ammoData.Name, FontColor.DontChange);
		}
		if (onWriteNote != null)
		{
			onWriteNote(i);
		}
		if (LayerDragGrid.Instance)
		{
			LayerDragGrid.Instance.owner.OnWriteNote(this, i);
		}
		if (EClass.debug.showExtra)
		{
			foreach (Element element2 in this.elements.dict.Values)
			{
				i.AddText(string.Concat(new string[]
				{
					element2.source.alias,
					"/",
					element2.Value.ToString(),
					"/",
					element2.vBase.ToString(),
					"/",
					element2.vSource.ToString()
				}), FontColor.DontChange);
			}
		}
		i.Build();
	}

	public static void AddAttackEvaluation(UINote n, Chara chara, Thing current = null)
	{
		n.AddHeader("headerAttackEval", null);
		int num = 0;
		foreach (BodySlot bodySlot in chara.body.slots)
		{
			if (bodySlot.thing != null && bodySlot.elementId == 35 && bodySlot.thing.source.offense.Length >= 2)
			{
				AttackProcess.Current.Prepare(chara, bodySlot.thing, null, null, num, false);
				string text = AttackProcess.Current.GetText();
				if (bodySlot.thing == current)
				{
					text = text.TagColor(() => true, null);
				}
				n.AddText(text, FontColor.DontChange);
				num++;
			}
		}
		AttackProcess.Current.Prepare(chara, null, null, null, 0, false);
		string text2 = AttackProcess.Current.GetText();
		if (num == 0)
		{
			text2 = text2.TagColor(() => true, null);
		}
		n.AddText(text2, FontColor.DontChange);
	}

	public override void SetRenderParam(RenderParam p)
	{
		p.matColor = (float)base.colorInt;
		p.mat = base.material;
		if (!this.renderer.usePass)
		{
			return;
		}
		switch (this.trait.tileMode)
		{
		case Trait.TileMode.Default:
			if (this.source._altTiles.Length != 0 && this.trait.UseAltTiles)
			{
				p.tile = (float)(this.source._altTiles[base.dir % this.source._altTiles.Length] * (this.flipX ? -1 : 1));
			}
			else
			{
				p.tile = (float)(this.sourceCard._tiles[base.dir % this.sourceCard._tiles.Length] * (this.flipX ? -1 : 1));
			}
			if (this.source.anime.Length != 0 && this.trait.IsAnimeOn)
			{
				if (this.source.anime.Length > 2)
				{
					float num = Time.realtimeSinceStartup * 1000f / (float)this.source.anime[1] % (float)this.source.anime[2];
					if ((int)num == this.source.anime[0] - 1 && this.source.anime.Length > 3)
					{
						base.PlaySound("anime_sound" + this.source.anime[3].ToString(), 1f, true);
					}
					if (num < (float)this.source.anime[0])
					{
						p.tile += num * (float)(this.flipX ? -1 : 1);
					}
				}
				else
				{
					float num2 = Time.realtimeSinceStartup * 1000f / (float)this.source.anime[1] % (float)this.source.anime[0];
					p.tile += num2 * (float)(this.flipX ? -1 : 1);
				}
			}
			break;
		case Trait.TileMode.Door:
		{
			if (this.source._altTiles.Length != 0 && this.trait.UseAltTiles)
			{
				p.tile = (float)(this.source._altTiles[base.dir % this.source._altTiles.Length] * (this.flipX ? -1 : 1));
			}
			else
			{
				p.tile = (float)(this.sourceCard._tiles[base.dir % this.sourceCard._tiles.Length] * (this.flipX ? -1 : 1));
			}
			if (this.parent is Zone && this.pos.cell.HasFullBlock)
			{
				p.tile += (float)((p.tile < 0f) ? -64 : 64);
			}
			TraitDoorSwing traitDoorSwing = this.trait as TraitDoorSwing;
			if (traitDoorSwing != null && traitDoorSwing.IsOpen())
			{
				p.tile += (float)((p.tile < 0f) ? -1 : 1);
			}
			break;
		}
		case Trait.TileMode.Illumination:
			if (base.isOn || base.isRoofItem)
			{
				int num3 = (int)((float)base.uid + Time.realtimeSinceStartup * 5f);
				int num4 = (int)(Time.realtimeSinceStartup * 5f);
				p.tile = (float)((this.sourceCard._tiles[base.dir % this.sourceCard._tiles.Length] + num3 % 3 + 1) * (this.flipX ? -1 : 1));
				if (num4 % 16 == 0)
				{
					p.color = 5242880f;
				}
				else if (num4 % 11 == 0)
				{
					p.color = 7864320f;
				}
				else
				{
					p.color = 13107200f;
				}
			}
			else
			{
				p.tile = (float)(this.sourceCard._tiles[base.dir % this.sourceCard._tiles.Length] * (this.flipX ? -1 : 1));
			}
			break;
		case Trait.TileMode.DefaultNoAnime:
			if (this.source._altTiles.Length != 0 && this.trait.UseAltTiles)
			{
				p.tile = (float)(this.source._altTiles[base.dir % this.source._altTiles.Length] * (this.flipX ? -1 : 1));
			}
			else
			{
				p.tile = (float)(this.sourceCard._tiles[base.dir % this.sourceCard._tiles.Length] * (this.flipX ? -1 : 1));
			}
			break;
		}
		if (base.idSkin != 0)
		{
			int num5 = base.idSkin - 1;
			if (this.sourceCard.skins.Length != 0)
			{
				p.tile += (float)((p.tile < 0f) ? (-(float)this.sourceCard.skins[num5]) : this.sourceCard.skins[num5]);
			}
		}
	}

	public override SubPassData GetSubPassData()
	{
		Chara chara = base.GetRootCard() as Chara;
		if ((this.trait.ShowAsTool && (chara != EClass.pc || !HotItemHeld.disableTool)) || (((chara != null) ? chara.held : null) != this && (this.placeState == PlaceState.installed || !this.renderer.data.subCrate.enable || !(this.parent is Zone))))
		{
			return SubPassData.Default;
		}
		if (!this.renderer.data.subCrate.enable)
		{
			return EClass.setting.pass.subCrate;
		}
		return this.renderer.data.subCrate;
	}

	public override bool CanStackTo(Thing to)
	{
		if (this.trait.HasCharges || to.isEquipped || base.isModified || to.isModified || to.id != this.id || to.idMaterial != base.idMaterial || to.refVal != base.refVal || to.blessedState != base.blessedState || to.rarityLv != base.rarityLv || to.qualityTier != base.qualityTier || to.idSkin != base.idSkin || to.isGifted != base.isGifted)
		{
			return false;
		}
		if ((to.isDyed || base.isDyed) && to.c_dyeMat != base.c_dyeMat)
		{
			return false;
		}
		if (base.c_idRefCard != to.c_idRefCard || base.c_idRefCard2 != to.c_idRefCard2)
		{
			return false;
		}
		if (base.IsDecayed != to.IsDecayed)
		{
			return false;
		}
		if (!this.trait.CanStackTo(to))
		{
			return false;
		}
		if (base.noSell != to.noSell || base.isCopy != to.isCopy)
		{
			return false;
		}
		if (base.isStolen != to.isStolen)
		{
			return false;
		}
		if (base.isCrafted != to.isCrafted)
		{
			return false;
		}
		if ((to.isWeightChanged || base.isWeightChanged) && to.SelfWeight != this.SelfWeight)
		{
			return false;
		}
		if (to.c_IDTState != base.c_IDTState)
		{
			return false;
		}
		if (to.c_priceAdd != base.c_priceAdd || to.c_priceFix != base.c_priceFix)
		{
			return false;
		}
		if (to.ChildrenAndSelfWeight + base.ChildrenAndSelfWeight > 1000000000)
		{
			return false;
		}
		if (this.trait.IsRequireFuel && base.c_charges != to.c_charges)
		{
			return false;
		}
		bool flag = false;
		if (to.parent is Card)
		{
			Window.SaveData windowSaveData = (to.parent as Card).GetWindowSaveData();
			if (windowSaveData != null && windowSaveData.compress)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (base.encLV != to.encLV && !base.IsFood)
			{
				return false;
			}
			if (this.elements.dict.Count<KeyValuePair<int, Element>>() != to.elements.dict.Count<KeyValuePair<int, Element>>())
			{
				return false;
			}
			foreach (Element element in this.elements.dict.Values)
			{
				if (to.elements.GetElement(element.id) == null)
				{
					return false;
				}
			}
			base.encLV = (to.encLV = Mathf.CeilToInt(1f * (float)(base.encLV * base.Num + to.encLV * to.Num) / (float)(base.Num + to.Num)));
			foreach (Element element2 in this.elements.dict.Values)
			{
				Element element3 = to.elements.GetElement(element2.id);
				element2.vBase = (element3.vBase = (element2.vBase * base.Num + element3.vBase * to.Num) / (base.Num + to.Num));
			}
			return true;
		}
		else
		{
			if (base.encLV != to.encLV)
			{
				return false;
			}
			if (this.elements.dict.Count<KeyValuePair<int, Element>>() != to.elements.dict.Count<KeyValuePair<int, Element>>())
			{
				return false;
			}
			foreach (Element element4 in this.elements.dict.Values)
			{
				Element element5 = to.elements.GetElement(element4.id);
				if (element5 == null || element4.vBase / 10 * 10 != element5.vBase / 10 * 10)
				{
					return false;
				}
			}
			return true;
		}
		bool result;
		return result;
	}

	public void GetIngredients(Recipe.Ingredient ing, List<Thing> list)
	{
		if (ing.CanSetThing(this))
		{
			list.Add(this);
		}
		if (this.things.Count > 0 && base.c_lockLv == 0 && !base.isNPCProperty)
		{
			foreach (Thing thing in this.things)
			{
				thing.GetIngredients(ing, list);
			}
		}
	}

	public bool IsValidIngredient(Recipe.Ingredient ing)
	{
		return !this.isDestroyed && base.GetRootCard().Dist(EClass.pc) <= 1 && ing.CanSetThing(this);
	}

	public void GetRecipes(HashSet<Recipe> recipes)
	{
	}

	public void GetDisassembles(List<Thing> list)
	{
	}

	public void Disassemble()
	{
	}

	public void ShowSplitMenu(ButtonGrid button, InvOwner.Transaction trans = null)
	{
		Thing.<>c__DisplayClass43_0 CS$<>8__locals1 = new Thing.<>c__DisplayClass43_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.trans = trans;
		CS$<>8__locals1.button = button;
		CS$<>8__locals1.count = 1;
		CS$<>8__locals1.m = EClass.ui.CreateContextMenuInteraction();
		CS$<>8__locals1.buy = (CS$<>8__locals1.trans != null);
		CS$<>8__locals1.buttonBuy = null;
		CS$<>8__locals1.itemSlider = null;
		CS$<>8__locals1.itemSlider = CS$<>8__locals1.m.AddSlider("sliderSplitMenu", "adjustmentNum", delegate(float a)
		{
			if (!EClass.core.IsGameStarted)
			{
				return "";
			}
			return "/" + CS$<>8__locals1.<>4__this.Num.ToString();
		}, (float)CS$<>8__locals1.count, delegate(float b)
		{
			CS$<>8__locals1.count = (int)b;
			if (CS$<>8__locals1.trans != null)
			{
				CS$<>8__locals1.trans.num = CS$<>8__locals1.count;
			}
			base.<ShowSplitMenu>g__UpdateButton|0();
		}, 1f, (float)base.Num, true, false, true).GetComponent<UIItem>();
		if (CS$<>8__locals1.buy)
		{
			CS$<>8__locals1.buttonBuy = CS$<>8__locals1.m.AddButton("invBuy", delegate()
			{
				base.<ShowSplitMenu>g__Process|1();
			}, true);
		}
		CS$<>8__locals1.m.onDestroy = delegate()
		{
			if (!CS$<>8__locals1.buy && !CS$<>8__locals1.m.wasCanceled)
			{
				base.<ShowSplitMenu>g__Process|1();
			}
		};
		CS$<>8__locals1.m.Show();
		if (CS$<>8__locals1.buttonBuy)
		{
			CS$<>8__locals1.buttonBuy.gameObject.AddComponent<CanvasGroup>();
		}
		CS$<>8__locals1.<ShowSplitMenu>g__UpdateButton|0();
	}

	public void ShowSplitMenu2(ButtonGrid button, string lang, Action<int> onSplit = null)
	{
		Thing.<>c__DisplayClass44_0 CS$<>8__locals1 = new Thing.<>c__DisplayClass44_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.lang = lang;
		CS$<>8__locals1.button = button;
		CS$<>8__locals1.onSplit = onSplit;
		CS$<>8__locals1.count = 1;
		UIContextMenu uicontextMenu = EClass.ui.CreateContextMenuInteraction();
		CS$<>8__locals1.buttonBuy = null;
		CS$<>8__locals1.itemSlider = null;
		CS$<>8__locals1.itemSlider = uicontextMenu.AddSlider("sliderSplitMenu", "adjustmentNum", delegate(float a)
		{
			if (!EClass.core.IsGameStarted)
			{
				return "";
			}
			return "/" + CS$<>8__locals1.<>4__this.Num.ToString();
		}, (float)CS$<>8__locals1.count, delegate(float b)
		{
			CS$<>8__locals1.count = (int)b;
			base.<ShowSplitMenu2>g__UpdateButton|0();
		}, 1f, (float)base.Num, true, false, true).GetComponent<UIItem>();
		CS$<>8__locals1.buttonBuy = uicontextMenu.AddButton("invBuy", delegate()
		{
			base.<ShowSplitMenu2>g__Process|1();
		}, true);
		uicontextMenu.onDestroy = delegate()
		{
		};
		uicontextMenu.Show();
		if (CS$<>8__locals1.buttonBuy)
		{
			CS$<>8__locals1.buttonBuy.gameObject.AddComponent<CanvasGroup>();
		}
		CS$<>8__locals1.<ShowSplitMenu2>g__UpdateButton|0();
	}

	public void DoAct(Act act)
	{
		if (!EClass.pc.HasNoGoal || (act.LocalAct && EClass._zone.IsRegion))
		{
			SE.Beep();
			return;
		}
		EClass.player.hotItemToRestore = EClass.player.currentHotItem;
		if (act.IsAct)
		{
			act.Perform(EClass.pc, null, null);
			return;
		}
		AIAct ai = act as AIAct;
		EClass.pc.SetAI(ai);
		ActionMode.Adv.SetTurbo(-1);
	}

	public bool CanSearchContents
	{
		get
		{
			return base.IsContainer && base.c_lockLv == 0 && !base.isNPCProperty && this.trait.CanSearchContents;
		}
	}

	public bool IsSharedContainer
	{
		get
		{
			if (base.IsContainer && base.c_lockLv == 0 && !base.isNPCProperty)
			{
				Window.SaveData obj = base.GetObj<Window.SaveData>(2);
				return obj != null && obj.sharedType == ContainerSharedType.Shared;
			}
			return false;
		}
	}

	public static Tuple<SourceElement.Row, int> GetEnchant(int lv, Func<SourceElement.Row, bool> func, bool neg)
	{
		List<SourceElement.Row> list = new List<SourceElement.Row>();
		int num = 0;
		int num2 = lv + 5 + EClass.rndSqrt(10);
		float num3 = (float)(3 + Mathf.Min(lv / 10, 15)) + Mathf.Sqrt((float)lv);
		foreach (SourceElement.Row row in EClass.sources.elements.rows)
		{
			if ((!neg || !row.tag.Contains("flag")) && func(row) && row.LV < num2)
			{
				list.Add(row);
				num += row.chance;
			}
		}
		if (num == 0)
		{
			return null;
		}
		int num4 = EClass.rnd(num);
		int num5 = 0;
		foreach (SourceElement.Row row2 in list)
		{
			num5 += row2.chance;
			if (num4 < num5)
			{
				string category = EClass.sources.elements.map[row2.id].category;
				bool flag = category == "skill" || category == "attribute" || category == "resist";
				int item = (row2.mtp + EClass.rnd(row2.mtp + (int)num3)) / row2.mtp * ((flag && neg) ? -1 : 1);
				return new Tuple<SourceElement.Row, int>(row2, item);
			}
		}
		return null;
	}

	public void TryLickEnchant(Chara c, bool msg = true, Chara tg = null, BodySlot slot = null)
	{
		if (!base.IsEquipmentOrRanged)
		{
			return;
		}
		if (base.IsCursed || base.rarity <= Rarity.Normal)
		{
			return;
		}
		if (base.GetInt(107, null) > 0)
		{
			return;
		}
		if (tg == null)
		{
			Rand.SetSeed(EClass.world.date.day + this.source._index + c.uid);
			if (msg)
			{
				c.Say("lick", c, this, null, null);
				base.PlaySound("offering", 1f, true);
				base.PlayEffect("mutation", true, 0f, default(Vector3));
			}
			this.AddEnchant(base.LV);
		}
		else
		{
			Rand.SetSeed(base.uid);
			List<Element> list = new List<Element>();
			foreach (Element element in this.elements.dict.Values)
			{
				if (element.id != 67 && element.id != 66 && element.id != 64 && element.id != 65)
				{
					list.Add(element);
				}
			}
			if (list.Count > 0)
			{
				Element element2 = list.RandomItem<Element>();
				this.elements.ModBase(element2.id, Mathf.Max(EClass.rnd(Mathf.Abs(element2.vBase / 5)), 1));
			}
			if (msg)
			{
				c.Say("lick2", c, tg, slot.name.ToLower(), null);
				tg.PlaySound("offering", 1f, true);
				tg.PlayEffect("mutation", true, 0f, default(Vector3));
			}
		}
		Rand.SetSeed(-1);
		base.SetInt(107, 1);
	}

	public Element AddEnchant(int lv = -1)
	{
		if (base.IsToolbelt || base.IsLightsource)
		{
			return null;
		}
		Tuple<SourceElement.Row, int> enchant = Thing.GetEnchant(lv, (SourceElement.Row r) => r.IsEncAppliable(base.category), base.IsCursed);
		if (enchant == null)
		{
			return null;
		}
		return this.elements.ModBase(enchant.Item1.id, enchant.Item2);
	}

	public void RemoveEnchant()
	{
	}

	public Thing Identify(bool show = true, IDTSource idtSource = IDTSource.Identify)
	{
		if (base.IsIdentified)
		{
			return this;
		}
		string @ref = "";
		if (show)
		{
			@ref = this.GetName(NameStyle.Full, base.Num);
		}
		Rarity rarity = (idtSource == IDTSource.Skill) ? Rarity.Superior : ((idtSource == IDTSource.SkillHigh) ? Rarity.Legendary : Rarity.Normal);
		if (rarity != Rarity.Normal && ((base.IsEquipmentOrRanged && base.rarity >= rarity) || base.rarity >= Rarity.Mythical))
		{
			base.c_IDTState = 3;
		}
		else if (base.rarity >= Rarity.Mythical && idtSource != IDTSource.SuperiorIdentify)
		{
			base.c_IDTState = 1;
		}
		else
		{
			base.c_IDTState = 0;
		}
		if (show)
		{
			string name = this.GetName(NameStyle.Full, base.Num);
			if (base.c_IDTState == 0)
			{
				Msg.Say("identified", @ref, name, null, null);
			}
			else
			{
				Msg.Say((idtSource == IDTSource.Skill) ? "identified3" : "identified2", @ref, name, base.TextRarity, null);
			}
		}
		if (base.IsIdentified)
		{
			Card rootCard = base.GetRootCard();
			if (rootCard != null)
			{
				rootCard.TryStack(this);
			}
		}
		LayerInventory.SetDirty(this);
		return this;
	}

	[CompilerGenerated]
	private void <ApplyMaterial>g__SetBase|27_0(int ele, int a, ref Thing.<>c__DisplayClass27_0 A_3)
	{
		this.elements.SetBase(ele, a, 0);
		if (ele == 67)
		{
			A_3.dmgSet = true;
		}
		if (ele == 65)
		{
			A_3.pvSet = true;
		}
		if (ele == 66)
		{
			A_3.hitSet = true;
		}
	}

	public const int MaxFurnitureEnc = 12;

	public SourceThing.Row source;

	public int stackOrder;

	public string tempName;
}
