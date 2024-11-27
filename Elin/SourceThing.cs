using System;
using System.Collections.Generic;

public class SourceThing : SourceDataString<SourceThing.Row>
{
	public override SourceThing.Row CreateRow()
	{
		return new SourceThing.Row
		{
			id = SourceData.GetString(0),
			name_JP = SourceData.GetString(1),
			unknown_JP = SourceData.GetString(2),
			unit_JP = SourceData.GetString(3),
			naming = SourceData.GetString(4),
			name = SourceData.GetString(5),
			unit = SourceData.GetString(6),
			unknown = SourceData.GetString(7),
			category = SourceData.GetString(8),
			sort = SourceData.GetInt(10),
			_tileType = SourceData.GetString(11),
			_idRenderData = SourceData.GetString(12),
			tiles = SourceData.GetIntArray(13),
			altTiles = SourceData.GetIntArray(14),
			anime = SourceData.GetIntArray(15),
			skins = SourceData.GetIntArray(16),
			size = SourceData.GetIntArray(17),
			colorMod = SourceData.GetInt(18),
			colorType = SourceData.GetString(19),
			recipeKey = SourceData.GetStringArray(20),
			factory = SourceData.GetStringArray(21),
			components = SourceData.GetStringArray(22),
			disassemble = SourceData.GetStringArray(23),
			defMat = SourceData.GetString(24),
			tierGroup = SourceData.GetString(25),
			value = SourceData.GetInt(26),
			LV = SourceData.GetInt(27),
			chance = SourceData.GetInt(28),
			quality = SourceData.GetInt(29),
			HP = SourceData.GetInt(30),
			weight = SourceData.GetInt(31),
			electricity = SourceData.GetInt(32),
			trait = SourceData.GetStringArray(33),
			elements = Core.ParseElements(SourceData.GetStr(34, false)),
			range = SourceData.GetInt(35),
			attackType = SourceData.GetString(36),
			offense = SourceData.GetIntArray(37),
			substats = SourceData.GetIntArray(38),
			defense = SourceData.GetIntArray(39),
			lightData = SourceData.GetString(40),
			idExtra = SourceData.GetString(41),
			idToggleExtra = SourceData.GetString(42),
			idActorEx = SourceData.GetString(43),
			idSound = SourceData.GetString(44),
			tag = SourceData.GetStringArray(45),
			workTag = SourceData.GetString(46),
			filter = SourceData.GetStringArray(47),
			roomName_JP = SourceData.GetStringArray(48),
			roomName = SourceData.GetStringArray(49),
			detail_JP = SourceData.GetString(50),
			detail = SourceData.GetString(51)
		};
	}

	public override void SetRow(SourceThing.Row r)
	{
		this.map[r.id] = r;
	}

	public override void BackupPref()
	{
		this._rows.Clear();
		foreach (SourceThing.Row row in this.rows)
		{
			this._rows[row.id] = row;
		}
	}

	public override void RestorePref()
	{
	}

	public override string[] ImportFields
	{
		get
		{
			return new string[]
			{
				"unit",
				"unknown",
				"roomName",
				"name2"
			};
		}
	}

	public Dictionary<string, SourceThing.Row> _rows = new Dictionary<string, SourceThing.Row>();

	[Serializable]
	public class Row : CardRow
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

		public override string RecipeID
		{
			get
			{
				return this.id;
			}
		}

		public override void OnImportData(SourceData data)
		{
			base.OnImportData(data);
			this._altTiles = new int[0];
		}

		public override void SetTiles()
		{
			if (!this.renderData || !this.renderData.pass)
			{
				return;
			}
			base.SetTiles();
			if (this._altTiles.Length != this.altTiles.Length)
			{
				this._altTiles = new int[this.altTiles.Length];
				int num = 0;
				if (this.origin != null && !this.ignoreAltFix)
				{
					num = this._tiles[0] - this.origin._tiles[0];
				}
				for (int i = 0; i < this.altTiles.Length; i++)
				{
					this._altTiles[i] = this.altTiles[i] / 100 * (int)this.renderData.pass.pmesh.tiling.x + this.altTiles[i] % 100 + num;
				}
			}
		}

		public override string GetName(SourceMaterial.Row mat, int sum)
		{
			if (this.naming == "m")
			{
				return base.GetName(mat, sum);
			}
			if (this.naming == "ma")
			{
				return mat.GetName() + " (" + sum.ToString() + ")";
			}
			return this.GetName() + " (" + sum.ToString() + ")";
		}

		public override string GetName()
		{
			string text = base.GetText("name", false);
			if (Lang.setting.nameStyle == 0)
			{
				return text;
			}
			if (!this.unit.IsEmpty())
			{
				return "_of".lang(text, this.unit, null, null, null);
			}
			return text;
		}

		public override string GetSearchName(bool jp)
		{
			if (jp)
			{
				string result;
				if ((result = this._nameSearchJP) == null)
				{
					result = (this._nameSearchJP = base.GetText("name", false).ToLower());
				}
				return result;
			}
			string result2;
			if ((result2 = this._nameSearch) == null)
			{
				result2 = (this._nameSearch = (this.unit.IsEmpty() ? this.name : (this.unit + " of " + this.name)).ToLower());
			}
			return result2;
		}

		public string unknown_JP;

		public string unit_JP;

		public string naming;

		public string unit;

		public string unknown;

		public int[] altTiles;

		public int[] anime;

		public string[] disassemble;

		public int HP;

		public int weight;

		public int electricity;

		public int range;

		public string attackType;

		public int[] offense;

		public int[] substats;

		public int[] defense;

		public string idToggleExtra;

		public string idActorEx;

		public string workTag;

		public string[] roomName_JP;

		public string[] roomName;

		public int[] _altTiles;

		public bool ignoreAltFix;

		public string name_L;

		public string detail_L;

		public string unit_L;

		public string unknown_L;

		public string[] name2_L;

		public string[] roomName_L;
	}
}
