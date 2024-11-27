using System;

public class HomeResourceSafety : HomeResourceRate
{
	public override int GetDestValue()
	{
		int num = 0;
		foreach (Chara chara in this.branch.members)
		{
			num += chara.Evalue(70);
		}
		return num;
	}
}
