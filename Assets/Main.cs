using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices; 
using UnityEngine.UI;
using TMPro;
public class test : MonoBehaviour
{
    public AudioSource audiosource;
    public AudioClip clip;
    public GameObject cop_car;
    public GameObject boat;
    public GameObject cord;
    public static int flag=1;//一为下单点 二为顾客点
    public static int rest_x,rest_y;
    public static int Index=0;
    public float time=0;
    Vector3 l = new Vector3 (-30, 0, 0);
    Vector3 u = new Vector3 (0, 0, 30);
    Vector3 d = new Vector3 (0, 0, -30);
    Vector3 r = new Vector3 (30, 0, 0);
    //public Player players;
    public struct Rider{
        public int x;
        public int y;
    };
    public Rider[] rider = new Rider[20];
    public int[] move_staus = new int[20];//确认骑手下一步运行状态
    public int current_rider_cnt = 0;
    public int temp_x;
    public int temp_y;
    public int orderA,orderB,x1,y1,x2,y2;
    public int stop_rest_cnt,stop_cust_cnt;
    public GameObject[] instance_rider = new GameObject[20];
    public GameObject[] stop_rest=new GameObject[82];
    public GameObject[] stop_cust=new GameObject[82];
    public GameObject[] cord_rest=new GameObject[82]; 
    public GameObject[] cord_cust=new GameObject[82]; 
    void Awake () {
        audiosource = gameObject.AddComponent<AudioSource>();
        audiosource.playOnAwake = false;  //playOnAwake设为false时，通过调用play()方法启用
	}    
    void Start()
    {
        cop_car = (GameObject)Resources.Load("cop_seperate_mesh");
        boat = (GameObject)Resources.Load("Vehicle_Boat_01");
        cord=(GameObject)Resources.Load("Text_cord");
        recruit();
    }
    void Update()
    {
        if(time<0.01f){
            FuncsInDelay();
            // Debug.Log("here!");
        }
        for(int i=0;i<current_rider_cnt;i++){
            transit(move_staus[i],i,time);
            // Debug.LogFormat("move_status:{0},i:{1}",move_staus[i],i);
        }
        time+=Time.deltaTime;
        if(time>=2f){
            time=0f;
        }
        
    }
    public void set_move_status_here(){
        //获取骑手移动方向
        for(int i=0;i<current_rider_cnt;i++){
            move_staus[i]=get_move_status(i);
        }
    }
    void FuncsInDelay(){
        for(int i=0;i<stop_rest_cnt;i++){
            GameObject.DestroyImmediate(stop_rest[i]);
            GameObject.DestroyImmediate(cord_rest[i]);
        }
        stop_rest_cnt=0;
        for(int i=0;i<stop_cust_cnt;i++){
            GameObject.DestroyImmediate(stop_cust[i]);
            GameObject.DestroyImmediate(cord_cust[i]);
        }
        stop_cust_cnt=0;
            if(get_flag()==0 &&get_money()>=0){
                reset();
                income();
                distribute();
                income();
                fine();
                recruit();
                Init_riders();
                move();
                set_move_status_here();
                renew_riders();
                set_ttime(get_ttime()+1);
                //停靠信息
                for(int i=0;i<current_rider_cnt;i++){
                    int stats=pos(i,ref orderA,ref x1,ref y1,ref orderB,ref x2,ref y2);
                    if(stats==1){//停餐馆
                        stop_rest[stop_rest_cnt] = (GameObject)Instantiate(cop_car,new Vector3((x1+1)*30+7,17f,(y1+1)*-30),cop_car.transform.rotation);
                        cord_rest[stop_rest_cnt] = (GameObject)Instantiate(cord,new Vector3((x1+1)*30,35f,(y1+1)*-30),cord.transform.rotation);
                        cord_rest[stop_rest_cnt].GetComponentInChildren<TextMeshPro>().text=string.Format("{0},{1},({2},{3})",orderA+1,"Rest.",x1,y1);
                        stop_rest_cnt++;
                    }
                    else if(stats==2){//停食客
                        stop_cust[stop_cust_cnt] = (GameObject)Instantiate(boat,new Vector3((x2+1)*30+7,17f,(y2+1)*-30),boat.transform.rotation);
                        cord_cust[stop_cust_cnt] = (GameObject)Instantiate(cord,new Vector3((x2+1)*30,35f,(y2+1)*-30),cord.transform.rotation);
                        cord_cust[stop_cust_cnt].GetComponentInChildren<TextMeshPro>().text=string.Format("{0},{1},({2},{3})",orderB+1,"Cust.",x2,y2);
                        stop_cust_cnt++;
                        //播放结单音乐
                        if (audiosource.isPlaying)
                        {
                            audiosource.Stop();
                        }
                        audiosource.clip = (AudioClip)Resources.Load("finished");
                        audiosource.Play();
                    }
                    else if(stats==3){//都停
                        stop_rest[stop_rest_cnt] = (GameObject)Instantiate(cop_car,new Vector3((x1+1)*30+7,17f,(y1+1)*-30),cop_car.transform.rotation);
                        cord_rest[stop_rest_cnt] = (GameObject)Instantiate(cord,new Vector3((x1+1)*30,35f,(y1+1)*-30),cord.transform.rotation);
                        cord_rest[stop_rest_cnt].GetComponentInChildren<TextMeshPro>().text=string.Format("{0},{1},({2},{3})",orderA+1,"Rest.",x1,y1);
                        stop_cust[stop_cust_cnt] = (GameObject)Instantiate(boat,new Vector3((x2+1)*30+7,17f,(y2+1)*-30),boat.transform.rotation);
                        cord_cust[stop_cust_cnt] = (GameObject)Instantiate(cord,new Vector3((x2+1)*30,35f,(y2+1)*-30),cord.transform.rotation);
                        cord_cust[stop_cust_cnt].GetComponentInChildren<TextMeshPro>().text=string.Format("{0},{1},({2},{3})",orderB+1,"Cust.",x2,y2);
                        stop_rest_cnt++;
                        stop_cust_cnt++;
                        //播放结单音乐
                        if (audiosource.isPlaying)
                        {
                            audiosource.Stop();
                        }
                        audiosource.clip = (AudioClip)Resources.Load("finished");
                        audiosource.Play();
                    }
                }
            }
            if(get_flag()==1||get_money()<0)
           {
                Debug.Log("Broke");//破产
                // Quit();
           }  

    }
    void Init_riders(){
        if(current_rider_cnt<get_cnt_rider()){ //实例化骑手
            for(;current_rider_cnt<get_cnt_rider();current_rider_cnt++){
            instance_rider[current_rider_cnt] = (GameObject)Instantiate(Resources.Load("SimplePeople_Trucker_White"),new Vector3((float)270,(float)0.6,(float)-240),Quaternion.identity);
            instance_rider[current_rider_cnt].GetComponentInChildren<TextMeshPro>().text=current_rider_cnt+"";
            }
            //播放骑手招募音乐
            if (audiosource.isPlaying)
            {
                audiosource.Stop();
            }
            audiosource.clip = (AudioClip)Resources.Load("show_up");
            audiosource.Play();
        }
    }
    void renew_riders(){//更新骑手坐标
            for(int i=0;i<get_cnt_rider();i++){
            renew_rider_pos(i,ref temp_x,ref temp_y);
            rider[i].x=temp_x;
            rider[i].y=temp_y;
            }
    }
    public void transit(int type,int num,float step){
        //骑手移动方法
        switch(type){
            case 5:
                if(step<1f)
                move("left",num);
                else 
                move("down",num);
                break;
            case 7:
                if(step<1f)
                move("left",num);
                else
                move("up",num);
                break;   
            case 6:
                if(step<1f)
                move("right",num);
                else
                move("down",num);
                break;
            case 8:
                if(step<1f)
                move("right",num);
                else
                move("up",num);
                break;
            case 10:
                if(step<1f)
                move("up",num);
                else
                move("left",num);
                break;
            case 12:
                if(step<1f)
                move("down",num);
                else
                move("left",num);
                break;
            case 9:
                if(step<1f)
                move("up",num);
                else
                move("right",num);
                break;     
            case 11:    
                if(step<1f)
                move("down",num);
                else
                move("right",num);
                break;
            case 1:
                if(step<1f)
                move("right",num);
                else
                move("right",num);
                break;
            case 2:
                if(step<1f)
                move("left",num);
                else
                move("left",num);
                break; 
            case 3:
                if(step<1f)
                move("down",num);
                else
                move("down",num);
                break;    
            case 4:   
                if(step<1f)
                move("up",num);
                else
                move("up",num);
                break;
            case 0:
                instance_rider[num].transform.position = new Vector3((float)((rider[num].x+1)*30),(float)0.6,(float)((rider[num].y+1)*-30));
                break;                    
        }
    }
    private void move(string type,int num){
        switch(type){//确定
            case "left":
                //确定方向 
                instance_rider[num].transform.rotation = Quaternion.Euler(new Vector3(0,-90,0));
                //朝该方向移动
                instance_rider[num].transform.Translate(l*Time.deltaTime,Space.World);
                // instance_rider[num].transform.Translate(-distance,0,0,Space.World);
                break;
            case "right":
                instance_rider[num].transform.rotation = Quaternion.Euler(new Vector3(0,90,0));
                instance_rider[num].transform.Translate(r*Time.deltaTime,Space.World);
                //  instance_rider[num].transform.Translate(distance,0,0,Space.World);
                break; 
            case "up":
                instance_rider[num].transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
                instance_rider[num].transform.Translate(u*Time.deltaTime,Space.World);
                //  instance_rider[num].transform.Translate(0,0,-distance,Space.World);
                break;   
            case "down":
                instance_rider[num].transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
                instance_rider[num].transform.Translate(d*Time.deltaTime,Space.World);
                //  instance_rider[num].transform.Translate(0,0,distance,Space.World);
                break;
        }
        //校正骑手坐标
        if (time>1.982f)
        instance_rider[num].transform.position = new Vector3((float)((rider[num].x+1)*30),(float)0.6,(float)((rider[num].y+1)*-30));
    }
    // public void Quit(){
    //     UnityEditor.EditorApplication.isPlaying = false;
    //  }
     
