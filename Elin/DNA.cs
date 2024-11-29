using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using UnityEngine;

public class DNA : EClass
{
	public DNA.Type type
	{
		get
		{
			return this.ints[0].ToEnum<DNA.Type>();
		}
		set
		{
			this.ints[0] = (int)value;
		}
	}

	public int cost
	{
		get
		{
			return this.ints[1];
		}
		set
		{
			this.ints[1] = value;
		}
	}

	public int lv
	{
		get
		{
			return this.ints[2];
		}
		set
		{
			this.ints[2] = value;
		}
	}

	public int seed
	{
		get
		{
			return this.ints[3];
		}
		set
		{
			this.ints[3] = value;
		}
	}

	public static Thing GenerateGene(Chara c, DNA.Type? type = null)
	{
		DNA.Type? type2 = type;
		DNA.Type type3 = DNA.Type.Brain;
		Thing thing = ThingGen.Create((type2.GetValueOrDefault() == type3 & type2 != null) ? "gene_brain" : "gene", -1, -1);
		DNA dna = new DNA();
		thing.MakeRefFrom(c, null);
		dna.id = c.id;
		dna.lv = c.LV;
		dna.seed = EClass.rnd(20000);
		thing.c_DNA = dna;
		dna.GenerateWithGene(type ?? dna.GetRandomType(), thing, c);
		return thing;
	}

	public static Thing GenerateGene(CardRow r, DNA.Type? type = null, int lv = -1, int seed = -1)
	{
		DNA.Type? type2 = type;
		DNA.Type type3 = DNA.Type.Brain;
		Thing thing = ThingGen.Create((type2.GetValueOrDefault() == type3 & type2 != null) ? "gene_brain" : "gene", -1, -1);
		DNA dna = new DNA();
		thing.MakeRefFrom(r.id);
		dna.id = r.id;
		dna.lv = ((lv == -1) ? r.LV : lv);
		dna.seed = ((seed == -1) ? EClass.rnd(20000) : seed);
		thing.c_DNA = dna;
		dna.GenerateWithGene(type ?? dna.GetRandomType(), thing, null);
		return thing;
	}

	public void Apply(Chara c)
	{
		if (c.c_genes == null)
		{
			c.c_genes = new CharaGenes();
		}
		CharaGenes c_genes = c.c_genes;
		if (this.type == DNA.Type.Inferior)
		{
			c.c_genes.inferior++;
			return;
		}
		c.feat -= this.cost;
		this.Apply(c, false);
		c_genes.items.Add(this);
	}

	public void Apply(Chara c, bool reverse)
	{
		if (this.type == DNA.Type.Brain)
		{
			c._tactics = null;
			return;
		}
		for (int i = 0; i < this.vals.Count; i += 2)
		{
			int num = this.vals[i];
			int num2 = this.vals[i + 1];
			SourceElement.Row row = EClass.sources.elements.map[num];
			string category = row.category;
			if (!(category == "slot"))
			{
				if (!(category == "feat"))
				{
					if (!(category == "ability"))
					{
						if (!reverse && c.elements.ValueWithoutLink(row.id) == 0)
						{
							c.elements.Learn(row.id, num2);
						}
						else
						{
							c.elements.ModBase(num, reverse ? (-num2) : num2);
						}
					}
					else if (reverse)
					{
						c.ability.Remove(num * ((num2 < 0) ? -1 : 1));
					}
					else
					{
						c.ability.Add(num, Mathf.Abs(num2), num2 < 0);
					}
				}
				else
				{
					c.SetFeat(num, c.elements.ValueWithoutLink(num) + (reverse ? -1 : 1), !reverse);
				}
			}
			else
			{
				if (reverse)
				{
					c.body.RemoveBodyPart(row.id);
				}
				else
				{
					c.body.AddBodyPart(row.id, null);
				}
				c.body.RefreshBodyParts();
			}
		}
	}

	public bool CanRemove()
	{
		for (int i = 0; i < this.vals.Count; i += 2)
		{
			if (this.vals[i] == 1415)
			{
				return false;
			}
		}
		return true;
	}

	public void GenerateWithGene(DNA.Type _type, Card gene, Chara model = null)
	{
		this.Generate(_type, model);
		gene.ChangeMaterial(this.GetMaterialId(this.type));
		gene.elements.SetTo(10, 0);
	}

