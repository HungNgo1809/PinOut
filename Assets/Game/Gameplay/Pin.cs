using DG.Tweening;
using Funzilla;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pin : MonoBehaviour
{
    internal int pinId;
    public PinType type;

    public List<int> innerPins = new List<int>();
    public List<int> frontPins = new List<int>();
    public List<int> dependencePins = new List<int>();

    [SerializeField] internal int size = 1;
    [SerializeField] float backChangePerSize = -0.6f;
    [SerializeField] List<GameObject> sizeObjs = new List<GameObject>();
    [SerializeField] Transform checkPos;

    bool isSelecting;
    Vector3 originalPosition;
    Vector3 stopPoint;

    [SerializeField] Transform innerPos;
    [SerializeField] Transform outPos;

    [SerializeField] Vector3 innerCheckBoxCenterOffset;
    [SerializeField] Vector3 innerCheckBoxSize;

    [SerializeField] Vector3 frontCheckBoxCenterOffset;
    [SerializeField] Vector3 frontCheckBoxSize;

    internal Level curLevel;
    private void Start()
    {
        RunningInit();
    }
    internal void RunningInit()
    {
        originalPosition = stopPoint = transform.position;
    }
    internal void HandlePinSize()
    {
        sizeObjs[size - 1].SetActive(true);
        checkPos.transform.localPosition = new Vector3(checkPos.transform.localPosition.x, checkPos.transform.localPosition.y, checkPos.transform.localPosition.z + backChangePerSize * (size - 1) * 0.5f);
        innerCheckBoxCenterOffset = new Vector3(innerCheckBoxCenterOffset.x, innerCheckBoxCenterOffset.y, innerCheckBoxCenterOffset.z * size);
        innerCheckBoxSize = new Vector3(innerCheckBoxSize.x, innerCheckBoxSize.y, innerCheckBoxSize.z * size);
    }
    internal void Init(Level level)
    {
        curLevel = level;
        GetLinkedPin();
    }
        
    internal void MouseDown()
    {
        isSelecting = true;
        if (!vibrating)
            StartCoroutine(Vibrate());
    }
    internal void MouseUp()
    {
        if (!isSelecting)
            return;

        int checkPinOutResult = CanPinOut();
        //check pin out
        if(checkPinOutResult == 0)
        {
            StartCoroutine(MoveOut());
        }    
        else
        {
            //do something when cant move
            StartCoroutine(MoveWrongAndGetBack(checkPinOutResult == 1));
        }
        isSelecting = false;
    }
    internal void MouseExit()
    {
        isSelecting = false;
    }
    bool vibrating = false;
    IEnumerator Vibrate()
    {
        vibrating = true;
        while(isSelecting)
        {
            // Rung ngẫu nhiên trong khoảng nhỏ xung quanh vị trí ban đầu
            float offsetX = Random.Range(-GlobalDefine.VibrateIntensity, GlobalDefine.VibrateIntensity); // Điều chỉnh giá trị này để tăng/giảm độ rung
            float offsetY = Random.Range(-GlobalDefine.VibrateIntensity, GlobalDefine.VibrateIntensity);
            float offsetZ = Random.Range(-GlobalDefine.VibrateIntensity, GlobalDefine.VibrateIntensity);

            // Cập nhật vị trí rung cho đối tượng
            transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, offsetZ);

            // Đợi một khung hình trước khi tiếp tục
            yield return null;
        }
        Normalize();
    }
    void Normalize()
    {
        transform.position = originalPosition;
        vibrating = false;
    }  
    int CanPinOut()
    {
        GetStopPos();
        if (innerPins.Count > 0)
        {
            return 1;
        }
        if(frontPins.Count > 0)
        {
            //Get stopPoint
            return 2;
        }    

        return 0;//find stop point here
    }
    IEnumerator MoveOut()
    {
        Vector3 targetPosition = transform.position + transform.forward * 10.0f;

        float distance = Vector3.Distance(transform.position, targetPosition);
        float moveDuration = distance / GlobalDefine.PinMoveSpeed;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(targetPosition, moveDuration).SetEase(Ease.OutCubic));

        yield return seq.WaitForCompletion();
        OnRemove();
    }
    IEnumerator MoveWrongAndGetBack(bool wrongIn = true)
    {
        Vector3 targetPosition = wrongIn ? (originalPosition + (stopPoint - innerPos.position)) : (originalPosition + (stopPoint - outPos.position));

        float distance = Vector3.Distance(transform.position, targetPosition);
        float moveDuration = distance / GlobalDefine.PinMoveSpeed;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(targetPosition, moveDuration).SetEase(Ease.OutCubic));

        yield return seq.WaitForCompletion();
        yield return new WaitForSeconds(0.5f);

        transform.DOMove(originalPosition, moveDuration).SetEase(Ease.OutCubic);
    } 
    void GetLinkedPin()
    {
        //Get inner
        Collider[] hitColliders = Physics.OverlapBox(innerPos.position + transform.rotation * innerCheckBoxCenterOffset, innerCheckBoxSize / 2, transform.rotation);
        foreach (Collider hitCollider in hitColliders)
        {
            Pin hitPin = hitCollider.gameObject.GetComponentInParent<Pin>();
            if (hitPin == null || innerPins.Contains(hitPin.pinId) || hitPin == this)
                continue;

            if (!hitPin.dependencePins.Contains(pinId))
                hitPin.dependencePins.Add(pinId);
            innerPins.Add(hitPin.pinId);
        }

        //Get front
        Collider[] hitOutColliders = Physics.OverlapBox(outPos.position + transform.rotation * frontCheckBoxCenterOffset, frontCheckBoxSize / 2, transform.rotation);
        foreach (Collider hitCollider in hitOutColliders)
        {
            Pin hitPin = hitCollider.gameObject.GetComponentInParent<Pin>();
            if (hitPin == null || innerPins.Contains(hitPin.pinId))
                continue;

            if (!hitPin.dependencePins.Contains(pinId))
                hitPin.dependencePins.Add(pinId);
            frontPins.Add(pinId);
        }
    } 
    internal void OnRemove()
    {
        if(dependencePins.Count > 0)
        {
            foreach(int pinId in dependencePins)
            {
                if (pinId < 0 || curLevel.pins.Count <= pinId)
                    continue;

                Pin pin = curLevel.pins[pinId];
                if (pin.innerPins.Contains(pinId))
                    pin.innerPins.Remove(pinId);
                else if (pin.frontPins.Contains(pinId))
                    pin.frontPins.Remove(pinId);
            }
        }

        Destroy(gameObject);
    }
    internal void ClearAllLinkedPins()
    {
        innerPins.Clear();
        frontPins.Clear();
        dependencePins.Clear();
    }
    void GetStopPos()
    {
        float minDis = Mathf.Infinity;
        if (innerPins.Count > 0)
        {
            foreach(int pinId in innerPins)
            {
                Pin pin = curLevel.pins[pinId];
                Vector3 point = Calculate.GetProjectionOnPlane(pin.transform.position, innerPos.position, innerPos.up);
                float distance = GetDistanceToInnerPins(innerPos.position, point);
                if(distance < minDis)
                {
                    minDis = distance;
                    stopPoint = Calculate.GetProjectionOnVector(point, innerPos.position, innerPos.forward);
                }    
            }    
        }
        else if(frontPins.Count > 0)
        {
            foreach (int pinId in frontPins)
            {
                Pin pin = curLevel.pins[pinId];
                Vector3 point = Calculate.GetProjectionOnPlane(pin.transform.position, outPos.position, outPos.up);
                float distance = GetDistanceToInnerPins(outPos.position, point); //TODO: replace by out

                if (distance < minDis)
                {
                    minDis = distance;
                    stopPoint = Calculate.GetProjectionOnVector(point, outPos.position, outPos.forward);
                }
            }
        }    
    }
    float GetDistanceToInnerPins(Vector3 curPos, Vector3 pinPosOnCurPlane)
    {
        //chiếu curPos đến mặt phẳng của Pin muốn kiểm tra hiện tại (điểm A), sau đó bắn raycast từ điểm A đến pinPosOnCurPlane, nếu va chạm thì tính distance từ A đến va chạm, không thì tính từ A đến pinPosOnCurPlane
        
        Vector3 point = Calculate.GetProjectionOnPlane(curPos, pinPosOnCurPlane, outPos.right); //=> chưa hoàn thiện cần check lại

        // Tính hướng raycast từ điểm đã tính toán đến vị trí của pin
        Vector3 direction = pinPosOnCurPlane - point;

        // Thực hiện raycast
        RaycastHit hit;
        if (Physics.Raycast(point, direction.normalized, out hit, direction.magnitude))
        {
            // Nếu raycast va chạm, trả về khoảng cách từ điểm đến điểm va chạm
            return Vector3.Distance(point, hit.point);
        }
        else
        {
            // Nếu không va chạm, trả về khoảng cách từ point đến pinPosOnCurPlane
            return Vector3.Distance(point, pinPosOnCurPlane);
        }
    }
    void OnDrawGizmos()
    {
        Vector3 innerOffsetWorld = transform.rotation * innerCheckBoxCenterOffset;
        Vector3 frontOffsetWorld = transform.rotation * frontCheckBoxCenterOffset;

        // Vẽ inner check box với offset đã được tính toán theo local
        Gizmos.color = Color.green;
        Matrix4x4 innerRotationMatrix = Matrix4x4.TRS(checkPos.position + innerOffsetWorld, transform.rotation, Vector3.one);
        Gizmos.matrix = innerRotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, innerCheckBoxSize);

        // Vẽ front check box với offset đã được tính toán theo local
        Gizmos.color = Color.red;
        Matrix4x4 frontRotationMatrix = Matrix4x4.TRS(outPos.position + frontOffsetWorld, transform.rotation, Vector3.one);
        Gizmos.matrix = frontRotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, frontCheckBoxSize);
    }
}

[System.Serializable]
public enum PinType
{
    Single = 0,
}
