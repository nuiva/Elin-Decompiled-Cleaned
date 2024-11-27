using System;

public class SourceKeyItem : SourceDataInt<SourceKeyItem.Row>
{
	public override SourceKeyItem.Row CreateRow()
	{
		return new SourceKeyItem.Row
		{
			id = SourceData.GetInt(0),
			alias = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			detail_JP = SourceData.GetString(4),
			detail = SourceData.GetString(5)
		};
	}

	public override void SetRow(SourceKeyItem.Row r)
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

		public int id;

		public string alias;

		public string name_JP;

		public string name;

		public string detail_JP;

		public string detail;

		public string name_L;

		public string detail_L;
	}
}
