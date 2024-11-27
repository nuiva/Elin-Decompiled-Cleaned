using System;
using System.Collections.Generic;
using UnityEngine;

public class AI_HaulResource : TaskPoint
{
	public override IEnumerable<AIAct.Status> Run()
	{
		bool flag;
		do
		{
			flag = true;
			int num2;
			for (int i = 0; i < this.recipe.ingredients.Count; i = num2 + 1)
			{
				Recipe.Ingredient ing = this.recipe.ingredients[i];
				if (this.reqs[i] > 0)
				{
					if (ing.thing == null)
					{
						ing.RefreshThing();
					}
					if (ing.thing == null || ing.thing.isDestroyed)
					{
						ing.thing = null;
						yield return this.Cancel();
					}
					if (!this.HoldingResource(ing))
					{
						yield return base.DoGrab(ing.thing, this.reqs[i], false, null);
					}
					if (this.target == null)
					{
						for (;;)
						{
							Point objPos = null;
							Point blockPos = null;
							this.pos.ForeachMultiSize(this.recipe.W, this.recipe.H, delegate(Point p, bool main)
							{
								if (p.HasObj && !p.HasMinableBlock && !this.recipe.tileType.AllowObj)
								{
									objPos = p.Copy();
								}
								if (p.HasBlock && !this.recipe.tileType.CanBuiltOnBlock)
								{
									blockPos = p.Copy();
								}
							});
							if (blockPos != null)
							{
								yield return base.Do(new TaskMine
								{
									pos = blockPos
								}, null);
							}
							else
							{
								if (objPos == null)
								{
									break;
								}
								yield return base.Do(new TaskCut
								{
									pos = objPos
								}, null);
							}
						}
						yield return base.DoGoto(this.pos, this.destDist, this.destIgnoreConnection, null);
					}
					else
					{
						yield return base.DoGoto(this.target, null);
					}
					if (this.reqs[i] > 0 && this.HoldingResource(ing))
					{
						int num = Mathf.Min(this.reqs[i], this.owner.held.Num);
						this.reqs[i] -= num;
						this.resources.Add(this.owner.SplitHeld(num) as Thing);
						this.owner.PlaySound("build_resource", 1f, true);
					}
					flag = false;
					break;
				}
				num2 = i;
			}
		}
		while (!flag);
		yield break;
	}

	public bool HoldingResource(Recipe.Ingredient ing)
	{
		return this.owner != null && this.owner.held != null && this.owner.held.id == ing.id && (ing.mat == -1 || this.owner.held.material.id == ing.mat) && (ing.refVal == -1 || this.owner.held.refVal == ing.refVal);
	}

	public Recipe recipe;

	public Thing target;

	public List<Thing> resources;

	public int[] reqs;
}
