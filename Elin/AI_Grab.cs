using System;
using System.Collections.Generic;

public class AI_Grab : AIAct
{
	public virtual Card GetTarget()
	{
		return this.target;
	}

	public virtual bool IsValidTarget(Card c)
	{
		return c != null && c == this.target;
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		if (this.IsValidTarget(this.owner.held))
		{
			yield return base.Success(null);
		}
		this.target = this.GetTarget();
		if (this.target == null)
		{
			yield return this.Cancel();
		}
		Card root = this.target.GetRootCard();
		if (!root.ExistsOnMap)
		{
			yield return this.Cancel();
		}
		yield return base.DoGoto(root, 1, null);
		if (!this.owner.TryHoldCard(this.target, (this.num == -1) ? this.target.Num : this.num, false))
		{
			yield return this.Cancel();
		}
		yield break;
	}

	public Card target;

	public int num = -1;

	public bool pickHeld;
}
