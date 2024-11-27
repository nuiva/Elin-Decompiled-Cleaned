using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SourceMaterial : SourceDataInt<SourceMaterial.Row>
{
	public override SourceMaterial.Row CreateRow()
	{
		return new SourceMaterial.Row
		{
			id = SourceData.GetInt(0),
			alias = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			category = SourceData.GetString(4),
			tag = SourceData.GetStringArray(5),
			thing = SourceData.GetString(6),
			goods = SourceData.GetStringArray(7),
			minerals = SourceData.GetStringArray(8),
			decal = SourceData.GetInt(9),
			decay = SourceData.GetInt(10),
			grass = SourceData.GetInt(11),
			defFloor = SourceData.GetInt(12),
			defBlock = SourceData.GetInt(13),
			edge = SourceData.GetInt(14),
			ramp = SourceData.GetInt(15),
			idSound = SourceData.GetString(16),
			soundFoot = SourceData.GetString(17),
			hardness = SourceData.GetInt(18),
			groups = SourceData.GetStringArray(19),
			tier = SourceData.GetInt(20),
			chance = SourceData.GetInt(21),
			weight = SourceData.GetInt(22),
			value = SourceData.GetInt(23),
			quality = SourceData.GetInt(24),
			atk = SourceData.GetInt(25),
			dmg = SourceData.GetInt(26),
			dv = SourceData.GetInt(27),
			pv = SourceData.GetInt(28),
			dice = SourceData.GetInt(29),
			bits = SourceData.GetStringArray(30),
			elements = Core.ParseElements(SourceData.GetStr(31, false)),
			altName = SourceData.GetStringArray(32),
			altName_JP = SourceData.GetStringArray(33)
		};
	}

	public override void SetRow(SourceMaterial.Row r)
	{
		this.map[r.id] = r;
	}

	public override void OnInit()
	{
		Cell.matList = this.rows;
		SourceMaterial.tierMap.Clear();
		SourceMaterial.tierMap.Add("gem", new SourceMaterial.TierList());
		SourceMaterial.tierMap.Add("ore", new SourceMaterial.TierList());
		foreach (SourceMaterial.Row row in this.rows)
		{
			row.Init();
			row.elementMap = Element.GetElementMap(row.elements);
			if (!row.groups.IsEmpty())
			{
				foreach (string key in row.groups)
				{
					SourceMaterial.TierList tierList = SourceMaterial.tierMap.TryGetValue(key, null);
					if (tierList == null)
					{
						tierList = new SourceMaterial.TierList();
						SourceMaterial.tierMap[key] = tierList;
					}
					SourceMaterial.Tier tier = tierList.tiers[row.tier];
					tier.list.Add(row);
					tier.sum += row.chance;
				}
			}
			string category = row.category;
			if (!(category == "gem"))
			{
				if (category == "ore")
				{
					SourceMaterial.Tier tier2 = SourceMaterial.tierMap["ore"].tiers[row.tier];
					tier2.list.Add(row);
					tier2.sum += row.chance;
				}
			}
			else
			{
				SourceMaterial.Tier tier3 = SourceMaterial.tierMap["gem"].tiers[row.tier];
				tier3.list.Add(row);
				tier3.sum += row.chance;
			}
		}
	}

	public override void OnAfterImportData()
	{
		this.rows.Sort((SourceMaterial.Row a, SourceMaterial.Row b) => a.id - b.id);
	}

	public void OnImportRow(SourceMaterial.Row r)
	{
		SourceMaterial.<>c__DisplayClass8_0 CS$<>8__locals1;
		CS$<>8__locals1.list = new List<int>(r.elements);
		for (int i = 0; i < r.elements.Length; i += 2)
		{
			SourceMaterial.<OnImportRow>g__Add|8_0(r.elements[i], r.elements[i + 1], ref CS$<>8__locals1);
		}
		SourceMaterial.<OnImportRow>g__Add|8_0(13, r.hardness, ref CS$<>8__locals1);
		r.elements = CS$<>8__locals1.list.ToArray();
	}

	public override string[] ImportFields
	{
		get
		{
			return new string[]
			{
				"altName"
			};
		}
	}

	[CompilerGenerated]
	internal static void <OnImportRow>g__Add|8_0(int ele, int a, ref SourceMaterial.<>c__DisplayClass8_0 A_2)
	{
		A_2.list.Add(ele);
		A_2.list.Add(a);
	}

	public static Dictionary<string, SourceMaterial.TierList> tierMap = new Dictionary<string, SourceMaterial.TierList>();

	[Serializable]
	public class Row : SourceData.BaseRow
	{
		public override bool UseAlias
		{
			get
			{
				return true;
			}
		}

		public override string GetAlias
		{
			get
			{
				return this.alias;
			}
		}

		public override void OnImportData(SourceData data)
		{
			base.OnImportData(data);
			this.SetTiles();
		}

		public void Init()
		{
			MatColors matColors = Core.Instance.Colors.matColors.TryGetValue(this.alias, null);
			this.matColor = matColors.main;
			this.altColor = matColors.alt;
			this.SetTiles();
		}

		public void SetTiles()
		{
		}

		public void AddBlood(Point p, int a = 1)
		{
			if (this.decal == 0)
			{
				return;
			}
			if (p.cell.IsSnowTile && this != MATERIAL.sourceSnow)
			{
				MATERIAL.sourceSnow.AddBlood(p, a);
				return;
			}
			for (int i = 0; i < a; i++)
			{
				EClass._map.AddDecal(p.x + ((EClass.rnd(2) == 0) ? 0 : (EClass.rnd(3) - 1)), p.z + ((EClass.rnd(2) == 0) ? 0 : (EClass.rnd(3) - 1)), this.decal, 1, true);
			}
		}

		public void PlayHitEffect(Point p)
		{
			Effect.Get("mine2").Play(p, 0f, null, null).SetParticleColor(this.GetColor()).Emit(2 + EClass.rnd(4));
		}

		public Color GetColor()
		{
			return Core.Instance.Colors.matColors[this.alias].main;
		}

		public string TryGetSound(string suffx, RenderRow c = null)
		{
			string soundImpact = this.GetSoundImpact(c);
			if (EClass.Sound.Exist(soundImpact + "_" + suffx))
			{
				return soundImpact + "_" + suffx;
			}
			return soundImpact;
		}

		public string GetSoundDead(RenderRow c = null)
		{
			return this.TryGetSound("dead", c);
		}

		public string GetSoundDrop(RenderRow c = null)
		{
			return this.TryGetSound("drop", c);
		}

		public string GetSoundCraft(RenderRow c = null)
		{
			if (this.category == "wood")
			{
				return "build_progress";
			}
			return this.TryGetSound("craft", c);
		}

		public string GetSoundImpact(RenderRow c = null)
		{
			if (c != null && !c.idSound.IsEmpty())
			{
				return "Material/" + c.idSound;
			}
			return "Material/" + this.idSound;
		}

		public void CreateByProduct(Thing container, int num)
		{
			SourceMaterial.Row.<>c__DisplayClass52_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.num = num;
			CS$<>8__locals1.container = container;
			Thing thing = ThingGen.CreateRawMaterial(this);
			thing.ModNum(CS$<>8__locals1.num, true);
			CS$<>8__locals1.container.AddThing(thing, true, -1, -1);
			this.<CreateByProduct>g__C|52_0("dye", ref CS$<>8__locals1);
			string text = this.category;
			uint num2 = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num2 <= 1167392201U)
			{
				if (num2 <= 270655681U)
				{
					if (num2 != 174734082U)
					{
						if (num2 == 270655681U)
						{
							if (text == "fiber")
							{
								this.<CreateByProduct>g__C|52_0("thread", ref CS$<>8__locals1);
								this.<CreateByProduct>g__C|52_0("texture", ref CS$<>8__locals1);
								this.<CreateByProduct>g__C|52_0("string", ref CS$<>8__locals1);
							}
						}
					}
					else if (text == "soil")
					{
						this.<CreateByProduct>g__C|52_0("glass", ref CS$<>8__locals1);
						this.<CreateByProduct>g__C|52_0("clay", ref CS$<>8__locals1);
						this.<CreateByProduct>g__C|52_0("brick", ref CS$<>8__locals1);
					}
				}
				else if (num2 != 862676408U)
				{
					if (num2 != 974867124U)
					{
						if (num2 == 1167392201U)
						{
							if (text == "crystal")
							{
								this.<CreateByProduct>g__C|52_0("glass", ref CS$<>8__locals1);
								this.<CreateByProduct>g__C|52_0("gem", ref CS$<>8__locals1);
							}
						}
					}
					else if (text == "rock")
					{
						this.<CreateByProduct>g__C|52_0("rock", ref CS$<>8__locals1);
						this.<CreateByProduct>g__C|52_0("pebble", ref CS$<>8__locals1);
						this.<CreateByProduct>g__C|52_0("cutstone", ref CS$<>8__locals1);
					}
				}
				else if (text == "skin")
				{
					this.<CreateByProduct>g__C|52_0("texture", ref CS$<>8__locals1);
				}
			}
			else if (num2 <= 1527558748U)
			{
				if (num2 != 1237752336U)
				{
					if (num2 == 1527558748U)
					{
						if (text == "gem")
						{
							this.<CreateByProduct>g__C|52_0("cutstone", ref CS$<>8__locals1);
							this.<CreateByProduct>g__C|52_0("ingot", ref CS$<>8__locals1);
							this.<CreateByProduct>g__C|52_0("gem", ref CS$<>8__locals1);
						}
					}
				}
				else if (!(text == "water"))
				{
				}
			}
			else if (num2 != 2226448744U)
			{
				if (num2 != 2585652531U)
				{
					if (num2 == 3683705231U)
					{
						if (text == "bone")
						{
							this.<CreateByProduct>g__C|52_0("glass", ref CS$<>8__locals1);
							this.<CreateByProduct>g__C|52_0("stick", ref CS$<>8__locals1);
						}
					}
				}
				else if (text == "ore")
				{
					this.<CreateByProduct>g__C|52_0("cutstone", ref CS$<>8__locals1);
					this.<CreateByProduct>g__C|52_0("ingot", ref CS$<>8__locals1);
				}
			}
			else if (text == "wood")
			{
				this.<CreateByProduct>g__C|52_0("plank", ref CS$<>8__locals1);
				this.<CreateByProduct>g__C|52_0("stick", ref CS$<>8__locals1);
				this.<CreateByProduct>g__C|52_0("bark", ref CS$<>8__locals1);
			}
			foreach (string text2 in this.goods)
			{
				this.<CreateByProduct>g__C|52_0(text2, ref CS$<>8__locals1);
			}
		}

		public bool UsePick
		{
			get
			{
				return SourceMaterial.Row.IDPick.Contains(this.category);
			}
		}

		public bool UseAxe
		{
			get
			{
				return SourceMaterial.Row.IDAxe.Contains(this.category);
			}
		}

		[CompilerGenerated]
		private void <CreateByProduct>g__C|52_0(string _id, ref SourceMaterial.Row.<>c__DisplayClass52_0 A_2)
		{
			Thing thing = ThingGen.Create(_id, -1, -1);
			thing.ChangeMaterial(this.id);
			thing.ModNum(A_2.num, true);
			A_2.container.AddThing(thing, true, -1, -1);
		}

		public int id;

		public string alias;

		public string name_JP;

		public string name;

		public string category;

		public string[] tag;

		public string thing;

		public string[] goods;

		public string[] minerals;

		public int decal;

		public int decay;

		public int grass;

		public int defFloor;

		public int defBlock;

		public int edge;

		public int ramp;

		public string idSound;

		public string soundFoot;

		public int hardness;

		public string[] groups;

		public int tier;

		public int chance;

		public int weight;

		public int value;

		public int quality;

		public int atk;

		public int dmg;

		public int dv;

		public int pv;

		public int dice;

		public string[] bits;

		public int[] elements;

		public string[] altName;

		public string[] altName_JP;

		[NonSerialized]
		public Color matColor;

		[NonSerialized]
		public Color altColor;

		public Dictionary<int, int> elementMap;

		public static string[] IDPick = new string[]
		{
			"rock",
			"ore",
			"gem",
			"crystal",
			"bone"
		};

		public static string[] IDAxe = new string[]
		{
			"wood"
		};

		public string name_L;

		public string detail_L;

		public string[] altName_L;
	}

	public class TierList
	{
		public TierList()
		{
			this.tiers = new SourceMaterial.Tier[5];
			for (int i = 0; i < 5; i++)
			{
				this.tiers[i] = new SourceMaterial.Tier();
			}
		}

		public SourceMaterial.Tier[] tiers;
	}

	public class Tier
	{
		public SourceMaterial.Row Select()
		{
			int num = 0;
			int num2 = EClass.rnd(this.sum);
			foreach (SourceMaterial.Row row in this.list)
			{
				num += row.chance;
				if (num2 < num)
				{
					return row;
				}
			}
			return this.list.RandomItem<SourceMaterial.Row>();
		}

		public int sum;

		public List<SourceMaterial.Row> list = new List<SourceMaterial.Row>();
	}
}