    [DllImport("HEAD")]
    private static extern void close_file();
    [DllImport("HEAD")]
    private static extern void get_order();
    [DllImport("HEAD")]
    private static extern void recruit();
    [DllImport("HEAD")]
    private static extern void distribute();
    [DllImport("HEAD")]
    private static extern void income();
    [DllImport("HEAD")]
    private static extern void fine();
    [DllImport("HEAD")]
    private static extern void move();
    [DllImport("HEAD")]
    private static extern void file_out();
    [DllImport("HEAD")]
    private static extern void reset();
    [DllImport("HEAD")]
    private static extern int dist(int a,int b,int c,int d);
    [DllImport("HEAD")]
    private static extern void add_order(int a,int b,int c,int d,int e,int f);
    [DllImport("HEAD")]
    private static extern int pos(int i,ref int orderA,ref int x1,ref int y1,ref int orderB,ref int x2,ref int y2);
    [DllImport("HEAD")]
    private static extern int get_num();
    [DllImport("HEAD")]
    private static extern int get_finished();
    [DllImport("HEAD")]
    private static extern int get_money();
    [DllImport("HEAD")]
    private static extern int get_flag();
    [DllImport("HEAD")]
    private static extern int get_ttime();
    [DllImport("HEAD")]
    private static extern int get_cnt_rider();
    [DllImport("HEAD")]
    private static extern int get_move_status(int i);
    [DllImport("HEAD")]
    private static extern void set_num(int n);
    [DllImport("HEAD")]
    private static extern void set_ttime(int t);
    [DllImport("HEAD")]
    private static extern void renew_rider_pos(int cnt,ref int temp_x,ref int temp_y);
    //在DLL中修改x,y的值
    
}
