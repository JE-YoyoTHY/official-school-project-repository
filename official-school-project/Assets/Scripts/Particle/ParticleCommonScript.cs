using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCommonScript : MonoBehaviour
{
    private ParticleSystem movingParticle;
	[SerializeField] private ParticleSystem ParticleSystem;
	private ParticleSystem ParticleInstance;

    [SerializeField] private ParticleSystem[] ParticleSystemWithIndex;

    public void emitParticle(){
        //particle
		ParticleInstance = Instantiate(ParticleSystem, transform.position, Quaternion.identity);
    }

    public void emitParticleWithIndex(int i)
    {
        ParticleInstance = Instantiate(ParticleSystemWithIndex[i], transform.position, Quaternion.identity);
    }
}
