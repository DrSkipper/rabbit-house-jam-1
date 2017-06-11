using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitAnimBehavior : MonoBehaviour {

    public Animation anim;
    private float timer = 0;

    IEnumerator Start()
    {
        anim = GetComponent<Animation>();
        anim.Play(anim.clip.name);
        yield return new WaitForSeconds(anim.clip.length);
        
    }

	// Update is called once per frame
	void Update () {
        if (!anim.isPlaying && timer <= 0 )
        {
            anim = GetComponent<Animation>();
            anim.Play(anim.clip.name);
            timer = 10;
        }
        timer -= Time.deltaTime;
	}

}
