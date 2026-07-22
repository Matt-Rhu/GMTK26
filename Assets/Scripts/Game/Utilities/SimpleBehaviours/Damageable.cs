using System;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{   
    [SerializeField] protected int maxHealth;
    public int MaxHealth
    {
        get => maxHealth;
        private set => maxHealth = value;
    }
    
    protected int currentHealth;
    public int Health
    {
        get => currentHealth;
        protected set => currentHealth = value;
    }

    [Space]
    public UnityEvent onDeath;
    public UnityEvent onDamaged;
    public UnityEvent onHealed;

    protected bool dead = false;
    public bool Dead
    {
        get => dead;
        protected set => dead = value;
    }
    
    [HideInInspector] public bool iframes;


    protected virtual void Start()
    {
        Health = MaxHealth;
    }
    
    //bool return type here is not super useful, it's for children classes
    public virtual bool TakeDamage(int dmg, bool bypassIframes = false)
    {
        if (dead) return false;
        if (iframes && !bypassIframes) return false;
        
        Health -= dmg;
        onDamaged.Invoke();

        if (Health <= 0)
            Death();

        return true;
    }

    protected virtual void Death()
    {
        onDeath.Invoke();
        dead = true;
    }

    public virtual void Heal(int heal)
    {
        if (Health >= MaxHealth || dead) return;
        
        Health = Mathf.Clamp(Health + heal, 0, MaxHealth);
        onHealed.Invoke();
    }

    public virtual void Kill()
    {
        if (dead) return;
        Death();
    }
}
