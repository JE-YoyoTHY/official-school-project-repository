using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCommonScript : MonoBehaviour
{
    private ParticleSystem movingParticle;
	[SerializeField] private ParticleSystem ParticleSystem;
	private ParticleSystem ParticleInstance;

    public void emitParticle(){
        //particle
		ParticleInstance = Instantiate(ParticleSystem, transform.position, Quaternion.identity);
    }
}
