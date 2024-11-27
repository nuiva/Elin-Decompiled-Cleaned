using System;

public class AI_OpenLock : AI_TargetThing
{
	public override CursorInfo CursorIcon
	{
		get
		{
			return CursorSystem.Container;
		}
	}

	public override bool HasProgress
	{
		get
		{
			return true;
		}
	}

	public override bool CanTargetInventory
	{
		get
		{
			return true;
		}
	}

	public override AIProgress CreateProgress()
	{
		return new Progress_Custom
		{
			canProgress = new Func<bool>(this.CanProgress),
			onProgressBegin = delegate()
			{
				Thing thing = this.owner.things.FindBest<TraitLockpick>((Thing t) => -t.c_charges);
				if (thing != null)
				{
					this.owner.Say("lockpick_start_pick", thing, base.target, null, null);
				}
				else
				{
					this.owner.Say("lockpick_start", this.owner, base.target, null, null);
				}
				this.owner.PlaySound("lock_pick", 1f, true);
			},
			onProgress = delegate(Progress_Custom p)
			{
				LockOpenState lockOpenState = base.target.trait.TryOpenLock(this.owner, false);
				if (lockOpenState == LockOpenState.Success)
				{
					p.CompleteProgress();
					EClass.Sound.Stop("lock_pick", 0f);
					return;
				}
				if (lockOpenState != LockOpenState.NotEnoughSkill)
				{
					return;
				}
				this.owner.Say("lockpick_end", this.owner, base.target, null, null);
				p.Cancel();
				EClass.Sound.Stop("lock_pick", 0f);
			},
			onProgressComplete = delegate()
			{
				if (base.target.c_lockLv != 0 && this.owner != null)
				{
					this.owner.Say("lockpick_end", this.owner, base.target, null, null);
				}
			}
		}.SetDuration(30, 10);
	}
}
