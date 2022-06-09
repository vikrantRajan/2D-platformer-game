using System.Collections;
using UnityEngine;
using System;

public class CharacterAnimationSFX : MonoBehaviour
{
    [Serializable]
    public struct AudioTracks
    {
        public string name;
        public AudioSource sfx;
        public float delay_time;

    }
    public AudioTracks[] sfx;
    private bool change = false;
    private float cur_delay_time;
    private AudioSource cur;
    private string currentName;
    Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        var currentAnimation = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        foreach (AudioTracks x in sfx)
        {
            if (currentAnimation == x.name && !change)
            {
                cur_delay_time = x.delay_time;
                //Debug.Log("Play audio" + x.name);
                cur = x.sfx;
                if (x.delay_time > 0) StartCoroutine(delayPlaySound());
                else cur.Play();
                currentName = x.name;
                change = true;
            }

        }
        if (currentAnimation != currentName & change)
        {
            //Debug.Log("Stop Audio");
            cur.Stop();
            //cur.Pause();
            change = false;
        }

    }

    IEnumerator delayPlaySound()
    {
        yield return new WaitForSeconds(cur_delay_time);
        cur.Play();
    }

}
