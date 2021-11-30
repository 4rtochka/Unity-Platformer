using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed;
    public float jumpHeigth;
    public Transform groundCheck;
    public bool isGrounded;//на земле проверка
    Animator anim;//переменая для быстрого обращения к функции аниматор
    int curHp;//здоровье
    int maxHp=3;//макс количество очков здоровья
    bool isHit;//урон получен
    public Main main;//
    public bool key = false;//есть ключ
    public bool canTP = true;//может телепортироватьться
    public bool inWater = false;//в воде
    bool isClimbing = false;//на леснице 
    int coins = 0;//монетки изначальное число
    public GameObject BlueGem, GreenGem;
    int countGem = 0;
    bool canHit = true;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        curHp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        if (inWater && !isClimbing)
        {
            anim.SetInteger("State", 4);
            isGrounded = true;
            if (Input.GetAxis("Horizontal") != 0)
                Flip();
        }
        else
        {

            CheckGround();
            if (Input.GetAxis("Horizontal") == 0 && (isGrounded&& !isClimbing))
            {
                anim.SetInteger("State", 1);
            }
            else
            {
                Flip();
                if (isGrounded && !isClimbing)
                    anim.SetInteger("State", 2);
            }
        }  
    }
    //функция фиксированого обновления работает на физику 
    void FixedUpdate()
    {
        rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, rb.velocity.y);
if(Input.GetKeyDown(KeyCode.Space)&& isGrounded == true)
        rb.AddForce(transform.up * jumpHeigth, ForceMode2D.Impulse);
        
    } 
    //метод поворот
    void Flip() 
    {
        if(Input.GetAxis("Horizontal") > 0)
         transform.rotation = Quaternion.Euler(0, 0, 0);

        if (Input.GetAxis("Horizontal") < 0)
            transform.rotation = Quaternion.Euler(0, 180, 0);


    }
    //метка для проверки на земле игрок или нет
    void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.2f);
        isGrounded = colliders.Length > 1;
        if (!isGrounded&& !isClimbing)
        {
            anim.SetInteger("State", 3);
        }
    }

    //метод очков здоровья игрока
    public void RecountHp(int deltaHp)
    {
        curHp = curHp + deltaHp;
        if (deltaHp < 0 && canHit)
        {
            StopCoroutine(OnHit());
            isHit = true;

            StartCoroutine(OnHit());
        }
        else if (curHp > maxHp)
        {
            curHp = maxHp;
        }

        print(curHp);
        if (curHp <= 0)
        {
            GetComponent<CapsuleCollider2D>().enabled = false;
            Invoke("Lose",2f);
        }
       
    }
    //коррутина получения урона игроком
