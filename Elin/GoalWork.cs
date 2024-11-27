using System;
using System.Collections.Generic;

public class GoalWork : Goal
{
	public virtual bool IsHobby
	{
		get
		{
			return false;
		}
	}

	public virtual List<Hobby> GetWorks()
	{
		return this.owner.ListWorks(true);
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		if (this.FindWork(this.owner, true))
		{
			yield return base.Success(null);
		}
		yield return base.DoIdle(3);
		yield break;
	}

	public void ValidateHobby(Chara c)
	{
		this.owner = c;
		WorkSummary workSummary = this.owner.GetWorkSummary();
		List<Hobby> list = this.owner.ListHobbies(true);
		workSummary.hobbies.Clear();
		Room room = this.owner.FindRoom();
		int i = 0;
		while (i < list.Count)
		{
			if (room == null)
			{
				goto IL_BE;
			}
			Lot lot = room.lot;
			if (!this.TryWork(room, list[i], false))
			{
				bool flag = false;
				foreach (Room room2 in EClass._map.rooms.listRoom)
				{
					if (room2.lot == lot && room2 != room && this.TryWork(room2, list[i], false))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					goto IL_BE;
				}
			}
			IL_CE:
			i++;
			continue;
			IL_BE:
			this.TryWork(null, list[i], false);
			goto IL_CE;
		}
	}

	public bool FindWork(Chara c, bool setAI = true)
	{
		this.owner = c;
		WorkSummary workSummary = this.owner.GetWorkSummary();
		if (this.IsHobby)
		{
			workSummary.hobbies.Clear();
		}
		else
		{
			workSummary.work = null;
		}
		Room room = this.owner.FindRoom();
		if (room != null)
		{
			if (this.TryWork(room, setAI))
			{
				return true;
			}
			Lot lot = room.lot;
			foreach (Room room2 in EClass._map.rooms.listRoom)
			{
				if (room2.lot == lot && room2 != room && !room2.IsPrivate && this.TryWork(room2, setAI))
				{
					return true;
				}
			}
		}
		return this.TryWork(null, setAI);
	}

	public bool TryWork(BaseArea destArea, bool setAI = true)
	{
		List<Hobby> works = this.GetWorks();
		if (this.IsHobby)
		{
			Hobby h = works.RandomItem<Hobby>();
			return this.TryWork(destArea, h, setAI);
		}
		foreach (Hobby h2 in works)
		{
			if (this.TryWork(destArea, h2, setAI))
			{
				return true;
			}
		}
		return false;
	}

	public bool TryWork(BaseArea destArea, Hobby h, bool setAI)
	{
		AIWork ai = h.GetAI(this.owner);
		ai.destArea = destArea;
		if (ai.SetDestination())
		{
			WorkSummary workSummary = this.owner.GetWorkSummary();
			if (this.IsHobby)
			{
				workSummary.hobbies.Add(ai.GetSession());
			}
			else
			{
				workSummary.work = ai.GetSession();
			}
			if (setAI)
			{
				this.owner.SetAI(ai);
			}
			return true;
		}
		return false;
	}

	public override void OnSimulatePosition()
	{
		Room room = this.owner.FindRoom();
		if (room != null)
		{
			this.owner.MoveImmediate(room.GetRandomFreePos(), true, true);
		}
	}
}
