using System;

public class SourceCheck : SourceDataString<SourceCheck.Row>
{
	public override SourceCheck.Row CreateRow()
	{
		return new SourceCheck.Row
		{
			id = SourceData.GetString(0),
			element = Core.GetElement(SourceData.GetStr(1, false)),
			targetElement = Core.GetElement(SourceData.GetStr(2, false)),
			subFactor = SourceData.GetFloat(3),
			targetSubFactor = SourceData.GetFloat(4),
			baseDC = SourceData.GetInt(5),
			critRange = SourceData.GetInt(6),
			fumbleRange = SourceData.GetInt(7),
			dice = SourceData.GetInt(8),
			lvMod = SourceData.GetFloat(9)
		};
	}

	public override void SetRow(SourceCheck.Row r)
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
	}
}