IEnumerator OnHit()
    {
        if(isHit)
        
        GetComponent<SpriteRenderer>().color = new Color(1f,GetComponent<SpriteRenderer>().color.g - 0.04f,  GetComponent<SpriteRenderer>().color.b - 0.04f);
        else  
            GetComponent<SpriteRenderer>().color = new Color(1f,GetComponent<SpriteRenderer>().color.g + 0.04f,  GetComponent<SpriteRenderer>().color.b + 0.04f);
    
        if (GetComponent<SpriteRenderer>().color.g == 1)
            StopCoroutine(OnHit());

        if (GetComponent<SpriteRenderer>().color.g <= 0)
            isHit = false;
        
        yield return new WaitForSeconds(0.04f);
        StartCoroutine(OnHit());
    }
    void Lose()
    {
       main.GetComponent<Main>().Lose();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //ключ уничтожиться и появиться у игрока
        if (collision.gameObject.tag == "Key")
        {
            Destroy(collision.gameObject);
            key = true;
        }

        //дверь телепорт
        if (collision.gameObject.tag == "Door")
        {
            if (collision.gameObject.GetComponent<Door>().isOpen && canTP)
            {
                collision.gameObject.GetComponent<Door>().Teleport(gameObject);
                canTP = false;
                StartCoroutine(TPwait());
            }
            else if (key)
                collision.gameObject.GetComponent<Door>().Unlock();
        }


        //монетки
        if (collision.gameObject.tag == "Coin")
        {
            Destroy(collision.gameObject);
            coins++;
            print("Количество равно  " + coins);
        }
        //сердечки
        if (collision.gameObject.tag == "Hearth")
        {
            Destroy(collision.gameObject);
            RecountHp(1);
            
        }
        //гриб
        if (collision.gameObject.tag == "mushroom")
        {
            Destroy(collision.gameObject);
            RecountHp(-1);

        }
        //ускорение
        if (collision.gameObject.tag == "GreenGem")
        {
            Destroy(collision.gameObject);
            StartCoroutine(speedUp());

        }
        //бесмертие
        if (collision.gameObject.tag == "BlueGem")
        {
            Destroy(collision.gameObject);
            StartCoroutine(noHit());

        }
        
    }

    IEnumerator TPwait()
    {
        yield return new WaitForSeconds(1f);
        canTP = true;
    }

    //лесница выключает гравитацию игрока
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ladder")
        {
            isClimbing = true;

            if (Input.GetAxis("Vertical") == 0)
            {
                anim.SetInteger("State", 5);
            }
            else
            {
                transform.Translate(Vector3.up * Input.GetAxis("Vertical") * speed * 0.5f * Time.deltaTime);
                anim.SetInteger("State", 6);
            }
            
            
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        }
    }
    //покинув лесницу включает гравитацию игрока
    private void OnTriggerExit2D(Collider2D collision)
    {
        isClimbing = false;
        
        if (collision.gameObject.tag == "Ladder")
        {
           
            transform.Translate(Vector3.up * Input.GetAxis("Vertical") * speed * 0.5f * Time.deltaTime);
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
    }
    //кнопка пружина меняет спрайт при нажатии 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Trampoline")
        {
            StartCoroutine(TrampolineAnim(collision.gameObject.GetComponentInParent<Animator>()));
        }
    }
    //корутин для смены спрайта пружины
  IEnumerator TrampolineAnim(Animator an)
    {
        an.SetBool("isJump", true);
        yield return new WaitForSeconds(0.5f);
        an.SetBool("isJump", false);
    }
    //корутин бонус ускорения 
    IEnumerator speedUp()
    {
        countGem++;
        GreenGem.SetActive(true);
        checkGems(GreenGem);
        GreenGem.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        speed = speed * 2;
        print("ускорение включенно ");
        yield return new WaitForSeconds(4f);
        StartCoroutine(Invis(GreenGem.GetComponent<SpriteRenderer>(), 0.02f));
        yield return new WaitForSeconds(1f);
        countGem--;
        GreenGem.SetActive(false);
        checkGems(BlueGem);
        speed = speed / 2;
        print("ускорение законченно ");
    }
    
    //корутин бонус бессмертия 
    IEnumerator noHit()
    {
        countGem++;
        BlueGem.SetActive(true);
        checkGems(BlueGem);

        canHit = false;
        BlueGem.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

        print("бессмертие включенно ");
        yield return new WaitForSeconds(3f);
        StartCoroutine(Invis(BlueGem.GetComponent<SpriteRenderer>(), 0.02f));
        yield return new WaitForSeconds(1f);

        countGem--;
        BlueGem.SetActive(false);
        checkGems(GreenGem);
        canHit = true;
        print("не бессмертен ");
    }

   void checkGems(GameObject obj)
    {
        if (countGem == 1)
        {
            obj.transform.localPosition = new Vector3(0f, 0.6f, obj.transform.localPosition.z);
        }
        else if (countGem == 2)
        {
            BlueGem.transform.localPosition = new Vector3(0.5f, 0.6f, BlueGem.transform.localPosition.z);
            GreenGem.transform.localPosition = new Vector3(-0.5f, 0.6f, GreenGem.transform.localPosition.z);
        }

    }
    IEnumerator Invis(SpriteRenderer spr , float time)
    {
        spr.color = new Color(1f, 1f, 1f, spr.color.a - time * 2);
        yield return new WaitForSeconds(time);
        if (spr.color.a > 0)
            StartCoroutine(Invis(spr, time));
    }
}



