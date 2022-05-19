using UnityEngine;

public class RandomRadiousSpawner : MonoBehaviour
{
    [SerializeField] GameObject _collectable;
    [SerializeField] Collider _colliderBounds;
    [SerializeField] LayerMask floorOnly;
    [SerializeField] int possibleSpawnAmmount = 10;

    private void Start()
    {
        SpawnNow(possibleSpawnAmmount);
    }
    private void SpawnNow(int ammount)
    {
        for (int i = 0; i < ammount; i++)
        {
            RaycastHit hit;
            Vector3 randomPos = new Vector3(Random.Range(_colliderBounds.bounds.min.x, _colliderBounds.bounds.max.x),
                _colliderBounds.bounds.center.y, Random.Range(_colliderBounds.bounds.min.z, _colliderBounds.bounds.max.z));

            if (Physics.Raycast(randomPos, _colliderBounds.transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, floorOnly))//front 
            {
                var collect = Instantiate(_collectable, hit.point, Quaternion.identity, this.transform);
                collect.transform.up = hit.normal;
            }

        }
    }

}
