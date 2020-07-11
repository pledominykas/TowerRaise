using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreController : MonoBehaviour
{
    private const int FIRST_BRICK_SCORE = 10;
    private const int NEXT_BRICK_SCORE_ADDEND = 10;
    private const int PERFECT_HIT_MULTIPLIER = 2;

    [SerializeField] TextMeshProUGUI ScoreText;
    [SerializeField] TextMeshProUGUI ScoreAddText;
    [SerializeField] TextMeshProUGUI PerfectHitText;
    [SerializeField] Animator ScoreAnimator;

    private int Score = 0;
    private int BestScore = 0;

    public void AddScore(int _currentTowerHeight, int _perfectHitCount)
    {
        if(ScoreAnimator.gameObject.activeSelf == false) { return; }
        if(_perfectHitCount != 0)
        {
            Score += ((FIRST_BRICK_SCORE + NEXT_BRICK_SCORE_ADDEND * _currentTowerHeight) * PERFECT_HIT_MULTIPLIER * _perfectHitCount);
            ScoreAddText.text = "+" + ((FIRST_BRICK_SCORE + NEXT_BRICK_SCORE_ADDEND * _currentTowerHeight) * PERFECT_HIT_MULTIPLIER * _perfectHitCount).ToString();
            ScoreAddText.gameObject.SetActive(true);
            ScoreAnimator.Play("AddScoreAnimation", 1);
            PerfectHitText.text = "(" + _perfectHitCount.ToString() + ") " + "Perfect Hit! X" + (PERFECT_HIT_MULTIPLIER * _perfectHitCount).ToString();
            PerfectHitText.gameObject.SetActive(true);
            ScoreAnimator.Play("PerfectHitAnimation", 2);
        }
        else
        {
            Score += (FIRST_BRICK_SCORE + NEXT_BRICK_SCORE_ADDEND * _currentTowerHeight);
            ScoreAddText.text = "+" + (FIRST_BRICK_SCORE + NEXT_BRICK_SCORE_ADDEND * _currentTowerHeight).ToString();
            ScoreAddText.gameObject.SetActive(true);
            ScoreAnimator.Play("AddScoreAnimation", 1);
        }
        CheckBestScore();
        StartCoroutine(PlayOnScoreAddAnimation());
    }

    private IEnumerator PlayOnScoreAddAnimation()
    {
        yield return new WaitForSeconds(0.25f);
        if (ScoreAnimator.gameObject.activeSelf == false) { yield break; }
        ScoreAnimator.Play("OnScoreAdd");
        yield return new WaitForSeconds(0.03f);
        ScoreText.text = Score.ToString();
    }

    public void ResetScore()
    {
        Score = 0;
        ScoreText.text = Score.ToString();
    }

    public int GetScore() { return Score; }
    public int GetBestScore() { return BestScore; }
    private void CheckBestScore() { if (Score >= BestScore) { BestScore = Score; } }
    public void SetBestScore(int _score) { BestScore = _score; }
}
