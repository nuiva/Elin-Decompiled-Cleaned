using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Dungen;
using Ionic.Zip;
using Newtonsoft.Json;
using SFB;
using UnityEngine;

public class Zone : Spatial, ICardParent, IInspect
{
	public Chara Boss
	{
		get
		{
			return EClass._map.FindChara(base.uidBoss);
		}
		set
		{
			base.uidBoss = ((value != null) ? value.uid : 0);
		}
	}

	public override int DangerLv
	{
		get
		{
			if (this.GetTopZone() != this)
			{
				return this.GetTopZone().DangerLv + Mathf.Abs(base.lv) - 1;
			}
			return (int)Mathf.Max(1f, (float)base._dangerLv + MathF.Abs((float)base.lv) + (float)this.DangerLvFix);
		}
	}

	public virtual bool DisableRooms
	{
		get
		{
			return false;
		}
	}

	public int HourSinceLastActive
	{
		get
		{
			return EClass.world.date.GetElapsedHour(base.lastActive);
		}
	}

	public int MinsSinceLastActive
	{
		get
		{
			return EClass.world.date.GetElapsedMins(base.lastActive);
		}
	}

	public virtual string pathExport
	{
		get
		{
			return CorePath.ZoneSave + this.idExport.IsEmpty("_new") + ".z";
		}
	}

	public BiomeProfile biome
	{
		get
		{
			BiomeProfile result;
			if ((result = this._biome) == null)
			{
				result = (this._biome = EClass.core.refs.biomes.dict[this.IdBiome]);
			}
			return result;
		}
	}

	public virtual string IdBiome
	{
		get
		{
			return this.map.config.idBiome.IsEmpty(base.source.idBiome);
		}
	}

	public virtual string GetDungenID()
	{
		return null;
	}

	public virtual string IDGenerator
	{
		get
		{
			return null;
		}
	}

	public virtual string TextWidgetDate
	{
		get
		{
			return "";
		}
	}

	public MapGenerator Generator
	{
		get
		{
			return ResourceCache.Load<MapGenerator>("DunGenProfile/Generator_" + this.GetDungenID());
		}
	}

	public virtual string IdProfile
	{
		get
		{
			return this.idProfile.IsEmpty(base.source.idProfile);
		}
	}

	public virtual string IDPlayList
	{
		get
		{
			return base.source.idPlaylist.IsEmpty((base.lv != 0) ? "Underground" : null);
		}
	}

	public virtual string IDPlaylistOverwrite
	{
		get
		{
			return null;
		}
	}

	public virtual string IDHat
	{
		get
		{
			return null;
		}
	}

	public virtual string IDBaseLandFeat
	{
		get
		{
			return base.Tile.source.trait[0];
		}
	}

	public virtual string idExport
	{
		get
		{
			if (base.source.idFile.Length != 0)
			{
				return base.source.idFile[this.fileVariation] + ((base.lv == 0) ? "" : ("_F" + base.lv.ToString()));
			}
			return "";
		}
	}

	public string pathTemp
	{
		get
		{
			return GameIO.pathTemp + base.uid.ToString() + "/";
		}
	}

	public Region Region
	{
		get
		{
			return (this as Region) ?? (this.parent as Region);
		}
	}

	public Zone ParentZone
	{
		get
		{
			return this.parent as Zone;
		}
	}

	public virtual ActionMode DefaultActionMode
	{
		get
		{
			return ActionMode.Adv;
		}
	}

	public virtual bool BlockBorderExit
	{
		get
		{
			return base.lv != 0;
		}
	}

	public virtual int ExpireDays
	{
		get
		{
			return EClass.setting.balance.dateExpireRandomMap;
		}
	}

	public virtual ZoneTransition.EnterState RegionEnterState
	{
		get
		{
			return ZoneTransition.EnterState.Dir;
		}
	}

	public virtual ZoneFeatureType FeatureType
	{
		get
		{
			return ZoneFeatureType.Default;
		}
	}

	public virtual string IDAmbience
	{
		get
		{
			if (this.IsRegion || this.map.IsIndoor)
			{
				return null;
			}
			if (!this.IsTown)
			{
				return "forest";
			}
			return "town";
		}
	}

	public virtual string IDSceneTemplate
	{
		get
		{
			if (EClass._map.IsIndoor)
			{
				return "Indoor";
			}
			if (!this.IsSnowZone)
			{
				return "Default";
			}
			return "Snow";
		}
	}

	public virtual bool IsFestival
	{
		get
		{
			return false;
		}
	}

	public virtual string IDSubset
	{
		get
		{
			if (!this.IsFestival)
			{
				return null;
			}
			return "festival";
		}
	}

	public virtual bool IsTown
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanBeDeliverDestination
	{
		get
		{
			return this.IsTown;
		}
	}

	public virtual bool CountDeepestLevel
	{
		get
		{
			return false;
		}
	}

	public virtual bool HasLaw
	{
		get
		{
			return false;
		}
	}

	public virtual bool MakeEnemiesNeutral
	{
		get
		{
			return this.IsTown || this.HasLaw;
		}
	}

	public virtual bool MakeTownProperties
	{
		get
		{
			return this.HasLaw;
		}
	}

	public virtual bool AllowCriminal
	{
		get
		{
			return !this.HasLaw;
		}
	}

	public virtual bool AllowNewZone
	{
		get
		{
			return true;
		}
	}

	public virtual bool WillAutoSave
	{
		get
		{
			return true;
		}
	}

	public virtual bool RegenerateOnEnter
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsSkyLevel
	{
		get
		{
			return base.lv > 0;
		}
	}

	public virtual bool IsUserZone
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanDigUnderground
	{
		get
		{
			return false;
		}
	}

	public virtual bool LockExit
	{
		get
		{
			return true;
		}
	}

	public virtual bool CanUnlockExit
	{
		get
		{
			return !this.LockExit;
		}
	}

	public virtual int MaxLv
	{
		get
		{
			return 99999;
		}
	}

	public virtual int MinLv
	{
		get
		{
			return -99999;
		}
	}

	public virtual bool AddPrefix
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsNefia
	{
		get
		{
			return false;
		}
	}

	public virtual bool PetFollow
	{
		get
		{
			return true;
		}
	}

	public virtual bool RestrictBuild
	{
		get
		{
			return this is Zone_Civilized && !this.IsPCFaction;
		}
	}

	public virtual bool KeepAllyDistance
	{
		get
		{
			return this.HasLaw || this.IsPCFaction;
		}
	}

	public virtual int MaxSpawn
	{
		get
		{
			return this.map.bounds.Width * this.map.bounds.Height / 175 + 2;
		}
	}

	public int MaxRespawn
	{
		get
		{
			return (int)((float)this.MaxSpawn * this.RespawnRate) + 1;
		}
	}

	public virtual float RuinChance
	{
		get
		{
			return 0.1f;
		}
	}

	public virtual float OreChance
	{
		get
		{
			return 1f;
		}
	}

	public virtual float BigDaddyChance
	{
		get
		{
			return 0f;
		}
	}

	public virtual float EvolvedChance
	{
		get
		{
			return 0f;
		}
	}

	public virtual float ShrineChance
	{
		get
		{
			return 0f;
		}
	}

	public virtual float PrespawnRate
	{
		get
		{
			return 0f;
		}
	}

	public virtual float RespawnRate
	{
		get
		{
			return this.PrespawnRate * 0.5f;
		}
	}

	public bool ShowEnemyOnMinimap
	{
		get
		{
			return this.instance != null && this.instance.ShowEnemyOnMinimap;
		}
	}

	public virtual int RespawnPerHour
	{
		get
		{
			return this.MaxSpawn / 5 + 1;
		}
	}

	public virtual float ChanceSpawnNeutral
	{
		get
		{
			return 0.05f;
		}
	}

	public virtual bool GrowPlant
	{
		get
		{
			return base.mainFaction == EClass.pc.faction;
		}
	}

	public virtual bool GrowWeed
	{
		get
		{
			return this.GrowPlant;
		}
	}

	public virtual bool IsExplorable
	{
		get
		{
			return !base.isRandomSite;
		}
	}

	public virtual bool IsReturnLocation
	{
		get
		{
			return EClass.pc.homeZone == this || base.source.tag.Contains("return");
		}
	}

	public virtual bool ShouldMakeExit
	{
		get
		{
			return base.lv > this.MinLv && base.lv < this.MaxLv;
		}
	}

	public virtual bool ShouldRegenerate
	{
		get
		{
			return false;
		}
	}

	public virtual bool ShouldAutoRevive
	{
		get
		{
			return this.ShouldRegenerate || this.IsPCFaction;
		}
	}

	public virtual bool UseFog
	{
		get
		{
			return base.lv < 0;
		}
	}

	public virtual bool RevealRoom
	{
		get
		{
			return false;
		}
	}

	public virtual bool AlwaysLowblock
	{
		get
		{
			return this.map.IsIndoor;
		}
	}

	public virtual bool UseLight
	{
		get
		{
			return base.mainFaction == EClass.pc.faction || base.source.tag.Contains("light");
		}
	}

	public virtual int StartLV
	{
		get
		{
			return 0;
		}
	}

	public virtual bool ScaleMonsterLevel
	{
		get
		{
			return false;
		}
	}

	public virtual bool HiddenInRegionMap
	{
		get
		{
			return false;
		}
	}

	public virtual string GetNewZoneID(int level)
	{
		return base.source.id;
	}

	public virtual FlockController.SpawnType FlockType
	{
		get
		{
			return FlockController.SpawnType.Default;
		}
	}

	public override string ToString()
	{
		string[] array = new string[10];
		array[0] = this.Name;
		array[1] = "(";
		array[2] = base.uid.ToString();
		array[3] = ")(";
		int num = 4;
		Point regionPos = this._regionPos;
		array[num] = ((regionPos != null) ? regionPos.ToString() : null);
		array[5] = ") instance?";
		array[6] = this.IsInstance.ToString();
		array[7] = "/";
		array[8] = EClass.world.date.GetRemainingHours(base.dateExpire).ToString();
		array[9] = "h";
		return string.Concat(array);
	}

	public override string NameSuffix
	{
		get
		{
			if (!this.IsNefia || !this.GetTopZone().isConquered)
			{
				return "";
			}
			return "conquered".lang();
		}
	}

	public string NameWithLevel
	{
		get
		{
			return this.Name + this.TextLevel(base.lv);
		}
	}

	public string TextLevel(int _lv)
	{
		if (_lv == 0)
		{
			return "";
		}
		if (base.lv <= 0)
		{
			return " " + "zoneLevelB".lang((_lv * -1).ToString() ?? "", null, null, null, null);
		}
		return " " + "zoneLevel".lang((_lv + 1).ToString() ?? "", null, null, null, null);
	}

	public string TextDeepestLv
	{
		get
		{
			if (this.GetDeepestLv() != 0)
			{
				return "zoneLevelMax".lang(this.TextLevel(this.GetDeepestLv()), null, null, null, null);
			}
			return "";
		}
	}

	public bool CanEnterBuildMode
	{
		get
		{
			return EClass.debug.godBuild || EClass.debug.ignoreBuildRule || base.mainFaction == EClass.pc.faction;
		}
	}

	public bool CanEnterBuildModeAnywhere
	{
		get
		{
			return EClass.debug.godBuild || EClass.debug.ignoreBuildRule || (base.mainFaction == EClass.pc.faction && EClass.Branch.elements.Has(4003));
		}
	}

	public bool IsPCFaction
	{
		get
		{
			return base.mainFaction == EClass.pc.faction;
		}
	}

	public bool IsStartZone
	{
		get
		{
			return this == EClass.game.StartZone;
		}
	}

	public bool IsInstance
	{
		get
		{
			return this.instance != null;
		}
	}

	public bool IsLoaded
	{
		get
		{
			return this.map != null;
		}
	}

	public virtual int BaseElectricity
	{
		get
		{
			return 0;
		}
	}

	public bool IsActiveZone
	{
		get
		{
			return EClass.game.activeZone == this;
		}
	}

	public bool CanInspect
	{
		get
		{
			return !this.IsInstance && !this.HiddenInRegionMap;
		}
	}

	public string InspectName
	{
		get
		{
			return this.Name + ((this.IsTown || this.IsPCFaction || this is Zone_Civilized) ? "" : "dangerLv".lang(this.DangerLv.ToString() ?? "", null, null, null, null));
		}
	}

	public Point InspectPoint
	{
		get
		{
			return null;
		}
	}

	public Vector3 InspectPosition
	{
		get
		{
			return default(Vector3);
		}
	}

	public override void OnCreate()
	{
		this.events.zone = this;
		base.lv = this.StartLV;
	}

	public override void OnAfterCreate()
	{
		if (this.AddPrefix)
		{
			if (this.GetTopZone() == this)
			{
				base.idPrefix = EClass.sources.zoneAffixes.rows.RandomItem<SourceZoneAffix.Row>().id;
				return;
			}
			base.idPrefix = this.GetTopZone().idPrefix;
		}
	}

