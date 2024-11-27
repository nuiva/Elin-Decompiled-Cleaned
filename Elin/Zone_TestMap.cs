using System;

public class Zone_TestMap : Zone
{
	public override bool BlockBorderExit
	{
		get
		{
			return true;
		}
	}

	public override string idExport
	{
		get
		{
			return this.idMap.IsEmpty(base.idExport);
		}
	}

	public string idMap;
}
