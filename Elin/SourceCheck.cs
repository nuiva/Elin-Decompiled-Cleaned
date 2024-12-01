using System;

public class SourceCheck : SourceDataString<SourceCheck.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public int element;

		public int targetElement;

		public float subFactor;

		public float targetSubFactor;

		public int baseDC;

		public int critRange;

		public int fumbleRange;

		public int dice;

		public float lvMod;

		public override bool UseAlias => false;

		public override string GetAlias => "n";
	}

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			element = Core.GetElement(SourceData.GetStr(1)),
			targetElement = Core.GetElement(SourceData.GetStr(2)),
			subFactor = SourceData.GetFloat(3),
			targetSubFactor = SourceData.GetFloat(4),
			baseDC = SourceData.GetInt(5),
			critRange = SourceData.GetInt(6),
			fumbleRange = SourceData.GetInt(7),
			dice = SourceData.GetInt(8),
			lvMod = SourceData.GetFloat(9)
		};
	}

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}
}
