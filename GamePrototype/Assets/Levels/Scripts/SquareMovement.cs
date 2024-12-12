using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Ethan added this script
// Script to move a game object in a square pattern

public class squareMovement : MonoBehaviour
{
    enum Direction { Up, Down, Left, Right };
    Direction currentDirection = Direction.Right;

    public float speed = 5f;
    public Vector3 startPosition;
    public float moveDistance = 5f;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        MoveInSquarePattern();
    }

    void MoveInSquarePattern()
    {
        switch (currentDirection)
        {
            case Direction.Right:
                transform.Translate(Vector3.right * speed * Time.deltaTime);
                if (transform.position.x >= startPosition.x + moveDistance)
                {
                    ChangeDirection();
                }
                break;
            case Direction.Down:
                transform.Translate(Vector3.down * speed * Time.deltaTime);
                if (transform.position.y <= startPosition.y - moveDistance)
                {
                    ChangeDirection();
                }
                break;
            case Direction.Left:
                transform.Translate(Vector3.left * speed * Time.deltaTime);
                if (transform.position.x <= startPosition.x - moveDistance)
                {
                    ChangeDirection();
                }
                break;
            case Direction.Up:
                transform.Translate(Vector3.up * speed * Time.deltaTime);
                if (transform.position.y >= startPosition.y + moveDistance)
                {
                    ChangeDirection();
                }
                break;
        }

    }

    void ChangeDirection()
    {
        // Cycle through the Direction enum
        currentDirection = (Direction)(((int)currentDirection + 1) % 4);

        // Reset start position for the next leg of the square
        startPosition = transform.position;
    }

}
