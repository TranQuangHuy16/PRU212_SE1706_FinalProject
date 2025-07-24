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
//using System.Collections;
//using UnityEngine;
//using TMPro;

//public class TextTriggerZone : MonoBehaviour
//{
//    [Header("Text Settings")]
//    public TextMeshProUGUI textComponent;        // Text nằm trong Canvas
//    [TextArea] public string fullText;
//    public float typingSpeed = 0.05f;
//    public float displayDuration = 10f;          // Thời gian hiển thị sau khi đánh máy xong

//    [Header("Trigger Settings")]
//    public bool triggerOnce = true;
//    private bool hasTriggered = false;

//    private void Start()
//    {
//        textComponent.text = "";
//        textComponent.gameObject.SetActive(false);
//    }

//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            if (triggerOnce && hasTriggered) return;

//            hasTriggered = true;

//            // Tìm và tắt script điều khiển nhân vật (giả định tên là "PlayerController")
//            MonoBehaviour playerController = other.GetComponent<QueenController>(); // ← bạn nên thay bằng tên cụ thể nếu có
//            if (playerController != null)
//                playerController.enabled = false;

//            StartCoroutine(PlayText(playerController));
//        }
//    }

//    private IEnumerator PlayText(MonoBehaviour playerController)
//    {
//        textComponent.text = "";
//        textComponent.gameObject.SetActive(true);

//        // Hiện từng chữ
//        foreach (char c in fullText)
//        {
//            textComponent.text += c;
//            yield return new WaitForSeconds(typingSpeed);
//        }


//        // Ẩn đi
//        textComponent.gameObject.SetActive(false);

//        // Mở lại điều khiển nhân vật nếu cần
//        if (playerController != null)
//            playerController.enabled = true;
//    }
//}
