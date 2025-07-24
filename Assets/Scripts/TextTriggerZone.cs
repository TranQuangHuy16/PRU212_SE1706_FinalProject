using System.Collections;
using UnityEngine;
using TMPro;

public class TextTriggerZone : MonoBehaviour
{
    [Header("Text Settings")]
    public TextMeshProUGUI textComponent;        // Text nằm trong Canvas
    [TextArea] public string fullText;
    public float typingSpeed = 0.05f;
    public float displayDuration = 10f;          // Thời gian hiển thị sau khi đánh máy xong

    [Header("Trigger Settings")]
    public bool triggerOnce = true;
    private bool hasTriggered = false;

    private void Start()
    {
        textComponent.text = "";
        textComponent.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerOnce && hasTriggered) return;

            hasTriggered = true;
            StartCoroutine(PlayText());
        }
    }

    private IEnumerator PlayText()
    {
        textComponent.text = "";
        textComponent.gameObject.SetActive(true);

        // Hiện từng chữ
        foreach (char c in fullText)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Giữ văn bản trong 10s
        yield return new WaitForSeconds(displayDuration);

        // Ẩn đi
        textComponent.gameObject.SetActive(false);
    }
}
