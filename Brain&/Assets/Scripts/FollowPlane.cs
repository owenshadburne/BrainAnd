using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlane : MonoBehaviour
{
    [SerializeField] Transform following;
    [SerializeField] float followSharpness = 0.05f;
    [SerializeField] float indiviualOffsetY;

    List<Quaternion> pastRotations;
    List<Vector3> pastPositions;

    private void Start()
    {
        pastRotations = new List<Quaternion>();
        pastRotations.Add(following.rotation);
        pastPositions = new List<Vector3>();
        pastPositions.Add(following.position);

        indiviualOffsetY = transform.localPosition.y;
    }

    void LateUpdate() 
    {
        //Blend();
        Path();
        //OffsetPosition();
    }

    void Blend()
    {
        transform.position = Vector3.Slerp(following.position, transform.position, followSharpness);
        transform.rotation = Quaternion.Slerp(following.rotation, transform.rotation, followSharpness);
    }

    void Path()
    {
        if (pastRotations.Count > 100)
        {
            Quaternion rot = pastRotations[0];
            pastRotations.RemoveAt(0);

            transform.rotation = Quaternion.Slerp(rot, transform.rotation, followSharpness);
        }
        pastRotations.Add(following.rotation);

        if(pastPositions.Count > 100)
        {
            Vector3 pos = pastPositions[0];
            pastPositions.RemoveAt(0);

            transform.position = Vector3.Slerp(pos, transform.position, followSharpness);
        }
        pastPositions.Add(following.position);
    }
}
