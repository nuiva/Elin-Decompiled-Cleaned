using System;

public class RenderDataChara : RenderDataCard
{
	public override string prefabName
	{
		get
		{
			return "CharaActor";
		}
	}

	private void OnValidate()
	{
		this._offset = this.offset;
	}
}
