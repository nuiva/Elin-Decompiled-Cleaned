using System;
using System.Collections.Generic;

public class GoalList
{
	public void Refresh(Chara owner, GoalListType type)
	{
		this.list.Clear();
		this.index = -1;
		if (type == GoalListType.None)
		{
			if (owner.IsHomeMember())
			{
				type = GoalListType.Home;
			}
			else if (owner.IsGuest())
			{
				type = GoalListType.Guest;
			}
		}
		switch (type)
		{
		case GoalListType.Home:
		case GoalListType.Guest:
			this.list.Add(new GoalIdle());
			return;
		case GoalListType.Enemy:
			this.list.Add(new GoalVisitorEnemy());
			return;
		}
		this.list.Add(new GoalIdle());
	}

	public Goal Next()
	{
		this.index++;
		if (this.index >= this.list.Count)
		{
			this.index = 0;
		}
		return this.list[this.index].Duplicate();
	}

	public int index = -2;

	public List<Goal> list = new List<Goal>();
}
