using System;

public class InspectGroupEloPos : InspectGroup<EloPos>
{
	public EloMapActor actor
	{
		get
		{
			return EClass.scene.elomapActor;
		}
	}
}
