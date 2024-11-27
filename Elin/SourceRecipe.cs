using System;

public class SourceRecipe : SourceDataInt<SourceRecipe.Row>
{
	public override SourceRecipe.Row CreateRow()
	{
		return new SourceRecipe.Row
		{
			id = SourceData.GetInt(0),
			factory = SourceData.GetString(1),
			type = SourceData.GetString(2),
			thing = SourceData.GetString(3),
			num = SourceData.GetString(4),
			sp = SourceData.GetInt(5),
			time = SourceData.GetInt(6),
			ing1 = SourceData.GetStringArray(7),
			ing2 = SourceData.GetStringArray(8),
			ing3 = SourceData.GetStringArray(9),
			tag = SourceData.GetStringArray(10)
		};
	}

	public override void SetRow(SourceRecipe.Row r)
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

		public int id;

		public string factory;

		public string type;

		public string thing;

		public string num;

		public int sp;

		public int time;

		public string[] ing1;

		public string[] ing2;

		public string[] ing3;

		public string[] tag;

		public string name_L;

		public string detail_L;
	}
}
