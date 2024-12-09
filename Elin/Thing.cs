using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Thing : Card
{
	public const int MaxFurnitureEnc = 12;

	public SourceThing.Row source;

	public int stackOrder;

	public string tempName;

	public bool isEquipped => base.c_equippedSlot != 0;

	public bool IsMeleeWithAmmo
	{
		get
		{
			if (trait is TraitToolRange)
			{
				return isEquipped;
			}
			return false;
		}
	}

	public int range => source.range;

	public int Penetration
	{
		get
		{
			if (source.substats.Length == 0)
			{
				return 0;
			}
			return source.substats[0];
		}
	}

	public override bool isThing => true;

	public override CardRow sourceCard => source;

	public override SourcePref Pref
	{
		get
		{
			if (source.origin == null || source.pref.UsePref)
			{
				return source.pref;
			}
			return source.origin.pref;
		}
	}

	public override int SelfWeight
	{
		get
		{
			if (!base.IsUnique)
			{
				return (base.isWeightChanged ? base.c_weight : source.weight) * base.material.weight / 100;
			}
			if (!base.isWeightChanged)
			{
				return source.weight;
			}
			return base.c_weight;
		}
	}

	public override int[] Tiles => sourceCard._tiles;

	public bool CanSearchContents
	{
		get
		{
			if (base.IsContainer && base.c_lockLv == 0 && !base.isNPCProperty)
			{
				return trait.CanSearchContents;
			}
			return false;
		}
	}

	public bool IsSharedContainer
	{
		get
		{
			if (base.IsContainer && base.c_lockLv == 0 && !base.isNPCProperty)
			{
				Window.SaveData obj = GetObj<Window.SaveData>(2);
				if (obj == null)
				{
					return false;
				}
				return obj.sharedType == ContainerSharedType.Shared;
			}
			return false;
		}
	}

	public bool CanAutoFire(Chara c, Card tg, bool reloading = false)
	{
		if (GetRootCard() != c)
		{
			return false;
		}
		if (HasTag(CTAG.throwWeapon))
		{
			return true;
		}
		if (!trait.CanAutofire)
		{
			return false;
		}
		if (trait is TraitToolRange)
		{
			if ((c.IsPCFaction && c.body.IsTooHeavyToEquip(this)) || reloading)
			{
				return false;
			}
		}
		else if (trait is TraitAbility && c.IsPC)
		{
			Act act = (trait as TraitAbility).act;
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
		return 50 + base.LV * 10 + base.encLV * 10 + (int)base.rarity * 10 + (int)base.blessedState * 10;
	}

	public override void SetSource()
	{
		source = EClass.sources.things.map.TryGetValue(id);
		if (source != null && source.isOrigin)
		{
			source = EClass.sources.cards.firstVariations[id] as SourceThing.Row;
			id = source.id;
		}
		if (source == null)
		{
			Debug.LogWarning("Thing " + id + " not found");
			id = "1101";
			source = EClass.sources.things.map[id];
		}
	}

	public override void OnCreate(int genLv)
	{
		if (bp.blesstedState.HasValue)
		{
			SetBlessedState(bp.blesstedState.GetValueOrDefault());
		}
		else if (base.category.ignoreBless == 0 && bp.rarity == Rarity.Random && base.rarity != Rarity.Artifact)
		{
			if (EClass.rnd(25) == 0)
			{
				SetBlessedState(BlessedState.Blessed);
			}
			else if (EClass.rnd(25) == 0)
			{
				SetBlessedState(BlessedState.Cursed);
			}
			else if (EClass.rnd(50) == 0 && base.category.slot != 0)
			{
				SetBlessedState(BlessedState.Doomed);
			}
		}
		if (!EClass.debug.autoIdentify && (!source.unknown_JP.IsEmpty() || (base.category.slot != 0 && base.rarity >= Rarity.Superior)))
		{
			base.c_IDTState = 5;
		}
		string text = id;
		if (text == "bill_tax" || text == "bill")
		{
			base.c_bill = 100 + EClass.rnd(100);
		}
		if (base.category.slot != 0)
		{
			int numGeneratedEnchants = 0;
			if (base.rarity == Rarity.Superior)
			{
				numGeneratedEnchants = EClass.rnd(3);
			}
			else if (base.rarity == Rarity.Legendary)
			{
				numGeneratedEnchants = EClass.rnd(4) + 2;
			}
			else if (base.rarity == Rarity.Mythical)
			{
				numGeneratedEnchants = EClass.rnd(3) + 5;
			}
			else if (base.rarity >= Rarity.Artifact)
			{
				numGeneratedEnchants = EClass.rnd(2) + 1;
			}
			if (numGeneratedEnchants > 0 && !HasTag(CTAG.godArtifact))
			{
				for (int i = 0; i < numGeneratedEnchants; i++)
				{
					AddEnchant(genLv);
				}
			}
		}
		if (base.IsRangedWeapon && !IsMeleeWithAmmo)
		{
			if (HasTag(CTAG.godArtifact))
			{
				AddSocket();
				AddSocket();
			}
			else
			{
				int num2 = 1;
				int num3 = ((EClass.rnd(10) == 0) ? 1 : 0);
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
						AddSocket();
					}
					for (int k = 0; k < EClass.rnd(num2 + 1); k++)
					{
						Tuple<SourceElement.Row, int> enchant = GetEnchant(genLv, (SourceElement.Row r) => r.tag.Contains("modRanged"), neg: false);
						if (enchant != null && InvOwnerMod.IsValidMod(this, enchant.Item1))
						{
							ApplySocket(enchant.Item1.id, enchant.Item2);
						}
					}
				}
			}
		}
		if ((bp.rarity != 0 || bp.qualityBonus != 0) && base.rarity < Rarity.Artifact && base.category.tag.Contains("enc"))
		{
			int num4 = 0;
			if (EClass.rnd(6) == 0)
			{
				if (bp.qualityBonus == 0)
				{
					num4 = EClass.rnd(EClass.rnd(11) + 1);
					if (num4 == 1 && EClass.rnd(3) != 0)
					{
						num4 = 0;
					}
				}
				else if (bp.qualityBonus < 0)
				{
					if (EClass.rnd(3) == 0)
					{
						num4 = 1;
					}
				}
				else if (bp.qualityBonus >= 10)
				{
					num4 = Mathf.Min(bp.qualityBonus / 10 + 2, 7) + EClass.rnd(EClass.rnd(5) + 1);
				}
			}
			if (num4 > 0)
			{
				SetEncLv(Mathf.Min(num4, 12));
			}
		}
		if (HasTag(CTAG.randomSkin))
		{
			base.idSkin = EClass.rnd(source.skins.Length + 1);
		}
	}

	public override void ApplyMaterialElements(bool remove)
	{
		Chara chara = null;
		if (EClass.core.IsGameStarted && isEquipped)
		{
			chara = GetRootCard()?.Chara;
			if (chara != null)
			{
				elements.SetParent();
			}
		}
		elements.ApplyMaterialElementMap(this, remove);
		if (chara != null)
		{
			elements.SetParent(chara);
		}
	}

	public override void ApplyMaterial(bool remove = false)
	{
		if (source.HasTag(CTAG.replica))
		{
			base.isReplica = true;
		}
		if (remove)
		{
			ApplyMaterialElements(remove: true);
			bool constantFalse = (base.isFireproof = false);
			base.isAcidproof = constantFalse;
			return;
		}
		bool pvSet = false;
		bool dmgSet = false;
		bool hitSet = false;
		if (sourceCard.quality == 4)
		{
			if (source.offense.Length != 0)
			{
				base.c_diceDim = source.offense[1];
			}
			if (source.offense.Length > 2)
			{
				SetBase(66 /* HIT */, source.offense[2]);
			}
			if (source.offense.Length > 3)
			{
				SetBase(67 /* DMG */, source.offense[3]);
			}
			if (source.defense.Length != 0)
			{
				SetBase(64 /* DV */, source.defense[0]);
			}
			if (source.defense.Length > 1)
			{
				SetBase(65 /* PV */, source.defense[1]);
			}
		}
		else
		{
			int num = 120;
			bool isBaseNotAmmo = !base.IsAmmo;
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
			if (source.offense.Length != 0)
			{
				base.c_diceDim = source.offense[1] * base.material.dice / (num + (isBaseNotAmmo ? EClass.rnd(25) : 0));
			}
			if (source.offense.Length > 2)
			{
				SetBase(66 /* HIT */, source.offense[2] * base.material.atk * 9 / (num - (isBaseNotAmmo ? EClass.rnd(30) : 0)));
			}
			if (source.offense.Length > 3)
			{
				SetBase(67 /* DMG */, source.offense[3] * base.material.dmg * 5 / (num - (isBaseNotAmmo ? EClass.rnd(30) : 0)));
			}
			if (source.defense.Length != 0)
			{
				SetBase(64 /* DV */, source.defense[0] * base.material.dv * 7 / (num - (isBaseNotAmmo ? EClass.rnd(30) : 0)));
			}
			if (source.defense.Length > 1)
			{
				SetBase(65 /* PV */, source.defense[1] * base.material.pv * 9 / (num - (isBaseNotAmmo ? EClass.rnd(30) : 0)));
			}
		}
		if (base.isReplica)
		{
			if (source.offense.Length != 0)
			{
				base.c_diceDim = Mathf.Max(source.offense[1] / 3, 1);
			}
			if (source.offense.Length > 2)
			{
				SetBase(66, source.offense[2] / 3);
			}
			if (source.offense.Length > 3)
			{
				SetBase(67, source.offense[3] / 3);
			}
			if (source.defense.Length != 0)
			{
				SetBase(64, source.defense[0] / 3);
			}
			if (source.defense.Length > 1)
			{
				SetBase(65, source.defense[1] / 3);
			}
		}
		if (base.IsEquipmentOrRanged || base.IsAmmo)
		{
			if (base.IsWeapon || base.IsAmmo)
			{
				if (dmgSet)
				{
					elements.ModBase(67, base.encLV + ((base.blessedState == BlessedState.Blessed) ? 1 : 0));
				}
			}
			else if (pvSet)
			{
				elements.ModBase(65, (base.encLV + ((base.blessedState == BlessedState.Blessed) ? 1 : 0)) * 2);
			}
		}
		if (sockets != null)
		{
			for (int i = 0; i < sockets.Count; i++)
			{
				int num2 = sockets[i];
				int num3 = num2 / 100;
				if (num3 == 67 && dmgSet)
				{
					elements.ModBase(67, num2 % 100);
				}
				if (num3 == 66 && hitSet)
				{
					elements.ModBase(66, num2 % 100);
				}
				if (num3 == 65 && pvSet)
				{
					elements.ModBase(65, num2 % 100);
				}
			}
		}
		if (base.material == null || base.material.elements == null)
		{
			Debug.Log(base.idMaterial + "/" + base.material?.name + "/" + base.material?.elements);
		}
		ApplyMaterialElements(remove: false);
		string[] bits = base.material.bits;
		foreach (string text in bits)
		{
			if (!(text == "fire"))
			{
				if (text == "acid")
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
			bool constantTrue = (base.isFireproof = true);
			base.isAcidproof = constantTrue;
		}
		_colorInt = 0;
		void SetBase(int ele, int a)
		{
			elements.SetBase(ele, a);
			if (ele == 67)
			{
				dmgSet = true;
			}
			if (ele == 65)
			{
				pvSet = true;
			}
			if (ele == 66)
			{
				hitSet = true;
			}
		}
	}

	public override string GetName(NameStyle style, int _num = -1)
	{
		int num = ((_num == -1) ? base.Num : _num);
		string text = "";
		string text2 = "";
		string text3 = "";
		string text4 = "";
		string sig = "";
		string text5 = "";
		string text6 = source.GetText("unit");
		ArticleStyle style2 = ((style == NameStyle.FullNoArticle) ? ArticleStyle.None : ArticleStyle.Default);
		bool hasNormalNameGeneration = base.IsIdentified || source.unknown.IsEmpty();
		bool isEquipmentOrRanged = base.IsEquipmentOrRanged;
		bool flag = Lang.setting.nameStyle == 0;
		if (hasNormalNameGeneration)
		{
			if (base.c_idRefCard.IsEmpty() && !base.c_altName.IsEmpty())
			{
				text = base.c_altName;
			}
			else
			{
				string[] array = trait.GetName().Split(',');
				text = array[0];
				if (array.Length > 1)
				{
					text6 = array[1];
				}
			}
			if (text.IsEmpty())
			{
				text = id;
			}
			if (!isEquipmentOrRanged || !base.IsIdentified || base.rarity < Rarity.Legendary)
			{
				if (source.naming == "m" || (source.naming == "ms" && base.material != source.DefaultMaterial))
				{
					if (isEquipmentOrRanged)
					{
						string[] textArray = base.material.GetTextArray("altName");
						if (textArray != null && textArray.Length >= 2)
						{
							text = base.material.GetTextArray("altName")[1] + Lang.space + text;
							goto IL_01f2;
						}
					}
					text = "_of2".lang(base.material.GetName(), text);
				}
				goto IL_01f2;
			}
			if (base.rarity != Rarity.Artifact && !base.material.GetTextArray("altName").IsEmpty())
			{
				text = base.material.GetTextArray("altName")[0] + Lang.space + text;
			}
		}
		else
		{
			text = "unknown";
			string idUnknown = source.GetText("unknown");
			if (idUnknown.StartsWith("#"))
			{
				Rand.UseSeed(EClass.game.seed + (trait.CanStack ? sourceCard._index : base.uid) + base.refVal, delegate
				{
					idUnknown = Lang.GetList(idUnknown.Remove(0, 1)).RandomItem();
				});
			}
			text = idUnknown;
		}
		goto IL_02c9;
		IL_02c9:
		if (!base.c_idRefCard.IsEmpty() && trait.RefCardName != RefCardName.None)
		{
			string text7 = base.c_altName.IsEmpty(base.refCard.GetName());
			if (!base.c_idRefCard2.IsEmpty())
			{
				text7 = "_and".lang(text7, base.c_altName2.IsEmpty(base.refCard2.GetName()));
			}
			if (!(text7 == "*r"))
			{
				text = ((!source.name2.IsEmpty()) ? source.GetTextArray("name2")[0].Replace("#1", text7) : (source.naming.Contains("last") ? (text + Lang.space + text7) : (source.naming.Contains("first") ? (text7 + Lang.space + text) : ((!source.naming.Contains("of")) ? (text6.IsEmpty() ? "_of3" : "_of2").lang(text7, text) : "_of".lang(text7, text)))));
			}
			else
			{
				string text8 = base.refCard.GetText("aka");
				if (!text8.IsEmpty())
				{
					text = "_of".lang(text8, text);
				}
			}
		}
		if (base.c_bill != 0)
		{
			text = "_of".lang(Lang._currency(base.c_bill, showUnit: true, 0), text);
		}
		trait.SetName(ref text);
		switch (style)
		{
		case NameStyle.Simple:
			return text;
		case NameStyle.Ref:
			return text;
		default:
		{
			if (!base.c_refText.IsEmpty())
			{
				text = "_named".lang(base.c_refText, text);
			}
			if (base.IsIdentified)
			{
				int hIT = base.HIT;
				int dMG = base.DMG;
				if ((base.IsMeleeWeapon || base.IsRangedWeapon || base.IsAmmo || hIT != 0 || dMG != 0) && source.offense.Length != 0)
				{
					string text9 = "";
					if (source.offense[0] != 0)
					{
						text9 = text9 + source.offense[0] + "d" + base.c_diceDim;
					}
					if (dMG != 0)
					{
						text9 += ((base.IsMeleeWeapon || base.IsRangedWeapon || base.IsAmmo) ? dMG.ToText() : (dMG.ToString() ?? ""));
					}
					if (hIT != 0)
					{
						text9 = text9 + ((dMG != 0 || source.offense[0] != 0) ? ", " : "") + hIT;
					}
					text2 = text2 + " (" + text9.IsEmpty(" - ") + ") ";
				}
				int dV = DV;
				int pV = PV;
				if (dV != 0 || pV != 0)
				{
					text2 += " [";
					text2 = text2 + dV + ", " + pV;
					text2 += "] ";
				}
				if (trait.HasCharges && trait.ShowCharges)
				{
					text2 = text2 + " " + "itemCharges".lang(base.c_charges.ToString() ?? "");
				}
			}
			else if (base.c_IDTState == 3 || base.c_IDTState == 1)
			{
				text2 = "(" + base.TextRarity.ToTitleCase() + ")";
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
				if (base.blessedState != 0)
				{
					text4 = ("bs" + base.blessedState).lang();
				}
				switch (base.rarity)
				{
				case Rarity.Artifact:
					style2 = ArticleStyle.None;
					text3 = "★";
					text = (isEquipmentOrRanged ? text.Bracket(3) : text);
					break;
				case Rarity.Legendary:
				case Rarity.Mythical:
					style2 = ArticleStyle.The;
					text3 = "☆";
					if (isEquipmentOrRanged)
					{
						Rand.UseSeed(base.uid + EClass.game.seed, delegate
						{
							sig = AliasGen.GetRandomAlias().Bracket((base.rarity == Rarity.Mythical) ? 3 : 2);
						});
						sig = Lang.space + sig;
					}
					break;
				}
			}
			if (base.encLV != 0)
			{
				if (base.category.tag.Contains("enc"))
				{
					if (base.c_altName.IsEmpty())
					{
						string[] list = Lang.GetList("quality_furniture");
						text = "_qualityFurniture".lang(list[Mathf.Clamp(base.encLV - 1, 0, list.Length - 1)], text);
					}
				}
				else
				{
					sig = sig + Lang.space + ((base.encLV > 0) ? ("+" + base.encLV) : (base.encLV.ToString() ?? ""));
				}
			}
			if (base.c_lockLv != 0 && base.c_revealLock)
			{
				sig = sig + Lang.space + "+" + base.c_lockLv;
			}
			if (base.isLostProperty)
			{
				text = "_lostproperty".lang(text);
			}
			if (trait is TraitEquipItem && EClass.player.eqBait == this && EClass.player.eqBait.GetRootCard() == EClass.pc)
			{
				text5 += "equippedItem".lang();
			}
			if (!base.c_note.IsEmpty() && (!base.isBackerContent || EClass.core.config.backer.Show(base.c_note)))
			{
				string text10 = base.c_note;
				if (text10.StartsWith('@'))
				{
					text10 = Lang.Note.map.TryGetValue(text10.TrimStart('@'))?.GetText("text") ?? base.c_note;
				}
				string text11 = (base.category.IsChildOf("book") ? "_written" : "_engraved");
				if (id == "grave_dagger1" || id == "grave_dagger2")
				{
					text11 = "_daggerGrave";
				}
				text = ((!text10.Contains("_bracketLeft".lang())) ? text11.lang(text10, text) : (text11 + "Alt").lang(text10, text));
			}
			text = (flag ? ((num <= 1) ? (text4 + text) : "_unit".lang(num.ToFormat() ?? "", text4 + text, text6)) : ((trait is TraitAbility) ? text.ToTitleCase(wholeText: true) : ((!text6.IsEmpty() && (base.IsIdentified || source.unknown.IsEmpty())) ? "_unit".lang((num == 1) ? "" : (num.ToFormat() ?? ""), text, (text4 + text6).AddArticle(num, style2, source.unit)) : (text4 + text).AddArticle(num, style2, source.name))));
			if (base.rarity >= Rarity.Legendary)
			{
				text = text.ToTitleCase(wholeText: true);
			}
			string text12 = ((base.isSale && things.Count > 0) ? "forSale2".lang() : ((base.isSale || (base.parentThing != null && base.parentThing.isSale && TraitSalesTag.CanTagSale(this, insideContainer: true))) ? "forSale".lang(Lang._currency(GetPrice(CurrencyType.Money, sell: true, PriceType.PlayerShop), "money")) : ""));
			if (trait is TraitSalesTag && base.isOn && !GetStr(11).IsEmpty())
			{
				text12 += "soldOut".lang(EClass.sources.categories.map[GetStr(11)].GetName());
			}
			if (GetInt(101) != 0)
			{
				text5 = "_limitedStock".lang(text5);
			}
			return text5 + text3 + text + sig + text2 + text12;
		}
		}
		IL_01f2:
		if (source.naming == "ma")
		{
			text = base.material.GetName();
		}
		if (base.qualityTier > 0)
		{
			text = Lang.GetList("quality_general")[Mathf.Clamp(base.qualityTier, 0, 3)] + text;
		}
		goto IL_02c9;
	}

	public override string GetHoverText()
	{
		string text = "";
		text = text + " <size=14>(" + Lang._weight(base.ChildrenAndSelfWeight) + ")</size> ";
		if (EClass.debug.showExtra)
		{
			text += Environment.NewLine;
			text = text + "id:" + id + "  tile:" + source.idRenderData + "/" + ((source.tiles.Length != 0) ? ((object)source.tiles[0]) : "-")?.ToString() + " num:" + base.Num + " lv:" + base.LV + " enc:" + base.encLV + " / " + base.material.alias;
		}
		string hoverText = trait.GetHoverText();
		if (!hoverText.IsEmpty())
		{
			text = text + Environment.NewLine + hoverText;
		}
		return base.GetHoverText() + text;
	}

	public override string GetExtraName()
	{
		string text = "";
		if (trait.ShowChildrenNumber && base.c_lockLv == 0)
		{
			if (things.Count > 0)
			{
				text += "childCount".lang(things.Count.ToString() ?? "");
			}
			else if (trait.CanOpenContainer)
			{
				text += "empty".lang();
			}
		}
		if ((trait is TraitRoomPlate || trait is TraitHouseBoard) && pos.IsValid)
		{
			Room room = pos.cell.room;
			if (EClass.debug.enable && room != null && room.data.group != 0)
			{
				text = text + " #" + room.data.group;
			}
		}
		return text;
	}

	public List<Element> ListLimitedValidTraits(bool limit)
	{
		List<Element> list = new List<Element>();
		if (base.ShowFoodEnc)
		{
			foreach (Element value in elements.dict.Values)
			{
				if (value.IsFoodTraitMain && value.Value > 0)
				{
					list.Add(value);
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
		List<Element> list = ListLimitedValidTraits(limit);
		bool showFoodEnc = base.ShowFoodEnc;
		bool playerHasFeatGourmet = EClass.pc.HasElement(1650 /* featGourmet */);
		if (showFoodEnc)
		{
			foreach (Element value in elements.dict.Values)
			{
				if (value.IsFoodTrait && !list.Contains(value) && (isCraft || playerHasFeatGourmet || value.IsFoodTraitMain) && (!value.IsFoodTraitMain || value.Value < 0))
				{
					list.Add(value);
				}
			}
		}
		foreach (Element value2 in elements.dict.Values)
		{
			if ((isCraft || playerHasFeatGourmet || ((!value2.IsFoodTrait || value2.IsFoodTraitMain) && (!showFoodEnc || !value2.IsTrait || value2.Value >= 0))) && !list.Contains(value2) && (value2.IsTrait || (value2.IsFoodTrait && !value2.IsFoodTraitMain)))
			{
				list.Add(value2);
			}
		}
		return list;
	}

	public override void WriteNote(UINote n, Action<UINote> onWriteNote = null, IInspect.NoteMode mode = IInspect.NoteMode.Default, Recipe recipe = null)
	{
		n.Clear();
		string text = "";
		TraitAbility traitAbility = trait as TraitAbility;
		bool showEQStats = base.IsEquipmentOrRanged || base.IsAmmo;
		bool flag = mode == IInspect.NoteMode.Product;
		bool flag2 = base.IsIdentified || flag;
		text = base.Name;
		if (base.rarity == Rarity.Legendary || base.rarity == Rarity.Mythical)
		{
			string text2 = (text.Contains("『") ? "『" : (text.Contains("《") ? "《" : ""));
			if (text2 != "")
			{
				string[] array = text.Split(text2);
				text = array[0] + Environment.NewLine + text2 + array[1];
			}
		}
		if (flag)
		{
			text = recipe.GetName();
		}
		if (mode != IInspect.NoteMode.Recipe)
		{
			UIItem uIItem = n.AddHeaderCard(text);
			SetImage(uIItem.image2);
			uIItem.image2.Rect().pivot = new Vector2(0.5f, 0.5f);
			string text3 = base.Num.ToFormat() ?? "";
			string text4 = (Mathf.Ceil(0.01f * (float)base.ChildrenAndSelfWeight) * 0.1f).ToString("F1") + "s";
			if (things.Count > 0)
			{
				text3 = text3 + " (" + things.Count + ")";
			}
			if (base.ChildrenAndSelfWeight != SelfWeight)
			{
				text4 = text4 + " (" + (Mathf.Ceil(0.01f * (float)SelfWeight) * 0.1f).ToString("F1") + "s)";
			}
			text = "_quantity".lang(text3 ?? "", text4);
			if (flag && recipe != null && (bool)LayerCraft.Instance)
			{
				text = text + "  " + "_recipe_lv".lang(recipe.RecipeLv.ToString() ?? "");
			}
			uIItem.text2.SetText(text);
			if (showEQStats && flag2)
			{
				if (!flag)
				{
					text = "";
					if (DV != 0 || PV != 0 || base.HIT != 0 || base.DMG != 0 || Penetration != 0)
					{
						if (base.DMG != 0)
						{
							text = text + "DMG".lang() + ((base.DMG > 0) ? "+" : "") + base.DMG + ", ";
						}
						if (base.HIT != 0)
						{
							text = text + "HIT".lang() + ((base.HIT > 0) ? "+" : "") + base.HIT + ", ";
						}
						if (DV != 0)
						{
							text = text + "DV".lang() + ((DV > 0) ? "+" : "") + DV + ", ";
						}
						if (PV != 0)
						{
							text = text + "PV".lang() + ((PV > 0) ? "+" : "") + PV + ", ";
						}
						if (Penetration != 0)
						{
							text = text + "PEN".lang() + ((Penetration > 0) ? "+" : "") + Penetration + "%, ";
						}
						text = text.TrimEnd(' ').TrimEnd(',');
					}
					if (!text.IsEmpty())
					{
						n.AddText("NoteText_eqstats", text);
					}
				}
				if (trait is TraitToolRange traitToolRange)
				{
					n.AddText("NoteText_eqstats", "tip_range".lang(traitToolRange.BestDist.ToString() ?? ""));
				}
			}
			else
			{
				string text5 = "";
				if (EClass.debug.showExtra)
				{
					int totalQuality = GetTotalQuality();
					int totalQuality2 = GetTotalQuality(applyBonus: false);
					text5 = text5 + "Lv. " + base.LV + " TQ. " + GetTotalQuality() + ((totalQuality == totalQuality2) ? "" : (" (" + totalQuality2 + ")"));
				}
				if (HasElement(10))
				{
					text5 = text5 + (text5.IsEmpty() ? "" : "  ") + "_nutrition".lang(Evalue(10).ToFormat() ?? "");
				}
				if ((base.category.IsChildOf("resource") || trait.IsTool) && !(trait is TraitAbility))
				{
					text5 = text5 + (text5.IsEmpty() ? "" : "  ") + "_hardness".lang(base.material.hardness.ToString() ?? "");
				}
				if (flag && recipe != null && (bool)LayerCraft.Instance)
				{
					text5 = text5 + (text5.IsEmpty() ? "" : "  ") + "_max_quality".lang(recipe.GetQualityBonus().ToString() ?? "");
				}
				if (!text5.IsEmpty())
				{
					n.AddText("NoteText_eqstats", text5);
				}
			}
			string detail = GetDetail();
			if (!detail.IsEmpty())
			{
				LayoutElement component = n.AddText("NoteText_flavor", detail).GetComponent<LayoutElement>();
				if (flag)
				{
					component.preferredWidth = 400f;
				}
				n.Space(8);
			}
		}
		if (trait is TraitBookPlan)
		{
			TraitBookPlan traitBookPlan = trait as TraitBookPlan;
			n.AddText("NoteText_flavor", traitBookPlan.source.GetDetail());
			n.Space(8);
		}
		if (traitAbility != null)
		{
			n.Space(8);
			Act act = traitAbility.CreateAct();
			Element orCreateElement = EClass.pc.elements.GetOrCreateElement(act.source.id);
			orCreateElement._WriteNote(n, EClass.pc.elements, null, isRef: false, addHeader: false);
			orCreateElement._WriteNote(n, EClass.pc, act);
			return;
		}
		if (EClass.debug.showExtra)
		{
			n.AddText("(id:" + id + " tile:" + (source.tiles.IsEmpty() ? "-" : ((object)source.tiles[0]))?.ToString() + ") lv:" + base.LV + " price:" + GetPrice());
		}
		Card rootCard = GetRootCard();
		if (rootCard != null && rootCard != EClass.pc && rootCard != this && rootCard.ExistsOnMap && !((parent as Thing)?.trait is TraitChestMerchant))
		{
			n.AddText("isChildOf".lang(GetRootCard().Name), FontColor.ItemName);
		}
		if (flag2)
		{
			n.AddText("isMadeOf".lang(base.material.GetText(), base.material.hardness.ToString() ?? ""));
		}
		n.AddText("isCategorized".lang(base.category.GetText()));
		if (base.category.skill != 0)
		{
			int key = base.category.skill;
			int key2 = 132;
			if (base.IsRangedWeapon)
			{
				key2 = 133;
			}
			if (trait is TraitToolRangeCane)
			{
				key2 = 304;
			}
			if (Evalue(482) > 0)
			{
				key = 305;
				key2 = 304;
			}
			n.AddText("isUseSkill".lang(EClass.sources.elements.map[key].GetName().ToTitleCase(wholeText: true), EClass.sources.elements.map[key2].GetName().ToTitleCase(wholeText: true)));
		}
		if (base.IsContainer)
		{
			n.AddText("isContainer".lang(things.MaxCapacity.ToString() ?? ""));
		}
		if (base.c_lockLv != 0)
		{
			n.AddText((base.c_lockedHard ? "isLockedHard" : "isLocked").lang(base.c_lockLv.ToString() ?? ""), FontColor.Warning);
		}
		if (base.isCrafted && recipe == null)
		{
			n.AddText("isCrafted".lang());
		}
		if (trait.Decay > 0)
		{
			string text6 = "";
			text6 = (base.IsDecayed ? "isRotten" : (base.IsRotting ? "isRotting" : ((!base.IsFresn) ? "isNotFresh" : "isFresh")));
			n.AddText(text6.lang());
		}
		if (base.isDyed)
		{
			n.AddText("isDyed".lang());
		}
		if (base.IsEquipment)
		{
			text = "isEquipable".lang(Element.Get(base.category.slot).GetText());
			n.AddText(text);
		}
		if (base.isFireproof)
		{
			n.AddText("isFreproof");
		}
		if (base.isAcidproof)
		{
			n.AddText("isAcidproof");
		}
		if (trait.Electricity > 0)
		{
			n.AddText("isGenerateElectricity".lang(trait.Electricity.ToString() ?? ""));
		}
		if (trait.Electricity < 0)
		{
			n.AddText("isConsumeElectricity".lang(Mathf.Abs(trait.Electricity).ToString() ?? ""));
		}
		if (base.IsUnique)
		{
			n.AddText("isPrecious");
		}
		if (base.isCopy)
		{
			n.AddText("isCopy");
		}
		if (!trait.CanBeDestroyed)
		{
			n.AddText("isIndestructable");
		}
		if (GetInt(107) > 0)
		{
			n.AddText("isLicked");
		}
		if (!base.c_idDeity.IsEmpty())
		{
			Religion religion = EClass.game.religions.Find(base.c_idDeity) ?? EClass.game.religions.Eyth;
			n.AddText("isDeity".lang(religion.Name), FontColor.Myth);
		}
		if (base.isGifted && GetRoot() != EClass.pc)
		{
			n.AddText("isGifted", FontColor.Ether);
		}
		if (base.isNPCProperty)
		{
			n.AddText("isNPCProperty", FontColor.Ether);
		}
		if (base.c_priceFix != 0)
		{
			n.AddText(((base.c_priceFix > 0) ? "isPriceUp" : "isPriceDown").lang(Mathf.Abs(base.c_priceFix).ToString() ?? ""), FontColor.Ether);
		}
		if (base.noSell)
		{
			n.AddText("isNoSell", FontColor.Ether);
		}
		if (base.isStolen)
		{
			n.AddText("isStolen", FontColor.Ether);
		}
		if (base.c_isImportant)
		{
			n.AddText("isMarkedImportant", FontColor.Ether);
		}
		if (GetInt(25) != 0)
		{
			n.AddText("isDangerLv".lang((GetInt(25) + 1).ToString() ?? "", (EClass.pc.FameLv + 10).ToString() ?? ""));
		}
		FontColor color = FontColor.Util;
		if (trait is TraitTool && !(trait is TraitToolRange))
		{
			if (HasElement(220))
			{
				n.AddText("canMine".lang(), color);
			}
			if (HasElement(225))
			{
				n.AddText("canLumberjack".lang(), color);
				n.AddText("canLumberjack2".lang(), color);
			}
			if (HasElement(230))
			{
				n.AddText("canDig", color);
			}
			if (HasElement(286))
			{
				n.AddText("canFarm", color);
			}
			if (HasElement(245))
			{
				n.AddText("canFish", color);
			}
			if (HasElement(237))
			{
				n.AddText("canTame", color);
			}
		}
		if (trait is TraitToolMusic)
		{
			n.AddText("canPlayMusic".lang(), color);
		}
		if (Lang.Has("hint_" + trait.ToString()))
		{
			n.AddText("hint_" + trait.ToString(), FontColor.Util);
		}
		if (Lang.Has("hint_" + trait.ToString() + "2"))
		{
			n.AddText("hint_" + trait.ToString() + "2", FontColor.Util);
		}
		if (HasTag(CTAG.tourism))
		{
			n.AddText("isTourism", FontColor.Util);
		}
		string langPlaceType = base.TileType.LangPlaceType;
		if (langPlaceType == "place_Door" || langPlaceType == "place_WallMount")
		{
			n.AddText(base.TileType.LangPlaceType + "_hint".lang(), FontColor.Util);
		}
		if (trait.IsHomeItem)
		{
			n.AddText("isHomeItem".lang(), FontColor.Util);
		}
		if (HasTag(CTAG.throwWeapon))
		{
			n.AddText("isThrowWeapon");
		}
		if (EClass.debug.showExtra && HasTag(CTAG.throwWeaponEnemy))
		{
			n.AddText("isThrowWeaponEnemy");
		}
		if (HasElement(10))
		{
			n.AddText("isEdible");
		}
		if (HasTag(CTAG.rareResource))
		{
			n.AddText("isRareResource", FontColor.Great);
		}
		if (trait is TraitBed traitBed)
		{
			n.AddText("isBed".lang(traitBed.MaxHolders.ToString() ?? ""));
		}
		bool flag3 = base.IsEquipmentOrRanged || base.IsAmmo || base.IsThrownWeapon;
		if (flag2)
		{
			if (flag3)
			{
				Element element = elements.GetElement(653);
				if (element != null)
				{
					n.AddText("isAlive".lang(element.vBase.ToString() ?? "", (element.vExp / 10).ToString() ?? "", (element.ExpToNext / 10).ToString() ?? ""), FontColor.Great);
				}
				string[] rangedSubCats = new string[2] { "eleConvert", "eleAttack" };
				elements.AddNote(n, delegate(Element e)
				{
					if (trait is TraitToolRange && base.category.slot == 0 && !(e is Ability) && !rangedSubCats.Contains(e.source.categorySub) && !e.HasTag("modRanged"))
					{
						return false;
					}
					if (e.IsTrait)
					{
						return false;
					}
					if (e.source.categorySub == "eleAttack" && !base.IsWeapon && !base.IsRangedWeapon && !base.IsAmmo && !base.IsThrownWeapon)
					{
						return false;
					}
					return (!showEQStats || (e.id != 64 && e.id != 65 && e.id != 66 && e.id != 67)) ? true : false;
				});
			}
			if (sockets != null)
			{
				foreach (int socket in sockets)
				{
					n.AddText((socket == 0) ? "emptySocket".lang() : "socket".lang(EClass.sources.elements.map[socket / 100].GetName(), (socket % 100).ToString() ?? ""), FontColor.Gray);
				}
			}
		}
		else
		{
			n.AddText("isUnidentified".lang(), FontColor.Flavor);
			if (base.c_IDTState == 1)
			{
				n.AddText("isUnidentified2".lang(), FontColor.Flavor);
			}
		}
		trait.WriteNote(n, flag2);
		if (flag2 && !flag3)
		{
			bool infoMode = mode == IInspect.NoteMode.Info;
			List<Element> list = ListValidTraits(isCraft: false, !infoMode);
			List<Element> list2 = ListValidTraits(isCraft: false, limit: false);
			if (list2.Count - list.Count <= 1)
			{
				list = list2;
			}
			elements.AddNote(n, (Element e) => list.Contains(e), null, ElementContainer.NoteMode.Trait, addRaceFeat: false, delegate(Element e, string s)
			{
				string text7 = s;
				string text8 = e.source.GetText("textExtra");
				if (!text8.IsEmpty())
				{
					string text9 = "";
					if (e.id == 2 && mode == IInspect.NoteMode.Product)
					{
						int num = recipe.GetQualityBonus() / 10;
						if (num >= 0)
						{
							num++;
						}
						text9 = "qualityLimit".lang(num.ToString() ?? "");
					}
					int num2 = e.Value / 10;
					num2 = ((e.Value < 0) ? (num2 - 1) : (num2 + 1));
					text8 = "Lv." + num2 + text9 + " " + text8;
					if (infoMode && e.IsFoodTraitMain)
					{
						text8 += "traitAdditive".lang();
					}
					text7 += (" <size=12>" + text8 + "</size>").TagColor(FontColor.Passive);
				}
				return text7;
			}, delegate
			{
			});
			if (base.ShowFoodEnc && EClass.pc.HasElement(1650))
			{
				if (FoodEffect.IsHumanFlesh(this))
				{
					n.AddText("foodHuman".lang(), FontColor.Ether);
				}
				if (FoodEffect.IsUndeadFlesh(this))
				{
					n.AddText("foodUndead".lang(), FontColor.Ether);
				}
			}
			if (list.Count != list2.Count)
			{
				n.AddText("traitOther".lang((list2.Count - list.Count).ToString() ?? ""));
			}
			if (mode == IInspect.NoteMode.Product && HasTag(CTAG.dish_bonus))
			{
				n.AddHeader("HeaderAdditionalTrait", "additional_trait");
				source.model.elements.AddNote(n, (Element e) => e.IsFoodTraitMain, null, ElementContainer.NoteMode.Trait, addRaceFeat: false, delegate(Element e, string s)
				{
					string text10 = s;
					string text11 = e.source.GetText("textExtra");
					if (!text11.IsEmpty())
					{
						string text12 = "";
						int num3 = e.Value / 10;
						num3 = ((e.Value < 0) ? (num3 - 1) : (num3 + 1));
						text11 = "Lv." + num3 + text12 + " " + text11;
						if (infoMode && e.IsFoodTraitMain)
						{
							text11 += "traitAdditive".lang();
						}
						text10 += (" <size=12>" + text11 + "</size>").TagColor(FontColor.Passive);
					}
					return text10;
				});
			}
		}
		if (EClass.debug.showExtra)
		{
			n.AddText("decay:" + base.decay);
			n.AddText(base.isDyed + "/" + id + "/" + base.refVal + "/" + base.LV + "/" + trait);
			if (source.origin != null)
			{
				n.AddText(source.origin.id);
			}
		}
		if (id == "statue_weird")
		{
			n.AddText("weirdStatue");
		}
		if (base.isReplica)
		{
			n.AddText("isReplica", FontColor.Passive);
		}
		if (flag2)
		{
			Chara chara = GetRootCard() as Chara;
			if (base.parentCard?.trait is TraitChestMerchant)
			{
				chara = null;
			}
			if (base.c_equippedSlot != 0 && base.category.slot == 35 && chara != null)
			{
				AddAttackEvaluation(n, chara, this);
			}
			if (base.IsThrownWeapon || base.IsRangedWeapon || (base.IsMeleeWeapon && base.c_equippedSlot == 0))
			{
				n.AddHeader("headerAttackEval");
				AttackProcess.Current.Prepare(chara ?? EClass.pc, this, null, null, 0, base.IsThrownWeapon);
				string text13 = AttackProcess.Current.GetText();
				text13 = text13.TagColor(() => true);
				n.AddText(text13);
			}
		}
		if (base.ammoData != null)
		{
			n.AddHeader("headerAttackAmmo");
			n.AddText(base.ammoData.Name);
		}
		onWriteNote?.Invoke(n);
		if ((bool)LayerDragGrid.Instance)
		{
			LayerDragGrid.Instance.owner.OnWriteNote(this, n);
		}
		if (EClass.debug.showExtra)
		{
			foreach (Element value in elements.dict.Values)
			{
				n.AddText(value.source.alias + "/" + value.Value + "/" + value.vBase + "/" + value.vSource);
			}
		}
		n.Build();
	}

	public static void AddAttackEvaluation(UINote n, Chara chara, Thing current = null)
	{
		n.AddHeader("headerAttackEval");
		int num = 0;
		foreach (BodySlot slot in chara.body.slots)
		{
			if (slot.thing == null || slot.elementId != 35 || slot.thing.source.offense.Length < 2)
			{
				continue;
			}
			AttackProcess.Current.Prepare(chara, slot.thing, null, null, num);
			string text = AttackProcess.Current.GetText();
			if (slot.thing == current)
			{
				text = text.TagColor(() => true);
			}
			n.AddText(text);
			num++;
		}
		AttackProcess.Current.Prepare(chara, null);
		string text2 = AttackProcess.Current.GetText();
		if (num == 0)
		{
			text2 = text2.TagColor(() => true);
		}
		n.AddText(text2);
	}

	public override void SetRenderParam(RenderParam p)
	{
		p.matColor = base.colorInt;
		p.mat = base.material;
		if (!renderer.usePass)
		{
			return;
		}
		switch (trait.tileMode)
		{
		case Trait.TileMode.DefaultNoAnime:
			if (source._altTiles.Length != 0 && trait.UseAltTiles)
			{
				p.tile = source._altTiles[base.dir % source._altTiles.Length] * ((!flipX) ? 1 : (-1));
			}
			else
			{
				p.tile = sourceCard._tiles[base.dir % sourceCard._tiles.Length] * ((!flipX) ? 1 : (-1));
			}
			break;
		case Trait.TileMode.Default:
			if (source._altTiles.Length != 0 && trait.UseAltTiles)
			{
				p.tile = source._altTiles[base.dir % source._altTiles.Length] * ((!flipX) ? 1 : (-1));
			}
			else
			{
				p.tile = sourceCard._tiles[base.dir % sourceCard._tiles.Length] * ((!flipX) ? 1 : (-1));
			}
			if (source.anime.Length == 0 || !trait.IsAnimeOn)
			{
				break;
			}
			if (source.anime.Length > 2)
			{
				float num = Time.realtimeSinceStartup * 1000f / (float)source.anime[1] % (float)source.anime[2];
				if ((int)num == source.anime[0] - 1 && source.anime.Length > 3)
				{
					PlaySound("anime_sound" + source.anime[3]);
				}
				if (!(num >= (float)source.anime[0]))
				{
					p.tile += num * (float)((!flipX) ? 1 : (-1));
				}
			}
			else
			{
				float num2 = Time.realtimeSinceStartup * 1000f / (float)source.anime[1] % (float)source.anime[0];
				p.tile += num2 * (float)((!flipX) ? 1 : (-1));
			}
			break;
		case Trait.TileMode.SignalAnime:
			if (source._altTiles.Length != 0 && trait.UseAltTiles)
			{
				p.tile = source._altTiles[base.dir % source._altTiles.Length] * ((!flipX) ? 1 : (-1));
			}
			else
			{
				p.tile = sourceCard._tiles[base.dir % sourceCard._tiles.Length] * ((!flipX) ? 1 : (-1));
			}
			if (animeCounter > 0f)
			{
				animeCounter += Time.deltaTime;
				int num3 = (int)(animeCounter / (0.001f * (float)source.anime[1]));
				if (num3 > source.anime[2])
				{
					animeCounter = 0f;
				}
				else
				{
					p.tile += num3 % source.anime[0] * ((!flipX) ? 1 : (-1));
				}
			}
			break;
		case Trait.TileMode.Illumination:
			if (base.isOn || base.isRoofItem)
			{
				int num4 = (int)((float)base.uid + Time.realtimeSinceStartup * 5f);
				int num5 = (int)(Time.realtimeSinceStartup * 5f);
				p.tile = (sourceCard._tiles[base.dir % sourceCard._tiles.Length] + num4 % 3 + 1) * ((!flipX) ? 1 : (-1));
				if (num5 % 16 == 0)
				{
					p.color = 5242880f;
				}
				else if (num5 % 11 == 0)
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
				p.tile = sourceCard._tiles[base.dir % sourceCard._tiles.Length] * ((!flipX) ? 1 : (-1));
			}
			break;
		case Trait.TileMode.Door:
			if (source._altTiles.Length != 0 && trait.UseAltTiles)
			{
				p.tile = source._altTiles[base.dir % source._altTiles.Length] * ((!flipX) ? 1 : (-1));
			}
			else
			{
				p.tile = sourceCard._tiles[base.dir % sourceCard._tiles.Length] * ((!flipX) ? 1 : (-1));
			}
			if (parent is Zone && pos.cell.HasFullBlock)
			{
				p.tile += ((p.tile < 0f) ? (-64) : 64);
			}
			if (trait is TraitDoorSwing traitDoorSwing && traitDoorSwing.IsOpen())
			{
				p.tile += ((!(p.tile < 0f)) ? 1 : (-1));
			}
			break;
		}
		if (base.idSkin != 0)
		{
			int num6 = base.idSkin - 1;
			if (sourceCard.skins.Length != 0)
			{
				p.tile += ((p.tile < 0f) ? (-sourceCard.skins[num6]) : sourceCard.skins[num6]);
			}
		}
	}

	public override SubPassData GetSubPassData()
	{
		Chara chara = GetRootCard() as Chara;
		if ((!trait.ShowAsTool || (chara == EClass.pc && HotItemHeld.disableTool)) && (chara?.held == this || (placeState != PlaceState.installed && renderer.data.subCrate.enable && parent is Zone)))
		{
			if (!renderer.data.subCrate.enable)
			{
				return EClass.setting.pass.subCrate;
			}
			return renderer.data.subCrate;
		}
		return SubPassData.Default;
	}

	public override bool CanStackTo(Thing to)
	{
		if (trait.HasCharges || to.isEquipped || base.isModified || to.isModified || to.id != id || to.idMaterial != base.idMaterial || to.refVal != base.refVal || to.blessedState != base.blessedState || to.rarityLv != base.rarityLv || to.qualityTier != base.qualityTier || to.idSkin != base.idSkin || to.isGifted != base.isGifted)
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
		if (!trait.CanStackTo(to))
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
		if ((to.isWeightChanged || base.isWeightChanged) && to.SelfWeight != SelfWeight)
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
		if (trait.IsRequireFuel && base.c_charges != to.c_charges)
		{
			return false;
		}
		if (base.c_altName != to.c_altName)
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
			if (elements.dict.Count() != to.elements.dict.Count())
			{
				return false;
			}
			foreach (Element value in elements.dict.Values)
			{
				if (to.elements.GetElement(value.id) == null)
				{
					return false;
				}
			}
			int num2 = (to.encLV = Mathf.CeilToInt(1f * (float)(base.encLV * base.Num + to.encLV * to.Num) / (float)(base.Num + to.Num)));
			base.encLV = num2;
			foreach (Element value2 in elements.dict.Values)
			{
				Element element = to.elements.GetElement(value2.id);
				value2.vBase = (element.vBase = (value2.vBase * base.Num + element.vBase * to.Num) / (base.Num + to.Num));
			}
			return true;
		}
		if (base.encLV != to.encLV)
		{
			return false;
		}
		if (elements.dict.Count() != to.elements.dict.Count())
		{
			return false;
		}
		foreach (Element value3 in elements.dict.Values)
		{
			Element element2 = to.elements.GetElement(value3.id);
			if (element2 == null || value3.vBase / 10 * 10 != element2.vBase / 10 * 10)
			{
				return false;
			}
		}
		return true;
	}

	public void GetIngredients(Recipe.Ingredient ing, List<Thing> list)
	{
		if (ing.CanSetThing(this))
		{
			list.Add(this);
		}
		if (things.Count <= 0 || base.c_lockLv != 0 || base.isNPCProperty)
		{
			return;
		}
		foreach (Thing thing in things)
		{
			thing.GetIngredients(ing, list);
		}
	}

	public bool IsValidIngredient(Recipe.Ingredient ing)
	{
		if (isDestroyed)
		{
			return false;
		}
		if (GetRootCard().Dist(EClass.pc) > 1 || !ing.CanSetThing(this))
		{
			return false;
		}
		return true;
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
		int count = 1;
		UIContextMenu m = EClass.ui.CreateContextMenuInteraction();
		bool buy = trans != null;
		UIButton buttonBuy = null;
		UIItem itemSlider = null;
		itemSlider = m.AddSlider("sliderSplitMenu", "adjustmentNum", (float a) => (!EClass.core.IsGameStarted) ? "" : ("/" + base.Num), count, delegate(float b)
		{
			count = (int)b;
			if (trans != null)
			{
				trans.num = count;
			}
			UpdateButton();
		}, 1f, base.Num, isInt: true, hideOther: false, useInput: true).GetComponent<UIItem>();
		if (buy)
		{
			buttonBuy = m.AddButton("invBuy", delegate
			{
				Process();
			});
		}
		m.onDestroy = delegate
		{
			if (!buy && !m.wasCanceled)
			{
				Process();
			}
		};
		m.Show();
		if ((bool)buttonBuy)
		{
			buttonBuy.gameObject.AddComponent<CanvasGroup>();
		}
		UpdateButton();
		void Process()
		{
			if (!EClass.core.IsGameStarted || button == null || button.card == null)
			{
				Debug.Log("Split bug1");
			}
			else if (button.card.isDestroyed || button.card.Num < count)
			{
				Debug.Log("Split bug2");
			}
			else if (EClass.pc.isDead)
			{
				Debug.Log("Split bug3");
			}
			else if (count != 0 && !Input.GetMouseButton(1))
			{
				if (trans != null)
				{
					trans.Process(startTransaction: true);
				}
				else
				{
					DragItemCard dragItemCard = new DragItemCard(button);
					if (count != base.Num)
					{
						Thing thing = button.card.Split(base.Num - count);
						button.invOwner.Container.AddThing(thing, tryStack: false);
						thing.invX = dragItemCard.from.invX;
						thing.invY = dragItemCard.from.invY;
						thing.posInvX = button.card.Thing.posInvX;
						thing.posInvY = button.card.Thing.posInvY;
					}
					EClass.ui.StartDrag(dragItemCard);
				}
			}
		}
		void UpdateButton()
		{
			itemSlider.text1.text = GetName(NameStyle.FullNoArticle, 1);
			itemSlider.text2.text = Lang._weight(SelfWeight * count);
			if ((bool)buttonBuy)
			{
				buttonBuy.mainText.SetText(trans.GetTextDetail());
				buttonBuy.mainText.RebuildLayoutTo<UIButton>();
				buttonBuy.interactable = trans.IsValid();
				buttonBuy.RebuildLayout(recursive: true);
				buttonBuy.gameObject.GetComponent<CanvasGroup>().alpha = (trans.IsValid() ? 1f : 0.9f);
			}
		}
	}

	public void ShowSplitMenu2(ButtonGrid button, string lang, Action<int> onSplit = null)
	{
		int count = 1;
		UIContextMenu uIContextMenu = EClass.ui.CreateContextMenuInteraction();
		UIButton buttonBuy = null;
		UIItem itemSlider = null;
		itemSlider = uIContextMenu.AddSlider("sliderSplitMenu", "adjustmentNum", (float a) => (!EClass.core.IsGameStarted) ? "" : ("/" + base.Num), count, delegate(float b)
		{
			count = (int)b;
			UpdateButton();
		}, 1f, base.Num, isInt: true, hideOther: false, useInput: true).GetComponent<UIItem>();
		buttonBuy = uIContextMenu.AddButton("invBuy", delegate
		{
			Process();
		});
		uIContextMenu.onDestroy = delegate
		{
		};
		uIContextMenu.Show();
		if ((bool)buttonBuy)
		{
			buttonBuy.gameObject.AddComponent<CanvasGroup>();
		}
		UpdateButton();
		void Process()
		{
			if (!EClass.core.IsGameStarted || button == null || button.card == null)
			{
				Debug.Log("Split bug1");
			}
			else if (button.card.isDestroyed || button.card.Num < count)
			{
				Debug.Log("Split bug2");
			}
			else if (EClass.pc.isDead)
			{
				Debug.Log("Split bug3");
			}
			else if (count != 0 && !Input.GetMouseButton(1))
			{
				onSplit?.Invoke(count);
			}
		}
		void UpdateButton()
		{
			itemSlider.text1.text = GetName(NameStyle.FullNoArticle, 1);
			itemSlider.text2.text = Lang._weight(SelfWeight * count);
			buttonBuy.mainText.SetText(lang.lang(count.ToString() ?? ""));
			buttonBuy.mainText.RebuildLayoutTo<UIButton>();
			buttonBuy.interactable = true;
			buttonBuy.RebuildLayout(recursive: true);
		}
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
			act.Perform(EClass.pc);
			return;
		}
		AIAct aI = act as AIAct;
		EClass.pc.SetAI(aI);
		ActionMode.Adv.SetTurbo();
	}

	public static Tuple<SourceElement.Row, int> GetEnchant(int lv, Func<SourceElement.Row, bool> func, bool neg)
	{
		List<SourceElement.Row> list = new List<SourceElement.Row>();
		int num = 0;
		int num2 = lv + 5 + EClass.rndSqrt(10);
		float num3 = (float)(3 + Mathf.Min(lv / 10, 15)) + Mathf.Sqrt(lv);
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
		foreach (SourceElement.Row item2 in list)
		{
			num5 += item2.chance;
			if (num4 < num5)
			{
				string text = EClass.sources.elements.map[item2.id].category;
				bool flag = text == "skill" || text == "attribute" || text == "resist";
				int item = (item2.mtp + EClass.rnd(item2.mtp + (int)num3)) / item2.mtp * ((!(flag && neg)) ? 1 : (-1));
				return new Tuple<SourceElement.Row, int>(item2, item);
			}
		}
		return null;
	}

	public void TryLickEnchant(Chara c, bool msg = true, Chara tg = null, BodySlot slot = null)
	{
		if (!base.IsEquipmentOrRanged || base.IsCursed || base.rarity <= Rarity.Normal || GetInt(107) > 0)
		{
			return;
		}
		if (tg == null)
		{
			Rand.SetSeed(EClass.world.date.day + source._index + c.uid);
			if (msg)
			{
				c.Say("lick", c, this);
				PlaySound("offering");
				PlayEffect("mutation");
			}
			AddEnchant(base.LV);
		}
		else
		{
			Rand.SetSeed(base.uid);
			List<Element> list = new List<Element>();
			foreach (Element value in elements.dict.Values)
			{
				if (value.id != 67 && value.id != 66 && value.id != 64 && value.id != 65)
				{
					list.Add(value);
				}
			}
			if (list.Count > 0)
			{
				Element element = list.RandomItem();
				elements.ModBase(element.id, Mathf.Max(EClass.rnd(Mathf.Abs(element.vBase / 5)), 1));
			}
			if (msg)
			{
				c.Say("lick2", c, tg, slot.name.ToLower());
				tg.PlaySound("offering");
				tg.PlayEffect("mutation");
			}
		}
		Rand.SetSeed();
		SetInt(107, 1);
	}

	public Element AddEnchant(int lv = -1)
	{
		if (base.IsToolbelt || base.IsLightsource)
		{
			return null;
		}
		Tuple<SourceElement.Row, int> enchant = GetEnchant(lv, (SourceElement.Row r) => r.IsEncAppliable(base.category), base.IsCursed);
		if (enchant == null)
		{
			return null;
		}
		return elements.ModBase(enchant.Item1.id, enchant.Item2);
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
		string text = "";
		if (show)
		{
			@ref = GetName(NameStyle.Full, base.Num);
		}
		Rarity rarity = idtSource switch
		{
			IDTSource.SkillHigh => Rarity.Legendary, 
			IDTSource.Skill => Rarity.Superior, 
			_ => Rarity.Normal, // == 0
		};
		if (rarity != 0 && ((base.IsEquipmentOrRanged && base.rarity >= rarity) || base.rarity >= Rarity.Mythical)) // rarity != 0 checks whether idtSource was Skill or SkillHigh
		{
			base.c_IDTState = 3;
		}
		else if (base.rarity >= Rarity.Mythical && idtSource != IDTSource.SuperiorIdentify) // Mythical rarity requires superior identify
		{
			base.c_IDTState = 1;
		}
		else // Other items are identified successfully
		{
			base.c_IDTState = 0;
		}
		if (show)
		{
			text = GetName(NameStyle.Full, base.Num);
			if (base.c_IDTState == 0)
			{
				Msg.Say("identified", @ref, text);
			}
			else
			{
				Msg.Say((idtSource == IDTSource.Skill) ? "identified3" : "identified2", @ref, text, base.TextRarity);
			}
		}
		if (base.IsIdentified)
		{
			GetRootCard()?.TryStack(this);
		}
		LayerInventory.SetDirty(this);
		return this;
	}
}
public static class THING
{
	public const string potionCureCorruption = "1165";
}
