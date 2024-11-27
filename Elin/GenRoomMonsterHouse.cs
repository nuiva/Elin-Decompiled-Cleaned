using System;

public class GenRoomMonsterHouse : GenRoom
{
	public override void OnPopulate()
	{
		for (int i = 0; i < this.Size / 5 + EClass.rnd(this.Size / 4 + 1); i++)
		{
			base.SetRandomPoint(delegate(Point p)
			{
				base.SpawnMob(p);
			});
		}
	}
}
