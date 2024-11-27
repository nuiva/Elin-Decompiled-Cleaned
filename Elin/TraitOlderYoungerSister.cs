using System;

public class TraitOlderYoungerSister : TraitUniqueChara
{
	public override bool CanInvite
	{
		get
		{
			return EClass.player.dialogFlags.TryGetValue("olderyoungersister", 0) >= 1;
		}
	}
}
