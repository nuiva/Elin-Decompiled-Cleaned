public class TraitOlderYoungerSister : TraitUniqueChara
{
	public override bool CanInvite => EClass.player.dialogFlags.TryGetValue("olderyoungersister", 0) >= 1;
}
