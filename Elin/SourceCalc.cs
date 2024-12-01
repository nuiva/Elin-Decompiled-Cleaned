using System;

public class SourceCalc : SourceDataString<SourceCalc.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public string num;

		public string sides;

		public string bonus;

		public override bool UseAlias => false;

		public override string GetAlias => "n";
	}

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			num = SourceData.GetString(2),
			sides = SourceData.GetString(3),
			bonus = SourceData.GetString(4)
		};
	}

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}
}
