public class TraitWhistlePeace : TraitItem
{
	public override bool IsTool => true;

	public override bool ShowAsTool => true;

	public override bool OnUse(Chara c)
	{
		EClass._zone.isPeace = !EClass._zone.isPeace;
		EClass.pc.Say("whistle", EClass.pc, owner);
		EClass.pc.Say("whistle_" + (EClass._zone.isPeace ? "peace" : "peace_end"));
		EClass.pc.PlaySound("whistle" + (EClass._zone.isPeace ? "" : "_end"));
		return false;
	}
}
