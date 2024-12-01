public class TraitStairsUp : TraitStairs
{
	public override ZoneTransition.EnterState enterState => ZoneTransition.EnterState.Up;

	public override string langOnUse => "stairsUp";

	public override bool IsUpstairs => true;
}
