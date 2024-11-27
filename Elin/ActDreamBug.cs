using System;

public class ActDreamBug : Ability
{
	public override bool Perform()
	{
		if (!Act.TC.isChara || (!Act.TC.IsPC && Act.TC.things.IsFull(0)))
		{
			Msg.SayNothingHappen();
			return true;
		}
		Act.TC.Chara.Pick(ThingGen.Create("dreambug", -1, -1), false, true);
		Act.CC.Say("dreambug", Act.CC, Act.TC, null, null);
		Act.CC.PlaySound("pick_thing", 1f, true);
		return true;
	}
}
