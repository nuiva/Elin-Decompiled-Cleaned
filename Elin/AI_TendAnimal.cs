using System;

public class AI_TendAnimal : AI_Fuck
{
	public override AI_Fuck.FuckType Type
	{
		get
		{
			return AI_Fuck.FuckType.tame;
		}
	}

	public override bool CanTame()
	{
		return this.target != null && !this.target.isDead && this.target.trait.CanBeTamed;
	}
}
