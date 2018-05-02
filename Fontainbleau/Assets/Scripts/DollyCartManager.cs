using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class DollyCartManager : MonoBehaviour {
    public Transform boidTarget;
    public Transform player;
    public List<CinemachineDollyCart> carts;
    public GPUFlock boidFlock;
    int selectedCartNum = 0;

    bool active = false;
	// Use this for initialization
	void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        GameObject[] tempCarts = GameObject.FindGameObjectsWithTag("ForestBoidTracks");

        foreach (var item in tempCarts)
        {
            carts.Add(item.GetComponent<CinemachineDollyCart>());
        }

		if(carts != null && carts.Count > 0)
        {
            active = true;
            SwitchCart(selectedCartNum);
        }
	}

    void GetClosestPointOnPaths(Transform player, out int pathIndex, out float curShortestDistance)
    {
        pathIndex = 0;
        curShortestDistance = 10;
        float prevDistance = 10;

        CinemachineDollyCart cart = carts[0];
        CinemachinePathBase cartPath = cart.m_Path;
        float closestPoint = cartPath.FindClosestPoint(player.transform.position, 0, -1, 10);
        float convClosestPoint = cartPath.FromPathNativeUnits(closestPoint, CinemachinePathBase.PositionUnits.Distance);
        Vector3 pos = cartPath.EvaluatePositionAtUnit(closestPoint, CinemachinePathBase.PositionUnits.Distance);
        float distance = Vector3.Distance(pos, player.position);
        curShortestDistance = convClosestPoint;
        pathIndex = 0;
        prevDistance = distance;

        for (int i = 1; i < carts.Count; i++)
        {
             cart = carts[i];
             cartPath = cart.m_Path;
             closestPoint = cartPath.FindClosestPoint(player.transform.position, 0, -1, 10);
             convClosestPoint = cartPath.FromPathNativeUnits(closestPoint, CinemachinePathBase.PositionUnits.Distance);
             pos = cartPath.EvaluatePositionAtUnit(closestPoint, CinemachinePathBase.PositionUnits.Distance);
             distance = Vector3.Distance(pos, player.position);
            if (prevDistance > distance)
            {
                curShortestDistance = convClosestPoint;
                prevDistance = distance;
                pathIndex = i;
            }
          
        }
    }

    void SwitchCart(int cartNum)
    {
        boidTarget.transform.parent = carts[cartNum].transform;
        boidTarget.localPosition = Vector3.zero;
        carts[cartNum].m_Speed = 1;
    }

    void SwitchCart(int cartNum, float distanceInPath)
    {
        boidTarget.transform.parent = carts[cartNum].transform;
        boidTarget.localPosition = Vector3.zero;
        carts[cartNum].m_Speed = 1;
        carts[cartNum].m_Position = distanceInPath;
    }

    public void GetNearestCart()
    {
        if (player == null)
        {
            Debug.Log("Warning no player assigned to object: " + name);
            return;
        }

        int cartIndex = 0;
        float distanceInPath = 0;
        GetClosestPointOnPaths(player, out cartIndex, out distanceInPath);

        SwitchCart(cartIndex, distanceInPath);
    }


    public void GetForestTrack()
    {
        carts[0] = GameObject.FindGameObjectWithTag("ForestBoidTracks").transform.GetComponent<CinemachineDollyCart>();
    }

    // Update is called once per frame
    void Update () {
        if (!active)
            return;
    }
}
