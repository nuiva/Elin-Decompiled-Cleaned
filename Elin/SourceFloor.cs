using System;
using System.Collections.Generic;

public class SourceFloor : SourceDataInt<SourceFloor.Row>
{
	public override SourceFloor.Row CreateRow()
	{
		return new SourceFloor.Row
		{
			id = SourceData.GetInt(0),
			alias = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			sort = SourceData.GetInt(4),
			idBiome = SourceData.GetString(5),
			reqHarvest = SourceData.GetStringArray(6),
			hp = SourceData.GetInt(7),
			_tileType = SourceData.GetString(8),
			_idRenderData = SourceData.GetString(9),
			tiles = SourceData.GetIntArray(10),
			colorMod = SourceData.GetInt(11),
			value = SourceData.GetInt(12),
			LV = SourceData.GetInt(13),
			recipeKey = SourceData.GetStringArray(14),
			factory = SourceData.GetStringArray(15),
			components = SourceData.GetStringArray(16),
			defMat = SourceData.GetString(17),
			defBlock = SourceData.GetString(18),
			bridgeBlock = SourceData.GetString(19),
			category = SourceData.GetString(20),
			edge = SourceData.GetInt(21),
			autotile = SourceData.GetInt(22),
			autotilePriority = SourceData.GetInt(23),
			autotileBrightness = SourceData.GetFloat(24),
			soundFoot = SourceData.GetString(25),
			tag = SourceData.GetStringArray(26),
			detail_JP = SourceData.GetString(27),
			detail = SourceData.GetString(28)
		};
	}

	public override void SetRow(SourceFloor.Row r)
	{
		this.map[r.id] = r;
	}

	public override void BackupPref()
	{
		this._rows.Clear();
		foreach (SourceFloor.Row row in this.rows)
		{
			this._rows[row.id] = row;
		}
	}

	public override void RestorePref()
	{
		foreach (SourceFloor.Row row in this.rows)
		{
			RenderRow renderRow = row;
			SourceFloor.Row row2 = this._rows.TryGetValue(row.id, null);
			renderRow.pref = (((row2 != null) ? row2.pref : null) ?? new SourcePref());
		}
	}

	public override void ValidatePref()
	{
		foreach (SourceFloor.Row row in this.rows)
		{
			row.pref.Validate();
		}
	}

	public override void OnAfterImportData()
	{
		int num = 0;
		foreach (SourceFloor.Row row in this.rows)
		{
			if (row.sort != 0)
			{
				num = row.sort;
			}
			row.sort = num;
			num++;
		}
		this.rows.Sort((SourceFloor.Row a, SourceFloor.Row b) => a.id - b.id);
	}

	public override void OnInit()
	{
		SourceFloor.FallbackRenderData = ResourceCache.Load<RenderData>("Scene/Render/Data/floor");
		Cell.floorList = this.rows;
		foreach (SourceFloor.Row row in this.rows)
		{
			row.Init();
		}
	}

	public void OnAfterInit()
	{
		foreach (SourceFloor.Row row in this.rows)
		{
			row._defBlock = EClass.sources.blocks.alias[row.defBlock];
			row._bridgeBlock = EClass.sources.blocks.alias[row.bridgeBlock];
			row.nonGradient = row.ContainsTag("nonGradient");
		}
	}

	public Dictionary<int, SourceFloor.Row> _rows = new Dictionary<int, SourceFloor.Row>();

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
				return "f" + this.id.ToString();
			}
		}

		public override RenderData defaultRenderData
		{
			get
			{
				return SourceFloor.FallbackRenderData;
			}
		}

		public override void OnInit()
		{
			this.ignoreTransition = this.tag.Contains("noTransition");
			this.ignoreSnow = this.tag.Contains("noSnow");
			this.isBeach = this.tag.Contains("beach");
			this.snowtile = this.tag.Contains("snowtile");
			if (!this.idBiome.IsEmpty())
			{
				this.biome = EClass.core.refs.biomes.dict[this.idBiome];
			}
		}

		public override int GetTile(SourceMaterial.Row mat, int dir = 0)
		{
			return this._tiles[dir % this._tiles.Length];
		}

		public string idBiome;

		public string[] reqHarvest;

		public string defBlock;

		public string bridgeBlock;

		public int edge;

		public int autotile;

		public int autotilePriority;

		public float autotileBrightness;

		public bool nonGradient;

		public bool isBeach;

		public bool snowtile;

		public bool ignoreTransition;

		[NonSerialized]
		public SourceBlock.Row _defBlock;

		[NonSerialized]
		public SourceBlock.Row _bridgeBlock;

		[NonSerialized]
		public BiomeProfile biome;

		public string name_L;

		public string detail_L;
	}
}
