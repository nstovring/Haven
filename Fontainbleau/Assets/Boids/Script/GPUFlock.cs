//
//  Created by jiadong chen
//  http://www.chenjd.me
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUFlock : MonoBehaviour {

    #region 字段

    public ComputeShader cshader;

    public GameObject boidPrefab;
    public Transform boidTarget;
    public int boidsCount;
    public float spawnRadius;
    public GameObject[] boidsGo;
    public GPUBoid[] boidsData;
    public float flockSpeed;
    public float nearbyDis;
    public float nearbyDisScale = 1;
    private Vector3 targetPos = Vector3.zero;
    private int kernelHandle;
    
    #endregion

    #region 方法

    void Start()
    {
        this.boidsGo = new GameObject[this.boidsCount];
        this.boidsData = new GPUBoid[this.boidsCount];
        this.kernelHandle = cshader.FindKernel("CSMain");


        for (int i = 0; i < this.boidsCount; i++)
        {
            //int mod3 = i % this.boidsCount/2;


            this.boidsData[i] = this.CreateBoidData();
            this.boidsGo[i] = Instantiate(boidPrefab, this.boidsData[i].pos, Quaternion.Euler(this.boidsData[i].rot),transform) as GameObject;
            this.boidsData[i].rot = this.boidsGo[i].transform.forward;
            //if (mod3 == 0)
                //this.boidsGo[i].AddComponent<Light>();
        }
    }

    GPUBoid CreateBoidData()
    {
        GPUBoid boidData = new GPUBoid();
        Vector3 pos = boidTarget.position + Random.insideUnitSphere * spawnRadius;
        Quaternion rot = Quaternion.Slerp(transform.rotation, Random.rotation, 0.3f);
        boidData.pos = pos;
        boidData.flockPos = transform.position;
        boidData.boidsCount = this.boidsCount;
        boidData.nearbyDis = this.nearbyDis;
        boidData.speed = this.flockSpeed + Random.Range(-0.5f, 0.5f);

        return boidData;
    }
    private Transform targetTransform;

    ComputeBuffer buffer;

    void Update()
    {

        if (boidTarget)
        {
            targetTransform = boidTarget;
        }
        else
        {
            targetTransform = this.transform;
            this.targetPos += new Vector3(2f, 5f, 3f);
            this.transform.localPosition += new Vector3(
                (Mathf.Sin(Mathf.Deg2Rad * this.targetPos.x) * -0.2f),
                (Mathf.Sin(Mathf.Deg2Rad * this.targetPos.y) * 0.2f),
                (Mathf.Sin(Mathf.Deg2Rad * this.targetPos.z) * 0.2f)
            );

        }


        buffer = new ComputeBuffer(boidsCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(GPUBoid)));
        Random.InitState(453);
        for (int i = 0; i < this.boidsData.Length; i++)
        {
            this.boidsData[i].nearbyDis = (Mathf.Abs(Mathf.Sin(Time.time/6) + Mathf.Sin(Time.time / 4))) * 0.5f + nearbyDis;
            this.boidsData[i].flockPos = targetTransform.position + (new Vector3(Mathf.Sin((Time.time / 8) + i), Mathf.Tan((Time.time/8) + i), Mathf.Cos((Time.time / 4) + i))) * nearbyDisScale;
            this.boidsData[i].speed = this.flockSpeed + Random.Range(-0.5f, 0.5f);
        }

        buffer.SetData(this.boidsData);

        cshader.SetBuffer(this.kernelHandle, "boidBuffer", buffer);
        cshader.SetFloat("deltaTime", Time.deltaTime);

        cshader.Dispatch(this.kernelHandle, this.boidsCount, 1, 1);

        buffer.GetData(this.boidsData);

        buffer.Release();

        for (int i = 0; i < this.boidsData.Length; i++)
        {

            this.boidsGo[i].transform.position = this.boidsData[i].pos;

            if(!this.boidsData[i].rot.Equals(Vector3.zero))
            {
                this.boidsGo[i].transform.rotation = Quaternion.LookRotation(this.boidsData[i].rot);
            }

        }
    }

    private void OnDestroy()
    {
        buffer.Release();
    }


    #endregion

}
