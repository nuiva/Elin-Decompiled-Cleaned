using System;
using System.Runtime.CompilerServices;

public class TaskDrawWater : TaskDesignation
{
	public override bool CanProgress()
	{
		return base.CanProgress() && this.pos.cell.IsTopWater && this.pot.owner.c_charges < this.pot.MaxCharge && this.owner.Tool == this.pot.owner;
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

	public override CursorInfo CursorIcon
	{
		get
		{
			return CursorSystem.Hand;
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
				this.owner.Say("drawWater_start", this.owner, this.owner.Tool, null, null);
			}
		};
		p.onProgress = delegate(Progress_Custom _p)
		{
			SourceMaterial.Row row = this.pos.cell.HasBridge ? this.pos.cell.matBridge : this.pos.cell.matFloor;
			row.PlayHitEffect(this.pos);
			this.owner.PlaySound(row.GetSoundImpact(null), 1f, true);
		};
		p.onProgressComplete = delegate()
		{
			Effect.Get("mine").Play(this.pos, 0f, null, null).SetParticleColor(this.pos.cell.HasBridge ? this.pos.matBridge.GetColor() : this.pos.matFloor.GetColor()).Emit(10 + EClass.rnd(10));
			this.pos.Animate(AnimeID.Dig, true);
			this.owner.PlaySound("water", 1f, true);
			this.pot.owner.Dye(this.pos.HasBridge ? this.pos.matBridge : this.pos.matFloor);
			string alias = (this.pos.HasBridge ? this.pos.sourceBridge : this.pos.sourceFloor).alias;
			if (!(alias == "floor_water_shallow"))
			{
				if (!(alias == "floor_water"))
				{
					if (!(alias == "floor_water_deep"))
					{
						this.<OnCreateProgress>g__ChangeFloor|11_3("floor_raw3");
					}
					else
					{
						this.<OnCreateProgress>g__ChangeFloor|11_3("floor_water");
					}
				}
				else
				{
					this.<OnCreateProgress>g__ChangeFloor|11_3("floor_water_shallow");
				}
			}
			else
			{
				this.<OnCreateProgress>g__ChangeFloor|11_3("floor_water_shallow2");
			}
			this.pot.owner.ModCharge(1, false);
			this.owner.elements.ModExp(286, 5, false);
			if (EClass.rnd(3) == 0)
			{
				this.owner.stamina.Mod(-1);
			}
		};
	}

	public override HitResult GetHitResult()
	{
		if (!this.pos.cell.IsTopWater || this.pos.HasObj || this.pos.cell.HasFullBlock)
		{
			return HitResult.Invalid;
		}
		return HitResult.Valid;
	}

	[CompilerGenerated]
	private void <OnCreateProgress>g__ChangeFloor|11_3(string id)
	{
		SourceFloor.Row row = EClass.sources.floors.alias[id];
		if (this.pos.HasBridge)
		{
			this.pos.cell._bridge = (byte)row.id;
			if (id == "floor_raw3")
			{
				this.pos.cell._bridgeMat = 45;
			}
		}
		else
		{
			this.pos.cell._floor = (byte)row.id;
			if (id == "floor_raw3")
			{
				this.pos.cell._floorMat = 45;
			}
		}
		EClass._map.SetLiquid(this.pos.x, this.pos.z, null);
		this.pos.RefreshNeighborTiles();
	}

	public TraitToolWaterPot pot;
}
