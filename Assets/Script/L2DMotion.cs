using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L2DMotion : L2DMotionCtrl
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        PlayMotion("mtn_00", false, 1);
    }

    // Update is called once per frame
    void Update()
    {
        PlayAnim();
        if(!GetMotion.IsPlayingAnimation()) PlayMotion("mtn_00",false, 1);
    }

    public override void PlayMotion(string name, bool isLoop, int priority)
    {
        if(!GetClip.ContainsKey(name)) return;
        GetMotion.PlayAnimation(GetClip[name],isLoop:isLoop,priority:priority);
    }

    private void PlayAnim()
    {
        if(Input.GetMouseButtonDown(0))
        {
            PlayMotion("mtn_01",false,2);

        }
        if(Input.GetMouseButtonDown(1))
        {
            PlayMotion("mtn_02", false, 3);
        }
    }
}
