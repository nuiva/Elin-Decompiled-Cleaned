using System;
using UnityEngine;

public class RenderDataObjV : RenderDataObj
{
	public override bool ForceAltHeldPosition
	{
		get
		{
			return true;
		}
	}

	public override void Draw(RenderParam p)
	{
		bool flag = false;
		MeshPass meshPass = (this.hasSubPass && SubPassData.Current.enable) ? this.pass.subPass : this.pass;
		MeshBatch meshBatch = meshPass.batches[meshPass.batchIdx];
		int num = (p.tile > 0f) ? 1 : -1;
		if (meshPass == this.pass.subPass)
		{
			meshBatch.colors[meshPass.idx] = p.color - 1572864f;
			meshBatch.matrices[meshPass.idx].SetTRS(p.NewVector3 + this.offset + SubPassData.Current.offset, SubPassData.Current.rotation, SubPassData.Current.scale);
		}
		else
		{
			meshBatch.colors[meshPass.idx] = p.color;
			meshBatch.matrices[meshPass.idx].m03 = p.x + this.offset.x * (float)num;
			meshBatch.matrices[meshPass.idx].m13 = p.y + this.offset.y;
			meshBatch.matrices[meshPass.idx].m23 = p.z + this.offset.z;
		}
		meshBatch.tiles[meshPass.idx] = p.tile + ((this.multiSize && flag) ? meshPass.pmesh.tiling.x : 0f) + (float)(p.liquidLv * 10000 * num);
		meshBatch.matColors[meshPass.idx] = p.matColor;
		meshPass.idx++;
		if (meshPass.idx == meshPass.batchSize)
		{
			meshPass.NextBatch();
			meshBatch = meshPass.batches[meshPass.batchIdx];
		}
		if (this.multiSize && !flag)
		{
			meshBatch.tiles[meshPass.idx] = p.tile - meshPass.pmesh.tiling.x * (float)num;
			meshBatch.matColors[meshPass.idx] = p.matColor;
			if (meshPass == this.pass.subPass)
			{
				meshBatch.colors[meshPass.idx] = p.color - 1572864f;
				meshBatch.matrices[meshPass.idx].SetTRS(p.NewVector3 + this.offset + new Vector3(0f, meshPass.pmesh.size.y * SubPassData.Current.scale.y, 0f) + SubPassData.Current.offset, SubPassData.Current.rotation, SubPassData.Current.scale);
			}
			else
			{
				meshBatch.colors[meshPass.idx] = p.color;
				meshBatch.matrices[meshPass.idx].m03 = p.x + this.offset.x * (float)num;
				meshBatch.matrices[meshPass.idx].m13 = p.y + this.offset.y + meshPass.pmesh.size.y;
				meshBatch.matrices[meshPass.idx].m23 = p.z + this.offset.z + RenderData.renderSetting.vFix.z;
			}
			meshPass.idx++;
			if (meshPass.idx == meshPass.batchSize)
			{
				meshPass.NextBatch();
			}
		}
		if (this.hasSnowPass && p.snow && meshPass == this.pass)
		{
			meshPass = this.pass.snowPass;
			meshBatch = meshPass.batches[meshPass.batchIdx];
			meshBatch.colors[meshPass.idx] = p.color;
			meshBatch.matrices[meshPass.idx].m03 = p.x + this.offset.x * (float)num;
			meshBatch.matrices[meshPass.idx].m13 = p.y + this.offset.y;
			meshBatch.matrices[meshPass.idx].m23 = p.z + this.offset.z + this.snowZ;
			meshBatch.tiles[meshPass.idx] = p.tile + ((this.multiSize && flag) ? meshPass.pmesh.tiling.x : 0f) + (float)(p.liquidLv * 10000 * num);
			meshBatch.matColors[meshPass.idx] = 104025f;
			meshPass.idx++;
			if (meshPass.idx == meshPass.batchSize)
			{
				meshPass.NextBatch();
				meshBatch = meshPass.batches[meshPass.batchIdx];
			}
			if (this.multiSize && !flag)
			{
				meshBatch.tiles[meshPass.idx] = p.tile - meshPass.pmesh.tiling.x * (float)num;
				meshBatch.matColors[meshPass.idx] = 104025f;
				meshBatch.colors[meshPass.idx] = p.color;
				meshBatch.matrices[meshPass.idx].m03 = p.x + this.offset.x * (float)num;
				meshBatch.matrices[meshPass.idx].m13 = p.y + this.offset.y + meshPass.pmesh.size.y;
				meshBatch.matrices[meshPass.idx].m23 = p.z + this.offset.z + RenderData.renderSetting.vFix.z + this.snowZ;
				meshPass.idx++;
				if (meshPass.idx == meshPass.batchSize)
				{
					meshPass.NextBatch();
				}
			}
		}
	}

	private void OnValidate()
	{
		this._offset = this.offset;
	}

	public bool topOnly;
}
