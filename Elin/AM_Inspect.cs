using UnityEngine;

public class AM_Inspect : AM_MoveInstalled
{
	public override bool ShowMouseoverTarget => target == null;

	public override BuildMenu.Mode buildMenuMode => BuildMenu.Mode.Build;

	public override void OnUpdateCursor()
	{
		SetCursorOnMap(CursorSystem.Hand);
	}

	public override void OnUpdateInput()
	{
		if (EClass.debug.enable && Input.GetKeyDown(KeyCode.M))
		{
			Card card = EClass.scene.mouseTarget.card;
			if (card != null)
			{
				card.ChangeMaterial(card.sourceCard.defMat);
				SE.Click();
			}
		}
		base.OnUpdateInput();
	}

	public override string GetHintText()
	{
		if (target == null)
		{
			return null;
		}
		return base.GetHintText();
	}
}
