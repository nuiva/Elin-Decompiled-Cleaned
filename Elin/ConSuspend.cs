using Newtonsoft.Json;

public class ConSuspend : BadCondition
{
	[JsonProperty]
	public int uidMachine;

	[JsonProperty]
	public int duration;

	[JsonProperty]
	public int dateFinish;

	[JsonProperty]
	public Thing gene;

	public override bool ConsumeTurn => true;

	public bool HasGene
	{
		get
		{
			if (gene != null)
			{
				return gene.GetRootCard() == owner;
			}
			return false;
		}
	}

	public override int GetPhase()
	{
		return 0;
	}

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner);
		owner.conSuspend = this;
	}

	public override void Tick()
	{
		if (uidMachine != 0)
		{
			TraitGeneMachine traitGeneMachine = owner.pos.FindThing<TraitGeneMachine>();
			if (traitGeneMachine == null || !traitGeneMachine.owner.isOn || (duration > 0 && !HasGene))
			{
				Kill();
			}
		}
	}

	public override void OnRemoved()
	{
		owner.conSuspend = null;
		if (HasGene)
		{
			owner.PickOrDrop(owner.pos, gene);
		}
	}
}
