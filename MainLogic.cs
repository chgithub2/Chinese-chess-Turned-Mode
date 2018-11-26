using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLogic : MonoBehaviour {

    public int[, ] CheckerBoardStateArray;
    //0 = 有棋子，未翻开
    //1 = 有棋子，已翻开
    //2 = 有两枚棋子
    //3 = 无棋子
    public int[, , ] ChessPiecePositionArray;
    public int ChessAttachedToMouseID = 0;

    private IList<int> TempList = new List<int>();

    void Awake ()
    {
        //棋盘状态初始化
        CheckerBoardStateArray = new int[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                CheckerBoardStateArray[i, j] = 0;
            }
        }

        //棋子位置初始化
        ChessPiecePositionArray = new int[8, 8, 2];
        for (int i = 0; i < 64; i++)
        {
            TempList.Add(i + 1);
        }
        int TempIndex;
        System.Random random = new System.Random();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                TempIndex = random.Next(0, TempList.Count);
                ChessPiecePositionArray[i, j, 0] = TempList[TempIndex];
                ChessPiecePositionArray[i, j, 1] = 0;
                TempList.RemoveAt(TempIndex);
                Debug.Log("第"+Convert.ToString(i+1)+"行"+ "第" + Convert.ToString(j+1) + "个" + Convert.ToString(ChessPiecePositionArray[i, j, 0]));
            }
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}
}
