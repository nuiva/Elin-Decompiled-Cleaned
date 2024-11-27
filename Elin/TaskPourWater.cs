using System;
using System.Runtime.CompilerServices;

public class TaskPourWater : TaskDesignation
{
	public override bool CanProgress()
	{
		return base.CanProgress() && !this.pos.HasBridge && this.pos.cell.sourceSurface.alias != "floor_water_deep" && this.pot.owner.c_charges > 0 && this.owner.Tool == this.pot.owner;
	}

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
			return true;
		}
	}

	public override bool Loop
	{
		get
		{
			return this.CanProgress();
		}
	}

	public override void OnCreateProgress(Progress_Custom p)
	{
		p.textHint = this.Name;
		p.maxProgress = 6;
		p.onProgressBegin = delegate()
		{
			if (this.owner.Tool != null)
			{
				this.owner.Say("pourWater_start", this.owner, this.owner.Tool, null, null);
			}
		};
		p.onProgress = delegate(Progress_Custom _p)
		{
			(this.pos.cell.HasBridge ? this.pos.cell.matBridge : this.pos.cell.matFloor).PlayHitEffect(this.pos);
			this.owner.PlaySound(MATERIAL.sourceWaterSea.GetSoundImpact(null), 1f, true);
		};
		p.onProgressComplete = delegate()
		{
			if (this.pot.owner.DyeMat == null)
			{
				this.pot.owner.Dye(MATERIAL.sourceWaterSea);
			}
			string alias = (this.pos.HasBridge ? this.pos.sourceBridge : this.pos.sourceFloor).alias;
			if (!(alias == "floor_water_shallow2"))
			{
				if (!(alias == "floor_water_shallow"))
				{
					if (!(alias == "floor_water"))
					{
						this.<OnCreateProgress>g__ChangeFloor|9_3("floor_water_shallow2");
					}
					else
					{
						this.<OnCreateProgress>g__ChangeFloor|9_3("floor_water_deep");
					}
				}
				else
				{
					this.<OnCreateProgress>g__ChangeFloor|9_3("floor_water");
				}
			}
			else
			{
				this.<OnCreateProgress>g__ChangeFloor|9_3("floor_water_shallow");
			}
			Effect.Get("mine").Play(this.pos, 0f, null, null).SetParticleColor(this.pos.cell.HasBridge ? this.pos.matBridge.GetColor() : this.pos.matFloor.GetColor()).Emit(10 + EClass.rnd(10));
			this.pos.Animate(AnimeID.Dig, true);
			this.owner.PlaySound("water_farm", 1f, true);
			this.pot.owner.ModCharge(-1, false);
			if (this.pot.owner.c_charges <= 0)
			{
				this.pot.owner.Dye(EClass.sources.materials.alias["void"]);
			}
			this.owner.elements.ModExp(286, 5, false);
			if (EClass.rnd(3) == 0)
			{
				this.owner.stamina.Mod(-1);
			}
		};
	}

	public override HitResult GetHitResult()
	{
		if (this.pos.HasBridge || this.pos.HasObj || this.pos.cell.blocked)
		{
			return HitResult.Invalid;
		}
		return HitResult.Valid;
	}

	[CompilerGenerated]
	private void <OnCreateProgress>g__ChangeFloor|9_3(string id)
	{
		SourceFloor.Row row = EClass.sources.floors.alias[id];
		if (this.pos.HasBridge)
		{
			this.pos.cell._bridge = (byte)row.id;
			this.pos.cell._bridgeMat = (byte)this.pot.owner.DyeMat.id;
		}
		else
		{
			this.pos.cell._floor = (byte)row.id;
			this.pos.cell._floorMat = (byte)this.pot.owner.DyeMat.id;
		}
		EClass._map.SetLiquid(this.pos.x, this.pos.z, null);
		this.pos.RefreshNeighborTiles();
	}

	public TraitToolWaterPot pot;
}
