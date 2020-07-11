using UnityEngine;
using TMPro;

public class ClaimGems : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI GemCountText;
    [SerializeField] GameObject ClaimGemsButton;
    [SerializeField] GameObject GemExplosionPrefab;
    private int gemClaimCount = 10;
    
    public void SetGemCountText()
    {
        gemClaimCount = Mathf.Clamp(((GameController.GetCurrentLevel()-2) * 2 + 10), 10, 35);
        ClaimGemsButton.SetActive(true);
        GemCountText.text = "+" + gemClaimCount;
    }

    public void OnClaimGemsClick()
    {
        GameController.ads.SimulateAd(GemsAnimation);
    }

    private void GemsAnimation()
    {
        ParticleSystem gemExplosion = Instantiate(GemExplosionPrefab, transform).GetComponent<ParticleSystem>();
        var gemParMainModule = gemExplosion.main;
        gemParMainModule.duration = gemClaimCount / 20f;
        gemExplosion.Play();
        Destroy(gemExplosion.gameObject, 4f);
        ClaimGemsButton.SetActive(false);
        GameController.gemc.ClaimGems(gemClaimCount);
    }
}
