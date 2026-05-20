using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private float leftEdge;

    private void Start()
    {
        leftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 2f;
    }
  private void Update()
    {
        transform.position += Vector3.left * GameManager.Instance.gameSpeed * Time.deltaTime;

        if (transform.position.x < leftEdge)
        {
            Destroy(gameObject);
        }
    }
}
//object pooling is a technique used to optimize performance by reusing objects instead of creating and destroying them frequently. In the context of the Obstacle class, instead of destroying the obstacle when it goes off-screen, we can disable it and return it to a pool of available obstacles for reuse. This way, we can avoid the overhead of instantiating new obstacles and improve the performance of the game.