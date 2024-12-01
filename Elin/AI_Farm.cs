public class AI_Farm : TaskPoint
{
	public override int LeftHand => -1;

	public override int RightHand => 1006;

	public override bool HasProgress => true;

	public override void OnProgress()
	{
		owner.PlaySound("Material/mud");
	}

	public override void OnProgressComplete()
	{
	}
}
