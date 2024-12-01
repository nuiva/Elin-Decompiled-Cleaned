using System.Collections.Generic;
using UnityEngine;

public class AI_HaulResource : TaskPoint
{
	public Recipe recipe;

	public Thing target;

	public List<Thing> resources;

	public int[] reqs;

	public override IEnumerable<Status> Run()
	{
		bool flag;
		do
		{
			flag = true;
			for (int i = 0; i < recipe.ingredients.Count; i++)
			{
				Recipe.Ingredient ing = recipe.ingredients[i];
				if (reqs[i] <= 0)
				{
					continue;
				}
				if (ing.thing == null)
				{
					ing.RefreshThing();
				}
				if (ing.thing == null || ing.thing.isDestroyed)
				{
					ing.thing = null;
					yield return Cancel();
				}
				if (!HoldingResource(ing))
				{
					yield return DoGrab(ing.thing, reqs[i]);
				}
				if (target == null)
				{
					while (true)
					{
						Point objPos = null;
						Point blockPos = null;
						pos.ForeachMultiSize(recipe.W, recipe.H, delegate(Point p, bool main)
						{
							if (p.HasObj && !p.HasMinableBlock && !recipe.tileType.AllowObj)
							{
								objPos = p.Copy();
							}
							if (p.HasBlock && !recipe.tileType.CanBuiltOnBlock)
							{
								blockPos = p.Copy();
							}
						});
						if (blockPos != null)
						{
							yield return Do(new TaskMine
							{
								pos = blockPos
							});
							continue;
						}
						if (objPos == null)
						{
							break;
						}
						yield return Do(new TaskCut
						{
							pos = objPos
						});
					}
					yield return DoGoto(pos, destDist, destIgnoreConnection);
				}
				else
				{
					yield return DoGoto(target);
				}
				if (reqs[i] > 0 && HoldingResource(ing))
				{
					int num = Mathf.Min(reqs[i], owner.held.Num);
					reqs[i] -= num;
					resources.Add(owner.SplitHeld(num) as Thing);
					owner.PlaySound("build_resource");
				}
				flag = false;
				break;
			}
		}
		while (!flag);
	}

	public bool HoldingResource(Recipe.Ingredient ing)
	{
		if (owner != null && owner.held != null && owner.held.id == ing.id && (ing.mat == -1 || owner.held.material.id == ing.mat))
		{
			if (ing.refVal != -1)
			{
				return owner.held.refVal == ing.refVal;
			}
			return true;
		}
		return false;
	}
}
