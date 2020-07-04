using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;  
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    float i=2.0f;
    public Text Money_Text;
    public Text Time_Text;
    public Text Orders_Text;
    public Text Finishied_Text;
    void Start()
    {
        Instance = this;
    }
    void Update()
    {
        //刷新UI，2s一次
        if(i>=2.0f){
           Money_Text.text=get_money()+"";
            Time_Text.text=get_ttime()+"";
            Orders_Text.text=get_num()+"";
            Finishied_Text.text=get_finished()+"";
            i=0;
        }
        i+=Time.deltaTime;
        
    }
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
}
