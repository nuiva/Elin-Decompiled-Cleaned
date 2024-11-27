using System;
using System.Collections.Generic;

public class DomainManager : EClass
{
	public void OnCreateGame()
	{
		this.Build();
	}

	public void OnLoad()
	{
		this.Build();
	}

	public void Build()
	{
		foreach (SourceElement.Row row in EClass.sources.elements.rows)
		{
			if (!(row.group != "DOMAIN"))
			{
				this.dictAll.Add(row.alias, new Domain
				{
					source = row
				});
			}
		}
	}

	public Dictionary<string, Domain> dictAll = new Dictionary<string, Domain>();
}
