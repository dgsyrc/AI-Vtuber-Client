using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Framework.Motion;
using System.IO;
using System;


public class Live2DMotionCtrl : MonoBehaviour
{

    private Animator ani;
    private int motion_num;
    private DateTime startTime;
    public int maxMotion = 3;
    private int timeFlag = 0;
    // Start is called before the first frame update
    public void Start()
    {
        ani = this.GetComponent<Animator>();
        this.motion_num = 0;
        startTime = DateTime.Now;
    }


    void Update()
    {
        DateTime curtimer = DateTime.Now;
        int timer = GetSubSeconds(startTime, curtimer);
        Debug.Log(timer);
        if(timer % 10 == 0)
        {
            timeFlag++;
        }
        else
        {
            timeFlag = 0;
        }
        //if (Input.GetMouseButtonDown(0))//�����
        if(timeFlag == 1 && ani.GetInteger("motion_num") == 0)//ʱ�����Զ�����
        {
            motion_num += 1;
            if (motion_num > maxMotion) motion_num = 1;
            ani.SetInteger("motion_num", motion_num);
        }


    }

    public void Motion_num_setzero()
    {
        ani.SetInteger("motion_num", 0);
    }

    public int GetSubSeconds(DateTime startTimer, DateTime endTimer)
    {
        TimeSpan startSpan = new TimeSpan(startTimer.Ticks);

        TimeSpan nowSpan = new TimeSpan(endTimer.Ticks);

        TimeSpan subTimer = nowSpan.Subtract(startSpan).Duration();

        //���ؼ�������������ķ��Ӻ�Сʱ�ȣ�������������֮��Ĳ
        return subTimer.Seconds;

        //�������ʱ�������Ϸ֡�ʱ�Ĳ�ֵ������������������
        //return (int)subTimer.TotalSeconds;
    }
    

}
