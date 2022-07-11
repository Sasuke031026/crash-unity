using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TooltipHandler : MonoBehaviour
{

    public Text txt;
    public float time_live = 1f;
    Transform target;
    Camera mCamera;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("spaceship").transform;
        mCamera = Camera.main;

        Vector3 targPos = target.position;
        Vector3 camForward = mCamera.transform.forward;
        Vector3 camPos = mCamera.transform.position + camForward;
        float distInFrontOfCamera = Vector3.Dot(targPos - camPos, camForward);
        if (distInFrontOfCamera < 0f)
        {
            targPos -= camForward * distInFrontOfCamera;
        }
        Vector3 pos = RectTransformUtility.WorldToScreenPoint(mCamera, targPos);
        pos.y += 5;
        GetComponent<RectTransform>().position = pos;
        StartCoroutine(iDestroy(time_live));
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * 50f);
    }

    public void SetVal(string str)
    {
        txt.text = str;
    }

    IEnumerator iDestroy(float t)
    {
        yield return new WaitForSeconds(t);
        Destroy(gameObject);
    }
}
