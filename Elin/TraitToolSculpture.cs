using System;

public class TraitToolSculpture : TraitWorkbench
{
	public override bool Contains(RecipeSource r)
	{
		return r.idFactory == "factory_sculpture";
	}
}
