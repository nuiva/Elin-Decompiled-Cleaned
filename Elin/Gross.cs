using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

public class Gross : EClass
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct Mod
	{
	}

	[JsonProperty]
	public int value;

	public virtual int Refresh()
	{
		return value;
	}

	public virtual List<Mod> GetMods()
	{
		return new List<Mod>();
	}
}
