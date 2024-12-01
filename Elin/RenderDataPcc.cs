using UnityEngine;

public class RenderDataPcc : RenderDataChara
{
	public Vector3 _scale;

	public override string prefabName => "CharaActorPCC";

	private void OnValidate()
	{
		_offset = offset;
	}
}
