using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;  
using TMPro;
public class box : MonoBehaviour
{   
    public int toastcnt=0;
    public GameObject t;
    public GameObject s;
    public GameObject text,toast;
    public AudioSource audiosource;
    public AudioClip clip;
    void Awake () {
        audiosource = gameObject.AddComponent<AudioSource>();
        audiosource.playOnAwake = false;  //playOnAwake设为false时，通过调用play()方法启用
	}
    void Start(){
        text = (GameObject)Resources.Load("Toast");
    }
    private void OnMouseDown()       //鼠标按下
    {
        //换算点击坐标
        int x=2*(int)(transform.position.x/60-1);
        int z=2*(-(int)transform.position.z/60-1);
        int cust_x=2*(int)(transform.position.x/60-1);
        int cust_y=2*(-(int)transform.position.z/60-1);
        
        //分情况处理点击餐馆和食客
        if(test.flag==1){
            test.rest_x=x;
            test.rest_y=z;
            test.flag++;
            //加入餐馆标示
            t = (GameObject)Instantiate(Resources.Load("Prop_Tree_02"),new Vector3(transform.position.x-54,12f,transform.position.z+36),Quaternion.identity);
            //定时销毁
            GameObject.Destroy(t,6.0f);    
        }
        else {
            test.flag=1;
            //加入订单
            add_order(test.Index++,get_ttime(),test.rest_x,test.rest_y,cust_x,cust_y);
            //Debug.LogFormat("已增加订单：单号：{0} 下单时间：{1} 餐馆坐标：{2},{3} 食客坐标：{4},{5}",test.Index,get_ttime(),test.rest_x,test.rest_y,cust_x,cust_y);//已增加
            //加入食客标示
            s = (GameObject)Instantiate(Resources.Load("Prop_Umbrella_02"),new Vector3(transform.position.x-54,14f,transform.position.z+36),Quaternion.identity);
            //界面显示新加订单
            toast = (GameObject)Instantiate(text,new Vector3((4.5f)*60,90f,0),text.transform.rotation);
            toast.GetComponentInChildren<TextMeshPro>().text=string.Format("New_Order: Num:{0} time:{1} rest_cord:({2},{3}) cust_cord:({4},{5})",test.Index,get_ttime(),test.rest_x,test.rest_y,cust_x,cust_y);
            //定时销毁
            GameObject.Destroy(s,6.0f);
            GameObject.Destroy(toast,3f);
            //播放下单音乐
            if (audiosource.isPlaying)
            {
                audiosource.Stop();
            }
            audiosource.clip = (AudioClip)Resources.Load("new_order");
            audiosource.Play();
        }
    }
    [DllImport("HEAD")]
    private static extern void add_order(int a,int b,int c,int d,int e,int f);
    [DllImport("HEAD")]
    private static extern int get_ttime();
}
