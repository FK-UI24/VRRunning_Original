using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//RigidBody���K�{�ł��邱�Ƃ𖾎�
[RequireComponent(typeof(Rigidbody))]


//���̃X�N���v�g�̓��C���J�����ɕt���邪�A������g�p����V�[���ł̓p�l���̐ؑւ�
//�V�[���̂悤�Ɍ����Ă��邽�߁A�J�������P�����Ȃ��󋵂ł͑z��O�̋������N����
//���̂��߁A�ق��̃X�N���v�g�������̃p�l���ɂȂ����Ƃ��ɃI���ɂ���悤�ɂ���


public class Script_CameraControll : MonoBehaviour
{
    [Header("�J�����̏����ʒu�Fx")]
    [SerializeField] private float Startx;

    [Header("�J�����̏����ʒu�Fy")]
    [SerializeField] private float Starty;

    [Header("�J�����̏����ʒu�Fz")]
    [SerializeField] private float Startz;

    //�J���������ʒu�pVector3�ϐ�
    Vector3 StartPos;


    [Header("�p���ړ����x")]
    [SerializeField]private float panSpeed;

    [Header("�Y�[�����x")]
    [SerializeField] private float zoomSpeed;

    [Header("�Y�[���ȍŒZ����")]
    [SerializeField] private float minZoom;

    [Header("�Y�[���ȍŒ�����")]
    [SerializeField] private float maxZoom;

    //�E�N���b�N�����Ă��邩�̊Ǘ��p�ϐ�
    bool isRightClicking;

    //�Ō�ɃN���b�N�����ӏ�
    private Vector3 lastMousePosition;

    //Rigidbody�̎Q��
    private Rigidbody rb;

    private void Awake()
    {
        StartPos.x = Startx;
        StartPos.y = Starty;
        StartPos.z = Startz;
    }

    void Start()
    {
        rb=GetComponent<Rigidbody>();

        //Rigidbody�̊�{�ݒ�
        //�d�͕͂s�v
        rb.useGravity = false;

        //MousePosition���g�����߂�false�ɂ���
        rb.isKinematic = false;

        //��]�͌Œ�
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        mousePan();
        
        scrollZoom();

        ResetPosition();
    }

    void mousePan()
    {
        //�E�N���b�N�����Ă��邩�𔻒肷��
        isRightClicking = Input.GetMouseButton(1);

        //�p������i���N���b�N�h���b�O�j
        if (isRightClicking == false)
        {
            //���N���b�N���������u�ԂɃ}�E�X�ʒu���L�^����
            if (Input.GetMouseButtonDown(0))
            {
                lastMousePosition = Input.mousePosition;
            }

            //���N���b�N�����������Ă����
            if (Input.GetMouseButton(0))
            {
                //�}�E�X�̈ړ��ʁi���݈ʒu-�Ō�ɋL�^�����ʒu�j
                Vector3 delta = (Input.mousePosition - lastMousePosition) / Screen.height;

                //�J�����𓮂����ʁi�}�C�i�X�ŋt�����j
                Vector3 move = new Vector3(-delta.x, 0, -delta.y) * panSpeed;

                //�ړI�n���v�Z����
                Vector3 nextPosition=rb.position + move;

                //�p�������ɏ�Q���`�F�b�N������
                if (!Physics.Raycast(rb.position, move.normalized, move.magnitude + 1.0f))
                {
                    //Rigidbody���g���Ĉړ�����
                    rb.MovePosition(nextPosition);
                }

                //����̃}�E�X�ʒu�����̃t���[���̔�r�p�ɕۑ�����
                lastMousePosition = Input.mousePosition;
            }
        }
        //�E�N���b�N���͊����^�����~�߂邽�߂ɑ��x�B��]���x���[���ɂ���
        else
        {
            rb.linearVelocity=Vector3.zero;
            rb.angularVelocity=Vector3.zero;
        }
    }

    void scrollZoom()
    {
        //�}�E�X�z�C�[���̉�]�ʂ��擾����
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        //�J�����̌����Ă�������ɃX�N���[�����̈ړ��ʂ��v�Z����
        Vector3 zoomMove = transform.forward * scroll * zoomSpeed * Time.deltaTime;

        //���Ɉړ�����ʒu���v�Z����
        Vector3 nextPos = rb.position + zoomMove;

        //�����iY�j�Ő������鋗������
        float nextHeight = nextPos.y;

        //RayCast�ŏ�Q���ɂ����邩���v�Z����
        if (!Physics.Raycast(rb.position, zoomMove.normalized, zoomMove.magnitude + 3.0f))
        {
            //�������͈͓��Ȃ�ړ�����
            if (nextHeight > minZoom && nextHeight < maxZoom)
            {
                nextHeight = minZoom;
                
                //Rigidbody���g�p���Ĉړ�����
                rb.MovePosition(nextPos);
            }
        }
    }

    //�J�����̈ʒu���C���X�y�N�^�[���Őݒ肵�����W�ɖ߂��֐�
    void ResetPosition()
    {
        if (Input.GetMouseButtonDown(2))
        {
            transform.position = StartPos;
        }
    }
}
