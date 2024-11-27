using System;

public class SourceFaction : SourceDataString<SourceFaction.Row>
{
	public override SourceFaction.Row CreateRow()
	{
		return new SourceFaction.Row
		{
			id = SourceData.GetString(0),
			name_JP = SourceData.GetString(1),
			name = SourceData.GetString(2),
			type = SourceData.GetString(3),
			faith = SourceData.GetString(4),
			domain = SourceData.GetString(5),
			relation = SourceData.GetInt(6),
			textType_JP = SourceData.GetString(7),
			textType = SourceData.GetString(8),
			textAvatar = SourceData.GetString(9),
			detail_JP = SourceData.GetString(10),
			detail = SourceData.GetString(11)
		};
	}

	public override void SetRow(SourceFaction.Row r)
	{
		this.map[r.id] = r;
	}

	public override string[] ImportFields
	{
		get
		{
			return new string[]
			{
				"textType",
				"textBenefit",
				"textPet"
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

		public string type;

		public string faith;

		public string domain;

		public int relation;

		public string textType_JP;

		public string textType;

		public string textAvatar;

		public string detail_JP;

		public string detail;

		public string name_L;

		public string detail_L;

		public string textType_L;

		public string textBenefit_L;

		public string textPet_L;
	}
}
