using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_Core
{
    public class Samurai : MonoBehaviour
    {
        // �������� ----------------------------------------------------------------
        [Header("�������ֵ")]
        public float MaxHealth = 100;
        [Header("��������")]
        public float defendProbability = 0.7f;
        [Header("�׶ΰٷֱ�")]
        [Range(0f, 1f)] public float firstStagePercentage = 0.9f;
        [Range(0f, 1f)] public float secondStagePercentage = 0.7f;
        [Range(0f, 1f)] public float thirdStagePercentage = 0.5f;

        // ��Ҫ��� -----------------------------------------------------------------
        private Rigidbody rb;
        private Animator anim_body;
        private Transform body;
        private BloodSplatteringEffect blood;
        [HideInInspector] public DartSpawner dartSpawner;

        // �˺����� ------------------------------------------------------------------
        private static readonly string s_popupTextPrefabPath = "PopupDamage/Popup";
        private GameObject popupTextPrefab;
        private Transform popupTextBirthplace;
        private float countDamaged = 0;                 // ��ʱ���˺��ۼƼ���
        private float popupTextFlashTime = 0.2f;        // �˺�����ˢ��ʱ��

        private float lastHurtTime = 0;
        private float lastDefendTime = 0;
        private float hitCooldownTime = 0.5f;           // �����޵�֡ʱ��
        private float breakDefenseHitTime = 1f;          // �Ʒ�ʱ��

        [Header("�ܻ��ӽ����ű仯")]
        [SerializeField] private float meleeAttackFov = 50;     //��ս
        [SerializeField] private float rangedAttackFov = 80;    //Զ��
        [SerializeField] private float truceFov = 60;           //��ս
        private float truceTimer = 0;

        [Header("��")]
        [SerializeField] private Collider foot;
        [SerializeField] private LayerMask ground;


        [SerializeField] private float hp = 0;
        public float Hp
        {
            get
            {
                return hp;
            }
            private set
            {
                hp = Mathf.Clamp(value, 0, MaxHealth);
            }
        }

        /// <summary>
        /// �Ʒ���
        /// </summary>
        public bool IsBroken => Time.time - lastHurtTime < breakDefenseHitTime;
        /// <summary>
        /// ������
        /// </summary>
        public bool IsDefend => Time.time - lastDefendTime < hitCooldownTime;
        /// <summary>
        /// վ�ڵ��ϣ���������
        /// </summary>
        public bool IsOnGround => CheckIfGrounded();
        /// <summary>
        /// �����ж�, ���ã�վ�ţ�û��
        /// </summary>
        public bool CanAction => anim_body.GetCurrentAnimatorStateInfo(0).IsName("Idle") && IsOnGround;
        /// <summary>
        /// �泯����
        /// </summary>
        public int Facing => body.localScale.x > 0 ? 1 : -1;
        /// <summary>
        /// �������
        /// </summary>
        public GameObject Body => body.gameObject;
        /// <summary>
        /// �׶�
        /// </summary>
        public int CurrentStage
        {
            get
            {
                if (Hp > MaxHealth * firstStagePercentage)
                {
                    // ��ʼ�׶�
                    return 0;
                }
                if (Hp > MaxHealth * secondStagePercentage)
                {
                    // ����˲��
                    return 1;
                }
                if (Hp > MaxHealth * thirdStagePercentage)
                {
                    // ��������
                    return 2;
                }
                // ��
                return 3;
            }
        }
        /// <summary>
        /// �ȴ�ʱ�����̱���
        /// </summary>
        public float MultipleOfReductionForWaiting
        {
            get
            {
                int currentState = CurrentStage;
                float result = (4 - currentState) / 4;
                if (GameManager.Instance.GameDifficulty == GameManager.GameDifficultyType.Normal)
                {
                    result += 1;
                }
                return result;
            }
        }

        #region - Public -
        public void FaceTarget(Vector3 target)
        {
            float dir = target.x - transform.position.x;
            body.transform.localScale = new Vector3(dir > 0 ? 1 : -1, 1, 1);
        }
        public void MoveOnly(float speed)
        {
            rb.velocity = new Vector3(speed, rb.velocity.y, rb.velocity.z);
        }
        public void MoveTarget(Vector3 target, float speed)
        {
            float dir = target.x - transform.position.x;
            dir = Mathf.Clamp(dir, -1, 1);
            rb.velocity = new Vector3(dir * speed, rb.velocity.y, rb.velocity.z);
            anim_body.SetBool("running", true);
        }
        public void StopMove()
        {
            rb.velocity = Vector3.zero;
            anim_body.SetBool("running", false);
        }
        public void ShootDart(Vector3 dir)
        {
            dartSpawner.Shoot(dir);
        }
        public void PlayAnimation(string name)
        {
            anim_body.Play(name);
        }
        #endregion

        #region - Hurt -
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("PlayerAttackBox"))
            {
                PlayerAttackBox attackBox = other.GetComponent<PlayerAttackBox>();
                GetHurt(attackBox.Player.transform.position, attackBox.HitStrength, attackBox);              
            }
        }

        [SerializeField] private int triggerNumber = 0;
        [SerializeField] private float cooldownDuration = 0f;
        public void GetHurt(Vector3 hurtSource, float hitStrength, PlayerAttackBox attackBox)
        {
            // ��ֹһֱ����������һ���жϣ���������һ��ʱ�����һС��ʱ�䣬���ᴥ��
            if (cooldownDuration >= 0f)
            {
                return;
            }

            // ���Ӵ���������
            triggerNumber++;

            // ÿ�ﵽ10�δ�����������ȴʱ��
            if (triggerNumber > 10)
            {
                cooldownDuration = 0.5f;
                triggerNumber = 0; // ���ô���������
            }

            // �����ж��Ƿ���ս��Timer
            truceTimer = 5;

            // �����޵�֡�ڲ�����
            if (IsDefend) return;

            // �ܻ�����
            float getForce = hitStrength;
            rb.velocity = new Vector3(body.localScale.x < 0 ? getForce : -getForce, rb.velocity.y, rb.velocity.z);

            // �ܻ�����
            if (CalculateDefendProbability(hurtSource) && !IsBroken)
            {
                anim_body.SetTrigger("defend");
                if (!IsDefend)
                {
                    lastDefendTime = Time.time;
                }
                // �ܻ�����
                float dir = hurtSource.x - transform.position.x;
                body.transform.localScale = new Vector3(dir > 0 ? 1 : -1, 1, 1);
            }
            else
            {
                anim_body.SetTrigger("hurt");
                blood.Splatter();
                if (!IsBroken)
                {
                    lastHurtTime = Time.time;
                }
                // �˺���ʾ
                countDamaged += Random.Range(50, 100);
                AttackManager.Instance.ComboUp(attackBox);
            }

            // ��ͷЧ��
            if (hitStrength >= 1)
            {
                AttackManager.Instance.ChangeCameraFov(meleeAttackFov);
            }
            else
            {
                AttackManager.Instance.ChangeCameraFov(rangedAttackFov);
            }
        }

        private bool CalculateDefendProbability(Vector3 hurtSource)
        {
            float baseProbility = defendProbability;

            int dir = hurtSource.x - transform.position.x > 0 ? 1 : -1;

            // ���Ա���Ĺ���������ϵ�����Ͱٷ�֮30
            if (dir != Facing)
            {
                baseProbility *= 0.3f;
            }
            // ��������ǹ���״̬������ϵ�����Ͱٷ�֮30
            if (anim_body.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
            {
                baseProbility *= 0.3f;
            }
            return UnityEngine.Random.value < baseProbility;
        }

        IEnumerator PopupDamage()
        {
            while (true)
            {
                if (countDamaged != 0)
                {
                    GameObject popup = Instantiate(popupTextPrefab, popupTextBirthplace.position, Quaternion.identity);
                    Text text = popup.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
                    popup.transform.localScale = body.transform.localScale;
                    text.transform.localScale = body.transform.localScale;
                    text.text = countDamaged.ToString();

                    Hp -= countDamaged;

                    countDamaged = 0;

                    Destroy(popup, 1);
                }

                yield return new WaitForSeconds(popupTextFlashTime);
            }
        }
        #endregion

        #region - Life -
        private void Start()
        {
            InitComponents();

            Hp = MaxHealth;
        }
        private void Update()
        {
            if(truceTimer > 0)
            {
                truceTimer -= Time.deltaTime;
                if(truceTimer <= 0)
                {
                    AttackManager.Instance.ChangeCameraFov(truceFov);
                }
            }

            if (transform.position.y < 2f)
            {
                Debug.LogWarning("��ģ��");
                transform.position = new Vector3(transform.position.x, 2.17f, transform.position.z);
            }

            if (cooldownDuration >= 0f)
            {
                cooldownDuration -= Time.deltaTime;
            }

        }
        #endregion

        private void InitComponents()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Rigidbody ���δ�ҵ���");
            }

            body = transform.Find("Body");
            if (body == null)
            {
                Debug.LogError("Body ���δ�ҵ���");
            }
            else
            {
                anim_body = body.GetComponent<Animator>();
                if (anim_body == null)
                {
                    Debug.LogError("Animator ���δ�ҵ���");
                }
            }

            blood = GetComponentInChildren<BloodSplatteringEffect>();
            if (blood == null)
            {
                Debug.LogError("HurtBlood ���δ�ҵ���");
            }

            popupTextPrefab = Resources.Load<GameObject>(s_popupTextPrefabPath);
            if (popupTextPrefab == null)
            {
                Debug.LogError($"�޷�����Ԥ���壺{s_popupTextPrefabPath}");
            }

            popupTextBirthplace = transform.Find("Popup");
            if (popupTextBirthplace == null)
            {
                Debug.LogError("Popup ���Ϊ�գ���Ҫ����");
            }

            if (popupTextPrefab != null && popupTextBirthplace != null)
            {
                StartCoroutine(PopupDamage());
            }
            else
            {
                Debug.LogError("PopupDamage Э��δ�������� Popup �����Ԥ����Ϊ�ա�");
            }

            dartSpawner = GetComponentInChildren<DartSpawner>();
            if (dartSpawner == null)
            {
                Debug.LogError("DartSpawner ���Ϊ�գ���Ҫ����");
            }
        }

        private bool CheckIfGrounded()
        {
            return Physics.CheckCapsule(foot.bounds.center,
                                        new Vector3(foot.bounds.center.x, foot.bounds.min.y, foot.bounds.center.z),
                                        foot.bounds.extents.x,
                                        ground);
        }
    }
}