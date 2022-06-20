using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class Main_control : MonoBehaviour
{
    //单例模式 Singleton mode
    public static Main_control instance;

    //存放的颜色序列 Stored color sequence
    public static string[] colors = {
        "#8B968D"
    };

    //Game_stau
    public Game_stau game_statu;

    [Header("last block")]
    public GameObject game_obj_last_block;

    [Header("block prefab")]
    public GameObject prefab_block;

    [Header("text score")]
    public Text text_score;
    public int num_score;

    [Header("game start ui object")]
    public GameObject game_obj_game_start;

    [Header("game over ui object")]
    public GameObject game_obj_game_over;

    [Header("game_obj_block_base")]
    public GameObject game_obj_block_base;

    [Header("game_obj_block_0")]
    public GameObject game_obj_block_0;

    [Header("ParticleSystem score")]
    public ParticleSystem particle_score;

    [DllImport("__Internal")]
    private static extern void ImageDownloader(string str, string fn);

    void Awake()
    {
        Main_control.instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //设置底座和第一块方块的颜色 Set the color of the base and the first square
        #region 
        Color nowColor;
        ColorUtility.TryParseHtmlString("#8b968d", out nowColor);
        this.game_obj_block_base.GetComponent<Renderer>().material.color = nowColor;
        this.game_obj_block_0.GetComponent<Renderer>().material.color = nowColor;
        #endregion

        this.change_to_game_start();

        GetColors();
    }

    async public void GetColors()
    {
        string[] nftColors = await GetNFT.Colors();
        if (nftColors.Length > 0)
        {
            colors = nftColors;
        }
    }

    //create new block
    public void creat_new_block()
    {
        GameObject block = Instantiate(this.prefab_block);

        //加分 Add points
        this.num_score++;
        this.text_score.text = this.num_score + "";
    }

    //change to game start
    public void change_to_game_start()
    {
        //显示UI界面 Display UI interface
        this.game_obj_game_over.SetActive(false);
        StartCoroutine(Canvas_grounp_fade.show(this.game_obj_game_start));

        //设置分数 Set score
        this.num_score = 0;
        this.text_score.text = this.num_score + "";

        //change statu
        this.game_statu = Game_stau.game_start;
    }

    //change to gaming
    public void change_to_gaming()
    {
        //隐藏UI Hide UI
        StartCoroutine(Canvas_grounp_fade.hide(this.game_obj_game_start));
        StartCoroutine(Canvas_grounp_fade.hide(this.game_obj_game_over));

        //play sound
        Audio_control.instance.play_sound_game_start();

        //删除所有的方块 Delete all blocks
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("block");
        for (int i = 0; i < blocks.Length; i++)
        {
            Destroy(blocks[i]);
        }

        //修改分数值为-1 Modify the score value to -1
        this.num_score = -1;

        //设置最后一个方块 Set the last square
        this.game_obj_last_block = GameObject.FindGameObjectWithTag("block_0");

        //生成第一个方块 Generate the first square
        this.creat_new_block();

        //change statu
        this.game_statu = Game_stau.gaming;
    }


    //change to game_over
    public void change_to_game_over()
    {
        //显示UI界面 Display UI interface
        this.game_obj_game_start.SetActive(false);
        StartCoroutine(Canvas_grounp_fade.show(this.game_obj_game_over));

        //play sound
        Audio_control.instance.play_sound_game_over();

        //todo 把摄像头拉远

        //change statu
        this.game_statu = Game_stau.game_over;
    }

    private IEnumerator CoroutineScreenshot()
    {
        game_obj_game_over.SetActive(false);
        yield return new WaitForEndOfFrame();
        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        Rect rect = new Rect(0, 0, width, height);
        tex.ReadPixels(rect, 0, 0);
        tex.Apply();
        byte[] bytes = tex.EncodeToPNG();
        ImageDownloader(System.Convert.ToBase64String(bytes), "screenshot.png");
        game_obj_game_over.SetActive(true);
    }


    //event
    #region 

    //on start game btn
    public void on_start_game_btn()
    {
        this.change_to_gaming();
    }

    public void on_marketplace_btn() {
        Application.OpenURL("https://tofunft.com/discover/items?contracts=36335&network=42262");
    }

    public void on_screenshot_btn() {
        // ScreenCapture.CaptureScreenshot("screenshot.png");
        StartCoroutine(CoroutineScreenshot());
    }
    #endregion
}

//Game_stau
public enum Game_stau
{
    game_start,
    gaming,
    game_over
}
