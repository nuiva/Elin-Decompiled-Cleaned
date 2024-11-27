using System;

public class SourceSpawnList : SourceDataString<SourceSpawnList.Row>
{
	public override SourceSpawnList.Row CreateRow()
	{
		return new SourceSpawnList.Row
		{
			id = SourceData.GetString(0),
			parent = SourceData.GetString(2),
			type = SourceData.GetString(3),
			category = SourceData.GetStringArray(4),
			idCard = SourceData.GetStringArray(5),
			tag = SourceData.GetStringArray(6),
			filter = SourceData.GetStringArray(7)
		};
	}

	public override void SetRow(SourceSpawnList.Row r)
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

		public override string GetEditorListName()
		{
			return this.GetField("id") ?? "";
		}

		public string id;

		public string parent;

		public string type;

		public string[] category;

		public string[] idCard;

		public string[] tag;

		public string[] filter;
	}
}
