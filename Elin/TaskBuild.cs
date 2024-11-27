using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class TaskBuild : TaskBaseBuild
{
	public override int destDist
	{
		get
		{
			return 1;
		}
	}

	public override bool CanManualCancel()
	{
		return true;
	}

	public override bool CanPressRepeat
	{
		get
		{
			return this.useHeld && EClass.pc.held != null && EClass.pc.held.trait.CanExtendBuild;
		}
	}

	public override bool HasProgress
	{
		get
		{
			return false;
		}
	}

	public override bool destIgnoreConnection
	{
		get
		{
			return this.recipe.IsFloorOrBridge;
		}
	}

	public bool useHeld
	{
		get
		{
			return this.held != null;
		}
	}

	public override int W
	{
		get
		{
			return this.recipe.W;
		}
	}

	public override int H
	{
		get
		{
			return this.recipe.H;
		}
	}

	public override bool isBlock
	{
		get
		{
			return this.recipe.IsBlock;
		}
	}

	public override string GetText(string str = "")
	{
		if (this.useHeld && EClass.pc.held != null)
		{
			if (EClass.pc.held.category.id == "seed")
			{
				return "actInstallSeed".lang();
			}
			if (EClass.pc.held.id == "fertilizer")
			{
				return "actInstallFertilizer".lang();
			}
		}
		return base.GetText(str);
	}

	public override bool _CanPerformTask(Chara chara, int radius)
	{
		Recipe.Ingredient ingredient = this.recipe.ingredients[0];
		if (!ingredient.IsThingSpecified && ingredient.thing == null)
		{
			ingredient.thing = EClass._map.Stocked.Find(ingredient.id, ingredient.mat, ingredient.refVal, false);
			if (ingredient.thing == null)
			{
				base.TryLayer(30);
				return false;
			}
		}
		return true;
	}

	public override void OnAdd()
	{
		if (this.reqs == null)
		{
			this.reqs = new int[this.recipe.ingredients.Count];
			for (int i = 0; i < this.recipe.ingredients.Count; i++)
			{
				this.reqs[i] = this.recipe.ingredients[i].req;
			}
		}
	}

	public override HitResult GetHitResult()
	{
		if (!this.pos.IsValid || ((this.useHeld ? (!this.pos.IsInBounds) : (!this.pos.IsInBoundsPlus)) && !EClass.debug.ignoreBuildRule))
		{
			return HitResult.Invalid;
		}
		if (this.recipe == null)
		{
			Debug.Log("recipe is null");
			return HitResult.Invalid;
		}
		if (this.useHeld)
		{
			if (EClass.pc.held == null || EClass.pc.held.GetRootCard() != EClass.pc || EClass.pc.held != this.held)
			{
				return HitResult.Invalid;
			}
			if (EClass.pc.held.TileType.IsBlockPass && this.pos.HasChara)
			{
				return HitResult.Invalid;
			}
			if (this.pos.HasBlock && EClass.pc.held.TileType.IsDoor && this.pos.HasWallOrFence && this.pos.cell.blockDir != 2)
			{
				this.recipe.SetDir((this.pos.cell.blockDir == 0) ? 0 : 1);
			}
			if (!EClass.debug.ignoreBuildRule && !EClass._zone.IsPCFaction)
			{
				if (!(EClass._zone is Zone_Tent) && !EClass._zone.IsPCFaction && EClass.pc.held.trait.CanBeOnlyBuiltInHome)
				{
					return HitResult.Invalid;
				}
				if (EClass._zone.RestrictBuild && !EClass.pc.held.trait.CanBuildInTown)
				{
					return HitResult.Invalid;
				}
			}
		}
		if (this.lastPos != null)
		{
			if (this.recipe.IsBlock && this.lastPos.x != this.pos.x && this.lastPos.z != this.pos.z)
			{
				return HitResult.Invalid;
			}
			if (this.recipe.IsWallOrFence && ((this.recipe._dir == 0 && this.pos.z != this.lastPos.z) || (this.recipe._dir == 1 && this.pos.x != this.lastPos.x)))
			{
				return HitResult.Invalid;
			}
		}
		if (!false && this.recipe.HasSameTile(this.pos, this.recipe._dir, this.altitude, this.bridgeHeight))
		{
			return HitResult.Default;
		}
		if (this.recipe.IsThing)
		{
			CardRow cardRow = this.recipe.renderRow as CardRow;
			if (cardRow != null && !base.CanPlaceCard(this.pos, cardRow.model))
			{
				return HitResult.Invalid;
			}
			if (this.recipe.MultiSize && this.useHeld)
			{
				Point point = new Point();
				for (int i = 0; i < this.H; i++)
				{
					for (int j = 0; j < this.W; j++)
					{
						point.Set(this.pos.x - j, this.pos.z + i);
						if (!point.IsValid)
						{
							return HitResult.Invalid;
						}
						HitResult hitResult = this._GetHitResult(point);
						if (hitResult != HitResult.Valid && hitResult != HitResult.Warning)
						{
							return HitResult.Invalid;
						}
					}
				}
				return HitResult.Valid;
			}
		}
		if (this.recipe.IsWallOrFence && this.pos.HasWallOrFence && AM_Adv.actCount == 0 && this.recipe._dir != this.pos.cell.blockDir)
		{
			return HitResult.Valid;
		}
		if (!this.useHeld && EClass.debug.ignoreBuildRule)
		{
			return HitResult.Valid;
		}
		if (EClass.scene.actionMode.IsRoofEditMode(null))
		{
			return HitResult.Valid;
		}
		return this._GetHitResult(this.pos);
	}

	public HitResult _GetHitResult(Point p)
	{
		if (this.useHeld)
		{
			if (EClass.pc.held == null || EClass.pc.held.isDestroyed)
			{
				return HitResult.Invalid;
			}
			if (p.Installed != null)
			{
				if (p.Installed.trait is TraitSeed && !(EClass.pc.held.trait is TraitFertilizer))
				{
					return HitResult.Invalid;
				}
				if (EClass.pc.held.trait is TraitFertilizer && p.Things.LastItem<Thing>().trait is TraitFertilizer)
				{
					return HitResult.Invalid;
				}
				if (EClass.pc.held.trait is TraitSeed)
				{
					using (List<Thing>.Enumerator enumerator = p.Things.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.trait is TraitSeed)
							{
								return HitResult.Invalid;
							}
						}
					}
				}
			}
		}
		return this.recipe.tileType._HitTest(p, this.recipe.Mold, true);
	}

	public override void OnProgressComplete()
	{
		if (this.useHeld)
		{
			if (EClass.pc.held == null || EClass.pc.held.GetRootCard() != EClass.pc || this.pos.Distance(EClass.pc.pos) > 1 || !this.pos.IsInBounds)
			{
				return;
			}
			ActionMode.Build.FixBridge(this.pos, this.recipe);
			this.bridgeHeight = ActionMode.Build.bridgeHeight;
			this.target = (EClass.pc.held.category.installOne ? EClass.pc.held.Split(1) : EClass.pc.held);
			if (this.target.trait is TraitTile)
			{
				this.target.ModNum(-1, true);
			}
			this.dir = this.recipe._dir;
			EClass.pc.LookAt(this.pos);
			EClass.pc.renderer.PlayAnime(AnimeID.Attack_Place, this.pos);
			if (this.target.id == "statue_weird")
			{
				EClass.pc.Say("statue_install", null, null);
			}
		}
		this.lastPos = this.pos.Copy();
		if (ActionMode.Build.IsActive && ActionMode.Build.IsFillMode())
		{
			if (this.recipe.IsBridge)
			{
				this.dir = this.pos.cell.floorDir;
				this.bridgeHeight = (int)this.pos.cell.bridgeHeight;
				this.altitude = 0;
			}
			else if (this.recipe.IsFloor)
			{
				this.dir = this.pos.cell.floorDir;
			}
			else if (this.recipe.IsBlock)
			{
				this.dir = this.pos.cell.blockDir;
			}
		}
		else
		{
			Effect.Get("smoke").Play(this.pos, 0f, null, null);
			Effect.Get("mine").Play(this.pos, 0f, null, null).SetParticleColor(this.recipe.GetColorMaterial().GetColor()).Emit(10 + EClass.rnd(10));
			if (this.recipe.IsWallOrFence)
			{
				if (this.pos.HasWallOrFence && this.pos.cell.blockDir != 2 && this.pos.cell.blockDir != this.recipe._dir)
				{
					this.pos.cell.blockDir = 2;
					EClass.pc.PlaySound(this.pos.matBlock.GetSoundImpact(null), 1f, true);
					this.pos.RefreshTile();
					return;
				}
				if (this.pos.sourceRoofBlock.tileType.IsWallOrFence && this.pos.cell._roofBlockDir % 4 != 2 && (int)(this.pos.cell._roofBlockDir % 4) != this.recipe._dir)
				{
					this.pos.cell._roofBlockDir = this.pos.cell._roofBlockDir / 4 * 4 + 2;
					EClass.pc.PlaySound(this.pos.matBlock.GetSoundImpact(null), 1f, true);
					this.pos.RefreshTile();
					return;
				}
			}
		}
		if (this.bridgeHeight > 150)
		{
			this.bridgeHeight = 150;
		}
		this.recipe.Build(this);
		this.resources.Clear();
		EClass.player.flags.OnBuild(this.recipe);
		EClass._map.RefreshShadow(this.pos.x, this.pos.z);
		EClass._map.RefreshShadow(this.pos.x, this.pos.z - 1);
		EClass._map.RefreshFOV(this.pos.x, this.pos.z, 6, false);
		EClass.pc.renderer.SetFirst(true);
		if (this.recipe.IsFloor)
		{
			foreach (Card card in this.pos.ListThings<TraitNewZone>(true))
			{
				bool isDownstairs = (card.trait as TraitNewZone).IsDownstairs;
			}
		}
	}

	public override void OnDestroy()
	{
		foreach (Thing t in this.resources)
		{
			EClass._zone.AddCard(t, this.pos);
		}
	}

	public override void DrawMarker(int x, int z, RenderParam p)
	{
		this.recipe.OnRenderMarker(Point.shared.Set(x, z), this.owner != null, HitResult.Default, x == this.pos.x && z == this.pos.z, this.dir, this.bridgeHeight + this.altitude);
	}

	[JsonProperty]
	public Recipe recipe;

	[JsonProperty]
	public int dir;

	[JsonProperty]
	public int bridgeHeight;

	[JsonProperty]
	public int altitude;

	[JsonProperty]
	public List<Thing> resources = new List<Thing>();

	[JsonProperty]
	public int[] reqs;

	public Card held;

	public float fx;

	public float fy;

	public bool freePos;

	public Card target;

	public Point lastPos;
}
