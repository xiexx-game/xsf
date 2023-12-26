//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/PacMan/MonoEnergyBean.cs
// 作者：Xoen Xie
// 时间：2023/06/21
// 描述：能量豆 脚本
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;


public class MonoEnergyBean : MonoBehaviour
{
    public SpriteRenderer Image;
    public float RotaSpeed;

    public Color Start;
    public Color End;

    public float ColorSpeed;

    private float change;

    private int param = 1;

    void Awake()
    {
        Image.color = Start;
        param = 1;
        change = 0;
    }

    void Update()
    {
        var current = transform.localRotation.eulerAngles;
        current.y += Time.deltaTime * RotaSpeed;
        transform.localRotation = Quaternion.Euler(current);

        change += param * Time.deltaTime * ColorSpeed;
        Image.color = Color.Lerp(Start, End, change);
        if(change >= 1.0f)
        {
            change = 1.0f;
            param *= -1;
        }
        else if(change <= 0.0f)
        {
            change = 0;
            param *= -1;
        }
    }
}