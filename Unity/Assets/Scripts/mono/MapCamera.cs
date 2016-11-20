using UnityEngine;
using System.Collections;

public class MapCamera : MonoBehaviour {

    public Vector3 scrollSpeed;
    public float closestZoomY;
    public float fartherstZoomY;
    public float closestZoomXAngle;
    public float fartherstZoomXAngle;
    public float angleChangeZoomPercent;
    public float closestAdjustZ;
    public GameObject Map;

    float xRange;
    float zRange;

    float zoomPercent;
    float xPercent;
    float zPercent;

    GameAnimation translateAnimation;

    void Awake()
    {
       
    }

    // Use this for initialization
    void Start()
    {


        xRange = Map.transform.localScale.x / 0.2f;
        zRange = Map.transform.localScale.z / 0.2f;

        xPercent = 0.5f;
        zPercent = 0.5f;
        zoomPercent = 0;

	}


    void OnGUI()
    {
    }

    public float YfromZoomPercent()
    {
        return fartherstZoomY - zoomPercent * (fartherstZoomY - closestZoomY);
    }
    public Vector3 MouseToWorld()
    {
        return GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, YfromZoomPercent()));
    }

    public Vector3 WorldToPercent(Vector3 world)
    {
        world -= adjust;
        float xP = (world.x + (xRange)) / (2 * xRange);
        float zP = (world.z + (zRange)) / (2 * zRange);

      
        return new Vector3(xP, 0, zP);
    }

    Vector3 adjust;
	// Update is called once per frame
	void Update () {

        float scrollUpdate = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * scrollSpeed.y;
        float xUpdate = Input.GetAxis("Horizontal") * Time.deltaTime * scrollSpeed.x;
        float zUpdate = Input.GetAxis("Vertical") * Time.deltaTime * scrollSpeed.z;

        xPercent += xUpdate;
        zPercent += zUpdate;
        
        if (scrollUpdate != 0)
        {
            /*xPercent = xPercent + (scrollUpdate * scrollSpeed.x) * (tentativePercent.x - xPercent);
            zPercent = zPercent + (scrollUpdate * scrollSpeed.z) * (tentativePercent.z - zPercent);*/

            if (zoomPercent != 1 )
            {
                Vector3 tentativePercent = WorldToPercent(MouseToWorld());

                xPercent = tentativePercent.x;
                zPercent = tentativePercent.z;

            }
            zoomPercent = zoomPercent + scrollUpdate;

            zoomPercent = Mathf.Min(zoomPercent, 1);
            zoomPercent = Mathf.Max(0, zoomPercent);
            

        }

        xPercent = Mathf.Min(xPercent, 1);
        xPercent = Mathf.Max(0, xPercent);

        zPercent = Mathf.Min(zPercent, 1);
        zPercent = Mathf.Max(0, zPercent);


  
         float zoomX = zoomPercent * (xPercent - 0.5f) *2 * xRange;
        float zoomY = YfromZoomPercent();
        float zoomZ = zoomPercent  *(zPercent - 0.5f) * 2 * zRange;

        float zoomAngleX = fartherstZoomXAngle;


        if (zoomPercent >= angleChangeZoomPercent)
        {
            float partialPercent = (zoomPercent - angleChangeZoomPercent) / (1 - angleChangeZoomPercent);
            zoomAngleX = fartherstZoomXAngle - partialPercent * (fartherstZoomXAngle - closestZoomXAngle);
            zoomZ += partialPercent * closestAdjustZ;
        }

     
        translateAnimation = new GameAnimation(transform, AnimationTypes.Simple, adjust+ new Vector3(zoomX, zoomY, zoomZ), Quaternion.Euler(zoomAngleX, 0, 0), 0.25f);
        translateAnimation.Continue();
        
	}
}
