using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class SourceThingV : SourceDataString<SourceThingV.Row>
{
	public override SourceThingV.Row CreateRow()
	{
		return new SourceThingV.Row
		{
			id = SourceData.GetString(0),
			_origin = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			unit_JP = SourceData.GetString(3),
			name = SourceData.GetString(4),
			unit = SourceData.GetString(5),
			tiles = SourceData.GetIntArray(6),
			skins = SourceData.GetIntArray(7),
			parse = SourceData.GetStringArray(8),
			vals = SourceData.GetStringArray(9),
			trait = SourceData.GetStringArray(11),
			LV = SourceData.GetInt(12),
			chance = SourceData.GetInt(13),
			value = SourceData.GetInt(14),
			weight = SourceData.GetInt(15),
			recipeKey = SourceData.GetStringArray(16),
			factory = SourceData.GetStringArray(17),
			components = SourceData.GetStringArray(18),
			defMat = SourceData.GetString(19),
			category = SourceData.GetString(20),
			tag = SourceData.GetStringArray(21),
			detail_JP = SourceData.GetString(22),
			detail = SourceData.GetString(23)
		};
	}

	public override void SetRow(SourceThingV.Row r)
	{
		this.map[r.id] = r;
	}

	public override void RestorePref()
	{
		foreach (SourceThing.Row row in EClass.sources.things.rows)
		{
			SourceThing.Row row2 = EClass.sources.things._rows.TryGetValue(row.id, null);
			SourcePref sourcePref = (row2 != null) ? row2.pref : null;
			if (sourcePref == null && EClass.sources.asset.renames.ContainsKey(row.id))
			{
				SourceThing.Row row3 = EClass.sources.things._rows.TryGetValue(EClass.sources.asset.renames[row.id], null);
				sourcePref = ((row3 != null) ? row3.pref : null);
			}
			row.pref = (sourcePref ?? new SourcePref());
		}
		Dictionary<string, SourceThing.Row> dictionary = new Dictionary<string, SourceThing.Row>();
		foreach (SourceThing.Row row4 in EClass.sources.things.rows)
		{
			dictionary[row4.id] = row4;
		}
		foreach (SourceThing.Row row5 in EClass.sources.things.rows)
		{
			if (!row5.pref.UsePref && !row5._origin.IsEmpty())
			{
				SourceThing.Row row6 = dictionary[row5._origin];
				row5.pref = IO.DeepCopy<SourcePref>(row6.pref);
				row5.pref.flags |= PrefFlag.UsePref;
			}
		}
	}

	public override void ValidatePref()
	{
		foreach (SourceThing.Row row in EClass.sources.things.rows)
		{
			row.pref.Validate();
		}
	}

	public override void OnAfterImportData()
	{
		Dictionary<string, SourceThing.Row> dictionary = new Dictionary<string, SourceThing.Row>();
		foreach (SourceThing.Row row in EClass.sources.things.rows)
		{
			dictionary[row.id] = row;
		}
		System.Reflection.FieldInfo[] fields = EClass.sources.things.rows[0].GetType().GetFields();
		foreach (SourceThingV.Row row2 in this.rows)
		{
			SourceThing.Row row3 = new SourceThing.Row();
			SourceThing.Row o = dictionary[row2._origin];
			foreach (System.Reflection.FieldInfo fieldInfo in fields)
			{
				if (!(fieldInfo.Name == "parse"))
				{
					row3.SetField(fieldInfo.Name, o.GetField(fieldInfo.Name));
				}
			}
			row3.id = row2.id;
			row3._origin = row2._origin;
			if (row2.LV != 0)
			{
				row3.LV = row2.LV;
			}
			if (row2.chance != -1)
			{
				row3.chance = row2.chance;
			}
			if (row2.value != -1)
			{
				row3.value = row2.value;
			}
			else
			{
				row3.value += EClass.rnd(row3.value / 2);
			}
			if (row2.weight != -1)
			{
				row3.weight = row2.weight;
			}
			if (!row2.tiles.IsEmpty())
			{
				row3.tiles = row2.tiles;
			}
			if (!row2.skins.IsEmpty())
			{
				row3.skins = row2.skins;
			}
			if (!row2.name.IsEmpty())
			{
				row3.name = row2.name;
			}
			if (!row2.name_JP.IsEmpty())
			{
				row3.name_JP = row2.name_JP;
			}
			if (!row2.detail.IsEmpty())
			{
				row3.detail = row2.detail;
			}
			if (!row2.detail_JP.IsEmpty())
			{
				row3.detail_JP = row2.detail_JP;
			}
			if (!row2.unit.IsEmpty())
			{
				row3.unit = row2.unit;
			}
			if (!row2.unit_JP.IsEmpty())
			{
				row3.unit_JP = row2.unit_JP;
			}
			if (!row2.vals.IsEmpty())
			{
				row3.vals = row2.vals;
			}
			if (!row2.components.IsEmpty())
			{
				row3.components = row2.components;
			}
			if (!row2.defMat.IsEmpty())
			{
				row3.defMat = row2.defMat;
			}
			if (!row2.trait.IsEmpty())
			{
				row3.trait = row2.trait;
			}
			if (!row2.category.IsEmpty())
			{
				row3.category = row2.category;
			}
			if (!row2.factory.IsEmpty())
			{
				row3.factory = row2.factory;
			}
			if (!row2.tag.IsEmpty())
			{
				row3.tag = row2.tag;
			}
			row3.recipeKey = row2.recipeKey;
			if (!row2.parse.IsEmpty())
			{
				string origin = row2._origin;
				if (origin == "lamp_ceil2" || origin == "window" || origin == "windowL")
				{
					row3.idExtra = row2.parse[0];
				}
				else
				{
					string[] parse = row2.parse;
					for (int i = 0; i < parse.Length; i++)
					{
						string[] array2 = parse[i].Split('/', StringSplitOptions.None);
						string text = array2[0];
						uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
						if (num <= 1260025160U)
						{
							if (num <= 455432284U)
							{
								if (num != 235771284U)
								{
									if (num == 455432284U)
									{
										if (text == "alt")
										{
											row3.altTiles = new int[]
											{
												array2[1].ToInt()
											};
											row3.ignoreAltFix = true;
										}
									}
								}
								else if (text == "sound")
								{
									row3.idSound = array2[1];
								}
							}
							else if (num != 1031692888U)
							{
								if (num != 1035877158U)
								{
									if (num == 1260025160U)
									{
										if (text == "ex")
										{
											row3.idActorEx = array2[1];
										}
									}
								}
								else if (text == "unique")
								{
									row3.quality = 4;
								}
							}
							else if (text == "color")
							{
								row3.colorMod = 100;
							}
						}
						else if (num <= 2981401199U)
						{
							if (num != 1468549011U)
							{
								if (num != 2121067537U)
								{
									if (num == 2981401199U)
									{
										if (text == "anime")
										{
											SourceThing.Row row4 = row3;
											int[] anime;
											if (array2.Length <= 4)
											{
												if (array2.Length <= 3)
												{
													int[] array3 = new int[2];
													array3[0] = array2[1].ToInt();
													anime = array3;
													array3[1] = array2[2].ToInt();
												}
												else
												{
													int[] array4 = new int[3];
													array4[0] = array2[1].ToInt();
													array4[1] = array2[2].ToInt();
													anime = array4;
													array4[2] = array2[3].ToInt();
												}
											}
											else
											{
												int[] array5 = new int[4];
												array5[0] = array2[1].ToInt();
												array5[1] = array2[2].ToInt();
												array5[2] = array2[3].ToInt();
												anime = array5;
												array5[3] = array2[4].ToInt();
											}
											row4.anime = anime;
										}
									}
								}
								else if (text == "naming")
								{
									row3.naming = array2[1];
								}
							}
							else if (text == "ele")
							{
								int[] second = new int[]
								{
									Core.GetCurrent().sources.elements.alias[array2[1]].id,
									array2[2].ToInt()
								};
								row3.elements = row3.elements.Concat(second).ToArray<int>();
							}
						}
						else if (num != 3822567969U)
						{
							if (num != 3933629262U)
							{
								if (num == 4009327117U)
								{
									if (text == "render")
									{
										row3._idRenderData = array2[1];
									}
								}
							}
							else if (text == "no_color")
							{
								row3.colorMod = 0;
							}
						}
						else if (text == "tiletype")
						{
							row3._tileType = array2[1];
						}
					}
				}
			}
			this.OnImportRow(row2, row3);
			row3.OnImportData(EClass.sources.things);
			EClass.sources.things.rows.Add(row3);
		}
		this.rows.Clear();
	}

	public virtual void OnImportRow(SourceThingV.Row _r, SourceThing.Row c)
	{
	}

	public override string[] ImportFields
	{
		get
		{
			return new string[]
			{
				"unit"
			};
		}
	}

	[Serializable]
	public class Row : SourceThing.Row
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

		public string[] parse;
	}
}
