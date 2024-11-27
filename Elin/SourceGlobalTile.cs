using System;
using System.Collections.Generic;

public class SourceGlobalTile : SourceDataInt<SourceGlobalTile.Row>
{
	public override SourceGlobalTile.Row CreateRow()
	{
		return new SourceGlobalTile.Row
		{
			id = SourceData.GetInt(0),
			alias = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			tiles = SourceData.GetIntArray(4),
			floor = SourceData.GetInt(5),
			zoneProfile = SourceData.GetString(6),
			tag = SourceData.GetStringArray(7),
			dangerLv = SourceData.GetInt(8),
			trait = SourceData.GetStringArray(9),
			idBiome = SourceData.GetString(10),
			attribs = SourceData.GetIntArray(11),
			detail_JP = SourceData.GetString(12),
			detail = SourceData.GetString(13)
		};
	}

	public override void SetRow(SourceGlobalTile.Row r)
	{
		this.map[r.id] = r;
	}

	public override void OnInit()
	{
		foreach (SourceGlobalTile.Row row in this.rows)
		{
			foreach (int key in row.tiles)
			{
				this.tileAlias[key] = row;
			}
		}
	}

	public Dictionary<int, SourceGlobalTile.Row> tileAlias = new Dictionary<int, SourceGlobalTile.Row>();

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

		public string name_JP;

		public string name;

		public int[] tiles;

		public int floor;

		public string zoneProfile;

		public string[] tag;

		public int dangerLv;

		public string[] trait;

		public string idBiome;

		public int[] attribs;

		public string detail_JP;

		public string detail;
	}
}
