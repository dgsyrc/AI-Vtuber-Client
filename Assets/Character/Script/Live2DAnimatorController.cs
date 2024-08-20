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
        //animator.Play("hiyori_m01");
        // ���ſ��ж���
        PlayIdleAnimation();

        // ��ʼ������Ŷ�����Э��
        StartCoroutine(PlayRandomAnimations());

        // Ϊ�������ύ�¼���Ӽ�����
        inputField.onValueChanged.AddListener(OnEndEdit);

    }

    // ���ſ��ж���
    private void PlayIdleAnimation()
    {
        animator.Play("empty"); // ���ж�����״̬����Ϊ"empty"
    }

    // ����΢Ц����
    public void PlaySmileAnimation()
    {
        
        animator.Play("hiyori_m05"); // ΢Ц������״̬����
    }

    // �����ɻ󶯻�
    public void PlayDoubtAnimation()
    {
        
        animator.Play("hiyori_m07"); // �ɻ󶯻���״̬����
    }

    // ������Ŷ���
    private IEnumerator PlayRandomAnimations()
    {
        string[] randomAnimations = { "hiyori_m01", "hiyori_m02", "hiyori_m03" }; // ���������״̬����

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
    public void OnEndEdit(string inputText)
    {
        // ��ӡ������ı����������������
        Debug.LogError("Input Field Text: " + inputText);

        // ���������ı�������Ӧ�Ķ���
        if (inputText.Contains("(smile)"))
        {
            PlaySmileAnimation();
        }
        else if (inputText.Contains("(doubt)"))
        {
            PlayDoubtAnimation();
        }

        // �����������ݣ������Ҫ��
        //inputField.text = "";
    }
}
