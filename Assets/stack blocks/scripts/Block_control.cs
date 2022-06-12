using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block_control : MonoBehaviour
{
    //move speed
    private float move_speed = 1.23f;

    //move_direction
    private Move_direction move_direction = Move_direction.direction_z;

    //是否正处于运动状态
    private bool is_moving = true;

    void Start()
    {
        //设置方向 Set direction
        if (Main_control.instance.game_obj_last_block.GetComponent<Block_control>() != null)
        {
            if (Main_control.instance.game_obj_last_block.GetComponent<Block_control>().move_direction == Move_direction.direction_z)
            {
                this.move_direction = Move_direction.direction_x;
            }
            else
            {
                this.move_direction = Move_direction.direction_z;
            }
        }

        //设置一个颜色 Set a color
        int color_index = Random.Range(0, Main_control.colors.Length);
        // int color_index = Main_control.instance.num_score;
        // while (color_index >= Main_control.colors.Length)
        // {
        //     color_index -= Main_control.colors.Length;
        // }
        Color nowColor;
        ColorUtility.TryParseHtmlString(Main_control.colors[color_index], out nowColor);
        this.GetComponent<Renderer>().material.color = nowColor;

        //设置大小 set size
        this.transform.localScale = new Vector3(Main_control.instance.game_obj_last_block.transform.localScale.x, this.transform.localScale.y, Main_control.instance.game_obj_last_block.transform.localScale.z);

        //设置位置 set position
        if (this.move_direction == Move_direction.direction_z)
            this.transform.position = new Vector3(Main_control.instance.game_obj_last_block.transform.position.x, Main_control.instance.game_obj_last_block.transform.position.y + 0.1f, -1.09f);
        else
            this.transform.position = new Vector3(-1.09f, Main_control.instance.game_obj_last_block.transform.position.y + 0.1f, Main_control.instance.game_obj_last_block.transform.position.z);

        //运动状态 Motion state
        this.is_moving = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Main_control.instance.game_statu == Game_stau.gaming)
        {
            if (this.is_moving == false)
                return;

            if (this.move_direction == Move_direction.direction_z)
            {
                if (this.transform.position.z > 1.1f || this.transform.position.z < -1.1f)
                    this.move_speed *= -1;

                this.transform.position += transform.forward * Time.deltaTime * this.move_speed;
            }
            else
            {
                if (this.transform.position.x > 1.1f || this.transform.position.x < -1.1f)
                    this.move_speed *= -1;
                this.transform.position += transform.right * Time.deltaTime * this.move_speed;
            }

            //玩家点击了屏幕 The player taps the screen
            if (Input.GetMouseButtonDown(0))
            {
                this.stack_the_block();
            }
        }
    }

    //堆方块 stack blocks
    private void stack_the_block()
    {
        this.is_moving = false;

        //计算跟之前的那个方块比位置差了多少 Calculate how far the square is from the previous one
        if (this.move_direction == Move_direction.direction_z)
        {
            //跟上一个方块间隔的Z的距离 Z distance from the previous square
            float space_z = this.transform.position.z - Main_control.instance.game_obj_last_block.transform.position.z;

            //如果间距相差比较小，就让这块方块跟上一块方块重合，播放音效和特效 If the distance is relatively small, let this block overlap with the previous block and play sound effects and special effects
            if (Mathf.Abs(space_z) < 0.025f)
            {
                //set position
                this.transform.position = new Vector3(Main_control.instance.game_obj_last_block.transform.position.x, this.transform.position.y, Main_control.instance.game_obj_last_block.transform.position.z);

                //play sound
                Audio_control.instance.play_sound_score();

                //play particle
                Main_control.instance.particle_score.gameObject.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 0.05f, this.transform.position.z);
                Main_control.instance.particle_score.gameObject.transform.localScale = this.transform.localScale;
                Main_control.instance.particle_score.Play();
            }
            else
            {
                //新的方块的z的尺寸大小 The z size of the new square
                float new_block_size_z = Main_control.instance.game_obj_last_block.transform.localScale.z - Mathf.Abs(space_z);

                //下落的方块z的大小 The size of the falling square z
                float falling_block_size_z = this.transform.localScale.z - new_block_size_z;

                //这个地方要判断游戏有没有结束 This place has to judge whether the game is over
                if (new_block_size_z <= 0)
                {
                    //添加刚体并销毁 Add rigid body and destroy
                    this.gameObject.AddComponent<Rigidbody>();
                    Destroy(this.gameObject, 2f);

                    Main_control.instance.change_to_game_over();

                    return;
                }

                //新的方块的z轴上的位置 The position on the z axis of the new square
                float new_block_position_z = Main_control.instance.game_obj_last_block.transform.position.z + (space_z / 2);

                //设置新方块的大小和位置 Set the size and position of the new square
                this.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, new_block_size_z);
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, new_block_position_z);

                //生成下落出来的方块 Generate falling blocks
                #region 

                //计算出下落方块的位置 Calculate the position of the falling cube
                int direction = 1;
                if (space_z < 0)
                    direction = -1;
                float cudeEdge = this.transform.position.z + (new_block_size_z / 2f * direction);
                float falling_block_posiiton_z = cudeEdge + falling_block_size_z / 2f * direction;

                //生成下落方块 Generate falling blocks
                GameObject game_obj_falling_block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                game_obj_falling_block.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, falling_block_size_z);
                game_obj_falling_block.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, falling_block_posiiton_z);

                //设置颜色与原来的方块一样 Set the color to be the same as the original square
                game_obj_falling_block.GetComponent<Renderer>().material.color = this.GetComponent<Renderer>().material.color;

                //添加刚体并销毁 Add rigid body and destroy
                game_obj_falling_block.AddComponent<Rigidbody>();
                Destroy(game_obj_falling_block, 2f);
                #endregion

                //play sound
                Audio_control.instance.play_sound_cut();
            }
        }
        else
        {
            //跟上一个方块间隔的x的距离 X distance from the previous square
            float space_x = this.transform.position.x - Main_control.instance.game_obj_last_block.transform.position.x;


            //如果间距相差比较小，就让这块方块跟上一块方块重合，播放音效和特效 If the distance is relatively small, let this block overlap with the previous block and play sound effects and special effects
            if (Mathf.Abs(space_x) < 0.025f)
            {
                //set position
                this.transform.position = new Vector3(Main_control.instance.game_obj_last_block.transform.position.x, this.transform.position.y, Main_control.instance.game_obj_last_block.transform.position.z);

                //play sound
                Audio_control.instance.play_sound_score();

                //play particle
                Main_control.instance.particle_score.gameObject.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 0.05f, this.transform.position.z);
                Main_control.instance.particle_score.gameObject.transform.localScale = this.transform.localScale;
                Main_control.instance.particle_score.Play();
            }
            else
            {

                //新的方块的x的尺寸大小 The x size of the new square
                float new_block_size_x = Main_control.instance.game_obj_last_block.transform.localScale.x - Mathf.Abs(space_x);

                //下落的方块x的大小 The size of the falling square x
                float falling_block_size_x = this.transform.localScale.x - new_block_size_x;

                //这个地方要判断游戏有没有结束 This place has to judge whether the game is over
                if (new_block_size_x <= 0)
                {
                    //添加刚体并销毁 Add rigid body and destroy
                    this.gameObject.AddComponent<Rigidbody>();
                    Destroy(this.gameObject, 2f);

                    Main_control.instance.change_to_game_over();
                    return;
                }

                //新的方块的x轴上的位置 The position on the x axis of the new square
                float new_block_position_x = Main_control.instance.game_obj_last_block.transform.position.x + (space_x / 2);

                //设置新方块的大小和位置 Set the size and position of the new square
                this.transform.localScale = new Vector3(new_block_size_x, this.transform.localScale.y, this.transform.localScale.z);
                this.transform.position = new Vector3(new_block_position_x, this.transform.position.y, this.transform.position.z);

                //生成下落出来的方块 Generate falling blocks
                #region 
                //计算出下落方块的位置 Calculate the position of the falling cube
                int direction = 1;
                if (space_x < 0)
                    direction = -1;
                float cudeEdge = this.transform.position.x + (new_block_size_x / 2f * direction);
                float falling_block_posiiton_x = cudeEdge + falling_block_size_x / 2f * direction;

                //生成下落方块 Generate falling blocks
                GameObject game_obj_falling_block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                game_obj_falling_block.transform.localScale = new Vector3(falling_block_size_x, this.transform.localScale.y, this.transform.localScale.z);
                game_obj_falling_block.transform.position = new Vector3(falling_block_posiiton_x, this.transform.position.y, this.transform.position.z);

                //设置颜色与原来的方块一样 Set the color to be the same as the original square
                game_obj_falling_block.GetComponent<Renderer>().material.color = this.GetComponent<Renderer>().material.color;

                //添加刚体并销毁 Add rigid body and destroy
                game_obj_falling_block.AddComponent<Rigidbody>();
                Destroy(game_obj_falling_block, 2f);
                #endregion

                //play sound
                Audio_control.instance.play_sound_cut();
            }
        }

        //设置最近的一块方块为自己 Set the nearest block as yourself
        Main_control.instance.game_obj_last_block = this.gameObject;

        //新生成一个方块 Create a new block
        Main_control.instance.creat_new_block();

        //去除掉这个代码 Remove this code
        Destroy(this);
    }
}

public enum Move_direction
{
    direction_z,
    direction_x
}
