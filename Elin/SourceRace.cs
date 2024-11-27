using System;
using System.Collections.Generic;

public class SourceRace : SourceDataString<SourceRace.Row>
{
	public override SourceRace.Row CreateRow()
	{
		return new SourceRace.Row
		{
			id = SourceData.GetString(0),
			name_JP = SourceData.GetString(1),
			name = SourceData.GetString(2),
			playable = SourceData.GetInt(3),
			tag = SourceData.GetStringArray(4),
			life = SourceData.GetInt(5),
			mana = SourceData.GetInt(6),
			vigor = SourceData.GetInt(7),
			DV = SourceData.GetInt(8),
			PV = SourceData.GetInt(9),
			PDR = SourceData.GetInt(10),
			EDR = SourceData.GetInt(11),
			EP = SourceData.GetInt(12),
			STR = SourceData.GetInt(13),
			END = SourceData.GetInt(14),
			DEX = SourceData.GetInt(15),
			PER = SourceData.GetInt(16),
			LER = SourceData.GetInt(17),
			WIL = SourceData.GetInt(18),
			MAG = SourceData.GetInt(19),
			CHA = SourceData.GetInt(20),
			SPD = SourceData.GetInt(21),
			INT = SourceData.GetInt(23),
			martial = SourceData.GetInt(24),
			pen = SourceData.GetInt(25),
			elements = Core.ParseElements(SourceData.GetStr(26, false)),
			skill = SourceData.GetString(27),
			figure = SourceData.GetString(28),
			material = SourceData.GetString(29),
			corpse = SourceData.GetStringArray(30),
			loot = SourceData.GetStringArray(31),
			blood = SourceData.GetInt(32),
			meleeStyle = SourceData.GetString(33),
			castStyle = SourceData.GetString(34),
			EQ = SourceData.GetStringArray(35),
			sex = SourceData.GetInt(36),
			age = SourceData.GetIntArray(37),
			height = SourceData.GetInt(38),
			breeder = SourceData.GetInt(39),
			food = SourceData.GetStringArray(40),
			fur = SourceData.GetString(41),
			detail_JP = SourceData.GetString(42),
			detail = SourceData.GetString(43)
		};
	}

	public override void SetRow(SourceRace.Row r)
	{
		this.map[r.id] = r;
	}

	public override void OnInit()
	{
		foreach (SourceRace.Row row in this.rows)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			dictionary[70] = row.STR;
			dictionary[71] = row.END;
			dictionary[72] = row.DEX;
			dictionary[73] = row.PER;
			dictionary[74] = row.LER;
			dictionary[75] = row.WIL;
			dictionary[76] = row.MAG;
			dictionary[77] = row.CHA;
			dictionary[79] = row.SPD;
			dictionary[80] = row.INT;
			dictionary[100] = row.martial;
			dictionary[60] = row.life;
			dictionary[61] = row.mana;
			dictionary[62] = row.vigor;
			dictionary[65] = row.PV;
			dictionary[64] = row.DV;
			dictionary[55] = row.PDR;
			dictionary[56] = row.EDR;
			dictionary[57] = row.EP;
			dictionary[261] = 1;
			dictionary[225] = 1;
			dictionary[255] = 1;
			dictionary[220] = 1;
			dictionary[250] = 1;
			dictionary[101] = 1;
			dictionary[102] = 1;
			dictionary[103] = 1;
			dictionary[107] = 1;
			dictionary[106] = 1;
			dictionary[110] = 1;
			dictionary[111] = 1;
			dictionary[104] = 1;
			dictionary[109] = 1;
			dictionary[108] = 1;
			dictionary[123] = 1;
			dictionary[122] = 1;
			dictionary[120] = 1;
			dictionary[150] = 1;
			dictionary[301] = 1;
			dictionary[306] = 1;
			row.elementMap = Element.GetElementMap(row.elements, dictionary);
			row.visibleWithTelepathy = (!row.IsUndead && !row.IsMachine && !row.IsHorror);
		}
	}

	[Serializable]
	public class Row : SourceData.BaseRow
	{
		public override bool UseAlias
		{
			get
			{
				return false;
			}
		}

		public override string GetAlias
		{
			get
			{
				return "n";
			}
		}

		public bool IsAnimal
		{
			get
			{
				return this.tag.Contains("animal");
			}
		}

		public bool IsHuman
		{
			get
			{
				return this.tag.Contains("human");
			}
		}

		public bool IsUndead
		{
			get
			{
				return this.tag.Contains("undead");
			}
		}

		public bool IsMachine
		{
			get
			{
				return this.tag.Contains("machine");
			}
		}

		public bool IsHorror
		{
			get
			{
				return this.tag.Contains("horror");
			}
		}

		public bool IsFish
		{
			get
			{
				return this.tag.Contains("fish");
			}
		}

		public bool IsFairy
		{
			get
			{
				return this.tag.Contains("fairy");
			}
		}

		public bool IsGod
		{
			get
			{
				return this.tag.Contains("god");
			}
		}

		public bool IsDragon
		{
			get
			{
				return this.tag.Contains("dragon");
			}
		}

		public bool IsPlant
		{
			get
			{
				return this.tag.Contains("plant");
			}
		}

		public string id;

		public string name_JP;

		public string name;

		public int playable;

		public string[] tag;

		public int life;

		public int mana;

		public int vigor;

		public int DV;

		public int PV;

		public int PDR;

		public int EDR;

		public int EP;

		public int STR;

		public int END;

		public int DEX;

		public int PER;

		public int LER;

		public int WIL;

		public int MAG;

		public int CHA;

		public int SPD;

		public int INT;

		public int martial;

		public int pen;

		public int[] elements;

		public string skill;

		public string figure;

		public string material;

		public string[] corpse;

		public string[] loot;

		public int blood;

		public string meleeStyle;

		public string castStyle;

		public string[] EQ;

		public int sex;

		public int[] age;

		public int height;

		public int breeder;

		public string[] food;

		public string fur;

		public string detail_JP;

		public string detail;

		public bool visibleWithTelepathy;

		public Dictionary<int, int> elementMap;

		public string name_L;

		public string detail_L;
	}
}
