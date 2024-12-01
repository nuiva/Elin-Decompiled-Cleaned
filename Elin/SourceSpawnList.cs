using System;

public class SourceSpawnList : SourceDataString<SourceSpawnList.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public string parent;

		public string type;

		public string[] category;

		public string[] idCard;

		public string[] tag;

		public string[] filter;

		public override bool UseAlias => false;

		public override string GetAlias => "n";

		public override string GetEditorListName()
		{
			return this.GetField<string>("id") ?? "";
		}
	}

	public override Row CreateRow()
	{
		return new Row
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

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}
}
