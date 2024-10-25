using ChenChen_Core.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChenChen_Core
{
    public class RedHood : MonoBehaviour
    {
        public LayerMask ground;

        [Header("����")]
        public KeyCode jumpKey = KeyCode.Space;
        public KeyCode ligthHitOrBowHitKey = KeyCode.J;
        public KeyCode heavyHitKey = KeyCode.K;
        public KeyCode slidingKey = KeyCode.C;
        public KeyCode dodgeKey = KeyCode.L;
        public KeyCode keySkillFirst = KeyCode.U;
        public KeyCode keySkillSecond = KeyCode.I;
        public KeyCode keySkillSpecialSkill = KeyCode.O;

        // �������� ------------------------------------------------------------
        [Header("�������ֵ")]
        public float MaxHealth = 100;
        [Header("�������ֵ")]
        public float MaxPower = 100;
        [Header("�ƶ��ٶ�")]
        public float MoveSpeed;
        [Header("��Ծ��")]
        public float jumpForceSmall;
        public float jumpForceBig;
        [Header("����ǿ��")]
        public float ligthHitStrength = 1.1f;
        public float heavyHitStrength = 1.5f;
        [Header("�����ٶ�")]
        public float slidingSpeed = 15f;
        [Header("����")]
        public float dodgeDuration = 0.5f;          
        public float dodgeCooldownTime = 0.6f;  
        public float dodgeSpeed = 15f;                    
        [Header("����")]
        public float hurtCooldownTime = 0.5f;
        [Header("��������������")]
        public float power_skill1 = 25;
        public float power_skill2 = 50;
        public float power_SpecialSkill = 75;

        // �ر���� ------------------------------------------------------------
        [HideInInspector] public Rigidbody rb;
        [HideInInspector] public Animator anim;
        [HideInInspector] public Transform body;
        [HideInInspector] public Collider bodyColl;
        [HideInInspector] public RedHoodFSM fsm;
        [HideInInspector] public RedHoodBow myBow;
        [HideInInspector] public PlayerAttackBox playerAttackBox;
        [HideInInspector] public AfterImageEffect aie;
        [HideInInspector] public Collider foot;
        [HideInInspector] public Transform bulletBirthplace;

        // ������ -----------------------------------------------------------

        public RHStateType CurrentStateType => fsm.CurrentStateType;

        public int Facing => body.transform.localScale.x > 0 ? 1 : -1;

        public bool OnFall => rb.velocity.y < -0.01f;

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

        [SerializeField] private float power = 0;
        public float Power
        {
            get
            {
                return power;
            }
            private set
            {
                power = Mathf.Clamp(value, 0, MaxPower);
            }
        }

        public int MoveTrend
        {
            get
            {
                if (InputHorizon > 0)
                {
                    return 1;
                }
                else if (InputHorizon < 0)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }

        private float inputHorizon = 0;
        private float InputHorizon
        {
            get
            {
                return inputHorizon;
            }
            set
            {
                inputHorizon = value;
                anim.SetFloat("speed", Mathf.Abs(value));
            }
        }

        private bool preJump;
        public bool PreJump
        {
            get
            {
                return preJump;
            }
            set
            {
                preJump = value;
                anim.SetBool("preJump", value);
            }
        }

        private bool onGround;
        public bool OnGround
        {
            get
            {
                return onGround;
            }
            set
            {
                onGround = value;
                anim.SetBool("onGround", onGround);
            }
        }

        [HideInInspector] public string attackType = string.Empty;
        [HideInInspector] public bool canAttack = true;
        private float lastComboTime;
        private float comboMaxInterval = 1.0f;
        private int comboStep = 0;
        public int ComboStep
        {
            get
            {
                if(Time.time - comboMaxInterval >= lastComboTime)
                {
                    comboStep = 0;
                }
                lastComboTime = Time.time;
                comboStep++;
                if(comboStep > 3)
                {
                    comboStep = 1;
                }
                return comboStep;
            }
            set
            {
                comboStep = value;
                lastComboTime = Time.time;
            } 
        }

        private Transform bulletTarget = null;
        public Transform BulletTarget
        {
            get
            {
                if (bulletTarget == null)
                {
                    var enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    var target = enemies.FirstOrDefault<GameObject>(x => x.TryGetComponent<Samurai>(out _));
                    if (target != null)
                    {
                        bulletTarget = target.transform;
                    }
                    else
                    {
                        Debug.LogError("Error not Find");
                    }
                }
                return bulletTarget;
            }
        }

        [Header("����")]
        [SerializeField] private float minBowHitTime;       // ���������Ҫ����С��סʱ��

        [Header("�ӵ�")]
        [SerializeField] private BulletSetting skillEffect1;
        [SerializeField] private BulletSetting skillEffect2;
        [SerializeField] private BulletSetting skillEffect3;
        [SerializeField] private BulletSetting skillEffect4;
        [SerializeField] private float skillEffect3_interval;

        private void Start()
        {
            InitComponent();

            bulletPool = new SimpleComponentPool<RedHoodBullet>(bulletPrefabPath, GetFromBulletPool, ReleaseToBulletPool, 100, 1000);

            Hp = MaxHealth;

            Power = 0;

            StartCoroutine(Skill_3(skillEffect3_interval,skillEffect3));
        }

        private void Update()
        {
            InputHorizon = Input.GetAxis("Horizontal");

            if (transform.position.y < 3f)
            {
                Debug.LogWarning("��ģ��");
                transform.position = new Vector3(transform.position.x, 3.3f, transform.position.z);
            }

            OnGround = CheckIfGrounded();

            if (CurrentStateType == RHStateType.Idle || CurrentStateType == RHStateType.Jump)
            {
                Move();
            }
            if (CurrentStateType == RHStateType.Idle)
            {
                Jump();
                Sliding();
                Dodge();
                Skill();
            }
            if (CurrentStateType == RHStateType.Idle)
            {
                Attack();
            } 
            if (canAttack && (CurrentStateType == RHStateType.LightHit || CurrentStateType == RHStateType.HeavyHit))
            {
                Attack();
            }          
        }

        #region - Jump -
        private float jumpTimer = 0;
        private float bigJumpRequiredTime = 0.2f;
        private void Jump()
        {
            if (Input.GetKeyDown(jumpKey) && OnGround)
            {
                PreJump = true;
                jumpTimer = 0;
            }

            if (Input.GetKey(jumpKey) && OnGround)
            {
                jumpTimer += Time.deltaTime;
            }

            if (Input.GetKeyUp(jumpKey) && PreJump)
            {
                if(jumpTimer < bigJumpRequiredTime)
                {
                    rb.velocity += new Vector3(0, jumpForceSmall, 0);
                }
                else
                {
                    rb.velocity += new Vector3(0, jumpForceBig, 0);
                }

                PreJump = false;
                fsm.ChangeState("Air");
            }

        }
        #endregion

        #region - Move -
        private void Move()
        {
            if (PreJump) return;

            if (InputHorizon != 0f)
            {
                if (InputHorizon < 0)
                    body.localScale = new Vector3(-1, 1, 1);
                else
                    body.localScale = new Vector3(1, 1, 1);

                rb.velocity = new Vector3(InputHorizon * MoveSpeed, rb.velocity.y, rb.velocity.z);
            }
        }
        #endregion

        #region - Attack -
        private float downLigthHitOrBowHitKeyTime = -1;      // ��ס��λ��ʱ��
        public int bowState = 0;

        private void Attack()
        {
            if (Input.GetKey(ligthHitOrBowHitKey) && bowState == 0)
            {
                downLigthHitOrBowHitKeyTime = Time.time;
                bowState = 1;
            }

            if (Input.GetKey(ligthHitOrBowHitKey) && bowState == 1)
            {
                // ����ﵽ����Ҫ���ʱ�䣬��������״̬
                if (Time.time - downLigthHitOrBowHitKeyTime >= minBowHitTime)
                {
                    fsm.ChangeState("BowHit");
                    return;
                }
            }

            if (Input.GetKeyUp(ligthHitOrBowHitKey) && CurrentStateType != RHStateType.BowHit && bowState == 1)
            {
                // δ��������״̬, ��Ϊ�ṥ��
                fsm.ChangeState("LightHit");
            }

            if (Input.GetKeyDown(heavyHitKey) && CurrentStateType != RHStateType.BowHit)
            {
                fsm.ChangeState("HeavyHit");
            }
        }

        // �����ڶ��������жϹ�������
        public void AttackOver()
        {
            canAttack = true;
        }
        #endregion

        #region - Sliding -
        private void Sliding()
        {
            if (Input.GetKeyDown(slidingKey))
            {            
                fsm.ChangeState("Sliding");
            }
        }
        #endregion

        #region - Dodge -
        private bool canDodge = true;

        private void Dodge()
        {
            if (Input.GetKeyDown(dodgeKey) && canDodge)
            {
                StartCoroutine(DodgeCoolDownClock());
                aie.OpenEffect(dodgeDuration);
                fsm.ChangeState("Dodge");
            }
        }

        IEnumerator DodgeCoolDownClock()
        {
            canDodge = false;
            yield return new WaitForSeconds(dodgeCooldownTime); 
            canDodge = true;
        }
        #endregion

        #region - Skill -
        private void Skill()
        {
            // ��һ�У���ָ���Ƿ�Χ����һ���������εĵ�Ļ
            if (Input.GetKeyDown(keySkillFirst) && Power >= power_skill1)
            {
                Power -= power_skill1;
                Skill_1(skillEffect1);
            }

            // �ڶ��У���ָ���Ƿ���һ�������ĵ�Ļ���������ͣ��
            if (Input.GetKeyDown(keySkillSecond) && Power >= power_skill2)
            {
                Power -= power_skill2;
                Skill_2(skillEffect2);
            }
        }

        private void Skill_1(BulletSetting effect)
        {
            float unitAngle = Mathf.Abs(effect.angleStart - effect.angleEnd) / effect.num;

            float angleBegin = effect.angleStart;

            for (int i = 0; i < effect.num; i++)
            {
                var bullet = bulletPool.Get();
                var rot = angleBegin + i * unitAngle;
                bullet.transform.Rotate(0, 0, rot);
                InitBullet(effect, bullet);
            }
        }

        private void Skill_2(BulletSetting effect)
        {
            float unitAngle = Mathf.Abs(effect.angleStart - effect.angleEnd) / (effect.num / effect.count);

            float angleBegin = effect.angleStart;

            for (int i = 0; i < effect.num; i++)
            {
                var rot = angleBegin + i * unitAngle;

                for (int j = 0; j < effect.count; j++)
                {
                    var bullet = bulletPool.Get();
                    bullet.transform.Rotate(0, 0, rot);
                    InitBullet(effect, bullet);
                }
            }
        }

        IEnumerator Skill_3(float interval, BulletSetting effect)
        {
            while (true)
            {
                yield return null;

                if (Input.GetKeyDown(keySkillSpecialSkill) && Power >= power_SpecialSkill)
                {
                    fsm.ChangeState("SpecialSkill");
                }

                if (CurrentStateType == RHStateType.SpecialSkill)
                {
                    anim.SetBool("Skill", true);

                    for (int i = 0; i < effect.num; i++)
                    {
                        var rot = UnityEngine.Random.Range(effect.angleStart, effect.angleEnd);
                        var bullet = bulletPool.Get();
                        bullet.transform.Rotate(0, 0, rot);
                        InitBullet(effect, bullet);
                    }

                    Power -= 5;

                    yield return new WaitForSeconds(interval);
                }
                else
                {
                    anim.SetBool("Skill", false);
                }
            }
        }

        private void Skill_4(BulletSetting effect)
        {
            float unitAngle = Mathf.Abs(effect.angleStart - effect.angleEnd) / (effect.num / effect.count);

            float angleBegin = effect.angleStart;

            for (int i = 0; i < effect.num; i++)
            {
                var rot = angleBegin + i * unitAngle;

                for (int j = 0; j < effect.count; j++)
                {
                    var bullet = bulletPool.Get();
                    bullet.transform.Rotate(0, 0, rot);
                    InitBullet(effect, bullet);
                }
            }
        }

        private static void InitBullet(BulletSetting effect, RedHoodBullet bullet)
        {
            bullet.speed = effect.speed;
            bullet.initialMoveDuration = effect.initialMoveDuration;
            bullet.lerpArgument = effect.lerpArgument;

            bullet.useRandomStop = effect.useRamdomStop;
            bullet.randomStopStrengh = effect.ramdomStopStrength;
        }

        #endregion

        #region - Hurt -

        [HideInInspector] public bool canGetHurt = true;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("EnemyAttackBox") && canGetHurt)
            {
                // ����ʱ���޵е�
                if (CurrentStateType == RHStateType.Jump)
                {
                    return;
                }

                // ����ʱ�޷�ѡ��
                if (CurrentStateType == RHStateType.DodgeLeft || CurrentStateType == RHStateType.DodgeRight)
                {
                    return;
                }

                EnemyAttackBox enemyAttackBox = other.GetComponent<EnemyAttackBox>();
                if (enemyAttackBox != null)
                {
                    StartCoroutine(AttackJuggleCo(enemyAttackBox));
                }
                else
                {
                    Debug.LogError("ȱʧ���������");
                }         
            }
        }

        private IEnumerator AttackJuggleCo(EnemyAttackBox enemyAttackBox)
        {
            if (enemyAttackBox == null)
            {
                yield break;
            }

            float timer = enemyAttackBox.allowDodgeTime;

            while (timer > 0)
            {
                if(CurrentStateType == RHStateType.DodgeLeft || CurrentStateType == RHStateType.DodgeRight)
                {
                    AttackManager.Instance.SuddenCamera(dodgeDuration);
                    Power += MaxPower / 4;
                    yield break;
                }

                timer -= Time.deltaTime;
                yield return null;
            }

            if (enemyAttackBox == null)
            {
                yield break;
            }

            // δ���ܣ�ʧ�ܣ��˺��ж�
            if (enemyAttackBox.canShootDown)
            {
                if (CurrentStateType == RHStateType.LightHit ||
                    CurrentStateType == RHStateType.HeavyHit ||
                    CurrentStateType == RHStateType.SpecialSkill)
                {
                    enemyAttackBox.ShoorDown();
                    yield break;
                }
            }

            if (enemyAttackBox.parent != null && enemyAttackBox.parent.TryGetComponent<Samurai>(out Samurai samurai))
            {
                // �������Ϊ��ҵĹ�������ײ�����ˣ��ж������ڷ���������״̬
                if (samurai.IsDefend || samurai.IsBroken)
                {
                    yield break;
                }
            }

            Hp -= enemyAttackBox.damage;
            fsm.ChangeState("Hurt");
        }

        #endregion

        #region - Bullet Pool -

        private SimpleComponentPool<RedHoodBullet> bulletPool;
        private static readonly string bulletPrefabPath = "Items/Bullet/Bullet";

        private void GetFromBulletPool(RedHoodBullet bullet)
        {
            bullet.pool = bulletPool;
            bullet.gameObject.SetActive(true);
            bullet.target = BulletTarget;
            bullet.transform.position = bulletBirthplace.position;
        }

        private void ReleaseToBulletPool(RedHoodBullet bullet)
        {
            bullet.gameObject.SetActive(false);           
            bullet.transform.rotation = Quaternion.identity;
        }

        #endregion

        private bool CheckIfGrounded()
        {
            return Physics.CheckCapsule(foot.bounds.center,
                                        new Vector3(foot.bounds.center.x, foot.bounds.min.y, foot.bounds.center.z),
                                        foot.bounds.extents.x,
                                        ground);
        }

        private void InitComponent()
        {
            rb = GetComponent<Rigidbody>();
            body = transform.GetChild(0);
            anim = body.GetComponent<Animator>();
            bodyColl = body.GetComponent<Collider>();
            playerAttackBox = body.GetChild(0).GetComponent<PlayerAttackBox>();
            fsm = GetComponent<RedHoodFSM>();
            myBow = GetComponentInChildren<RedHoodBow>();
            aie = GetComponentInChildren<AfterImageEffect>();
            foot = transform.Find("Foot").GetComponent<Collider>();
            bulletBirthplace = transform.Find("��Ļ");
        }
    }
}
