using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using UnityEngine;

public class SerializedCards : EClass
{
	public void Add(Card c)
	{
		SerializedCards.<>c__DisplayClass4_0 CS$<>8__locals1;
		CS$<>8__locals1.c = c;
		CS$<>8__locals1.data = new SerializedCards.Data
		{
			id = CS$<>8__locals1.c.id,
			idEditor = CS$<>8__locals1.c.c_idEditor,
			idRefCard = CS$<>8__locals1.c.c_idRefCard,
			idTrait = CS$<>8__locals1.c.c_idTrait,
			tags = CS$<>8__locals1.c.c_editorTags,
			traitVals = CS$<>8__locals1.c.c_editorTraitVal,
			obj = CS$<>8__locals1.c.mapObj,
			idMat = CS$<>8__locals1.c.material.id,
			x = CS$<>8__locals1.c.pos.x,
			z = CS$<>8__locals1.c.pos.z,
			placeState = (int)CS$<>8__locals1.c.placeState,
			dir = CS$<>8__locals1.c.dir,
			altitude = CS$<>8__locals1.c.altitude,
			fx = (int)(CS$<>8__locals1.c.fx * 1000f),
			fy = (int)(CS$<>8__locals1.c.fy * 1000f),
			lightColor = CS$<>8__locals1.c.c_lightColor,
			bits1 = CS$<>8__locals1.c._bits1.ToInt(),
			bits2 = CS$<>8__locals1.c._bits2.ToInt(),
			tile = CS$<>8__locals1.c.sourceCard.tiles[0],
			idRender = CS$<>8__locals1.c.sourceCard.idRenderData,
			refVal = CS$<>8__locals1.c.refVal,
			idSkin = CS$<>8__locals1.c.idSkin,
			idDeity = CS$<>8__locals1.c.c_idDeity
		};
		if (CS$<>8__locals1.c.c_idBacker != 0)
		{
			SourceBacker.Row row = EClass.sources.backers.map.TryGetValue(CS$<>8__locals1.c.c_idBacker, null);
			if (row != null && row.isStatic != 0)
			{
				CS$<>8__locals1.data.idBacker = CS$<>8__locals1.c.c_idBacker;
			}
		}
		if (CS$<>8__locals1.c.material.id == CS$<>8__locals1.c.DefaultMaterial.id)
		{
			CS$<>8__locals1.data.idMat = -1;
		}
		CS$<>8__locals1.data.idDyeMat = (CS$<>8__locals1.c.isDyed ? CS$<>8__locals1.c.c_dyeMat : -1);
		if (CS$<>8__locals1.c.isChara)
		{
			Point orgPos = CS$<>8__locals1.c.Chara.orgPos;
			if (orgPos != null)
			{
				CS$<>8__locals1.data.x = orgPos.x;
				CS$<>8__locals1.data.z = orgPos.z;
			}
			if (CS$<>8__locals1.c.Chara.isDead)
			{
				CS$<>8__locals1.data.isDead = true;
			}
			if (CS$<>8__locals1.c.Chara.uidEditor == 0)
			{
				int num = 1;
				foreach (Chara chara in EClass._map.charas.Concat(EClass._map.deadCharas))
				{
					if (chara.uidEditor >= num)
					{
						num = chara.uidEditor + 1;
					}
				}
				CS$<>8__locals1.c.Chara.uidEditor = num;
			}
			CS$<>8__locals1.data.lv = CS$<>8__locals1.c.LV;
			CS$<>8__locals1.data.element = CS$<>8__locals1.c.c_idMainElement;
			CS$<>8__locals1.data.uidEditor = CS$<>8__locals1.c.Chara.uidEditor;
		}
		SerializedCards.<Add>g__TryAddStr|4_0(52, ref CS$<>8__locals1);
		this.cards.Add(CS$<>8__locals1.data);
		CS$<>8__locals1.data.ints[0] = CS$<>8__locals1.data._bits1.ToInt();
	}

