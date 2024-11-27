using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Element : EClass
{
	public static string GetName(string alias)
	{
		return EClass.sources.elements.alias[alias].GetName();
	}

	public static SourceElement.Row Get(int id)
	{
		return EClass.sources.elements.map[id];
	}

	public virtual SourceElement.Row GetSource()
	{
		return EClass.sources.elements.alias.TryGetValue(base.GetType().ToString(), null);
	}

	public SourceElement.Row source
	{
		get
		{
			SourceElement.Row result;
			if ((result = this._source) == null)
			{
				result = (this._source = (((this.id == 0) ? this.GetSource() : Element.Get(this.id)) ?? EClass.sources.elements.rows[0]));
			}
			return result;
		}
	}

	public virtual int DisplayValue
	{
		get
		{
			return this.Value;
		}
	}

	public virtual int MinValue
	{
		get
		{
			return -100;
		}
	}

	public int Value
	{
		get
		{
			return this.ValueWithoutLink + this.vLink + ((this.owner != null) ? this.owner.ValueBonus(this) : 0);
		}
	}

	public int ValueWithoutLink
	{
		get
		{
			return this.vBase + this.vSource;
		}
	}

	public virtual int MinPotential
	{
		get
		{
			return 100;
		}
	}

	public int Potential
	{
		get
		{
			return this.vPotential + this.vTempPotential + this.vSourcePotential + this.MinPotential;
		}
	}

	public virtual bool CanGainExp
	{
		get
		{
			return this.ValueWithoutLink > 0;
		}
	}

	public bool IsFlag
	{
		get
		{
			return this.source.tag.Contains("flag");
		}
	}

	public virtual int GetSourcePotential(int v)
	{
		return 0;
	}

	public virtual Sprite GetSprite()
	{
		return null;
	}

	public int GetMaterialSourceValue(Thing t, int v)
	{
		if (this.id == 2)
		{
			return v;
		}
		if (!this.IsTrait)
		{
			return v * (100 + t.encLV * 10) / 100;
		}
		if (t.IsFurniture)
		{
			return v;
		}
		return Mathf.Min(v + t.encLV * 10, 60);
	}

	public virtual int GetSourceValue(int v, int lv, SourceValueType type)
	{
		if (type == SourceValueType.Chara)
		{
			return v * (100 + (lv - 1 + EClass.rnd(lv / 2 + 1)) * this.source.lvFactor / 10) / 100 + EClass.rnd(lv / 3) * this.source.lvFactor / 100;
		}
		if (type != SourceValueType.Fixed)
		{
			return v * ((this.source.encFactor == 0) ? 100 : (50 + EClass.rnd(100) + EClass.rnd((int)Mathf.Sqrt((float)(lv * 100))) * this.source.encFactor / 100)) / 100;
		}
		return v;
	}

	public virtual string Name
	{
		get
		{
			return this.source.GetName();
		}
	}

	public virtual string FullName
	{
		get
		{
			return this.Name;
		}
	}

	public virtual Sprite GetIcon(string suffix = "")
	{
		Sprite result;
		if ((result = SpriteSheet.Get("Media/Graphics/Icon/Element/icon_elements", "ele_" + this.source.alias + suffix)) == null)
		{
			result = (SpriteSheet.Get("Media/Graphics/Icon/Element/icon_elements", "ele_" + this.source.aliasParent + suffix) ?? SpriteSheet.Get("Media/Graphics/Icon/Element/icon_elements", "cat_" + this.source.category));
		}
		return result;
	}

	public virtual int ExpToNext
	{
		get
		{
			return 1000;
		}
	}

	public virtual bool UseExpMod
	{
		get
		{
			return true;
		}
	}

	public virtual int CostTrain
	{
		get
		{
			return Mathf.Max((this.ValueWithoutLink / 10 + 5) * (100 + this.vTempPotential) / 500, 1);
		}
	}

	public virtual int CostLearn
	{
		get
		{
			return 5;
		}
	}

	public virtual bool ShowXP
	{
		get
		{
			return EClass.debug.showExtra || this.source.category != "attribute";
		}
	}

	public virtual bool ShowMsgOnValueChanged
	{
		get
		{
			return true;
		}
	}

	public virtual bool ShowValue
	{
		get
		{
			return true;
		}
	}

	public virtual bool ShowPotential
	{
		get
		{
			return true;
		}
	}

	public virtual bool UsePotential
	{
		get
		{
			return true;
		}
	}

	public virtual bool PotentialAsStock
	{
		get
		{
			return false;
		}
	}

	public virtual bool ShowRelativeAttribute
	{
		get
		{
			return false;
		}
	}

	public virtual string ShortName
	{
		get
		{
			return this.Name;
		}
	}

	public bool IsGlobalElement
	{
		get
		{
			return this.vExp == -1 || this.vExp == -2;
		}
	}

	public bool IsFactionWideElement
	{
		get
		{
			return this.vExp == -1;
		}
	}

	public bool IsPartyWideElement
	{
		get
		{
			return this.vExp == -2;
		}
	}

	public int SortVal(bool charaSheet = false)
	{
		int num = (this.source.sort != 0) ? this.source.sort : this.id;
		return (this.IsFlag ? 100000 : 0) + ((!charaSheet && this.IsGlobalElement) ? -1000000 : 0) + num;
	}

	public virtual bool CanLink(ElementContainer owner)
	{
		return false;
	}

	public virtual bool ShowEncNumber
	{
		get
		{
			return true;
		}
	}

	public bool IsTrait
	{
		get
		{
			return this.source.tag.Contains("trait");
		}
	}

	public bool IsFoodTrait
	{
		get
		{
			return !this.source.foodEffect.IsEmpty();
		}
	}

	public bool IsFoodTraitMain
	{
		get
		{
			return this.IsFoodTrait && (this.source.tag.Contains("primary") || this.source.tag.Contains("foodpot"));
		}
	}

	public bool IsMainAttribute
	{
		get
		{
			return this.source.category == "attribute" && this.source.tag.Contains("primary");
		}
	}

	public Act act
	{
		get
		{
			return (this as Act) ?? ACT.Create(this.id);
		}
	}

	public bool HasTag(string tag)
	{
		return this.source.tag.Contains(tag);
	}

	public void SetImage(Image i)
	{
		Sprite icon = this.GetIcon("");
		if (icon)
		{
			i.sprite = icon;
			i.SetNativeSize();
		}
	}

	public virtual string GetDetail()
	{
		return this.source.GetDetail();
	}

	public bool IsFactionElement(Chara c)
	{
		if (c == null)
		{
			return false;
		}
		if (c.IsPCFaction)
		{
			foreach (Element element in EClass.pc.faction.charaElements.dict.Values)
			{
				if (element.id == this.id && element.Value > 0)
				{
					return true;
				}
			}
		}
		if (c.faithElements != null)
		{
			foreach (Element element2 in c.faithElements.dict.Values)
			{
				if (element2.id == this.id && element2.Value > 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	public Element GetParent(Card c)
	{
		if (!this.source.aliasParent.IsEmpty())
		{
			return c.elements.GetOrCreateElement(this.source.aliasParent);
		}
		return null;
	}

	public static Dictionary<int, int> GetElementMap(int[] list)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		if (list != null)
		{
			for (int i = 0; i < list.Length / 2; i++)
			{
				dictionary[list[i * 2]] = list[i * 2 + 1];
			}
		}
		return dictionary;
	}

	public static Dictionary<int, int> GetElementMap(int[] list, Dictionary<int, int> map)
	{
		if (list != null)
		{
			for (int i = 0; i < list.Length / 2; i++)
			{
				map[list[i * 2]] = list[i * 2 + 1];
			}
		}
		return map;
	}

	public static SourceElement.Row GetRandomElement(int lv = 1)
	{
		if (Element.ListElements.Count == 0)
		{
			foreach (SourceElement.Row row in EClass.sources.elements.rows)
			{
				if (row.categorySub == "eleAttack" && row.chance > 0)
				{
					Element.ListElements.Add(row);
				}
			}
		}
		List<Tuple<SourceElement.Row, int>> list = new List<Tuple<SourceElement.Row, int>>();
		foreach (SourceElement.Row row2 in Element.ListElements)
		{
			int num = 40 * (row2.eleP - 100) / 100;
			if (list.Count == 0 || num < lv)
			{
				list.Add(new Tuple<SourceElement.Row, int>(row2, num));
			}
		}
		return list.RandomItemWeighted((Tuple<SourceElement.Row, int> a) => (float)(10000 / (100 + (lv - a.Item2) * 25))).Item1;
	}

	public void WriteNote(UINote n, ElementContainer owner = null, Action<UINote> onWriteNote = null)
	{
		n.Clear();
		this._WriteNote(n, owner, onWriteNote, false, true);
		n.Build();
	}

	public void WriteNoteWithRef(UINote n, ElementContainer owner, Action<UINote> onWriteNote, Element refEle)
	{
		n.Clear();
		this._WriteNote(n, owner, onWriteNote, false, true);
		refEle._WriteNote(n, owner, onWriteNote, true, true);
		n.Build();
	}

	public void _WriteNote(UINote n, Chara c, Act act)
	{
		Element.<>c__DisplayClass101_0 CS$<>8__locals1;
		CS$<>8__locals1.c = c;
		CS$<>8__locals1.e = CS$<>8__locals1.c.elements.GetOrCreateElement(act.source.id);
		Act.Cost cost = CS$<>8__locals1.e.GetCost(CS$<>8__locals1.c);
		CS$<>8__locals1.p = CS$<>8__locals1.e.GetPower(CS$<>8__locals1.c);
		n.Space(6, 1);
		string text = this.source.GetText("textExtra", false);
		if (!text.IsEmpty())
		{
			foreach (string text2 in text.Split(',', StringSplitOptions.None))
			{
				if (text2.StartsWith("@"))
				{
					Condition condition = Condition.Create(text2.Replace("@", ""), CS$<>8__locals1.p, null);
					condition.owner = CS$<>8__locals1.c;
					if (!this.source.aliasRef.IsEmpty())
					{
						condition.SetElement(EClass.sources.elements.alias[this.source.aliasRef].id);
					}
					int num = act.id;
					if (num != 6902)
					{
						if (num == 8510 || num == 8710)
						{
							condition.SetRefVal(79, (act.id == 8710) ? 221 : 220);
						}
					}
					else
					{
						condition.SetRefVal(79, 266);
					}
					n.AddText("_bullet".lang() + "hintCon".lang(condition.Name, condition.EvaluateTurn(CS$<>8__locals1.p).ToString() ?? "", null, null, null), FontColor.DontChange);
					condition._WriteNote(n, true);
				}
				else
				{
					string text3 = text2.Replace("#calc", Element.<_WriteNote>g__Calc|101_0(ref CS$<>8__locals1));
					if (!this.source.aliasRef.IsEmpty())
					{
						text3 = text3.Replace("#ele", EClass.sources.elements.alias[this.source.aliasRef].GetName().ToLower());
					}
					n.AddText("_bullet".lang() + text3, FontColor.DontChange);
				}
			}
		}
		if (this.source.tag.Contains("syncRide"))
		{
			n.AddText("_bullet".lang() + "hintSyncRide".lang(), FontColor.DontChange);
		}
		if (act.HaveLongPressAction)
		{
			int i = this.id;
			if (i != 8230 && i != 8232)
			{
				n.AddText("_bullet".lang() + "hintPartyAbility".lang(), FontColor.DontChange);
			}
		}
		if (!act.LocalAct)
		{
			n.Space(0, 1);
			n.AddText("isGlobalAct".lang(), FontColor.DontChange);
		}
		if (cost.type != Act.CostType.None && cost.cost != 0)
		{
			n.Space(4, 1);
			UIItem uiitem = n.AddExtra<UIItem>("costPrice");
			uiitem.text1.SetText(cost.cost.ToString() ?? "", (((cost.type == Act.CostType.MP) ? CS$<>8__locals1.c.mana.value : CS$<>8__locals1.c.stamina.value) >= cost.cost) ? FontColor.Good : FontColor.Bad);
			uiitem.image1.sprite = ((cost.type == Act.CostType.MP) ? EClass.core.refs.icons.mana : EClass.core.refs.icons.stamina);
			uiitem.image1.SetNativeSize();
		}
	}

	public void _WriteNote(UINote n, ElementContainer owner, Action<UINote> onWriteNote, bool isRef, bool addHeader = true)
	{
		if (addHeader)
		{
			if (isRef)
			{
				UIText.globalSizeMod = -2;
				n.AddHeader("prevElement".lang(this.FullName, null, null, null, null), null);
			}
			else
			{
				n.AddHeader(this.FullName.ToTitleCase(true), null);
			}
		}
		string detail = this.GetDetail();
		if (!detail.IsEmpty())
		{
			n.AddText("NoteText_flavor_element", detail, FontColor.DontChange);
			n.Space(6, 1);
		}
		int num = this.vLink;
		bool flag = this.ShowValue;
		bool flag2 = this.ShowRelativeAttribute;
		if (this.source.category == "landfeat")
		{
			flag = false;
			flag2 = false;
		}
		if (this is Act)
		{
			Act act = ACT.Create(this.source.id);
			UIItem uiitem = n.AddItem("ItemAbility");
			uiitem.text1.text = "vValue".lang(this.DisplayValue.ToString() ?? "", this.ValueWithoutLink.ToString() + ((num == 0) ? "" : ((num > 0) ? (" + " + num.ToString()) : (" - " + (-num).ToString()))), null, null, null);
			uiitem.text2.text = act.TargetType.ToString().lang();
			uiitem.text3.text = (((this is Spell) ? (owner.Chara.CalcCastingChance(owner.GetOrCreateElement(act.source.id), 1).ToString() + "%") : "-") ?? "");
		}
		else if (flag)
		{
			n.AddTopic("TopicLeft", "vCurrent".lang(), "vValue".lang(this.DisplayValue.ToString() ?? "", this.ValueWithoutLink.ToString() + ((num == 0) ? "" : ((num > 0) ? (" + " + num.ToString()) : (" - " + (-num).ToString()))), null, null, null));
			if (this.ShowPotential)
			{
				num = this.vTempPotential;
				n.AddTopic("TopicLeft", "vPotential".lang(), "vValue".lang(this.Potential.ToString() ?? "", (this.vPotential + this.vSourcePotential + this.MinPotential).ToString() + ((num == 0) ? "" : ((num > 0) ? (" + " + num.ToString()) : (" - " + (-num).ToString()))), null, null, null));
			}
			bool potentialAsStock = this.PotentialAsStock;
		}
		if (flag2 && !this.source.aliasParent.IsEmpty())
		{
			Element element = Element.Create(this.source.aliasParent, 1);
			UIItem uiitem2 = n.AddItem("ItemRelativeAttribute");
			uiitem2.text1.SetText(element.Name);
			element.SetImage(uiitem2.image1);
			bool flag3 = this.source.lvFactor > 0 && this is Act;
			uiitem2.text2.SetActive(flag3);
			uiitem2.text3.SetActive(flag3);
			if (flag3)
			{
				uiitem2.text2.SetText(this.GetPower(EClass.pc).ToString() ?? "");
			}
		}
		this.OnWriteNote(n, owner);
		if (EClass.debug.showExtra)
		{
			string text = "TopicLeft";
			string text2 = "Class:";
			Type type = base.GetType();
			n.AddTopic(text, text2, ((type != null) ? type.ToString() : null) ?? "");
			n.AddTopic("TopicLeft", "vExp".lang(), this.vExp.ToString() ?? "");
			n.AddTopic("TopicLeft", "vSource", this.vSource.ToString() ?? "");
			n.AddTopic("TopicLeft", "vSourcePotential", this.vSourcePotential.ToString() ?? "");
			n.AddTopic("TopicLeft", "vPotential", this.vPotential.ToString() ?? "");
			n.AddTopic("TopicLeft", "Potential", this.Potential.ToString() ?? "");
		}
		this.CheckLevelBonus(owner, n);
		if (onWriteNote != null)
		{
			onWriteNote(n);
		}
		UIText.globalSizeMod = 0;
	}

	public virtual void OnWriteNote(UINote n, ElementContainer owner)
	{
	}

	public virtual void OnChangeValue()
	{
	}

	public void CheckLevelBonus(ElementContainer owner, UINote n = null)
	{
		if (owner == null || this.source.levelBonus.IsEmpty())
		{
			return;
		}
		bool flag = n == null;
		string[] array = (this.source.GetText("levelBonus", true) ?? this.source.levelBonus).Split(Environment.NewLine.ToCharArray());
		if (!flag)
		{
			n.Space(10, 1);
		}
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			string text = array2[i];
			string[] array3 = text.Split(',', StringSplitOptions.None);
			int lv = array3[0].ToInt();
			SourceElement.Row row = EClass.sources.elements.alias.ContainsKey(array3[1]) ? EClass.sources.elements.alias[array3[1]] : null;
			if (flag)
			{
				if (lv > this.ValueWithoutLink)
				{
					break;
				}
				if (row != null && !owner.Has(row.id) && owner is ElementContainerZone)
				{
					owner.Learn(row.id, 1);
				}
			}
			else
			{
				string s = (row != null) ? row.GetName() : array3[1];
				n.AddText(("  Lv " + lv.ToString()).TagColor(FontColor.Topic, null) + "  " + s.TagColorGoodBad(delegate()
				{
					if (row != null)
					{
						return owner.Has(row.id);
					}
					return lv <= this.ValueWithoutLink;
				}, () => false, false), FontColor.DontChange);
			}
		}
		if (!flag)
		{
			n.Space(4, 1);
		}
	}

	public int GetSortVal(UIList.SortMode m)
	{
		if (m == UIList.SortMode.ByNumber)
		{
			return -this.vPotential * 10000 + this.source.id;
		}
		if (m == UIList.SortMode.ByCategory)
		{
			return this.source.id + (this.source.aliasParent.IsEmpty() ? 0 : (EClass.sources.elements.alias[this.source.aliasParent].id * 10000));
		}
		if (m != UIList.SortMode.ByElementParent)
		{
			return this.source.id;
		}
		if (!this.source.aliasParent.IsEmpty())
		{
			return EClass.sources.elements.alias[this.source.aliasParent].sort;
		}
		return this.id;
	}

	public virtual Act.Cost GetCost(Chara c)
	{
		if (this.source.cost[0] == 0)
		{
			return new Act.Cost
			{
				type = Act.CostType.None
			};
		}
		Act.Cost cost = default(Act.Cost);
		if (this is Spell)
		{
			cost.type = Act.CostType.MP;
			int num = EClass.curve(this.Value, 50, 10, 75);
			cost.cost = this.source.cost[0] * (100 + (this.source.tag.Contains("noCostInc") ? 0 : (num * 3))) / 100;
		}
		else
		{
			cost.type = Act.CostType.SP;
			cost.cost = this.source.cost[0];
			if (this.source.id == 6020)
			{
				cost.cost = c.stamina.max / 3 + 10;
			}
		}
		if (!c.IsPC && cost.cost > 2)
		{
			cost.cost /= 2;
		}
		return cost;
	}

	public virtual int GetPower(Card c)
	{
		return 100;
	}

	public virtual void SetTextValue(UIText text)
	{
		string text2 = this.DisplayValue.ToString() ?? "";
		if (this.ShowXP)
		{
			text2 += ".".TagSize((this.vExp / 10).ToString("D2") ?? "", 11);
		}
		if (this.vLink != 0)
		{
			string str = ((this.vLink > 0) ? "+" : "") + this.vLink.ToString();
			text2 = string.Concat(new string[]
			{
				"<color=",
				((this.DisplayValue > this.ValueWithoutLink) ? SkinManager.CurrentColors.textGood : SkinManager.CurrentColors.textBad).ToHex(),
				">",
				text2,
				(" (" + str + ")").TagSize(13),
				"</color>"
			});
		}
		text.text = text2;
	}

	public static Element Create(int id, int v = 0)
	{
		SourceElement.Row row = EClass.sources.elements.map.TryGetValue(id, null);
		if (row == null)
		{
			return null;
		}
		Element element = ClassCache.Create<Element>(row.type.IsEmpty("Element"), "Elin");
		element.id = id;
		element.vBase = v;
		element._source = row;
		return element;
	}

	public static Element Create(string id, int v = 1)
	{
		return Element.Create(EClass.sources.elements.alias[id].id, v);
	}

	public static int GetId(string alias)
	{
		return EClass.sources.elements.alias[alias].id;
	}

	public static int GetResistLv(int v)
	{
		int num = v / 5;
		if (num < -2)
		{
			num = -2;
		}
		return num;
	}

	public static int GetResistDamage(int dmg, int v)
	{
		int resistLv = Element.GetResistLv(v);
		if (resistLv >= 4)
		{
			return 0;
		}
		switch (resistLv)
		{
		case -2:
			return dmg * 2;
		case -1:
			return dmg * 3 / 2;
		case 1:
			return dmg / 2;
		case 2:
			return dmg / 3;
		case 3:
			return dmg / 4;
		}
		return dmg;
	}

	[CompilerGenerated]
	internal static string <_WriteNote>g__Calc|101_0(ref Element.<>c__DisplayClass101_0 A_0)
	{
		Dice dice = Dice.Create(A_0.e, A_0.c);
		if (dice == null)
		{
			return A_0.p.ToString() ?? "";
		}
		return dice.ToString();
	}

	public const int Div = 5;

	public static Element Void = new Element();

	public static int[] List_MainAttributes = new int[]
	{
		70,
		72,
		71,
		77,
		74,
		75,
		76,
		73,
		79
	};

	public static int[] List_MainAttributesMajor = new int[]
	{
		70,
		72,
		71,
		77,
		74,
		75,
		76,
		73
	};

	public static int[] List_Body = new int[]
	{
		70,
		72,
		71,
		77
	};

	public static int[] List_Mind = new int[]
	{
		74,
		75,
		76,
		73
	};

	public SourceElement.Row _source;

	public int id;

	public int vBase;

	public int vExp;

	public int vPotential;

	public int vTempPotential;

	public int vLink;

	public int vSource;

	public int vSourcePotential;

	public ElementContainer owner;

	public static List<SourceElement.Row> ListElements = new List<SourceElement.Row>();

	public static List<SourceElement.Row> ListAttackElements = new List<SourceElement.Row>();
}
