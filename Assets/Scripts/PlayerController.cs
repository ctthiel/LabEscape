using UnityEngine;

// Handles the input and manages the player's resources
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance { get; private set; }

    public Acid acid;

    public int shiftCount { get; private set; }

    // Vars to store information about the last input
    private float lastInputTime;
    private Vector3Int lastInputDir;
    private bool lastInputShift;
    private bool lastInputDuringShift;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        GameManager.instance.initializeOthers += Initialize;
        GameManager.instance.gameUpdate += GameUpdate;
    }
    private void OnDisable()
    {
        GameManager.instance.initializeOthers -= Initialize;
        GameManager.instance.gameUpdate -= GameUpdate;
    }

    public void Initialize()
    {
        shiftCount = GameManager.instance.settings.startingKeycards;
        PickUp();
    }

    private void GameUpdate()
    {
        // Determine directional input
        Vector3Int inputDir = Vector3Int.zero;
        if (Input.GetButtonDown("Right"))
        {
            inputDir = Vector3Int.right;
        }
        else if (Input.GetButtonDown("Left"))
        {
            inputDir = Vector3Int.left;
        }
        else if (Input.GetButtonDown("Up"))
        {
            inputDir = Vector3Int.up;
        }
        else if (Input.GetButtonDown("Down"))
        {
            inputDir = Vector3Int.down;
        }

        // Determine shift input
        bool inputShift = Input.GetButton("Shift");

        // If there is input, remember it
        if (inputDir != Vector3Int.zero)
        {
            lastInputDir = inputDir;
            lastInputDuringShift = PlayerMovement.instance.riding;
            lastInputShift = inputShift;
            lastInputTime = Time.time;
        }
        // Otherwise, if the player inputed recently, use that input
        else if ((!lastInputDuringShift && Time.time < lastInputTime + GameManager.instance.settings.moveTime/2) || (lastInputDuringShift && Time.time < lastInputTime + GameManager.instance.settings.shiftTime/2))
        {
            inputDir = lastInputDir;
            inputShift = lastInputShift;
        }

        // If there is input, attempt to either move or shift
        if (inputDir != Vector3Int.zero)
        {
            if (inputShift)
            {
                // Attempt to shift if player has enough keycards
                if (shiftCount > 0)
                {
                    // If successful, consume a keycard
                    if (PlayerMovement.instance.Shift(inputDir))
                    {
                        shiftCount--;
                        SoundManager.instance.Shift();
                    }
                }
            }
            else
            {
                // Attempt to move
                PlayerMovement.instance.Move(inputDir);
            }
        }

        // Restart scene if player goes below the height of the lava
        if (transform.position.y < acid.height)
        {
            GameManager.instance.GameOver();
        }
    }

    // Consume a mana if there is one at gridPos
    // Called by PlayerMovement when the player ends a movement
    public bool PickUp()
    {
        Keycard keycard = LevelController.instance.GetBlock(PlayerMovement.instance.gridPos).GetComponentInChildren<Keycard>();
        if (keycard != null)
        {
            shiftCount += keycard.value;
            // Cap shift count
            if ( shiftCount > 12)
            {
                shiftCount = 12;
            }

            if (keycard.value > 1)
            {
                SoundManager.instance.GoodPickup();
            }
            else
            {
                SoundManager.instance.Pickup();
            }
            
            Destroy(keycard.gameObject);
            return true;
        }
        return false;
    }
}
