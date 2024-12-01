using System.Collections.Generic;

public class GoalList
{
	public int index = -2;

	public List<Goal> list = new List<Goal>();

	public void Refresh(Chara owner, GoalListType type)
	{
		list.Clear();
		index = -1;
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
			list.Add(new GoalIdle());
			break;
		case GoalListType.Enemy:
			list.Add(new GoalVisitorEnemy());
			break;
		default:
			list.Add(new GoalIdle());
			break;
		}
	}

	public Goal Next()
	{
		index++;
		if (index >= list.Count)
		{
			index = 0;
		}
		return list[index].Duplicate();
	}
}
