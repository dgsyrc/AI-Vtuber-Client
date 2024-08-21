using UnityEngine;
using TMPro;
using System.Collections;

public class Live2DAnimatorController : MonoBehaviour
{
    public Animator animator; // Animator���
    public TMP_InputField inputField; // TMP_InputField���
    public float randomAnimationInterval = 5f; // ����������ż��ʱ��
    private void Start()
    {
        
        // ���ſ��ж���
        PlayIdleAnimation();

        // ��ʼ������Ŷ�����Э��
        StartCoroutine(PlayRandomAnimations());

        // Ϊ�������ύ�¼���Ӽ�����
        //inputField.onEndEdit.AddListener(OnEndEdit);
    }

    // ���ſ��ж���
    private void PlayIdleAnimation()
    {
        animator.Play("empty"); // ���ж�����״̬����Ϊ"empty"
    }

    // ����΢Ц����
    public void PlayAngerAnimation()
    {
        
        animator.Play("Anger"); // Anger������״̬����
    }

    // �����ɻ󶯻�
    public void PlayAstonishAnimation()
    {
        
        animator.Play("Astonish"); // Astonish������״̬����
    }

    public void PlayMadAnimation()
    {

        animator.Play("Mad"); // Mad������״̬����
    }

    public void PlayNodAnimation()
    {

        animator.Play("Nod Head"); // Nod Head������״̬����
    }

    public void PlaySadAnimation()
    {

        animator.Play("Sad"); // Sad������״̬����
    }

    public void PlayShakeAnimation()
    {

        animator.Play("Shake Head"); // Shake Head������״̬����
    }

    public void PlayShyAnimation()
    {

        animator.Play("Shy"); // Shy������״̬����
    }

    public void PlaySmileAnimation()
    {

        animator.Play("Smile"); // Smile������״̬����
    }

    public void PlaySurpriseAnimation()
    {

        animator.Play("Surprise"); // Surprise������״̬����
    }

    // ������Ŷ���
    private IEnumerator PlayRandomAnimations()
    {
        string[] randomAnimations = { "Idle1", "Idle2", "Idle3" }; // ���������״̬����

        while (true)
        {
            // ÿ��һ��ʱ�䲥��һ���������
            yield return new WaitForSeconds(randomAnimationInterval);

            if (IsInIdleState())
            {
                int randomIndex = Random.Range(0, randomAnimations.Length);
                string selectedAnimation = randomAnimations[randomIndex];
                animator.Play(selectedAnimation);
            }
        }
    }

    // ����Ƿ��ڿ���״̬
    private bool IsInIdleState()
    {
        // ��ȡ��ǰ״̬��AnimatorStateInfo
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // �����Ƿ��ڿ���״̬
        return stateInfo.IsName("empty"); // ����״̬������Ϊ"empty"
    }

    // �ڶ����ؼ�֡���õ�״̬�ع鷽��
    public void OnAnimationComplete()
    {
        PlayIdleAnimation();
        StartCoroutine(PlayRandomAnimations()); // ȷ�����������������
    }

    // ��������ı��ύʱ����
    public void OnEndEdit()
    {
        // ��ӡ������ı����������������
        string inputText = inputField.text;
        Debug.LogError("Input Field Text: " + inputText);

        // ���������ı�������Ӧ�Ķ���
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

        // �����������ݣ������Ҫ��
        //inputField.text = "";
    }
}
