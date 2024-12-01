public class ConEntangle : BadCondition
{
	public override int GetPhase()
	{
		return 0;
	}

	public override bool TryMove(Point p)
	{
		return false;
	}

	public override void Tick()
	{
		if (EClass.rnd(10) == 0 && owner.IsHumanSpeak)
		{
			owner.Talk((EClass.rnd(3) == 0) ? "scold" : ((EClass.rnd(3) == 0) ? "pervert" : ((EClass.rnd(3) == 0) ? "labor" : "restrained")));
		}
		base.Tick();
	}
}
