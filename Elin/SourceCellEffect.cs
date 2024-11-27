using System;

public class SourceCellEffect : SourceDataInt<SourceCellEffect.Row>
{
	public override SourceCellEffect.Row CreateRow()
	{
		return new SourceCellEffect.Row
		{
			id = SourceData.GetInt(0),
			alias = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			sort = SourceData.GetInt(4),
			_tileType = SourceData.GetString(5),
			_idRenderData = SourceData.GetString(6),
			tiles = SourceData.GetIntArray(7),
			anime = SourceData.GetIntArray(8),
			colorMod = SourceData.GetInt(9),
			value = SourceData.GetInt(10),
			recipeKey = SourceData.GetStringArray(11),
			factory = SourceData.GetStringArray(12),
			components = SourceData.GetStringArray(13),
			defMat = SourceData.GetString(14),
			category = SourceData.GetString(15),
			tag = SourceData.GetStringArray(16),
			detail_JP = SourceData.GetString(17),
			detail = SourceData.GetString(18)
		};
	}

	public override void SetRow(SourceCellEffect.Row r)
	{
		this.map[r.id] = r;
	}

	public override void OnAfterImportData()
	{
		int num = 0;
		foreach (SourceCellEffect.Row row in this.rows)
		{
			if (row.sort != 0)
			{
				num = row.sort;
			}
			row.sort = num;
			num++;
		}
		this.rows.Sort((SourceCellEffect.Row a, SourceCellEffect.Row b) => a.id - b.id);
	}

	public override void OnInit()
	{
		SourceCellEffect.FallbackRenderData = ResourceCache.Load<RenderData>("Scene/Render/Data/liquid");
		Cell.effectList = this.rows;
		foreach (SourceCellEffect.Row row in this.rows)
		{
			row.Init();
		}
	}

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
				return "l" + this.id.ToString();
			}
		}

		public override RenderData defaultRenderData
		{
			get
			{
				return SourceCellEffect.FallbackRenderData;
			}
		}

		public override int GetTile(SourceMaterial.Row mat, int dir = 0)
		{
			return this._tiles[0] + 3;
		}

		public int[] anime;
	}
}
