using System;

public class SourcePerson : SourceDataString<SourcePerson.Row>
{
	public override SourcePerson.Row CreateRow()
	{
		return new SourcePerson.Row
		{
			id = SourceData.GetString(0),
			idActor = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			aka_JP = SourceData.GetString(4),
			aka = SourceData.GetString(5),
			portrait = SourceData.GetString(6),
			faction = SourceData.GetString(7),
			LV = SourceData.GetInt(8),
			job = SourceData.GetString(9),
			race = SourceData.GetString(10),
			material = SourceData.GetString(11),
			bio = SourceData.GetString(12),
			detail_JP = SourceData.GetString(13),
			detail = SourceData.GetString(14)
		};
	}

	public override void SetRow(SourcePerson.Row r)
	{
		this.map[r.id] = r;
	}

	public override string[] ImportFields
	{
		get
		{
			return new string[]
			{
				"aka"
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

		public string idActor;

		public string name_JP;

		public string name;

		public string aka_JP;

		public string aka;

		public string portrait;

		public string faction;

		public int LV;

		public string job;

		public string race;

		public string material;

		public string bio;

		public string detail_JP;

		public string detail;

		public string name_L;

		public string detail_L;

		public string aka_L;
	}
}
