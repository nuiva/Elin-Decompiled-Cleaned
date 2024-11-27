using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Trait : EClass
{
	public string[] Params
	{
		get
		{
			if (!this.owner.c_editorTraitVal.IsEmpty())
			{
				return ("," + this.owner.c_editorTraitVal).Split(',', StringSplitOptions.None);
			}
			return this.owner.sourceCard.trait;
		}
	}

	public string GetParam(int i, string def = null)
	{
		if (i < this.Params.Length)
		{
			return this.Params[i];
		}
		return def;
	}

	public int GetParamInt(int i, int def)
	{
		if (i < this.Params.Length)
		{
			return this.Params[i].ToInt();
		}
		return def;
	}

	public virtual byte WeightMod
	{
		get
		{
			return 0;
		}
	}

	public virtual string Name
	{
		get
		{
			return this.owner.NameSimple;
		}
	}

	public virtual bool Contains(RecipeSource r)
	{
		return r.idFactory == ((this.owner.sourceCard.origin != null) ? this.owner.sourceCard.origin.id : this.owner.id);
	}

	public virtual TileType tileType
	{
		get
		{
			return this.owner.TileType;
		}
	}

	public virtual RefCardName RefCardName
	{
		get
		{
			return RefCardName.Default;
		}
	}

	public virtual bool IsBlockPath
	{
		get
		{
			return this.tileType.IsBlockPass;
		}
	}

	public virtual bool IsBlockSight
	{
		get
		{
			return this.tileType.IsBlockSight;
		}
	}

	public virtual bool IsDoor
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsOpenSight
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsOpenPath
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsFloating
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsGround
	{
		get
		{
			return false;
		}
	}

	public virtual bool InvertHeldSprite
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsChangeFloorHeight
	{
		get
		{
			return this.owner.Pref.Surface;
		}
	}

	public virtual bool ShouldRefreshTile
	{
		get
		{
			return this.IsBlockPath || this.IsOpenSight || this.IsBlockSight;
		}
	}

	public virtual bool ShouldTryRefreshRoom
	{
		get
		{
			return this.IsDoor;
		}
	}

	public virtual bool CanHarvest
	{
		get
		{
			return true;
		}
	}

	public virtual int radius
	{
		get
		{
			return 0;
		}
	}

	public virtual TraitRadiusType radiusType
	{
		get
		{
			return TraitRadiusType.Default;
		}
	}

	public virtual bool CanUseRoomRadius
	{
		get
		{
			return true;
		}
	}

	public virtual int GuidePriotiy
	{
		get
		{
			return 0;
		}
	}

	public virtual int Electricity
	{
		get
		{
			if (!this.owner.isThing)
			{
				return 0;
			}
			int electricity = this.owner.Thing.source.electricity;
			if (electricity > 0 || EClass._zone == null || EClass._zone.branch == null)
			{
				return electricity;
			}
			return electricity * 100 / (100 + EClass._zone.branch.Evalue(2700) / 2);
		}
	}

	public virtual bool IgnoreLastStackHeight
	{
		get
		{
			return false;
		}
	}

	public virtual int Decay
	{
		get
		{
			return this.owner.material.decay;
		}
	}

	public virtual int DecaySpeed
	{
		get
		{
			return 100;
		}
	}

	public virtual int DecaySpeedChild
	{
		get
		{
			return 100;
		}
	}

	public virtual bool IsFridge
	{
		get
		{
			return false;
		}
	}

	public virtual int GetValue()
	{
		return this.owner.sourceCard.value;
	}

	public virtual int DefaultStock
	{
		get
		{
			return 1;
		}
	}

	public virtual bool HoldAsDefaultInteraction
	{
		get
		{
			return false;
		}
	}

	public virtual int CraftNum
	{
		get
		{
			return 1;
		}
	}

	public virtual bool ShowOrbit
	{
		get
		{
			return false;
		}
	}

	public virtual bool HaveUpdate
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsSpot
	{
		get
		{
			return this.radius > 0;
		}
	}

	public virtual bool IsFactory
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanAutofire
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanName
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanPutAway
	{
		get
		{
			return true;
		}
	}

	public virtual bool CanChangeHeight
	{
		get
		{
			return true;
		}
	}

	public virtual bool CanStack
	{
		get
		{
			return this.owner.category.maxStack > 1;
		}
	}

	public virtual bool CanCopyInBlueprint
	{
		get
		{
			return this.owner.rarity <= Rarity.Superior && this.owner.sourceCard.value > 0 && this.CanBeDestroyed;
		}
	}

	public virtual bool CanStackTo(Thing to)
	{
		return this.CanStack;
	}

	public virtual bool CanBeAttacked
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanExtendBuild
	{
		get
		{
			return false;
		}
	}

	public virtual string langNote
	{
		get
		{
			return "";
		}
	}

	public virtual string IDInvStyle
	{
		get
		{
			return "default";
		}
	}

	public virtual string IDActorEx
	{
		get
		{
			return this.owner.Thing.source.idActorEx;
		}
	}

	public virtual string GetHoverText()
	{
		return null;
	}

	public virtual bool MaskOnBuild
	{
		get
		{
			return false;
		}
	}

	public virtual bool ShowContextOnPick
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsThrowMainAction
	{
		get
		{
			return this.owner.HasTag(CTAG.throwWeapon) || this.owner.IsMeleeWeapon;
		}
	}

	public virtual bool LevelAsQuality
	{
		get
		{
			return false;
		}
	}

	public virtual bool UseDummyTile
	{
		get
		{
			return true;
		}
	}

	public virtual bool RequireFullStackCheck
	{
		get
		{
			return false;
		}
	}

	public virtual bool DisableAutoCombat
	{
		get
		{
			return false;
		}
	}

	public virtual Action GetHealAction(Chara c)
	{
		return null;
	}

	public virtual InvGridSize InvGridSize
	{
		get
		{
			if (!this.owner.IsPC)
			{
				return InvGridSize.Default;
			}
			return InvGridSize.Backpack;
		}
	}

	public virtual bool IsContainer
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanOpenContainer
	{
		get
		{
			return this.IsContainer && !this.owner.isNPCProperty;
		}
	}

	public virtual bool IsSpecialContainer
	{
		get
		{
			return false;
		}
	}

	public virtual ContainerType ContainerType
	{
		get
		{
			return ContainerType.Default;
		}
	}

	public virtual ThrowType ThrowType
	{
		get
		{
			return ThrowType.Default;
		}
	}

	public virtual EffectDead EffectDead
	{
		get
		{
			return EffectDead.Default;
		}
	}

	public virtual bool IsHomeItem
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsAltar
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsRestSpot
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanBeMasked
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsBlendBase
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanBlend(Thing t)
	{
		return false;
	}

	public virtual void OnBlend(Thing t, Chara c)
	{
	}

	public virtual bool CanBeOnlyBuiltInHome
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanBuildInTown
	{
		get
		{
			return !this.owner.TileType.IsBlockPass && !this.owner.TileType.IsBlockSight;
		}
	}

	public virtual bool CanBeHeld
	{
		get
		{
			return this.ReqHarvest == null;
		}
	}

	public virtual bool CanBeStolen
	{
		get
		{
			return !this.CanOnlyCarry && this.CanBeHeld;
		}
	}

	public virtual bool CanOnlyCarry
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanBeDestroyed
	{
		get
		{
			return true;
		}
	}

	public virtual bool CanBeHallucinated
	{
		get
		{
			return true;
		}
	}

	public virtual bool CanSearchContents
	{
		get
		{
			return true;
		}
	}

	public virtual bool CanBeDropped
	{
		get
		{
			return true;
		}
	}

	public virtual string ReqHarvest
	{
		get
		{
			return null;
		}
	}

	public virtual bool CanBeDisassembled
	{
		get
		{
			return this.CanBeDestroyed && !(this is TraitTrap) && this.owner.things.Count == 0 && this.owner.rarity < Rarity.Artifact;
		}
	}

	public virtual bool CanBeShipped
	{
		get
		{
			return !this.owner.IsImportant && !this.owner.IsUnique;
		}
	}

	public virtual bool HasCharges
	{
		get
		{
			return false;
		}
	}

	public virtual bool ShowCharges
	{
		get
		{
			return this.HasCharges;
		}
	}

	public virtual bool ShowChildrenNumber
	{
		get
		{
			return this.IsContainer;
		}
	}

	public virtual int GetActDuration(Chara c)
	{
		return 0;
	}

	public virtual bool ShowAsTool
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanBeHeldAsFurniture
	{
		get
		{
			return this is TraitTool || this.IsThrowMainAction;
		}
	}

	public virtual bool HideInAdv
	{
		get
		{
			return false;
		}
	}

	public virtual bool NoHeldDir
	{
		get
		{
			return false;
		}
	}

	public virtual bool AlwaysHideOnLowWall
	{
		get
		{
			return false;
		}
	}

	public virtual SourceElement.Row GetRefElement()
	{
		return null;
	}

	public virtual Sprite GetRefSprite()
	{
		return null;
	}

	public bool ExistsOnMap
	{
		get
		{
			return this.owner.ExistsOnMap;
		}
	}

	public virtual bool RenderExtra
	{
		get
		{
			return false;
		}
	}

	public virtual float DropChance
	{
		get
		{
			return 0f;
		}
	}

	public virtual string IdNoRestock
	{
		get
		{
			return this.owner.id;
		}
	}

	public virtual void OnRenderExtra(RenderParam p)
	{
	}

	public virtual Emo2 GetHeldEmo(Chara c)
	{
		return Emo2.none;
	}

	public virtual void SetOwner(Card _owner)
	{
		this.owner = _owner;
		this.OnSetOwner();
	}

	public virtual bool IdleUse(Chara c, int dist)
	{
		return false;
	}

	public virtual int IdleUseChance
	{
		get
		{
			return 3;
		}
	}

	public virtual void OnSetOwner()
	{
	}

	public virtual void OnImportMap()
	{
	}

	public virtual void SetParams(params string[] s)
	{
	}

	public virtual void OnCrafted(Recipe recipe)
	{
	}

	public virtual void OnCreate(int lv)
	{
	}

	public virtual void OnChangePlaceState(PlaceState state)
	{
	}

	public virtual void OnAddedToZone()
	{
	}

	public virtual void OnRemovedFromZone()
	{
	}

	public virtual void OnSimulateHour(VirtualDate date)
	{
	}

	public virtual string GetName()
	{
		return this.owner.sourceCard.GetText("name", false);
	}

	public virtual void SetName(ref string s)
	{
	}

	public unsafe virtual void OnRenderTile(Point point, HitResult result, int dir)
	{
		if (this.radius == 0)
		{
			return;
		}
		Vector3 vector = *point.Position();
		vector.z += EClass.setting.render.thingZ;
		foreach (Point point2 in this.ListPoints(point, true))
		{
			Vector3 vector2 = *point2.Position();
			EClass.screen.guide.passGuideFloor.Add(vector2.x, vector2.y, vector2.z, 10f, 0.3f);
		}
	}

	public virtual int CompareTo(Card b)
	{
		return 0;
	}

	public virtual bool CanBuiltAt(Point p)
	{
		return true;
	}

	public virtual void Update()
	{
	}

	public Point GetPoint()
	{
		return this.owner.pos;
	}

	public Point GetRandomPoint(Func<Point, bool> func = null, Chara accessChara = null)
	{
		if (this.radius == 0)
		{
			return this.owner.pos;
		}
		List<Point> list = this.ListPoints(null, true);
		for (int i = 0; i < 50; i++)
		{
			Point point = list.RandomItem<Point>();
			if (point.IsValid && (func == null || func(point)) && (accessChara == null || accessChara.HasAccess(point)))
			{
				return point;
			}
		}
		return list[0];
	}

	public virtual List<Point> ListPoints(Point center = null, bool onlyPassable = true)
	{
		Trait.listRadiusPoints.Clear();
		if (center == null)
		{
			center = this.owner.pos;
		}
		if (this.radius == 0)
		{
			Trait.listRadiusPoints.Add(center.Copy());
			return Trait.listRadiusPoints;
		}
		Room room = center.cell.room;
		if (room != null && this.CanUseRoomRadius)
		{
			using (List<Point>.Enumerator enumerator = room.points.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Point point = enumerator.Current;
					if (this.radiusType == TraitRadiusType.Farm)
					{
						Trait.listRadiusPoints.Add(point.Copy());
					}
					else if ((!onlyPassable || !point.cell.blocked) && !point.cell.HasBlock && point.cell.HasFloor)
					{
						Trait.listRadiusPoints.Add(point.Copy());
					}
				}
				goto IL_141;
			}
		}
		EClass._map.ForeachSphere(center.x, center.z, (float)(this.radius + 1), delegate(Point p)
		{
			if (this.radiusType == TraitRadiusType.Farm)
			{
				if (p.cell.HasBlock && !p.cell.HasFence)
				{
					return;
				}
				Trait.listRadiusPoints.Add(p.Copy());
				return;
			}
			else
			{
				if (onlyPassable && p.cell.blocked)
				{
					return;
				}
				if (!p.cell.HasBlock && p.cell.HasFloor && (!onlyPassable || Los.IsVisible(center, p, null)))
				{
					Trait.listRadiusPoints.Add(p.Copy());
				}
				return;
			}
		});
		IL_141:
		if (Trait.listRadiusPoints.Count == 0)
		{
			Trait.listRadiusPoints.Add(center.Copy());
		}
		return Trait.listRadiusPoints;
	}

	public virtual Recipe GetRecipe()
	{
		return Recipe.Create(this.owner.Thing);
	}

	public virtual Recipe GetBuildModeRecipe()
	{
		return Recipe.Create(this.owner.Thing);
	}

	public virtual string RecipeCat
	{
		get
		{
			return this.owner.sourceCard.RecipeCat;
		}
	}

	public virtual bool CanCook(Card c)
	{
		return c != null && this.ExistsOnMap && c.trait is TraitFood;
	}

	public void CookProgress()
	{
		if (!this.ExistsOnMap)
		{
			return;
		}
		foreach (Card card in this.owner.pos.ListCards(false))
		{
			this.owner.PlaySound("cook", 1f, true);
			card.renderer.PlayAnime(AnimeID.Jump, default(Vector3), false);
		}
	}

	public virtual bool CanOffer(Card tg)
	{
		return tg != null && !tg.isChara && !tg.trait.CanOnlyCarry && tg.rarity != Rarity.Artifact;
	}

	public void OfferProcess(Chara cc)
	{
		if (!this.ExistsOnMap)
		{
			return;
		}
		SourceReligion.Row row = EClass.sources.religions.map.TryGetValue(this.owner.c_idDeity, EClass.sources.religions.map["eyth"]);
		string @ref = row.GetTextArray("name2")[1];
		string ref2 = row.GetTextArray("name2")[0];
		if (EClass.rnd(3) == 0)
		{
			cc.Talk("offer", @ref, ref2, false);
		}
		foreach (Card card in this.owner.pos.ListCards(false))
		{
			if (this.CanOffer(card))
			{
				card.renderer.PlayAnime(AnimeID.Shiver, default(Vector3), false);
			}
		}
	}

	public void Offer(Chara cc)
	{
		if (!this.ExistsOnMap)
		{
			return;
		}
		foreach (Card card in this.owner.pos.ListCards(false))
		{
			if (this.CanOffer(card))
			{
				card.Destroy();
				cc.depression.Mod(100);
				this.owner.PlaySound("offering", 1f, true);
			}
		}
	}

	public virtual bool TryProgress(AIProgress p)
	{
		return true;
	}

	public virtual LockOpenState TryOpenLock(Chara cc, bool msgFail = true)
	{
		Thing thing = cc.things.FindBest<TraitLockpick>((Thing t) => -t.c_charges);
		int num = (thing == null) ? (cc.Evalue(280) / 2 + 2) : (cc.Evalue(280) + 10);
		int num2 = this.owner.c_lockLv;
		bool flag = this is TraitChestPractice;
		if (flag)
		{
			num2 = num / 4 * 3 - 1;
		}
		if (num <= num2 && cc.IsPC)
		{
			cc.PlaySound("lock", 1f, true);
			cc.Say("openLockFail2", null, null);
			this.owner.PlayAnime(AnimeID.Shiver, false);
			return LockOpenState.NotEnoughSkill;
		}
		if (thing != null && !flag)
		{
			thing.ModCharge(-1, true);
		}
		if (EClass.rnd(num + 6) > num2 + 5 || (!cc.IsPC && EClass.rnd(20) == 0) || EClass.rnd(200) == 0)
		{
			cc.PlaySound("lock_open", 1f, true);
			cc.Say("lockpick_success", cc, this.owner, null, null);
			int num3 = 100 + num2 * 10;
			if (this.owner.c_lockedHard)
			{
				num3 *= 10;
			}
			cc.ModExp(280, num3);
			this.owner.c_lockLv = 0;
			if (this.owner.c_lockedHard)
			{
				this.owner.c_lockedHard = false;
				this.owner.c_priceAdd = 0;
			}
			if (cc.IsPC && this.owner.isLostProperty)
			{
				EClass.player.ModKarma(-8);
			}
			this.owner.isLostProperty = false;
			return LockOpenState.Success;
		}
		cc.PlaySound("lock", 1f, true);
		if (cc.IsPC)
		{
			cc.Say("openLockFail", null, null);
		}
		cc.ModExp(280, (thing != null) ? 50 : 30);
		if (thing == null | EClass.rnd(2) == 0)
		{
			cc.stamina.Mod(-1);
		}
		return LockOpenState.Fail;
	}

	public virtual void WriteNote(UINote n, bool identified)
	{
	}

	public int GetSortVal(UIList.SortMode m)
	{
		return this.owner.sourceCard._index;
	}

	public virtual HotItem GetHotItem()
	{
		return new HotItemHeld(this.owner as Thing);
	}

	public virtual bool IsTool
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanRead(Chara c)
	{
		return false;
	}

	public virtual void OnRead(Chara c)
	{
	}

	public virtual bool CanEat(Chara c)
	{
		return this.owner.HasElement(10, 1);
	}

	public virtual void OnEat(Chara c)
	{
	}

	public virtual bool CanDrink(Chara c)
	{
		return false;
	}

	public virtual void OnDrink(Chara c)
	{
	}

	public virtual void OnThrowGround(Chara c, Point p)
	{
	}

	public virtual string LangUse
	{
		get
		{
			return "actUse";
		}
	}

	public virtual bool CanUse(Chara c)
	{
		return false;
	}

	public virtual bool CanUse(Chara c, Card tg)
	{
		return false;
	}

	public virtual bool CanUse(Chara c, Point p)
	{
		return false;
	}

	public virtual bool OnUse(Chara c)
	{
		if (c.held != this.owner)
		{
			c.TryHoldCard(this.owner, -1, false);
		}
		return true;
	}

	public virtual bool OnUse(Chara c, Card tg)
	{
		return true;
	}

	public virtual bool OnUse(Chara c, Point p)
	{
		return true;
	}

	public virtual void TrySetAct(ActPlan p)
	{
		if (this.CanUse(this.owner.Chara))
		{
			p.TrySetAct(this.LangUse, () => this.OnUse(p.cc), this.owner, null, 1, false, true, false);
		}
	}

	public virtual void TrySetHeldAct(ActPlan p)
	{
	}

	public virtual void OnHeld()
	{
	}

	public virtual void OnTickHeld()
	{
	}

	public virtual void OnSetCurrentItem()
	{
	}

	public virtual void OnUnsetCurrentItem()
	{
	}

	public virtual bool OnChildDecay(Card c, bool firstDecay)
	{
		return true;
	}

	public virtual bool CanChildDecay(Card c)
	{
		return false;
	}

	public virtual void OnSetCardGrid(ButtonGrid b)
	{
	}

	public virtual void OnStepped(Chara c)
	{
	}

	public virtual void OnSteppedOut(Chara c)
	{
	}

	public virtual void OnOpenDoor(Chara c)
	{
	}

	public void Install(bool byPlayer)
	{
		if (this.Electricity != 0)
		{
			EClass._zone.dirtyElectricity = true;
			if (this.Electricity > 0)
			{
				EClass._zone.electricity += this.Electricity;
				EClass.pc.PlaySound("electricity_on", 1f, true);
			}
		}
		this.TryToggle();
		this.owner.RecalculateFOV();
		if (EClass._zone.isStarted && this.ToggleType == ToggleType.Fire && this.owner.isOn)
		{
			this.owner.PlaySound("fire", 1f, true);
		}
		this.OnInstall(byPlayer);
	}

	public void Uninstall()
	{
		if (this.Electricity != 0)
		{
			if (this.owner.isOn)
			{
				this.Toggle(false, true);
			}
			EClass._zone.dirtyElectricity = true;
			if (this.Electricity > 0)
			{
				EClass._zone.electricity -= this.Electricity;
				EClass.pc.PlaySound("electricity_off", 1f, true);
			}
		}
		this.OnUninstall();
	}

	public virtual void OnInstall(bool byPlayer)
	{
	}

	public virtual void OnUninstall()
	{
	}

	public virtual bool IsOn
	{
		get
		{
			return this.owner.isOn;
		}
	}

	public virtual bool IsAnimeOn
	{
		get
		{
			return this.IsOn || !this.IsToggle;
		}
	}

	public bool IsToggle
	{
		get
		{
			return this.ToggleType > ToggleType.None;
		}
	}

	public virtual bool AutoToggle
	{
		get
		{
			return (this.IsLighting || this.ToggleType == ToggleType.Curtain || this.ToggleType == ToggleType.Electronics) && !this.owner.disableAutoToggle;
		}
	}

	public bool IsLighting
	{
		get
		{
			return this.ToggleType == ToggleType.Fire || this.ToggleType == ToggleType.Light;
		}
	}

	public virtual bool IsLightOn
	{
		get
		{
			return this.owner.isChara || this.owner.isOn;
		}
	}

	public virtual bool IsNightOnlyLight
	{
		get
		{
			return this.ToggleType != ToggleType.Electronics && this.IsToggle && !this.owner.isRoofItem;
		}
	}

	public virtual Trait.TileMode tileMode
	{
		get
		{
			return Trait.TileMode.Default;
		}
	}

	public virtual bool UseAltTiles
	{
		get
		{
			return this.owner.isOn;
		}
	}

	public virtual bool UseLowblock
	{
		get
		{
			return false;
		}
	}

	public virtual bool UseExtra
	{
		get
		{
			return true;
		}
	}

	public virtual bool UseLightColor
	{
		get
		{
			return true;
		}
	}

	public virtual Color? ColorExtra
	{
		get
		{
			return null;
		}
	}

	public virtual int MaxFuel
	{
		get
		{
			if (this.ToggleType != ToggleType.Fire)
			{
				return 0;
			}
			return 100;
		}
	}

	public virtual int FuelCost
	{
		get
		{
			return 1;
		}
	}

	public virtual bool ShowFuelWindow
	{
		get
		{
			return true;
		}
	}

	public bool IsRequireFuel
	{
		get
		{
			return this.MaxFuel > 0;
		}
	}

	public string IdToggleExtra
	{
		get
		{
			Thing thing = this.owner.Thing;
			if (thing == null)
			{
				return null;
			}
			return thing.source.idToggleExtra;
		}
	}

	public virtual ToggleType ToggleType
	{
		get
		{
			if (this.Electricity >= 0)
			{
				return ToggleType.None;
			}
			return ToggleType.Electronics;
		}
	}

	public virtual void TryToggle()
	{
		if (!this.owner.IsInstalled)
		{
			return;
		}
		if (this.Electricity < 0 && this.owner.isOn && EClass._zone.electricity < 0)
		{
			this.Toggle(false, true);
			return;
		}
		if (this.AutoToggle)
		{
			int hour = EClass.world.date.hour;
			bool on = !this.IsNightOnlyLight || hour >= 17 || hour <= 5 || EClass._map.IsIndoor;
			if (this.ToggleType == ToggleType.Fire && EClass.world.weather.IsRaining && !EClass._map.IsIndoor && !this.owner.Cell.HasRoof)
			{
				on = false;
			}
			this.Toggle(on, true);
		}
	}

	public virtual void Toggle(bool on, bool silent = false)
	{
		if (this.owner.isOn == on)
		{
			return;
		}
		if (this.Electricity < 0)
		{
			if (on)
			{
				if (EClass._zone.electricity + this.Electricity < 0)
				{
					if (EClass._zone.isStarted)
					{
						if (!silent)
						{
							this.owner.Say("notEnoughElectricity", this.owner, null, null);
						}
						this.owner.PlaySound("electricity_insufficient", 1f, true);
					}
					return;
				}
				EClass._zone.electricity += this.Electricity;
			}
			else
			{
				EClass._zone.electricity -= this.Electricity;
			}
		}
		this.owner.isOn = on;
		bool flag = this.ToggleType == ToggleType.Fire;
		ToggleType toggleType = this.ToggleType;
		if (toggleType != ToggleType.None)
		{
			if (toggleType != ToggleType.Curtain)
			{
				if (toggleType == ToggleType.Lever)
				{
					if (!silent)
					{
						this.owner.Say("lever", EClass.pc, this.owner, null, null);
						this.owner.PlaySound("switch_lever", 1f, true);
					}
				}
				else if (on)
				{
					if (!silent)
					{
						this.owner.Say(flag ? "toggle_fire" : "toggle_ele", EClass.pc, this.owner, null, null);
						this.owner.PlaySound((this.Electricity < 0) ? "switch_on_electricity" : (flag ? "torch_lit" : "switch_on"), 1f, true);
					}
					this.RefreshRenderer();
					this.owner.RecalculateFOV();
				}
				else
				{
					if (!silent)
					{
						this.owner.PlaySound((this.Electricity < 0) ? "switch_off_electricity" : (flag ? "torch_unlit" : "switch_off"), 1f, true);
					}
					this.RefreshRenderer();
					this.owner.RecalculateFOV();
				}
			}
			else
			{
				if (!silent)
				{
					this.owner.Say(on ? "close" : "open", EClass.pc, this.owner, null, null);
					this.owner.PlaySound("Material/leather_drop", 1f, true);
				}
				this.owner.pos.RefreshNeighborTiles();
				EClass.pc.RecalculateFOV();
			}
		}
		this.OnToggle();
	}

	public virtual void OnToggle()
	{
	}

	public virtual void TrySetToggleAct(ActPlan p)
	{
		if (!p.IsSelfOrNeighbor)
		{
			return;
		}
		switch (this.ToggleType)
		{
		case ToggleType.Fire:
		case ToggleType.Light:
		case ToggleType.Electronics:
		{
			bool flag = this.ToggleType == ToggleType.Fire;
			if (EClass._zone.IsPCFaction || p.altAction || this is TraitCrafter || this.Electricity < 0)
			{
				if (this.owner.isOn)
				{
					if (p.altAction)
					{
						p.TrySetAct(flag ? "ActExtinguishTorch" : "ActToggleOff", delegate()
						{
							this.Toggle(false, false);
							return true;
						}, this.owner, null, 1, false, true, false);
					}
				}
				else if (!(this is TraitFactory) && !(this is TraitIncubator) && (!this.IsRequireFuel || this.owner.c_charges > 0))
				{
					p.TrySetAct(flag ? "ActTorch" : "ActToggleOn", delegate()
					{
						this.Toggle(true, false);
						return true;
					}, this.owner, null, 1, false, true, false);
				}
				if (this.IsRequireFuel && ((p.altAction && this.owner.c_charges < this.MaxFuel) || (!this.owner.isOn && this.owner.c_charges == 0)) && this.ShowFuelWindow)
				{
					p.TrySetAct("ActFuel", delegate()
					{
						LayerDragGrid.Create(new InvOwnerRefuel(this.owner, null, CurrencyType.None), false);
						return false;
					}, this.owner, null, 1, false, true, false);
				}
			}
			if (p.altAction)
			{
				p.TrySetAct("disableAutoToggle".lang((this.owner.disableAutoToggle ? "off" : "on").lang(), null, null, null, null), delegate()
				{
					this.owner.disableAutoToggle = !this.owner.disableAutoToggle;
					SE.Click();
					return true;
				}, this.owner, null, 1, false, true, false);
			}
			return;
		}
		case ToggleType.Curtain:
			p.TrySetAct(this.owner.isOn ? "actOpen" : "actClose", delegate()
			{
				this.Toggle(!this.owner.isOn, false);
				return true;
			}, this.owner, null, 1, false, true, false);
			return;
		case ToggleType.Lever:
			p.TrySetAct("ActToggleLever", delegate()
			{
				this.Toggle(!this.owner.isOn, false);
				return true;
			}, this.owner, null, 1, false, true, false);
			return;
		default:
			return;
		}
	}

	public bool IsFuelEnough(int num = 1, List<Thing> excludes = null, bool tryRefuel = true)
	{
		if (!this.IsRequireFuel)
		{
			return true;
		}
		if (this.owner.c_charges >= this.FuelCost * num)
		{
			return true;
		}
		if (this.owner.autoRefuel)
		{
			this.TryRefuel(this.FuelCost * num - this.owner.c_charges, excludes);
		}
		return this.owner.c_charges >= this.FuelCost * num;
	}

	public bool IsFuel(string s)
	{
		return this.GetFuelValue(s) > 0;
	}

	public bool IsFuel(Thing t)
	{
		return this.GetFuelValue(t) > 0;
	}

	public int GetFuelValue(Thing t)
	{
		if (t.c_isImportant)
		{
			return 0;
		}
		return this.GetFuelValue(t.id);
	}

	public int GetFuelValue(string id)
	{
		if (this.ToggleType == ToggleType.Electronics)
		{
			if (id == "battery")
			{
				return 20;
			}
		}
		else
		{
			if (id == "log")
			{
				return 20;
			}
			if (id == "branch")
			{
				return 5;
			}
		}
		return 0;
	}

	public void Refuel(Thing t)
	{
		t.PlaySoundDrop(false);
		this.owner.ModCharge(t.Num * this.GetFuelValue(t), false);
		Msg.Say("fueled", t, null, null, null);
		if (!this.owner.isOn)
		{
			this.owner.trait.Toggle(true, false);
		}
		t.Destroy();
		this.owner.renderer.PlayAnime(AnimeID.Shiver, default(Vector3), false);
	}

	public void TryRefuel(int dest, List<Thing> excludes)
	{
		Trait.<>c__DisplayClass311_0 CS$<>8__locals1 = new Trait.<>c__DisplayClass311_0();
		CS$<>8__locals1.dest = dest;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.excludes = excludes;
		if (CS$<>8__locals1.<TryRefuel>g__FindFuel|0(false))
		{
			CS$<>8__locals1.<TryRefuel>g__FindFuel|0(true);
		}
	}

	public virtual void OnEnterScreen()
	{
		this.RefreshRenderer();
	}

	public virtual void RefreshRenderer()
	{
		if (!this.owner.renderer.isSynced || this.IdToggleExtra.IsEmpty())
		{
			return;
		}
		if (this.owner.isOn)
		{
			this.owner.renderer.AddExtra(this.IdToggleExtra);
			return;
		}
		this.owner.renderer.RemoveExtra(this.IdToggleExtra);
	}

	public virtual void SetMainText(UIText t, bool hotitem)
	{
		if (this.owner.isThing && !this.owner.Thing.source.attackType.IsEmpty() && this.owner.ammoData != null)
		{
			string text = this.owner.c_ammo.ToString() ?? "";
			t.SetText(text ?? "", FontColor.Charge);
			t.SetActive(true);
			return;
		}
		if (this.owner.Num == 1 && this.ShowCharges && this.owner.IsIdentified)
		{
			t.SetText(this.owner.c_charges.ToString() ?? "", FontColor.Charge);
			t.SetActive(true);
			return;
		}
		string text2 = (this.owner.Num >= 1000000) ? ((this.owner.Num / 1000000).ToString() + "M") : ((this.owner.Num >= 1000) ? ((this.owner.Num / 1000).ToString() + "K") : (this.owner.Num.ToString() ?? ""));
		t.SetText(text2 ?? "", FontColor.ButtonGrid);
		t.SetActive(this.owner.Num > 1);
	}

	public virtual int ShopLv
	{
		get
		{
			return Mathf.Max(1, EClass._zone.development / 10 + this.owner.c_invest + 1);
		}
	}

	public virtual bool CanCopy(Thing t)
	{
		return false;
	}

	public virtual Trait.CopyShopType CopyShop
	{
		get
		{
			return Trait.CopyShopType.None;
		}
	}

	public virtual int NumCopyItem
	{
		get
		{
			return 2 + Mathf.Min(this.owner.c_invest / 10, 3);
		}
	}

	public virtual ShopType ShopType
	{
		get
		{
			return ShopType.None;
		}
	}

	public virtual CurrencyType CurrencyType
	{
		get
		{
			return CurrencyType.Money;
		}
	}

	public virtual PriceType PriceType
	{
		get
		{
			return PriceType.Default;
		}
	}

	public virtual bool AllowSell
	{
		get
		{
			return this.CurrencyType == CurrencyType.Money || this.CurrencyType == CurrencyType.None;
		}
	}

	public virtual int CostRerollShop
	{
		get
		{
			if (this.CurrencyType == CurrencyType.Money || this.CurrencyType == CurrencyType.Influence)
			{
				return 1;
			}
			return 0;
		}
	}

	public virtual bool AllowCriminal
	{
		get
		{
			return this.owner.isThing;
		}
	}

	public virtual int RestockDay
	{
		get
		{
			return 5;
		}
	}

	public virtual SlaverType SlaverType
	{
		get
		{
			return SlaverType.None;
		}
	}

	public virtual string LangBarter
	{
		get
		{
			return "daBuy";
		}
	}

	public string TextNextRestock
	{
		get
		{
			return this.GetTextRestock(this.LangBarter, false);
		}
	}

	public string TextNextRestockPet
	{
		get
		{
			return this.GetTextRestock((this.SlaverType == SlaverType.Slave) ? "daBuySlave" : "daBuyPet", true);
		}
	}

	public string GetTextRestock(string lang, bool pet)
	{
		int rawDeadLine = 0;
		if (pet)
		{
			SlaverData obj = this.owner.GetObj<SlaverData>(5);
			if (obj != null)
			{
				rawDeadLine = obj.dateRefresh;
			}
		}
		else
		{
			rawDeadLine = this.owner.c_dateStockExpire;
		}
		int remainingHours = EClass.world.date.GetRemainingHours(rawDeadLine);
		if (remainingHours > 0)
		{
			return "nextRestock".lang(lang.lang(), Date.GetText(remainingHours) ?? "", null, null, null);
		}
		return lang.lang();
	}

	public Emo2 GetRestockedIcon()
	{
		if (this.SlaverType != SlaverType.None)
		{
			SlaverData obj = this.owner.GetObj<SlaverData>(5);
			if (obj != null && EClass.world.date.IsExpired(obj.dateRefresh))
			{
				return Emo2.restock;
			}
		}
		int c_dateStockExpire = this.owner.c_dateStockExpire;
		if (c_dateStockExpire == 0 || !EClass.world.date.IsExpired(c_dateStockExpire))
		{
			return Emo2.none;
		}
		if (this.ShopType == ShopType.None)
		{
			return Emo2.blessing;
		}
		return Emo2.restock;
	}

	public void OnBarter()
	{
		Trait.<>c__DisplayClass347_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.t = this.owner.things.Find("chest_merchant", -1, -1);
		if (CS$<>8__locals1.t == null)
		{
			CS$<>8__locals1.t = ThingGen.Create("chest_merchant", -1, -1);
			this.owner.AddThing(CS$<>8__locals1.t, true, -1, -1);
		}
		CS$<>8__locals1.t.c_lockLv = 0;
		if (!EClass.world.date.IsExpired(this.owner.c_dateStockExpire))
		{
			return;
		}
		this.owner.c_dateStockExpire = EClass.world.date.GetRaw(24 * this.RestockDay);
		this.owner.isRestocking = true;
		CS$<>8__locals1.t.things.DestroyAll((Thing _t) => _t.GetInt(101, null) != 0);
		foreach (Thing thing in CS$<>8__locals1.t.things)
		{
			thing.invX = -1;
		}
		ShopType shopType = this.ShopType;
		string id;
		switch (shopType)
		{
		case ShopType.Medal:
			this.<OnBarter>g__NoRestock|347_2(ThingGen.Create("sword_dragon", -1, -1), ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("sword_dragon", 1, 0, ref CS$<>8__locals1).SetReplica(true);
			this.<OnBarter>g__NoRestock|347_2(ThingGen.Create("axe_destruction", -1, -1), ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("axe_destruction", 1, 0, ref CS$<>8__locals1).SetReplica(true);
			this.<OnBarter>g__NoRestock|347_2(ThingGen.Create("blunt_bonehammer", -1, -1), ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("blunt_bonehammer", 1, 0, ref CS$<>8__locals1).SetReplica(true);
			this.<OnBarter>g__NoRestock|347_2(ThingGen.Create("pole_gunlance", -1, -1), ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("pole_gunlance", 1, 0, ref CS$<>8__locals1).SetReplica(true);
			this.<OnBarter>g__NoRestock|347_2(ThingGen.Create("sword_muramasa", -1, -1), ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("sword_muramasa", 1, 0, ref CS$<>8__locals1).SetReplica(true);
			this.<OnBarter>g__NoRestock|347_2(ThingGen.Create("sword_forgetmenot", -1, -1), ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("sword_forgetmenot", 1, 0, ref CS$<>8__locals1).SetReplica(true);
			this.<OnBarter>g__NoRestock|347_2(ThingGen.Create("dagger_fish", -1, -1), ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("dagger_fish", 1, 0, ref CS$<>8__locals1).SetReplica(true);
			this.<OnBarter>g__NoRestock|347_2(ThingGen.Create("sword_zephir", -1, -1), ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("sword_zephir", 1, 0, ref CS$<>8__locals1).SetReplica(true);
			this.<OnBarter>g__Add|347_0("helm_sage", 1, 0, ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("diary_sister", 1, 0, ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("diary_catsister", 1, 0, ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("diary_lady", 1, 0, ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("1165", 1, 0, ref CS$<>8__locals1).SetNum(5);
			this.<OnBarter>g__AddThing|347_1(ThingGen.CreateScroll(9160, 1).SetNum(5), ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("monsterball", 1, 0, ref CS$<>8__locals1).SetNum(3).SetLv(20);
			this.<OnBarter>g__Add|347_0("monsterball", 1, 0, ref CS$<>8__locals1).SetNum(3).SetLv(40);
			this.<OnBarter>g__Add|347_0("bill_tax", 1, 0, ref CS$<>8__locals1).c_bill = 1;
			this.<OnBarter>g__Add|347_0("bill_tax", 1, 0, ref CS$<>8__locals1).c_bill = 1;
			this.<OnBarter>g__Add|347_0("bill_tax", 1, 0, ref CS$<>8__locals1).c_bill = 1;
			this.<OnBarter>g__Add|347_0("container_magic", 1, 0, ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("container_magic", 1, 0, ref CS$<>8__locals1).ChangeMaterial("iron").idSkin = 1;
			this.<OnBarter>g__Add|347_0("container_magic", 1, 0, ref CS$<>8__locals1).ChangeMaterial("bamboo").idSkin = 2;
			this.<OnBarter>g__Add|347_0("wrench_tent_elec", 1, 0, ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("wrench_bed", 1, 0, ref CS$<>8__locals1).SetNum(20);
			this.<OnBarter>g__Add|347_0("wrench_storage", 1, 0, ref CS$<>8__locals1).SetNum(10);
			this.<OnBarter>g__Add|347_0("wrench_fridge", 1, 0, ref CS$<>8__locals1).SetNum(1);
			this.<OnBarter>g__Add|347_0("wrench_extend_v", 1, 0, ref CS$<>8__locals1).SetNum(2);
			this.<OnBarter>g__Add|347_0("wrench_extend_h", 1, 0, ref CS$<>8__locals1).SetNum(2);
			goto IL_E3E;
		case ShopType.CaravanMaster:
			break;
		case ShopType.Starter:
		case ShopType.StarterEx:
			this.<OnBarter>g__Add|347_0("board_home", 1, 0, ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("board_resident", 1, 0, ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("1", 1, 0, ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("2", 1, 0, ref CS$<>8__locals1);
			if (this.ShopType == ShopType.StarterEx)
			{
				this.<OnBarter>g__Add|347_0("board_expedition", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("mailpost", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("record", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("tent2", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("tent1", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("wagon1", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("wagon_big", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("wagon_big2", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("wagon_big3", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("wagon_big4", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("wagon_big5", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("teleporter", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("teleporter2", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("recharger", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("machine_gene2", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__NoRestock|347_2(ThingGen.CreateRecipe("torch_wall"), ref CS$<>8__locals1);
				this.<OnBarter>g__NoRestock|347_2(ThingGen.CreateRecipe("factory_sign"), ref CS$<>8__locals1);
				this.<OnBarter>g__NoRestock|347_2(ThingGen.CreateRecipe("beehive"), ref CS$<>8__locals1);
				this.<OnBarter>g__NoRestock|347_2(ThingGen.Create("rp_food", -1, -1).SetNum(5).SetLv(10).Thing, ref CS$<>8__locals1);
				goto IL_E3E;
			}
			this.<OnBarter>g__AddThing|347_1(ThingGen.CreatePlan(2119), ref CS$<>8__locals1);
			this.<OnBarter>g__NoRestock|347_2(ThingGen.Create("rp_food", -1, -1).SetNum(5).SetLv(5).Thing, ref CS$<>8__locals1);
			goto IL_E3E;
		case ShopType.Farris:
			this.<OnBarter>g__AddThing|347_1(ThingGen.CreateScroll(8220, 4 + EClass.rnd(6)), ref CS$<>8__locals1);
			this.<OnBarter>g__AddThing|347_1(ThingGen.CreateScroll(8221, 4 + EClass.rnd(6)), ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("drawing_paper", 10, 0, ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("drawing_paper2", 10, 0, ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("stethoscope", 1, 0, ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("whip_love", 1, 0, ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("whip_interest", 1, 0, ref CS$<>8__locals1);
			goto IL_E3E;
		default:
			switch (shopType)
			{
			case ShopType.Guild:
				if (this is TraitClerk_Merchant)
				{
					this.<OnBarter>g__Add|347_0("flyer", 1, 0, ref CS$<>8__locals1).SetNum(99);
					goto IL_E3E;
				}
				goto IL_E3E;
			case ShopType.Exotic:
			case ShopType.Gun:
			case ShopType.Dye:
			case ShopType.Healer:
			case ShopType.Milk:
			case ShopType.Ecopo:
				goto IL_DB1;
			case ShopType.Influence:
			{
				bool flag = this.owner.id == "big_sister";
				TraitTicketFurniture.SetZone(flag ? EClass.game.spatials.Find("little_garden") : EClass._zone, this.<OnBarter>g__Add|347_0("ticket_furniture", 1, 0, ref CS$<>8__locals1).SetNum(99));
				if (flag)
				{
					this.<OnBarter>g__Add|347_0("littleball", 10, 0, ref CS$<>8__locals1);
					goto IL_E3E;
				}
				for (int i = 0; i < 10; i++)
				{
					Thing thing2 = ThingGen.Create(EClass._zone.IsFestival ? "1123" : ((EClass.rnd(3) == 0) ? "1169" : "1160"), -1, -1);
					thing2.DyeRandom();
					this.<OnBarter>g__AddThing|347_1(thing2, ref CS$<>8__locals1);
				}
				goto IL_E3E;
			}
			case ShopType.Deed:
				goto IL_396;
			case ShopType.Seed:
				this.<OnBarter>g__AddThing|347_1(TraitSeed.MakeSeed("rice"), ref CS$<>8__locals1).SetNum(4 + EClass.rnd(4));
				this.<OnBarter>g__AddThing|347_1(TraitSeed.MakeSeed("cabbage"), ref CS$<>8__locals1).SetNum(4 + EClass.rnd(4));
				this.<OnBarter>g__AddThing|347_1(TraitSeed.MakeSeed("carrot"), ref CS$<>8__locals1).SetNum(4 + EClass.rnd(4));
				this.<OnBarter>g__AddThing|347_1(TraitSeed.MakeSeed("potato"), ref CS$<>8__locals1).SetNum(4 + EClass.rnd(4));
				this.<OnBarter>g__AddThing|347_1(TraitSeed.MakeSeed("corn"), ref CS$<>8__locals1).SetNum(4 + EClass.rnd(4));
				for (int j = 0; j < EClass.rnd(3) + 1; j++)
				{
					this.<OnBarter>g__Add|347_0("462", 1, 0, ref CS$<>8__locals1);
				}
				for (int k = 0; k < EClass.rnd(3) + 1; k++)
				{
					this.<OnBarter>g__Add|347_0("1167", 1, 0, ref CS$<>8__locals1);
				}
				goto IL_E3E;
			case ShopType.RedBook:
				for (int l = 0; l < 30; l++)
				{
					this.<OnBarter>g__AddThing|347_1(ThingGen.CreateFromCategory("book", -1), ref CS$<>8__locals1);
				}
				goto IL_E3E;
			case ShopType.Casino:
				this.<OnBarter>g__Add|347_0("chest_tax", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("1165", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("monsterball", 1, 0, ref CS$<>8__locals1).SetNum(3).SetLv(10);
				this.<OnBarter>g__Add|347_0("1175", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("1176", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("pillow_ehekatl", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("grave_dagger1", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("grave_dagger2", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("434", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("433", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("714", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("1017", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("1155", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("1011", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__AddThing|347_1(ThingGen.CreatePerfume(9500, 5), ref CS$<>8__locals1);
				this.<OnBarter>g__AddThing|347_1(ThingGen.CreatePerfume(9501, 5), ref CS$<>8__locals1);
				this.<OnBarter>g__AddThing|347_1(ThingGen.CreatePerfume(9502, 5), ref CS$<>8__locals1);
				this.<OnBarter>g__AddThing|347_1(ThingGen.CreatePerfume(9503, 5), ref CS$<>8__locals1);
				for (int m = 0; m < 5; m++)
				{
					Thing thing3 = ThingGen.CreateFromCategory("seasoning", -1).SetNum(10);
					thing3.elements.SetBase(2, 40, 0);
					thing3.c_priceFix = 1000;
					this.<OnBarter>g__AddThing|347_1(thing3, ref CS$<>8__locals1);
				}
				goto IL_E3E;
			case ShopType.Loytel:
				this.<OnBarter>g__Add|347_0("board_map", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("board_build", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("book_resident", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("3", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("4", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("5", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__AddThing|347_1(ThingGen.CreatePlan(2512), ref CS$<>8__locals1);
				this.<OnBarter>g__AddThing|347_1(ThingGen.CreatePlan(2810), ref CS$<>8__locals1);
				this.<OnBarter>g__NoRestock|347_2(ThingGen.Create("rp_block", -1, -1).SetLv(1).SetNum(10), ref CS$<>8__locals1);
				if (EClass.game.quests.GetPhase<QuestVernis>() >= 3)
				{
					this.<OnBarter>g__NoRestock|347_2(ThingGen.CreateRecipe("explosive"), ref CS$<>8__locals1);
					goto IL_E3E;
				}
				goto IL_E3E;
			case ShopType.Specific:
				break;
			case ShopType.Copy:
			{
				Thing c_copyContainer = this.owner.c_copyContainer;
				if (c_copyContainer == null)
				{
					goto IL_E3E;
				}
				int num = 0;
				using (List<Thing>.Enumerator enumerator = c_copyContainer.things.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Thing thing4 = enumerator.Current;
						if (this.owner.trait.CanCopy(thing4))
						{
							Thing thing5 = thing4.Duplicate(1);
							thing5.isStolen = false;
							thing5.isCopy = true;
							int num2 = 1;
							Trait.CopyShopType copyShop = this.owner.trait.CopyShop;
							if (copyShop != Trait.CopyShopType.Item)
							{
								if (copyShop == Trait.CopyShopType.Spellbook)
								{
									thing5.c_charges = thing4.c_charges;
								}
							}
							else
							{
								num2 = (1000 + this.owner.c_invest * 100) / (thing5.GetPrice(CurrencyType.Money, false, PriceType.Default, null) + 50);
								foreach (int ele in new int[]
								{
									701,
									704,
									703,
									702
								})
								{
									if (thing5.HasElement(ele, 1))
									{
										num2 = 1;
									}
								}
							}
							if (num2 > 1 && thing5.trait.CanStack)
							{
								thing5.SetNum(num2);
							}
							this.<OnBarter>g__AddThing|347_1(thing5, ref CS$<>8__locals1);
							num++;
							if (num > this.owner.trait.NumCopyItem)
							{
								break;
							}
						}
					}
					goto IL_E3E;
				}
				break;
			}
			case ShopType.Plat:
				this.<OnBarter>g__NoRestock|347_2(ThingGen.Create("lucky_coin", -1, -1).SetNum(10), ref CS$<>8__locals1);
				goto IL_E3E;
			default:
				goto IL_DB1;
			}
			id = this.owner.id;
			if (id == "mogu")
			{
				this.<OnBarter>g__AddThing|347_1(ThingGen.Create("casino_coin", -1, -1).SetNum(5000), ref CS$<>8__locals1);
				goto IL_E3E;
			}
			if (!(id == "felmera"))
			{
				goto IL_E3E;
			}
			using (List<Thing>.Enumerator enumerator = new DramaOutcome().ListFelmeraBarter().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Thing t = enumerator.Current;
					this.<OnBarter>g__AddThing|347_1(t, ref CS$<>8__locals1);
				}
				goto IL_E3E;
			}
			IL_396:
			this.<OnBarter>g__Add|347_0("deed", 1, 0, ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("deed_move", 2 + EClass.rnd(5), 0, ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("license_void", 1, 0, ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("license_adv", 1, 0, ref CS$<>8__locals1);
			goto IL_E3E;
		}
		IL_DB1:
		float num3 = (float)(3 + Mathf.Min(this.ShopLv / 5, 10)) + Mathf.Sqrt((float)this.ShopLv);
		num3 = num3 * (float)(100 + EClass.pc.Evalue(1406) * 5) / 100f;
		int num4 = 0;
		while ((float)num4 < num3)
		{
			Thing thing6 = this.CreateStock();
			if ((!(thing6.trait is TraitRod) || thing6.c_charges != 0) && thing6.GetPrice(CurrencyType.Money, false, PriceType.Default, null) > 0)
			{
				CS$<>8__locals1.t.AddThing(thing6, true, -1, -1);
			}
			num4++;
		}
		IL_E3E:
		foreach (RecipeSource recipeSource in RecipeManager.list)
		{
			if (!recipeSource.row.recipeKey.IsEmpty())
			{
				string[] recipeKey = recipeSource.row.recipeKey;
				for (int n = 0; n < recipeKey.Length; n++)
				{
					if (recipeKey[n] == this.ShopType.ToString())
					{
						this.<OnBarter>g__NoRestock|347_2(ThingGen.CreateRecipe(recipeSource.id), ref CS$<>8__locals1);
						break;
					}
				}
			}
		}
		shopType = this.ShopType;
		if (shopType <= ShopType.Food)
		{
			if (shopType <= ShopType.Magic)
			{
				if (shopType == ShopType.GeneralExotic)
				{
					this.<OnBarter>g__Add|347_0("tool_talisman", 1, 0, ref CS$<>8__locals1);
					goto IL_12B8;
				}
				if (shopType != ShopType.Magic)
				{
					goto IL_12B8;
				}
				if ((EClass._zone.id == "lumiest" && EClass._zone.lv == 0) || (EClass._zone.id != "lumiest" && EClass.rnd(4) == 0))
				{
					CS$<>8__locals1.t.AddThing(ThingGen.Create("letter_trial", -1, -1), true, -1, -1);
					goto IL_12B8;
				}
				goto IL_12B8;
			}
			else if (shopType != ShopType.Junk)
			{
				if (shopType != ShopType.Food)
				{
					goto IL_12B8;
				}
				this.<OnBarter>g__Add|347_0("ration", 2 + EClass.rnd(4), 0, ref CS$<>8__locals1);
				goto IL_12B8;
			}
		}
		else if (shopType <= ShopType.Gun)
		{
			if (shopType != ShopType.Festival)
			{
				if (shopType != ShopType.Gun)
				{
					goto IL_12B8;
				}
				this.<OnBarter>g__Add|347_0("bullet", 1, 0, ref CS$<>8__locals1).SetNum(300 + EClass.rnd(100)).ChangeMaterial("iron");
				this.<OnBarter>g__Add|347_0("bullet_energy", 1, 0, ref CS$<>8__locals1).SetNum(100 + EClass.rnd(100)).ChangeMaterial("iron");
				goto IL_12B8;
			}
			else
			{
				if (!EClass._zone.IsFestival)
				{
					goto IL_12B8;
				}
				this.<OnBarter>g__Add|347_0("1085", 1, 0, ref CS$<>8__locals1);
				if (EClass._zone.id == "noyel")
				{
					this.<OnBarter>g__Add|347_0("holyFeather", 1, 0, ref CS$<>8__locals1);
					goto IL_12B8;
				}
				goto IL_12B8;
			}
		}
		else
		{
			if (shopType == ShopType.Healer)
			{
				this.<OnBarter>g__AddThing|347_1(ThingGen.CreatePotion(8400, 1).SetNum(4 + EClass.rnd(6)), ref CS$<>8__locals1);
				this.<OnBarter>g__AddThing|347_1(ThingGen.CreatePotion(8401, 1).SetNum(2 + EClass.rnd(4)), ref CS$<>8__locals1);
				this.<OnBarter>g__AddThing|347_1(ThingGen.CreatePotion(8402, 1).SetNum(1 + EClass.rnd(3)), ref CS$<>8__locals1);
				goto IL_12B8;
			}
			switch (shopType)
			{
			case ShopType.Ecopo:
				this.<OnBarter>g__Add|347_0("ecomark", 5, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__Add|347_0("1165", 1, 0, ref CS$<>8__locals1);
				this.<OnBarter>g__AddThing|347_1(ThingGen.CreateScroll(9160, 1).SetNum(5), ref CS$<>8__locals1);
				goto IL_12B8;
			case ShopType.Copy:
			case ShopType.Plat:
				goto IL_12B8;
			case ShopType.LoytelMart:
				break;
			case ShopType.StrangeGirl:
			{
				int num5 = EClass.debug.enable ? 20 : (EClass._zone.development / 10);
				if (num5 > 0)
				{
					this.<OnBarter>g__Add|347_0("syringe_gene", num5, 0, ref CS$<>8__locals1);
				}
				if (num5 > 10)
				{
					this.<OnBarter>g__Add|347_0("syringe_heaven", num5 / 5, 0, ref CS$<>8__locals1);
					goto IL_12B8;
				}
				goto IL_12B8;
			}
			default:
				goto IL_12B8;
			}
		}
		if (this.ShopType == ShopType.LoytelMart)
		{
			this.<OnBarter>g__Add|347_0("ticket_massage", 1, 0, ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("ticket_armpillow", 1, 0, ref CS$<>8__locals1);
			this.<OnBarter>g__Add|347_0("ticket_champagne", 1, 0, ref CS$<>8__locals1);
		}
		for (int num6 = 0; num6 < (EClass.debug.enable ? 30 : 3); num6++)
		{
			if (EClass.rnd(5) == 0)
			{
				TreasureType treasureType = (EClass.rnd(10) == 0) ? TreasureType.BossNefia : ((EClass.rnd(10) == 0) ? TreasureType.Map : TreasureType.RandomChest);
				int num7 = EClass.rnd(EClass.rnd(this.ShopLv + (EClass.debug.enable ? 200 : 50)) + 1) + 1;
				Thing thing7 = ThingGen.Create((treasureType == TreasureType.BossNefia) ? "chest_boss" : ((treasureType == TreasureType.Map) ? "chest_treasure" : "chest3"), -1, -1);
				thing7.c_lockedHard = true;
				thing7.c_lockLv = num7;
				thing7.c_priceAdd = 2000 + num7 * 250 * ((treasureType != TreasureType.RandomChest) ? 5 : 1);
				thing7.c_revealLock = true;
				ThingGen.CreateTreasureContent(thing7, num7, treasureType, true);
				this.<OnBarter>g__AddThing|347_1(thing7, ref CS$<>8__locals1);
			}
		}
		IL_12B8:
		shopType = this.ShopType;
		if (shopType == ShopType.General || shopType == ShopType.Food || shopType == ShopType.Festival)
		{
			for (int num8 = 0; num8 < (EClass.debug.enable ? 30 : 3); num8++)
			{
				if (EClass.rnd(3) == 0)
				{
					int lv = EClass.rnd(EClass.rnd(this.ShopLv + (EClass.debug.enable ? 200 : 50)) + 1) + 1;
					Thing t2 = ThingGen.Create("chest_gamble", -1, lv).SetNum(1 + EClass.rnd(20));
					this.<OnBarter>g__AddThing|347_1(t2, ref CS$<>8__locals1);
				}
			}
		}
		id = this.owner.id;
		if (!(id == "rodwyn"))
		{
			if (!(id == "girl_blue"))
			{
				if (id == "nola")
				{
					this.<OnBarter>g__AddThing|347_1(ThingGen.CreateRecipe("ic").SetPriceFix(400), ref CS$<>8__locals1);
					this.<OnBarter>g__AddThing|347_1(ThingGen.CreateRecipe("bullet").SetPriceFix(300), ref CS$<>8__locals1);
					this.<OnBarter>g__AddThing|347_1(ThingGen.CreateRecipe("break_powder").SetPriceFix(1000), ref CS$<>8__locals1);
					this.<OnBarter>g__AddThing|347_1(ThingGen.CreateRecipe("quarrel").SetPriceFix(100), ref CS$<>8__locals1);
					this.<OnBarter>g__AddThing|347_1(ThingGen.CreateRecipe("1099").SetPriceFix(400), ref CS$<>8__locals1);
					this.<OnBarter>g__AddThing|347_1(ThingGen.CreateRecipe("detector").SetPriceFix(700), ref CS$<>8__locals1);
					this.<OnBarter>g__AddThing|347_1(ThingGen.CreatePlan(2710), ref CS$<>8__locals1).SetPriceFix(-100);
					this.<OnBarter>g__AddThing|347_1(ThingGen.CreatePlan(2711), ref CS$<>8__locals1).SetPriceFix(-100);
				}
			}
			else
			{
				this.<OnBarter>g__Add|347_0("779", 1 + EClass.rnd(3), 0, ref CS$<>8__locals1);
			}
		}
		else
		{
			this.<OnBarter>g__AddThing|347_1(ThingGen.CreateSpellbook(8790, 1), ref CS$<>8__locals1);
			this.<OnBarter>g__AddThing|347_1(ThingGen.CreatePotion(8791, 1).SetNum(3 + EClass.rnd(3)), ref CS$<>8__locals1);
		}
		if (Guild.Thief.IsCurrentZone)
		{
			this.<OnBarter>g__Add|347_0("lockpick", 1, 0, ref CS$<>8__locals1);
			if (EClass.rnd(2) == 0)
			{
				this.<OnBarter>g__Add|347_0("lockpick", 1, 0, ref CS$<>8__locals1);
			}
			this.<OnBarter>g__AddThing|347_1(ThingGen.CreateScroll(8780, EClass.rndHalf(5)), ref CS$<>8__locals1);
		}
		foreach (Thing thing8 in CS$<>8__locals1.t.things)
		{
			thing8.c_idBacker = 0;
			if (this.ShopType != ShopType.Copy)
			{
				thing8.TryMakeRandomItem(this.ShopLv);
				if (thing8.Num == 1)
				{
					thing8.SetNum(thing8.trait.DefaultStock);
				}
				if (thing8.trait is TraitFoodMeal)
				{
					CraftUtil.MakeDish(thing8, this.ShopLv, this.owner.Chara);
				}
				if (thing8.IsFood && this.owner.id == "rodwyn")
				{
					SourceElement.Row row = (from e in EClass.sources.elements.rows
					where !e.foodEffect.IsEmpty() && e.category != "feat"
					select e).RandomItem<SourceElement.Row>();
					thing8.elements.SetBase(row.id, 10 + EClass.rnd(10), 0);
				}
			}
			if (this.CurrencyType == CurrencyType.Casino_coin)
			{
				thing8.noSell = true;
			}
			if (Guild.Thief.IsCurrentZone)
			{
				thing8.isStolen = true;
			}
			if (this.CurrencyType == CurrencyType.Money || !thing8.IsUnique)
			{
				thing8.c_IDTState = 0;
			}
			if (this.CurrencyType == CurrencyType.Money && (thing8.category.IsChildOf("meal") || thing8.category.IsChildOf("preserved")) && thing8.id != "ration")
			{
				thing8.c_priceFix = -70;
			}
			if (thing8.trait is TraitErohon)
			{
				thing8.c_IDTState = 5;
			}
			if (thing8.IsContainer && !thing8.c_revealLock)
			{
				thing8.RemoveThings();
				CS$<>8__locals1.t.c_lockLv = 0;
			}
		}
		if (CS$<>8__locals1.t.things.Count > CS$<>8__locals1.t.things.GridSize)
		{
			int num9 = CS$<>8__locals1.t.things.width * 10;
			if (CS$<>8__locals1.t.things.Count > num9)
			{
				int num10 = CS$<>8__locals1.t.things.Count - num9;
				for (int num11 = 0; num11 < num10; num11++)
				{
					CS$<>8__locals1.t.things.LastItem<Thing>().Destroy();
				}
			}
			CS$<>8__locals1.t.things.ChangeSize(CS$<>8__locals1.t.things.width, Mathf.Min(CS$<>8__locals1.t.things.Count / CS$<>8__locals1.t.things.width + 1, 10));
		}
	}

	public Thing CreateStock()
	{
		switch (this.ShopType)
		{
		case ShopType.GeneralExotic:
			return this.<CreateStock>g__FromFilter|348_1("shop_generalExotic");
		case ShopType.Map:
			return ThingGen.CreateMap(null, -1);
		case ShopType.Plan:
			return this.<CreateStock>g__Create|348_3("book_plan");
		case ShopType.Weapon:
			return this.<CreateStock>g__FromFilter|348_1("shop_weapon");
		case ShopType.Bread:
			if (EClass.rnd(2) == 0)
			{
				return this.<CreateStock>g__Create|348_3("dough");
			}
			return this.<CreateStock>g__FromFilter|348_1("shop_bread");
		case ShopType.Furniture:
			return this.<CreateStock>g__FromFilter|348_1("shop_furniture");
		case ShopType.Magic:
			return this.<CreateStock>g__FromFilter|348_1("shop_magic");
		case ShopType.Blackmarket:
		case ShopType.Exotic:
		{
			int num = 30;
			if (Guild.Thief.IsCurrentZone)
			{
				num = 25;
			}
			if (Guild.Merchant.IsCurrentZone)
			{
				num = 15;
			}
			CardBlueprint.SetRarity((EClass.rnd(num * 5) == 0) ? Rarity.Mythical : ((EClass.rnd(num) == 0) ? Rarity.Legendary : ((EClass.rnd(5) == 0) ? Rarity.Superior : Rarity.Normal)));
			return this.<CreateStock>g__FromFilter|348_1("shop_blackmarket");
		}
		case ShopType.Meat:
			if (EClass.rnd(5) == 0)
			{
				return this.<CreateStock>g__Create|348_3("seasoning");
			}
			return this.<CreateStock>g__FromFilter|348_1("shop_meat");
		case ShopType.Fish:
			if (EClass.rnd(2) == 0)
			{
				return this.<CreateStock>g__Create|348_3("bait");
			}
			if (EClass.rnd(3) == 0)
			{
				return this.<CreateStock>g__Create|348_3("fishingRod");
			}
			return this.<CreateStock>g__FromFilter|348_1("shop_fish");
		case ShopType.Book:
			return this.<CreateStock>g__FromFilter|348_1("shop_book");
		case ShopType.Souvenir:
			return this.<CreateStock>g__FromFilter|348_1("shop_souvenir");
		case ShopType.Junk:
			return this.<CreateStock>g__FromFilter|348_1("shop_junk");
		case ShopType.Drug:
			return this.<CreateStock>g__FromFilter|348_1("shop_drug");
		case ShopType.Drink:
			return this.<CreateStock>g__FromFilter|348_1("shop_drink");
		case ShopType.Fruit:
			return this.<CreateStock>g__FromFilter|348_1("shop_fruit");
		case ShopType.Booze:
			return this.<CreateStock>g__FromFilter|348_1("shop_booze");
		case ShopType.Food:
			if (EClass.rnd(5) == 0)
			{
				return this.<CreateStock>g__Create|348_3("seasoning");
			}
			return this.<CreateStock>g__FromFilter|348_1("shop_food");
		case ShopType.VMachine:
			if (EClass.rnd(10) == 0)
			{
				return this.<CreateStock>g__Create|348_3("panty");
			}
			if (EClass.rnd(5) == 0)
			{
				return this.<CreateStock>g__Create|348_3("234");
			}
			return this.<CreateStock>g__FromFilter|348_1("shop_drink");
		case ShopType.Festival:
			if (EClass.rnd(3) == 0)
			{
				if (Trait.<CreateStock>g__IsFestival|348_0("olvina"))
				{
					return this.<CreateStock>g__Create|348_3(new string[]
					{
						"1125",
						"1126"
					}.RandomItem<string>());
				}
				if (Trait.<CreateStock>g__IsFestival|348_0("yowyn"))
				{
					return this.<CreateStock>g__Create|348_3(new string[]
					{
						"hat_mushroom",
						"hat_witch",
						"hat_kumiromi"
					}.RandomItem<string>());
				}
				if (Trait.<CreateStock>g__IsFestival|348_0("noyel"))
				{
					return this.<CreateStock>g__Create|348_3(new string[]
					{
						"1127",
						"1128"
					}.RandomItem<string>());
				}
			}
			if (EClass.rnd(2) == 0)
			{
				return this.<CreateStock>g__Create|348_3(new string[]
				{
					"1081",
					"1082",
					"1083",
					"1084"
				}.RandomItem<string>());
			}
			if (EClass.rnd(3) == 0)
			{
				return this.<CreateStock>g__FromFilter|348_1("shop_junk");
			}
			return this.<CreateStock>g__FromFilter|348_1("shop_souvenir");
		case ShopType.Fireworks:
			if (EClass.rnd(3) == 0)
			{
				return this.<CreateStock>g__Create|348_3("firework_launcher");
			}
			return this.<CreateStock>g__Create|348_3("firework");
		case ShopType.Lamp:
			if (EClass.rnd(3) != 0)
			{
				if (Trait.<CreateStock>g__IsFestival|348_0("kapul"))
				{
					return this.<CreateStock>g__Create|348_3(new string[]
					{
						"999",
						"1000",
						"1001",
						"1002",
						"1003",
						"1004"
					}.RandomItem<string>());
				}
				if (Trait.<CreateStock>g__IsFestival|348_0("yowyn"))
				{
					return this.<CreateStock>g__Create|348_3(new string[]
					{
						"1072",
						"1073"
					}.RandomItem<string>());
				}
				if (Trait.<CreateStock>g__IsFestival|348_0("noyel"))
				{
					return this.<CreateStock>g__Create|348_3(new string[]
					{
						"1069"
					}.RandomItem<string>());
				}
				if (Trait.<CreateStock>g__IsFestival|348_0("olvina"))
				{
					return this.<CreateStock>g__Create|348_3(new string[]
					{
						"1070",
						"1071"
					}.RandomItem<string>());
				}
			}
			if (EClass._zone.IsFestival && EClass.rnd(2) == 0)
			{
				return this.<CreateStock>g__Create|348_3(new string[]
				{
					"953",
					"954",
					"955",
					"956"
				}.RandomItem<string>());
			}
			return this.<CreateStock>g__FromFilter|348_1("shop_lamp");
		case ShopType.Gun:
			if (EClass.rnd(8) == 0)
			{
				return this.<CreateStock>g__Create|348_3("mod_ranged");
			}
			return this.<CreateStock>g__FromFilter|348_1("shop_gun");
		case ShopType.Dye:
		{
			Thing thing = ThingGen.Create("dye", -1, -1).SetNum(15 + EClass.rnd(30));
			thing.ChangeMaterial(EClass.sources.materials.rows.RandomItem<SourceMaterial.Row>().alias);
			return thing;
		}
		case ShopType.Healer:
		{
			Thing thing2 = null;
			for (int i = 0; i < 1000; i++)
			{
				thing2 = this.<CreateStock>g__FromFilter|348_1("shop_healer");
				TraitScroll traitScroll = thing2.trait as TraitScroll;
				if (traitScroll != null && traitScroll.source != null)
				{
					if (!(traitScroll.source.aliasParent != "WIL"))
					{
						if (!(traitScroll.source.categorySub == "attack"))
						{
							break;
						}
					}
				}
				else
				{
					TraitPotionRandom traitPotionRandom = thing2.trait as TraitPotionRandom;
					if (traitPotionRandom != null && traitPotionRandom.source != null)
					{
						if (!(traitPotionRandom.source.aliasParent != "WIL") && !(traitPotionRandom.source.categorySub == "attack"))
						{
							thing2.SetNum(EClass.rnd(5) + 1);
							break;
						}
					}
					else
					{
						TraitRodRandom traitRodRandom = thing2.trait as TraitRodRandom;
						if (traitRodRandom != null && traitRodRandom.source != null && !(traitRodRandom.source.aliasParent != "WIL") && !(traitRodRandom.source.categorySub == "attack"))
						{
							break;
						}
					}
				}
			}
			return thing2;
		}
		case ShopType.Milk:
			if (EClass._zone is Zone_Nefu && EClass.rnd(2) == 0)
			{
				Thing thing3 = ThingGen.Create("milk", -1, -1);
				thing3.MakeRefFrom((from r in EClass.sources.charas.rows
				where r.race == "mifu" || r.race == "nefu"
				select r).RandomItem<SourceChara.Row>().model, null);
				Debug.Log(thing3);
				return thing3;
			}
			return this.<CreateStock>g__Create|348_3("milk");
		case ShopType.Ecopo:
		{
			Thing thing4 = TraitSeed.MakeRandomSeed(true);
			TraitSeed.LevelSeed(thing4, (thing4.trait as TraitSeed).row, 1);
			return thing4;
		}
		case ShopType.LoytelMart:
			if (EClass.player.flags.loytelMartLv >= 1)
			{
				if (EClass.rnd(10) == 0)
				{
					return this.<CreateStock>g__Create|348_3("monsterball").SetLv(40 + EClass.rnd(this.ShopLv)).Thing;
				}
				if (EClass.rnd(30) == 0)
				{
					return ThingGen.Create("rp_random", -1, this.ShopLv + 10);
				}
				if (EClass.rnd(100) == 0)
				{
					return ThingGen.Create("map_treasure", -1, EClass.rndHalf(this.ShopLv));
				}
				if (EClass.rnd(40) == 0)
				{
					return this.<CreateStock>g__Create|348_3("water").SetPriceFix(1000);
				}
				if (EClass.rnd(EClass.debug.enable ? 20 : 1000) == 0)
				{
					return this.<CreateStock>g__Create|348_3("1165");
				}
			}
			return this.<CreateStock>g__FromFilter|348_1("shop_junk");
		case ShopType.StrangeGirl:
			return DNA.GenerateGene(SpawnList.Get("chara", null, null).Select(this.ShopLv + 10, -1), new DNA.Type?(DNA.Type.Brain), -1, -1);
		}
		if (EClass.rnd(100) == 0)
		{
			return this.<CreateStock>g__Create|348_3("lockpick");
		}
		return this.<CreateStock>g__FromFilter|348_1("shop_general");
	}

	[CompilerGenerated]
	private Thing <OnBarter>g__Add|347_0(string id, int a, int idSkin, ref Trait.<>c__DisplayClass347_0 A_4)
	{
		CardBlueprint.SetNormalRarity(false);
		Thing thing = ThingGen.Create(id, -1, this.ShopLv).SetNum(a);
		thing.idSkin = idSkin;
		return A_4.t.AddThing(thing, true, -1, -1);
	}

	[CompilerGenerated]
	private Thing <OnBarter>g__AddThing|347_1(Thing _t, ref Trait.<>c__DisplayClass347_0 A_2)
	{
		return A_2.t.AddThing(_t, true, -1, -1);
	}

	[CompilerGenerated]
	private void <OnBarter>g__NoRestock|347_2(Thing _t, ref Trait.<>c__DisplayClass347_0 A_2)
	{
		HashSet<string> hashSet = EClass.player.noRestocks.TryGetValue(this.owner.id, null);
		if (hashSet == null)
		{
			hashSet = new HashSet<string>();
		}
		if (hashSet.Contains(_t.trait.IdNoRestock))
		{
			return;
		}
		hashSet.Add(_t.trait.IdNoRestock);
		EClass.player.noRestocks[this.owner.id] = hashSet;
		_t.SetInt(101, 1);
		this.<OnBarter>g__AddThing|347_1(_t, ref A_2);
	}

	[CompilerGenerated]
	internal static bool <CreateStock>g__IsFestival|348_0(string id)
	{
		return EClass._zone.id == id && EClass._zone.IsFestival;
	}

	[CompilerGenerated]
	private Thing <CreateStock>g__FromFilter|348_1(string s)
	{
		return ThingGen.CreateFromFilter(s, this.ShopLv);
	}

	[CompilerGenerated]
	private Thing <CreateStock>g__FromCat|348_2(string s)
	{
		return ThingGen.CreateFromCategory(s, this.ShopLv);
	}

	[CompilerGenerated]
	private Thing <CreateStock>g__Create|348_3(string s)
	{
		return ThingGen.Create(s, -1, this.ShopLv);
	}

	public static TraitSelfFactory SelfFactory = new TraitSelfFactory();

	public Card owner;

	protected static List<Point> listRadiusPoints = new List<Point>();

	public enum TileMode
	{
		Default,
		Door,
		Illumination,
		DefaultNoAnime
	}

	public enum CopyShopType
	{
		None,
		Item,
		Spellbook
	}
}
