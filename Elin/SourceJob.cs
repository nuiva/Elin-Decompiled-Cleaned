using System;
using System.Collections.Generic;

public class SourceJob : SourceDataString<SourceJob.Row>
{
	public override SourceJob.Row CreateRow()
	{
		return new SourceJob.Row
		{
			id = SourceData.GetString(0),
			name_JP = SourceData.GetString(1),
			name = SourceData.GetString(2),
			playable = SourceData.GetInt(3),
			STR = SourceData.GetInt(4),
			END = SourceData.GetInt(5),
			DEX = SourceData.GetInt(6),
			PER = SourceData.GetInt(7),
			LER = SourceData.GetInt(8),
			WIL = SourceData.GetInt(9),
			MAG = SourceData.GetInt(10),
			CHA = SourceData.GetInt(11),
			SPD = SourceData.GetInt(12),
			elements = Core.ParseElements(SourceData.GetStr(14, false)),
			weapon = SourceData.GetStringArray(15),
			equip = SourceData.GetString(16),
			domain = Core.ParseElements(SourceData.GetStr(17, false)),
			detail_JP = SourceData.GetString(18),
			detail = SourceData.GetString(19)
		};
	}

	public override void SetRow(SourceJob.Row r)
	{
		this.map[r.id] = r;
	}

	public override void OnInit()
	{
		foreach (SourceJob.Row row in this.rows)
		{
			Dictionary<int, int> dictionary = row.elementMap = Element.GetElementMap(row.elements);
			dictionary[70] = row.STR;
			dictionary[71] = row.END;
			dictionary[72] = row.DEX;
			dictionary[73] = row.PER;
			dictionary[74] = row.LER;
			dictionary[75] = row.WIL;
			dictionary[76] = row.MAG;
			dictionary[77] = row.CHA;
			dictionary[79] = row.SPD;
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

		public void WriteNote(UINote n)
		{
			n.Clear();
			n.AddHeader(this.GetName().ToTitleCase(false), null);
			n.AddText(base.GetDetail(), FontColor.DontChange).SetWidth(400);
			n.Build();
		}

		public string id;

		public string name_JP;

		public string name;

		public int playable;

		public int STR;

		public int END;

		public int DEX;

		public int PER;

		public int LER;

		public int WIL;

		public int MAG;

		public int CHA;

		public int SPD;

		public int[] elements;

		public string[] weapon;

		public string equip;

		public int[] domain;

		public string detail_JP;

		public string detail;

		public Dictionary<int, int> elementMap;

		public string name_L;

		public string detail_L;
	}
}
