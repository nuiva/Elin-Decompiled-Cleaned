public class GenRoomMonsterHouse : GenRoom
{
	public override void OnPopulate()
	{
		for (int i = 0; i < Size / 5 + EClass.rnd(Size / 4 + 1); i++)
		{
			SetRandomPoint(delegate(Point p)
			{
				SpawnMob(p);
			});
		}
	}
}
