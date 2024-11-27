using System;

public class TraitAltarChaos : Trait
{
	public override bool CanBeHeld
	{
		get
		{
			return false;
		}
	}

	public override bool CanBeDestroyed
	{
		get
		{
			return false;
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		p.TrySetAct("actWorship", delegate()
		{
			LayerDrama.currentReligion = EClass.game.religions.Earth;
			LayerDrama.Activate("_adv", "god", "worship", null, null, "");
			return false;
		}, this.owner, null, 1, false, true, false);
	}
}
