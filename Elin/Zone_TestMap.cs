public class Zone_TestMap : Zone
{
	public string idMap;

	public override bool BlockBorderExit => true;

	public override string idExport => idMap.IsEmpty(base.idExport);
}
