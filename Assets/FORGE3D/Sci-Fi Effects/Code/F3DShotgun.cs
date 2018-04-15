using UnityEngine;
using System.Collections.Generic;

namespace Forge3D
{
	public class F3DShotgun : F3DSprite
	{
		// Particle collision events
		private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>(16);
		// private List<ParticleCollisionEvent>
		// On particle collision
		void OnParticleCollision(GameObject other)
		{
			int numCollisionEvents = GetComponent<ParticleSystem>().GetCollisionEvents(other, collisionEvents);

			int i = 0;
			// Play collision sound and apply force to the rigidbody was hit 
			while (i < numCollisionEvents)
			{
				if (ImpactAction != null)
				{
					RaycastBullet bullet = new RaycastBullet() {
						point = collisionEvents[i].intersection,
						forward = collisionEvents[i].velocity.normalized,
						normal = collisionEvents[i].normal,
						transform = other.transform,
						rigidbody = other.GetComponent<Rigidbody>()
					};
					ImpactAction.Invoke(bullet);
					i++;
					continue;
				}

				F3DAudioController.instance.ShotGunHit(collisionEvents[i].intersection);

				if (other.GetComponent<Rigidbody>())
				{
					Vector3 pos = collisionEvents[i].intersection;
					Vector3 force = collisionEvents[i].velocity.normalized * 50f;

					other.GetComponent<Rigidbody>().AddForceAtPosition(force, pos);
				}

				i++;
			}
		}
	}
}