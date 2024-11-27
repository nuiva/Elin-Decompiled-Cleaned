using System;

public class SourceHomeResource : SourceDataString<SourceHomeResource.Row>
{
	public override SourceHomeResource.Row CreateRow()
	{
		return new SourceHomeResource.Row
		{
			id = SourceData.GetString(0),
			name_JP = SourceData.GetString(1),
			name = SourceData.GetString(2),
			expMod = SourceData.GetInt(3),
			maxLv = SourceData.GetInt(4),
			reward = SourceData.GetStringArray(5),
			detail_JP = SourceData.GetString(6),
			detail = SourceData.GetString(7)
		};
	}

	public override void SetRow(SourceHomeResource.Row r)
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

		public int expMod;

		public int maxLv;

		public string[] reward;

		public string detail_JP;

		public string detail;

		public string name_L;

		public string detail_L;
	}
}
