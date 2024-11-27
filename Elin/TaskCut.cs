using System;

public class TaskCut : TaskDesignation
{
	public override CursorInfo CursorIcon
	{
		get
		{
			return CursorSystem.IconCut;
		}
	}

	public override int destDist
	{
		get
		{
			return 1;
		}
	}

	public override void OnStart()
	{
		if (this.pos.cell.CanMakeStraw())
		{
			this.owner.ShowEmo(Emo.straw, 0f, true);
			return;
		}
		this.owner.ShowEmo(Emo.cut, 0f, true);
	}

	public override HitResult GetHitResult()
	{
		if (this.pos.HasObj && !this.pos.HasMinableBlock)
		{
			return HitResult.Valid;
		}
		if (this.pos.HasDecal && EClass.debug.godBuild && BuildMenu.Instance)
		{
			return HitResult.Valid;
		}
		return HitResult.Default;
	}

	public override void OnCreateProgress(Progress_Custom p)
	{
		p.maxProgress = this.pos.cell.sourceObj.hp / 10 + 1;
	}

	public override void OnProgress()
	{
		SourceObj.Row sourceObj = this.pos.cell.sourceObj;
		if (this.pos.cell.CanMakeStraw())
		{
			this.owner.SetTempHand(1006, -1);
		}
		else if (this.pos.cell.matObj.UseAxe)
		{
			this.owner.SetTempHand(1100, -1);
		}
		else if (this.pos.cell.matObj.UsePick)
		{
			this.owner.SetTempHand(1004, -1);
		}
		else
		{
			this.owner.SetTempHand(1000, -1);
		}
		this.owner.LookAt(this.pos);
		this.pos.PlaySound(this.pos.cell.matObj.GetSoundImpact(null), true, 1f, true);
		this.pos.cell.matObj.PlayHitEffect(this.pos);
		this.pos.cell.matObj.AddBlood(this.pos, 1);
		this.pos.Animate(AnimeID.HitObj, false);
		if (this.IsToolValid() && EClass.setting.toolConsumeHP)
		{
			Act.TOOL.DamageHP(1, AttackSource.None, null);
		}
	}

	public override void OnProgressComplete()
	{
		string text = this.pos.sourceObj.reqHarvest[0];
		EClass._map.MineObj(this.pos, null, null);
		EClass._map.SetDecal(this.pos.x, this.pos.z, 0, 1, true);
	}
}
