public class TraitAltarAncient : TraitAltar
{
	public override string idDeity => EClass.Branch?.faith.id ?? EClass.game.religions.Eyth.id;

	public override Religion Deity => EClass.Branch?.faith ?? EClass.game.religions.Eyth;
}
