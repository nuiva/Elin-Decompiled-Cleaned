using System;

public class SourceZone : SourceDataString<SourceZone.Row>
{
	public override SourceZone.Row CreateRow()
	{
		return new SourceZone.Row
		{
			id = SourceData.GetString(0),
			parent = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			type = SourceData.GetString(4),
			LV = SourceData.GetInt(5),
			chance = SourceData.GetInt(6),
			faction = SourceData.GetString(7),
			value = SourceData.GetInt(8),
			idProfile = SourceData.GetString(9),
			idFile = SourceData.GetStringArray(10),
			idBiome = SourceData.GetString(11),
			idGen = SourceData.GetString(12),
			idPlaylist = SourceData.GetString(13),
			tag = SourceData.GetStringArray(14),
			cost = SourceData.GetInt(15),
			dev = SourceData.GetInt(16),
			image = SourceData.GetString(17),
			pos = SourceData.GetIntArray(18),
			questTag = SourceData.GetStringArray(19),
			textFlavor_JP = SourceData.GetString(20),
			textFlavor = SourceData.GetString(21),
			detail_JP = SourceData.GetString(22),
			detail = SourceData.GetString(23)
		};
	}

	public override void SetRow(SourceZone.Row r)
	{
		this.map[r.id] = r;
	}

	public override string[] ImportFields
	{
		get
		{
			return new string[]
			{
				"textFlavor"
			};
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

		public string id;

		public string parent;

		public string name_JP;

		public string name;

		public string type;

		public int LV;

		public int chance;

		public string faction;

		public int value;

		public string idProfile;

		public string[] idFile;

		public string idBiome;

		public string idGen;

		public string idPlaylist;

		public string[] tag;

		public int cost;

		public int dev;

		public string image;

		public int[] pos;

		public string[] questTag;

		public string textFlavor_JP;

		public string textFlavor;

		public string detail_JP;

		public string detail;

		public string name_L;

		public string detail_L;

		public string textFlavor_L;
	}
}
