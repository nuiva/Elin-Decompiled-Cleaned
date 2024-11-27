using System;
using UnityEngine;

namespace Assets.Resources.Scene.Render
{
	public class RenderDataRoof : RenderDataTile
	{
		public override void Draw(RenderParam p)
		{
			MeshPass meshPass = (this.hasSubPass && SubPassData.Current.enable) ? this.pass.subPass : this.pass;
			MeshBatch meshBatch = meshPass.batches[meshPass.batchIdx];
			int num = 1;
			this._offset = this.offsets[(int)p.tile % this.offsets.Length] + this.offsetFixes[(int)p.tile % 2];
			if (p.shadowFix != 0f)
			{
				this._offset += this.offsetFixes2[(int)p.shadowFix - 1];
			}
			if (meshPass == this.pass.subPass)
			{
				meshBatch.colors[meshPass.idx] = p.color - 1572864f;
				meshBatch.matrices[meshPass.idx].SetTRS(p.NewVector3 + this._offset + SubPassData.Current.offset, SubPassData.Current.rotation, SubPassData.Current.scale);
			}
			else
			{
				meshBatch.colors[meshPass.idx] = p.color;
				meshBatch.matrices[meshPass.idx].m03 = p.x + this._offset.x * (float)num;
				meshBatch.matrices[meshPass.idx].m13 = p.y + this._offset.y;
				meshBatch.matrices[meshPass.idx].m23 = p.z + this._offset.z;
			}
			meshBatch.tiles[meshPass.idx] = p.tile + (float)(p.liquidLv * 10000 * num);
			meshBatch.matColors[meshPass.idx] = p.matColor;
			meshPass.idx++;
			if (meshPass.idx == meshPass.batchSize)
			{
				meshPass.NextBatch();
			}
			if (p.snow && this.hasSnowPass && meshPass == this.pass)
			{
				meshPass = this.pass.snowPass;
				meshBatch = meshPass.batches[meshPass.batchIdx];
				meshBatch.colors[meshPass.idx] = p.color;
				meshBatch.matrices[meshPass.idx].m03 = p.x + this._offset.x * (float)num;
				meshBatch.matrices[meshPass.idx].m13 = p.y + this._offset.y;
				meshBatch.matrices[meshPass.idx].m23 = p.z + this._offset.z - 0.01f;
				meshBatch.tiles[meshPass.idx] = p.tile + (float)(p.liquidLv * 10000 * num);
				meshBatch.matColors[meshPass.idx] = 104025f;
				meshPass.idx++;
				if (meshPass.idx == meshPass.batchSize)
				{
					meshPass.NextBatch();
				}
			}
		}

		public Vector3[] offsets;

		public Vector3[] offsetFixes;

		public Vector3[] offsetFixes2;
	}
}
