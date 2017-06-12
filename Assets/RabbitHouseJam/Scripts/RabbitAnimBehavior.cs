using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitAnimBehavior : MonoBehaviour {

    public Animation anim;
    public bool constantLoopAnim = true;
    private float timer = 0;


	// Update is called once per frame
	void Update () {

        if (!anim.isPlaying)
        {
            anim = GetComponent<Animation>();
            anim.Play(anim.clip.name);
            timer = 0.2f;
        }
        timer -= Time.deltaTime;
	}

}
