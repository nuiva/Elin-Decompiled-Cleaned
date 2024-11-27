using System;

public class TaskMine : BaseTaskHarvest
{
	public override BaseTaskHarvest.HarvestType harvestType
	{
		get
		{
			return BaseTaskHarvest.HarvestType.Block;
		}
	}

	public override int destDist
	{
		get
		{
			return 1;
		}
	}

	public override bool isBlock
	{
		get
		{
			return true;
		}
	}

	public override int RightHand
	{
		get
		{
			return 1004;
		}
	}

	public override bool destIgnoreConnection
	{
		get
		{
			return true;
		}
	}

	public override bool ShowMapHighlightBlock
	{
		get
		{
			return true;
		}
	}

	public override CursorInfo CursorIcon
	{
		get
		{
			return CursorSystem.Mine;
		}
	}

	public override bool IsHostileAct
	{
		get
		{
			return true;
		}
	}

	public static bool CanMine(Point pos, Card t)
	{
		if (t != null && pos.HasBlock && (!pos.HasObj || !pos.sourceObj.tileType.IsBlockMount))
		{
			if (t.HasElement(220, 1))
			{
				return true;
			}
			if ((pos.matBlock.category == "wood" || pos.matBlock.category == "grass") && t.HasElement(225, 1))
			{
				return true;
			}
		}
		return false;
	}

	public override string GetTextSmall(Card c)
	{
		if (!this.pos.cell.HasBlock)
		{
			return this.pos.cell.GetFloorName();
		}
		return this.pos.cell.GetBlockName();
	}

	public override void OnCreateProgress(Progress_Custom p)
	{
		base.SetTarget(this.owner, null);
		p.textHint = this.pos.cell.GetBlockName();
		p.maxProgress = this.maxProgress;
		p.onProgressBegin = delegate()
		{
			if (!TaskMine.CanMine(this.pos, this.owner.Tool))
			{
				p.Cancel();
				return;
			}
			if (this.IsTooHard)
			{
				this.owner.Say("tooHardToMine", this.owner, this.pos.cell.GetBlockName(), null);
				p.Cancel();
				return;
			}
			if (this.owner.Tool != null)
			{
				this.owner.Say("mine_start", this.owner, this.owner.Tool, null, null);
			}
		};
		p.onProgress = delegate(Progress_Custom _p)
		{
			this.owner.LookAt(this.pos);
			this.owner.PlaySound(this.pos.matBlock.GetSoundImpact(null), 1f, true);
			this.pos.Animate(AnimeID.HitObj, true);
			this.pos.matBlock.PlayHitEffect(this.pos);
			this.pos.matBlock.AddBlood(this.pos, 1);
			this.owner.renderer.NextFrame();
			this.owner.elements.ModExp(220, 5, false);
			if (EClass._zone.IsCrime(this.owner, this))
			{
				this.owner.pos.TryWitnessCrime(this.owner, null, 4, null);
			}
		};
	}

	public override void DrawMarker(int x, int z, RenderParam p)
	{
		if (ActionMode.Mine.IsRoofEditMode(null) && this.pos.HasWallOrFence)
		{
			EClass.screen.guide.DrawWall(this.pos, this.Working ? EClass.Colors.blockColors.ActiveOpacity : EClass.Colors.blockColors.InactiveOpacity, false, 0f);
			return;
		}
		base.DrawMarker(x, z, p);
	}

	public override HitResult GetHitResult()
	{
		if (ActionMode.Mine.IsRoofEditMode(null))
		{
			if (this.pos.cell._roofBlock == 0)
			{
				return HitResult.Default;
			}
			return HitResult.Valid;
		}
		else if (this.pos.cell.HasBlock || !this.pos.cell.isSeen)
		{
			if (this.pos.sourceBlock.tileType.Invisible && !ActionMode.Mine.IsActive)
			{
				return HitResult.Default;
			}
			if (this.mode == TaskMine.Mode.Ramp && (this.pos.cell.HasRamp || EClass._map.GetRampDir(this.pos.x, this.pos.z, EClass.sources.blocks.rows[this.ramp].tileType) == -1))
			{
				return HitResult.Default;
			}
			return HitResult.Valid;
		}
		else
		{
			if (!this.mined && !this.pos.HasObj && this.owner != null && !this.owner.IsAgent && this.pos.Equals(this.owner.pos) && this.pos.Installed == null && EClass._zone.CanDigUnderground)
			{
				return HitResult.Valid;
			}
			return HitResult.Default;
		}
	}

	public override void OnProgressComplete()
	{
		string recipeID = this.pos.sourceBlock.RecipeID;
		int hardness = this.pos.matBlock.hardness;
		TaskMine.Mode mode = this.mode;
		if (mode != TaskMine.Mode.Default)
		{
			if (mode == TaskMine.Mode.Ramp)
			{
				EClass._map.MineRamp(this.pos, (this.ramp == 3) ? this.pos.matBlock.ramp : this.ramp, false);
			}
		}
		else if (this.pos.HasBlock || ActionMode.Mine.IsRoofEditMode(null))
		{
			if (this.owner.IsPC)
			{
				EClass.player.stats.digs++;
			}
			EClass._map.MineBlock(this.pos, false, this.owner);
		}
		else if (this.pos.Installed == null)
		{
			EClass._zone.AddThing("stairsDown_cave", this.pos.x, this.pos.z).Install();
			return;
		}
		if (!this.owner.IsAgent)
		{
			this.owner.elements.ModExp(220, 20 + hardness, false);
			if (EClass.rnd(10) == 0)
			{
				EClass._map.TrySmoothPick(this.pos, ThingGen.Create("pebble", -1, -1), this.owner);
			}
			if (EClass.rnd(10) == 0)
			{
				EClass._map.TrySmoothPick(this.pos, ThingGen.Create("stone", -1, -1), this.owner);
			}
			if (EClass._zone.IsCrime(this.owner, this))
			{
				EClass.player.ModKarma(-1);
			}
			if (EClass.rnd(2) == 0)
			{
				this.owner.stamina.Mod(-1);
			}
		}
		if (this.owner != null && this.owner.IsPC)
		{
			EClass.player.recipes.ComeUpWithRecipe(recipeID, 30);
		}
		this.mined = true;
	}

	public TaskMine.Mode mode;

	public int ramp = 3;

	public bool mined;

	public enum Mode
	{
		Default,
		Ramp
	}
}
