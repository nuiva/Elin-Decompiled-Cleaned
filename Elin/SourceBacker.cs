using System;
using System.Collections.Generic;

public class SourceBacker : SourceDataInt<SourceBacker.Row>
{
	public override SourceBacker.Row CreateRow()
	{
		return new SourceBacker.Row
		{
			id = SourceData.GetInt(0),
			valid = SourceData.GetBool(3),
			lang = SourceData.GetString(7),
			name = SourceData.GetString(8),
			text = SourceData.GetString(9),
			type = SourceData.GetInt(10),
			skin = SourceData.GetInt(11),
			gender = SourceData.GetInt(12),
			tree = SourceData.GetString(13),
			deity = SourceData.GetString(14),
			chara = SourceData.GetString(15),
			loot = SourceData.GetString(16),
			isStatic = SourceData.GetInt(17),
			done = SourceData.GetBool(18),
			loc = SourceData.GetString(19),
			original = SourceData.GetString(20)
		};
	}

	public override void SetRow(SourceBacker.Row r)
	{
		this.map[r.id] = r;
	}

	public override void OnInit()
	{
		foreach (SourceBacker.Row row in this.rows)
		{
			if (row.isStatic == 0 && row.valid)
			{
				switch (row.type)
				{
				case 1:
					this.listRemain.Add(row);
					break;
				case 2:
					this.listLantern.Add(row);
					break;
				case 3:
					this.listTree.Add(row);
					break;
				case 4:
					this.listPet.Add(row);
					break;
				case 5:
					this.listSister.Add(row);
					break;
				case 6:
					this.listFollower.Add(row);
					break;
				case 7:
					this.listSnail.Add(row);
					break;
				}
			}
		}
		this.listRemain.Shuffle<SourceBacker.Row>();
		this.listLantern.Shuffle<SourceBacker.Row>();
		this.listTree.Shuffle<SourceBacker.Row>();
		this.listPet.Shuffle<SourceBacker.Row>();
		this.listSister.Shuffle<SourceBacker.Row>();
		this.listFollower.Shuffle<SourceBacker.Row>();
		this.listSnail.Shuffle<SourceBacker.Row>();
	}

	[NonSerialized]
	public List<SourceBacker.Row> listRemain = new List<SourceBacker.Row>();

	[NonSerialized]
	public List<SourceBacker.Row> listLantern = new List<SourceBacker.Row>();

	[NonSerialized]
	public List<SourceBacker.Row> listTree = new List<SourceBacker.Row>();

	[NonSerialized]
	public List<SourceBacker.Row> listPet = new List<SourceBacker.Row>();

	[NonSerialized]
	public List<SourceBacker.Row> listSister = new List<SourceBacker.Row>();

	[NonSerialized]
	public List<SourceBacker.Row> listFollower = new List<SourceBacker.Row>();

	[NonSerialized]
	public List<SourceBacker.Row> listSnail = new List<SourceBacker.Row>();

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

		public string Name
		{
			get
			{
				return BackerContent.ConvertName((Lang.langCode == "CN") ? base.GetText("name", false) : this.name);
			}
		}

		public string Text
		{
			get
			{
				if (!(Lang.langCode == "CN"))
				{
					return this.text;
				}
				return base.GetText("text", false);
			}
		}

		public int id;

		public bool valid;

		public string lang;

		public string name;

		public string text;

		public int type;

		public int skin;

		public int gender;

		public string tree;

		public string deity;

		public string chara;

		public string loot;

		public int isStatic;

		public bool done;

		public string loc;

		public string original;

		public string name_L;

		public string text_L;
	}
}
