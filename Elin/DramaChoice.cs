using System;

public class DramaChoice
{
	public DramaChoice(string text, string idJump, string idAction = "", string CHECK = "", string IF = "")
	{
		this.text = text;
		this.idJump = idJump;
		this.idAction = idAction;
		this.CHECK = CHECK;
		this.IF = IF;
	}

	public DramaChoice DisableSound()
	{
		this.sound = false;
		return this;
	}

	public DramaChoice SetOnTooltip(Action<UITooltip> action)
	{
		this.onTooltip = action;
		return this;
	}

	public DramaChoice SetOnClick(Action action)
	{
		this.onClick = action;
		return this;
	}

	public DramaChoice SetCondition(Func<bool> condition)
	{
		this.activeCondition = condition;
		return this;
	}

	public static DramaChoice lastChoice;

	public int index;

	public string text;

	public string idJump;

	public string idAction;

	public string CHECK;

	public string IF;

	public bool sound = true;

	public bool forceHighlight;

	public Action onClick;

	public Action onJump;

	public Action<UITooltip> onTooltip;

	public Func<bool> activeCondition;

	public Check check;

	public UIButton button;
}
