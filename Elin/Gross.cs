using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Gross : EClass
{
	public virtual int Refresh()
	{
		return this.value;
	}

	public virtual List<Gross.Mod> GetMods()
	{
		return new List<Gross.Mod>();
	}

	[JsonProperty]
	public int value;

	public struct Mod
	{
	}
}
