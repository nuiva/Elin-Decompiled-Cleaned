using UnityEngine;
using UnityEngine.UI;

public class LayerTreasureMap : ELayer
{
	public RawImage image;

	public Vector3 fix;

	public void SetMap(TraitScrollMapTreasure trait)
	{
		ELayer.scene.elomapActor.Initialize(ELayer.world.region.elomap);
		if (!ELayer._zone.IsRegion)
		{
			ELayer.scene.elomapActor.transMap.SetActive(enable: true);
		}
		Point dest = trait.GetDest();
		ELayer.scene.camTreasure.transform.position = dest.PositionTopdownTreasure() + fix;
		ELayer.screen.UpdateShaders(1f);
		ELayer.scene.camTreasure.Render();
		ELayer.screen.UpdateShaders(ELayer.scene.timeRatio);
		if (!ELayer._zone.IsRegion)
		{
			ELayer.scene.elomapActor.transMap.SetActive(enable: false);
		}
	}
}
