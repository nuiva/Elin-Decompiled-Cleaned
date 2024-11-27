using System;
using System.Collections.Generic;

public class SourceHobby : SourceDataInt<SourceHobby.Row>
{
	public override SourceHobby.Row CreateRow()
	{
		return new SourceHobby.Row
		{
			id = SourceData.GetInt(0),
			alias = SourceData.GetString(1),
			type = SourceData.GetString(2),
			name_JP = SourceData.GetString(3),
			name = SourceData.GetString(4),
			ai = SourceData.GetString(5),
			talk = SourceData.GetString(6),
			area = SourceData.GetString(7),
			destTrait = SourceData.GetString(8),
			workTag = SourceData.GetString(9),
			expedition = SourceData.GetString(10),
			resources = SourceData.GetIntArray(11),
			randomRange = SourceData.GetInt(12),
			modifiers = SourceData.GetStringArray(13),
			tax = SourceData.GetInt(14),
			things = SourceData.GetStringArray(15),
			elements = Core.ParseElements(SourceData.GetStr(16, false)),
			skill = SourceData.GetString(17),
			detail_JP = SourceData.GetString(18),
			detail = SourceData.GetString(19)
		};
	}

	public override void SetRow(SourceHobby.Row r)
	{
		this.map[r.id] = r;
	}

	public override void OnInit()
	{
		foreach (SourceHobby.Row row in this.rows)
		{
			string type = row.type;
			if (!(type == "Hobby"))
			{
				if (!(type == "Work"))
				{
					if (type == "Both")
					{
						this.listHobbies.Add(row);
						this.listWorks.Add(row);
					}
				}
				else
				{
					this.listWorks.Add(row);
				}
			}
			else
			{
				this.listHobbies.Add(row);
			}
		}
	}

	[NonSerialized]
	public List<SourceHobby.Row> listHobbies = new List<SourceHobby.Row>();

	[NonSerialized]
	public List<SourceHobby.Row> listWorks = new List<SourceHobby.Row>();

	[Serializable]
	public class Row : SourceData.BaseRow
	{
		public override bool UseAlias
		{
			get
			{
				return true;
			}
		}

		public override string GetAlias
		{
			get
			{
				return this.alias;
			}
		}

		public int id;

		public string alias;

		public string type;

		public string name_JP;

		public string name;

		public string ai;

		public string talk;

		public string area;

		public string destTrait;

		public string workTag;

		public string expedition;

		public int[] resources;

		public int randomRange;

		public string[] modifiers;

		public int tax;

		public string[] things;

		public int[] elements;

		public string skill;

		public string detail_JP;

		public string detail;

		public string name_L;

		public string detail_L;
	}
}
