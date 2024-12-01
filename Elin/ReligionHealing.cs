public class ReligionHealing : Religion
{
	public override string id => "healing";

	public override bool IsAvailable => true;

	public override void OnBecomeBranchFaith()
	{
	}
}
