public class TraitAltarChaos : Trait
{
	public override bool CanBeHeld => false;

	public override bool CanBeDestroyed => false;

	public override void TrySetAct(ActPlan p)
	{
		p.TrySetAct("actWorship", delegate
		{
			LayerDrama.currentReligion = EClass.game.religions.Earth;
			LayerDrama.Activate("_adv", "god", "worship");
			return false;
		}, owner);
	}
}
