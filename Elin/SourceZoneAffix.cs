using System;

public class SourceZoneAffix : SourceDataInt<SourceZoneAffix.Row>
{
	public override SourceZoneAffix.Row CreateRow()
	{
		return new SourceZoneAffix.Row
		{
			id = SourceData.GetInt(0),
			zone = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			textAssign_JP = SourceData.GetString(4),
			textAssign = SourceData.GetString(5),
			detail_JP = SourceData.GetString(6),
			detail = SourceData.GetString(7)
		};
	}

	public override void SetRow(SourceZoneAffix.Row r)
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

		public int id;

		public string zone;

		public string name_JP;

		public string name;

		public string textAssign_JP;

		public string textAssign;

		public string detail_JP;

		public string detail;

		public string name_L;

		public string detail_L;
	}
}
