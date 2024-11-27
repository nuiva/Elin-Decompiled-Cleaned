using System;

public class AIWork_Clean : AIWork_Chore
{
	public override void OnPerformWork(bool realtime)
	{
		for (int i = 0; i < 30; i++)
		{
			Point randomPoint = EClass._map.bounds.GetRandomPoint();
			if (randomPoint.HasDecal)
			{
				EClass._map.SetDecal(randomPoint.x, randomPoint.z, 0, 1, true);
			}
		}
	}
}
