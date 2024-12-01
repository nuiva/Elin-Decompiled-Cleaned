using UnityEngine;

public class GameScreen : BaseGameScreen
{
	public Transform[] moons;

	public RenderData renderTempEQ;

	public override bool IsLocalMap => true;

	public override void OnActivate()
	{
		bool indoor = EMono._map.config.indoor;
		moons[0].SetLocalPositionY((float)moonLevel * (0f - planeSpeed.y));
		moons[1].SetLocalPositionY((float)moonLevel * (0f - planeSpeed.y) + 1.4f);
		Transform[] array = moons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(!indoor);
		}
		EMono.scene.RefreshBG();
	}

	public override void SetUnitSize()
	{
		tileAlign = new Vector2(tileSize.x * 0.005f, tileSize.y * 0.005f);
		tileWorldSize = new Vector2(tileSize.x * 0.01f, tileSize.y * 0.01f);
		float x = tileSize.x;
		float num = 100f / x;
		float num2 = num * 100f;
		float num3 = num * 100f / 4f;
		tileViewSize = new Vector2(num2 * 0.01f, num3 * 0.01f);
	}
}
