using System;

public class ConFreeze : Condition
{
	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner, false);
		this.elements = new ElementContainer();
		this.elements.SetParent(this.owner);
	}

	public override void OnChangePhase(int lastPhase, int newPhase)
	{
		if (newPhase == 0)
		{
			this.elements.SetBase(79, -25, 0);
			return;
		}
		if (newPhase != 1)
		{
			return;
		}
		this.elements.SetBase(79, -50, 0);
	}
}