	public void Generate(DNA.Type _type, Chara model = null)
	{
		DNA.<>c__DisplayClass22_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.model = model;
		this.type = _type;
		this.cost = 0;
		this.vals.Clear();
		Debug.Log(this.seed);
		Rand.SetSeed(this.seed);
		if (CS$<>8__locals1.model == null)
		{
			CS$<>8__locals1.model = CharaGen.Create(this.id, this.lv);
		}
		if (this.type == DNA.Type.Inferior || CS$<>8__locals1.model == null)
		{
			this.type = DNA.Type.Inferior;
			return;
		}
		if (this.type == DNA.Type.Brain)
		{
			this.cost = 4;
			return;
		}
		CS$<>8__locals1.body = 0;
		CS$<>8__locals1.action = 0;
		CS$<>8__locals1.feat = 0;
		CS$<>8__locals1.listAttb = CS$<>8__locals1.model.elements.ListBestAttributes();
		CS$<>8__locals1.listSkill = CS$<>8__locals1.model.elements.ListBestSkills();
		CS$<>8__locals1.listFeat = CS$<>8__locals1.model.elements.ListGeneFeats();
		Rand.SetSeed(this.seed);
		if (CS$<>8__locals1.listFeat.Count == 0)
		{
			CS$<>8__locals1.listFeat = CS$<>8__locals1.model.ListAvailabeFeats(true);
		}
		Rand.SetSeed(this.seed);
		DNA.Type type = this.type;
		if (type != DNA.Type.Default)
		{
			if (type == DNA.Type.Superior)
			{
				this.<Generate>g__AddRandom|22_6(EClass.rnd(EClass.rnd(4)) + 3, ref CS$<>8__locals1);
				if (EClass.rnd(3) == 0)
				{
					this.<Generate>g__AddSpecial|22_7(ref CS$<>8__locals1);
				}
				this.<Generate>g__AddSpecial|22_7(ref CS$<>8__locals1);
			}
		}
		else
		{
			this.<Generate>g__AddRandom|22_6(EClass.rnd(EClass.rnd(4)) + 1, ref CS$<>8__locals1);
			if (EClass.rnd(3) == 0 || this.vals.Count == 0)
			{
				this.<Generate>g__AddSpecial|22_7(ref CS$<>8__locals1);
			}
		}
		if (this.vals.Count == 0)
		{
			for (int i = 0; i < 10; i++)
			{
				if (EClass.rnd(4) == 0)
				{
					this.<Generate>g__AddSpecial|22_7(ref CS$<>8__locals1);
				}
				else
				{
					this.<Generate>g__AddRandom|22_6(1, ref CS$<>8__locals1);
				}
				if (this.vals.Count > 0)
				{
					break;
				}
			}
		}
		Rand.SetSeed(-1);
		this.CalcCost();
	}

	public void CalcCost()
	{
		for (int i = 0; i < this.vals.Count; i += 2)
		{
			Element.Create(this.vals[i], this.vals[i + 1]);
		}
	}

	public static DNA.Type GetType(string idMat)
	{
		if (idMat == "jelly")
		{
			return DNA.Type.Default;
		}
		if (idMat == "gold")
		{
			return DNA.Type.Superior;
		}
		if (!(idMat == "amethyst"))
		{
			return DNA.Type.Inferior;
		}
		return DNA.Type.Brain;
	}

	public string GetMaterialId(DNA.Type type)
	{
		if (type == DNA.Type.Default)
		{
			return "jelly";
		}
		if (type == DNA.Type.Superior)
		{
			return "gold";
		}
		if (type != DNA.Type.Brain)
		{
			return "process";
		}
		return "amethyst";
	}

	public int GetDurationHour()
	{
		return this.cost * this.cost / 2;
	}

	public DNA.Type GetRandomType()
	{
		if (EClass.rnd(5) == 0)
		{
			return DNA.Type.Superior;
		}
		if (EClass.rnd(10) == 0)
		{
			return DNA.Type.Inferior;
		}
		return DNA.Type.Default;
	}

	public string GetText()
	{
		string s = "gene";
		CardRow cardRow = EClass.sources.cards.map.TryGetValue(this.id, null);
		return s.lang((((cardRow != null) ? cardRow.GetName() : null) ?? "???").ToTitleCase(false), this.cost.ToString() ?? "", null, null, null);
	}

