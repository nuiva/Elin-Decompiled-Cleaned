using System;
using System.IO;
using Newtonsoft.Json;

public class Zone_User : Zone
{
	public override bool IsUserZone
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
			return Path.GetFileNameWithoutExtension(this.path);
		}
	}

	public override string pathExport
	{
		get
		{
			return this.path;
		}
	}

	public override bool HasLaw
	{
		get
		{
			return true;
		}
	}

	public override bool MakeTownProperties
	{
		get
		{
			return true;
		}
	}

	public override int BaseElectricity
	{
		get
		{
			return 1000;
		}
	}

	[JsonProperty]
	public string path;
}
