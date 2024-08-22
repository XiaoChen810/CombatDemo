using ChenChen_Core;
using UnityEngine;

public abstract class RedHood_Base : IRedHoodState
{
    protected RedHoodFSM fsm;
    protected RedHood redHood;

    public RedHood_Base(RedHoodFSM fsm, RedHood redHood)
    {
        this.fsm = fsm;
        this.redHood = redHood;
    }

    public abstract float MaxDuration { get; }
    public abstract RHStateType StateType { get; }

    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public virtual void OnUpdate() { }
}

// 空闲态，为默认状态
public class RedHood_Idle : RedHood_Base
{
    public RedHood_Idle(RedHoodFSM fsm, RedHood redHood) : base(fsm, redHood)
    {
    }

    public override float MaxDuration => -1;

    public override RHStateType StateType => RHStateType.Idle;

    public override void OnEnter()
    {
        AnimatorStateInfo info = redHood.anim.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("Idle"))
        {
            redHood.anim.Play("Idle");
        }
    }
}

// 在空中的状态
public class RedHood_Air : RedHood_Base
{
    public RedHood_Air(RedHoodFSM fsm, RedHood redHood) : base(fsm, redHood)
    {
    }

    public override float MaxDuration => -1;

    public override RHStateType StateType => redHood.OnFall ? RHStateType.Fall : RHStateType.Jump;

    private bool canDoubleJump = true;

    public override void OnEnter()
    {
        redHood.anim.SetBool("jump", true);
        canDoubleJump = true;
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(redHood.jumpKey) && canDoubleJump)
        {
            redHood.rb.velocity = new Vector3(redHood.rb.velocity.x, redHood.jumpForceSmall, redHood.rb.velocity.z);
            canDoubleJump = false;
        }
        if (redHood.OnFall)
        {
            redHood.anim.SetBool("fall", true);
        }
        if (redHood.OnGround && redHood.OnFall)
        {
            redHood.anim.SetBool("jump", false);
            redHood.anim.SetBool("fall", false);
            fsm.ChangeState("Idle");
        }
    }

    public override void OnExit()
    {
        // 将动画立刻回到默认状态
        redHood.anim.SetBool("jump", false);
        redHood.anim.SetBool("fall", false);

        redHood.anim.Play("Idle", -1, 0f);
    }

}

// 轻攻击
public class RedHood_LightHit : RedHood_Base
{
    public RedHood_LightHit(RedHoodFSM fsm, RedHood redHood) : base(fsm, redHood)
    {
    }

    public override float MaxDuration => 1.5f;
    public override RHStateType StateType => RHStateType.LightHit;

    private int comboStep = 0;

    public override void OnEnter()
    {
        if (redHood.attackType == "LigthHit")
        {
            comboStep = redHood.ComboStep;
        }
        else
        {
            redHood.attackType = "LigthHit";
            comboStep = 1;
            redHood.ComboStep = 1;
        }
        redHood.playerAttackBox.HitStrength = redHood.ligthHitStrength;
        redHood.anim.SetTrigger("lightAttack");
        redHood.anim.SetInteger("comboStep", comboStep);

        redHood.rb.velocity = Vector3.zero;

        redHood.canAttack = false;

        if (redHood.MoveTrend != 0f)
        {
            if (redHood.MoveTrend < 0)
                redHood.body.localScale = new Vector3(-1, 1, 1);
            else
                redHood.body.localScale = new Vector3(1, 1, 1);
        }

        redHood.bowState = 0;
    }

    public override void OnUpdate()
    {
        AnimatorStateInfo info = redHood.anim.GetCurrentAnimatorStateInfo(0);

        if (info.IsName("Idle"))
        {
            redHood.canAttack = true;
            fsm.ChangeState("Idle");
        }
    }
}

// 重攻击
public class RedHood_HeavyHit : RedHood_Base
{
    public RedHood_HeavyHit(RedHoodFSM fsm, RedHood redHood) : base(fsm, redHood)
    {
    }

    public override float MaxDuration => 1.5f;

    public override RHStateType StateType => RHStateType.HeavyHit;

    private int comboStep = 0;

    public override void OnEnter()
    {
        if (redHood.attackType == "HeavyHit")
        {
            comboStep = redHood.ComboStep;
        }
        else
        {
            redHood.attackType = "HeavyHit";
            comboStep = 1;
            redHood.ComboStep = 1;
        }
        redHood.playerAttackBox.HitStrength = redHood.heavyHitStrength;
        redHood.anim.SetTrigger("heavyAttack");
        redHood.anim.SetInteger("comboStep", comboStep);

        redHood.rb.velocity = Vector3.zero;

        redHood.canAttack = false;

        if (redHood.MoveTrend != 0f)
        {
            if (redHood.MoveTrend < 0)
                redHood.body.localScale = new Vector3(-1, 1, 1);
            else
                redHood.body.localScale = new Vector3(1, 1, 1);
        }

        redHood.bowState = 0;
    }

