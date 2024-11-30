using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class ElementContainer : EClass
{
	public virtual Card Card
	{
		get
		{
			return null;
		}
	}

	public virtual Chara Chara
	{
		get
		{
			return null;
		}
	}

	public virtual bool IsMeleeWeapon
	{
		get
		{
			return false;
		}
	}

	[OnSerializing]
	private void OnSerializing(StreamingContext context)
	{
		this.list = new List<int>();
		foreach (Element element in this.dict.Values)
		{
			if (element.vBase != 0 || element.vExp != 0 || element.vPotential != 0 || element.vTempPotential != 0)
			{
				this.list.AddRange(new int[]
				{
					element.id,
					element.vBase,
					element.vExp,
					element.vPotential,
					element.vTempPotential
				});
			}
		}
		if (this.list.Count == 0)
		{
			this.list = null;
		}
	}

	[OnDeserialized]
	private void OnDeserialized(StreamingContext context)
	{
		if (this.list != null)
		{
			for (int i = 0; i < this.list.Count; i += 5)
			{
				Element orCreateElement = this.GetOrCreateElement(this.list[i]);
				if (orCreateElement != null)
				{
					orCreateElement.vBase += this.list[i + 1];
					orCreateElement.vExp += this.list[i + 2];
					orCreateElement.vPotential += this.list[i + 3];
					orCreateElement.vTempPotential = this.list[i + 4];
					orCreateElement.owner = this;
				}
			}
		}
	}

	public void ApplyElementMap(int uid, SourceValueType type, Dictionary<int, int> map, int lv, bool invert = false, bool applyFeat = false)
	{
		int num = invert ? -1 : 1;
		Rand.SetSeed(uid);
		foreach (KeyValuePair<int, int> keyValuePair in map)
		{
			Element orCreateElement = this.GetOrCreateElement(keyValuePair.Key);
			int value = keyValuePair.Value;
			if (value != 0)
			{
				if (orCreateElement.source.category == "skill")
				{
					orCreateElement.vSourcePotential += orCreateElement.GetSourcePotential(value) * num;
				}
				int num2 = orCreateElement.GetSourceValue(value, lv, type) * num;
				orCreateElement.vSource += num2;
				if (applyFeat && orCreateElement is Feat)
				{
					(orCreateElement as Feat).Apply(num2, this, false);
				}
			}
		}
		Rand.SetSeed(-1);
	}

	public void ApplyMaterialElementMap(Thing t, bool invert = false)
	{
		int num = invert ? -1 : 1;
		SourceMaterial.Row material = t.material;
		Rand.SetSeed(t.uid);
		foreach (KeyValuePair<int, int> keyValuePair in material.elementMap)
		{
			int value = keyValuePair.Value;
			if (value != 0)
			{
				Element orCreateElement = this.GetOrCreateElement(keyValuePair.Key);
				if (!orCreateElement.source.IsEncAppliable(t))
				{
					if (orCreateElement.vBase == 0 && orCreateElement.vSource == 0 && orCreateElement.vLink == 0 && orCreateElement.vExp == 0 && orCreateElement.vPotential == 0)
					{
						this.Remove(orCreateElement.id);
					}
				}
				else
				{
					int num2 = orCreateElement.GetMaterialSourceValue(t, value) * num;
					orCreateElement.vSource += num2;
					if (orCreateElement.vBase == 0 && orCreateElement.vSource == 0 && orCreateElement.vLink == 0 && orCreateElement.vExp == 0 && orCreateElement.vPotential == 0)
					{
						this.Remove(orCreateElement.id);
					}
				}
			}
		}
		Rand.SetSeed(-1);
	}

	public void ImportElementMap(Dictionary<int, int> map)
	{
		foreach (KeyValuePair<int, int> keyValuePair in map)
		{
			this.GetOrCreateElement(keyValuePair.Key).vSource += keyValuePair.Value;
		}
	}

	public ElementContainer ImportElementMap(int[] ints)
	{
		for (int i = 0; i < ints.Length; i += 2)
		{
			this.GetOrCreateElement(ints[i]).vSource += ints[i + 1];
		}
		return this;
	}

	public void ApplyPotential(int mode = 0)
	{
		foreach (Element element in this.dict.Values)
		{
			if (element.HasTag("primary"))
			{
				element.vTempPotential = (element.ValueWithoutLink - ((mode == 2) ? 0 : 7)) * 5;
			}
		}
	}

	public int Value(int ele)
	{
		Element element = this.GetElement(ele);
		if (element != null)
		{
			return element.Value;
		}
		if (EClass.core.game == null)
		{
			return 0;
		}
		if (this.Card == null || !this.Card.IsPCFactionOrMinion)
		{
			return 0;
		}
		if (ele != 78)
		{
			return EClass.pc.faction.charaElements.Value(ele);
		}
		return this.GetOrCreateElement(ele).Value;
	}

	public virtual int ValueBonus(Element e)
	{
		return 0;
	}

	public int ValueWithoutLink(int ele)
	{
		Element element = this.GetElement(ele);
		if (element == null)
		{
			return 0;
		}
		return element.ValueWithoutLink;
	}

	public int ValueWithoutLink(string alias)
	{
		Element element = this.GetElement(alias);
		if (element == null)
		{
			return 0;
		}
		return element.ValueWithoutLink;
	}

	public int GetFeatRef(int ele, int idx = 0)
	{
		Feat feat = this.GetElement(ele) as Feat;
		if (feat == null)
		{
			return 0;
		}
		feat.Apply(feat.Value, this, false);
		return Feat.featRef[idx].ToInt();
	}

	public int Exp(int ele)
	{
		Element element = this.GetElement(ele);
		if (element == null)
		{
			return 0;
		}
		return element.vExp;
	}

	public bool Has(int ele)
	{
		Element element = this.GetElement(ele);
		return element != null && element.Value > 0;
	}

	public bool Has(SourceElement.Row row)
	{
		return this.Has(row.id);
	}

	public bool Has(string alias)
	{
		return this.Has(EClass.sources.elements.alias[alias].id);
	}

	public bool HasBase(int ele)
	{
		Element element = this.GetElement(ele);
		if (element == null)
		{
			return false;
		}
		int num = element.ValueWithoutLink;
		if (ele != 300)
		{
			if (ele == 307)
			{
				num += this.Value(1524) * -4;
				num += this.Value(1525) * 4;
			}
		}
		else
		{
			num += this.Value(1516) * -4;
			num += this.Value(1517) * 4;
		}
		return num != 0;
	}

	public int Base(int ele)
	{
		Element element = this.GetElement(ele);
		if (element == null)
		{
			return 0;
		}
		return element.ValueWithoutLink;
	}

	public void Learn(int ele, int v = 1)
	{
		this.ModBase(ele, v);
		this.OnLearn(ele);
	}

	public void Train(int ele, int a = 10)
	{
		this.OnTrain(ele);
		this.ModTempPotential(ele, a, 0);
	}

	public void ModExp(int ele, int a, bool chain = false)
	{
		if (this.Card != null && this.Card.isChara && this.Card.Chara.isDead)
		{
			return;
		}
		if (a == 0)
		{
			return;
		}
		Element element = this.GetElement(ele);
		if (element == null || !element.CanGainExp)
		{
			return;
		}
		int value = element.UsePotential ? element.Potential : 100;
		if (element.UseExpMod)
		{
			a = a * Mathf.Clamp(value, 10, 1000) / (100 + Mathf.Max(0, element.ValueWithoutLink) * 25);
			if (a >= 0 && EClass.rnd(element.ValueWithoutLink + 1) < 10)
			{
				a++;
			}
		}
		element.vExp += a;
		if (!chain && element.source.parentFactor > 0f && this.Card != null && !element.source.aliasParent.IsEmpty())
		{
			Element element2 = element.GetParent(this.Card);
			if (element2.CanGainExp)
			{
				this.ModExp(element2.id, (int)Math.Max(1f, (float)a * element.source.parentFactor / 100f), true);
			}
		}
		if (element.vExp >= element.ExpToNext)
		{
			int num = element.vExp - element.ExpToNext;
			int vBase = element.vBase;
			this.ModBase(ele, 1);
			this.OnLevelUp(element, vBase);
			element.vExp = Mathf.Clamp(num / 2, 0, element.ExpToNext / 2);
			if (element.vTempPotential > 0)
			{
				element.vTempPotential -= element.vTempPotential / 4 + EClass.rnd(5) + 5;
				if (element.vTempPotential < 0)
				{
					element.vTempPotential = 0;
					return;
				}
			}
			else if (element.vTempPotential < 0)
			{
				element.vTempPotential += -element.vTempPotential / 4 + EClass.rnd(5) + 5;
				if (element.vTempPotential > 0)
				{
					element.vTempPotential = 0;
					return;
				}
			}
		}
		else if (element.vExp < 0)
		{
			if (element.ValueWithoutLink <= 1)
			{
				element.vExp = 0;
				return;
			}
			int vBase2 = element.vBase;
			this.ModBase(ele, -1);
			this.OnLevelDown(element, vBase2);
			element.vExp = Mathf.Max(element.ExpToNext / 2, element.ExpToNext + element.vExp);
		}
	}

	public virtual void OnLearn(int ele)
	{
	}

	public virtual void OnTrain(int ele)
	{
	}

	public virtual void OnLevelUp(Element e, int lastValue)
	{
	}

	public virtual void OnLevelDown(Element e, int lastValue)
	{
	}

	public Element SetBase(string alias, int v, int potential = 0)
	{
		return this.SetBase(EClass.sources.elements.alias[alias].id, v, potential);
	}

	public Element SetBase(int id, int v, int potential = 0)
	{
		Element orCreateElement = this.GetOrCreateElement(id);
		if (this.parent != null && orCreateElement.CanLink(this))
		{
			this.parent.ModLink(id, -orCreateElement.vBase + v);
		}
		orCreateElement.vBase = v;
		orCreateElement.vExp = 0;
		orCreateElement.vPotential = potential;
		orCreateElement.OnChangeValue();
		if (orCreateElement.vBase == 0 && orCreateElement.vSource == 0 && orCreateElement.vLink == 0 && orCreateElement.vPotential == 0 && orCreateElement.vExp == 0)
		{
			this.Remove(orCreateElement.id);
		}
		return orCreateElement;
	}

	public void SetTo(int id, int v)
	{
		Element orCreateElement = this.GetOrCreateElement(id);
		int num = v - (orCreateElement.vBase + orCreateElement.vSource);
		if (num != 0)
		{
			this.ModBase(id, num);
		}
		if (orCreateElement.vBase == 0 && orCreateElement.vSource == 0 && orCreateElement.vLink == 0 && orCreateElement.vPotential == 0 && orCreateElement.vExp == 0)
		{
			this.Remove(orCreateElement.id);
		}
	}

	public void Remove(int id)
	{
		Element element = this.GetElement(id);
		if (element == null)
		{
			return;
		}
		if (this.parent != null && element.CanLink(this))
		{
			this.parent.ModLink(id, -element.Value);
		}
		this.dict.Remove(id);
	}

	public Element ModBase(int ele, int v)
	{
		Element orCreateElement = this.GetOrCreateElement(ele);
		orCreateElement.vBase += v;
		if (this.parent != null && orCreateElement.CanLink(this))
		{
			this.parent.ModLink(ele, v);
		}
		orCreateElement.CheckLevelBonus(this, null);
		orCreateElement.OnChangeValue();
		if (orCreateElement.vBase == 0 && orCreateElement.vSource == 0 && orCreateElement.vLink == 0 && orCreateElement.vPotential == 0 && orCreateElement.vExp == 0)
		{
			this.Remove(orCreateElement.id);
		}
		return orCreateElement;
	}

	public virtual void OnChangeValue()
	{
	}

	public Element ModPotential(int ele, int v)
	{
		Element orCreateElement = this.GetOrCreateElement(ele);
		orCreateElement.vPotential += v;
		if (orCreateElement.vPotential > 1000)
		{
			orCreateElement.vPotential = 1000;
		}
		return orCreateElement;
	}

	public Element ModTempPotential(int ele, int v, int threshMsg = 0)
	{
		Element orCreateElement = this.GetOrCreateElement(ele);
		orCreateElement.vTempPotential += v;
		if (orCreateElement.vTempPotential > 1000)
		{
			orCreateElement.vTempPotential = 1000;
		}
		this.OnModTempPotential(orCreateElement, v, threshMsg);
		return orCreateElement;
	}

	public virtual void OnModTempPotential(Element e, int v, int threshMsg)
	{
	}

	private Element ModLink(int id, int v)
	{
		Element orCreateElement = this.GetOrCreateElement(id);
		orCreateElement.vLink += v;
		orCreateElement.OnChangeValue();
		if (this.parent != null && orCreateElement.CanLink(this))
		{
			this.parent.ModLink(id, v);
		}
		return orCreateElement;
	}

	public int GetSpellExp(Chara c, Element e, int costMod = 100)
	{
		Act.Cost cost = e.GetCost(c);
		int num = cost.cost * ((cost.type == Act.CostType.SP) ? 20 : 5) * (100 + c.Evalue(1208) * 30) / 100 + 10;
		num = num * costMod / 100;
		if (!e.source.aliasParent.IsEmpty())
		{
			int num2 = this.ValueWithoutLink(e.source.aliasParent) - this.ValueWithoutLink(e.source.id);
			if (num2 >= 0)
			{
				num = num * (100 + num2 * 5) / 100;
			}
			else
			{
				num = num * 100 / (100 - num2 * 25);
			}
		}
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}

	public Element GetElement(string alias)
	{
		SourceElement.Row row = EClass.sources.elements.alias.TryGetValue(alias, null);
		return this.GetElement((row != null) ? row.id : 0);
	}

	public Element GetElement(int id)
	{
		return this.dict.TryGetValue(id, null);
	}

	public Element CreateElement(int id)
	{
		Element element = Element.Create(id, 0);
		if (element == null)
		{
			return null;
		}
		element.owner = this;
		this.dict.Add(id, element);
		return element;
	}

	public Element GetOrCreateElement(Element ele)
	{
		return this.GetOrCreateElement(ele.id);
	}

	public Element GetOrCreateElement(string alias)
	{
		return this.GetOrCreateElement(EClass.sources.elements.alias[alias].id);
	}

	public Element GetOrCreateElement(int id)
	{
		Element result = null;
		if (!this.dict.TryGetValue(id, out result))
		{
			result = this.CreateElement(id);
		}
		return result;
	}

	public void SetParent(Card c)
	{
		this.SetParent((c != null) ? c.elements : null);
	}

	public void SetParent(ElementContainer newParent = null)
	{
		if (this.parent != null)
		{
			foreach (Element element in this.dict.Values)
			{
				if (element.CanLink(this))
				{
					this.parent.ModLink(element.id, -(element.vBase + element.vSource));
				}
			}
		}
		if (newParent != null)
		{
			foreach (Element element2 in this.dict.Values)
			{
				if (element2.CanLink(this))
				{
					newParent.ModLink(element2.id, element2.vBase + element2.vSource);
				}
			}
		}
		this.parent = newParent;
	}

	public List<Element> ListElements(Func<Element, bool> shoudList = null, Comparison<Element> comparison = null)
	{
		ElementContainer.<>c__DisplayClass55_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		List<Element> list = new List<Element>();
		CS$<>8__locals1.eles = this.dict.Values.ToList<Element>();
		if (this.Card != null && this.Card.Chara != null)
		{
			if (this.Card.Chara.IsPCFaction)
			{
				this.<ListElements>g__AddElements|55_0(EClass.pc.faction.charaElements, true, ref CS$<>8__locals1);
			}
			this.<ListElements>g__AddElements|55_0(this.Card.Chara.faithElements, false, ref CS$<>8__locals1);
		}
		foreach (Element element in CS$<>8__locals1.eles)
		{
			if (shoudList == null || shoudList(element))
			{
				list.Add(element);
			}
		}
		if (comparison != null)
		{
			list.Sort(comparison);
		}
		return list;
	}

	public List<Element> ListBestAttributes()
	{
		List<Element> list = this.ListElements((Element a) => a.HasTag("primary"), null);
		list.Sort((Element a, Element b) => (b.ValueWithoutLink - a.ValueWithoutLink) * 100000 + a.id - b.id);
		return list;
	}

	public List<Element> ListBestSkills()
	{
		List<Element> list = this.ListElements((Element a) => a.source.category == "skill", null);
		list.Sort((Element a, Element b) => (b.ValueWithoutLink - a.ValueWithoutLink) * 100000 + a.id - b.id);
		return list;
	}

	public List<Element> ListGeneFeats()
	{
		return this.ListElements((Element a) => a.Value > 0 && a.source.category == "feat" && a.source.cost.Length != 0 && a.source.cost[0] > 0, null);
	}

	public List<Element> ListLearnable(Chara c)
	{
		List<Element> list = new List<Element>();
		foreach (KeyValuePair<int, Element> keyValuePair in c.elements.dict)
		{
			if (!this.dict.ContainsKey(keyValuePair.Key))
			{
				list.Add(keyValuePair.Value);
			}
		}
		return list;
	}

	public void CopyTo(ElementContainer container)
	{
		container.dict.Clear();
		foreach (KeyValuePair<int, Element> keyValuePair in this.dict)
		{
			Element element = container.CreateElement(keyValuePair.Key);
			element.vBase = keyValuePair.Value.vBase;
			element.vExp = keyValuePair.Value.vExp;
			element.vSource = keyValuePair.Value.vSource;
		}
	}

	public static int GetSortVal(Element a)
	{
		int num = a.Value;
		if (a.source.textAlt.Length <= 2 || a.Value < 0)
		{
			num -= 100000;
		}
		if (a.id == 2)
		{
			num += 20000;
		}
		if (a.IsFoodTraitMain)
		{
			num += 10000;
		}
		return num;
	}

	public void AddNote(UINote n, Func<Element, bool> isValid = null, Action onAdd = null, ElementContainer.NoteMode mode = ElementContainer.NoteMode.Default, bool addRaceFeat = false, Func<Element, string, string> funcText = null, Action<UINote, Element> onAddNote = null)
	{
		List<Element> list = new List<Element>();
		foreach (Element element in this.dict.Values)
		{
			if ((isValid == null || isValid(element)) && (mode != ElementContainer.NoteMode.CharaMake || element.ValueWithoutLink != 0) && (element.Value != 0 || mode == ElementContainer.NoteMode.CharaMakeAttributes) && (!element.HasTag("hidden") || EClass.debug.showExtra))
			{
				list.Add(element);
			}
		}
		if (addRaceFeat)
		{
			Element element2 = Element.Create(29, 1);
			element2.owner = this;
			list.Add(element2);
		}
		if (list.Count == 0)
		{
			return;
		}
		if (onAdd != null)
		{
			onAdd();
		}
		if (mode - ElementContainer.NoteMode.CharaMake > 1)
		{
			if (mode != ElementContainer.NoteMode.Trait)
			{
				list.Sort((Element a, Element b) => a.SortVal(false) - b.SortVal(false));
			}
			else
			{
				list.Sort((Element a, Element b) => ElementContainer.GetSortVal(b) - ElementContainer.GetSortVal(a));
			}
		}
		else
		{
			list.Sort((Element a, Element b) => a.GetSortVal(UIList.SortMode.ByElementParent) - b.GetSortVal(UIList.SortMode.ByElementParent));
		}
		using (List<Element>.Enumerator enumerator2 = list.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				Element e = enumerator2.Current;
				switch (mode)
				{
				case ElementContainer.NoteMode.Default:
				case ElementContainer.NoteMode.Trait:
				{
					bool flag = e.source.tag.Contains("common");
					string categorySub = e.source.categorySub;
					bool flag2 = false;
					bool flag3 = e.source.tag.Contains("neg") ? (e.Value > 0) : (e.Value < 0);
					int num = Mathf.Abs(e.Value);
					bool flag4 = this.Card != null && this.Card.ShowFoodEnc;
					bool flag5 = this.Card != null && this.Card.IsWeapon && e is Ability;
					string text;
					if (e.IsTrait || (flag4 && e.IsFoodTrait))
					{
						string[] textArray = e.source.GetTextArray("textAlt");
						int num2 = Mathf.Clamp(e.Value / 10 + 1, (e.Value >= 0 && textArray.Length > 2) ? 2 : 1, textArray.Length - 1);
						text = "altEnc".lang(textArray[0].IsEmpty(e.Name), textArray[num2], EClass.debug.showExtra ? (e.Value.ToString() + " " + e.Name) : "", null, null);
						flag3 = (num2 <= 1 || textArray.Length <= 2);
						flag2 = true;
					}
					else if (flag5)
					{
						text = "isProc".lang(e.Name, null, null, null, null);
						flag3 = false;
					}
					else if (categorySub == "resist")
					{
						text = ("isResist" + (flag3 ? "Neg" : "")).lang(e.Name, null, null, null, null);
					}
					else if (categorySub == "eleAttack")
					{
						text = "isEleAttack".lang(e.Name, null, null, null, null);
					}
					else if (!e.source.textPhase.IsEmpty() && e.Value > 0)
					{
						text = e.source.GetText("textPhase", false);
					}
					else
					{
						string name = e.Name;
						bool flag6 = e.source.category == "skill" || (e.source.category == "attribute" && !e.source.textPhase.IsEmpty());
						bool flag7 = e.source.category == "enchant";
						if (e.source.tag.Contains("multiplier"))
						{
							flag7 = (flag6 = false);
							name = EClass.sources.elements.alias[e.source.aliasRef].GetName();
						}
						flag2 = (!flag6 && !flag7);
						text = (flag6 ? "textEncSkill" : (flag7 ? "textEncEnc" : "textEnc")).lang(name, num.ToString() + (e.source.tag.Contains("ratio") ? "%" : ""), ((e.Value > 0) ? "encIncrease" : "encDecrease").lang(), null, null);
					}
					int num3 = (e is Resistance) ? 0 : 1;
					if (!flag && !flag2 && !e.source.tag.Contains("flag"))
					{
						text = string.Concat(new string[]
						{
							text,
							" [",
							"*".Repeat(Mathf.Clamp(num * e.source.mtp / 5 + num3, 1, 5)),
							(num * e.source.mtp / 5 + num3 > 5) ? "+" : "",
							"]"
						});
					}
					if (e.HasTag("hidden"))
					{
						text = "(debug)" + text;
					}
					FontColor color = flag ? FontColor.Default : (flag3 ? FontColor.Bad : FontColor.Good);
					if (e.IsGlobalElement)
					{
						text = text + " " + (e.IsFactionWideElement ? "_factionWide" : "_partyWide").lang();
						if (this.Card != null && !this.Card.c_idDeity.IsEmpty() && this.Card.c_idDeity != EClass.pc.idFaith)
						{
							continue;
						}
						color = FontColor.Myth;
					}
					if (flag4 && e.IsFoodTrait && !e.IsFoodTraitMain)
					{
						color = FontColor.FoodMisc;
					}
					if (e.id == 2 && e.Value >= 0)
					{
						color = FontColor.FoodQuality;
					}
					if (funcText != null)
					{
						text = funcText(e, text);
					}
					n.AddText("NoteText_prefwidth", text, color);
					if (onAddNote != null)
					{
						onAddNote(n, e);
						continue;
					}
					continue;
				}
				case ElementContainer.NoteMode.Domain:
					n.AddText(e.Name, FontColor.Default);
					continue;
				}
				UIItem uiitem = n.AddTopic("TopicAttribute", e.Name, "".TagColor((e.ValueWithoutLink > 0) ? SkinManager.CurrentColors.textGood : SkinManager.CurrentColors.textBad, e.ValueWithoutLink.ToString() ?? ""));
				if (uiitem.button1)
				{
					uiitem.button1.tooltip.onShowTooltip = delegate(UITooltip t)
					{
						e.WriteNote(t.note, EClass.pc.elements, null);
					};
				}
				e.SetImage(uiitem.image1);
				Image image = uiitem.image2;
				int value = (e.Potential - 80) / 20;
				image.enabled = (e.Potential != 80);
				image.sprite = EClass.core.refs.spritesPotential[Mathf.Clamp(Mathf.Abs(value), 0, EClass.core.refs.spritesPotential.Count - 1)];
				image.color = ((e.Potential - 80 >= 0) ? Color.white : new Color(1f, 0.7f, 0.7f));
			}
		}
	}

	public void AddNoteAll(UINote n)
	{
		Transform transform = n.AddExtra<Transform>("noteRace");
		UINote n2 = transform.Find("note1").GetComponent<UINote>();
		UINote n3 = transform.Find("note2").GetComponent<UINote>();
		this.AddNote(n3, (Element e) => e.HasTag("primary"), delegate
		{
			n3.AddHeader("HeaderNoteSmall", "attributes", null);
		}, ElementContainer.NoteMode.CharaMakeAttributes, false, null, null);
		this.AddNote(n2, (Element e) => e.source.category == "skill" && !e.HasTag("hidden") && e.ValueWithoutLink > 1 && e.source.categorySub != "weapon", delegate
		{
			n2.AddHeader("HeaderNoteSmall", "skills", null);
		}, ElementContainer.NoteMode.CharaMake, false, null, null);
		this.AddNote(n2, (Element e) => e is Feat, delegate
		{
			n2.AddHeader("HeaderNoteSmall", "feats", null);
		}, ElementContainer.NoteMode.CharaMake, false, null, null);
	}

	[CompilerGenerated]
	private void <ListElements>g__AddElements|55_0(ElementContainer container, bool isGlobal, ref ElementContainer.<>c__DisplayClass55_0 A_3)
	{
		if (container == null)
		{
			return;
		}
		foreach (Element element in container.dict.Values)
		{
			bool flag = true;
			foreach (Element element2 in A_3.eles)
			{
				if (element.id == element2.id)
				{
					flag = false;
					break;
				}
			}
			if (flag && element.Value != 0)
			{
				if (isGlobal)
				{
					Element item = this.Card.Chara.elements.CreateElement(element.id);
					A_3.eles.Add(item);
				}
				else
				{
					A_3.eles.Add(element);
				}
			}
		}
	}

	public Dictionary<int, Element> dict = new Dictionary<int, Element>();

	public ElementContainer parent;

	public const int sizeElement = 5;

	[JsonProperty(PropertyName = "A")]
	public List<int> list;

	public enum NoteMode
	{
		Default,
		CharaMake,
		CharaMakeAttributes,
		Domain,
		Trait
	}
}
