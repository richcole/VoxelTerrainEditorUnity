using UnityEngine;

public class DigController : MonoBehaviour, Interactor {

    public ParticleSystem explosionPrototype;
    public DigHead digHead;
    public AudioClip digClip;

	// Use this for initialization
	void Start () {
        digHead = FindObjectOfType<DigHead>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Interact(Ray ray, RaycastHit hit)
    {
        if (Input.GetMouseButtonDown(0))
        {
            GridArray gridArray = hit.collider.gameObject.GetComponentInParent<GridArray>();
            var explosion = Instantiate(explosionPrototype);
            explosion.Play();
            explosion.transform.position = hit.point - ray.direction * 0.3f;
            explosion.transform.rotation = Quaternion.LookRotation(-ray.direction);

            digHead.transform.position = hit.transform.position;
            digHead.GetComponent<AudioSource>().PlayOneShot(digClip);
            gridArray.Dig(hit.point, 2f);
        }

        if (Input.GetMouseButtonDown(1))
        {
            GridArray gridArray = hit.collider.gameObject.GetComponentInParent<GridArray>();
            gridArray.Build(hit.point, 2f);
        }
    }
}