	public void WriteNote(UINote n)
	{
		if (!this.CanRemove())
		{
			n.AddText("isPermaGene".lang(), FontColor.Warning);
		}
		if (this.type == DNA.Type.Brain)
		{
			SourceChara.Row row = EClass.sources.charas.map.TryGetValue(this.id, null);
			if (row != null)
			{
				string tactics = row.tactics;
				SourceTactics.Row row2 = EClass.sources.tactics.map.TryGetValue(row.id, null);
				string defaultStr;
				if ((defaultStr = ((row2 != null) ? row2.id : null)) == null)
				{
					SourceTactics.Row row3 = EClass.sources.tactics.map.TryGetValue(row.job, null);
					defaultStr = (((row3 != null) ? row3.id : null) ?? "predator");
				}
				string key = tactics.IsEmpty(defaultStr);
				n.AddText("gene_info".lang(EClass.sources.tactics.map[key].GetName().ToTitleCase(false), "", null, null, null), FontColor.ButtonGeneral);
			}
			for (int i = 0; i < this.vals.Count; i += 2)
			{
				int num = this.vals[i];
				int num2 = this.vals[i + 1];
				FontColor color = (num2 >= 0) ? FontColor.Good : FontColor.Bad;
				string @ref = (num + 1).ToString() ?? "";
				string text = "";
				num2 = Mathf.Abs(num2 / 20) + 1;
				text = string.Concat(new string[]
				{
					text,
					"[",
					"*".Repeat(Mathf.Clamp(num2, 1, 5)),
					(num2 > 5) ? "+" : "",
					"]"
				});
				n.AddText("gene_info_brain".lang(@ref, text, null, null, null), color);
			}
			return;
		}
		for (int j = 0; j < this.vals.Count; j += 2)
		{
			Element element = Element.Create(this.vals[j], this.vals[j + 1]);
			string text2 = "";
			int num3 = element.Value / 10;
			FontColor color2 = FontColor.Good;
			string category = element.source.category;
			if (!(category == "slot"))
			{
				if (!(category == "feat"))
				{
					if (category == "ability")
					{
						color2 = FontColor.Topic2;
						num3 = -1;
					}
				}
				else
				{
					color2 = FontColor.FoodMisc;
					num3 = -1;
				}
			}
			else
			{
				color2 = FontColor.Myth;
				num3 = -1;
			}
			if (num3 >= 0)
			{
				text2 = string.Concat(new string[]
				{
					text2,
					"[",
					"*".Repeat(Mathf.Clamp(num3, 1, 5)),
					(num3 > 5) ? "+" : "",
					"]"
				});
			}
			if (EClass.debug.showExtra)
			{
				text2 = text2 + " " + element.Value.ToString();
			}
			n.AddText("gene_info".lang(element.Name.ToTitleCase(true), text2, null, null, null), color2);
		}
	}

	public Element GetInvalidFeat(Chara c)
	{
		for (int i = 0; i < this.vals.Count; i += 2)
		{
			Element element = Element.Create(this.vals[i], this.vals[i + 1]);
			if (element.source.category == "feat" && c.Evalue(element.id) >= element.source.max)
			{
				return element;
			}
		}
		return null;
	}

