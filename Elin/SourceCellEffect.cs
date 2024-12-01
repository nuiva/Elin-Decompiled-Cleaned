using System;

public class SourceCellEffect : SourceDataInt<SourceCellEffect.Row>
{
	[Serializable]
	public class Row : TileRow
	{
		public int[] anime;

		public override bool UseAlias => true;

		public override string GetAlias => alias;

		public override string RecipeID => "l" + id;

		public override RenderData defaultRenderData => FallbackRenderData;

		public override int GetTile(SourceMaterial.Row mat, int dir = 0)
		{
			return _tiles[0] + 3;
		}
	}

	public static RenderData FallbackRenderData;

	public override Row CreateRow()
	{
		return new Row
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

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}

	public override void OnAfterImportData()
	{
		int num = 0;
		foreach (Row row in rows)
		{
			if (row.sort != 0)
			{
				num = row.sort;
			}
			row.sort = num;
			num++;
		}
		rows.Sort((Row a, Row b) => a.id - b.id);
	}

	public override void OnInit()
	{
		FallbackRenderData = ResourceCache.Load<RenderData>("Scene/Render/Data/liquid");
		Cell.effectList = rows;
		foreach (Row row in rows)
		{
			row.Init();
		}
	}
}
