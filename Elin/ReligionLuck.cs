public class ReligionLuck : Religion
{
	public override string id => "luck";

	public override bool IsAvailable => true;

	public override void OnBecomeBranchFaith()
	{
	}
}
