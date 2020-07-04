using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Player : MonoBehaviour
{   
    // public test Instance;
    public Player Instance;
    public Animator ani;
    public Rigidbody rb;
    public float last_x,last_z;
    void Start() {
        rb = this.transform.GetComponent<Rigidbody>();
        Instance=this;
        last_x=Instance.transform.position.x;
        last_z=Instance.transform.position.z;
    }
    // float speed = rb.velocity;

    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        float speed = rb.velocity.magnitude;
        //控制骑手摆臂动作
        if(Math.Abs(Instance.transform.position.x-last_x)>0.02f||Math.Abs(Instance.transform.position.z-last_z)>0.02f){
            SetSpeed((float)1.0);
        }
        else{
              SetSpeed((float)0.0);
        }
        last_x=Instance.transform.position.x;
        last_z=Instance.transform.position.z;
    }
    private void SetSpeed(float f){
        ani.SetFloat("Speed_f",f);
    }
}
