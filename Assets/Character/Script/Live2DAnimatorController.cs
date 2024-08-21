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
        
        // 播放空闲动画
        PlayIdleAnimation();

        // 开始随机播放动画的协程
        StartCoroutine(PlayRandomAnimations());

        // 为输入框的提交事件添加监听器
        //inputField.onEndEdit.AddListener(OnEndEdit);
    }

    // 播放空闲动画
    private void PlayIdleAnimation()
    {
        animator.Play("empty"); // 空闲动画的状态名称为"empty"
    }

    // 播放微笑动画
    public void PlayAngerAnimation()
    {
        
        animator.Play("Anger"); // Anger动画的状态名称
    }

    // 播放疑惑动画
    public void PlayAstonishAnimation()
    {
        
        animator.Play("Astonish"); // Astonish动画的状态名称
    }

    public void PlayMadAnimation()
    {

        animator.Play("Mad"); // Mad动画的状态名称
    }

    public void PlayNodAnimation()
    {

        animator.Play("Nod Head"); // Nod Head动画的状态名称
    }

    public void PlaySadAnimation()
    {

        animator.Play("Sad"); // Sad动画的状态名称
    }

    public void PlayShakeAnimation()
    {

        animator.Play("Shake Head"); // Shake Head动画的状态名称
    }

    public void PlayShyAnimation()
    {

        animator.Play("Shy"); // Shy动画的状态名称
    }

    public void PlaySmileAnimation()
    {

        animator.Play("Smile"); // Smile动画的状态名称
    }

    public void PlaySurpriseAnimation()
    {

        animator.Play("Surprise"); // Surprise动画的状态名称
    }

    // 随机播放动画
    private IEnumerator PlayRandomAnimations()
    {
        string[] randomAnimations = { "Idle1", "Idle2", "Idle3" }; // 随机动画的状态名称

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
    public void OnEndEdit()
    {
        // 打印输入的文本（或进行其他处理）
        string inputText = inputField.text;
        Debug.LogError("Input Field Text: " + inputText);

        // 根据输入文本触发相应的动画
        if (inputText.Contains("(anger)"))
        {
            PlayAngerAnimation();
        }
        else if (inputText.Contains("(astonish)"))
        {
            PlayAstonishAnimation();
        }
        else if (inputText.Contains("(mad)"))
        {
            PlayMadAnimation();
        }
        else if (inputText.Contains("(nod)"))
        {
            PlayNodAnimation();
        }
        else if (inputText.Contains("(sad)"))
        {
            PlaySadAnimation();
        }
        else if (inputText.Contains("(shake)"))
        {
            PlayShakeAnimation();
        }
        else if (inputText.Contains("(shy)"))
        {
            PlayShyAnimation();
        }
        else if (inputText.Contains("(smile)"))
        {
            PlaySmileAnimation();
        }
        else if (inputText.Contains("(surprise)"))
        {
            PlaySurpriseAnimation();
        }

        // 清空输入框内容（如果需要）
        //inputField.text = "";
    }
}
