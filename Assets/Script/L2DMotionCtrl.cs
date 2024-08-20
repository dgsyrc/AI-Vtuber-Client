using Live2D.Cubism.Framework.Motion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L2DMotionCtrl : MonoBehaviour
{
    // Start is called before the first frame update
    public AnimationClip[] animClip;

    private CubismMotionController motion;
    private Dictionary<string, AnimationClip> clip = new Dictionary<string, AnimationClip>();
    public virtual void Start()
    {
        motion = GetComponent<CubismMotionController>();
        AddMotion();
    }

    public virtual void PlayMotion(string name,bool isLoop,int priority)
    {

    }
    public CubismMotionController GetMotion
    {
        get
        {
            return motion;
        }
    }

    public Dictionary<string, AnimationClip> GetClip
    {
        get
        {
            return clip;
        }
    }

    private void AddMotion()
    {
        for(int i = 0; i < animClip.Length; i++)
        {
            clip.Add(animClip[i].name, animClip[i]);
        }
    }
    // Update is called once per frame
}
