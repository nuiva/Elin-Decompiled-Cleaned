using System;
using System.Collections.Generic;

public class SourceCategory : SourceDataString<SourceCategory.Row>
{
	public override SourceCategory.Row CreateRow()
	{
		return new SourceCategory.Row
		{
			id = SourceData.GetString(0),
			uid = SourceData.GetInt(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			_parent = SourceData.GetString(4),
			recipeCat = SourceData.GetString(5),
			slot = Core.GetElement(SourceData.GetStr(6, false)),
			skill = Core.GetElement(SourceData.GetStr(7, false)),
			maxStack = SourceData.GetInt(8),
			tileDummy = SourceData.GetInt(9),
			installOne = SourceData.GetBool(10),
			ignoreBless = SourceData.GetInt(11),
			tag = SourceData.GetStringArray(12),
			idThing = SourceData.GetString(13),
			recycle = SourceData.GetStringArray(14),
			costSP = SourceData.GetInt(15),
			gift = SourceData.GetInt(16),
			deliver = SourceData.GetInt(17),
			offer = SourceData.GetInt(18),
			ticket = SourceData.GetInt(19),
			sortVal = SourceData.GetInt(20),
			flag = SourceData.GetInt(21)
		};
	}

	public override void SetRow(SourceCategory.Row r)
	{
		this.map[r.id] = r;
	}

	public override void OnInit()
	{
		foreach (SourceCategory.Row row in this.rows)
		{
			row.children = new List<SourceCategory.Row>();
		}
		foreach (SourceCategory.Row row2 in this.rows)
		{
			if (!row2._parent.IsEmpty())
			{
				SourceCategory.Row row3 = this.map[row2._parent];
				row2.parent = row3;
				row3.children.Add(row2);
			}
		}
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

		public bool IsChildOf(string id)
		{
			return this.IsChildOf(EClass.sources.categories.map[id]);
		}

		public bool IsChildOf(SourceCategory.Row r)
		{
			return r == this || (this.parent != null && this.parent.IsChildOf(r));
		}

		public bool IsChildOf(int _uid)
		{
			return this.uid == _uid || (this.parent != null && this.parent.IsChildOf(_uid));
		}

		public bool Contatin(int _uid)
		{
			if (this.uid == _uid)
			{
				return true;
			}
			using (List<SourceCategory.Row>.Enumerator enumerator = this.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Contatin(_uid))
					{
						return true;
					}
				}
			}
			return false;
		}

		public SourceCategory.Row GetRoot()
		{
			if (this.parent == null)
			{
				return this;
			}
			return this.parent.GetRoot();
		}

		public SourceCategory.Row GetSecondRoot()
		{
			if (this.parent == null || this.parent.parent == null)
			{
				return this;
			}
			return this.parent.GetSecondRoot();
		}

		public string GetIdThing()
		{
			if (!this.idThing.IsEmpty())
			{
				return this.idThing;
			}
			return this.parent.GetIdThing();
		}

		public string id;

		public int uid;

		public string name_JP;

		public string name;

		public string _parent;

		public string recipeCat;

		public int slot;

		public int skill;

		public int maxStack;

		public int tileDummy;

		public bool installOne;

		public int ignoreBless;

		public string[] tag;

		public string idThing;

		public string[] recycle;

		public int costSP;

		public int gift;

		public int deliver;

		public int offer;

		public int ticket;

		public int sortVal;

		public int flag;

		[NonSerialized]
		public SourceCategory.Row parent;

		[NonSerialized]
		public List<SourceCategory.Row> children;

		public string name_L;
	}
}
