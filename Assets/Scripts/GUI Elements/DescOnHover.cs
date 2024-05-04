using System.Collections;
using UnityEngine;

public class DescOnHover : MonoBehaviour
{
    [SerializeField] GameObject DescriptionBox;

    private void OnMouseOver()
    {
        DescriptionBox.SetActive(true);
    }

    private void OnMouseExit() {
        StartCoroutine(HideDesc());
    }

    private IEnumerator HideDesc() {
        DescriptionBox.GetComponent<Animator>().SetTrigger("Leave");
        yield return new WaitForSeconds(0.3f);
        DescriptionBox.SetActive(false);
    }
}
