using System;

public class SourceCalc : SourceDataString<SourceCalc.Row>
{
	public override SourceCalc.Row CreateRow()
	{
		return new SourceCalc.Row
		{
			id = SourceData.GetString(0),
			num = SourceData.GetString(2),
			sides = SourceData.GetString(3),
			bonus = SourceData.GetString(4)
		};
	}

	public override void SetRow(SourceCalc.Row r)
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

		public string num;

		public string sides;

		public string bonus;
	}
}
