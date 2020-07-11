using UnityEngine;
using UnityEngine.Audio;

public class BrickAudio : MonoBehaviour
{
    private const float PERFECT_HIT_PITCH_INCREASE = 0.5f;
    private const float TOWER_BUILT_PITCH_INCREASE = 0.1f;
    private const float MIN_HEIGHT_ADD_PITCH = 0.75f;
    private const float MAX_HEIGHT_ADD_PITCH = 1f;

    [SerializeField] AudioSource OnAddHeight;
    [SerializeField] AudioSource OnPerfectHit;
    [SerializeField] AudioSource OnTowerBuilt;
    [SerializeField] AudioSource OnBrickCreate;

    public void PlayHeightAdd()
    {
        OnAddHeight.pitch = Random.Range(MIN_HEIGHT_ADD_PITCH, MAX_HEIGHT_ADD_PITCH);
        OnAddHeight.Play();
    }

    public void PlayPerfectHit(int _hitsInARow)
    {
        OnPerfectHit.pitch = 0.5f + PERFECT_HIT_PITCH_INCREASE * _hitsInARow;
        OnPerfectHit.Play();
    }

    public void PlayTowerBuilt(int _brickNum)
    {
        OnTowerBuilt.pitch = 1f + _brickNum * TOWER_BUILT_PITCH_INCREASE;
        OnTowerBuilt.Play();
    }

    public void PlayBrickCreate()
    {
        OnBrickCreate.Play();
    }

}
