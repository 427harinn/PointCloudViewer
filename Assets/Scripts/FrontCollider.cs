using System.Collections.Generic;
using UnityEngine;
using Share;

public class FrontCollider : MonoBehaviour
{
    [SerializeField]
    private Point PointPrefab;

    private List<Point> collidedPoints = new List<Point>();
    public int requiredPointCount = 3;  // 何個以上の点が当たったら出力するか

    public AudioSource audiosource;
    private bool isAudioPlaying = false;

    void OnTriggerEnter(Collider other)
    {
        Point pointPrefab = other.GetComponent<Point>();
        if (pointPrefab != null && !collidedPoints.Contains(pointPrefab))
        {
            collidedPoints.Add(pointPrefab);
            //Debug.Log("点に当たりました：" + pointPrefab.name);

            if (collidedPoints.Count >= requiredPointCount && !isAudioPlaying)
            {
                Debug.Log(requiredPointCount + "個以上の点に当たりました！");
                PlayAudio();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        Point pointPrefab = other.GetComponent<Point>();
        if (pointPrefab != null && collidedPoints.Contains(pointPrefab))
        {
            collidedPoints.Remove(pointPrefab);
            if (collidedPoints.Count < requiredPointCount && isAudioPlaying)
            {
                StopAudio();
            }
        }
    }

    private void PlayAudio()
    {
        Debug.Log("音を再生");
        audiosource.Play();
        isAudioPlaying = true;
    }

    private void StopAudio()
    {
        Debug.Log("音を停止");
        audiosource.Stop();
        isAudioPlaying = false;
    }
}