	public Element GetInvalidAction(Chara c)
	{
		for (int i = 0; i < this.vals.Count; i += 2)
		{
			Element element = Element.Create(this.vals[i], this.vals[i + 1]);
			if (element.source.category == "ability")
			{
				using (List<ActList.Item>.Enumerator enumerator = c.ability.list.items.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.act.source.id == element.id)
						{
							return element;
						}
					}
				}
			}
		}
		return null;
	}

	[CompilerGenerated]
	private void <Generate>g__AddVal|22_0(int id, int v, bool allowStack, Func<int, int> funcCost, ref DNA.<>c__DisplayClass22_0 A_5)
	{
		bool flag = false;
		int num = EClass.curve(v, 20, 10, 90);
		v = EClass.curve(v, 20, 10, 80);
		int i = 0;
		while (i < this.vals.Count)
		{
			if (this.vals[i] == id)
			{
				if (!allowStack)
				{
					return;
				}
				v /= 2;
				num /= 2;
				List<int> list = this.vals;
				int index = i + 1;
				list[index] += v;
				Debug.Log(string.Concat(new string[]
				{
					this.vals[i + 1].ToString(),
					": ",
					v.ToString(),
					"/",
					num.ToString()
				}));
				flag = true;
				break;
			}
			else
			{
				i += 2;
			}
		}
		if (v == 0)
		{
			return;
		}
		if (!flag)
		{
			this.vals.Add(id);
			this.vals.Add(v);
		}
		this.cost += funcCost(num);
	}

	[CompilerGenerated]
	private void <Generate>g__AddSkill|22_1(ref DNA.<>c__DisplayClass22_0 A_1)
	{
		Element element = A_1.listSkill[Mathf.Clamp(EClass.rnd(6), 0, A_1.listSkill.Count - 1)];
		this.<Generate>g__AddVal|22_0(element.id, EClass.rndHalf(element.ValueWithoutLink / 2), true, (int v) => v / 5 + 1, ref A_1);
	}

	[CompilerGenerated]
	private void <Generate>g__AddAttribute|22_2(ref DNA.<>c__DisplayClass22_0 A_1)
	{
		Element element = A_1.listAttb[EClass.rnd(3)];
		this.<Generate>g__AddVal|22_0(element.id, EClass.rndHalf(element.ValueWithoutLink / 2), true, (int v) => v / 5 + 1, ref A_1);
	}

	[CompilerGenerated]
	private void <Generate>g__AddFeat|22_3(ref DNA.<>c__DisplayClass22_0 A_1)
	{
		if (A_1.listFeat.Count == 0)
		{
			return;
		}
		int feat = A_1.feat;
		A_1.feat = feat + 1;
		Element e = A_1.listFeat.RandomItem<Element>();
		this.<Generate>g__AddVal|22_0(e.id, 1, false, (int v) => e.source.cost[0] * 5, ref A_1);
	}

	[CompilerGenerated]
	private void <Generate>g__AddBody|22_4(ref DNA.<>c__DisplayClass22_0 A_1)
	{
		if (A_1.body != 0)
		{
			return;
		}
		BodySlot bodySlot = null;
		for (int i = 0; i < 100; i++)
		{
			BodySlot bodySlot2 = A_1.model.body.slots.RandomItem<BodySlot>();
			if (bodySlot2 != null && bodySlot2.elementId != 40)
			{
				bodySlot = bodySlot2;
				break;
			}
		}
		if (bodySlot == null)
		{
			return;
		}
		this.<Generate>g__AddVal|22_0(bodySlot.elementId, 1, false, (int v) => 20, ref A_1);
		int body = A_1.body;
		A_1.body = body + 1;
	}

	[CompilerGenerated]
	private void <Generate>g__AddAction|22_5(ref DNA.<>c__DisplayClass22_0 A_1)
	{
		if (A_1.model.ability.list.items.Count == 0)
		{
			return;
		}
		ActList.Item a = A_1.model.ability.list.items.RandomItem<ActList.Item>();
		if (a.act.source.category != "ability")
		{
			return;
		}
		this.<Generate>g__AddVal|22_0(a.act.source.id, a.chance * (a.pt ? -1 : 1), false, (int v) => 8 + a.act.source.cost[0] / 10 * 2, ref A_1);
		int action = A_1.action;
		A_1.action = action + 1;
	}

	[CompilerGenerated]
	private void <Generate>g__AddRandom|22_6(int n, ref DNA.<>c__DisplayClass22_0 A_2)
	{
		for (int i = 0; i < n; i++)
		{
			if (EClass.debug.enable && EClass.rnd(2) == 0)
			{
				this.<Generate>g__AddSpecial|22_7(ref A_2);
			}
			else if (EClass.rnd(2) == 0)
			{
				this.<Generate>g__AddSkill|22_1(ref A_2);
			}
			else
			{
				this.<Generate>g__AddAttribute|22_2(ref A_2);
			}
		}
	}

	[CompilerGenerated]
	private void <Generate>g__AddSpecial|22_7(ref DNA.<>c__DisplayClass22_0 A_1)
	{
		if (EClass.rnd(3) == 0)
		{
			this.<Generate>g__AddAction|22_5(ref A_1);
			return;
		}
		if (EClass.rnd(5) == 0)
		{
			this.<Generate>g__AddBody|22_4(ref A_1);
			return;
		}
		if (A_1.listFeat.Count > 0)
		{
			this.<Generate>g__AddFeat|22_3(ref A_1);
			return;
		}
		if (EClass.rnd(2) == 0)
		{
			this.<Generate>g__AddSkill|22_1(ref A_1);
			return;
		}
		this.<Generate>g__AddAttribute|22_2(ref A_1);
	}

	[JsonProperty]
	public string id;

	[JsonProperty]
	public int[] ints = new int[4];

	[JsonProperty]
	public List<int> vals = new List<int>();

	public enum Type
	{
		Inferior,
		Default = 3,
		Superior = 5,
		Brain = 8
	}
}
