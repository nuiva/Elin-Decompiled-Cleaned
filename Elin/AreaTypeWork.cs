public class AreaTypeWork : AreaType
{
	public override bool IsWork => true;

	public override bool CanAssign => true;

	public override bool IsPublicArea => false;
}