	public void Activate()
	{
		if (this.IsRegion)
		{
			EClass.scene.elomapActor.Initialize(this.Region.elomap);
		}
		this.isStarted = (this.isSimulating = false);
		base.isKnown = true;
		EClass.game.config.reverseSnow = false;
		if (this.RegenerateOnEnter && EClass.game.activeZone != null && EClass.game.activeZone.GetTopZone() != this.GetTopZone() && EClass.world.date.IsExpired(base.dateExpire))
		{
			Msg.Say("regenerateZone", this.Name, null, null, null);
			this.ClearZones(null);
			if (EClass.pc.global.transition != null)
			{
				EClass.pc.global.transition.uidLastZone = 0;
			}
		}
		Zone activeZone = EClass.game.activeZone;
		if (EClass.game.activeZone != null)
		{
			EClass.game.activeZone.Deactivate();
		}
		EClass.game.activeZone = this;
		ZoneExportData zoneExportData = null;
		Debug.Log(string.Concat(new string[]
		{
			this.NameWithLevel,
			"/",
			this.id,
			"/",
			base.uid.ToString(),
			"/",
			base.isGenerated.ToString(),
			"/",
			this.IsLoaded.ToString(),
			"/",
			File.Exists(base.pathSave + "map").ToString()
		}));
		if (base.isGenerated && !this.IsLoaded && !File.Exists(base.pathSave + "map"))
		{
			Debug.Log("(Bug) File does not exist:" + this.destryoed.ToString() + "/" + base.pathSave);
			base.isGenerated = false;
		}
		bool flag = false;
		Debug.Log(this.idCurrentSubset + "/" + this.IDSubset);
		bool flag2 = this.idCurrentSubset != this.IDSubset || Zone.forceSubset != null;
		if (flag2 && this.map != null)
		{
			this.UnloadMap();
		}
		Debug.Log(string.Concat(new string[]
		{
			this.idExport,
			"/",
			File.Exists(this.pathExport).ToString(),
			"/",
			this.pathExport
		}));
		if (!base.isGenerated && (this.idExport.IsEmpty() || !File.Exists(this.pathExport)))
		{
			Debug.Log("generating random map");
			flag = true;
			base.dateExpire = EClass.world.date.GetRaw(0) + 1440 * this.ExpireDays;
			this.Generate();
			this.OnGenerateMap();
			if (this.instance != null)
			{
				this.instance.OnGenerateMap();
			}
			if (!this.UseFog)
			{
				this.map.ForeachCell(delegate(global::Cell c)
				{
					c.isSeen = true;
				});
			}
			if (!(this.bp is GameBlueprint))
			{
				this.AddGlobalCharasOnActivate();
			}
		}
		else if (this.IsLoaded)
		{
			Debug.Log("zone is already loaded");
			this.map.SetZone(this);
			EClass.core.textures.ApplyLocalReplace(base.isMapSaved ? base.pathSave : this.pathTemp);
			this.AddGlobalCharasOnActivate();
		}
		else
		{
			this.subset = null;
			bool flag3 = (base.isGenerated && flag2) || (base.isGenerated && !this.IsInstance && !this.IsPCFaction && this.ShouldRegenerate && EClass.world.date.IsExpired(base.dateRegenerate)) || Zone.forceRegenerate;
			if (this.pathExport.IsEmpty() || !File.Exists(this.pathExport) || EClass.game.isLoading || EClass.player.simulatingZone)
			{
				flag3 = false;
				flag2 = false;
			}
			Debug.Log(string.Concat(new string[]
			{
				base.isGenerated.ToString(),
				"/",
				flag3.ToString(),
				"/",
				flag2.ToString(),
				"/",
				this.IDSubset
			}));
			if (!base.isGenerated || flag3 || flag2)
			{
				Debug.Log("importing map:" + this.pathExport);
				flag = true;
				base.dateRegenerate = EClass.world.date.GetRaw(0) + 1440 * EClass.setting.balance.dateRegenerateZone;
				if (!flag3)
				{
					IO.DeleteDirectory(this.pathTemp + "Texture Replace");
					Debug.Log(this.pathTemp);
				}
				zoneExportData = this.Import(this.pathExport);
				base.isGenerated = true;
				this.isImported = true;
				if (flag3)
				{
					zoneExportData.orgMap = GameIO.LoadFile<Map>(base.pathSave + "map");
				}
			}
			EClass.game.countLoadedMaps++;
			Debug.Log("loading map: imported? " + this.isImported.ToString() + " regenerate? " + flag3.ToString());
			this.map = GameIO.LoadFile<Map>((this.isImported ? this.pathTemp : base.pathSave) + "map");
			if (this.map == null)
			{
				EClass.ui.Say(string.Concat(new string[]
				{
					"System.IO.EndOfStreamException: Unexpected end of stream:",
					Environment.NewLine,
					"File may be corrupted. Try replacing the following file if you have a backup:",
					Environment.NewLine,
					this.isImported ? this.pathTemp : base.pathSave,
					"map"
				}), null);
				return;
			}
			this.map.SetZone(this);
			this.map.Load(this.isImported ? this.pathTemp : base.pathSave, this.isImported, null);
			this.map.SetReference();
			EClass.core.textures.ApplyLocalReplace(base.isMapSaved ? base.pathSave : this.pathTemp);
			if (this.isImported)
			{
				this.map.deadCharas.Clear();
				this.map.OnImport(zoneExportData);
				if (this.UseFog && !flag3)
				{
					this.map.ForeachCell(delegate(global::Cell c)
					{
						c.isSeen = false;
					});
				}
				if (zoneExportData.orgMap != null)
				{
					Map orgMap = zoneExportData.orgMap;
					List<Chara> serializedCharas = this.map.serializedCharas;
					this.map.charas = orgMap.charas;
					this.map.serializedCharas = orgMap.serializedCharas;
					this.map.deadCharas = orgMap.deadCharas;
					byte[] array = orgMap.TryLoadFile(base.pathSave, "flags", EClass._map.Size * EClass._map.Size);
					if (array != null && array.Length == EClass._map.Size * EClass._map.Size)
					{
						for (int i = 0; i < EClass._map.Size; i++)
						{
							for (int j = 0; j < EClass._map.Size; j++)
							{
								this.map.cells[i, j].isSeen = array[i * EClass._map.Size + j].GetBit(1);
							}
						}
					}
					foreach (Chara chara in serializedCharas)
					{
						string[] array2 = new string[6];
						array2[0] = "Importing New Chara:";
						array2[1] = chara.id;
						array2[2] = "/";
						array2[3] = chara.Name;
						array2[4] = "/";
						int num = 5;
						Point orgPos = chara.orgPos;
						array2[num] = ((orgPos != null) ? orgPos.ToString() : null);
						Debug.Log(string.Concat(array2));
						this.map.serializedCharas.Add(chara);
					}
					this.map.things.ForeachReverse(delegate(Thing t)
					{
						if (t.trait is TraitNewZone)
						{
							foreach (Thing thing4 in orgMap.things)
							{
								if (t.id == thing4.id && t.pos.Equals(thing4.pos))
								{
									this.RemoveCard(t);
									this.map.things.Insert(0, thing4);
									thing4.stackOrder = 0;
									return;
								}
							}
							return;
						}
						if (t.trait is TraitStairsLocked)
						{
							foreach (Thing thing5 in orgMap.things)
							{
								if (thing5.trait is TraitNewZone && t.pos.Equals(thing5.pos))
								{
									this.RemoveCard(t);
									this.map.things.Add(thing5);
									return;
								}
							}
							return;
						}
						if (t.id == "medal" || t.id == "856")
						{
							foreach (Thing thing6 in orgMap.things)
							{
								if (t.id == thing6.id && t.pos.Equals(thing6.pos))
								{
									return;
								}
							}
							this.RemoveCard(t);
						}
					});
					foreach (KeyValuePair<int, int> keyValuePair in EClass._map.backerObjs.ToList<KeyValuePair<int, int>>())
					{
						EClass._map.GetCell(keyValuePair.Key);
						SourceBacker.Row row = EClass.sources.backers.map[keyValuePair.Value];
						if (EClass.player.doneBackers.Contains(row.id) && !EClass.core.config.test.ignoreBackerDestoryFlag)
						{
							this.map.backerObjs.Remove(keyValuePair.Key);
						}
					}
					foreach (Chara chara2 in this.map.serializedCharas)
					{
						if (chara2.orgPos != null && chara2.orgPos.IsValid)
						{
							chara2.pos.Set(chara2.orgPos);
						}
					}
					foreach (Thing thing in orgMap.things)
					{
						if (thing.trait is TraitTent && !thing.isNPCProperty)
						{
							thing.AddEditorTag(EditorTag.NoNpcProperty);
							thing.isSubsetCard = false;
							this.map.things.Add(thing);
							Debug.Log(thing);
						}
					}
				}
			}
			foreach (Thing c2 in this.map.things)
			{
				this.map.AddCardOnActivate(c2);
			}
			foreach (Chara chara3 in this.map.serializedCharas)
			{
				this.map.charas.Add(chara3);
				this.map.AddCardOnActivate(chara3);
			}
			this.map.serializedCharas.Clear();
			if (this.isImported && this.IsTown)
			{
				this.RefreshListCitizen();
			}
			this.map.RefreshAllTiles();
			this.AddGlobalCharasOnActivate();
			this.map.OnLoad();
			if (flag3)
			{
				foreach (Card card in this.map.Cards.ToList<Card>())
				{
					if (card.isSubsetCard)
					{
						card.Destroy();
					}
				}
			}
			if (this.isImported)
			{
				this.idCurrentSubset = (Zone.forceSubset ?? this.IDSubset);
				if (this.idCurrentSubset != null)
				{
					this.subset = MapSubset.Load(this.idCurrentSubset);
					this.subset.Apply();
				}
			}
			if (this.isImported)
			{
				if (this.MakeTownProperties)
				{
					using (List<Thing>.Enumerator enumerator3 = this.map.things.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							Thing thing2 = enumerator3.Current;
							thing2.isNPCProperty = (!thing2.isHidden && !thing2.HasEditorTag(EditorTag.NoNpcProperty));
						}
						goto IL_C24;
					}
				}
				foreach (Thing thing3 in this.map.things)
				{
					thing3.isNPCProperty = false;
				}
				IL_C24:
				this.OnGenerateMap();
				if (this.instance != null)
				{
					this.instance.OnGenerateMap();
				}
			}
			if (this.isImported && !flag3 && !this.RevealRoom)
			{
				foreach (global::Room room in this.map.rooms.listRoom)
				{
					if (room.HasRoof && !room.data.atrium)
					{
						foreach (Point point in room.points)
						{
							point.cell.isSeen = false;
						}
					}
				}
			}
			if (flag3)
			{
				this.OnRegenerate();
			}
		}
		PathManager.Instance._pathfinder.PunishChangeDirection = false;
		this.isImported = false;
		if (flag && this.IsTown && base.lv == 0)
		{
			this.SpawnLostItems();
		}
		if (base.visitCount == 0)
		{
			base.dateRevive = EClass.world.date.GetRaw(0) + 1440 * EClass.setting.balance.dateRevive;
		}
		this.map.ForeachCell(delegate(global::Cell c)
		{
			if (c.HasFire)
			{
				this.map.effectManager.GetOrCreate(c.GetSharedPoint());
			}
			if (this.IsRegion)
			{
				c.decal = 0;
			}
		});
		if (EClass.world.weather.IsRaining)
		{
			this.RainWater();
		}
		if (EClass.debug.revealMap)
		{
			this.map.ForeachCell(delegate(global::Cell c)
			{
				c.isSeen = true;
			});
		}
		this.isStarted = true;
		this.map.RefreshAllTiles();
		if (this.events.listPreEnter.Count > 0)
		{
			foreach (ZonePreEnterEvent zonePreEnterEvent in this.events.listPreEnter)
			{
				zonePreEnterEvent.Execute();
			}
			this.events.listPreEnter.Clear();
		}
		foreach (Card card2 in this.map.Cards)
		{
			card2.CalculateFOV();
			if (card2.isChara)
			{
				Chara chara4 = card2.Chara;
				if (card2.IsUnique && !card2.IsPCFaction && !card2.IsPCParty)
				{
					Point point2 = chara4.orgPos ?? card2.pos;
					card2.c_uniqueData = new UniqueData
					{
						x = point2.x,
						y = point2.z,
						uidZone = base.uid
					};
				}
				int @int = card2.GetInt(55, null);
				if (@int != 0)
				{
					foreach (Chara chara5 in this.map.charas)
					{
						if (chara5.uid == @int)
						{
							if (!chara4.IsHostile(chara5))
							{
								break;
							}
							chara4.enemy = chara5;
							chara4.SetAI(new GoalCombat());
							chara4.calmCheckTurn = 20 + EClass.rnd(30);
							break;
						}
					}
					card2.SetInt(55, 0);
				}
				if (card2.pos.cell.IsBlocked && !card2.isRestrained && !card2.HasCondition<ConSuspend>())
				{
					card2.MoveImmediate(card2.pos.GetNearestPoint(false, true, true, false) ?? card2.pos, true, true);
				}
				chara4.SyncRide();
				if (card2.c_uidMaster != 0 && chara4.master == null)
				{
					chara4.FindMaster();
				}
				if (!EClass.game.isLoading)
				{
					chara4.enemy = null;
					if (chara4.IsInCombat)
					{
						chara4.SetNoGoal();
					}
				}
			}
			else if (card2.IsInstalled)
			{
				TraitDoor traitDoor = card2.trait as TraitDoor;
				if (traitDoor != null && card2.pos.HasChara && !traitDoor.IsOpen())
				{
					traitDoor.ToggleDoor(false, false);
				}
			}
		}
		this.RefreshHat();
		Zone.forceRegenerate = false;
		Zone.forceSubset = null;
		EClass.ui.OnActivateZone();
		EClass.scene.RebuildActorEx();
		EClass.Sound.LoadAmbience(this.IDAmbience);
		if (EClass.Branch != null)
		{
			EClass.Branch.OnActivateZone();
		}
		this.OnVisit();
		if (flag)
		{
			this.OnVisitNewMapOrRegenerate();
		}
		Guild currentGuild = Guild.GetCurrentGuild();
		if (currentGuild != null)
		{
			currentGuild.RefreshDevelopment();
		}
		if (this.IsPCFaction)
		{
			EClass.player.uidLastTown = 0;
		}
		else if (this.IsTown && base.lv == 0)
		{
			EClass.player.uidLastTown = base.uid;
		}
		this.RefreshBGM();
		Rand.InitBytes(this.map.seed + 1);
		this.RefreshElectricity();
		Zone.okaerinko = 0;
		if (EClass.debug.enable)
		{
			this.ModInfluence(2000);
		}
		if (this is Zone_TinkerCamp)
		{
			Tutorial.Reserve("tinker", null);
			return;
		}
		if (this is Zone_Town && !(this is Zone_SubTown))
		{
			Tutorial.Reserve("town", null);
		}
	}

	public void RefreshHat()
	{
		if (this.idHat != null && EClass.world.date.IsExpired(base.dateHat))
		{
			this.idHat = null;
			base.dateHat = 0;
		}
		Zone.sourceHat = ((this.idHat != null) ? EClass.sources.cards.map[this.idHat] : ((this.IDHat != null) ? EClass.sources.cards.map[this.IDHat] : null));
	}

	public void OnVisit()
	{
		if (this.CountDeepestLevel && this.DangerLv > EClass.player.stats.deepest)
		{
			EClass.player.stats.deepest = this.DangerLv;
		}
		if (EClass.world.date.IsExpired(base.dateRevive))
		{
			this.ResetHostility();
			this.Revive();
			foreach (Chara chara in EClass._map.charas)
			{
				chara.TryRestock(false);
				if (!chara.IsPCFaction)
				{
					chara.c_fur = 0;
				}
			}
		}
		this.RefreshCriminal();
		EClass._map.rooms.AssignCharas();
		this.events.OnVisit();
		this.OnActivate();
		this.UpdateQuests(false);
		this.OnBeforeSimulate();
		this.isSimulating = true;
		this.Simulate();
		this.isSimulating = false;
		this.OnAfterSimulate();
		if (EClass.Branch != null)
		{
			EClass.Branch.OnAfterSimulate();
		}
		base.lastActive = EClass.world.date.GetRaw(0);
		if (!EClass.game.isLoading)
		{
			int visitCount = base.visitCount;
			base.visitCount = visitCount + 1;
		}
		base.version = EClass.core.version.GetInt();
	}

	public void Revive()
	{
		base.dateRevive = EClass.world.date.GetRaw(0) + 1440 * EClass.setting.balance.dateRevive;
		if (this.ShouldAutoRevive)
		{
			foreach (Chara chara in this.map.deadCharas)
			{
				if (chara.trait.CanAutoRevive)
				{
					chara.Revive(null, false);
					if (chara.isBackerContent && EClass.player.doneBackers.Contains(chara.c_idBacker) && !EClass.core.config.test.ignoreBackerDestoryFlag)
					{
						chara.RemoveBacker();
					}
					EClass._zone.AddCard(chara, (chara.orgPos != null && chara.orgPos.IsInBounds) ? chara.orgPos : chara.pos);
				}
			}
		}
		foreach (Chara chara2 in EClass.game.cards.globalCharas.Values)
		{
			if (chara2.isDead && chara2.homeZone == this)
			{
				chara2.Revive(null, false);
				Point point = this.GetSpawnPos(chara2, ZoneTransition.EnterState.Auto);
				if (chara2.orgPos != null && chara2.orgPos.IsInBounds)
				{
					point = (chara2.orgPos.GetNearestPoint(false, true, true, false) ?? point);
				}
				EClass._zone.AddCard(chara2, point);
			}
		}
		this.map.deadCharas.Clear();
	}

	public virtual void OnRegenerate()
	{
	}

	public virtual void OnActivate()
	{
	}

	public virtual void OnBeforeDeactivate()
	{
	}

	public virtual void OnDeactivate()
	{
	}

	public virtual void OnBeforeSimulate()
	{
	}

	public virtual void OnVisitNewMapOrRegenerate()
	{
	}

	public virtual void OnAfterSimulate()
	{
	}

	public virtual void OnAdvanceHour()
	{
	}

	public void Simulate()
	{
		if (!EClass.game.isLoading && base.visitCount > 0)
		{
			if (this.Boss != null && this.IsNefia)
			{
				Msg.Say("bossLeave", this.Boss.Name.ToTitleCase(false), EClass._zone.Name, null, null);
				EClass._zone.RemoveCard(this.Boss);
				EClass._zone.GetTopZone().isConquered = true;
			}
			int num = Mathf.Clamp(this.MinsSinceLastActive, 1, 10000);
			foreach (Chara chara in EClass._map.charas)
			{
				if (chara.hp < 0)
				{
					chara.hp = 0;
				}
				if (!chara.IsPCParty)
				{
					if (chara.mana.value < 0)
					{
						chara.mana.value = 0;
					}
					if (chara.stamina.value < 0)
					{
						chara.stamina.value = 0;
					}
				}
				if (num > 60)
				{
					chara.resistCon = null;
				}
				if (chara.rarity >= Rarity.Legendary && !chara.IsPCFaction && num > 0)
				{
					Msg.ignoreAll = true;
					chara.Cure(CureType.Boss, 20 + num * 10, BlessedState.Normal);
					chara.HealHP(Mathf.Max(1, chara.MaxHP) * num / 20, HealSource.None);
					chara.mana.Mod(Mathf.Max(1, chara.mana.max) * num / 20);
					chara.stamina.Mod(Mathf.Max(1, chara.stamina.max) * num / 20);
					Msg.ignoreAll = false;
				}
			}
		}
		if (this.HourSinceLastActive <= 1)
		{
			return;
		}
		Debug.Log(this.Name + " Simulate:" + this.HourSinceLastActive.ToString() + " hours");
		VirtualDate virtualDate = new VirtualDate(this.HourSinceLastActive);
		List<Chara> list = EClass._map.charas.ToList<Chara>();
		int num2 = this.HourSinceLastActive / 24;
		if (num2 > 0)
		{
			foreach (Chara chara2 in list)
			{
				if (!chara2.IsPCParty)
				{
					chara2.OnSleep(200, num2);
					if (chara2.conSleep != null)
					{
						chara2.conSleep.Kill(false);
					}
					if (EClass.rnd(EClass.world.date.IsNight ? 20 : 200) == 0 && !chara2.IsPCFaction)
					{
						chara2.AddCondition<ConSleep>(1000 + EClass.rnd(1000), true);
					}
					if (chara2.things.Count > 20)
					{
						chara2.ClearInventory(ClearInventoryType.Purge);
					}
				}
			}
		}
		EClass._map.things.ForeachReverse(delegate(Thing t)
		{
			t.DecayNatural(this.HourSinceLastActive);
		});
		VirtualDate.current = virtualDate;
		for (int i = 0; i < this.HourSinceLastActive; i++)
		{
			virtualDate.SimulateHour();
		}
		VirtualDate.current = null;
		if (!this.IsPCFaction)
		{
			return;
		}
		int num3 = 0;
		foreach (Chara chara3 in list)
		{
			if (!chara3.IsPCParty)
			{
				if (!chara3.IsHomeMember())
				{
					if (chara3.id == "bee")
					{
						num3++;
					}
					if (num2 > 0 && chara3.IsGuest())
					{
						chara3.ChooseNewGoal();
						chara3.ai.SimulateZone(num2);
					}
				}
				else
				{
					if (num2 > 0)
					{
						Goal goal = chara3.GetGoalWork();
						chara3.SetAI(goal);
						if (goal is GoalWork)
						{
							(goal as GoalWork).FindWork(chara3, true);
						}
						chara3.ai.SimulateZone(num2);
						goal = chara3.GetGoalHobby();
						chara3.SetAI(goal);
						if (goal is GoalWork)
						{
							(goal as GoalWork).FindWork(chara3, true);
						}
						chara3.ai.SimulateZone(num2);
					}
					chara3.ChooseNewGoal();
					if (chara3.conSuspend == null)
					{
						chara3.ai.OnSimulatePosition();
					}
				}
			}
		}
		List<Thing> list2 = new List<Thing>();
		foreach (Thing thing in this.map.things)
		{
			if (thing.IsInstalled && thing.trait is TraitBeekeep)
			{
				list2.Add(thing);
			}
		}
		if (num3 < list2.Count)
		{
			for (int j = num3; j < list2.Count; j++)
			{
				if (EClass.rnd(200) <= this.HourSinceLastActive)
				{
					Chara chara4 = CharaGen.Create("bee", -1);
					this.AddCard(chara4, list2.RandomItem<Thing>().pos);
					chara4.c_originalHostility = (chara4.hostility = Hostility.Neutral);
				}
			}
		}
	}

	public Point GetSpawnPos(Chara c, ZoneTransition.EnterState destState = ZoneTransition.EnterState.Auto)
	{
		GlobalData global = c.global;
		ZoneTransition zoneTransition = (global != null) ? global.transition : null;
		Zone zone = (zoneTransition != null) ? zoneTransition.lastZone : null;
		ZoneTransition.EnterState enterState = (zoneTransition != null) ? zoneTransition.state : destState;
		bool flag = zone == null || zone.lv > base.lv;
		if (this.IsRegion)
		{
			if (zone != null && !EClass.player.simulatingZone)
			{
				Zone topZone = zone.GetTopZone();
				if (this.GetZoneAt(topZone.x, topZone.y) != null)
				{
					return new Point(topZone.mapX, topZone.mapY);
				}
			}
		}
		else if (c.IsPC && EClass.player.lastZonePos != null && zone != null && zone.IsRegion && (enterState == ZoneTransition.EnterState.Left || enterState == ZoneTransition.EnterState.Right || enterState == ZoneTransition.EnterState.Top || enterState == ZoneTransition.EnterState.Bottom))
		{
			return EClass.player.lastZonePos;
		}
		if (enterState == ZoneTransition.EnterState.Region)
		{
			enterState = this.RegionEnterState;
		}
		if (this is Zone_Kapul && enterState == ZoneTransition.EnterState.Left)
		{
			enterState = ZoneTransition.EnterState.Right;
		}
		float rate = (zoneTransition == null) ? -1f : zoneTransition.ratePos;
		if (this.IsPCFaction)
		{
			if (enterState - ZoneTransition.EnterState.Dir <= 4 || enterState == ZoneTransition.EnterState.Return)
			{
				Thing spot = EClass._map.props.installed.Find<TraitSpotGuidePC>();
				if (spot != null)
				{
					Point nearestPoint = (spot.trait as TraitSpotGuidePC).GetRandomPoint((Point p) => !p.IsBlocked && !p.Equals(spot.pos), null).GetNearestPoint(false, true, true, false);
					if (nearestPoint != null && nearestPoint.IsValid)
					{
						return nearestPoint;
					}
				}
			}
		}
		Thing randomThing;
		Thing randomThing2;
		Thing randomThing3;
		for (;;)
		{
			switch (enterState)
			{
			case ZoneTransition.EnterState.Center:
			case ZoneTransition.EnterState.Encounter:
				goto IL_4B7;
			case ZoneTransition.EnterState.Top:
				goto IL_504;
			case ZoneTransition.EnterState.Right:
				goto IL_520;
			case ZoneTransition.EnterState.Bottom:
				goto IL_53C;
			case ZoneTransition.EnterState.Left:
				goto IL_558;
			case ZoneTransition.EnterState.Dead:
			case ZoneTransition.EnterState.Exact:
			case ZoneTransition.EnterState.PortalReturn:
			case ZoneTransition.EnterState.Fall:
				goto IL_574;
			case ZoneTransition.EnterState.RandomVisit:
				goto IL_5C5;
			case ZoneTransition.EnterState.Down:
				goto IL_5CD;
			case ZoneTransition.EnterState.Up:
				goto IL_5D1;
			case ZoneTransition.EnterState.Return:
			case ZoneTransition.EnterState.Teleport:
			case ZoneTransition.EnterState.Elevator:
			case ZoneTransition.EnterState.Moongate:
				if (enterState == ZoneTransition.EnterState.Teleport)
				{
					foreach (Thing thing in this.map.things)
					{
						if (thing.IsInstalled)
						{
							TraitTeleporter traitTeleporter = thing.trait as TraitTeleporter;
							if (traitTeleporter != null)
							{
								Debug.Log(string.Concat(new string[]
								{
									zoneTransition.idTele,
									"/",
									traitTeleporter.id.IsEmpty(traitTeleporter.GetParam(3, null)),
									"/",
									traitTeleporter.GetParam(3, null)
								}));
							}
							if (traitTeleporter != null && !zoneTransition.idTele.IsEmpty() && zoneTransition.idTele == traitTeleporter.id.IsEmpty(traitTeleporter.GetParam(3, null)))
							{
								return thing.pos.GetNearestPoint(false, true, true, false);
							}
						}
					}
					foreach (Thing thing2 in this.map.things)
					{
						if (thing2.IsInstalled)
						{
							TraitTeleporter traitTeleporter2 = thing2.trait as TraitTeleporter;
							if (traitTeleporter2 != null && traitTeleporter2.IsFor(zoneTransition.lastZone))
							{
								return thing2.pos.GetNearestPoint(false, true, true, false);
							}
						}
					}
					randomThing = EClass._map.props.installed.traits.GetRandomThing<TraitTeleporter>();
					if (randomThing != null)
					{
						goto Block_29;
					}
				}
				if (enterState == ZoneTransition.EnterState.Elevator)
				{
					foreach (Thing thing3 in this.map.things)
					{
						if (thing3.IsInstalled)
						{
							TraitElevator traitElevator = thing3.trait as TraitElevator;
							if (traitElevator != null && traitElevator.IsFor(zoneTransition.lastZone))
							{
								return thing3.pos.GetNearestPoint(false, true, true, false);
							}
						}
					}
					randomThing2 = EClass._map.props.installed.traits.GetRandomThing<TraitElevator>();
					if (randomThing2 != null)
					{
						goto Block_32;
					}
				}
				randomThing3 = this.map.props.installed.traits.GetRandomThing<TraitWaystone>();
				if (randomThing3 != null)
				{
					goto Block_33;
				}
				randomThing3 = this.map.props.installed.traits.GetRandomThing<TraitCoreZone>();
				if (randomThing3 != null)
				{
					goto Block_34;
				}
				if (base.lv == 0)
				{
					enterState = ZoneTransition.EnterState.Center;
					continue;
				}
				goto IL_4A0;
			case ZoneTransition.EnterState.UndergroundOrSky:
				goto IL_1CB;
			}
			break;
		}
		goto IL_5D3;
		IL_1CB:
		return new Point(zoneTransition.x, zoneTransition.z);
		Block_29:
		return randomThing.pos.GetNearestPoint(false, true, true, false);
		Block_32:
		return randomThing2.pos.GetNearestPoint(false, true, true, false);
		Block_33:
		return randomThing3.pos.GetNearestPoint(false, true, true, false);
		Block_34:
		return randomThing3.pos.GetNearestPoint(false, true, true, false);
		IL_4A0:
		flag = (base.lv <= 0);
		goto IL_5D3;
		IL_4B7:
		if (this.map.config.embarkX != 0)
		{
			return new Point(this.map.config.embarkX, this.map.config.embarkY);
		}
		return this.map.GetCenterPos().GetNearestPoint(false, false, true, false);
		IL_504:
		return this.map.bounds.GetTopPos(rate).GetNearestPoint(false, false, true, false);
		IL_520:
		return this.map.bounds.GetRightPos(rate).GetNearestPoint(false, false, true, false);
		IL_53C:
		return this.map.bounds.GetBottomPos(rate).GetNearestPoint(false, false, true, false);
		IL_558:
		return this.map.bounds.GetLeftPos(rate).GetNearestPoint(false, false, true, false);
		IL_574:
		if (zoneTransition.x == 0 && EClass._map.bounds.x != 0)
		{
			return new Point(this.map.config.embarkX, this.map.config.embarkY);
		}
		return new Point(zoneTransition.x, zoneTransition.z);
		IL_5C5:
		return this.GetRandomVisitPos(c);
		IL_5CD:
		flag = true;
		goto IL_5D3;
		IL_5D1:
		flag = false;
		IL_5D3:
		foreach (Thing thing4 in this.map.things)
		{
			TraitNewZone traitNewZone = thing4.trait as TraitNewZone;
			if (traitNewZone != null && traitNewZone.zone != null && zone != null && traitNewZone.zone.uid == zone.uid)
			{
				if (c != null && enterState != ZoneTransition.EnterState.Auto)
				{
					c.SetDir(traitNewZone.owner.dir);
				}
				return traitNewZone.GetExitPos();
			}
		}
		foreach (Thing thing5 in this.map.things)
		{
			TraitNewZone traitNewZone2 = thing5.trait as TraitNewZone;
			if (traitNewZone2 != null && ((flag && traitNewZone2.IsUpstairs) || (!flag && traitNewZone2.IsDownstairs)))
			{
				if (c != null && enterState != ZoneTransition.EnterState.Auto)
				{
					c.SetDir(traitNewZone2.owner.dir);
				}
				return traitNewZone2.GetExitPos();
			}
		}
		return this.GetRandomVisitPos(c);
	}

	public Point GetRandomVisitPos(Chara c)
	{
		Point point = null;
		if (EClass.rnd(3) == 0 && this.map.rooms.listRoom.Count > 0)
		{
			point = this.map.rooms.listRoom.RandomItem<global::Room>().points.RandomItem<Point>();
		}
		if (point == null && EClass.rnd(4) != 0)
		{
			IEnumerable<Chara> enumerable = from t in this.map.charas
			where t.trait.ShopType != ShopType.None && t.pos != null && t.pos.IsValid
			select t;
			if (enumerable.Count<Chara>() > 0)
			{
				point = enumerable.RandomItem<Chara>().pos.GetRandomPoint(3, true, true, false, 100);
			}
		}
		if (point == null)
		{
			point = (this.map.bounds.GetRandomSurface(false, true, !this.IsPCFaction && !(this is Zone_Civilized)) ?? this.map.bounds.GetRandomPoint());
		}
		return point.GetNearestPoint(false, false, false, false) ?? EClass._map.GetCenterPos();
	}

	public void AddGlobalCharasOnActivate()
	{
		Point spawnPosPC = null;
		if (EClass.pc.currentZone == this)
		{
			spawnPosPC = this.GetSpawnPos(EClass.pc, ZoneTransition.EnterState.Auto);
		}
		if (spawnPosPC == null)
		{
			spawnPosPC = this.map.GetCenterPos();
		}
		if (spawnPosPC.IsValid && EClass.pc.global.transition != null && spawnPosPC.HasBlock)
		{
			spawnPosPC = spawnPosPC.GetNearestPoint(false, true, true, false);
		}
		spawnPosPC = spawnPosPC.Clamp(true).GetNearestPoint(false, true, true, false);
		using (Dictionary<int, Chara>.ValueCollection.Enumerator enumerator = EClass.game.cards.globalCharas.Values.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Chara c = enumerator.Current;
				if (c.currentZone == this)
				{
					if (c.parent is Chara)
					{
						Chara chara = c.parent as Chara;
						c.currentZone = chara.currentZone;
					}
					else
					{
						c.isRestrained = false;
						if (!c.isDead)
						{
							if (c.global.transition != null)
							{
								Point pos = c.IsPC ? spawnPosPC : (c.IsPCParty ? spawnPosPC.GetNearestPoint(false, false, true, true) : this.GetSpawnPos(c, ZoneTransition.EnterState.Auto));
								if (c.IsPCParty && !c.IsPC)
								{
									if (c.host == EClass.pc)
									{
										pos.Set(spawnPosPC);
									}
									else if (pos.Equals(spawnPosPC) || !PathManager.Instance.IsPathClear(spawnPosPC, pos, c, 5))
									{
										c.pos.Set(spawnPosPC);
										if (!spawnPosPC.ForeachNearestPoint(delegate(Point p)
										{
											if (PathManager.Instance.IsPathClear(spawnPosPC, p, c, 5) && !p.Equals(spawnPosPC))
											{
												pos.Set(p);
												return true;
											}
											return false;
										}, false, EClass.pc.party.members.Count >= 12, true, true))
										{
											pos.Set(spawnPosPC);
										}
									}
								}
								c.pos.Set(pos);
								c.global.transition = null;
							}
							this.map.charas.Add(c);
							this.map.AddCardOnActivate(c);
						}
					}
				}
			}
		}
		foreach (Chara chara2 in EClass.player.listSummon)
		{
			Point nearestPoint = spawnPosPC.GetNearestPoint(false, false, true, true);
			chara2.enemy = null;
			chara2.pos.Set(nearestPoint);
			this.map.charas.Add(chara2);
			this.map.AddCardOnActivate(chara2);
		}
		EClass.player.listSummon.Clear();
	}

	public void Deactivate()
	{
		if (!this.IsUserZone && !this.IsPCFaction && this.map != null)
		{
			List<Thing> list = new List<Thing>();
			foreach (Thing thing in this.map.things)
			{
				if (thing.rarity >= Rarity.Artifact && thing.HasTag(CTAG.godArtifact))
				{
					list.Add(thing);
				}
			}
			if (this is Zone_Tent)
			{
				foreach (Thing thing2 in this.map.props.stocked.Things.Concat(this.map.props.roaming.Things))
				{
					if (thing2.IsContainer)
					{
						foreach (Thing thing3 in thing2.things)
						{
							if (thing3.trait is TraitTent)
							{
								list.Add(thing3);
							}
						}
					}
					if (thing2.trait is TraitTent)
					{
						list.Add(thing2);
					}
				}
			}
			if (list.Count > 0)
			{
				Msg.Say("pick_valuable");
				foreach (Thing thing4 in list)
				{
					Msg.Say("pick_valuable2", thing4, null, null, null);
					EClass.pc.AddCard(thing4);
				}
			}
		}
		base.isPeace = false;
		this.OnBeforeDeactivate();
		if (this.IsPCFaction)
		{
			foreach (Chara chara in this.branch.members)
			{
				chara.ValidateWorks();
			}
		}
		EClass.game.lastActiveZone = EClass.game.activeZone;
		base.lastActive = EClass.world.date.GetRaw(0);
		this.map.OnDeactivate();
		EClass.scene.Clear();
		EClass.game.activeZone = null;
		if (this.IsInstance)
		{
			this.UnloadMap();
			base.isGenerated = false;
		}
		if (Zone.forceRegenerate)
		{
			this.UnloadMap();
		}
		this.OnDeactivate();
	}

	public void OnKillGame()
	{
		foreach (Thing thing in this.map.things)
		{
			if (thing.renderer.hasActor)
			{
				thing.renderer.KillActor();
			}
		}
	}

	public void UnloadMap()
	{
		this.map = null;
		if (this.bp != null)
		{
			this.bp.map = null;
		}
		if (this.branch != null)
		{
			this.branch.OnUnloadMap();
		}
		Debug.Log("Unloaded Map:" + ((this != null) ? this.ToString() : null));
	}

	public void ClaimZone(bool debug = false)
	{
		EClass._map.RevealAll(true);
		this.SetMainFaction(EClass.pc.faction);
		this.branch = new FactionBranch();
		this.branch.OnCreate(this);
		if (base.icon == 0)
		{
			base.icon = 332;
		}
		this.instance = null;
		base.dateExpire = 0;
		base.SetInt(2, EClass.world.date.GetRaw(0));
		base.Register();
		foreach (Thing thing in this.map.things)
		{
			thing.isNPCProperty = false;
		}
		EClass.Branch.OnClaimZone();
		EClass.scene.elomap.SetZone(EClass._zone.x, EClass._zone.y, EClass._zone, true);
		if (debug)
		{
			for (int i = 0; i < 7; i++)
			{
				Chara chara = CharaGen.CreateFromFilter("c_neutral", -1, -1);
				EClass._zone.AddCard(chara, EClass._map.bounds.GetRandomPoint().GetNearestPoint(false, false, false, false));
				this.branch.AddMemeber(chara);
			}
		}
		else
		{
			Msg.Say("claimedZone");
			EClass.Branch.Log("claimedZone", null, null, null, null);
			EClass.Sound.Play("jingle_embark");
			EClass.pc.PlaySound("build", 1f, true);
			Effect.Get("aura_heaven").Play(EClass.pc.pos, 0f, null, null);
			Point nearestPoint = EClass.pc.pos.GetNearestPoint(false, true, false, false);
			EClass._zone.AddCard(ThingGen.Create("core_zone", -1, -1), nearestPoint).SetPlaceState(PlaceState.installed, false);
		}
		base.idPrefix = 0;
		if (EClass._zone == EClass.game.StartZone)
		{
			EClass.player.spawnZone = EClass._zone;
		}
		if (EClass._zone != EClass.game.StartZone && !(EClass._zone is Zone_Vernis))
		{
			EClass._zone.SetBGM(new List<int>
			{
				41,
				90,
				44,
				43
			}, true);
			EClass._zone.RefreshPlaylist();
		}
	}

	public void AbandonZone()
	{
		FactionBranch destBranch = (EClass.pc.homeBranch == this.branch) ? EClass.game.StartZone.branch : EClass.pc.homeBranch;
		this.branch.members.ForeachReverse(delegate(Chara m)
		{
			destBranch.AddMemeber(m);
			if (!m.IsPCParty)
			{
				m.MoveZone(destBranch.owner, ZoneTransition.EnterState.Auto);
			}
		});
		EClass.Branch.OnUnclaimZone();
		this.SetMainFaction(null);
		base.dateExpire = EClass.world.date.GetRaw(0) + 10080;
	}

	public bool CanDestroy()
	{
		if (this.IsActiveZone || this.IsPCFaction || EClass.player.simulatingZone)
		{
			return false;
		}
		if (this.IsInstance)
		{
			return true;
		}
		if (base.dateExpire == 0 || !EClass.world.date.IsExpired(base.dateExpire))
		{
			return false;
		}
		Zone topZone = this.GetTopZone();
		return (base.isRandomSite || this is Zone_Field || (topZone != null && topZone != this && topZone.FindDeepestZone() != this)) && EClass.pc.currentZone != null && !(EClass.pc.currentZone is Zone_Tent) && topZone != EClass.pc.currentZone.GetTopZone() && (EClass.player.nextZone == null || topZone != EClass.player.nextZone.GetTopZone());
	}

	public override void _OnBeforeSave()
	{
		if (this.CanDestroy())
		{
			Debug.Log("Deleting Zone:" + ((this != null) ? this.ToString() : null));
			if (!this.destryoed)
			{
				this.Destroy();
			}
			return;
		}
		if (this.map != null)
		{
			Debug.Log("Saving Zone:" + ((this != null) ? this.ToString() : null));
			this.map.Save(base.pathSave, null, null);
			if (!base.isMapSaved)
			{
				base.isMapSaved = true;
				IO.DeleteDirectory(base.pathSave + "Texture Replace");
				IO.CopyDir(this.pathTemp + "Texture Replace", base.pathSave + "/Texture Replace", null);
			}
			if (!this.IsActiveZone)
			{
				this.UnloadMap();
			}
		}
	}

	public override void OnLoad()
	{
		if (this.branch != null)
		{
			this.branch.SetOwner(this);
		}
		this.events.OnLoad(this);
	}

	public override void Destroy()
	{
		this.children.ForeachReverse(delegate(Spatial z)
		{
			z.Destroy();
		});
		if (this.map != null)
		{
			this.UnloadMap();
		}
		if (this.parent != null && this.parent.IsRegion && this.instance == null)
		{
			EClass.scene.elomap.SetZone(base.x, base.y, null, true);
		}
		EClass.game.spatials.Remove(this);
		this.destryoed = true;
		base.isGenerated = false;
	}

	public void ClearZones(Zone current = null)
	{
		if (this.map != null)
		{
			this.UnloadMap();
		}
		base.isGenerated = false;
		Zone topZone = this.GetTopZone();
		if (topZone != this)
		{
			topZone.ClearZones(this);
			return;
		}
		this.children.ForeachReverse(delegate(Spatial c)
		{
			if (c != current)
			{
				c.Destroy();
			}
		});
	}

	public void OnChildNumChange(Card c)
	{
	}

	public ICardParent GetRoot()
	{
		return this;
	}

	public Zone GetTopZone()
	{
		if (this.parent == null || this.parent.IsRegion)
		{
			return this;
		}
		return this.parent as Zone;
	}

	public Card AddCardSplinkle(Card t, Point center, int radius = 4)
	{
		Point point = new Point(center);
		for (int i = 0; i < 1000; i++)
		{
			point.x = center.x + EClass.rnd(radius) - EClass.rnd(radius);
			point.z = center.z + EClass.rnd(radius) - EClass.rnd(radius);
			if (point.IsValid && !point.IsBlocked && !point.HasChara)
			{
				return this.AddCard(t, point);
			}
		}
		return this.AddCard(t, center);
	}

	public Card AddChara(string id, int x, int z)
	{
		return this.AddCard(CharaGen.Create(id, -1), x, z);
	}

	public Card AddThing(string id, int x, int z)
	{
		return this.AddCard(ThingGen.Create(id, -1, -1), x, z);
	}

	public Card AddThing(string id, Point p)
	{
		return this.AddThing(id, p.x, p.z);
	}

	public Card AddCard(Card t, Point point)
	{
		return this.AddCard(t, point.x, point.z);
	}

	public Card AddCard(Card t)
	{
		return this.AddCard(t, 0, 0);
	}

	public Card AddCard(Card t, int x, int z)
	{
		if (t.parent != null)
		{
			t.parent.RemoveCard(t);
		}
		t.parent = this;
		Chara chara = t.Chara;
		if (chara != null)
		{
			chara.currentZone = this;
			chara.SetAI(new NoGoal());
		}
		if (this.IsActiveZone)
		{
			this.map.OnCardAddedToZone(t, x, z);
			if (this.isStarted && t.isThing && t.placeState == PlaceState.roaming && !Zone.ignoreSpawnAnime)
			{
				t.PlayAnimeLoot();
			}
			Zone.ignoreSpawnAnime = false;
		}
		return t;
	}

	public void RemoveCard(Card t)
	{
		if (this.IsActiveZone)
		{
			this.map.OnCardRemovedFromZone(t);
		}
		t.parent = null;
		if (t.isChara)
		{
			t.Chara.currentZone = null;
		}
		if (t.isThing && !t.trait.IDActorEx.IsEmpty())
		{
			EClass.scene.RemoveActorEx(t);
		}
		if (t.renderer.hasActor)
		{
			t.renderer.OnLeaveScreen();
		}
	}

	public T GetRandomSpot<T>() where T : Trait
	{
		return EClass._map.props.installed.traits.GetRandomThing<T>() as T;
	}

	public bool TryAddThingInSpot<T>(Thing t, bool useContainer = true, bool putRandomPosIfNoSpot = true) where T : Trait
	{
		Thing randomThing = EClass._map.props.installed.traits.GetRandomThing<T>();
		if (randomThing != null)
		{
			if (useContainer && (!t.IsContainer || t.things.Count == 0))
			{
				List<Thing> list = new List<Thing>();
				foreach (Point point in randomThing.trait.ListPoints(null, false))
				{
					foreach (Card card in point.ListCards(false))
					{
						if (card.IsContainer)
						{
							list.Add(card.Thing);
						}
					}
				}
				if (this.TryAddThingInSharedContainer(t, list, true, false, null, false))
				{
					return true;
				}
			}
			this.AddCard(t, randomThing.trait.GetRandomPoint(null, null));
			return true;
		}
		if (putRandomPosIfNoSpot)
		{
			this.AddCard(t, EClass._map.bounds.GetRandomSurface(false, true, false));
			return true;
		}
		return false;
	}

	public List<Thing> TryListThingsInSpot<T>(Func<Thing, bool> func = null) where T : TraitSpot
	{
		Zone.<>c__DisplayClass255_0<T> CS$<>8__locals1 = new Zone.<>c__DisplayClass255_0<T>();
		CS$<>8__locals1.func = func;
		List<T> list = new List<T>();
		List<Thing> list2 = new List<Thing>();
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.IsInstalled && thing.trait is T)
			{
				list.Add(thing.trait as T);
			}
		}
		foreach (T t in list)
		{
			foreach (Point point in t.ListPoints(null, false))
			{
				foreach (Thing thing2 in point.Things)
				{
					if (thing2.IsInstalled)
					{
						if (thing2.things.Count == 0)
						{
							if (CS$<>8__locals1.<TryListThingsInSpot>g__IsValid|0(thing2))
							{
								list2.Add(thing2);
							}
						}
						else
						{
							ThingContainer things = thing2.things;
							Func<Thing, bool> func2;
							if ((func2 = CS$<>8__locals1.<>9__1) == null)
							{
								func2 = (CS$<>8__locals1.<>9__1 = ((Thing _t) => base.<TryListThingsInSpot>g__IsValid|0(_t)));
							}
							foreach (Thing item in things.List(func2, false))
							{
								list2.Add(item);
							}
						}
					}
				}
			}
		}
		return list2;
	}

	public bool TryAddThingInSharedContainer(Thing t, List<Thing> containers = null, bool add = true, bool msg = false, Chara chara = null, bool sharedOnly = true)
	{
		Zone.<>c__DisplayClass256_0 CS$<>8__locals1;
		CS$<>8__locals1.containers = containers;
		CS$<>8__locals1.sharedOnly = sharedOnly;
		CS$<>8__locals1.add = add;
		CS$<>8__locals1.t = t;
		CS$<>8__locals1.msg = msg;
		CS$<>8__locals1.chara = chara;
		CS$<>8__locals1.dest = null;
		CS$<>8__locals1.priority = -1;
		CS$<>8__locals1.flag = CS$<>8__locals1.t.category.GetRoot().id.ToEnum(true);
		if (CS$<>8__locals1.flag == ContainerFlag.none)
		{
			CS$<>8__locals1.flag = ContainerFlag.other;
		}
		if (CS$<>8__locals1.containers == null)
		{
			CS$<>8__locals1.containers = EClass._map.props.installed.containers;
		}
		if (Zone.<TryAddThingInSharedContainer>g__SearchDest|256_0(ref CS$<>8__locals1) != null)
		{
			return true;
		}
		if (CS$<>8__locals1.dest == null)
		{
			return false;
		}
		if (CS$<>8__locals1.add)
		{
			if (CS$<>8__locals1.msg)
			{
				CS$<>8__locals1.chara.Say("putSharedItem", CS$<>8__locals1.chara, CS$<>8__locals1.t, CS$<>8__locals1.dest.GetName(NameStyle.Full, -1), null);
			}
			CS$<>8__locals1.dest.AddThing(CS$<>8__locals1.t, true, -1, -1);
		}
		return true;
	}

	public bool TryAddThing(Thing target, Point p, bool destroyIfFail = false)
	{
		int num = 0;
		if (p.cell.detail != null)
		{
			foreach (Thing thing in p.cell.detail.things)
			{
				if (thing.placeState == PlaceState.roaming)
				{
					if (target.TryStackTo(thing))
					{
						return true;
					}
					num++;
				}
			}
		}
		if (num == 0 || !destroyIfFail)
		{
			EClass._zone.AddCard(target, p);
			return true;
		}
		target.Destroy();
		return false;
	}

	public Thing TryGetThingFromSharedContainer(Func<Thing, bool> func)
	{
		foreach (Thing thing in EClass._map.props.installed.containers)
		{
			if (thing.IsSharedContainer)
			{
				Thing thing2 = thing.things.Find(func, true);
				if (thing2 != null)
				{
					return thing2;
				}
			}
		}
		return null;
	}

	public Thing TryGetRestock<T>(string idCat) where T : TraitSpot
	{
		Zone.<>c__DisplayClass259_0<T> CS$<>8__locals1 = new Zone.<>c__DisplayClass259_0<T>();
		CS$<>8__locals1.idCat = idCat;
		List<T> list = new List<T>();
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.IsInstalled && thing.trait is T)
			{
				list.Add(thing.trait as T);
			}
		}
		foreach (T t in list)
		{
			foreach (Point point in t.ListPoints(null, false))
			{
				foreach (Thing thing2 in point.Things)
				{
					if (thing2.IsInstalled && !thing2.isSale)
					{
						if (thing2.things.Count == 0)
						{
							if (CS$<>8__locals1.<TryGetRestock>g__IsValid|0(thing2, false))
							{
								return thing2;
							}
						}
						else
						{
							ThingContainer things = thing2.things;
							Func<Thing, bool> func;
							if ((func = CS$<>8__locals1.<>9__1) == null)
							{
								func = (CS$<>8__locals1.<>9__1 = ((Thing _t) => base.<TryGetRestock>g__IsValid|0(_t, true)));
							}
							Thing thing3 = things.Find(func, true);
							if (thing3 != null)
							{
								return thing3;
							}
						}
					}
				}
			}
		}
		return null;
	}

	public ZoneProfile GetProfile()
	{
		string text = this.IdProfile;
		if (text.IsEmpty())
		{
			Region region = this.parent as Region;
			if (base.lv != 0)
			{
				text = ((base.lv < 0) ? "Underground" : "Sky");
			}
			else if (region != null)
			{
				EClass.scene.elomapActor.Initialize(region.elomap);
				text = EClass.scene.elomapActor.elomap.GetTileInfo(base.x, base.y).idZoneProfile;
				if (this.bp != null)
				{
					this.name = Lang.GetList("zone_" + text.Split('/', StringSplitOptions.None)[1]).RandomItem<string>();
					this.bp.surrounding = new EloMap.TileInfo[3, 3];
					for (int i = 0; i < 3; i++)
					{
						for (int j = 0; j < 3; j++)
						{
							this.bp.surrounding[j, i] = EClass.scene.elomapActor.elomap.GetTileInfo(base.x - 1 + j, base.y - 1 + i);
						}
					}
					if (text == "Random/R_Shore")
					{
						base.isBeach = true;
					}
				}
			}
			else
			{
				text = "Random";
			}
			this.idProfile = text;
		}
		return ZoneProfile.Load(text);
	}

	public void CreateBP()
	{
		this.bp = new ZoneBlueprint();
		this.bp.Create();
		this.bp.genSetting.seed = base.Seed;
		this.OnCreateBP();
	}

	public virtual void OnCreateBP()
	{
	}

	public void Generate()
	{
		base.isGenerated = true;
		if (this.bp == null)
		{
			this.CreateBP();
		}
		if (this.bp.map == null)
		{
			this.bp.GenerateMap(this);
		}
		this.map.SetZone(this);
		if (this is Zone_Field)
		{
			if (EClass.rnd(3) == 0)
			{
				int num = EClass.rnd(2);
				for (int i = 0; i < num; i++)
				{
					Point randomSurface = EClass._map.bounds.GetRandomSurface(false, true, false);
					if (!randomSurface.HasObj)
					{
						Card t = ThingGen.Create("chest3", -1, -1).ChangeMaterial(this.biome.style.matDoor);
						EClass._zone.AddCard(t, randomSurface).Install();
					}
				}
			}
			if (EClass.rnd(8) == 0)
			{
				this.SpawnAltar();
			}
			this.TrySpawnFollower();
		}
		this.map.plDay = this.CreatePlaylist(ref this.map._plDay, EClass.Sound.GetPlaylist(this.IDPlayList) ?? EClass.Sound.GetPlaylist("Day"));
	}

	public void TrySpawnFollower()
	{
		bool flag = EClass.pc.HasCondition<ConDrawBacker>();
		if (!EClass.debug.enable && EClass.rnd(flag ? 3 : 20) != 0)
		{
			return;
		}
		Point randomSurface = EClass._map.bounds.GetRandomSurface(false, true, false);
		if (!randomSurface.IsValid)
		{
			return;
		}
		Chara c = CharaGen.Create("follower", -1);
		EClass._zone.AddCard(c, randomSurface);
		(EClass._zone.AddThing("gallows", randomSurface).Install().trait as TraitShackle).Restrain(c, false);
		c.c_rescueState = RescueState.WaitingForRescue;
		if (EClass.rnd(flag ? 1 : 2) == 0 || EClass.debug.enable)
		{
			SourceBacker.Row row = EClass.sources.backers.listFollower.NextItem(ref BackerContent.indexFollower);
			if (row != null)
			{
				c.ApplyBacker(row.id);
			}
		}
		Religion faith = (from a in EClass.game.religions.dictAll.Values
		where a != c.faith
		select a).RandomItem<Religion>();
		for (int i = 0; i < 3 + EClass.rnd(4); i++)
		{
			Chara chara = CharaGen.Create("fanatic", -1);
			chara.SetFaith(faith);
			Point point = randomSurface.GetRandomPoint(4, true, true, false, 100) ?? randomSurface.GetNearestPoint(false, true, true, false);
			EClass._zone.AddCard(chara, point);
		}
	}

	public void SpawnAltar()
	{
		EClass.core.refs.crawlers.start.CrawlUntil(this.map, () => this.map.poiMap.GetCenterCell(1).GetCenter(), 1, delegate(Crawler.Result r)
		{
			if (r.points.Count <= 4)
			{
				return false;
			}
			this.map.poiMap.OccyupyPOI(r.points[0], 0);
			List<Point> points = r.points;
			Religion randomReligion = EClass.game.religions.GetRandomReligion(true, false);
			"altarPoint".lang(randomReligion.NameDomain.lang(), null, null, null, null);
			Thing thing = ThingGen.Create("altar", -1, -1);
			(thing.trait as TraitAltar).SetDeity(randomReligion.id);
			Chara t = CharaGen.Create("twintail", -1);
			EClass._zone.AddCard(t, points.RandomItem<Point>());
			for (int i = 0; i < 2 + EClass.rnd(2); i++)
			{
				Chara t2 = CharaGen.Create("twintail", -1);
				EClass._zone.AddCard(t2, points.RandomItem<Point>());
			}
			if (points[0].Installed == null)
			{
				EClass._zone.AddCard(thing, points[0]).Install();
			}
			foreach (Point point in points)
			{
				if (point.x % 3 == 0 && point.z % 2 == 0 && point != points[0] && !point.Equals(points[0].Front) && point.Installed == null)
				{
					thing = ThingGen.Create("pillar1", -1, -1);
					EClass._zone.AddCard(thing, point).Install();
				}
				point.SetObj(0, 1, 0);
				point.SetFloor(3, 6);
			}
			return true;
		}, null);
	}

	public virtual void OnGenerateMap()
	{
		if (this.MakeEnemiesNeutral)
		{
			foreach (Chara chara in EClass._map.charas)
			{
				if (!chara.IsGlobal && chara.hostility < Hostility.Neutral && chara.OriginalHostility < Hostility.Friend)
				{
					chara.hostility = (chara.c_originalHostility = Hostility.Neutral);
				}
			}
		}
		if (this.PrespawnRate != 0f && !this.IsSkyLevel)
		{
			for (int i = 0; i < (int)((float)this.MaxSpawn * this.PrespawnRate); i++)
			{
				this.SpawnMob(null, null);
			}
		}
		this.TryGenerateOre();
		this.TryGenerateBigDaddy();
		this.TryGenerateEvolved(false, null);
		this.TryGenerateShrine();
	}

	public void TryGenerateOre()
	{
		if (this.OreChance <= 0f)
		{
			return;
		}
		Crawler crawler = Crawler.Create("ore");
		int tries = EClass.rnd((int)((float)(this.map.bounds.Width * this.map.bounds.Height / 200 + 1) * this.OreChance + 2f));
		crawler.CrawlUntil(EClass._map, () => EClass._map.bounds.GetRandomPoint(), tries, delegate(Crawler.Result r)
		{
			byte b = 18;
			string group = "ore";
			if (EClass.rnd(5) == 0)
			{
				b += 1;
				group = "gem";
			}
			SourceMaterial.Row randomMaterial = MATERIAL.GetRandomMaterial(this.DangerLv, group, false);
			foreach (Point point in r.points)
			{
				if (point.sourceBlock.ContainsTag("ore"))
				{
					this.map.SetObj(point.x, point.z, randomMaterial.id, (int)b, 1, 0);
				}
			}
			return false;
		}, null);
	}

	public Chara TryGenerateEvolved(bool force = false, Point p = null)
	{
		if (!force && this.EvolvedChance <= EClass.rndf(1f))
		{
			return null;
		}
		Chara chara = this.SpawnMob(p, SpawnSetting.Evolved(-1));
		for (int i = 0; i < 2 + EClass.rnd(2); i++)
		{
			chara.ability.AddRandom();
		}
		chara.AddThing(chara.MakeGene(new DNA.Type?(DNA.Type.Default)), true, -1, -1);
		if (EClass.rnd(2) == 0)
		{
			chara.AddThing(chara.MakeGene(new DNA.Type?(DNA.Type.Superior)), true, -1, -1);
		}
		return chara;
	}

	public void TryGenerateBigDaddy()
	{
		if (this.BigDaddyChance <= EClass.rndf(1f))
		{
			return;
		}
		int num = this.DangerLv * 125 / 100;
		if (num >= 30)
		{
			CardBlueprint.Set(new CardBlueprint
			{
				lv = num
			});
		}
		Chara t = CharaGen.Create("big_daddy", -1);
		EClass._zone.AddCard(t, this.GetSpawnPos(SpawnPosition.Random, 10000));
		Msg.Say("sign_bigdaddy");
	}

	public void TryGenerateShrine()
	{
		for (int i = 0; i < 3; i++)
		{
			Rand.SetSeed(base.uid + i);
			if (this.ShrineChance > EClass.rndf(1f))
			{
				Point randomSpace = EClass._map.GetRandomSpace(3, 3, 100);
				if (randomSpace != null)
				{
					randomSpace.x++;
					randomSpace.z++;
					if (!randomSpace.HasThing && !randomSpace.HasChara)
					{
						randomSpace.SetObj(0, 1, 0);
						Rand.SetSeed(EClass.player.seedShrine);
						EClass.player.seedShrine++;
						if (EClass.rnd(EClass.debug.test ? 2 : 15) == 0)
						{
							EClass._zone.AddCard(ThingGen.Create("pedestal_power", -1, -1), randomSpace).Install();
							EClass._zone.AddCard(ThingGen.Create(EClass.gamedata.godStatues.RandomItemWeighted((GodStatueData a) => a.chance).idThing, -1, this.DangerLv), randomSpace).Install();
						}
						else
						{
							EClass._zone.AddCard(ThingGen.Create("statue_power", -1, this.DangerLv), randomSpace).Install().SetRandomDir();
						}
					}
				}
			}
		}
		Rand.SetSeed(-1);
	}

	public void ResetHostility()
	{
		foreach (Chara chara in EClass._map.charas)
		{
			if (!chara.source.hostility.IsEmpty() && chara.source.hostility.ToEnum(true) >= Hostility.Friend && !chara.IsPCFactionOrMinion)
			{
				chara.c_originalHostility = (Hostility)0;
			}
			chara.hostility = chara.OriginalHostility;
			if (chara.enemy != null && (chara.enemy.IsPCFaction || chara.IsPCFaction))
			{
				chara.SetEnemy(null);
			}
		}
	}

	public virtual void OnGenerateRooms(BaseMapGen gen)
	{
	}

	public Point GetSpawnPos(SpawnPosition type, int tries = 100)
	{
		Point point = new Point();
		for (int i = 0; i < tries; i++)
		{
			point = EClass._map.bounds.GetRandomSurface(false, true, true);
			if (point.IsValid && !point.cell.hasDoor && !point.IsSync && (type != SpawnPosition.Outside || (!point.cell.HasRoof && point.cell.light <= 0)))
			{
				return point;
			}
		}
		return null;
	}

	public Chara SpawnMob(Point pos = null, SpawnSetting setting = null)
	{
		if (setting == null)
		{
			setting = SpawnSetting.Default;
		}
		if (pos == null)
		{
			pos = this.GetSpawnPos(setting.position, setting.tries);
			if (pos == null)
			{
				pos = this.GetSpawnPos(SpawnPosition.Random, setting.tries);
				if (pos == null)
				{
					return null;
				}
			}
		}
		BiomeProfile biome = pos.cell.biome;
		SpawnList spawnList;
		if (setting.idSpawnList != null)
		{
			spawnList = SpawnList.Get(setting.idSpawnList, null, null);
		}
		else if (setting.hostility == SpawnHostility.Neutral || (setting.hostility != SpawnHostility.Enemy && Rand.Range(0f, 1f) < this.ChanceSpawnNeutral))
		{
			spawnList = SpawnList.Get("c_neutral", null, null);
		}
		else if (biome.spawn.chara.Count > 0)
		{
			spawnList = SpawnList.Get(biome.spawn.GetRandomCharaId(), null, null);
		}
		else
		{
			spawnList = SpawnList.Get(biome.name, "chara", new CharaFilter
			{
				ShouldPass = ((SourceChara.Row s) => !(s.hostility != "") && (s.biome == biome.name || s.biome.IsEmpty()))
			});
		}
		int dangerLv = this.DangerLv;
		CardBlueprint cardBlueprint = new CardBlueprint
		{
			rarity = Rarity.Normal
		};
		int lv = (setting.filterLv == -1) ? dangerLv : setting.filterLv;
		if (this.ScaleMonsterLevel)
		{
			lv = ((dangerLv - 1) % 50 + 5) * 150 / 100;
		}
		CardRow cardRow = setting.id.IsEmpty() ? spawnList.Select(lv, setting.levelRange) : EClass.sources.cards.map[setting.id];
		int num = (setting.fixedLv == -1) ? cardRow.LV : setting.fixedLv;
		if (this.ScaleMonsterLevel)
		{
			num = (50 + cardRow.LV) * Mathf.Max(1, (dangerLv - 1) / 50);
		}
		if (setting.rarity == Rarity.Random)
		{
			if (EClass.rnd(100) == 0)
			{
				cardBlueprint.rarity = Rarity.Legendary;
				num = num * 125 / 100;
			}
		}
		else
		{
			cardBlueprint.rarity = setting.rarity;
		}
		if (setting.isBoss)
		{
			num = num * 150 / 100;
		}
		if (setting.isEvolved)
		{
			num = num * 2 + 20;
		}
		if (num != cardRow.LV)
		{
			cardBlueprint.lv = num;
		}
		CardBlueprint.Set(cardBlueprint);
		Chara chara = CharaGen.Create(cardRow.id, lv);
		this.AddCard(chara, pos);
		if (setting.forcedHostility != null)
		{
			chara.c_originalHostility = (chara.hostility = setting.forcedHostility.Value);
		}
		if (setting.isBoss)
		{
			chara.c_bossType = BossType.Boss;
		}
		if (setting.isEvolved)
		{
			chara.c_bossType = BossType.Evolved;
		}
		return chara;
	}

	public void RefreshElectricity()
	{
		this.dirtyElectricity = false;
		base.electricity = this.elements.Value(2201) * 10 + this.BaseElectricity;
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.IsInstalled && thing.trait.Electricity != 0 && (thing.isOn || thing.trait.Electricity > 0))
			{
				base.electricity += thing.trait.Electricity;
			}
		}
		foreach (Thing thing2 in EClass._map.things)
		{
			if (thing2.IsInstalled)
			{
				thing2.trait.TryToggle();
			}
		}
	}

	public int GetElectricity(bool cost = false)
	{
		int num = 0;
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.IsInstalled && thing.trait.Electricity != 0)
			{
				if (cost)
				{
					if (thing.trait.Electricity < 0)
					{
						num += -thing.trait.Electricity;
					}
				}
				else if (thing.trait.Electricity > 0)
				{
					num += thing.trait.Electricity;
				}
			}
		}
		if (!cost)
		{
			num += this.elements.Value(2201) * 10;
		}
		return num;
	}

	public void SetBGM(List<int> ids, bool refresh = true)
	{
		this.map._plDay.Clear();
		if (ids.Count > 0)
		{
			foreach (int num in ids)
			{
				if (num != -1)
				{
					this.map._plDay.Add(num);
				}
			}
		}
		UnityEngine.Object.DestroyImmediate(this.map.plDay);
		this.map.plDay = null;
		this.RefreshPlaylist();
		if (refresh)
		{
			EClass.Sound.StopBGM(0f, false);
			this.RefreshBGM();
		}
	}

	public void SetBGM(int id = -1, bool refresh = true)
	{
		this.SetBGM(new List<int>
		{
			id
		}, refresh);
	}

	public void RefreshPlaylist()
	{
		if (this.map.plDay == null)
		{
			this.map.plDay = this.CreatePlaylist(ref this.map._plDay, EClass.Sound.GetPlaylist(this.IDPlayList));
		}
	}

	public void RefreshBGM()
	{
		if (!EClass.pc.IsInActiveZone || EClass.player.simulatingZone)
		{
			return;
		}
		this.RefreshPlaylist();
		Playlist playlist = this.map.plDay;
		foreach (ZoneEvent zoneEvent in this.events.list)
		{
			if (zoneEvent.playlist != null)
			{
				playlist = zoneEvent.playlist;
			}
		}
		if (this.IDPlaylistOverwrite != null)
		{
			playlist = EClass.Sound.GetPlaylist(this.IDPlaylistOverwrite);
		}
		if (EClass.pc.IsInActiveZone)
		{
			global::Room room = EClass.pc.pos.cell.room;
			if (room != null && room.lot != null && room.lot.idBGM != 0)
			{
				playlist = EClass.Sound.plLot;
				BGMData data = playlist.list[0].data;
				int? num = (data != null) ? new int?(data.id) : null;
				int idBGM = room.lot.idBGM;
				if (!(num.GetValueOrDefault() == idBGM & num != null))
				{
					playlist.list[0].data = EClass.core.refs.dictBGM.TryGetValue(room.lot.idBGM, null);
					playlist.Reset();
					if (!LayerDrama.keepBGM)
					{
						EClass.Sound.StopBGM(1f, false);
					}
				}
			}
		}
		EClass.core.config.SetBGMInterval();
		EClass.Sound.SwitchPlaylist(playlist, !LayerDrama.keepBGM);
	}

	public Playlist CreatePlaylist(ref List<int> list, Playlist mold = null)
	{
		Playlist playlist = EClass.Sound.plBlank.Instantiate<Playlist>();
		if (list.Count == 0 && mold)
		{
			list = mold.ToInts();
			playlist.shuffle = mold.shuffle;
			playlist.minSwitchTime = mold.minSwitchTime;
			playlist.nextBGMOnSwitch = mold.nextBGMOnSwitch;
			playlist.ignoreLoop = mold.ignoreLoop;
			playlist.keepBGMifSamePlaylist = mold.keepBGMifSamePlaylist;
			playlist.name = mold.name;
		}
		foreach (int key in list)
		{
			playlist.list.Add(new Playlist.Item
			{
				data = EClass.core.refs.dictBGM[key]
			});
		}
		return playlist;
	}

	public Chara FindChara(string id)
	{
		return this.map.charas.Find((Chara c) => c.id == id);
	}

	public Chara FindChara(int uid)
	{
		return this.map.charas.Find((Chara c) => c.uid == uid);
	}

	public int GetDeepestLv()
	{
		int lv = base.lv;
		return this.GetDeepestLv(ref lv);
	}

	public int GetDeepestLv(ref int max)
	{
		if (Mathf.Abs(base.lv) > Mathf.Abs(max))
		{
			max = base.lv;
		}
		foreach (Spatial spatial in this.children)
		{
			(spatial as Zone).GetDeepestLv(ref max);
		}
		return max;
	}

	public List<Element> ListLandFeats()
	{
		if (this.landFeats == null)
		{
			this.landFeats = new List<int>();
			Rand.SetSeed(EClass._zone.uid);
			string[] listBase = this.IDBaseLandFeat.Split(',', StringSplitOptions.None);
			foreach (string text in listBase)
			{
				if (!text.IsEmpty())
				{
					this.landFeats.Add(EClass.sources.elements.alias[text].id);
				}
			}
			if (listBase.Length == 1)
			{
				List<SourceElement.Row> list = EClass.sources.elements.rows.Where(delegate(SourceElement.Row e)
				{
					if (e.category != "landfeat" || e.chance == 0)
					{
						return false;
					}
					bool result = true;
					foreach (string text2 in e.tag)
					{
						if (text2.StartsWith("bf"))
						{
							result = false;
							if (listBase[0] == text2)
							{
								result = true;
								break;
							}
						}
					}
					return result;
				}).ToList<SourceElement.Row>();
				SourceElement.Row row = list.RandomItemWeighted((SourceElement.Row e) => (float)e.chance);
				this.landFeats.Add(row.id);
				list.Remove(row);
				row = list.RandomItemWeighted((SourceElement.Row e) => (float)e.chance);
				this.landFeats.Add(row.id);
			}
			Rand.SetSeed(-1);
		}
		List<Element> list2 = new List<Element>();
		foreach (int id in this.landFeats)
		{
			list2.Add(Element.Create(id, 0));
		}
		return list2;
	}

	public ZoneExportData Import(string path)
	{
		ZipFile zipFile = ZipFile.Read(path);
		zipFile.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
		zipFile.ExtractAll(this.pathTemp);
		zipFile.Dispose();
		return IO.LoadFile<ZoneExportData>(this.pathTemp + "export", false, null) ?? new ZoneExportData();
	}

	public void Export(string path, PartialMap partial = null, bool usermap = false)
	{
		if (this.subset != null)
		{
			SE.Beep();
			return;
		}
		try
		{
			ZoneExportData zoneExportData = new ZoneExportData
			{
				name = this.name,
				usermap = usermap
			};
			IO.CreateTempDirectory(null);
			if (!this.map.config.retainDecal)
			{
				this.map.ClearRainAndDecal();
			}
			this.map.Save(IO.TempPath + "/", zoneExportData, partial);
			this.map.ExportMetaData(IO.TempPath + "/", Path.GetFileNameWithoutExtension(path), partial);
			if (partial == null)
			{
				IO.CopyDir(base.pathSave + "Texture Replace", IO.TempPath + "/Texture Replace", null);
			}
			IO.SaveFile(IO.TempPath + "/export", zoneExportData, true, null);
			using (ZipFile zipFile = new ZipFile())
			{
				zipFile.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
				zipFile.AddDirectory(IO.TempPath);
				zipFile.Save(path);
				zipFile.Dispose();
			}
			IO.DeleteTempDirectory(null);
		}
		catch (Exception ex)
		{
			EClass.ui.Say(ex.Message + ":" + path, null);
		}
	}

	public void ExportDialog(string dir = null)
	{
		EClass.core.WaitForEndOfFrame(delegate
		{
			string text = StandaloneFileBrowser.SaveFilePanel("Export Zone", dir ?? CorePath.ZoneSave, "new zone", "z");
			if (!string.IsNullOrEmpty(text))
			{
				this.Export(text, null, false);
				Msg.SayRaw("Exported Zone");
			}
		});
	}

	public void ImportDialog(string dir = null)
	{
		EClass.core.WaitForEndOfFrame(delegate
		{
			string[] array = StandaloneFileBrowser.OpenFilePanel("Import Zone", dir ?? CorePath.ZoneSave, "z", false);
			if (array.Length != 0)
			{
				Zone_User zone_User = SpatialGen.Create("user", EClass.world.region, true, -99999, -99999, 0) as Zone_User;
				zone_User.path = array[0];
				Thing thing = ThingGen.Create("teleporter", -1, -1);
				thing.c_uidZone = zone_User.uid;
				EClass._zone.AddCard(thing, EClass.pc.pos);
				return;
			}
		});
	}

	public static bool IsImportValid(string path)
	{
		bool result;
		try
		{
			MapMetaData metaData = Map.GetMetaData(path);
			result = (metaData != null && metaData.IsValidVersion());
		}
		catch (Exception ex)
		{
			EClass.ui.Say(ex.Message, null);
			result = false;
		}
		return result;
	}

	public void Export()
	{
		EClass._map.ResetEditorPos();
		string pathExport = this.pathExport;
		IO.Copy(pathExport, CorePath.ZoneSave + "Backup/");
		this.Export(pathExport, null, false);
		Msg.Say("Exported Map:" + pathExport);
	}

	public void WriteNote(UINote n, Action<UINote> onWriteNote = null, IInspect.NoteMode mode = IInspect.NoteMode.Default, Recipe recipe = null)
	{
	}

	public void OnInspect()
	{
	}

	public int GetSortVal()
	{
		if (this.IsPCFaction)
		{
			return -100000 + base.uid;
		}
		if (this is Zone_Civilized)
		{
			return -90000 + base.uid;
		}
		return base.uid;
	}

	public Chara AddRandomVisitor(bool guest = false)
	{
		Trait random = this.map.Installed.traits.GetTraitSet<TraitSpotExit>().GetRandom();
		if (random == null)
		{
			return null;
		}
		Point point = random.GetPoint();
		Chara chara;
		if (guest)
		{
			Zone z = EClass.world.FindZone("wilds");
			chara = EClass.game.cards.ListGlobalChara(z).RandomItem<Chara>();
			if (chara != null)
			{
				this.AddCard(chara, point);
				Msg.Say("guestArrive", chara, null, null, null);
				chara.visitorState = VisitorState.Arrived;
			}
		}
		else
		{
			chara = CharaGen.CreateFromFilter("c_wilds", -1, -1);
			this.AddCard(chara, point);
			chara.goalListType = GoalListType.Enemy;
		}
		return chara;
	}

	public void OnSimulateHour(VirtualDate date)
	{
		if (base.IsPlayerFaction)
		{
			this.branch.OnSimulateHour(date);
		}
		this.events.OnSimulateHour();
		if (date.IsRealTime)
		{
			foreach (Thing thing in EClass._map.things)
			{
				if (thing.IsInstalled)
				{
					thing.trait.TryToggle();
				}
			}
			EClass.pc.DecayNatural(1);
		}
		EClass._map.things.ForeachReverse(delegate(Thing t)
		{
			t.OnSimulateHour(date);
		});
		foreach (Thing thing2 in Zone.Suckers)
		{
			thing2.Destroy();
		}
		Zone.Suckers.Clear();
		if (this.RespawnRate != 0f)
		{
			int num = 0;
			using (List<Chara>.Enumerator enumerator2 = this.map.charas.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (!enumerator2.Current.IsGlobal)
					{
						num++;
					}
				}
			}
			if (num < this.MaxRespawn)
			{
				for (int i = 0; i < this.RespawnPerHour; i++)
				{
					this.SpawnMob(null, null);
				}
			}
		}
		if (!date.IsRealTime && EClass.rnd(24) == 0)
		{
			this.RainWater();
		}
		if (date.hour == 6)
		{
			this.GrowPlants(date);
		}
	}

	public void OnSimulateDay(VirtualDate date)
	{
		if (base.IsPlayerFaction)
		{
			this.branch.OnSimulateDay(date);
		}
	}

	public void OnSimulateMonth(VirtualDate date)
	{
		if (base.IsPlayerFaction)
		{
			this.branch.OnSimulateMonth(date);
		}
		if (date.IsRealTime)
		{
			EClass._map.RefreshAllTiles();
		}
	}

	public void RainWater()
	{
		if (EClass._map.IsIndoor || !this.IsPCFaction)
		{
			return;
		}
		EClass._map.bounds.ForeachCell(delegate(global::Cell c)
		{
			if (c.IsFarmField && !c.HasRoof)
			{
				c.isWatered = true;
			}
		});
	}

	public void GrowPlants(VirtualDate date)
	{
		Zone.<>c__DisplayClass302_0 CS$<>8__locals1 = new Zone.<>c__DisplayClass302_0();
		CS$<>8__locals1.date = date;
		CS$<>8__locals1.<>4__this = this;
		bool flag = EClass.player.isAutoFarming = (this.IsPCFaction && EClass.Branch.policies.IsActive(2707, -1));
		CS$<>8__locals1.weedChance = 1;
		if (this.IsPCFaction && EClass.Branch.policies.IsActive(2703, -1))
		{
			CS$<>8__locals1.weedChance += (EClass.debug.enable ? 100000 : 20) + EClass.Branch.Evalue(2703) * 10;
		}
		if (CS$<>8__locals1.date.sunMap == null)
		{
			CS$<>8__locals1.date.BuildSunMap();
		}
		if (flag)
		{
			HashSet<int> hashSet = new HashSet<int>();
			foreach (Thing thing in EClass._map.things)
			{
				if (thing.IsInstalled)
				{
					TraitSpotFarm traitSpotFarm = thing.trait as TraitSpotFarm;
					if (traitSpotFarm != null)
					{
						foreach (Point point in traitSpotFarm.ListPoints(null, false))
						{
							hashSet.Add(point.index);
						}
					}
				}
			}
			CS$<>8__locals1.<GrowPlants>g__Perform|0(hashSet);
			EClass.player.isAutoFarming = false;
			CS$<>8__locals1.<GrowPlants>g__Perform|0(hashSet);
			return;
		}
		CS$<>8__locals1.<GrowPlants>g__Perform|0(null);
	}

	public Zone GetZoneAt(int _x, int _y)
	{
		if (this.IsRegion)
		{
			foreach (Spatial spatial in this.children)
			{
				if (!(spatial is Zone_Field) && _x == spatial.x && _y == spatial.y)
				{
					return spatial as Zone;
				}
			}
		}
		foreach (Spatial spatial2 in this.children)
		{
			if (_x == spatial2.x && _y == spatial2.y)
			{
				return spatial2 as Zone;
			}
		}
		return null;
	}

	public bool IsCrime(Chara c, Act act)
	{
		return act.IsHostileAct && this.HasLaw && !this.IsPCFaction && c.IsPC;
	}

	public void RefreshCriminal()
	{
		bool flag = EClass.player.IsCriminal && (this.HasLaw && !this.AllowCriminal) && !this.IsPCFaction;
		Hostility hostility = flag ? Hostility.Neutral : Hostility.Friend;
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.trait is TraitGuard)
			{
				chara.hostility = hostility;
				if (!flag && chara.enemy != null && chara.enemy.IsPCParty)
				{
					chara.SetEnemy(null);
				}
			}
		}
	}

	public void RefreshListCitizen()
	{
		if (base.lv != 0)
		{
			return;
		}
		this.dictCitizen.Clear();
		foreach (Chara chara in this.map.charas.Concat(this.map.deadCharas))
		{
			if (chara.trait.IsCitizen && !chara.IsGlobal && !chara.isSubsetCard)
			{
				this.dictCitizen[chara.uid] = chara.Name;
			}
		}
	}

	public void ModInfluence(int a)
	{
		base.influence += a;
		if (a > 0)
		{
			Msg.Say("gainInfluence", this.Name, a.ToString() ?? "", null, null);
		}
		Tutorial.Reserve("influence", null);
	}

	public void ModDevelopment(int a)
	{
		base.development += a;
		if (a > 0)
		{
			Msg.Say("gainDevelopment", this.Name, a.ToString() ?? "", null, null);
		}
	}

	public void UpdateQuests(bool force = false)
	{
		if (!this.IsPCFaction && (!(this is Zone_Town) || base.lv != 0))
		{
			return;
		}
		Debug.Log("Updating Quest:" + force.ToString());
		List<SourceQuest.Row> list = (from a in EClass.sources.quests.rows
		where a.@group == "random"
		select a).ToList<SourceQuest.Row>();
		int num = 0;
		foreach (Chara chara in this.map.charas.Concat(this.map.deadCharas))
		{
			if (chara.quest != null && !EClass.game.quests.list.Contains(chara.quest))
			{
				if (chara.quest.IsExpired || this.completedQuests.Contains(chara.quest.uid) || force)
				{
					chara.quest = null;
				}
				else
				{
					num++;
				}
			}
		}
		if (EClass._zone.dateQuest > EClass.world.date.GetRaw(0) && !force)
		{
			return;
		}
		EClass._zone.dateQuest = EClass.world.date.GetRaw(0) + 1440;
		int maxQuest = 3;
		Rand.UseSeed(base.uid + EClass.player.stats.days / 7 % 100, delegate
		{
			maxQuest = 4 + EClass.rnd(4);
		});
		this.completedQuests.Clear();
		List<Zone> list2 = Quest.ListDeliver();
		List<Tuple<string, int>> listTag = new List<Tuple<string, int>>();
		string[] array = EClass._zone.source.questTag;
		if (EClass._zone.IsPCFaction)
		{
			array = new string[]
			{
				"supply/8",
				"deliver/7",
				"food/8",
				"escort/4",
				"deliver/4",
				"monster/0",
				"war/0",
				"farm/0",
				"music/0"
			};
		}
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			string[] array3 = array2[i].Split('/', StringSplitOptions.None);
			listTag.Add(new Tuple<string, int>(array3[0], array3[1].ToInt()));
		}
		int num2 = 0;
		Func<SourceQuest.Row, float> <>9__2;
		while (num2 < this.map.charas.Count * 2 && num <= maxQuest && num <= 15)
		{
			Chara chara2 = this.map.charas.RandomItem<Chara>();
			if (chara2.trait.CanGiveRandomQuest && !chara2.isSubsetCard && chara2.homeZone == EClass._zone && !chara2.IsGuest() && chara2.memberType != FactionMemberType.Livestock && (chara2.quest == null || force))
			{
				IList<SourceQuest.Row> source = list;
				Func<SourceQuest.Row, float> getWeight;
				if ((getWeight = <>9__2) == null)
				{
					getWeight = (<>9__2 = delegate(SourceQuest.Row a)
					{
						int num3 = 1;
						foreach (Tuple<string, int> tuple in listTag)
						{
							if (a.tags.Contains(tuple.Item1))
							{
								num3 = tuple.Item2;
								break;
							}
						}
						if (!EClass._zone.IsPCFaction && a.tags.Contains("bulk"))
						{
							num3 = 0;
						}
						return (float)(a.chance * num3);
					});
				}
				SourceQuest.Row row = source.RandomItemWeighted(getWeight);
				if ((!row.tags.Contains("needDestZone") || list2.Count >= 2) && (row.minFame <= 0 || row.minFame < EClass.player.fame || EClass.debug.enable))
				{
					Quest.Create(row.id, null, chara2);
					num++;
				}
			}
			num2++;
		}
	}

	public int CountMinions(Chara c)
	{
		int num = 0;
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.c_uidMaster == c.uid && chara.c_minionType == MinionType.Default)
			{
				num++;
			}
		}
		return num;
	}

	public int GetSoilCost()
	{
		int n = 0;
		EClass._map.bounds.ForeachCell(delegate(global::Cell c)
		{
			n += c.sourceObj.costSoil;
		});
		return n / 10;
	}

	public void SpawnLostItems()
	{
		for (int i = 0; i < 2 + EClass.rnd(4); i++)
		{
			Point point = Zone.<SpawnLostItems>g__GetPos|312_0();
			if (point != null)
			{
				if (EClass.rnd(30) == 0)
				{
					Thing thing = ThingGen.Create("purse", -1, -1);
					thing.isLostProperty = true;
					thing.things.DestroyAll(null);
					int num = EClass.rndHalf(Mathf.Min(base.development / 10 + 10, 50));
					thing.c_lockLv = num;
					thing.Add("money", EClass.rndHalf(num * 60 + 1000), 1);
					if (EClass.rnd(2) == 0)
					{
						thing.Add("plat", EClass.rnd(4), 1);
					}
					else
					{
						thing.Add("medal", EClass.rnd(2), 1);
					}
					EClass._zone.AddCard(thing, point);
				}
				else
				{
					EClass._zone.AddCard(ThingGen.CreateFromCategory("junk", -1), point);
				}
			}
		}
	}

	public void ApplyBackerPet(bool draw)
	{
		bool flag = this is Zone_Yowyn && base.lv == -1;
		IList<SourceBacker.Row> list = EClass.sources.backers.listPet.Copy<SourceBacker.Row>();
		list.Shuffle<SourceBacker.Row>();
		if (!EClass.core.config.test.ignoreBackerDestoryFlag)
		{
			list.ForeachReverse(delegate(SourceBacker.Row a)
			{
				if (EClass.player.doneBackers.Contains(a.id))
				{
					list.Remove(a);
				}
			});
		}
		foreach (Chara chara in EClass._map.charas)
		{
			if (!chara.IsGlobal && !chara.IsMinion)
			{
				if (chara.isBackerContent)
				{
					if (chara.sourceBacker.isStatic != 0)
					{
						continue;
					}
					if (chara.id != "follower")
					{
						chara.RemoveBacker();
					}
				}
				if ((!flag || !(chara.race.id != "cat")) && EClass.rnd(flag ? (draw ? 1 : 2) : (draw ? 3 : 10)) == 0)
				{
					foreach (SourceBacker.Row row in list)
					{
						if (row.chara == chara.id)
						{
							chara.ApplyBacker(row.id);
							list.Remove(row);
							break;
						}
					}
				}
			}
		}
	}

	[CompilerGenerated]
	internal static Thing <TryAddThingInSharedContainer>g__SearchDest|256_0(ref Zone.<>c__DisplayClass256_0 A_0)
	{
		foreach (Thing thing in A_0.containers)
		{
			if (thing.trait is TraitShippingChest)
			{
				thing = EClass.game.cards.container_shipping;
			}
			if (!A_0.sharedOnly || thing.IsSharedContainer)
			{
				if (A_0.add)
				{
					Thing thing2 = thing.things.TryStack(A_0.t.Thing, -1, -1);
					if (thing2 != A_0.t)
					{
						if (A_0.msg)
						{
							A_0.chara.Say("putSharedItem", A_0.chara, thing, thing2.GetName(NameStyle.Full, -1), null);
						}
						return thing2;
					}
				}
				else if (thing.things.CanStack(A_0.t, -1, -1) != A_0.t)
				{
					return thing;
				}
				if (thing.things.Count < thing.things.MaxCapacity)
				{
					Window.SaveData windowSaveData = thing.GetWindowSaveData();
					if (windowSaveData != null)
					{
						if (windowSaveData.priority <= A_0.priority || (windowSaveData.noRotten && A_0.t.IsDecayed) || (windowSaveData.onlyRottable && A_0.t.trait.Decay == 0) || (windowSaveData.userFilter && !windowSaveData.IsFilterPass(A_0.t.GetName(NameStyle.Full, 1))))
						{
							continue;
						}
						if (windowSaveData.advDistribution)
						{
							bool flag = false;
							foreach (int num in windowSaveData.cats)
							{
								if (A_0.t.category.uid == num)
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								continue;
							}
						}
						else if (windowSaveData.flag.HasFlag(A_0.flag))
						{
							continue;
						}
						A_0.priority = windowSaveData.priority;
					}
					else
					{
						if (A_0.priority != -1)
						{
							continue;
						}
						A_0.priority = 0;
					}
					A_0.dest = thing;
				}
			}
		}
		return null;
	}

	[CompilerGenerated]
	internal static Point <SpawnLostItems>g__GetPos|312_0()
	{
		for (int i = 0; i < 10; i++)
		{
			Point randomPoint = EClass._zone.bounds.GetRandomPoint();
			if (!randomPoint.IsBlocked && !randomPoint.HasThing && !randomPoint.HasObj && !randomPoint.HasBlock)
			{
				return randomPoint;
			}
		}
		return null;
	}

	public static bool forceRegenerate;

	public static string forceSubset;

	public static CardRow sourceHat;

	public static int okaerinko = 0;

	[JsonProperty]
	public FactionBranch branch;

	[JsonProperty]
	public HashSet<int> completedQuests = new HashSet<int>();

	[JsonProperty]
	public ZoneEventManager events = new ZoneEventManager();

	[JsonProperty]
	public ElementContainerZone elements = new ElementContainerZone();

	[JsonProperty]
	public MapBounds bounds;

	[JsonProperty]
	public Dictionary<int, string> dictCitizen = new Dictionary<int, string>();

	[JsonProperty]
	public ZoneInstance instance;

	[JsonProperty]
	public List<int> landFeats;

	public Map map;

	public MapSubset subset;

	public ZoneBlueprint bp;

	public int fileVariation;

	public bool dirtyElectricity;

	public bool isStarted;

	public bool isSimulating;

	public int tempDist;

	public BiomeProfile _biome;

	public static bool ignoreSpawnAnime;

	public static List<Thing> Suckers = new List<Thing>();

	public class PortalReturnData
	{
		[JsonProperty]
		public int uidZone;

		[JsonProperty]
		public int x;

		[JsonProperty]
		public int z;
	}
}
