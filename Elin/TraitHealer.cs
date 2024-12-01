public class TraitHealer : TraitCitizen
{
	public override int GuidePriotiy => 20;

	public override bool CanHeal => true;

	public override bool CanInvest => true;

	public override ShopType ShopType => ShopType.Healer;
}
