using System;
using System.Collections.Generic;

public class AI_PracticeDummy : AIAct
{
	public override CursorInfo CursorIcon
	{
		get
		{
			return CursorSystem.IconMelee;
		}
	}

	public override bool HasProgress
	{
		get
		{
			return true;
		}
	}

	public override bool CanManualCancel()
	{
		return true;
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		this.isFail = (() => !this.target.IsAliveInCurrentZone);
		yield return base.DoProgress();
		yield break;
	}

	public override AIProgress CreateProgress()
	{
		Progress_Custom progress_Custom = new Progress_Custom();
		progress_Custom.canProgress = (() => !this.isFail());
		progress_Custom.onProgressBegin = delegate()
		{
		};
		progress_Custom.onProgress = delegate(Progress_Custom p)
		{
			if (this.throwItem != null)
			{
				if (!ActThrow.CanThrow(EClass.pc, this.throwItem, this.target, null))
				{
					p.Cancel();
					return;
				}
				ActThrow.Throw(EClass.pc, this.target.pos, this.target, this.throwItem, ThrowMethod.Default);
			}
			else if (this.range && this.owner.GetCondition<ConReload>() == null)
			{
				if (!ACT.Ranged.CanPerform(this.owner, this.target, this.target.pos))
				{
					p.Cancel();
					return;
				}
				if (!ACT.Ranged.Perform(this.owner, this.target, this.target.pos))
				{
					p.Cancel();
				}
			}
			else
			{
				ACT.Melee.Perform(this.owner, this.target, null);
			}
			if (this.owner != null && EClass.rnd(5) < 2)
			{
				this.owner.stamina.Mod(-1);
			}
			if (this.owner != null && this.owner.stamina.value < 0)
			{
				p.Cancel();
			}
		};
		progress_Custom.onProgressComplete = delegate()
		{
		};
		return progress_Custom.SetDuration(10000, 2);
	}

	public Card target;

	public Thing throwItem;

	public bool range;
}
