using UnityEngine;
using UnityEngine.UI;

public class DualHealthBar : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    
    [Header("UI References")]
    public Slider frontHealthBar;  // 즉시 변화하는 체력바
    public Slider backHealthBar;   // 지연되어 변화하는 체력바
    
    [Header("Animation Settings")]
    public float delayTime = 1f;           // 백그라운드 바가 줄어들기 시작하는 지연 시간
    public float decreaseSpeed = 50f;      // 백그라운드 바가 줄어드는 속도
    
    private float displayHealth;           // 백그라운드 바의 현재 표시 값
    private float delayTimer;              // 지연 타이머
    private bool isDelaying = false;       // 지연 중인지 확인
    
    void Start()
    {
        // 초기 설정
        currentHealth = maxHealth;
        displayHealth = maxHealth;
        
        // 슬라이더 초기화
        if (frontHealthBar != null)
        {
            frontHealthBar.maxValue = maxHealth;
            frontHealthBar.value = currentHealth;
        }
        
        if (backHealthBar != null)
        {
            backHealthBar.maxValue = maxHealth;
            backHealthBar.value = displayHealth;
        }
    }
    
    void Update()
    {
        // 백그라운드 바 업데이트 로직
        UpdateBackHealthBar();

    }
    
    void UpdateBackHealthBar()
    {
        // 현재 체력이 표시 체력보다 낮을 때 (데미지를 받았을 때)
        if (currentHealth < displayHealth)
        {
            if (isDelaying)
            {
                // 지연 시간 계산
                delayTimer -= Time.deltaTime;
                
                if (delayTimer <= 0)
                {
                    // 지연 시간이 끝나면 백그라운드 바를 줄이기 시작
                    displayHealth -= decreaseSpeed * Time.deltaTime;
                    
                    // 현재 체력보다 낮아지지 않도록 제한
                    if (displayHealth <= currentHealth)
                    {
                        displayHealth = currentHealth;
                        isDelaying = false;
                    }
                }
            }
        }
        // 현재 체력이 표시 체력보다 높을 때 (힐을 받았을 때)
        else if (currentHealth > displayHealth)
        {
            displayHealth = currentHealth;
            isDelaying = false;
        }
        
        // UI 업데이트
        if (backHealthBar != null)
        {
            backHealthBar.value = displayHealth;
        }
    }
    
    public void TakeDamage(float damage)
    {
        // 체력 감소
        currentHealth = Mathf.Max(0, currentHealth - damage);
        
        // 즉시 프론트 바 업데이트
        if (frontHealthBar != null)
        {
            frontHealthBar.value = currentHealth;
        }
        
        // 백그라운드 바 지연 시작
        if (currentHealth < displayHealth)
        {
            delayTimer = delayTime;
            isDelaying = true;
        }
        
       // Debug.Log($"Damage: {damage}, Current Health: {currentHealth}");
    }
    
    public void Heal(float healAmount)
    {
        // 체력 회복
        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        
        // 즉시 두 바 모두 업데이트
        if (frontHealthBar != null)
        {
            frontHealthBar.value = currentHealth;
        }
        
        displayHealth = currentHealth;
        isDelaying = false;
        
        Debug.Log($"Heal: {healAmount}, Current Health: {currentHealth}");
    }

    public void SetCurValue( float val , bool display_eq = false )
    {
        currentHealth = val;
        if (frontHealthBar != null)
            frontHealthBar.value = val;
        if( display_eq )displayHealth = val;
    }
    
    public void SetMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = maxHealth;
        displayHealth = maxHealth;
        
        if (frontHealthBar != null)
        {
            frontHealthBar.maxValue = maxHealth;
            frontHealthBar.value = currentHealth;
        }
        
        if (backHealthBar != null)
        {
            backHealthBar.maxValue = maxHealth;
            backHealthBar.value = displayHealth;
        }
    }
    
    // 현재 체력 비율 반환 (0~1)
    public float GetHealthRatio()
    {
        return currentHealth / maxHealth;
    }
    
    // 체력이 0인지 확인
    public bool IsDead()
    {
        return currentHealth <= 0;
    }
}