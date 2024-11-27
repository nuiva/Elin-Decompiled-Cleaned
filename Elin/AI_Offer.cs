using System;
using System.Collections.Generic;
using System.Linq;

public class AI_Offer : AIAct
{
	public bool IsValidTarget(Card c)
	{
		return this.altar != null && this.altar.trait.CanOffer(c);
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		if (this.target != null && this.target.ExistsOnMap)
		{
			yield return base.DoGoto(this.target, null);
		}
		this.altar = EClass._map.Installed.traits.altars.GetRandomInstalled();
		if (this.altar == null)
		{
			yield return this.Cancel();
		}
		if (!this.IsValidTarget(this.owner.held))
		{
			yield return base.DoGrab(EClass._map.charas.Where(new Func<Chara, bool>(this.IsValidTarget)).RandomItem<Chara>(), -1, false, null);
			if (!this.IsValidTarget(this.owner.held))
			{
				yield return this.Cancel();
			}
		}
		yield return base.DoGoto(this.altar, null);
		if (!this.IsValidTarget(this.owner.held))
		{
			yield return this.Cancel();
		}
		this.owner.DropHeld(this.altar.pos);
		Progress_Custom seq = new Progress_Custom
		{
			canProgress = (() => this.altar.ExistsOnMap),
			onProgress = delegate(Progress_Custom p)
			{
				this.altar.trait.OfferProcess(this.owner);
			},
			onProgressComplete = delegate()
			{
				this.altar.trait.Offer(this.owner);
			}
		}.SetDuration(15, 5);
		yield return base.Do(seq, null);
		yield break;
	}

	public Card target;

	public Card altar;
}
