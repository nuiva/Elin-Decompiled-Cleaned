public class TraitLetterOfWill : Trait
{
	public override void WriteNote(UINote n, bool identified)
	{
		base.WriteNote(n, identified);
		n.AddText("isPreventDeathPanalty", FontColor.Good);
		n.AddText("isGraveSkin", FontColor.Good);
	}
}
