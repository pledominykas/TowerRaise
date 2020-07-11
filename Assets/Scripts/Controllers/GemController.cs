using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GemController : MonoBehaviour
{
    private int Gems = 0;
    [SerializeField] Image GemImage;
    [SerializeField] TextMeshProUGUI GemText;
    [SerializeField] GameObject GemPickUpImagePrefab;
    [SerializeField] Animator GemAnimator;

    private void Start()
    {
        GemText.text = Gems.ToString();
    }

    private IEnumerator GemPickUpAnimation(Vector2 _gemScreenPos)
    {
        RectTransform gem = Instantiate(GemPickUpImagePrefab, transform).GetComponent<RectTransform>();
        gem.position = _gemScreenPos;
        Vector3 targetPos = GemImage.GetComponent<RectTransform>().position;
        Vector2 posVel = Vector2.zero;
        for (float i = 0f; i < Mathf.Infinity; i += Time.deltaTime)
        {
            gem.position = Vector2.SmoothDamp(gem.position, targetPos, ref posVel, 0.3f);
            if (Vector2.SqrMagnitude(gem.position - targetPos) <= 0.4f)
            {
                break;
            }
            yield return null;
        }
        Destroy(gem.gameObject);
        GemAnimator.Play("OnGemAdd");
        GemText.text = Gems.ToString();
    }

    private IEnumerator ClaimGemsAnimation(int _gemCount)
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < _gemCount; i++)
        {
            Gems += 1;
            GemAnimator.Play("OnGemAdd");
            GemText.text = Gems.ToString();
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void OnGemAdd(Vector2 _gemScreenPos)
    {
        Gems+=1;
        StartCoroutine(GemPickUpAnimation(_gemScreenPos));
    }

    public int GetGemCount() { return Gems; }
    public void RemoveGems(int _gems) { Gems -= _gems; GemText.text = Gems.ToString(); }
    public void ClaimGems(int _gemCount) { StartCoroutine(ClaimGemsAnimation(_gemCount)); }
    public void SetGemCount(int _count) { Gems = _count; }
}
