using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_Core
{
    public class Samurai : MonoBehaviour
    {
        // 公有属性 ----------------------------------------------------------------
        [Header("最大生命值")]
        public float MaxHealth = 100;
        [Header("防御概率")]
        public float defendProbability = 0.7f;
        [Header("阶段百分比")]
        [Range(0f, 1f)] public float firstStagePercentage = 0.9f;
        [Range(0f, 1f)] public float secondStagePercentage = 0.7f;
        [Range(0f, 1f)] public float thirdStagePercentage = 0.5f;

        // 必要组件 -----------------------------------------------------------------
        private Rigidbody rb;
        private Animator anim_body;
        private Transform body;
        private BloodSplatteringEffect blood;
        [HideInInspector] public DartSpawner dartSpawner;

        // 伤害跳字 ------------------------------------------------------------------
        private static readonly string s_popupTextPrefabPath = "PopupDamage/Popup";
        private GameObject popupTextPrefab;
        private Transform popupTextBirthplace;
        private float countDamaged = 0;                 // 短时间伤害累计计算
        private float popupTextFlashTime = 0.2f;        // 伤害跳字刷新时间

        private float lastHurtTime = 0;
        private float lastDefendTime = 0;
        private float hitCooldownTime = 0.5f;           // 防御无敌帧时间
        private float breakDefenseHitTime = 1f;          // 破防时间

        [Header("受击视角缩放变化")]
        [SerializeField] private float meleeAttackFov = 50;     //近战
        [SerializeField] private float rangedAttackFov = 80;    //远程
        [SerializeField] private float truceFov = 60;           //休战
        private float truceTimer = 0;

        [Header("脚")]
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
        /// 破防中
        /// </summary>
        public bool IsBroken => Time.time - lastHurtTime < breakDefenseHitTime;
        /// <summary>
        /// 防御中
        /// </summary>
        public bool IsDefend => Time.time - lastDefendTime < hitCooldownTime;
        /// <summary>
        /// 站在地上，有受力点
        /// </summary>
        public bool IsOnGround => CheckIfGrounded();
        /// <summary>
        /// 可以行动, 闲置，站着，没死
        /// </summary>
        public bool CanAction => anim_body.GetCurrentAnimatorStateInfo(0).IsName("Idle") && IsOnGround;
        /// <summary>
        /// 面朝方向
        /// </summary>
        public int Facing => body.localScale.x > 0 ? 1 : -1;
        /// <summary>
        /// 身体组件
        /// </summary>
        public GameObject Body => body.gameObject;
        /// <summary>
        /// 阶段
        /// </summary>
        public int CurrentStage
        {
            get
            {
                if (Hp > MaxHealth * firstStagePercentage)
                {
                    // 初始阶段
                    return 0;
                }
                if (Hp > MaxHealth * secondStagePercentage)
                {
                    // 解锁瞬移
                    return 1;
                }
                if (Hp > MaxHealth * thirdStagePercentage)
                {
                    // 解锁技能
                    return 2;
                }
                // 狂暴
                return 3;
            }
        }
        /// <summary>
        /// 等待时间缩短倍数
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
            // 防止一直被攻击，加一个判断，连续触发一段时间后有一小段时间，不会触发
            if (cooldownDuration >= 0f)
            {
                return;
            }

            // 增加触发计数器
            triggerNumber++;

            // 每达到10次触发，设置冷却时间
            if (triggerNumber > 10)
            {
                cooldownDuration = 0.5f;
                triggerNumber = 0; // 重置触发计数器
            }

            // 设置判断是否脱战的Timer
            truceTimer = 5;

            // 防御无敌帧内不触发
            if (IsDefend) return;

            // 受击力度
            float getForce = hitStrength;
            rb.velocity = new Vector3(body.localScale.x < 0 ? getForce : -getForce, rb.velocity.y, rb.velocity.z);

            // 受击动画
            if (CalculateDefendProbability(hurtSource) && !IsBroken)
            {
                anim_body.SetTrigger("defend");
                if (!IsDefend)
                {
                    lastDefendTime = Time.time;
                }
                // 受击方向
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
                // 伤害显示
                countDamaged += Random.Range(50, 100);
                AttackManager.Instance.ComboUp(attackBox);
            }

            // 镜头效果
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

            // 来自背面的攻击，防御系数降低百分之30
            if (dir != Facing)
            {
                baseProbility *= 0.3f;
            }
            // 如果现在是攻击状态，防御系数降低百分之30
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
                Debug.LogWarning("穿模了");
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
                Debug.LogError("Rigidbody 组件未找到！");
            }

            body = transform.Find("Body");
            if (body == null)
            {
                Debug.LogError("Body 物件未找到！");
            }
            else
            {
                anim_body = body.GetComponent<Animator>();
                if (anim_body == null)
                {
                    Debug.LogError("Animator 组件未找到！");
                }
            }

            blood = GetComponentInChildren<BloodSplatteringEffect>();
            if (blood == null)
            {
                Debug.LogError("HurtBlood 组件未找到！");
            }

            popupTextPrefab = Resources.Load<GameObject>(s_popupTextPrefabPath);
            if (popupTextPrefab == null)
            {
                Debug.LogError($"无法加载预制体：{s_popupTextPrefabPath}");
            }

            popupTextBirthplace = transform.Find("Popup");
            if (popupTextBirthplace == null)
            {
                Debug.LogError("Popup 物件为空，需要设置");
            }

            if (popupTextPrefab != null && popupTextBirthplace != null)
            {
                StartCoroutine(PopupDamage());
            }
            else
            {
                Debug.LogError("PopupDamage 协程未启动，因 Popup 物件或预制体为空。");
            }

            dartSpawner = GetComponentInChildren<DartSpawner>();
            if (dartSpawner == null)
            {
                Debug.LogError("DartSpawner 物件为空，需要设置");
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