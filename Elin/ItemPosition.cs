using System;
using Newtonsoft.Json;

public class ItemPosition : EClass
{
	public static ItemPosition Get(Thing t)
	{
		if (t == null || t.parentCard == null)
		{
			return null;
		}
		return new ItemPosition
		{
			uidContainer = t.parentCard.uid,
			invX = t.invX,
			invY = t.invY
		};
	}

	[JsonProperty]
	public int uidContainer;

	[JsonProperty]
	public int invX;

	[JsonProperty]
	public int invY;
}
