using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IkController : MonoBehaviour {

    public Animator anim;
    public float leftWeight = 1f;
    public float rightWeight = 1f;
    public Vector3? leftPosition;
    public Vector3? rightPosition;
    public Transform leftFootTransform;
    public Transform rightFootTransform;
    public Transform leftToeTransform;
    public Transform rightToeTransform;
    public Quaternion leftFootRotation;
    public Quaternion rightFootRotation;
    public float weight = 0.1f;
    public float offset = .1f;

    LayerMask layerMask;

    // Use this for initialization
    void Start () {
        anim = FindObjectOfType<Animator>();
        layerMask = LayerMask.GetMask("Terrain");
        leftFootTransform = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFootTransform = anim.GetBoneTransform(HumanBodyBones.RightFoot);
        leftToeTransform = anim.GetBoneTransform(HumanBodyBones.LeftToes);
        rightToeTransform = anim.GetBoneTransform(HumanBodyBones.RightToes);
    }

    // Update is called once per frame
    void Update () {
        leftPosition = GetTerrainHeight(leftFootTransform.position);
        rightPosition = GetTerrainHeight(rightFootTransform.position);
        var leftToePosition = GetTerrainHeight(leftToeTransform.position);
        var rightToePosition = GetTerrainHeight(rightToeTransform.position);

        if (leftToePosition.HasValue && leftPosition.HasValue)
        {
            leftFootRotation = Quaternion.LookRotation(leftToePosition.Value - leftPosition.Value);
        }

        if (rightToePosition.HasValue && rightPosition.HasValue)
        {
            rightFootRotation = Quaternion.LookRotation(rightToePosition.Value - rightPosition.Value);
        }
    }

    void OnAnimatorIK()
    {
        leftWeight = anim.GetFloat("LeftFoot");
        rightWeight = anim.GetFloat("RightFoot");

        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftWeight * weight);
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightWeight * weight);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftWeight * weight);
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightWeight * weight);

        if (leftPosition.HasValue)
        {
            anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftPosition.Value);
            anim.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootRotation);
        }

        if (rightPosition.HasValue)
        {
            anim.SetIKPosition(AvatarIKGoal.RightFoot, rightPosition.Value);
            anim.SetIKRotation(AvatarIKGoal.RightFoot, rightFootRotation);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (leftPosition.HasValue)
        {
            Gizmos.DrawCube(leftPosition.Value, Vector3.one * 0.1f);

        }
        if (rightPosition.HasValue)
        {
            Gizmos.DrawCube(rightPosition.Value, Vector3.one * 0.1f);
        }
    }

    Vector3? GetTerrainHeight(Vector3 p)
    {
        RaycastHit hit;
        if (Physics.Raycast(p + Vector3.up, Vector3.down, out hit, 3, layerMask))
        {
            return hit.point + Vector3.up * offset;
        }

        return null;
    }


}
