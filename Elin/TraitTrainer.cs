public class TraitTrainer : TraitCitizen
{
	public static string[] ids = new string[7] { "combat", "weapon", "general", "craft", "labor", "mind", "stealth" };

	public override int GuidePriotiy => 50;

	public override string IDTrainer => GetParam(1).IsEmpty(ids[base.owner.uid % ids.Length]);
}
