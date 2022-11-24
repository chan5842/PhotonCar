using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCar : MonoBehaviour
{
    [Header("Network")]
    [SerializeField] PhotonView pv =null;
    [Header("Components")]
    [SerializeField] Rigidbody rb;
    [SerializeField] BoxCollider boxcolider;
    [Header("Wheel Collider")]
    [SerializeField] WheelCollider FRC;
    [SerializeField] WheelCollider FLC;
    [SerializeField] WheelCollider RRC;
    [SerializeField] WheelCollider RLC;
    [Header("Wheel Transform")]
    [SerializeField] Transform FRM;
    [SerializeField] Transform FLM;
    [SerializeField] Transform RRM;
    [SerializeField] Transform RLM;
    [Header("FrictionCurve")]
    WheelFrictionCurve wheelFLF;
    WheelFrictionCurve wheelFRF;
    [Header("MoveOrRotate Key")]
    [SerializeField] float steer = 0f;
    [SerializeField] float brake = 0f;
    [SerializeField] float accel = 0f;
    [Header("WheelSpeed")]
    [SerializeField] float curSpeed = 0f;
    [SerializeField] float maxSteer = 40f;
    [SerializeField] float maxTorque = 400f;
    [SerializeField] float maxBrake = 3500f;
    [SerializeField] float motor = 0f;
    [SerializeField] float braking = 0f;

    [Header("Bool")] [SerializeField] bool Reverse;
    [SerializeField] float slipRate = 1f;
    [SerializeField] float Drift = 0.4f;

    [SerializeField] Text SpeedText;
    Vector3 curPos = Vector3.zero;
    Quaternion curRot = Quaternion.identity;

    void Awake()
    {
        pv = PhotonView.Get(this);
        pv.synchronization = ViewSynchronization.UnreliableOnChange;
        pv.ObservedComponents[0] = this;
        if(pv.isMine)
        {
            Camera.main.GetComponent<FollowCam>().target = this.transform;
        }
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0f, -0.5f, 0f); // ���� ���� �߽�
        boxcolider = GetComponentInChildren<BoxCollider>();

        FRC = transform.GetChild(2).GetChild(3).GetComponent<WheelCollider>();
        FLC = transform.GetChild(2).GetChild(2).GetComponent<WheelCollider>();
        RRC = transform.GetChild(2).GetChild(1).GetComponent<WheelCollider>();
        RLC = transform.GetChild(2).GetChild(0).GetComponent<WheelCollider>();

        FRM = transform.GetChild(1).GetChild(3).GetComponent<Transform>();
        FLM = transform.GetChild(1).GetChild(2).GetComponent<Transform>();
        RRM = transform.GetChild(1).GetChild(1).GetComponent<Transform>();
        RLM = transform.GetChild(1).GetChild(0).GetComponent<Transform>();

        curPos = transform.position;
        curRot = transform.rotation;
    }

    // �ڽ��� �������� �۽� �ٸ� ��Ʈ��ũ ������ �������� ����
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)    // �۽�
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else if (stream.isReading)   // �ٸ� ��Ʈ��ũ �������� ����
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
        }
    }


    void Update()
    {
        if(pv.isMine)
        {
            // �ι����� ũ���� ��ü�� �з����� ������ �ӵ��� ������ ����
            curSpeed = rb.velocity.sqrMagnitude;
            steer = Mathf.Clamp(Input.GetAxis("Horizontal"), -1f, 1f);
            accel = Mathf.Clamp(Input.GetAxis("Vertical"), 0f, 1f);
            brake = -1 * Mathf.Clamp(Input.GetAxis("Vertical"), -1f, 0f);
            if (accel > 0)
            {
                StartCoroutine(FowardCar());
            }
            if (brake > 0)
            {
                StartCoroutine(BackwardCar());
            }
            if (Reverse)
            {
                motor = -1 * brake;
            }
            else
            {
                motor = accel;
                braking = brake;
            }

            // �� �ݶ��̴��� ������ ������ �����̴� ����
            // �չ����� ������ ȸ��
            FLC.steerAngle = maxSteer * steer;
            FRC.steerAngle = maxSteer * steer;
            // �޹����� ������ ������ �����δ�
            RLC.motorTorque = motor * maxTorque;
            RRC.motorTorque = motor * maxTorque;

            // �չ��� �𵨸� �¿� ȸ��
            FLM.localEulerAngles = new Vector3(FLM.localEulerAngles.x, steer * maxSteer, FLM.localEulerAngles.z);
            FRM.localEulerAngles = new Vector3(FRM.localEulerAngles.x, steer * maxSteer, FRM.localEulerAngles.z);

            // ���� �𵨸� ȸ��
            FRM.Rotate(FRC.rpm * Time.deltaTime, 0f, 0f);
            FLM.Rotate(FLC.rpm * Time.deltaTime, 0f, 0f);
            RRM.Rotate(RRC.rpm * Time.deltaTime, 0f, 0f);
            RLM.Rotate(RLC.rpm * Time.deltaTime, 0f, 0f);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 3f);
            transform.rotation = Quaternion.Slerp(transform.rotation, curRot, Time.deltaTime * 3f);
        }
        SpeedMeter();
        pv.RPC("SpeedMeter", PhotonTargets.Others, null);

    }

    IEnumerator FowardCar()
    {
        yield return new WaitForSeconds(0.01f);
        curSpeed = 0f;
        if (accel > 0)
            Reverse = false;
        else
            Reverse = true;
    }
    IEnumerator BackwardCar()
    {
        yield return new WaitForSeconds(0.01f);
        curSpeed = 0f;
        if (accel > 0)
            Reverse = false;
        else
            Reverse = true;
    }

    [PunRPC]
    void SpeedMeter()
    {
        int speedMeter = (int)curSpeed / 36;
        SpeedText.text = "Speed : " + speedMeter.ToString("000") + "Km/h";
    }
}
