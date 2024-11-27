using System;
using System.Collections.Generic;

public class AI_Read : AIAct
{
	public override bool LocalAct
	{
		get
		{
			return this.target == null || (!(this.target.trait is TraitStoryBook) && !(this.target.trait is TraitDeedRelocate));
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
		if (this.target != null && this.target.trait.GetActDuration(this.owner) == 0 && this.target != null && (this.target.GetRootCard() == this.owner || this.target.parent == null))
		{
			this.owner.Say("read", this.owner, this.target.Duplicate(1), null, null);
			Chara owner = this.owner;
			this.target.trait.OnRead(this.owner);
			Thing thing = this.target.Thing;
			if (thing != null)
			{
				thing.Identify(owner.IsPCParty, IDTSource.Identify);
			}
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
		if (this.target.trait.GetActDuration(this.owner) == 0)
		{
			this.owner.Say("read", this.owner, this.target.Duplicate(1), null, null);
			this.target.trait.OnRead(this.owner);
			yield return base.Success(null);
		}
		Progress_Custom seq = new Progress_Custom
		{
			maxProgress = this.target.trait.GetActDuration(this.owner),
			interval = 2,
			canProgress = (() => this.owner.held == this.target),
			onProgressBegin = delegate()
			{
				this.owner.Say("read_start", this.owner, this.target.GetName(NameStyle.Full, 1), null);
				this.owner.PlaySound("read_book", 1f, true);
			},
			onProgress = delegate(Progress_Custom p)
			{
				if (!this.target.trait.TryProgress(p) || this.target.GetRootCard() != this.owner)
				{
					p.Cancel();
					return;
				}
			},
			onProgressComplete = delegate()
			{
				this.owner.PlaySound("read_book_end", 1f, true);
				this.owner.Say("read_end", this.owner, this.target.GetName(NameStyle.Full, 1), null);
				this.target.trait.OnRead(this.owner);
			}
		};
		yield return base.Do(seq, null);
		yield break;
	}

	public Card target;
}
