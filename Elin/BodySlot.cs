using System;
using Newtonsoft.Json;

public class BodySlot : EClass
{
	public string name
	{
		get
		{
			return EClass.sources.elements.map[this.elementId].GetText("name", false);
		}
	}

	public SourceElement.Row element
	{
		get
		{
			return EClass.sources.elements.map[this.elementId];
		}
	}

	public bool IsEquipping(Thing t)
	{
		return t.c_equippedSlot == this.index + 1;
	}

	[JsonProperty(PropertyName = "E")]
	public int elementId;

	public Thing thing;

	public int index;

	public int indexHnd;
}