    public override void OnUpdate()
    {
        AnimatorStateInfo info = redHood.anim.GetCurrentAnimatorStateInfo(0);

        if (info.IsName("Idle"))
        {
            redHood.canAttack = true;
            fsm.ChangeState("Idle");
        }
    }
}

// 弓箭攻击
public class RedHood_BowHit : RedHood_Base
{
    public RedHood_BowHit(RedHoodFSM fsm, RedHood redHood) : base(fsm, redHood)
    {
    }

    public override float MaxDuration => 3f;

    public override RHStateType StateType => RHStateType.BowHit;

    private Animator anim = null;
    private RedHoodBow bow = null;
    private KeyCode listenKey = KeyCode.None;

    private bool isShoot = false;

    public override void OnEnter()
    {
        anim = redHood.anim;
        bow = redHood.myBow;
        listenKey = redHood.ligthHitOrBowHitKey;

        anim.SetTrigger("bowAttack");
        anim.SetBool("drawBow", true);
        bow.DrawBow();

        redHood.rb.velocity = Vector3.zero;
        isShoot = false;

        redHood.canAttack = false;
        redHood.bowState = 2;
    }

    public override void OnUpdate()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        // 当动画已经播放超过95%并且没有按住键位时
        if (info.normalizedTime > 0.95f && !Input.GetKey(listenKey) && !isShoot)
        {
            // 发射
            bow.LooseBow();
            anim.SetBool("drawBow", false);
            isShoot = true;

            // 结束
            fsm.ChangeState("Idle");
        }
    }

    public override void OnExit()
    {
        if (!isShoot)
        {
            bow.DestroyArrow();
        }

        redHood.canAttack = true;
        redHood.bowState = 0;
    }
}

// 滑铲
public class RedHood_Sliding : RedHood_Base
{
    public RedHood_Sliding(RedHoodFSM fsm, RedHood redHood) : base(fsm, redHood)
    {
    }

    private float slidingDurationTime = 0.7f;

    public override float MaxDuration => slidingDurationTime;

    public override RHStateType StateType => RHStateType.Slide;

    public override void OnEnter()
    {
        redHood.rb.velocity = new Vector3(redHood.slidingSpeed * redHood.Facing, 0, 0);
        redHood.rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        //redHood.bodyColl.isTrigger = true;
        redHood.anim.SetBool("sliding", true);
    }

    public override void OnExit()
    {
        //redHood.bodyColl.isTrigger = false;
        redHood.rb.velocity = Vector3.zero;
        redHood.rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        redHood.anim.SetBool("sliding", false);
    }
}

// 闪避
public class RedHood_Dodge : RedHood_Base
{
    public RedHood_Dodge(RedHoodFSM fsm, RedHood redHood) : base(fsm, redHood)
    {
    }

    public override float MaxDuration => redHood.dodgeDuration;

    public override RHStateType StateType => trend == -1 ? RHStateType.DodgeLeft : RHStateType.DodgeRight;

    private int trend = -1; // 1 或 -1。

    public override void OnEnter()
    {
        // 优先以输入方向为准，其次面向的相反方向为准
        if (redHood.MoveTrend != 0)
        {
            trend = redHood.MoveTrend;
        }
        else
        {
            trend = -redHood.Facing;
        }

        redHood.canGetHurt = false;

        redHood.rb.velocity = new Vector3(redHood.dodgeSpeed * trend, 0, 0);
        redHood.rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        redHood.anim.SetInteger("dodge", trend);
    }

    public override void OnUpdate()
    {
        if (Input.GetKey(redHood.ligthHitOrBowHitKey))
        {
            fsm.ChangeState("LightHit");
        }

        if (Input.GetKey(redHood.heavyHitKey))
        {
            fsm.ChangeState("HeavyHit");
        }
    }

    public override void OnExit()
    {
        redHood.canGetHurt = true;

        redHood.rb.velocity = Vector3.zero;
        redHood.rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        redHood.anim.SetInteger("dodge", 0);
    }
}

// 特殊技能
public class RedHood_SpecialSkill : RedHood_Base
{
    public RedHood_SpecialSkill(RedHoodFSM fsm, RedHood redHood) : base(fsm, redHood)
    {
    }

    public override float MaxDuration => -1;

    public override RHStateType StateType => RHStateType.SpecialSkill;

    public override void OnEnter()
    {
        redHood.anim.Play("Idle");
    }

    public override void OnUpdate()
    {
        if (!Input.GetKey(redHood.keySkillSpecialSkill))
        {
            fsm.ChangeState("Idle");
        }
    }
}

// 受伤
public class RedHood_Hurt : RedHood_Base
{
    public RedHood_Hurt(RedHoodFSM fsm, RedHood redHood) : base(fsm, redHood)
    {
    }

    public override float MaxDuration => redHood.hurtCooldownTime;

    public override RHStateType StateType => RHStateType.Hurt;

    public override void OnEnter()
    {
        redHood.anim.SetTrigger("hurt");
        redHood.canGetHurt = false;
    }

    public override void OnExit()
    {
        redHood.canGetHurt = true;
    }
}