public class TraitAshland : TraitUniqueChara
{
	public override bool CanInvite => EClass._zone.id == "lothria";
}
