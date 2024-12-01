public class ListPeople : BaseListPeople
{
	public override bool ShowCharaSheet => true;

	public override bool ShowGoto => true;

	public override bool ShowHome => memberType != FactionMemberType.Guest;

	public override bool ShowShowMode => true;
}
