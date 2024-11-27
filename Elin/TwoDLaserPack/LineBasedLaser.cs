using System;
using System.Collections;
using UnityEngine;

namespace TwoDLaserPack
{
	public class LineBasedLaser : MonoBehaviour
	{
		public event LineBasedLaser.LaserHitTriggerHandler OnLaserHitTriggered;

		private void Start()
		{
			this.startLaserTextureXScale = this.laserLineRenderer.material.mainTextureScale.x;
			this.startLaserSegmentLength = this.laserArcSegments;
			this.laserLineRenderer.sortingLayerName = this.sortLayer;
			this.laserLineRenderer.sortingOrder = this.sortOrder;
			this.laserLineRendererArc.sortingLayerName = this.sortLayer;
			this.laserLineRendererArc.sortingOrder = this.sortOrder;
		}

		private void Awake()
		{
			this.hitSparkEmission = this.hitSparkParticleSystem.emission;
		}

		private void OnEnable()
		{
			this.gameObjectCached = base.gameObject;
			if (this.laserLineRendererArc != null)
			{
				this.laserLineRendererArc.SetVertexCount(this.laserArcSegments);
			}
		}

		private void OnDisable()
		{
		}

		private void Update()
		{
			if (this.gameObjectCached != null && this.laserActive)
			{
				this.laserLineRenderer.material.mainTextureOffset = new Vector2(this.laserTextureOffset, 0f);
				this.laserTextureOffset -= Time.deltaTime * this.laserTexOffsetSpeed;
				RaycastHit2D raycastHit2D;
				if (this.laserRotationEnabled && this.targetGo != null)
				{
					Vector3 vector = this.targetGo.transform.position - this.gameObjectCached.transform.position;
					this.laserAngle = Mathf.Atan2(vector.y, vector.x);
					if (this.laserAngle < 0f)
					{
						this.laserAngle = 6.2831855f + this.laserAngle;
					}
					float angle = this.laserAngle * 57.29578f;
					if (this.lerpLaserRotation)
					{
						base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.AngleAxis(angle, base.transform.forward), Time.deltaTime * this.turningRate);
						Vector3 v = base.transform.rotation * Vector3.right;
						raycastHit2D = Physics2D.Raycast(base.transform.position, v, this.maxLaserRaycastDistance, this.mask);
					}
					else
					{
						base.transform.rotation = Quaternion.AngleAxis(angle, base.transform.forward);
						raycastHit2D = Physics2D.Raycast(base.transform.position, vector, this.maxLaserRaycastDistance, this.mask);
					}
				}
				else
				{
					raycastHit2D = Physics2D.Raycast(base.transform.position, base.transform.right, this.maxLaserRaycastDistance, this.mask);
				}
				if (!this.ignoreCollisions)
				{
					if (!(raycastHit2D.collider != null))
					{
						this.SetLaserToDefaultLength();
						return;
					}
					this.SetLaserEndToTargetLocation(raycastHit2D);
					if (!this.waitingForTriggerTime)
					{
						base.StartCoroutine(this.HitTrigger(this.collisionTriggerInterval, raycastHit2D));
						return;
					}
				}
				else
				{
					this.SetLaserToDefaultLength();
				}
			}
		}

		private IEnumerator HitTrigger(float triggerInterval, RaycastHit2D hit)
		{
			this.waitingForTriggerTime = true;
			if (this.OnLaserHitTriggered != null)
			{
				this.OnLaserHitTriggered(hit);
			}
			yield return new WaitForSeconds(triggerInterval);
			this.waitingForTriggerTime = false;
			yield break;
		}

		public void SetLaserState(bool enabledStatus)
		{
			this.laserActive = enabledStatus;
			this.laserLineRenderer.enabled = enabledStatus;
			if (this.laserLineRendererArc != null)
			{
				this.laserLineRendererArc.enabled = enabledStatus;
			}
			if (this.hitSparkParticleSystem != null)
			{
				this.hitSparkEmission.enabled = enabledStatus;
			}
		}

