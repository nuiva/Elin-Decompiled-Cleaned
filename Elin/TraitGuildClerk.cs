public class TraitGuildClerk : TraitGuildPersonnel
{
	public override ShopType ShopType => ShopType.Guild;

	public override bool CanGuide => true;
}
