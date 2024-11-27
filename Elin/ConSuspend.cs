using System;
using Newtonsoft.Json;

public class ConSuspend : BadCondition
{
	public override int GetPhase()
	{
		return 0;
	}

	public override bool ConsumeTurn
	{
		get
		{
			return true;
		}
	}

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner, false);
		this.owner.conSuspend = this;
	}

	public override void Tick()
	{
		if (this.uidMachine == 0)
		{
			return;
		}
		TraitGeneMachine traitGeneMachine = this.owner.pos.FindThing<TraitGeneMachine>();
		if (traitGeneMachine == null || !traitGeneMachine.owner.isOn || (this.duration > 0 && !this.HasGene))
		{
			base.Kill(false);
		}
	}

	public bool HasGene
	{
		get
		{
			return this.gene != null && this.gene.GetRootCard() == this.owner;
		}
	}

	public override void OnRemoved()
	{
		this.owner.conSuspend = null;
		if (this.HasGene)
		{
			this.owner.PickOrDrop(this.owner.pos, this.gene, true);
		}
	}

	[JsonProperty]
	public int uidMachine;

	[JsonProperty]
	public int duration;

	[JsonProperty]
	public int dateFinish;

	[JsonProperty]
	public Thing gene;
}
