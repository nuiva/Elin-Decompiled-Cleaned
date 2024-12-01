public class HomeResourceEducation : HomeResourceRate
{
	public override int GetDestValue()
	{
		int num = 0;
		foreach (Chara member in branch.members)
		{
			num += member.Evalue(74);
		}
		return num;
	}
}