	public void Restore(Map map, Map orgMap, bool addToZone, PartialMap partial = null)
	{
		List<Thing> things = map.things;
		List<Chara> serializedCharas = map.serializedCharas;
		bool isUserZone = EClass._zone.IsUserZone;
		this.importedCards.Clear();
		foreach (SerializedCards.Data data in this.cards)
		{
			int num = data.dir;
			Point point = new Point(data.x, data.z);
			if (partial != null)
			{
				if (partial.result.ruined.Contains(point.index))
				{
					continue;
				}
				num -= partial.dir;
				int num2 = point.x - partial.offsetX;
				int num3 = point.z - partial.offsetZ;
				switch (partial.dir)
				{
				case 1:
				{
					int num4 = num2;
					num2 = num3;
					num3 = -num4;
					break;
				}
				case 2:
					num2 = -num2;
					num3 = -num3;
					break;
				case 3:
				{
					int num5 = num2;
					num2 = -num3;
					num3 = num5;
					break;
				}
				}
				point.x = num2 + partial.destX;
				point.z = num3 + partial.destZ;
				if (!partial.validPoints.Contains(point.index) || !point.IsValid)
				{
					continue;
				}
				if (!partial.editMode)
				{
					Card card = null;
					string id = data.id;
					if (!(id == "sign_spawnThing"))
					{
						if (id == "sign_spawnChara")
						{
							card = CharaGen.CreateFromFilter(data.idRefCard.IsEmpty(EClass._zone.biome.spawn.GetRandomCharaId()), EClass._zone.DangerLv, -1);
						}
					}
					else
					{
						card = ThingGen.CreateFromFilter(data.idRefCard.IsEmpty(EClass._zone.biome.spawn.GetRandomThingId()), EClass._zone.DangerLv);
					}
					if (card != null)
					{
						card.pos = point;
						EClass._zone.AddCard(card, card.pos);
						if (card.trait.IsDoor)
						{
							EClass._map.OnSetBlockOrDoor(card.pos.x, card.pos.z);
							continue;
						}
						continue;
					}
				}
			}
			data._bits1.SetInt(data.ints[0]);
			string text = data.id;
			if (data.idV != 0)
			{
				text = (data.idV.ToString() ?? "");
			}
			if (addToZone && partial != null && !partial.editMode)
			{
				string id = data.id;
				if (!(id == "editor_torch"))
				{
					if (id == "editor_torch_wall")
					{
						text = EClass._zone.biome.style.GetIdLight(true);
					}
				}
				else
				{
					text = EClass._zone.biome.style.GetIdLight(false);
				}
			}
			CardRow source = EClass.sources.cards.map.TryGetValue(text, null);
			if (source != null)
			{
				Card card2 = null;
				if (source.isChara)
				{
					if (orgMap != null)
					{
						bool flag = false;
						if (source.quality >= 4)
						{
							using (Dictionary<int, Chara>.ValueCollection.Enumerator enumerator2 = EClass.game.cards.globalCharas.Values.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									if (enumerator2.Current.id == text)
									{
										flag = true;
										break;
									}
								}
							}
						}
						if (!flag)
						{
							foreach (Chara chara in orgMap.charas.Concat(orgMap.serializedCharas).Concat(orgMap.deadCharas))
							{
								if (source.quality >= 4 && chara.id == text)
								{
									flag = true;
									break;
								}
								if (chara.id == text && chara.orgPos != null && chara.orgPos.Equals(point))
								{
									flag = true;
									break;
								}
							}
						}
						if (flag)
						{
							continue;
						}
					}
					card2 = CharaGen.Create(text, -1);
					if (data.ints.Length > 20)
					{
						card2.Chara.SetLv(data.lv);
						if (data.element != 0)
						{
							card2.Chara.SetMainElement(data.element, 0, false);
						}
					}
					card2.Chara.orgPos = point.Copy();
					if (data.isDead)
					{
						card2.hp = -1;
						card2.Chara.isDead = true;
					}
					card2.Chara.hunger.value = EClass.rnd(EClass.rnd(20) + 1);
					if (!addToZone)
					{
						serializedCharas.Add(card2.Chara);
					}
				}
				else
				{
					PlaceState placeState = data.placeState.ToEnum<PlaceState>();
					if (isUserZone && placeState != PlaceState.installed)
					{
						continue;
					}
					if (source.isOrigin)
					{
						text = SpawnListThing.Get("origin_" + text, (SourceThing.Row a) => a.origin == source).GetFirst().id;
					}
					card2 = ThingGen.Create(text, -1, EClass._zone.DangerLv);
					card2.ChangeMaterial((data.idMat == -1) ? card2.DefaultMaterial.id : data.idMat);
					if (!addToZone)
					{
						things.Add(card2.Thing);
					}
					card2.altitude = data.altitude;
					card2.placeState = placeState;
					card2.c_lightColor = data.lightColor;
				}
				if (num < 0)
				{
					num = Mathf.Abs(card2.sourceCard.tiles.Length + num);
				}
				card2.pos = point;
				card2.dir = num;
				card2._bits1.SetInt(data.bits1);
				card2._bits2.SetInt(data.bits2);
				card2.isPlayerCreation = true;
				card2.autoRefuel = true;
				card2.c_editorTraitVal = data.traitVals;
				card2.c_idRefCard = data.idRefCard;
				card2.isImported = true;
				card2.refVal = data.refVal;
				card2.idSkin = data.idSkin;
				card2.c_idDeity = data.idDeity;
				if (data.idBacker != 0)
				{
					Debug.Log(data.idBacker);
					card2.c_idBacker = data.idBacker;
				}
				if (this.version >= 2 && data.idDyeMat != -1)
				{
					card2.Dye(EClass.sources.materials.rows[data.idDyeMat]);
				}
				card2.mapObj = data.obj;
				Dictionary<int, object> mapObj = card2.mapObj;
				if (((mapObj != null) ? mapObj.TryGetValue(2, null) : null) != null)
				{
					card2.mapObj.Remove(2);
				}
				if (data.cstr != null)
				{
					foreach (KeyValuePair<int, string> keyValuePair in data.cstr)
					{
						card2.SetStr(keyValuePair.Key, keyValuePair.Value);
					}
				}
				if (card2.freePos)
				{
					card2.fx = (float)data.fx * 0.001f;
					card2.fy = (float)data.fy * 0.001f;
				}
				if (!data.idEditor.IsEmpty())
				{
					card2.c_idEditor = data.idEditor;
				}
				if (!data.idTrait.IsEmpty())
				{
					card2.c_idTrait = data.idTrait;
					card2.ApplyTrait();
					card2.trait.OnCreate(EClass._zone.lv);
				}
				if (!data.tags.IsEmpty())
				{
					card2.c_editorTags = data.tags;
					try
					{
						foreach (string value in data.tags.Split(',', StringSplitOptions.None))
						{
							card2.ApplyEditorTags(value.ToEnum(true));
						}
					}
					catch
					{
						Debug.LogWarning("Could not convert editor tag:" + card2.Name + "/" + data.tags);
					}
				}
				if (card2.isChara)
				{
					card2.Chara.homeZone = EClass._zone;
					card2.Chara.uidEditor = data.uidEditor;
					if (card2.isBackerContent)
					{
						card2.ApplyBacker(card2.c_idBacker);
					}
				}
				if (addToZone)
				{
					EClass._zone.AddCard(card2, card2.pos);
					if (card2.trait.IsDoor)
					{
						EClass._map.OnSetBlockOrDoor(card2.pos.x, card2.pos.z);
					}
					if (partial != null && !card2.sourceCard.lightData.IsEmpty())
					{
						partial.result.hasLight = true;
					}
				}
				this.importedCards.Add(card2);
			}
		}
		foreach (Card card3 in this.importedCards)
		{
			if (card3.trait is TraitShackle)
			{
				foreach (Card card4 in this.importedCards)
				{
					if (card4.isRestrained && card4.pos.Equals(card3.pos))
					{
						card3.c_uidRefCard = card4.uid;
					}
				}
			}
			card3.trait.OnImportMap();
		}
	}

	[CompilerGenerated]
	internal static void <Add>g__TryAddStr|4_0(int key, ref SerializedCards.<>c__DisplayClass4_0 A_1)
	{
		string str = A_1.c.GetStr(key, null);
		if (!str.IsEmpty())
		{
			if (A_1.data.cstr == null)
			{
				A_1.data.cstr = new Dictionary<int, string>();
			}
			A_1.data.cstr[key] = str;
		}
	}

	[JsonProperty]
	public List<SerializedCards.Data> cards = new List<SerializedCards.Data>();

	[JsonProperty]
	public int version = 2;

	public List<Card> importedCards = new List<Card>();

	public class Data : EClass
	{
		public int idDyeMat
		{
			get
			{
				return this.ints[2];
			}
			set
			{
				this.ints[2] = value;
			}
		}

		public int idV
		{
			get
			{
				return this.ints[3];
			}
			set
			{
				this.ints[3] = value;
			}
		}

		public int idMat
		{
			get
			{
				return this.ints[4];
			}
			set
			{
				this.ints[4] = value;
			}
		}

		public int x
		{
			get
			{
				return this.ints[5];
			}
			set
			{
				this.ints[5] = value;
			}
		}

		public int z
		{
			get
			{
				return this.ints[6];
			}
			set
			{
				this.ints[6] = value;
			}
		}

		public int dir
		{
			get
			{
				return this.ints[7];
			}
			set
			{
				this.ints[7] = value;
			}
		}

		public int placeState
		{
			get
			{
				return this.ints[8];
			}
			set
			{
				this.ints[8] = value;
			}
		}

		public int altitude
		{
			get
			{
				return this.ints[9];
			}
			set
			{
				this.ints[9] = value;
			}
		}

		public int fx
		{
			get
			{
				return this.ints[10];
			}
			set
			{
				this.ints[10] = value;
			}
		}

		public int fy
		{
			get
			{
				return this.ints[11];
			}
			set
			{
				this.ints[11] = value;
			}
		}

		public int lightColor
		{
			get
			{
				return this.ints[12];
			}
			set
			{
				this.ints[12] = value;
			}
		}

		public int bits1
		{
			get
			{
				return this.ints[13];
			}
			set
			{
				this.ints[13] = value;
			}
		}

		public int tile
		{
			get
			{
				return this.ints[14];
			}
			set
			{
				this.ints[14] = value;
			}
		}

		public int refVal
		{
			get
			{
				return this.ints[15];
			}
			set
			{
				this.ints[15] = value;
			}
		}

		public int idSkin
		{
			get
			{
				return this.ints[16];
			}
			set
			{
				this.ints[16] = value;
			}
		}

		public int idBacker
		{
			get
			{
				return this.ints[17];
			}
			set
			{
				this.ints[17] = value;
			}
		}

		public int bits2
		{
			get
			{
				return this.ints[18];
			}
			set
			{
				this.ints[18] = value;
			}
		}

		public int uidEditor
		{
			get
			{
				return this.ints[19];
			}
			set
			{
				this.ints[19] = value;
			}
		}

		public int lv
		{
			get
			{
				return this.ints[20];
			}
			set
			{
				this.ints[20] = value;
			}
		}

		public int element
		{
			get
			{
				return this.ints[21];
			}
			set
			{
				this.ints[21] = value;
			}
		}

		public string id
		{
			get
			{
				return this.strs[0];
			}
			set
			{
				this.strs[0] = value;
			}
		}

		public string idEditor
		{
			get
			{
				return this.strs[1];
			}
			set
			{
				this.strs[1] = value;
			}
		}

		public string idTrait
		{
			get
			{
				return this.strs[2];
			}
			set
			{
				this.strs[2] = value;
			}
		}

		public string tags
		{
			get
			{
				return this.strs[3];
			}
			set
			{
				this.strs[3] = value;
			}
		}

		public string idRender
		{
			get
			{
				return this.strs[4];
			}
			set
			{
				this.strs[4] = value;
			}
		}

		public string traitVals
		{
			get
			{
				return this.strs[5];
			}
			set
			{
				this.strs[5] = value;
			}
		}

		public string idRefCard
		{
			get
			{
				return this.strs[6];
			}
			set
			{
				this.strs[6] = value;
			}
		}

		public string idDeity
		{
			get
			{
				return this.strs[7];
			}
			set
			{
				this.strs[7] = value;
			}
		}

		public bool isDead
		{
			get
			{
				return this._bits1[0];
			}
			set
			{
				this._bits1[0] = value;
			}
		}

		[JsonProperty]
		public int[] ints = new int[30];

		[JsonProperty]
		public string[] strs = new string[10];

		[JsonProperty]
		public Dictionary<int, object> obj;

		[JsonProperty]
		public Dictionary<int, string> cstr;

		public BitArray32 _bits1;
	}
}
