using System;

public class ListPeople : BaseListPeople
{
	public override bool ShowCharaSheet
	{
		get
		{
			return true;
		}
	}

	public override bool ShowGoto
	{
		get
		{
			return true;
		}
	}

	public override bool ShowHome
	{
		get
		{
			return this.memberType != FactionMemberType.Guest;
		}
	}

	public override bool ShowShowMode
	{
		get
		{
			return true;
		}
	}
}
