using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public IEnumerator FlyToTarget(Vector3 targetPos, float speed)
    {
        Vector2 dir = targetPos - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);


        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }
}