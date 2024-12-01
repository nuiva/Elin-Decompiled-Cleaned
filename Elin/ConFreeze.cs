public class ConFreeze : Condition
{
	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner);
		elements = new ElementContainer();
		elements.SetParent(owner);
	}

	public override void OnChangePhase(int lastPhase, int newPhase)
	{
		switch (newPhase)
		{
		case 0:
			elements.SetBase(79, -25);
			break;
		case 1:
			elements.SetBase(79, -50);
			break;
		}
	}
}
