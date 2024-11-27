using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class SourceFood : SourceThingV
{
	public override SourceThingV.Row CreateRow()
	{
		return new SourceFood.Row2
		{
			id = SourceData.GetString(0),
			_origin = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			unit_JP = SourceData.GetString(3),
			name = SourceData.GetString(4),
			unit = SourceData.GetString(5),
			name2_JP = SourceData.GetStringArray(6),
			name2 = SourceData.GetStringArray(7),
			tiles = SourceData.GetIntArray(8),
			parse = SourceData.GetStringArray(9),
			vals = SourceData.GetStringArray(10),
			trait = SourceData.GetStringArray(11),
			elements = Core.ParseElements(SourceData.GetStr(12, false)),
			idTaste = SourceData.GetString(14),
			TST = SourceData.GetInt(15),
			NUT = SourceData.GetInt(16),
			STR = SourceData.GetInt(17),
			STR2 = SourceData.GetInt(18),
			END = SourceData.GetInt(19),
			END2 = SourceData.GetInt(20),
			DEX = SourceData.GetInt(21),
			DEX2 = SourceData.GetInt(22),
			PER = SourceData.GetInt(23),
			PER2 = SourceData.GetInt(24),
			LER = SourceData.GetInt(25),
			LER2 = SourceData.GetInt(26),
			WIL = SourceData.GetInt(27),
			WIL2 = SourceData.GetInt(28),
			MAG = SourceData.GetInt(29),
			MAG2 = SourceData.GetInt(30),
			CHA = SourceData.GetInt(31),
			CHA2 = SourceData.GetInt(32),
			LV = SourceData.GetInt(34),
			chance = SourceData.GetInt(35),
			value = SourceData.GetInt(36),
			weight = SourceData.GetInt(37),
			recipeKey = SourceData.GetStringArray(38),
			factory = SourceData.GetStringArray(39),
			components = SourceData.GetStringArray(40),
			defMat = SourceData.GetString(41),
			category = SourceData.GetString(42),
			tag = SourceData.GetStringArray(43),
			detail_JP = SourceData.GetString(44),
			detail = SourceData.GetString(45)
		};
	}

	public override void SetRow(SourceThingV.Row r)
	{
		this.map[r.id] = r;
	}

	public override void OnImportRow(SourceThingV.Row _r, SourceThing.Row c)
	{
		SourceFood.<>c__DisplayClass3_0 CS$<>8__locals1;
		CS$<>8__locals1.list = new List<int>(c.elements);
		SourceFood.Row2 row = _r as SourceFood.Row2;
		SourceFood.<OnImportRow>g__Add|3_0(10, row.NUT, ref CS$<>8__locals1);
		SourceFood.<OnImportRow>g__Parse|3_1(row.STR, 70, row.STR2, 440, ref CS$<>8__locals1);
		SourceFood.<OnImportRow>g__Parse|3_1(row.END, 71, row.END2, 441, ref CS$<>8__locals1);
		SourceFood.<OnImportRow>g__Parse|3_1(row.DEX, 72, row.DEX2, 442, ref CS$<>8__locals1);
		SourceFood.<OnImportRow>g__Parse|3_1(row.PER, 73, row.PER2, 443, ref CS$<>8__locals1);
		SourceFood.<OnImportRow>g__Parse|3_1(row.LER, 74, row.LER2, 444, ref CS$<>8__locals1);
		SourceFood.<OnImportRow>g__Parse|3_1(row.WIL, 75, row.WIL2, 445, ref CS$<>8__locals1);
		SourceFood.<OnImportRow>g__Parse|3_1(row.MAG, 76, row.MAG2, 446, ref CS$<>8__locals1);
		SourceFood.<OnImportRow>g__Parse|3_1(row.CHA, 77, row.CHA2, 447, ref CS$<>8__locals1);
		for (int i = 0; i < row.elements.Length; i += 2)
		{
			SourceFood.<OnImportRow>g__Add|3_0(_r.elements[i], row.elements[i + 1], ref CS$<>8__locals1);
		}
		c.elements = CS$<>8__locals1.list.ToArray();
		c.name2 = row.name2;
		c.name2_JP = row.name2_JP;
		if (!row.unit_JP.IsEmpty())
		{
			c.unit_JP = row.unit_JP;
		}
	}

	[CompilerGenerated]
	internal static void <OnImportRow>g__Add|3_0(int ele, int a, ref SourceFood.<>c__DisplayClass3_0 A_2)
	{
		A_2.list.Add(ele);
		A_2.list.Add(a);
	}

	[CompilerGenerated]
	internal static void <OnImportRow>g__Parse|3_1(int raw, int ele, int raw2, int ele2, ref SourceFood.<>c__DisplayClass3_0 A_4)
	{
		if (raw != 0)
		{
			SourceFood.<OnImportRow>g__Add|3_0(ele, raw, ref A_4);
		}
		if (raw2 != 0)
		{
			SourceFood.<OnImportRow>g__Add|3_0(ele2, raw2, ref A_4);
		}
	}

	[Serializable]
	public class Row2 : SourceThingV.Row
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

		public string idTaste;

		public int TST;

		public int NUT;

		public int STR;

		public int STR2;

		public int END;

		public int END2;

		public int DEX;

		public int DEX2;

		public int PER;

		public int PER2;

		public int LER;

		public int LER2;

		public int WIL;

		public int WIL2;

		public int MAG;

		public int MAG2;

		public int CHA;

		public int CHA2;
	}
}
