using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class BuoyancyScript : MonoBehaviour {

    [SerializeField]
    private LayerMask waterMask;

    [SerializeField]
    private List<Transform> floatPoints;

    private Rigidbody rigid;

    private Vector3 center;
    private Vector3 lastCenter;
    private Vector3 waterLine;
    //private Vector3 center;

    [SerializeField]
    private bool singleBuoyancyPoints;
    private bool inWater;

    [SerializeField]
    private float waterDensity = 25;
    [SerializeField]
    private float shipDepth;
    [SerializeField]
    private float waterDragg;
    [SerializeField]
    private float stabeliser = 2;
    private float velocity;
    private float upwardForce;

    private int Counter;
   
    private Dictionary<Transform, Timer> timerMap;
    void Awake()
    {
        timerMap = new Dictionary<Transform, Timer>();

        foreach (Transform T in floatPoints)
        {
            timerMap.Add(T, T.gameObject.AddComponent<Timer>());
        }
    }

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        /*GameObject[] getFloatPoints = GetComponentsInChildren<GameObject>();
        foreach(GameObject G in getFloatPoints)
        {
            if(G.CompareTag("Floatpoints"))
            {
                floatPoints.Add(G.transform);
            }
        }*/
        //floatPoints.Add(GameObject.FindGameObjectsWithTag("FloatPoint"));
    }
	
	void FixedUpdate () {
        velocity = Vector3.Magnitude(rigid.velocity);
        CheckBuoyancyPoints();
        CheckInWater();
        CheckWaterline();
        FloatAndDragg();
    }

    void CheckWaterline()
    {
        if (singleBuoyancyPoints)
        {
            Ray rayDown = new Ray(center, Vector3.down);
            Ray rayUp = new Ray(center, Vector3.up);
            RaycastHit hit;
            if (Physics.Raycast(rayDown, out hit, 5, waterMask, QueryTriggerInteraction.Collide) || Physics.Raycast(rayUp, out hit, 25, waterMask, QueryTriggerInteraction.Collide))
            {
                waterLine = hit.point - new Vector3(0,shipDepth,0);
                //print(waterLine + "waterline");
            }
        }
    }

    void CheckBuoyancyPoints()
    {
        center = Vector3.zero;
        Counter = 0;
        foreach(Transform T in floatPoints)
        {
            Ray rayDown = new Ray(T.position, Vector3.down);
            Ray rayUp = new Ray(T.position, Vector3.up);
            RaycastHit hit;
            if (Physics.Raycast(rayDown, out hit, 5, waterMask, QueryTriggerInteraction.Collide) || Physics.Raycast(rayUp, out hit, 25, waterMask, QueryTriggerInteraction.Collide))
            {
                if(T.position.y <= hit.point.y - shipDepth)
                {
                    if (!timerMap[T].IsRunning)
                    {
                        timerMap[T].StartTimer();
                    }
                    if (singleBuoyancyPoints)
                    {
                        //print(timerMap[T].TimerTime);
                        if (timerMap[T].TimerTime < stabeliser)
                        {
                            rigid.AddForceAtPosition(new Vector3(0, ((timerMap[T].TimerTime / stabeliser) * waterDensity)/floatPoints.Count, 0), T.position);
                            //print(timerMap[T].TimerTime / stabeliser + "keer" );
                            //print(timerMap[T].TimerTime);
                        }
                        else
                        {
                            rigid.AddForceAtPosition(new Vector3(0, waterDensity/floatPoints.Count, 0), T.position);
                        }
                    }
                    center += T.position;
                    Counter++;
                }
                else
                {
                    if (timerMap[T].IsRunning)
                    {
                        timerMap[T].StopTimer();
                    }
                }
            }
        }
        center = center / Counter;
        if(Vector3.Distance(center,lastCenter) > 0.5f)
        {
            //center = (center + lastCenter + lastCenter + lastCenter) / 4;
        }
        lastCenter = center;
    }

    void CheckInWater()
    {
        if (center.y < waterLine.y)
        {
            upwardForce = (1 - Mathf.Clamp(waterLine.y / transform.position.y,0.1f,1)/ (Mathf.Clamp(waterLine.y / transform.position.y, 0.1f, 1)+1f)) * waterDensity;
            inWater = true;
            //print(upwardForce+ "force");
        }
        else
        {
            inWater = false;
        }
    }

    void FloatAndDragg()
    {
        if (inWater)
        {
            if (Counter != 0 && !singleBuoyancyPoints)
            {
                rigid.AddForceAtPosition(new Vector3(0, upwardForce, 0), center);
            }
            if (velocity > 0.8f)
            {
                rigid.AddForce(rigid.velocity * -1 * waterDragg);
            }
        }
    }
}
