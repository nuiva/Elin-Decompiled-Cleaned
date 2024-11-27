using System;
using System.Collections.Generic;

public class SourceObj : SourceDataInt<SourceObj.Row>
{
	public override SourceObj.Row CreateRow()
	{
		return new SourceObj.Row
		{
			id = SourceData.GetInt(0),
			alias = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			_growth = SourceData.GetStringArray(4),
			costSoil = SourceData.GetInt(5),
			objType = SourceData.GetString(6),
			vals = SourceData.GetStringArray(7),
			tag = SourceData.GetStringArray(8),
			sort = SourceData.GetInt(9),
			reqHarvest = SourceData.GetStringArray(10),
			hp = SourceData.GetInt(11),
			_tileType = SourceData.GetString(12),
			valType = SourceData.GetString(13),
			_idRenderData = SourceData.GetString(14),
			tiles = SourceData.GetIntArray(15),
			snowTile = SourceData.GetInt(16),
			colorMod = SourceData.GetInt(17),
			colorType = SourceData.GetString(18),
			value = SourceData.GetInt(19),
			LV = SourceData.GetInt(20),
			chance = SourceData.GetInt(21),
			recipeKey = SourceData.GetStringArray(22),
			factory = SourceData.GetStringArray(23),
			components = SourceData.GetStringArray(24),
			defMat = SourceData.GetString(25),
			matCategory = SourceData.GetString(26),
			category = SourceData.GetString(27),
			idRoof = SourceData.GetInt(28),
			detail_JP = SourceData.GetString(29),
			detail = SourceData.GetString(30)
		};
	}

	public override void SetRow(SourceObj.Row r)
	{
		this.map[r.id] = r;
	}

	public override void BackupPref()
	{
		this._rows.Clear();
		foreach (SourceObj.Row row in this.rows)
		{
			this._rows[row.id] = row;
		}
	}

	public override void RestorePref()
	{
		foreach (SourceObj.Row row in this.rows)
		{
			RenderRow renderRow = row;
			SourceObj.Row row2 = this._rows.TryGetValue(row.id, null);
			renderRow.pref = (((row2 != null) ? row2.pref : null) ?? new SourcePref());
		}
	}

	public override void ValidatePref()
	{
		foreach (SourceObj.Row row in this.rows)
		{
			row.pref.Validate();
		}
	}

	public string GetName(int id)
	{
		return this.map[id].GetName().ToTitleCase(false);
	}

	public override void OnAfterImportData()
	{
		int num = 0;
		foreach (SourceObj.Row row in this.rows)
		{
			if (row.sort != 0)
			{
				num = row.sort;
			}
			row.sort = num;
			num++;
		}
		this.rows.Sort((SourceObj.Row a, SourceObj.Row b) => a.id - b.id);
	}

	public override void OnInit()
	{
		SourceObj.FallbackRenderData = ResourceCache.Load<RenderData>("Scene/Render/Data/obj");
		Cell.objList = this.rows;
		foreach (SourceObj.Row row in this.rows)
		{
			row.Init();
		}
	}

	public Dictionary<int, SourceObj.Row> _rows = new Dictionary<int, SourceObj.Row>();

	public static RenderData FallbackRenderData;

	[Serializable]
	public class Row : TileRow
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

		public override string RecipeID
		{
			get
			{
				return "o" + this.id.ToString();
			}
		}

		public override RenderData defaultRenderData
		{
			get
			{
				return SourceObj.FallbackRenderData;
			}
		}

		public override void OnInit()
		{
			this.objValType = (this.valType.IsEmpty() ? ObjValType.None : this.valType.ToEnum(true));
			this.autoTile = this.tag.Contains("autotile");
			if (!this._growth.IsEmpty())
			{
				this.growth = ClassCache.Create<GrowSystem>("GrowSystem" + this._growth[0], "Elin");
				this.growth.Init(this);
				this.HasGrowth = true;
				return;
			}
			this.HasGrowth = false;
		}

		public string[] _growth;

		public int costSoil;

		public string objType;

		public string[] vals;

		public string[] reqHarvest;

		public string valType;

		public string matCategory;

		public int idRoof;

		[NonSerialized]
		public bool HasGrowth;

		[NonSerialized]
		public bool autoTile;

		public GrowSystem growth;

		public ObjValType objValType;

		public string name_L;

		public string detail_L;
	}

	public class Stage
	{
		public int step;

		public int[] tiles;

		public string idThing;

		public bool harvest;
	}
}
