using UnityEngine;
using System.Collections.Generic;

public class BoxFacePlacer : MonoBehaviour
{
    public BoxCollider targetBox;  // 기준 박스

    [System.Serializable]
    public class ObjectPlacement
    {
        public GameObject targetObject;   // 위치시킬 대상 객체
        public Face face;                 // 어느 면에 붙을지
        public FacePosition position;     // 면 안에서의 9방향 중 하나
        public bool alignToNormal = true; // 노멀 방향 회전 여부
    }

    public enum Face { Front, Back, Left, Right, Top, Bottom }
    public enum FacePosition
    {
        TopLeft, Top, TopRight,
        Left, Center, Right,
        BottomLeft, Bottom, BottomRight
    }

    public List<ObjectPlacement> placements = new List<ObjectPlacement>();

    [ContextMenu("Apply Placements")]
    public void ApplyPlacements()
    {
        if (targetBox == null)
        {
            Debug.LogWarning("BoxCollider가 지정되지 않았습니다.");
            return;
        }

        Vector3 center = targetBox.transform.TransformPoint(targetBox.center);
        Vector3 size = Vector3.Scale(targetBox.size, targetBox.transform.lossyScale) * 0.5f;

        foreach (var placement in placements)
        {
            if (placement.targetObject == null) continue;

            // 면 노멀과 면의 중심 좌표 구하기
            Vector3 faceNormal, faceLocalOffset;
            GetFaceInfo(placement.face, size, out faceNormal, out faceLocalOffset);

            Vector3 worldFaceCenter = center + faceLocalOffset;

            // face 기준 좌표계 (right, up)
            Vector3 faceRight = Vector3.Cross(faceNormal, Vector3.up);
            if (faceRight == Vector3.zero) // 위/아래 예외 처리
                faceRight = Vector3.Cross(faceNormal, Vector3.forward);

            Vector3 faceUp = Vector3.Cross(faceRight, faceNormal).normalized;
            faceRight.Normalize();

            // offset 선택
            Vector2 offset2D = GetFacePositionOffset(placement.position);

            // 실제 위치
            Vector3 pos = worldFaceCenter
                          + faceRight * offset2D.x * size.x
                          + faceUp * offset2D.y * size.y;

            placement.targetObject.transform.position = pos;

            if (placement.alignToNormal)
                placement.targetObject.transform.rotation = Quaternion.LookRotation(faceNormal, faceUp);
        }
    }

    private void GetFaceInfo(Face face, Vector3 size, out Vector3 normal, out Vector3 localOffset)
    {
        switch (face)
        {
            case Face.Front:
                normal = targetBox.transform.forward;
                localOffset = new Vector3(0, 0, size.z);
                break;
            case Face.Back:
                normal = -targetBox.transform.forward;
                localOffset = new Vector3(0, 0, -size.z);
                break;
            case Face.Right:
                normal = targetBox.transform.right;
                localOffset = new Vector3(size.x, 0, 0);
                break;
            case Face.Left:
                normal = -targetBox.transform.right;
                localOffset = new Vector3(-size.x, 0, 0);
                break;
            case Face.Top:
                normal = targetBox.transform.up;
                localOffset = new Vector3(0, size.y, 0);
                break;
            case Face.Bottom:
                normal = -targetBox.transform.up;
                localOffset = new Vector3(0, -size.y, 0);
                break;
            default:
                normal = Vector3.forward;
                localOffset = Vector3.zero;
                break;
        }
    }

    private Vector2 GetFacePositionOffset(FacePosition pos)
    {
        switch (pos)
        {
            case FacePosition.TopLeft:     return new Vector2(-1,  1);
            case FacePosition.Top:         return new Vector2( 0,  1);
            case FacePosition.TopRight:    return new Vector2( 1,  1);
            case FacePosition.Left:        return new Vector2(-1,  0);
            case FacePosition.Center:      return new Vector2( 0,  0);
            case FacePosition.Right:       return new Vector2( 1,  0);
            case FacePosition.BottomLeft:  return new Vector2(-1, -1);
            case FacePosition.Bottom:      return new Vector2( 0, -1);
            case FacePosition.BottomRight: return new Vector2( 1, -1);
        }
        return Vector2.zero;
    }
}
