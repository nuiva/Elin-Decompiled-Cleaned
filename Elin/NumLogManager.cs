using System.Collections.Generic;
using Newtonsoft.Json;

public class NumLogManager : EClass
{
	public List<NumLog> all = new List<NumLog>();

	[JsonProperty]
	public List<NumLog> listCategory = new List<NumLog>();

	[JsonProperty]
	public List<NumLog> listImportant = new List<NumLog>();

	public void OnLoad()
	{
	}

	public void OnCreateGame()
	{
	}

	public void OnAdvanceDay()
	{
	}

	public void OnAdvanceMonth()
	{
	}

	public void OnAdvanceYear()
	{
	}
}
