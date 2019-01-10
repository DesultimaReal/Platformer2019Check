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
            bool tLeft = Physics2D.Raycast(new Vector2(player.transform.position.x - width, player.transform.position.y + height), -Vector2.right, length, 1 << LayerMask.NameToLayer("Walls"));
            bool tRight = Physics2D.Raycast(new Vector2(player.transform.position.x + width, player.transform.position.y + height), Vector2.right, length, 1 << LayerMask.NameToLayer("Walls"));

            bool bLeft = Physics2D.Raycast(new Vector2(player.transform.position.x - width, player.transform.position.y - height), -Vector2.right, length, 1 << LayerMask.NameToLayer("Walls"));
            bool bRight = Physics2D.Raycast(new Vector2(player.transform.position.x + width, player.transform.position.y - height), Vector2.right, length, 1 << LayerMask.NameToLayer("Walls"));

            if (tLeft || tRight || bLeft || bRight)
                return true;
            else
                return false;
        }

        //Returns whether or not player is touching ground.
        public bool isGround()
        {
            bool bottom1 = Physics2D.Raycast(new Vector2(player.transform.position.x, player.transform.position.y - height), -Vector2.up, length, 1 << LayerMask.NameToLayer("Walls"));
            bool bottom2 = Physics2D.Raycast(new Vector2(player.transform.position.x + (width - 0.1f), player.transform.position.y - height), -Vector2.up, length, 1 << LayerMask.NameToLayer("Walls"));
            bool bottom3 = Physics2D.Raycast(new Vector2(player.transform.position.x - (width - 0.1f), player.transform.position.y - height), -Vector2.up, length, 1 << LayerMask.NameToLayer("Walls"));
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

    #region Character physics variables
    float speed = 14f;
    float accel = 6f;
    float airAccel = 3f;
    float jump = 14f;  //I could use the "speed" variable, but this is only coincidental in my case.  Replace line 89 if you think otherwise.
    float walljump = 10f;
    float yvelocity;
    #endregion

    #region Character state variables
    string state = "";
    public GroundState groundState;
    public static Player instance;
    public float deathheight;
    #endregion

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

    //For controlling input x and y
    Vector2 input;

    #region Controlling the Flashlight variables
    public GameObject flashlight;
    float turnV, turnH, aimAngle, horizontal;
    Vector2 aimDirection;
    Quaternion aimRotation;
    float turnSpeed = 2;
    #endregion

    void Update()
    {
        
        


        #region FlashlightControlHandling'
        turnV = Input.GetAxis("RightJoystickVertical");
        turnH = Input.GetAxis("RightJoystickHorizontal");
        aimDirection = new Vector2(turnH, turnV);
        aimAngle = Mathf.Atan2(turnH, turnV) * Mathf.Rad2Deg;

        Debug.Log("HorizontalInput: " + turnH +
            "\nVertInput: " + turnV
            );

        if (turnV != 0 && turnH != 0)
        {
            aimRotation = Quaternion.AngleAxis(aimAngle, Vector3.forward);
            Debug.Log("\nAimRot: " + aimRotation);  
            flashlight.transform.rotation = Quaternion.Slerp(flashlight.transform.rotation, aimRotation, turnSpeed * Time.time);
        }
        #endregion

        #region MovementControlHandling
        horizontal = Input.GetAxis("LeftJoystickHorizontal");
        if (horizontal > 0.2)
        {
            input.x = 1;
        }
        else if(horizontal < -0.2)
        {
            input.x = -1;
        }
        else
        {
            input.x = 0;
        }
        input.y = Input.GetButton("Abutton") ? 1 : 0;
        // Reverse player if going a different direction.
        //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, (input.x == 0) ? transform.localEulerAngles.y : (input.x + 1) * 90, transform.localEulerAngles.z);
        #endregion

        if (transform.position.y < deathheight)
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