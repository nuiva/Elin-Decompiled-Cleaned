using System;

public class TaskDig : BaseTaskHarvest
{
	public override BaseTaskHarvest.HarvestType harvestType
	{
		get
		{
			return BaseTaskHarvest.HarvestType.Floor;
		}
	}

	public override int RightHand
	{
		get
		{
			return 1101;
		}
	}

	public override bool IsHostileAct
	{
		get
		{
			return true;
		}
	}

	public override bool LocalAct
	{
		get
		{
			return false;
		}
	}

	public override int destDist
	{
		get
		{
			if (EClass._zone.IsSkyLevel)
			{
				return 1;
			}
			if (!EClass._zone.IsRegion)
			{
				return 1;
			}
			return 0;
		}
	}

	public override bool destIgnoreConnection
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
			return CursorSystem.Dig;
		}
	}

	public override string GetTextSmall(Card c)
	{
		if (!this.pos.cell.HasBridge)
		{
			return this.pos.cell.GetFloorName();
		}
		return this.pos.cell.GetBridgeName();
	}

	public override void OnCreateProgress(Progress_Custom p)
	{
		base.SetTarget(this.owner, null);
		p.textHint = this.pos.cell.GetFloorName();
		p.maxProgress = this.maxProgress;
		p.onProgressBegin = delegate()
		{
			if (this.IsTooHard)
			{
				this.owner.Say("tooHardToDig", this.owner, this.pos.cell.HasBridge ? this.pos.cell.GetBridgeName() : this.pos.cell.GetFloorName(), null);
				p.Cancel();
				return;
			}
			if (this.owner.Tool != null)
			{
				this.owner.Say("dig_start", this.owner, this.owner.Tool, null, null);
			}
		};
		p.onProgress = delegate(Progress_Custom _p)
		{
			SourceMaterial.Row row = this.pos.cell.HasBridge ? this.pos.cell.matBridge : this.pos.cell.matFloor;
			this.owner.PlaySound(row.GetSoundImpact(null), 1f, true);
			row.PlayHitEffect(this.pos);
			row.AddBlood(this.pos, 1);
			this.owner.elements.ModExp(230, 5, false);
			this.owner.renderer.NextFrame();
			if (EClass._zone.IsCrime(this.owner, this))
			{
				this.owner.pos.TryWitnessCrime(this.owner, null, 4, null);
			}
		};
	}

	public override HitResult GetHitResult()
	{
		if (EClass._zone.IsRegion && this.GetTreasureMap() != null)
		{
			return HitResult.Valid;
		}
		if (this.mode == TaskDig.Mode.RemoveFloor)
		{
			if (EClass._zone.IsRegion)
			{
				if (this.pos.matFloor.category == "soil")
				{
					return HitResult.Valid;
				}
				return HitResult.Default;
			}
			else
			{
				if (EClass._zone.IsSkyLevel && (this.pos.Installed != null || this.pos.Charas.Count >= 2 || (this.pos.HasChara && this.pos.FirstChara != EClass.pc)))
				{
					return HitResult.Invalid;
				}
				if (this.pos.IsWater || this.pos.HasObj || (!EClass._zone.IsPCFaction && this.pos.HasBlock))
				{
					return HitResult.Invalid;
				}
				if (!this.pos.HasBridge && this.pos.sourceFloor.id == 40)
				{
					return HitResult.Invalid;
				}
				return HitResult.Valid;
			}
		}
		else
		{
			if (!this.pos.HasBridge)
			{
				return HitResult.Default;
			}
			if (this.pos.HasObj)
			{
				return HitResult.Warning;
			}
			return HitResult.Valid;
		}
	}

	public Thing GetTreasureMap()
	{
		foreach (Thing thing in EClass.pc.things.List((Thing t) => t.trait is TraitScrollMapTreasure, false))
		{
			TraitScrollMapTreasure traitScrollMapTreasure = thing.trait as TraitScrollMapTreasure;
			if (this.pos.Equals(traitScrollMapTreasure.GetDest(true)))
			{
				return thing;
			}
		}
		return null;
	}

	public override void OnProgressComplete()
	{
		string idRecipe = this.pos.HasBridge ? this.pos.sourceBridge.RecipeID : this.pos.sourceFloor.RecipeID;
		int num = this.pos.HasBridge ? this.pos.matBridge.hardness : this.pos.matFloor.hardness;
		if (EClass._zone.IsRegion)
		{
			Thing map = this.GetTreasureMap();
			if (map != null || EClass.debug.enable)
			{
				if (map == null)
				{
					map = ThingGen.Create("map_treasure", -1, EClass.pc.LV);
				}
				SE.Play("ding_skill");
				Msg.Say("digTreasure");
				Rand.UseSeed(map.refVal, delegate
				{
					Thing thing = ThingGen.CreateTreasure("chest_treasure", map.LV, TreasureType.Map);
					EClass._zone.AddCard(thing, EClass.pc.pos);
					ThingGen.TryLickChest(thing);
				});
				map.Destroy();
				EClass.player.willAutoSave = true;
				return;
			}
		}
		switch (this.mode)
		{
		case TaskDig.Mode.Default:
			EClass._map.SetBridge(this.pos.x, this.pos.z, 0, 0, 0, 0);
			break;
		case TaskDig.Mode.Ramp:
			EClass._map.MineFloor(this.pos, this.owner, false, true);
			break;
		case TaskDig.Mode.RemoveFloor:
			EClass._map.MineFloor(this.pos, this.owner, false, true);
			this.pos.Animate(AnimeID.Dig, true);
			if (!this.owner.IsAgent)
			{
				this.owner.elements.ModExp(230, 20 + num / 2, false);
			}
			break;
		}
		if (EClass._zone.IsCrime(this.owner, this))
		{
			EClass.player.ModKarma(-1);
		}
		if (EClass.rnd(2) == 0)
		{
			this.owner.stamina.Mod(-1);
		}
		if (this.owner == null)
		{
			return;
		}
		if (this.owner.IsPC)
		{
			EClass.player.recipes.ComeUpWithRecipe(idRecipe, 30);
		}
		if (this.owner.IsPC && this.owner.IsAliveInCurrentZone && EClass._zone.IsSkyLevel && this.owner.pos.IsSky)
		{
			EClass.pc.FallFromZone();
		}
	}

	public TaskDig.Mode mode;

	public int ramp = 3;

	public enum Mode
	{
		Default,
		Ramp,
		RemoveFloor
	}
}
