using System;

public class TargetTypeEnemy : TargetTypeChara
{
	public override bool CanOnlyTargetEnemy
	{
		get
		{
			return true;
		}
	}
}
