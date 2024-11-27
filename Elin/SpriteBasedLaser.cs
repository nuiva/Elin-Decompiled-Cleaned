using System;
using DG.Tweening;
using UnityEngine;

public class SpriteBasedLaser : MonoBehaviour
{
	public event SpriteBasedLaser.LaserHitTriggerHandler OnLaserHitTriggered;

	public void Play(Vector3 to)
	{
		base.transform.position = base.transform.position.SetZ(0f);
		Vector3 vector = to.SetZ(0f) + this.fix + new Vector3(0.2f, 0.2f, 0f).Random();
		this.dest = vector;
		this.dest2 = vector;
		this.hitSparkEmission = this.hitSparkParticleSystem.emission;
		this.gameObjectCached = base.gameObject;
		if (this.laserLineRendererArc != null)
		{
			this.laserLineRendererArc.SetVertexCount(this.laserArcSegments);
		}
		this.startLaserLength = this.maxLaserLength;
		if (this.laserOscillationPositionerScript != null)
		{
			this.laserOscillationPositionerScript.radius = this.oscillationThreshold;
		}
		this.Update();
		this.Update();
		this.trail.localScale = this.middleGoPiece.transform.localScale;
		this.trail.rotation = this.middleGoPiece.transform.rotation;
		this.trail.position = this.middleGoPiece.transform.position;
		DOTweenAnimation[] array = this.anime;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].DOPlay();
		}
	}

	private void OscillateLaserParts(float currentLaserDistance)
	{
		if (this.laserOscillationPositionerScript == null)
		{
			return;
		}
		this.lerpYValue = Mathf.Lerp(this.middleGoPiece.transform.localPosition.y, this.laserOscillationPositionerScript.randomPointInCircle.y, Time.deltaTime * this.oscillationSpeed);
		if (this.startGoPiece != null && this.middleGoPiece != null)
		{
			Vector2 b = new Vector2(this.startGoPiece.transform.localPosition.x, this.laserOscillationPositionerScript.randomPointInCircle.y);
			Vector2 v = Vector2.Lerp(this.startGoPiece.transform.localPosition, b, Time.deltaTime * this.oscillationSpeed);
			this.startGoPiece.transform.localPosition = v;
			Vector2 v2 = new Vector2(currentLaserDistance / 2f + this.startSpriteWidth / 4f, this.lerpYValue);
			this.middleGoPiece.transform.localPosition = v2;
		}
		if (this.endGoPiece != null)
		{
			Vector2 v3 = new Vector2(currentLaserDistance + this.startSpriteWidth / 2f, this.lerpYValue);
			this.endGoPiece.transform.localPosition = v3;
		}
	}

	private void SetLaserArcVertices(float distancePoint, bool useHitPoint)
	{
		for (int i = 1; i < this.laserArcSegments; i++)
		{
			float y = Mathf.Clamp(Mathf.Sin((float)i + Time.time * UnityEngine.Random.Range(0.5f, 1.3f)), -this.laserArcMax + UnityEngine.Random.RandomRange(0f, this.laserArcMax * 0.5f), this.laserArcMax + UnityEngine.Random.RandomRange(0f, this.laserArcMax * 0.5f));
			Vector2 v = new Vector2((float)i * 1.2f * this.arcNum2, y);
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

	private void Update()
	{
		if (this.gameObjectCached != null)
		{
			this.middleGoPiece.transform.localScale = new Vector3(this.maxLaserLength - this.startSpriteWidth + 0.2f, this.middleGoPiece.transform.localScale.y, this.middleGoPiece.transform.localScale.z);
			if (this.oscillateLaser)
			{
				this.OscillateLaserParts(this.maxLaserLength);
			}
			else
			{
				if (this.middleGoPiece != null)
				{
					this.middleGoPiece.transform.localPosition = new Vector2(this.maxLaserLength / 2f + this.startSpriteWidth / 4f, this.lerpYValue);
				}
				if (this.endGoPiece != null)
				{
					this.endGoPiece.transform.localPosition = new Vector3(this.maxLaserLength + this.startSpriteWidth / 2f, 0f, 0f) + this.fixTip;
				}
			}
			if (this.laserRotationEnabled)
			{
				Vector3 vector = this.dest2 - this.gameObjectCached.transform.position;
				this.laserAngle = Mathf.Atan2(vector.y, vector.x);
				if (this.laserAngle < 0f)
				{
					this.laserAngle = 6.2831855f + this.laserAngle;
				}
				float angle = this.laserAngle * 57.29578f;
				if (this.lerpLaserRotation)
				{
					base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.AngleAxis(angle, base.transform.forward), Time.deltaTime * this.turningRate);
				}
				else
				{
					base.transform.rotation = Quaternion.AngleAxis(angle, base.transform.forward);
				}
			}
			this.maxLaserLength = Vector2.Distance(this.dest, base.transform.position) + this.startSpriteWidth / 4f + this.fix2.x;
			if (this.hitSparkParticleSystem != null)
			{
				this.hitSparkParticleSystem.transform.position = this.dest;
				this.hitSparkEmission.enabled = true;
			}
			if (this.useArc)
			{
				if (!this.laserLineRendererArc.enabled)
				{
					this.laserLineRendererArc.enabled = true;
				}
				this.SetLaserArcVertices(this.maxLaserLength, true);
				this.SetLaserArcSegmentLength();
				return;
			}
			if (this.laserLineRendererArc.enabled)
			{
				this.laserLineRendererArc.enabled = false;
			}
		}
	}

	private void SetLaserArcSegmentLength()
	{
		int vertexCount = Mathf.Abs((int)this.maxLaserLength) * (int)this.arcNum;
		this.laserLineRendererArc.SetVertexCount(vertexCount);
		this.laserArcSegments = vertexCount;
	}

	public Material mat;

	public Vector3 dest;

	public Vector3 fixTip;

	public Vector3 dest2;

	public Vector3 fix;

	public Vector3 fix2;

	public float arcNum;

	public float arcNum2;

	public DOTweenAnimation[] anime;

	public LineRenderer laserLineRendererArc;

	public int laserArcSegments = 20;

	public RandomPositionMover laserOscillationPositionerScript;

	public bool oscillateLaser;

	public float maxLaserLength = 20f;

	public float oscillationSpeed = 1f;

	public GameObject targetGo;

	public ParticleSystem hitSparkParticleSystem;

	public float laserArcMax;

	public float maxLaserRaycastDistance;

	public bool laserRotationEnabled;

	public bool lerpLaserRotation;

	public float turningRate = 3f;

	public float collisionTriggerInterval = 0.25f;

	public LayerMask mask;

	public bool useArc;

	public float oscillationThreshold = 0.2f;

	private GameObject gameObjectCached;

	private float laserAngle;

	private float lerpYValue;

	private float startLaserLength;

	public Transform trail;

	public GameObject startGoPiece;

	public GameObject middleGoPiece;

	public GameObject endGoPiece;

	private float startSpriteWidth;

	private bool waitingForTriggerTime;

	private ParticleSystem.EmissionModule hitSparkEmission;

	public delegate void LaserHitTriggerHandler(RaycastHit2D hitInfo);
}
