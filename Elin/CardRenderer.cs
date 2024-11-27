using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CardRenderer : RenderObject
{
	public Vector3 PositionCenter()
	{
		return new Vector3(this.position.x + this.data.size.x + this.data.offset.x * 0.5f, this.position.y + this.data.size.y + this.data.offset.y, this.position.z);
	}

	public virtual bool IsMoving
	{
		get
		{
			return false;
		}
	}

	public virtual void SetOwner(Card c)
	{
		this.owner = c;
		if (this.data == null)
		{
			this.data = this.owner.sourceCard.renderData;
		}
		this.isChara = c.isChara;
		this.usePass = (this.data.pass != null);
	}

	public override void Draw(RenderParam p)
	{
		Vector3 newVector = p.NewVector3;
		this.Draw(p, ref newVector, true);
	}

	public override void RenderToRenderCam(RenderParam p)
	{
		Vector3 renderPos = EClass.scene.camSupport.renderPos;
		if (this.data.multiSize)
		{
			renderPos.y -= 0.8f;
		}
		p.x = renderPos.x;
		p.y = renderPos.y;
		p.z = renderPos.z;
		this.Draw(p, ref renderPos, false);
	}

	public override void Draw(RenderParam p, ref Vector3 v, bool drawShadow)
	{
		if (this.skip)
		{
			this.skip = false;
			return;
		}
		this.sync = RenderObject.syncFrame;
		RenderObject.currentParam = p;
		p.dir = this.owner.dir;
		if (!this.isSynced)
		{
			this.OnEnterScreen();
			RenderObject.syncList.Add(this);
		}
		if (this.orbit)
		{
			this.orbit.Refresh();
		}
		else
		{
			this.TrySpawnOrbit();
		}
		if (this.isChara && this.owner.parent == EClass.game.activeZone)
		{
			if (this.owner.Chara.bossText && !EClass.ui.IsActive && !SplashText.Instance)
			{
				SplashText splashText = Util.Instantiate<SplashText>("Media/Text/SplashText_boss2", EClass.ui.rectDynamic);
				string text = this.owner.Chara.Aka.ToTitleCase(true);
				string text2 = this.owner.Chara.NameSimple.ToTitleCase(false);
				if (!Lang.isBuiltin && Lang.langCode != "CN")
				{
					text = this.owner.Chara.source.aka.ToTitleCase(true);
					text2 = this.owner.Chara.source.name.ToTitleCase(false);
				}
				splashText.textSmall.text = text;
				splashText.textBig.text = text2;
				this.owner.Chara.bossText = false;
			}
			if (this.owner.Chara.host == null)
			{
				this.UpdatePosition(ref v, p);
			}
			AI_Trolley ai_Trolley = this.owner.Chara.ai as AI_Trolley;
			if (ai_Trolley != null && ai_Trolley.IsRunning)
			{
				drawShadow = false;
				if (ai_Trolley.trolley.HideChara)
				{
					if (this.hasActor)
					{
						this.actor.SetActive(false);
					}
					return;
				}
			}
			if (this.hasActor)
			{
				this.actor.SetActive(true);
			}
		}
		else
		{
			p.x = (this.position.x = v.x);
			p.y = (this.position.y = v.y);
			p.z = (this.position.z = v.z);
		}
		if (this.anime != null)
		{
			this.anime.Update();
		}
		if (!this.isChara && !this.owner.IsInstalled && this.owner.category.tileDummy != 0 && !this.owner.isRoofItem && this.owner.ExistsOnMap && this.owner.trait.UseDummyTile)
		{
			SubPassData.Current = SubPassData.Default;
			RenderDataObjDummy rendererObjDummy = EClass.scene.screenElin.tileMap.rendererObjDummy;
			p.tile = (float)rendererObjDummy.tile;
			p.dir = 0;
			if (this.hasActor)
			{
				this.actor.SetActive(false);
			}
			rendererObjDummy.Draw(p);
			if (drawShadow && !this.owner.pos.cell.ignoreObjShadow)
			{
				EClass.scene.screenElin.tileMap.passShadow.AddShadow(this.position.x + rendererObjDummy.offsetShadow.x, this.position.y + rendererObjDummy.offsetShadow.y + p.shadowFix, this.position.z + rendererObjDummy.offsetShadow.z, ShadowData.Instance.items[rendererObjDummy.shadowPref.shadow], rendererObjDummy.shadowPref, 0, p.snow);
			}
		}
		else
		{
			SubPassData.Current = this.owner.GetSubPassData();
			SourcePref pref = this.owner.Pref;
			RenderData renderData = this.data;
			int prefIndex = this.owner.PrefIndex;
			if (Player.seedHallucination != 0 && this.<Draw>g__CanBeHallucinated|18_0())
			{
				Rand.SetSeed(Player.seedHallucination + this.owner.uid);
				CardRow cardRow = null;
				bool flag = false;
				int i = 0;
				while (i < 20)
				{
					if (this.owner.isChara)
					{
						cardRow = EClass.sources.charas.rows.RandomItem<SourceChara.Row>();
						if (!cardRow.multisize)
						{
							goto IL_487;
						}
					}
					else
					{
						cardRow = EClass.sources.things.rows.RandomItem<SourceThing.Row>();
						if (cardRow.tileType == this.owner.TileType)
						{
							goto IL_487;
						}
					}
					IL_4E5:
					i++;
					continue;
					IL_487:
					if (cardRow.chance != 0 && cardRow._tiles.Length != 0 && cardRow.idActor.IsEmpty() && cardRow.idExtra.IsEmpty() && SubPassData.Current == SubPassData.Default && !(cardRow._idRenderData != this.owner.sourceCard._idRenderData))
					{
						flag = true;
						break;
					}
					goto IL_4E5;
				}
				if (flag)
				{
					renderData = cardRow.renderData;
					pref = cardRow.pref;
					cardRow.model.dir = this.owner.dir;
					cardRow.model.SetRenderParam(p);
					prefIndex = cardRow.model.PrefIndex;
				}
				Rand.SetSeed(-1);
			}
			if (this.owner.trait is TraitFigure)
			{
				if (!this.owner.c_idRefCard.IsEmpty() && (this.owner.IsInstalled || EClass.pc.held == this.owner || !this.owner.ExistsOnMap || this.owner.isRoofItem))
				{
					TraitFigure traitFigure = this.owner.trait as TraitFigure;
					SourceChara.Row row = EClass.sources.charas.map.TryGetValue(this.owner.c_idRefCard, null) ?? EClass.sources.charas.map["putty"];
					renderData = row.renderData;
					pref = row.pref;
					if (row._tiles.Length == 0 || this.data.pass == null)
					{
						renderData = this.owner.sourceCard.renderData;
						pref = this.owner.sourceCard.pref;
					}
					else
					{
						p.tile = (float)(row._tiles[this.owner.refVal % row._tiles.Length] * ((this.owner.dir % 2 == 0) ? 1 : -1));
						p.matColor = (float)traitFigure.GetMatColor();
						drawShadow = traitFigure.ShowShadow;
						pref = row.pref;
					}
				}
				else
				{
					renderData = this.owner.sourceCard.renderData;
				}
			}
			if (this.replacer != null)
			{
				renderData = this.replacer.data;
				pref = this.replacer.pref;
				SubPassData.Current = SubPassData.Default;
			}
			int shadow = pref.shadow;
			bool flag2 = this.isChara && this.owner.isHidden && !EClass.pc.canSeeInvisible && (!EClass.pc.hasTelepathy || !this.owner.Chara.race.visibleWithTelepathy);
			p.y += pref.y;
			if (drawShadow && shadow != 1 && SubPassData.Current.shadow && (!flag2 || this.owner.IsPC))
			{
				bool flag3 = this.isChara ? (this.owner.dir == 1 || this.owner.dir == 2) : (this.owner.dir % 2 == 1);
				EClass.scene.screenElin.tileMap.passShadow.AddShadow(this.position.x + (flag3 ? -1f : 1f) * renderData.offsetShadow.x, this.position.y + renderData.offsetShadow.y + (this.owner.TileType.UseMountHeight ? 0f : p.shadowFix) - RenderObject.altitudeFix * (float)this.owner.altitude, this.position.z + renderData.offsetShadow.z, ShadowData.Instance.items[shadow], pref, prefIndex, p.snow);
			}
			if (this.usePass)
			{
				if (this.owner.noSnow)
				{
					p.snow = false;
				}
				if (!flag2)
				{
					renderData.Draw(p);
				}
			}
			else if (this.hasActor)
			{
				if (this.owner.isChara && this.owner.Chara.ride != null && (EClass.core.config.game.showRide == 2 || (EClass.core.config.game.showRide == 1 && !this.owner.Cell.HasRoof && !EClass._map.IsIndoor)) && !this.owner.IsDeadOrSleeping)
				{
					Chara ride = this.owner.Chara.ride;
					CharaActorPCC charaActorPCC = ride.renderer.actor as CharaActorPCC;
					CharaActorPCC charaActorPCC2 = this.actor as CharaActorPCC;
					ride.angle = this.owner.angle;
					if (charaActorPCC != null && charaActorPCC2 != null)
					{
						charaActorPCC.provider.currentDir = charaActorPCC2.provider.currentDir;
						charaActorPCC.provider.currentFrame = charaActorPCC2.provider.currentFrame;
						charaActorPCC.provider.SetSpriteMain();
						charaActorPCC.RefreshSprite();
					}
					PCCData.RideData ride2 = (ride.renderer as CharaRenderer).pccData.ride;
					float x = p.x;
					float y = p.y;
					float z = p.z;
					Vector3 vector = new Vector3(v.x, v.y, v.z);
					ride.renderer.Draw(p, ref vector, false);
					int currentDir = this.actor.currentDir;
					p.x = x + RenderObject.renderSetting.ridePos[currentDir].x + ride2.x * (float)((currentDir == 1) ? 1 : ((currentDir == 2) ? -1 : 0));
					p.y = y + RenderObject.renderSetting.ridePos[currentDir].y + ride2.y + ride2.jump * (float)((this.actor.GetFrame() % 2 == 1) ? 1 : 0);
					p.z = z + RenderObject.renderSetting.ridePos[currentDir].z - ride2.z;
				}
				if (flag2)
				{
					this.actor.SetActive(false);
				}
				else
				{
					this.actor.SetActive(true);
					this.actor.OnRender(p);
				}
			}
			if (this.isChara)
			{
				if (this.owner.Chara.parasite != null)
				{
					this.owner.Chara.parasite.renderer.position = this.position;
				}
				if (this.owner.Chara.ride != null)
				{
					this.owner.Chara.ride.renderer.position = this.position;
				}
			}
			SubPassData.Current = SubPassData.Default;
		}
		if (this.listTC.Count > 0)
		{
			RenderObject.tempV = this.position;
			RenderObject.tempV.y = RenderObject.tempV.y + (this.data.offset.y + this.data.size.y);
			for (int j = this.listTC.Count - 1; j >= 0; j--)
			{
				this.listTC[j].OnDraw(ref RenderObject.tempV);
			}
		}
		if (this.owner.trait.RenderExtra)
		{
			this.owner.trait.OnRenderExtra(p);
		}
	}

	public virtual void UpdatePosition(ref Vector3 destPos, RenderParam p)
	{
	}

	public virtual void DrawHeld()
	{
	}

	public void RefreshSprite()
	{
		if (this.hasActor)
		{
			this.actor.RefreshSprite();
		}
	}

	public override void OnEnterScreen()
	{
		if (this.isSynced)
		{
			string str = "renderer alraedy synced:";
			Card card = this.owner;
			Debug.LogError(str + ((card != null) ? card.ToString() : null));
		}
		this.isSynced = true;
		if (!this.usePass)
		{
			if (this.hasActor)
			{
				if (!this.data.persistActor)
				{
					string str2 = "renderer alraedy have actor:";
					Card card2 = this.owner;
					Debug.LogError(str2 + ((card2 != null) ? card2.ToString() : null));
				}
			}
			else
			{
				this.actor = this.data.CreateActor();
				if (this.actor == null)
				{
					if (this.owner.sourceCard.idActor.IsEmpty())
					{
						this.actor = PoolManager.Spawn<CardActor>(Resources.Load<CardActor>("Scene/Render/Actor/" + (this.owner.isChara ? "CharaActor" : "ThingActor")), null);
					}
					else
					{
						this.actor = PoolManager.Spawn<CardActor>(Resources.Load<CardActor>("Scene/Render/Actor/" + this.owner.sourceCard.idActor[0]), null);
					}
				}
				this.hasActor = true;
			}
			this.actor.SetOwner(this.owner);
		}
		this.RefreshStateIcon();
		this.RefreshExtra();
		if (this.owner.isCensored)
		{
			this.SetCensored(true);
		}
		this.owner.trait.OnEnterScreen();
	}

	public void AddExtra(string id)
	{
		if (this.GetTC(id) == null)
		{
			TC tc = this.AddTC<TC>(PoolManager.Spawn<TC>("tcExtra_" + id, "Scene/Render/Actor/Component/Extra/" + id, null));
			if (tc.isUI)
			{
				tc.transform.SetParent(EClass.ui.rectDynamic, false);
			}
			tc.name = id;
		}
	}

	public void RefreshExtra()
	{
		string idExtra = this.owner.sourceCard.idExtra;
		if (!idExtra.IsEmpty())
		{
			this.AddExtra(idExtra);
		}
		if (this.isChara && this.owner.rarity >= Rarity.Legendary && this.owner.rarity != Rarity.Artifact && !this.owner.Chara.IsHomeMember())
		{
			if (this.owner.c_bossType == BossType.Evolved)
			{
				this.AddExtra("c_unique_evolved");
				return;
			}
			this.AddExtra("c_unique");
		}
	}

	public void RemoveExtra(string id)
	{
		TC tc = this.GetTC(id);
		if (tc != null)
		{
			this.RemoveTC(tc);
		}
	}

	public override void OnLeaveScreen()
	{
		if (!this.isSynced)
		{
			string str = "renderer alraedy not synced:";
			Card card = this.owner;
			Debug.LogWarning(str + ((card != null) ? card.ToString() : null));
		}
		this.isSynced = false;
		if (this.hasActor && (!this.owner.ExistsOnMap || !this.data.persistActor))
		{
			this.KillActor();
		}
		for (int i = this.listTC.Count - 1; i >= 0; i--)
		{
			this.RemoveTC(i);
		}
		if (this.hasText)
		{
			this.DespawnSimpleText();
		}
		if (this.orbit)
		{
			PoolManager.Despawn(this.orbit);
			this.orbit = null;
		}
	}

	public void KillActor()
	{
		this.actor.Kill();
		this.hasActor = false;
		this.actor = null;
	}

	public void PlayAnime(AnimeID id, bool force)
	{
		this.PlayAnime(id, default(Vector3), force);
	}

	public void PlayAnime(AnimeID id, Card dest)
	{
		this.PlayAnime(id, (dest.renderer.position - this.position).normalized, false);
	}

	public unsafe void PlayAnime(AnimeID id, Point dest)
	{
		this.PlayAnime(id, (*dest.PositionAuto() - this.position).normalized, false);
	}

	public void PlayAnime(AnimeID id, Vector3 dest = default(Vector3), bool force = false)
	{
		if (!force && this.owner.parent is Zone && (!this.owner.pos.IsValid || !this.owner.pos.IsSync))
		{
			return;
		}
		TransAnimeData transAnimeData = ResourceCache.Load<TransAnimeData>("Scene/Render/Anime/" + id.ToString());
		if (transAnimeData == null)
		{
			this.anime = null;
			return;
		}
		this.anime = new TransAnime
		{
			data = transAnimeData,
			renderer = this
		}.Init();
		if (id == AnimeID.Attack || id - AnimeID.Attack_Place <= 1)
		{
			this.anime.dest = dest;
		}
	}

	public virtual void KillAnime()
	{
		this.anime = null;
	}

	public virtual void IdleFrame()
	{
		if (this.hasActor)
		{
			this.actor.IdleFrame();
		}
	}

	public virtual void NextFrame()
	{
		if (this.hasActor)
		{
			this.actor.NextFrame();
		}
	}

	public virtual void NextDir()
	{
		if (this.hasActor)
		{
			this.actor.NextDir();
		}
	}

	public T AddTC<T>(TC tc) where T : TC
	{
		this.listTC.Add(tc);
		tc.SetOwner(this);
		return tc as T;
	}

	public T GetTC<T>() where T : TC
	{
		if (this.listTC.Count == 0)
		{
			return default(T);
		}
		for (int i = this.listTC.Count - 1; i >= 0; i--)
		{
			if (this.listTC[i] is T)
			{
				return this.listTC[i] as T;
			}
		}
		return default(T);
	}

	public TC GetTC(string id)
	{
		if (this.listTC.Count == 0)
		{
			return null;
		}
		for (int i = this.listTC.Count - 1; i >= 0; i--)
		{
			if (this.listTC[i].name == id)
			{
				return this.listTC[i];
			}
		}
		return null;
	}

	public T GetOrCreateTC<T>(Func<T> func) where T : TC
	{
		T result;
		if ((result = this.GetTC<T>()) == null)
		{
			result = this.AddTC<T>(func());
		}
		return result;
	}

	public void RemoveTC<T>() where T : TC
	{
		if (this.listTC.Count == 0)
		{
			return;
		}
		for (int i = this.listTC.Count - 1; i >= 0; i--)
		{
			if (this.listTC[i] is T)
			{
				this.listTC[i].Kill();
				this.listTC.RemoveAt(i);
				return;
			}
		}
	}

	public void RemoveTC(TC tc)
	{
		this.RemoveTC(this.listTC.IndexOf(tc));
	}

	public void RemoveTC(int index)
	{
		if (index == -1)
		{
			return;
		}
		this.listTC[index].Kill();
		this.listTC.RemoveAt(index);
	}

	public void TrySpawnOrbit()
	{
		if (this.owner.isChara)
		{
			string text = this.owner.IsPC ? "tcOrbitPC" : "tcOrbitChara";
			this.orbit = PoolManager.Spawn<TCOrbitChara>(text, "Scene/Render/Actor/Component/" + text, null);
			this.orbit.SetOwner(this);
			return;
		}
		if (this.owner.trait.ShowOrbit)
		{
			string text2 = "tcOrbitThing";
			this.orbit = PoolManager.Spawn<TCOrbitThing>(text2, "Scene/Render/Actor/Component/" + text2, null);
			this.orbit.SetOwner(this);
		}
	}

	public bool IsTalking()
	{
		TCText tc = this.GetTC<TCText>();
		return tc && tc.pop.items.Count > 0;
	}

	public TCSimpleText SpawnSimpleText()
	{
		if (!this.hasText)
		{
			this.hasText = true;
			this.simpleText = PoolManager.Spawn<TCSimpleText>(EClass.core.refs.tcs.simpleText, EClass.screen);
		}
		RenderObject.tempV.x = this.position.x + TC._setting.simpleTextPos.x;
		RenderObject.tempV.y = this.position.y + TC._setting.simpleTextPos.y;
		RenderObject.tempV.z = this.position.z + TC._setting.simpleTextPos.z;
		this.simpleText.transform.position = RenderObject.tempV;
		return this.simpleText;
	}

	public void DespawnSimpleText()
	{
		if (!this.hasText)
		{
			return;
		}
		PoolManager.Despawn(this.simpleText);
		this.hasText = false;
	}

	public void SetCensored(bool enable)
	{
		if (!this.isSynced)
		{
			return;
		}
		if (enable)
		{
			this.GetOrCreateTC<TCCensored>(() => PoolManager.Spawn<TCCensored>(EClass.core.refs.tcs.censored, null));
			return;
		}
		this.RemoveTC<TCCensored>();
	}

	public virtual void RefreshStateIcon()
	{
	}

	public void Say(string text, Color c = default(Color), float duration = 0f)
	{
		(this.GetTC<TCText>() ?? this.AddTC<TCText>(PoolManager.Spawn<TCText>(EClass.core.refs.tcs.text, EClass.ui.rectDynamic))).Say(text, duration);
	}

	public void ShowEmo(Emo emo, float duration = 0f)
	{
		if (!this.isSynced)
		{
			return;
		}
		(this.GetTC<TCText>() ?? this.AddTC<TCText>(PoolManager.Spawn<TCText>(EClass.core.refs.tcs.text, EClass.ui.rectDynamic))).ShowEmo(emo, duration);
	}

	public virtual void SetFirst(bool first, Vector3 pos)
	{
	}

	public virtual void SetFirst(bool first)
	{
	}

	public virtual void Refresh()
	{
	}

	[CompilerGenerated]
	private bool <Draw>g__CanBeHallucinated|18_0()
	{
		if (!this.usePass || !this.owner.ExistsOnMap || !this.owner.trait.CanBeHallucinated)
		{
			return false;
		}
		if (this.owner.isThing)
		{
			if (this.owner.altitude != 0 || this.owner.TileType.UseMountHeight || this.owner.pos.LastThing != this.owner)
			{
				return false;
			}
			if (this.listTC.Count > 0)
			{
				return false;
			}
		}
		return true;
	}

	public TCOrbit orbit;

	public Card owner;

	public CardActor actor;

	public bool hasActor;

	public bool hasText;

	public bool isChara;

	public bool skip;

	public Vector3 position;

	public TransAnime anime;

	public List<TC> listTC = new List<TC>();

	public TCSimpleText simpleText;

	public RendererReplacer replacer;
}
