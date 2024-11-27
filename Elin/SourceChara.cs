using System;
using System.Collections.Generic;

public class SourceChara : SourceDataString<SourceChara.Row>
{
	public override SourceChara.Row CreateRow()
	{
		return new SourceChara.Row
		{
			id = SourceData.GetString(0),
			_id = SourceData.GetInt(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			aka_JP = SourceData.GetString(4),
			aka = SourceData.GetString(5),
			idActor = SourceData.GetStringArray(6),
			sort = SourceData.GetInt(7),
			size = SourceData.GetIntArray(8),
			_idRenderData = SourceData.GetString(9),
			tiles = SourceData.GetIntArray(10),
			tiles_snow = SourceData.GetIntArray(11),
			colorMod = SourceData.GetInt(12),
			components = SourceData.GetStringArray(13),
			defMat = SourceData.GetString(14),
			LV = SourceData.GetInt(15),
			chance = SourceData.GetInt(16),
			quality = SourceData.GetInt(17),
			hostility = SourceData.GetString(18),
			biome = SourceData.GetString(19),
			tag = SourceData.GetStringArray(20),
			trait = SourceData.GetStringArray(21),
			race = SourceData.GetString(22),
			job = SourceData.GetString(23),
			tactics = SourceData.GetString(24),
			aiIdle = SourceData.GetString(25),
			aiParam = SourceData.GetIntArray(26),
			actCombat = SourceData.GetStringArray(27),
			mainElement = SourceData.GetStringArray(28),
			elements = Core.ParseElements(SourceData.GetStr(29, false)),
			equip = SourceData.GetString(30),
			loot = SourceData.GetStringArray(31),
			category = SourceData.GetString(32),
			filter = SourceData.GetStringArray(33),
			gachaFilter = SourceData.GetStringArray(34),
			tone = SourceData.GetString(35),
			actIdle = SourceData.GetStringArray(36),
			lightData = SourceData.GetString(37),
			idExtra = SourceData.GetString(38),
			bio = SourceData.GetString(39),
			faith = SourceData.GetString(40),
			works = SourceData.GetStringArray(41),
			hobbies = SourceData.GetStringArray(42),
			idText = SourceData.GetString(43),
			moveAnime = SourceData.GetString(44),
			factory = SourceData.GetStringArray(45),
			components = SourceData.GetStringArray(46),
			detail_JP = SourceData.GetString(47),
			detail = SourceData.GetString(48)
		};
	}

	public override void SetRow(SourceChara.Row r)
	{
		this.map[r.id] = r;
	}

	public override void BackupPref()
	{
		this._rows.Clear();
		foreach (SourceChara.Row row in this.rows)
		{
			this._rows[row.id] = row;
		}
	}

	public override void RestorePref()
	{
		foreach (SourceChara.Row row in this.rows)
		{
			RenderRow renderRow = row;
			SourceChara.Row row2 = this._rows.TryGetValue(row.id, null);
			renderRow.pref = (((row2 != null) ? row2.pref : null) ?? new SourcePref());
		}
	}

	public override void ValidatePref()
	{
		foreach (SourceChara.Row row in this.rows)
		{
			row.pref.Validate();
		}
	}

	public override string[] ImportFields
	{
		get
		{
			return new string[]
			{
				"aka"
			};
		}
	}

	public Dictionary<string, SourceChara.Row> _rows = new Dictionary<string, SourceChara.Row>();

	public static SourceChara.Row rowDefaultPCC;

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

		public SourceRace.Row race_row
		{
			get
			{
				SourceRace.Row result;
				if ((result = this._race_row) == null)
				{
					result = (this._race_row = base.sources.races.map[this.race]);
				}
				return result;
			}
		}

		public override string RecipeID
		{
			get
			{
				return this.id;
			}
		}

		public override string GetSearchName(bool jp)
		{
			string result;
			if (!jp)
			{
				if ((result = this._nameSearch) == null)
				{
					return this._nameSearch = (this.name.StartsWith("*") ? this.aka : this.name).ToLower();
				}
			}
			else if ((result = this._nameSearchJP) == null)
			{
				result = (this._nameSearchJP = (this.name_JP.StartsWith("*") ? base.GetText("aka", false) : base.GetText("name", false)).ToLower());
			}
			return result;
		}

		public override void SetTiles()
		{
			base.SetTiles();
			this.staticSkin = base.HasTag(CTAG.staticSkin);
			if (!this.renderData || !this.renderData.pass)
			{
				return;
			}
			if (this._tiles_snow == null || this._tiles_snow.Length != this.tiles_snow.Length)
			{
				this._tiles_snow = new int[this.tiles_snow.Length];
				for (int i = 0; i < this.tiles_snow.Length; i++)
				{
					this._tiles_snow[i] = this.tiles_snow[i] / 100 * (int)this.renderData.pass.pmesh.tiling.x + this.tiles_snow[i] % 100;
				}
			}
		}

		public override string GetName()
		{
			string text = base.GetText("name", false);
			text = text.Replace("#ele4", "").Replace("#ele3", "").Replace("#ele2", "").Replace("#ele", "");
			if (text == "*r")
			{
				text = base.GetText("aka", false);
			}
			if (text[0] == ' ')
			{
				text = text.TrimStart(' ');
			}
			return text;
		}

		public int _id;

		public string aka_JP;

		public string aka;

		public int[] tiles_snow;

		public string hostility;

		public string biome;

		public string race;

		public string job;

		public string tactics;

		public string aiIdle;

		public int[] aiParam;

		public string[] actCombat;

		public string[] mainElement;

		public string equip;

		public string[] gachaFilter;

		public string tone;

		public string[] actIdle;

		public string bio;

		public string faith;

		public string[] works;

		public string[] hobbies;

		public string idText;

		public string moveAnime;

		public bool staticSkin;

		public int[] _tiles_snow;

		[NonSerialized]
		public SourceRace.Row _race_row;

		public string name_L;

		public string detail_L;

		public string aka_L;
	}
}
