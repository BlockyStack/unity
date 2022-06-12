using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_move_control : MonoBehaviour
{
    //单例模式 Singleton mode
    public static Camera_move_control instance;

    //init camera position
    private Vector3 init_position;

    //move speed
    private float move_speed = 2f;

    void Awake()
    {
        Camera_move_control.instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.init_position = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        float height = 0.1f * (Main_control.instance.num_score - 3);
        if (height < 0)
            height = 0;

        if (Main_control.instance.game_statu == Game_stau.game_start || Main_control.instance.game_statu == Game_stau.gaming)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(this.init_position.x, this.init_position.y + height, this.init_position.z), Time.deltaTime * this.move_speed);
        }
        else if (Main_control.instance.game_statu == Game_stau.game_over)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(this.init_position.x + 1.3f, this.init_position.y + height, this.init_position.z + 1.3f), Time.deltaTime * this.move_speed);
        }
    }
}
