using System;

public class GridItem : EClass
{
	public virtual void SetButton(ButtonGrid b)
	{
		b.SetDummy();
	}

	public virtual void OnClick(ButtonGrid b)
	{
	}

	public virtual void AutoAdd()
	{
	}
}
