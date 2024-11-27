using System;

public class SourceCollectible : SourceDataString<SourceCollectible.Row>
{
	public override SourceCollectible.Row CreateRow()
	{
		return new SourceCollectible.Row
		{
			id = SourceData.GetString(0),
			name_JP = SourceData.GetString(1),
			name = SourceData.GetString(2),
			rarity = SourceData.GetInt(3),
			prefab = SourceData.GetString(4),
			num = SourceData.GetInt(5),
			filter = SourceData.GetString(6),
			tag = SourceData.GetStringArray(7),
			sound = SourceData.GetString(8),
			detail_JP = SourceData.GetString(9),
			detail = SourceData.GetString(10)
		};
	}

	public override void SetRow(SourceCollectible.Row r)
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

		public int rarity;

		public string prefab;

		public int num;

		public string filter;

		public string[] tag;

		public string sound;

		public string detail_JP;

		public string detail;

		public string name_L;

		public string detail_L;
	}
}