		private void SetLaserEndToTargetLocation(RaycastHit2D hit)
		{
			float num = Vector2.Distance(hit.point, this.laserLineRenderer.transform.position);
			this.laserLineRenderer.SetPosition(1, new Vector2(num, 0f));
			this.laserTextureXScale = this.startLaserTextureXScale * num;
			this.laserLineRenderer.material.mainTextureScale = new Vector2(this.laserTextureXScale, 1f);
			if (this.useArc)
			{
				if (!this.laserLineRendererArc.enabled)
				{
					this.laserLineRendererArc.enabled = true;
				}
				int vertexCount = Mathf.Abs((int)num);
				this.laserLineRendererArc.SetVertexCount(vertexCount);
				this.laserArcSegments = vertexCount;
				this.SetLaserArcVertices(num, true);
			}
			else if (this.laserLineRendererArc.enabled)
			{
				this.laserLineRendererArc.enabled = false;
			}
			if (this.hitSparkParticleSystem != null)
			{
				this.hitSparkParticleSystem.transform.position = hit.point;
				this.hitSparkEmission.enabled = true;
			}
		}

		private void SetLaserToDefaultLength()
		{
			this.laserLineRenderer.SetPosition(1, new Vector2((float)this.laserArcSegments, 0f));
			this.laserTextureXScale = this.startLaserTextureXScale * (float)this.laserArcSegments;
			this.laserLineRenderer.material.mainTextureScale = new Vector2(this.laserTextureXScale, 1f);
			if (this.useArc)
			{
				if (!this.laserLineRendererArc.enabled)
				{
					this.laserLineRendererArc.enabled = true;
				}
				this.laserLineRendererArc.SetVertexCount(this.startLaserSegmentLength);
				this.laserArcSegments = this.startLaserSegmentLength;
				this.SetLaserArcVertices(0f, false);
			}
			else
			{
				if (this.laserLineRendererArc.enabled)
				{
					this.laserLineRendererArc.enabled = false;
				}
				this.laserLineRendererArc.SetVertexCount(this.startLaserSegmentLength);
				this.laserArcSegments = this.startLaserSegmentLength;
			}
			if (this.hitSparkParticleSystem != null)
			{
				this.hitSparkEmission.enabled = false;
				this.hitSparkParticleSystem.transform.position = new Vector2((float)this.laserArcSegments, base.transform.position.y);
			}
		}

		private void SetLaserArcVertices(float distancePoint, bool useHitPoint)
		{
			for (int i = 1; i < this.laserArcSegments; i++)
			{
				float y = Mathf.Clamp(Mathf.Sin((float)i + Time.time * UnityEngine.Random.Range(0.5f, 1.3f)), this.laserArcMaxYDown, this.laserArcMaxYUp);
				Vector2 v = new Vector2((float)i * 1.2f, y);
				if (useHitPoint && i == this.laserArcSegments - 1)
				{
					this.laserLineRendererArc.SetPosition(i, new Vector2(distancePoint, 0f));
				}
				else
				{
					this.laserLineRendererArc.SetPosition(i, v);
				}
			}
		}

		public LineRenderer laserLineRendererArc;

		public LineRenderer laserLineRenderer;

		public int laserArcSegments = 20;

		public bool laserActive;

		public bool ignoreCollisions;

		public GameObject targetGo;

		public float laserTexOffsetSpeed = 1f;

		public ParticleSystem hitSparkParticleSystem;

		public float laserArcMaxYDown;

		public float laserArcMaxYUp;

		public float maxLaserRaycastDistance = 20f;

		public bool laserRotationEnabled;

		public bool lerpLaserRotation;

		public float turningRate = 3f;

		public float collisionTriggerInterval = 0.25f;

		public LayerMask mask;

		public string sortLayer = "Default";

		public int sortOrder;

		public bool useArc;

		private GameObject gameObjectCached;

		private float laserAngle;

		private float laserTextureOffset;

		private float laserTextureXScale;

		private float startLaserTextureXScale;

		private int startLaserSegmentLength;

		private bool waitingForTriggerTime;

		private ParticleSystem.EmissionModule hitSparkEmission;

		public delegate void LaserHitTriggerHandler(RaycastHit2D hitInfo);
	}
}
