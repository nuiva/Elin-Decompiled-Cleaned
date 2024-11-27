using System;

public class TraitSalaryChest : TraitContainer
{
	public override ContainerType ContainerType
	{
		get
		{
			return ContainerType.None;
		}
	}

	public override void Prespawn(int lv)
	{
	}
}
