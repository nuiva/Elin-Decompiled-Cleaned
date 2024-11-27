using System;

public class HomeResourceEducation : HomeResourceRate
{
	public override int GetDestValue()
	{
		int num = 0;
		foreach (Chara chara in this.branch.members)
		{
			num += chara.Evalue(74);
		}
		return num;
	}
}
