using System;

public class HomeResourceNature : HomeResourceRate
{
	public override int GetDestValue()
	{
		int p = 0;
		EClass._map.bounds.ForeachCell(delegate(Cell c)
		{
			if (c.sourceFloor.value == 0)
			{
				p++;
			}
			if (c.sourceBlock.value == 0)
			{
				p += 2;
			}
			if (c.sourceBridge.value == 0)
			{
				p++;
			}
		});
		return p / 10;
	}
}
