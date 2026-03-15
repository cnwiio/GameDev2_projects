using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro; // สำคัญ: ต้องเพิ่มเพื่อให้ใช้งาน TextMeshPro ได้

public class ClickableText : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("ใส่คำสั่งเมื่อคลิกซ้าย (เพิ่มค่า)")]
    public UnityEvent onLeftClick;

    [Tooltip("ใส่คำสั่งเมื่อคลิกขวา (ลดค่า)")]
    public UnityEvent onRightClick;

    // ตัวแปรสำหรับเก็บค่าเริ่มต้นของ Text
    private TextMeshProUGUI TMPtext;
    private Vector3 intialScale;
    private Color intialColor;

    #region Hover Animation Settings
    [Header("LeanTween Settings")]
    [SerializeField] float scale = 1.1f; // แนะนำให้ปรับเป็น 1.1 หรือ 1.2 เพื่อไม่ให้ใหญ่เกินไปจนล้นขอบครับ
    [SerializeField] float duration = 0.15f; // ลดเวลาลงนิดหน่อยเพื่อให้ปุ่มดูตอบสนองไวขึ้น
    [SerializeField] LeanTweenType easeType = LeanTweenType.easeOutBack; // ใช้ easeOutBack จะทำให้มันเด้งดึ๋งนิดๆ ครับ
    #endregion

    private void Awake()
    {
        // ดึงคอมโพเนนต์ TextMeshProUGUI มาเก็บไว้
        TMPtext = GetComponent<TextMeshProUGUI>();

        // บันทึกค่าขนาดและสีเริ่มต้น เอาไว้ใช้ตอนรีเซ็ต
        if (TMPtext != null)
        {
            intialScale = TMPtext.transform.localScale;
            intialColor = TMPtext.color;
        }
    }

    // ฟังก์ชันนี้จะทำงานตอนคลิกเมาส์
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            onLeftClick.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Right)
            onRightClick.Invoke();
        SoundManager.Instance.PlaySFX("Click");
    }

    // ฟังก์ชันนี้จะทำงานเมื่อ "นำเมาส์ไปชี้"
    public void OnPointerEnter(PointerEventData eventData)
    {
        UpScale();
        ChangeColor();
        SoundManager.Instance.PlaySFX("Hover");
    }

    // ฟังก์ชันนี้จะทำงานเมื่อ "นำเมาส์ออก"
    public void OnPointerExit(PointerEventData eventData)
    {
        DownScale();
        ResetColor();
    }

    #region Hover Animation Methods (จากโค้ดของคุณ)
    private void UpScale()
    {
        // ใช้ Vector3 เพื่อให้ขยายทั้งแกน X และ Y
        LeanTween.scale(TMPtext.gameObject, new Vector3(scale, scale, 1f), duration).setEase(easeType);
    }

    private void DownScale()
    {
        LeanTween.cancel(TMPtext.gameObject); // หยุดอนิเมชันเก่าก่อน
        TMPtext.transform.localScale = intialScale;
    }
    #endregion

    #region Color Change Methods (จากโค้ดของคุณ)
    private void ChangeColor()
    {
        TMPtext.color = Color.yellow;
    }

    private void ResetColor()
    {
        TMPtext.color = intialColor;
    }
    #endregion
}