using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceCruizerController : MonoBehaviour
{

    public GameObject obj_body;
    public GameObject obj_tail;
    public GameObject particle_explode;
    public Vector3 vec_initpos;
    public float speed = 0.5f;
    public bool isStarted = false;

    public static SpaceCruizerController _instance;
    public static SpaceCruizerController Instance {
        get {

            return _instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
        vec_initpos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if(isStarted)
            transform.Translate(transform.forward * Time.deltaTime * speed);
    }

    public void SetStarted(bool val)
    {

        isStarted = val;
    }

    public void Crash()
    {
        obj_body.SetActive(false);
        obj_tail.SetActive(false);
        isStarted = false;
        Instantiate(particle_explode, transform);
        StartCoroutine(iCrash(1f));
    }

    IEnumerator iCrash(float t)
    {
        yield return new WaitForSeconds(t);

        transform.position = vec_initpos;
        obj_body.SetActive(true);
        obj_tail.SetActive(true);
    }

}
