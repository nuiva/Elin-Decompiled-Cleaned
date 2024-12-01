using System.Collections.Generic;

public class AIWork_Explore : AIWork
{
	public override Work_Type WorkType => Work_Type.Explore;

	public override IEnumerable<Status> Run()
	{
		yield return DoIdle();
	}
}
