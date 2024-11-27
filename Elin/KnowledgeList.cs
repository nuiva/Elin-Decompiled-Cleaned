using System;
using System.Collections.Generic;

public class KnowledgeList<T> : HashSet<string> where T : Knowledge
{
	public new void Add(string id)
	{
	}

	public void Add(T k)
	{
	}
}
