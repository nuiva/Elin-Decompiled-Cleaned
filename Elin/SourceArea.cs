using System;

public class SourceArea : SourceDataString<SourceArea.Row>
{
	public override SourceArea.Row CreateRow()
	{
		return new SourceArea.Row
		{
			id = SourceData.GetString(0),
			name_JP = SourceData.GetString(1),
			name = SourceData.GetString(2),
			textAssign_JP = SourceData.GetString(3),
			textAssign = SourceData.GetString(4),
			detail_JP = SourceData.GetString(5),
			tag = SourceData.GetString(6),
			detail = SourceData.GetString(7)
		};
	}

	public override void SetRow(SourceArea.Row r)
	{
		this.map[r.id] = r;
	}

	public override string[] ImportFields
	{
		get
		{
			return new string[]
			{
				"textAssign"
			};
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

		public string id;

		public string name_JP;

		public string name;

		public string textAssign_JP;

		public string textAssign;

		public string detail_JP;

		public string tag;

		public string detail;

		public string name_L;

		public string detail_L;

		public string textAssign_L;
	}
}
