using System;
using UnityEngine;
using UnityEngine.UI;

public class LayerTreasureMap : ELayer
{
	public unsafe void SetMap(TraitScrollMapTreasure trait)
	{
		ELayer.scene.elomapActor.Initialize(ELayer.world.region.elomap);
		if (!ELayer._zone.IsRegion)
		{
			ELayer.scene.elomapActor.transMap.SetActive(true);
		}
		Point dest = trait.GetDest(false);
		ELayer.scene.camTreasure.transform.position = *dest.PositionTopdownTreasure() + this.fix;
		ELayer.screen.UpdateShaders(1f);
		ELayer.scene.camTreasure.Render();
		ELayer.screen.UpdateShaders(ELayer.scene.timeRatio);
		if (!ELayer._zone.IsRegion)
		{
			ELayer.scene.elomapActor.transMap.SetActive(false);
		}
	}

	public RawImage image;

	public Vector3 fix;
}
