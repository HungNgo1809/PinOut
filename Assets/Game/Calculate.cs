using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculate
{
    public static Vector3 GetProjectionOnPlane(Vector3 pointA, Vector3 planePoint, Vector3 planeNormal)
    {
        planeNormal.Normalize();

        // Vector từ điểm A tới một điểm trên mặt phẳng
        Vector3 pointToPlane = pointA - planePoint;

        // Tính khoảng cách từ điểm A đến mặt phẳng
        float distance = Vector3.Dot(pointToPlane, planeNormal);

        // Tính tọa độ hình chiếu của điểm A lên mặt phẳng
        Vector3 projection = pointA - distance * planeNormal;

        return projection;
    }
    public static Vector3 GetProjectionOnLine(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        Vector3 lineDirection = pointC - pointB;

        // Tính độ dài của vector đường thẳng BC
        float lineLengthSquared = lineDirection.sqrMagnitude;

        // Nếu độ dài đường thẳng BC là 0, trả về điểm B
        if (lineLengthSquared < Mathf.Epsilon)
        {
            return pointB;
        }

        // Tính vector từ B đến A
        Vector3 pointToB = pointA - pointB;

        // Tính t, là tỷ lệ để tìm hình chiếu
        float t = Vector3.Dot(pointToB, lineDirection) / lineLengthSquared;

        // Tính tọa độ hình chiếu
        Vector3 projection = pointB + t * lineDirection;

        return projection;
    }
    public static Vector3 GetProjectionOnVector(Vector3 pointA, Vector3 pointB, Vector3 vectorB)
    {
        // Tính vector từ B đến A
        Vector3 pointToB = pointA - pointB;

        // Tính độ dài bình phương của vector b
        float vectorBSquared = vectorB.sqrMagnitude;

        // Nếu vector b có độ dài bằng 0, trả về điểm B
        if (vectorBSquared < Mathf.Epsilon)
        {
            return pointB;
        }

        // Tính t, tỷ lệ để tìm hình chiếu
        float t = Vector3.Dot(pointToB, vectorB.normalized) / vectorBSquared;

        // Tính tọa độ hình chiếu
        Vector3 projection = pointB + t * vectorB.normalized * vectorBSquared;

        return projection;
    }
    public static float GetDistanceFromPointToLine(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        // Tính vector từ B đến C
        Vector3 lineVector = pointC - pointB;
        // Tính vector từ B đến A
        Vector3 pointToLineVector = pointA - pointB;

        // Tính độ dài đường thẳng
        float lineLengthSquared = lineVector.sqrMagnitude;

        // Nếu độ dài của đường thẳng bằng 0, trả về khoảng cách từ A đến B
        if (lineLengthSquared < Mathf.Epsilon)
        {
            return Vector3.Distance(pointA, pointB);
        }

        // Tính t, tỷ lệ để tìm điểm gần nhất trên đường thẳng
        float t = Vector3.Dot(pointToLineVector, lineVector) / lineLengthSquared;

        // Giới hạn t trong khoảng [0, 1] để đảm bảo điểm gần nhất nằm trong đoạn thẳng
        t = Mathf.Clamp01(t);

        // Tính tọa độ điểm gần nhất trên đường thẳng
        Vector3 closestPointOnLine = pointB + t * lineVector;

        // Tính khoảng cách từ điểm A đến điểm gần nhất trên đường thẳng
        float distance = Vector3.Distance(pointA, closestPointOnLine);

        return distance;
    }
    public static float GetDistanceFromPointToVector(Vector3 pointA, Vector3 pointB, Vector3 direction)
    {
        // Tính vector từ điểm B đến điểm A
        Vector3 pointToLineVector = pointA - pointB;

        // Tính vector pháp tuyến từ điểm A đến đường thẳng (bằng cách loại bỏ thành phần song song với direction)
        Vector3 projection = Vector3.Project(pointToLineVector, direction);

        // Tính vector vuông góc từ điểm A đến đường thẳng
        Vector3 perpendicularVector = pointToLineVector - projection;

        // Tính khoảng cách, chính là độ dài của vector vuông góc
        float distance = perpendicularVector.magnitude;

        return distance;
    }
}
