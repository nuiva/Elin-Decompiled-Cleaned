using System;

public class SourceCharaText : SourceDataString<SourceCharaText.Row>
{
	public override SourceCharaText.Row CreateRow()
	{
		return new SourceCharaText.Row
		{
			id = SourceData.GetString(0),
			calm_JP = SourceData.GetString(2),
			fov_JP = SourceData.GetString(3),
			aggro_JP = SourceData.GetString(4),
			dead_JP = SourceData.GetString(5),
			kill_JP = SourceData.GetString(6),
			calm = SourceData.GetString(7),
			fov = SourceData.GetString(8),
			aggro = SourceData.GetString(9),
			dead = SourceData.GetString(10),
			kill = SourceData.GetString(11)
		};
	}

	public override void SetRow(SourceCharaText.Row r)
	{
		this.map[r.id] = r;
	}

	public override string[] ImportFields
	{
		get
		{
			return new string[]
			{
				"calm",
				"fov",
				"aggro",
				"dead",
				"kill"
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

		public string calm_JP;

		public string fov_JP;

		public string aggro_JP;

		public string dead_JP;

		public string kill_JP;

		public string calm;

		public string fov;

		public string aggro;

		public string dead;

		public string kill;

		public string calm_L;

		public string fov_L;

		public string aggro_L;

		public string dead_L;

		public string kill_L;
	}
}
