using System;
using UnityEngine;

public class TaskPlow : TaskDesignation
{
	public override bool CanProgress()
	{
		return base.CanProgress() && !this.pos.IsFarmField;
	}

	public override CursorInfo CursorIcon
	{
		get
		{
			return CursorSystem.Dig;
		}
	}

	public override bool CanPressRepeat
	{
		get
		{
			return true;
		}
	}

	public override int destDist
	{
		get
		{
			if (!this.pos.cell.blocked)
			{
				return 0;
			}
			return 1;
		}
	}

	public override void OnCreateProgress(Progress_Custom p)
	{
		p.textHint = this.Name;
		p.maxProgress = Mathf.Max((15 + EClass.rnd(20)) * 100 / (100 + this.owner.Tool.material.hardness * 3), 2);
		p.onProgressBegin = delegate()
		{
			if (this.owner.Tool != null)
			{
				this.owner.Say("till_start", this.owner, this.owner.Tool, null, null);
			}
		};
		p.onProgress = delegate(Progress_Custom _p)
		{
			SourceMaterial.Row row = this.pos.cell.HasBridge ? this.pos.cell.matBridge : this.pos.cell.matFloor;
			row.PlayHitEffect(this.pos);
			row.AddBlood(this.pos, 1);
			this.pos.PlaySound(row.GetSoundImpact(null), true, 1f, true);
			this.owner.renderer.NextFrame();
			if (!(this.pos.HasBridge ? this.pos.sourceBridge : this.pos.sourceFloor).tag.Contains("soil"))
			{
				this.owner.Say("till_invalid", null, null);
				_p.Cancel();
				return;
			}
		};
		p.onProgressComplete = delegate()
		{
			this.owner.PlaySound(this.pos.cell.HasBridge ? this.pos.cell.matBridge.GetSoundDead(this.pos.sourceBridge) : this.pos.cell.matFloor.GetSoundDead(this.pos.sourceFloor), 1f, true);
			Effect.Get("mine").Play(this.pos, 0f, null, null).SetParticleColor(this.pos.cell.HasBridge ? this.pos.matBridge.GetColor() : this.pos.matFloor.GetColor()).Emit(10 + EClass.rnd(10));
			this.pos.Animate(AnimeID.Dig, true);
			if (this.pos.HasBridge)
			{
				this.pos.cell._bridge = 4;
			}
			else
			{
				this.pos.SetFloor(this.pos.matFloor.id, 4);
			}
			this.owner.elements.ModExp(286, 30, false);
			this.owner.stamina.Mod(-1);
		};
	}

	public override HitResult GetHitResult()
	{
		if (this.pos.cell.IsTopWater || this.pos.HasObj || this.pos.IsFarmField)
		{
			return HitResult.Invalid;
		}
		return HitResult.Valid;
	}
}
