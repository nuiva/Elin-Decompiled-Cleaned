using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class TaskCraft : Task
{
	public Point CraftPos
	{
		get
		{
			if (this.factory == null || !this.factory.ExistsOnMap)
			{
				return EClass.pc.pos;
			}
			return this.factory.pos;
		}
	}

	public override int MaxRestart
	{
		get
		{
			return 99999999;
		}
	}

	public override bool CanManualCancel()
	{
		return true;
	}

	public void ResetReq()
	{
		this.reqs = new int[this.recipe.ingredients.Count];
		for (int i = 0; i < this.recipe.ingredients.Count; i++)
		{
			this.reqs[i] = this.recipe.ingredients[i].req;
		}
	}

	public override bool CanProgress()
	{
		return true;
	}

	public override bool _CanPerformTask(Chara chara, int radius)
	{
		return true;
	}

	public override void OnCreateProgress(Progress_Custom p)
	{
		p.maxProgress = 5;
	}

	public override void OnBeforeProgress()
	{
	}

	public override void OnProgress()
	{
		Element orCreateElement = this.owner.elements.GetOrCreateElement(this.recipe.source.GetReqSkill());
		this.owner.PlaySound(this.recipe.GetMainMaterial().GetSoundCraft(this.recipe.renderRow), 1f, true);
		this.owner.elements.ModExp(orCreateElement.id, 20, false);
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		this.OnBeforeProgress();
		if (!this.CanProgress())
		{
			yield return this.Cancel();
		}
		yield return base.DoProgress();
		this.ResetReq();
		if (this.repeat)
		{
			yield return base.Restart();
		}
		if (this.layer)
		{
			this.layer.OnCompleteCraft();
		}
		if (LayerCraftFloat.Instance)
		{
			LayerCraftFloat.Instance.OnCompleteCraft();
		}
		yield break;
	}

	public bool IsIngredientsValid(bool destoryResources = false, int numCraft = 1)
	{
		bool flag = true;
		for (int i = 0; i < this.recipe.ingredients.Count; i++)
		{
			Thing thing = this.recipe.ingredients[i].thing;
			if (thing == null || thing.isDestroyed || thing.Num < this.reqs[i] * numCraft)
			{
				flag = false;
				break;
			}
			if (destoryResources)
			{
				this.resources.Add(thing.Split(this.reqs[i]));
			}
		}
		if (!flag)
		{
			return false;
		}
		if (destoryResources)
		{
			foreach (Thing thing2 in this.resources)
			{
				thing2.Destroy();
			}
			this.resources.Clear();
		}
		return true;
	}

	public override void OnProgressComplete()
	{
		if (this.owner == null)
		{
			return;
		}
		Element orCreateElement = this.owner.elements.GetOrCreateElement(this.recipe.source.GetReqSkill());
		for (int i = 0; i < this.num; i++)
		{
			this.ResetReq();
			if (!this.IsIngredientsValid(true, 1))
			{
				Msg.Say("invalidCraftResource");
				return;
			}
			this.recipe.Craft(this.blessed, i == 0, null, false);
			this.owner.elements.ModExp(orCreateElement.id, 200, false);
			this.resources.Clear();
		}
		EClass.Sound.Play("craft");
		Effect.Get("smoke").Play(this.CraftPos, 0f, null, null);
		Effect.Get("mine").Play(this.CraftPos, 0f, null, null).SetParticleColor(this.recipe.GetColorMaterial().GetColor()).Emit(10 + EClass.rnd(10));
		EClass.pc.stamina.Mod(-this.costSP);
	}

	public void PutOutResources()
	{
		foreach (Thing t in this.resources)
		{
			EClass._zone.AddCard(t, this.CraftPos);
		}
		this.resources.Clear();
	}

	[JsonProperty]
	public Thing factory;

	[JsonProperty]
	public Recipe recipe;

	[JsonProperty]
	public int num;

	[JsonProperty]
	public int costSP;

	[JsonProperty]
	public List<Thing> resources = new List<Thing>();

	[JsonProperty]
	public int[] reqs;

	[JsonProperty]
	public bool repeat;

	[JsonProperty]
	public bool floatMode;

	public BlessedState blessed;

	public LayerCraft layer;
}
