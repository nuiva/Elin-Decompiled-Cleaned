using System;

public class SourceQuest : SourceDataString<SourceQuest.Row>
{
	public override SourceQuest.Row CreateRow()
	{
		return new SourceQuest.Row
		{
			id = SourceData.GetString(0),
			name_JP = SourceData.GetString(1),
			name = SourceData.GetString(2),
			type = SourceData.GetString(3),
			drama = SourceData.GetStringArray(4),
			idZone = SourceData.GetString(5),
			group = SourceData.GetString(6),
			tags = SourceData.GetStringArray(7),
			money = SourceData.GetInt(8),
			chance = SourceData.GetInt(9),
			minFame = SourceData.GetInt(10),
			detail_JP = SourceData.GetString(11),
			detail = SourceData.GetString(12),
			talkProgress_JP = SourceData.GetString(13),
			talkProgress = SourceData.GetString(14),
			talkComplete_JP = SourceData.GetString(15),
			talkComplete = SourceData.GetString(16)
		};
	}

	public override void SetRow(SourceQuest.Row r)
	{
		this.map[r.id] = r;
	}

	public override string[] ImportFields
	{
		get
		{
			return new string[]
			{
				"talkProgress",
				"talkComplete"
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

		public string[] drama;

		public string idZone;

		public string group;

		public string[] tags;

		public int money;

		public int chance;

		public int minFame;

		public string detail_JP;

		public string detail;

		public string talkProgress_JP;

		public string talkProgress;

		public string talkComplete_JP;

		public string talkComplete;

		public string name_L;

		public string detail_L;

		public string talkProgress_L;

		public string talkComplete_L;
	}
}
