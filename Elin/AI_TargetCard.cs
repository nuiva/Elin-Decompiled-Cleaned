using System;
using System.Collections.Generic;

public class AI_TargetCard : AIAct
{
	public virtual bool GotoTarget
	{
		get
		{
			return false;
		}
	}

	public override bool HasProgress
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanTargetInventory
	{
		get
		{
			return false;
		}
	}

	public override bool CanProgress()
	{
		return this.target.ExistsOnMap || (this.CanTargetInventory && this.target.GetRootCard() == this.owner);
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		this.isFail = (() => !this.CanProgress());
		if (this.target.ExistsOnMap)
		{
			if (this.target.isThing && !this.GotoTarget)
			{
				yield return base.DoGotoInteraction(this.target.pos, null);
			}
			else
			{
				yield return base.DoGoto(this.target, null);
			}
			if (this.target.Dist(this.owner) > 1)
			{
				yield return this.Cancel();
			}
			this.owner.LookAt(this.target);
		}
		else if (!this.CanTargetInventory || this.target.GetRootCard() != this.owner)
		{
			yield return this.Cancel();
		}
		if (this.HasProgress)
		{
			yield return base.DoProgress();
		}
		else
		{
			this.OnProgressComplete();
		}
		yield break;
	}

	public Card target;
}
