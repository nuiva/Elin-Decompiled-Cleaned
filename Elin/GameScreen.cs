using System;
using UnityEngine;

public class GameScreen : BaseGameScreen
{
	public override bool IsLocalMap
	{
		get
		{
			return true;
		}
	}

	public override void OnActivate()
	{
		bool indoor = EMono._map.config.indoor;
		this.moons[0].SetLocalPositionY((float)this.moonLevel * -this.planeSpeed.y);
		this.moons[1].SetLocalPositionY((float)this.moonLevel * -this.planeSpeed.y + 1.4f);
		Transform[] array = this.moons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(!indoor);
		}
		EMono.scene.RefreshBG();
	}

	public override void SetUnitSize()
	{
		this.tileAlign = new Vector2(this.tileSize.x * 0.005f, this.tileSize.y * 0.005f);
		this.tileWorldSize = new Vector2(this.tileSize.x * 0.01f, this.tileSize.y * 0.01f);
		float x = this.tileSize.x;
		float num = 100f / x;
		float num2 = num * 100f;
		float num3 = num * 100f / 4f;
		this.tileViewSize = new Vector2(num2 * 0.01f, num3 * 0.01f);
	}

	public Transform[] moons;

	public RenderData renderTempEQ;
}
