using System;
using System.Collections.Generic;

public class GoalHobby : GoalWork
{
	public override bool IsHobby
	{
		get
		{
			return true;
		}
	}

	public override List<Hobby> GetWorks()
	{
		return this.owner.ListHobbies(true);
	}
}
