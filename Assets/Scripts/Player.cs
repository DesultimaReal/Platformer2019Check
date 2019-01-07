/********************************************************
 * 2D Meatboy style controller written entirely by Nyero.
 *
 * Thank you for using this script, it makes me feel all
 * warm and happy inside. ;)
 *                             -Nyero
 *
 * ------------------------------------------------------
 * Notes on usage:
 *     Please don't use the meatboy image, as your some
 * might consider it stealing.  Simply replace the sprite
 * used, and you'll have a 2D platform controller that is
 * very similar to meatboy.
 ********************************************************/
using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    private Rigidbody2D ourBody;

    public class GroundState
    {
        private GameObject player;
        
        private float width;
        private float height;
        private float length;

        //GroundState constructor.  Sets offsets for raycasting.
        public GroundState(GameObject playerRef)
        {
            player = playerRef;
            
            width = player.GetComponent<Collider2D>().bounds.extents.x;
            height = player.GetComponent<Collider2D>().bounds.extents.y;
            length = 0.5f;
        }

        //Returns whether or not player is touching wall.
        public bool isWall()
        {
            bool tLeft = Physics2D.Raycast(new Vector2(player.transform.position.x - width, player.transform.position.y + height), -Vector2.right, length);
            bool tRight = Physics2D.Raycast(new Vector2(player.transform.position.x + width, player.transform.position.y + height), Vector2.right, length);

            bool bLeft = Physics2D.Raycast(new Vector2(player.transform.position.x - width, player.transform.position.y - height), -Vector2.right, length);
            bool bRight = Physics2D.Raycast(new Vector2(player.transform.position.x + width, player.transform.position.y - height), Vector2.right, length);

            if (tLeft || tRight || bLeft || bRight)
                return true;
            else
                return false;
        }

        //Returns whether or not player is touching ground.
        public bool isGround()
        {
            bool bottom1 = Physics2D.Raycast(new Vector2(player.transform.position.x, player.transform.position.y - height), -Vector2.up, length);
            bool bottom2 = Physics2D.Raycast(new Vector2(player.transform.position.x + (width - 0.1f), player.transform.position.y - height), -Vector2.up, length);
            bool bottom3 = Physics2D.Raycast(new Vector2(player.transform.position.x - (width - 0.1f), player.transform.position.y - height), -Vector2.up, length);
            //Debug.Log("Touching ground: " + bottom1 + bottom2 + bottom3 + "height: " + height + "length: " + length);
            if (bottom1 || bottom2 || bottom3)
            {
                return true;
            }
                
            else
                return false;
        }

        //Returns whether or not player is touching wall or ground.
        public bool isTouching()
        {
            if (isGround() || isWall())
                return true;
            else
                return false;
        }

        //Returns direction of wall.
        public int wallDirection()
        {
            bool left = Physics2D.Raycast(new Vector2(player.transform.position.x - width, player.transform.position.y), -Vector2.right, length);
            bool right = Physics2D.Raycast(new Vector2(player.transform.position.x + width, player.transform.position.y), Vector2.right, length);

            if (left)
                return -1;
            else if (right)
                return 1;
            else
                return 0;
        }
    }

    //Feel free to tweak these values in the inspector to perfection.  I prefer them private.
    public float speed = 14f;
    public float accel = 6f;
    public float airAccel = 3f;
    public float jump = 14f;  //I could use the "speed" variable, but this is only coincidental in my case.  Replace line 89 if you think otherwise.
    public float walljump = 10f;
    float yvelocity;
    string state = "";

    public GroundState groundState;
    public static Player instance;
    public float deathheight;
    private void Awake()
    {
        //SingleTon
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

    }
    void Start()
    {
        

        //Create an object to check if player is grounded or touching wall
        groundState = new GroundState(transform.gameObject);
        ourBody = GetComponent<Rigidbody2D>();

        deathheight = Camera.main.transform.position.y - Camera.main.orthographicSize;
    }

    private Vector2 input;

    void Update()
    {
        //Handle input
        if (Input.GetKey(KeyCode.LeftArrow))
            input.x = -1;
        else if (Input.GetKey(KeyCode.RightArrow))
            input.x = 1;
        else
            input.x = 0;

        if (Input.GetKeyDown(KeyCode.Space))
            input.y = 1;

        //Reverse player if going different direction
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, (input.x == 0) ? transform.localEulerAngles.y : (input.x + 1) * 90, transform.localEulerAngles.z);



        if(transform.position.y < deathheight)
        {
            CameraController.instance.Respawn();
        }
    }
    string TestPrintState()
    {
        state = "";
        if (groundState.isTouching())
        {
            state+= "\n Is Touching a Wall in General.";
        }

        if (groundState.isGround())
        {
            state += "\n Is Touching a Wall on the ground.";
        }
        if (groundState.isWall())
        {
            state += "\n Is Touching a Wall on the side.";
        }
        return state;
    }

    void FixedUpdate()
    {
        Debug.Log(TestPrintState());
        ourBody.AddForce(new Vector2(((input.x * speed) - ourBody.velocity.x) * (groundState.isGround() ? accel : airAccel), 0)); //Move player.

        ////Stop player if input.x is 0 (and grounded)  
        ////jump if input.y is 1
        
        if (input.y == 1 && groundState.isTouching())
        {
            if (groundState.isWall())
            {
                yvelocity = walljump;
            }
            else
            {
                yvelocity = jump;
            }
        }
        else { yvelocity = ourBody.velocity.y; }
        
        ourBody.velocity = new Vector2(
            (input.x == 0 && groundState.isGround()) ? 0 : ourBody.velocity.x, 
            (yvelocity));

        //Walljump
        if (groundState.isWall() && !groundState.isGround() && input.y == 1)
        {
            Debug.Log("YVelocity: " + ourBody.velocity.y);
            ourBody.velocity = new Vector2(-groundState.wallDirection() * speed * 0.95f, ourBody.velocity.y); //Add force negative to wall direction (with speed reduction)
        }
            

        input.y = 0;
    }
}