using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject[] enemyPrefabs;   // ← multiple prefabs here
    public Transform player;

    [Header("Spawning")]
    public float initialDelay = 1f;
    public float spawnInterval = 2f;
    public int batchSize = 1;
    public int maxAlive = 30;

    [Header("Placement")]
    public float spawnRadius = 10f;
    public float minDistanceFromPlayer = 3f;
    public float offscreenMargin = 0.08f;
    public float separation = 0.8f;
    public LayerMask avoidMask;

    int alive;

    void Start()
    {
        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
        StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        if (initialDelay > 0) yield return new WaitForSeconds(initialDelay);
        while (true)
        {
            if (alive < maxAlive && enemyPrefabs != null && enemyPrefabs.Length > 0 && player)
            {
                int count = Mathf.Min(batchSize, maxAlive - alive);
                for (int i = 0; i < count; i++)
                {
                    Vector2 pos;
                    if (!TryGetSpawnPosition(out pos)) continue;
                    var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                    var go = Instantiate(prefab, pos, Quaternion.identity);
                    alive++;
                    var tracker = go.AddComponent<OnDestroyNotify>();
                    tracker.onDestroyed = () => alive--;
                }
            }
            spawnInterval = Mathf.Max(0.5f, spawnInterval * 0.985f);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    bool TryGetSpawnPosition(out Vector2 pos)
    {
        Camera cam = Camera.main;
        for (int attempt = 0; attempt < 25; attempt++)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            Vector2 candidate = (Vector2)player.position + dir * spawnRadius;

            if (Vector2.Distance(candidate, player.position) < minDistanceFromPlayer) continue;

            if (cam)
            {
                var vp = cam.WorldToViewportPoint(candidate);
                bool off = vp.x < -offscreenMargin || vp.x > 1 + offscreenMargin ||
                           vp.y < -offscreenMargin || vp.y > 1 + offscreenMargin;
                if (!off) continue;
            }

            if (avoidMask.value != 0 && Physics2D.OverlapCircle(candidate, separation, avoidMask)) continue;

            pos = candidate;
            return true;
        }
        pos = (Vector2)player.position + Random.insideUnitCircle.normalized * (spawnRadius + 1f);
        return true;
    }

    private class OnDestroyNotify : MonoBehaviour
    {
        public System.Action onDestroyed;
        void OnDestroy() { onDestroyed?.Invoke(); }
    }
}
