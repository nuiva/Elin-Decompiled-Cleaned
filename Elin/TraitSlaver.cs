public class TraitSlaver : TraitCitizen
{
	public override int GuidePriotiy => 15;

	public override string IDRumor => "slaver";

	public override SlaverType SlaverType => SlaverType.Slave;
}
