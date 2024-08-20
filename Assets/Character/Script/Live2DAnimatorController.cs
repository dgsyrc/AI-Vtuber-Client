using UnityEngine;
using TMPro;
using System.Collections;

public class Live2DAnimatorController : MonoBehaviour
{
    public Animator animator; // Animator组件
    public TMP_InputField inputField; // TMP_InputField组件
    public float randomAnimationInterval = 5f; // 随机动画播放间隔时间
    private void Start()
    {
        //animator.Play("hiyori_m01");
        // 播放空闲动画
        PlayIdleAnimation();

        // 开始随机播放动画的协程
        StartCoroutine(PlayRandomAnimations());

        // 为输入框的提交事件添加监听器
        inputField.onValueChanged.AddListener(OnEndEdit);

    }

    // 播放空闲动画
    private void PlayIdleAnimation()
    {
        animator.Play("empty"); // 空闲动画的状态名称为"empty"
    }

    // 播放微笑动画
    public void PlaySmileAnimation()
    {
        
        animator.Play("hiyori_m05"); // 微笑动画的状态名称
    }

    // 播放疑惑动画
    public void PlayDoubtAnimation()
    {
        
        animator.Play("hiyori_m07"); // 疑惑动画的状态名称
    }

    // 随机播放动画
    private IEnumerator PlayRandomAnimations()
    {
        string[] randomAnimations = { "hiyori_m01", "hiyori_m02", "hiyori_m03" }; // 随机动画的状态名称

        while (true)
        {
            // 每隔一段时间播放一个随机动画
            yield return new WaitForSeconds(randomAnimationInterval);

            if (IsInIdleState())
            {
                int randomIndex = Random.Range(0, randomAnimations.Length);
                string selectedAnimation = randomAnimations[randomIndex];
                animator.Play(selectedAnimation);
            }
        }
    }

    // 检查是否处于空闲状态
    private bool IsInIdleState()
    {
        // 获取当前状态的AnimatorStateInfo
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 返回是否处于空闲状态
        return stateInfo.IsName("empty"); // 空闲状态的名称为"empty"
    }

    // 在动画关键帧调用的状态回归方法
    public void OnAnimationComplete()
    {
        PlayIdleAnimation();
        StartCoroutine(PlayRandomAnimations()); // 确保继续播放随机动画
    }

    // 当输入框文本提交时调用
    public void OnEndEdit(string inputText)
    {
        // 打印输入的文本（或进行其他处理）
        Debug.LogError("Input Field Text: " + inputText);

        // 根据输入文本触发相应的动画
        if (inputText.Contains("(smile)"))
        {
            PlaySmileAnimation();
        }
        else if (inputText.Contains("(doubt)"))
        {
            PlayDoubtAnimation();
        }

        // 清空输入框内容（如果需要）
        //inputField.text = "";
    }
}
