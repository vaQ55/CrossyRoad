using UnityEngine;

public class Anima_Text_UI : MonoBehaviour
{
    public float speedAnimate = 1f;
    public float hightPos;
    public Vector3 startPos;
    public Vector3 endPos;
    public float startRotation = -3f;
    public float endRotation = 3f;
    public Vector3 mintScale = new Vector3(0.5f, 0.5f, 0);
    public Vector3 maxScale = new Vector3(0.5f, 0.5f, 0);
    private Vector3 startScale;
    private Vector3 endScale;

    void Start()
    {
        startPos = transform.localPosition - new Vector3(0, hightPos, 0);
        endPos = transform.localPosition + new Vector3(0, hightPos, 0);
        startScale = transform.localScale - mintScale;
        endScale = transform.localScale + maxScale;
    }
    void Update()
    {
        float time = Time.time * speedAnimate;
        float pingPong = (Mathf.Sin(time) + 1) / 2;
        transform.localPosition = Vector3.Lerp(startPos, endPos, pingPong);

        float rotationZ = Mathf.Lerp(startRotation, endRotation, pingPong*2);
        transform.localRotation = Quaternion.Euler(0, 0, rotationZ);

        transform.localScale = Vector3.Lerp(startScale, endScale, pingPong);
    }
}
