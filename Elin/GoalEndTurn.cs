public class GoalEndTurn : Goal
{
	public override bool InformCancel => false;

	public override bool CancelWhenDamaged => false;
}
