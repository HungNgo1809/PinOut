using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pin : MonoBehaviour
{
    public List<Pin> innerPins = new List<Pin>();
    public List<Pin> frontPins = new List<Pin>();
    public List<Pin> dependencePins = new List<Pin>();

    bool isSelecting;
    Vector3 originalPosition;
    Vector3 stopPoint;

    [SerializeField] Transform innerPos;
    [SerializeField] Transform outPos;

    [SerializeField] Vector3 innerCheckBoxCenterOffset;
    [SerializeField] Vector3 innerCheckBoxSize;

    [SerializeField] Vector3 frontCheckBoxCenterOffset;
    [SerializeField] Vector3 frontCheckBoxSize;
    private void Start()
    {
        RunningInit();
    }
    internal void RunningInit()
    {
        originalPosition = stopPoint = transform.position;
    }
    internal void Init()
    {
        GetLinkedPin();
    }
        
    internal void MouseDown()
    {
        isSelecting = true;
        Vibrate();
    }
    internal void MouseUp()
    {
        isSelecting = false;
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
    }
    internal void MouseExit()
    {
        isSelecting = false;
    }
    void Vibrate()
    {
        //vibrate
        while(isSelecting)
        {
            float xOffset = Mathf.Sin(Time.time * GlobalDefine.shakeFrequency) * GlobalDefine.shakeAmplitude;
            float yOffset = Mathf.Sin(Time.time * GlobalDefine.shakeFrequency) * GlobalDefine.shakeAmplitude;

            transform.position = originalPosition + new Vector3(xOffset, yOffset, 0);
        }
        Normalize();
    }
    void Normalize()
    {
        transform.position = originalPosition;
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
        float moveDuration = distance / GlobalDefine.pinMoveSpeed;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(targetPosition, moveDuration).SetEase(Ease.OutCubic));

        yield return seq.WaitForCompletion();
        OnRemove();
    }
    IEnumerator MoveWrongAndGetBack(bool wrongIn = true)
    {
        Vector3 targetPosition = wrongIn ? (originalPosition + (stopPoint - innerPos.position)) : (originalPosition + (stopPoint - outPos.position));

        float distance = Vector3.Distance(transform.position, targetPosition);
        float moveDuration = distance / GlobalDefine.pinMoveSpeed;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(targetPosition, moveDuration).SetEase(Ease.OutCubic));

        yield return seq.WaitForCompletion();
        yield return new WaitForSeconds(0.5f);

        transform.DOMove(originalPosition, moveDuration).SetEase(Ease.OutCubic);
    } 
    void GetLinkedPin()
    {
        //Get inner
        Collider[] hitColliders = Physics.OverlapBox(innerPos.position + innerCheckBoxCenterOffset, innerCheckBoxSize / 2, Quaternion.identity);
        foreach (Collider hitCollider in hitColliders)
        {
            Pin hitPin = hitCollider.gameObject.GetComponentInParent<Pin>();
            if (hitPin == null || innerPins.Contains(hitPin))
                continue;

            if (!hitPin.dependencePins.Contains(this))
                hitPin.dependencePins.Add(this);
            innerPins.Add(hitPin);
        }

        //Get front
        Collider[] hitOutColliders = Physics.OverlapBox(outPos.position + frontCheckBoxCenterOffset, frontCheckBoxSize / 2, Quaternion.identity);
        foreach (Collider hitCollider in hitOutColliders)
        {
            Pin hitPin = hitCollider.gameObject.GetComponentInParent<Pin>();
            if (hitPin == null || innerPins.Contains(hitPin))
                continue;

            if (!hitPin.dependencePins.Contains(this))
                hitPin.dependencePins.Add(this);
            frontPins.Add(hitPin);
        }
    }  
    internal void OnRemove()
    {
        if(dependencePins.Count > 0)
        {
            foreach(Pin pin in dependencePins)
            {
                if (pin.innerPins.Contains(this))
                    pin.innerPins.Remove(this);
                else if (pin.frontPins.Contains(this))
                    pin.frontPins.Remove(this);
            }
        }

        Destroy(gameObject);
    }
    internal void ClearAllPins()
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
            foreach(Pin pin in innerPins)
            {
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
            foreach (Pin pin in frontPins)
            {
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
}
