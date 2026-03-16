using UnityEngine;

public class ExitScrit : MonoBehaviour
{
    [SerializeField] private GameObject Panel;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Panel.SetActive(true);
        }
    }
}
