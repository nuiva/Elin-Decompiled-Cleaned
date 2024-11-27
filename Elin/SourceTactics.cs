using System;

public class SourceTactics : SourceDataString<SourceTactics.Row>
{
	public override SourceTactics.Row CreateRow()
	{
		return new SourceTactics.Row
		{
			id = SourceData.GetString(0),
			name_JP = SourceData.GetString(1),
			name = SourceData.GetString(2),
			dist = SourceData.GetInt(4),
			move = SourceData.GetInt(5),
			movePC = SourceData.GetInt(6),
			party = SourceData.GetInt(7),
			taunt = SourceData.GetInt(8),
			melee = SourceData.GetInt(9),
			range = SourceData.GetInt(10),
			spell = SourceData.GetInt(11),
			heal = SourceData.GetInt(12),
			summon = SourceData.GetInt(13),
			buff = SourceData.GetInt(14),
			debuff = SourceData.GetInt(15),
			tag = SourceData.GetStringArray(16),
			detail_JP = SourceData.GetString(17),
			detail = SourceData.GetString(18)
		};
	}

	public override void SetRow(SourceTactics.Row r)
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

		public string name_JP;

		public string name;

		public int dist;

		public int move;

		public int movePC;

		public int party;

		public int taunt;

		public int melee;

		public int range;

		public int spell;

		public int heal;

		public int summon;

		public int buff;

		public int debuff;

		public string[] tag;

		public string detail_JP;

		public string detail;

		public string name_L;

		public string detail_L;
	}
}
