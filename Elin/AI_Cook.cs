using System;
using System.Collections.Generic;

public class AI_Cook : AIAct
{
	public override int LeftHand
	{
		get
		{
			return 1001;
		}
	}

	public override int RightHand
	{
		get
		{
			return 1002;
		}
	}

	public bool IsValidTarget(Card c)
	{
		return this.factory != null && this.factory.trait.CanCook(c);
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		this.factory = EClass._map.Installed.traits.GetRandomThing<TraitCooker>();
		if (!this.IsValidTarget(this.owner.held))
		{
			yield return this.Cancel();
		}
		yield return base.DoGoto(this.factory, null);
		if (!this.IsValidTarget(this.owner.held))
		{
			yield return this.Cancel();
		}
		Card target = this.owner.DropHeld(this.factory.pos);
		target.TryReserve(this);
		Progress_Custom progress_Custom = new Progress_Custom();
		progress_Custom.canProgress = (() => this.factory.ExistsOnMap && target.ExistsOnMap);
		progress_Custom.onProgress = delegate(Progress_Custom p)
		{
			this.owner.LookAt(this.factory);
			this.factory.trait.CookProgress();
		};
		progress_Custom.onProgressComplete = delegate()
		{
		};
		Progress_Custom seq = progress_Custom.SetDuration(25, 5);
		this.owner.SetTempHand(-1, -1);
		yield return base.Do(seq, null);
		yield return AIAct.Status.Running;
		if (!this.owner.CanPick(target))
		{
			yield return this.Cancel();
		}
		this.owner.HoldCard(target, -1);
		yield break;
	}

	public Card factory;
}
