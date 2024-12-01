public class AI_TendAnimal : AI_Fuck
{
	public override FuckType Type => FuckType.tame;

	public override bool CanTame()
	{
		if (target == null || target.isDead)
		{
			return false;
		}
		if (!target.trait.CanBeTamed)
		{
			return false;
		}
		return true;
	}
}
