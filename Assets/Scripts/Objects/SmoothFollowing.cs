using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollowing : MonoBehaviour
{
    public Transform target;
    public bool autosetTargetAsPlayer = true;
    public string playerTag = "Player";
    public AnimationCurve smoothCurve;
    public bool saveCurrentPosition = true;
    public Vector3 offset = Vector3.zero;
    public float smoothDistance = 0.5f;
    public float smoothDelay = 3;
    public bool freezeY = false;

    private float _time = 0;
    private Vector3 _currPos = Vector3.zero;

    private void Start()
    {
        if (autosetTargetAsPlayer)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);

            if (player != null) target = player.transform;
        }

        if (saveCurrentPosition)
        {
            _currPos = transform.position;
        }
    }

    private void FixedUpdate()
    {
        if (target)
        {
            Vector3 targetPos = target.position + offset + _currPos;
            Vector3 toPos = Vector3.MoveTowards(transform.position, targetPos, smoothCurve.Evaluate(Timer(targetPos)));

            if (freezeY)
            {
                transform.position = new Vector3(toPos.x, transform.position.y, toPos.z);
            }
            else transform.position = toPos;
        }
    }

    private float Timer(Vector3 target)
    {

        if ((target - transform.position).magnitude < smoothDistance)
        {
            _time -= Time.deltaTime;

        }
        else _time += Time.deltaTime;

        if (_time >= smoothDelay)
        {
            _time = smoothDelay;
        }

        if (_time < 0)
        {
            _time = 0;
        }

        return _time / smoothDelay;
    }
}
