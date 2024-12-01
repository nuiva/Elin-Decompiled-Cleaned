public class TraitBitch : TraitCitizen
{
	public override string IDRumor => "bitch";

	public override bool CanWhore => true;

	public override bool CanGuide => EClass._zone.id == "derphy";
}
