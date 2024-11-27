using System;

public class SourceResearch : SourceDataString<SourceResearch.Row>
{
	public override SourceResearch.Row CreateRow()
	{
		return new SourceResearch.Row
		{
			id = SourceData.GetString(0),
			name_JP = SourceData.GetString(1),
			name = SourceData.GetString(2),
			resource = SourceData.GetStringArray(3),
			money = SourceData.GetInt(4),
			tech = SourceData.GetInt(5),
			req = SourceData.GetString(6),
			type = SourceData.GetString(7),
			expMod = SourceData.GetInt(8),
			maxLv = SourceData.GetInt(9),
			reward = SourceData.GetString(10),
			detail_JP = SourceData.GetString(11),
			detail = SourceData.GetString(12)
		};
	}

	public override void SetRow(SourceResearch.Row r)
	{
		this.map[r.id] = r;
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

		public string id;

		public string name_JP;

		public string name;

		public string[] resource;

		public int money;

		public int tech;

		public string req;

		public string type;

		public int expMod;

		public int maxLv;

		public string reward;

		public string detail_JP;

		public string detail;

		public string name_L;

		public string detail_L;
	}
}
