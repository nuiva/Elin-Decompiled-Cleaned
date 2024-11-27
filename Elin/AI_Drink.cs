using System;
using System.Collections.Generic;

public class AI_Drink : AIAct
{
	public bool IsValidTarget(Card c)
	{
		return c != null && c.trait is TraitDrink;
	}

	public override bool LocalAct
	{
		get
		{
			return false;
		}
	}

	public override bool IsHostileAct
	{
		get
		{
			return this.target != null && this.target.isNPCProperty;
		}
	}

	public override void OnSetOwner()
	{
		if (this.target != null && (this.target.GetRootCard() == this.owner || this.target.parent == null))
		{
			this.owner.Drink(this.target);
			base.Success(null);
		}
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		if (this.target != null && (this.target.GetRootCard() == this.owner || this.target.parent == null))
		{
			this.owner.HoldCard(this.target, 1);
		}
		else if (this.target != null)
		{
			yield return base.DoGrab(this.target, 1, false, null);
		}
		else
		{
			yield return base.DoGrab<TraitDrink>();
		}
		this.target = this.owner.held;
		if (this.target == null)
		{
			yield return this.Cancel();
		}
		yield return base.Success(delegate
		{
			this.owner.Drink(this.target);
		});
		yield break;
	}

	public Card target;
}
