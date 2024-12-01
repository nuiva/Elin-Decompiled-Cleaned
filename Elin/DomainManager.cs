using System.Collections.Generic;

public class DomainManager : EClass
{
	public Dictionary<string, Domain> dictAll = new Dictionary<string, Domain>();

	public void OnCreateGame()
	{
		Build();
	}

	public void OnLoad()
	{
		Build();
	}

	public void Build()
	{
		foreach (SourceElement.Row row in EClass.sources.elements.rows)
		{
			if (!(row.group != "DOMAIN"))
			{
				dictAll.Add(row.alias, new Domain
				{
					source = row
				});
			}
		}
	}
}
