using System;

public class AIWork_Chore : AIWork
{
	public override void SetDestPos()
	{
		if (EClass._map.rooms.listRoom.Count <= 0 || EClass.rnd(2) != 0)
		{
			this.destPos = EClass._map.bounds.GetRandomSurface(false, true, false);
			return;
		}
		Room room = EClass._map.rooms.listRoom.RandomItem<Room>();
		if (!room.IsPrivate)
		{
			this.destPos = room.GetRandomPoint(true, true);
			return;
		}
		this.destPos = EClass._map.bounds.GetRandomSurface(false, true, false);
	}

	public override void OnPerformWork(bool realtime)
	{
	}
}
