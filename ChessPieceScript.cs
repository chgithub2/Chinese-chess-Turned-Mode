using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChessPieceScript : MonoBehaviour {

    public GameObject ChessPiece;
    public GameObject ChessPieceText;
    public int ChessPieceID;
    public int ChessPieceLevel;
    public int Red0andBlack1;

    private bool IsChessPieceLiftUp = false;
    private bool IsChessPieceTurnedOver = false;
    private Vector3 MouseTarget;
    private Vector3 FormerPosition;
    private int ChessPiecePosX = 0;
    private int ChessPiecePosY = 0;
    private Vector3 TempPos;

    void Awake ()
    {

    }

	// Use this for initialization
	void Start () {
        //棋子位置初始化
        GameObject Plane = GameObject.Find("Plane");
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (Plane.GetComponent<MainLogic>().ChessPiecePositionArray[i, j, 0] == ChessPieceID)
                {
                    Debug.Log(Convert.ToString(ChessPieceID)+":"+Convert.ToString(j+1)+","+Convert.ToString(i+1));
                    ChessPiecePosX = j;
                    ChessPiecePosY = i;
                    break;
                }
            }
        }
        ChessPiece.transform.position = new Vector3(-3.5f + ChessPiecePosX, 0.2f, -3.5f + ChessPiecePosY);

        //棋子标签隐藏
        ChessPieceText.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (IsChessPieceLiftUp == true)
        {
            //// a 创建射线
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);// 从摄像机发射出一条经过鼠标当前位置的射线
            //                                                            // b 发射射线
            //RaycastHit hitInfo = new RaycastHit();
            //if (Physics.Raycast(ray, out hitInfo))
            //{
            //    target = hitInfo.point;
            //    target.y = 0.5f;// 地面的y坐标为0，而cube的y必须为0.5f才能看得见
            //}
            MouseTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            MouseTarget.y = 0.5f;
            ChessPiece.transform.position = MouseTarget;
        }
    }

    void OnMouseDown()
    {
        GameObject Plane = GameObject.Find("Plane");
        int TargetPosX;
        int TargetPosY;
        int TargetPositionState;
        int FormerPositionState;
        int TargetID;

        if (Plane.GetComponent<MainLogic>().ChessAttachedToMouseID == 0 || Plane.GetComponent<MainLogic>().ChessAttachedToMouseID == ChessPieceID)//鼠标上没有粘附棋子 或 粘附的是本棋子
        {
            if (Plane.GetComponent<MainLogic>().ChessAttachedToMouseID == 0 && IsChessPieceTurnedOver == false)//鼠标上没有粘附棋子 且 棋子未翻开
            {
                IsChessPieceTurnedOver = true;
                ChessPieceText.SetActive(true);
                Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 1;
            }
            else//棋子已翻开
            {
                if (Plane.GetComponent<MainLogic>().ChessAttachedToMouseID == 0 && IsChessPieceLiftUp == false)//鼠标上没有粘附棋子 且 棋子未抬起
                {
                    IsChessPieceLiftUp = true;
                    Plane.GetComponent<MainLogic>().ChessAttachedToMouseID = ChessPieceID;//鼠标上粘附本棋子
                    FormerPosition = ChessPiece.transform.position;
                }
                else//棋子已抬起
                {
                    if (System.Math.Abs(MouseTarget.x - FormerPosition.x) == 0 && System.Math.Abs(MouseTarget.z - FormerPosition.z) == 0)//不移动
                    {
                        IsChessPieceLiftUp = true;
                    }
                    else if (System.Math.Abs(MouseTarget.x - FormerPosition.x) >= System.Math.Abs(MouseTarget.z - FormerPosition.z))//左右走
                    {
                        if ((MouseTarget.x - FormerPosition.x) > 0)//右走
                        {
                            Debug.Log("右走");
                            if (ChessPiecePosX + 1 > 7)//超出边界
                            {
                                IsChessPieceLiftUp = true;
                            }
                            else//没有超出边界
                            {
                                Debug.Log("没有超出边界");
                                TargetPosX = ChessPiecePosX + 1;
                                TargetPosY = ChessPiecePosY;
                                TargetPositionState = Plane.GetComponent<MainLogic>().CheckerBoardStateArray[TargetPosX, TargetPosY];
                                Debug.Log(TargetPositionState);
                                FormerPositionState = Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY];

                                if (TargetPositionState == 0)//目标位置有棋子，未翻开
                                {
                                    //物理移动
                                    TempPos = FormerPosition + new Vector3(1, 0, 0);
                                    TempPos.y = 0.4f;
                                    ChessPiece.transform.position = TempPos;

                                    //原位置处理
                                    if (FormerPositionState == 1)//原位置只有一枚棋子
                                    {
                                        Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 0] = 0;
                                        Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 3;//原位置现在无棋子
                                    }
                                    else if (FormerPositionState == 2)//原位置有两枚棋子
                                    {
                                        Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 1] = 0;//原位置第二层现在无棋子
                                        Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 0;//原位置现在有棋子，未翻开
                                    }

                                    //逻辑移动
                                    ChessPiecePosX += 1;

                                    //新位置处理
                                    Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 1] = ChessPieceID;
                                    Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 2;//新位置现在有两枚棋子

                                    //棋子落下
                                    IsChessPieceLiftUp = false;
                                    Plane.GetComponent<MainLogic>().ChessAttachedToMouseID = 0;
                                }
                                else if (TargetPositionState == 1)//目标位置有棋子，已翻开
                                {
                                    Debug.Log("目标位置有棋子，已翻开");
                                    Debug.Log(ChessPiecePosX);
                                    Debug.Log(ChessPiecePosY);
                                    Debug.Log(TargetPosX);
                                    Debug.Log(TargetPosY);
                                    TargetID = Plane.GetComponent<MainLogic>().ChessPiecePositionArray[TargetPosY, TargetPosX, 0];
                                    GameObject TargetChessPiece = GameObject.Find("ChessPiece" + Convert.ToString(TargetID));
                                    Debug.Log(TargetID);
                                    Debug.Log(TargetChessPiece.GetComponent<ChessPieceScript>().ChessPieceLevel);

                                    if (TargetChessPiece.GetComponent<ChessPieceScript>().ChessPieceLevel <= ChessPieceLevel 
                                        && TargetChessPiece.GetComponent<ChessPieceScript>().Red0andBlack1 != Red0andBlack1)//目标比自身小或相等
                                    {
                                        //物理移动
                                        TempPos = FormerPosition + new Vector3(1, 0, 0);
                                        TempPos.y = 0.2f;
                                        ChessPiece.transform.position = TempPos;

                                        //原位置处理
                                        if (FormerPositionState == 1)//原位置只有一枚棋子
                                        {
                                            Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 0] = 0;
                                            Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 3;//原位置现在无棋子
                                        }
                                        else if (FormerPositionState == 2)//原位置有两枚棋子
                                        {
                                            Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 1] = 0;//原位置第二层现在无棋子
                                            Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 0;//原位置现在有棋子，未翻开
                                        }

                                        //逻辑移动
                                        ChessPiecePosX += 1;

                                        //新位置处理
                                        Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 0] = ChessPieceID;
                                        Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 1;//新位置现在有一枚棋子，已翻开

                                        //目标棋子销毁
                                        TargetChessPiece.SetActive(false);

                                        //棋子落下
                                        IsChessPieceLiftUp = false;
                                        Plane.GetComponent<MainLogic>().ChessAttachedToMouseID = 0;
                                    }
                                    else//目标比自身大
                                    {
                                        IsChessPieceLiftUp = true;
                                    }
                                }
                                else if (TargetPositionState == 2)//目标位置有两枚棋子
                                {
                                    TargetID = Plane.GetComponent<MainLogic>().ChessPiecePositionArray[TargetPosY, TargetPosX, 1];
                                    GameObject TargetChessPiece = GameObject.Find("ChessPiece" + Convert.ToString(TargetID));

                                    if (TargetChessPiece.GetComponent<ChessPieceScript>().ChessPieceLevel <= ChessPieceLevel
                                        && TargetChessPiece.GetComponent<ChessPieceScript>().Red0andBlack1 != Red0andBlack1)//目标比自身小或相等
                                    {
                                        //物理移动
                                        TempPos = FormerPosition + new Vector3(1, 0, 0);
                                        TempPos.y = 0.4f;
                                        ChessPiece.transform.position = TempPos;

                                        //原位置处理
                                        if (FormerPositionState == 1)//原位置只有一枚棋子
                                        {
                                            Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 0] = 0;
                                            Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 3;//原位置现在无棋子
                                        }
                                        else if (FormerPositionState == 2)//原位置有两枚棋子
                                        {
                                            Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 1] = 0;//原位置第二层现在无棋子
                                            Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 0;//原位置现在有棋子，未翻开
                                        }

                                        //逻辑移动
                                        ChessPiecePosX += 1;

                                        //新位置处理
                                        Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 1] = ChessPieceID;
                                        Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 2;//新位置现在有两枚棋子

                                        //目标棋子销毁
                                        TargetChessPiece.SetActive(false);

                                        //棋子落下
                                        IsChessPieceLiftUp = false;
                                        Plane.GetComponent<MainLogic>().ChessAttachedToMouseID = 0;
                                    }
                                    else//目标比自身大
                                    {
                                        IsChessPieceLiftUp = true;
                                    }
                                }
                                else if (TargetPositionState == 3)//目标位置无棋子
                                {
                                    //物理移动
                                    TempPos = FormerPosition + new Vector3(1, 0, 0);
                                    TempPos.y = 0.2f;
                                    ChessPiece.transform.position = TempPos;

                                    //原位置处理
                                    if (FormerPositionState == 1)//原位置只有一枚棋子
                                    {
                                        Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 0] = 0;
                                        Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 3;//原位置现在无棋子
                                    }
                                    else if (FormerPositionState == 2)//原位置有两枚棋子
                                    {
                                        Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 1] = 0;//原位置第二层现在无棋子
                                        Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 0;//原位置现在有棋子，未翻开
                                    }

                                    //逻辑移动
                                    ChessPiecePosX += 1;

                                    //新位置处理
                                    Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 0] = ChessPieceID;
                                    Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 2;//新位置现在有一枚棋子，已翻开

                                    //棋子落下
                                    IsChessPieceLiftUp = false;
                                    Plane.GetComponent<MainLogic>().ChessAttachedToMouseID = 0;
                                }
                            }
                        }
                        else//左走
                        {
                            if (ChessPiecePosX - 1 < 0)//超出边界
                            {
                                IsChessPieceLiftUp = true;
                            }
                            else//没有超出边界
                            {
                                TargetPosX = ChessPiecePosX - 1;
                                TargetPosY = ChessPiecePosY;
                                TargetPositionState = Plane.GetComponent<MainLogic>().CheckerBoardStateArray[TargetPosX, TargetPosY];
                                FormerPositionState = Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY];

                                if (TargetPositionState == 0)//目标位置有棋子，未翻开
                                {
                                    //物理移动
                                    TempPos = FormerPosition + new Vector3(-1, 0, 0);
                                    TempPos.y = 0.4f;
                                    ChessPiece.transform.position = TempPos;

                                    //原位置处理
                                    if (FormerPositionState == 1)//原位置只有一枚棋子
                                    {
                                        Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 0] = 0;
                                        Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 3;//原位置现在无棋子
                                    }
                                    else if (FormerPositionState == 2)//原位置有两枚棋子
                                    {
                                        Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 1] = 0;//原位置第二层现在无棋子
                                        Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 0;//原位置现在有棋子，未翻开
                                    }

                                    //逻辑移动
                                    ChessPiecePosX -= 1;

                                    //新位置处理
                                    Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 1] = ChessPieceID;
                                    Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 2;//新位置现在有两枚棋子

                                    //棋子落下
                                    IsChessPieceLiftUp = false;
                                    Plane.GetComponent<MainLogic>().ChessAttachedToMouseID = 0;
                                }
                                else if (TargetPositionState == 1)//目标位置有棋子，已翻开
                                {
                                    TargetID = Plane.GetComponent<MainLogic>().ChessPiecePositionArray[TargetPosY, TargetPosX, 0];
                                    GameObject TargetChessPiece = GameObject.Find("ChessPiece" + Convert.ToString(TargetID));

                                    if (TargetChessPiece.GetComponent<ChessPieceScript>().ChessPieceLevel <= ChessPieceLevel
                                        && TargetChessPiece.GetComponent<ChessPieceScript>().Red0andBlack1 != Red0andBlack1)//目标比自身小或相等
                                    {
                                        //物理移动
                                        TempPos = FormerPosition + new Vector3(-1, 0, 0);
                                        TempPos.y = 0.2f;
                                        ChessPiece.transform.position = TempPos;

                                        //原位置处理
                                        if (FormerPositionState == 1)//原位置只有一枚棋子
                                        {
                                            Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 0] = 0;
                                            Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 3;//原位置现在无棋子
                                        }
                                        else if (FormerPositionState == 2)//原位置有两枚棋子
                                        {
                                            Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 1] = 0;//原位置第二层现在无棋子
                                            Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 0;//原位置现在有棋子，未翻开
                                        }

                                        //逻辑移动
                                        ChessPiecePosX -= 1;

                                        //新位置处理
                                        Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 0] = ChessPieceID;
                                        Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 1;//新位置现在有一枚棋子，已翻开

                                        //目标棋子销毁
                                        TargetChessPiece.SetActive(false);

                                        //棋子落下
                                        IsChessPieceLiftUp = false;
                                        Plane.GetComponent<MainLogic>().ChessAttachedToMouseID = 0;
                                    }
                                    else//目标比自身大
                                    {
                                        IsChessPieceLiftUp = true;
                                    }
                                }
                                else if (TargetPositionState == 2)//目标位置有两枚棋子
                                {
                                    TargetID = Plane.GetComponent<MainLogic>().ChessPiecePositionArray[TargetPosY, TargetPosX, 1];
                                    GameObject TargetChessPiece = GameObject.Find("ChessPiece" + Convert.ToString(TargetID));

                                    if (TargetChessPiece.GetComponent<ChessPieceScript>().ChessPieceLevel <= ChessPieceLevel
                                        && TargetChessPiece.GetComponent<ChessPieceScript>().Red0andBlack1 != Red0andBlack1)//目标比自身小或相等
                                    {
                                        //物理移动
                                        TempPos = FormerPosition + new Vector3(-1, 0, 0);
                                        TempPos.y = 0.4f;
                                        ChessPiece.transform.position = TempPos;

                                        //原位置处理
                                        if (FormerPositionState == 1)//原位置只有一枚棋子
                                        {
                                            Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 0] = 0;
                                            Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 3;//原位置现在无棋子
                                        }
                                        else if (FormerPositionState == 2)//原位置有两枚棋子
                                        {
                                            Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 1] = 0;//原位置第二层现在无棋子
                                            Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 0;//原位置现在有棋子，未翻开
                                        }

                                        //逻辑移动
                                        ChessPiecePosX -= 1;

                                        //新位置处理
                                        Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 1] = ChessPieceID;
                                        Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 2;//新位置现在有两枚棋子

                                        //目标棋子销毁
                                        TargetChessPiece.SetActive(false);

                                        //棋子落下
                                        IsChessPieceLiftUp = false;
                                        Plane.GetComponent<MainLogic>().ChessAttachedToMouseID = 0;
                                    }
                                    else//目标比自身大
                                    {
                                        IsChessPieceLiftUp = true;
                                    }
                                }
                                else if (TargetPositionState == 3)//目标位置无棋子
                                {
                                    //物理移动
                                    TempPos = FormerPosition + new Vector3(-1, 0, 0);
                                    TempPos.y = 0.2f;
                                    ChessPiece.transform.position = TempPos;

                                    //原位置处理
                                    if (FormerPositionState == 1)//原位置只有一枚棋子
                                    {
                                        Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 0] = 0;
                                        Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 3;//原位置现在无棋子
                                    }
                                    else if (FormerPositionState == 2)//原位置有两枚棋子
                                    {
                                        Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 1] = 0;//原位置第二层现在无棋子
                                        Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 0;//原位置现在有棋子，未翻开
                                    }

                                    //逻辑移动
                                    ChessPiecePosX -= 1;

                                    //新位置处理
                                    Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 0] = ChessPieceID;
                                    Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 2;//新位置现在有一枚棋子，已翻开

                                    //棋子落下
                                    IsChessPieceLiftUp = false;
                                    Plane.GetComponent<MainLogic>().ChessAttachedToMouseID = 0;
                                }
                            }
                        }
                    }
                    else if (System.Math.Abs(MouseTarget.z - FormerPosition.z) > System.Math.Abs(MouseTarget.x - FormerPosition.x))//上下走
                    {
                        if ((MouseTarget.z - FormerPosition.z) > 0)//上走
                        {
                            if (ChessPiecePosY + 1 > 7)//超出边界
                            {
                                IsChessPieceLiftUp = true;
                            }
                            else//没有超出边界
                            {
                                TargetPosX = ChessPiecePosX;
                                TargetPosY = ChessPiecePosY + 1;
                                TargetPositionState = Plane.GetComponent<MainLogic>().CheckerBoardStateArray[TargetPosX, TargetPosY];
                                FormerPositionState = Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY];

                                if (TargetPositionState == 0)//目标位置有棋子，未翻开
                                {
                                    //物理移动
                                    TempPos = FormerPosition + new Vector3(0, 0, 1);
                                    TempPos.y = 0.4f;
                                    ChessPiece.transform.position = TempPos;

                                    //原位置处理
                                    if (FormerPositionState == 1)//原位置只有一枚棋子
                                    {
                                        Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 0] = 0;
                                        Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 3;//原位置现在无棋子
                                    }
                                    else if (FormerPositionState == 2)//原位置有两枚棋子
                                    {
                                        Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 1] = 0;//原位置第二层现在无棋子
                                        Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 0;//原位置现在有棋子，未翻开
                                    }

                                    //逻辑移动
                                    ChessPiecePosY += 1;

                                    //新位置处理
                                    Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 1] = ChessPieceID;
                                    Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 2;//新位置现在有两枚棋子

                                    //棋子落下
                                    IsChessPieceLiftUp = false;
                                    Plane.GetComponent<MainLogic>().ChessAttachedToMouseID = 0;
                                }
                                else if (TargetPositionState == 1)//目标位置有棋子，已翻开
                                {
                                    TargetID = Plane.GetComponent<MainLogic>().ChessPiecePositionArray[TargetPosY, TargetPosX, 0];
                                    GameObject TargetChessPiece = GameObject.Find("ChessPiece" + Convert.ToString(TargetID));

                                    if (TargetChessPiece.GetComponent<ChessPieceScript>().ChessPieceLevel <= ChessPieceLevel
                                        && TargetChessPiece.GetComponent<ChessPieceScript>().Red0andBlack1 != Red0andBlack1)//目标比自身小或相等
                                    {
                                        //物理移动
                                        TempPos = FormerPosition + new Vector3(0, 0, 1);
                                        TempPos.y = 0.2f;
                                        ChessPiece.transform.position = TempPos;

                                        //原位置处理
                                        if (FormerPositionState == 1)//原位置只有一枚棋子
                                        {
                                            Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 0] = 0;
                                            Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 3;//原位置现在无棋子
                                        }
                                        else if (FormerPositionState == 2)//原位置有两枚棋子
                                        {
                                            Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 1] = 0;//原位置第二层现在无棋子
                                            Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 0;//原位置现在有棋子，未翻开
                                        }

                                        //逻辑移动
                                        ChessPiecePosY += 1;

                                        //新位置处理
                                        Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 0] = ChessPieceID;
                                        Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 1;//新位置现在有一枚棋子，已翻开

                                        //目标棋子销毁
                                        TargetChessPiece.SetActive(false);

                                        //棋子落下
                                        IsChessPieceLiftUp = false;
                                        Plane.GetComponent<MainLogic>().ChessAttachedToMouseID = 0;
                                    }
                                    else//目标比自身大
                                    {
                                        IsChessPieceLiftUp = true;
                                    }
                                }
                                else if (TargetPositionState == 2)//目标位置有两枚棋子
                                {
                                    TargetID = Plane.GetComponent<MainLogic>().ChessPiecePositionArray[TargetPosY, TargetPosX, 1];
                                    GameObject TargetChessPiece = GameObject.Find("ChessPiece" + Convert.ToString(TargetID));

                                    if (TargetChessPiece.GetComponent<ChessPieceScript>().ChessPieceLevel <= ChessPieceLevel
                                        && TargetChessPiece.GetComponent<ChessPieceScript>().Red0andBlack1 != Red0andBlack1)//目标比自身小或相等
                                    {
                                        //物理移动
                                        TempPos = FormerPosition + new Vector3(0, 0, 1);
                                        TempPos.y = 0.4f;
                                        ChessPiece.transform.position = TempPos;

                                        //原位置处理
                                        if (FormerPositionState == 1)//原位置只有一枚棋子
                                        {
                                            Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 0] = 0;
                                            Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 3;//原位置现在无棋子
                                        }
                                        else if (FormerPositionState == 2)//原位置有两枚棋子
                                        {
                                            Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 1] = 0;//原位置第二层现在无棋子
                                            Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 0;//原位置现在有棋子，未翻开
                                        }

                                        //逻辑移动
                                        ChessPiecePosY += 1;

                                        //新位置处理
                                        Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 1] = ChessPieceID;
                                        Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 2;//新位置现在有两枚棋子

                                        //目标棋子销毁
                                        TargetChessPiece.SetActive(false);

                                        //棋子落下
                                        IsChessPieceLiftUp = false;
                                        Plane.GetComponent<MainLogic>().ChessAttachedToMouseID = 0;
                                    }
                                    else//目标比自身大
                                    {
                                        IsChessPieceLiftUp = true;
                                    }
                                }
                                else if (TargetPositionState == 3)//目标位置无棋子
                                {
                                    //物理移动
                                    TempPos = FormerPosition + new Vector3(0, 0, 1);
                                    TempPos.y = 0.2f;
                                    ChessPiece.transform.position = TempPos;

                                    //原位置处理
                                    if (FormerPositionState == 1)//原位置只有一枚棋子
                                    {
                                        Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 0] = 0;
                                        Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 3;//原位置现在无棋子
                                    }
                                    else if (FormerPositionState == 2)//原位置有两枚棋子
                                    {
                                        Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 1] = 0;//原位置第二层现在无棋子
                                        Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 0;//原位置现在有棋子，未翻开
                                    }

                                    //逻辑移动
                                    ChessPiecePosY += 1;

                                    //新位置处理
                                    Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 0] = ChessPieceID;
                                    Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 2;//新位置现在有一枚棋子，已翻开

                                    //棋子落下
                                    IsChessPieceLiftUp = false;
                                    Plane.GetComponent<MainLogic>().ChessAttachedToMouseID = 0;
                                }
                            }
                        }
                        else//下走
                        {
                            if (ChessPiecePosY - 1 < 0)//超出边界
                            {
                                IsChessPieceLiftUp = true;
                            }
                            else//没有超出边界
                            {
                                TargetPosX = ChessPiecePosX;
                                TargetPosY = ChessPiecePosY - 1;
                                TargetPositionState = Plane.GetComponent<MainLogic>().CheckerBoardStateArray[TargetPosX, TargetPosY];
                                FormerPositionState = Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY];

                                if (TargetPositionState == 0)//目标位置有棋子，未翻开
                                {
                                    //物理移动
                                    TempPos = FormerPosition + new Vector3(0, 0, -1);
                                    TempPos.y = 0.4f;
                                    ChessPiece.transform.position = TempPos;

                                    //原位置处理
                                    if (FormerPositionState == 1)//原位置只有一枚棋子
                                    {
                                        Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 0] = 0;
                                        Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 3;//原位置现在无棋子
                                    }
                                    else if (FormerPositionState == 2)//原位置有两枚棋子
                                    {
                                        Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 1] = 0;//原位置第二层现在无棋子
                                        Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 0;//原位置现在有棋子，未翻开
                                    }

                                    //逻辑移动
                                    ChessPiecePosY -= 1;

                                    //新位置处理
                                    Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 1] = ChessPieceID;
                                    Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 2;//新位置现在有两枚棋子

                                    //棋子落下
                                    IsChessPieceLiftUp = false;
                                    Plane.GetComponent<MainLogic>().ChessAttachedToMouseID = 0;
                                }
                                else if (TargetPositionState == 1)//目标位置有棋子，已翻开
                                {
                                    TargetID = Plane.GetComponent<MainLogic>().ChessPiecePositionArray[TargetPosY, TargetPosX, 0];
                                    GameObject TargetChessPiece = GameObject.Find("ChessPiece" + Convert.ToString(TargetID));

                                    if (TargetChessPiece.GetComponent<ChessPieceScript>().ChessPieceLevel <= ChessPieceLevel
                                        && TargetChessPiece.GetComponent<ChessPieceScript>().Red0andBlack1 != Red0andBlack1)//目标比自身小或相等
                                    {
                                        //物理移动
                                        TempPos = FormerPosition + new Vector3(0, 0, -1);
                                        TempPos.y = 0.2f;
                                        ChessPiece.transform.position = TempPos;

                                        //原位置处理
                                        if (FormerPositionState == 1)//原位置只有一枚棋子
                                        {
                                            Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 0] = 0;
                                            Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 3;//原位置现在无棋子
                                        }
                                        else if (FormerPositionState == 2)//原位置有两枚棋子
                                        {
                                            Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 1] = 0;//原位置第二层现在无棋子
                                            Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 0;//原位置现在有棋子，未翻开
                                        }

                                        //逻辑移动
                                        ChessPiecePosY -= 1;

                                        //新位置处理
                                        Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 0] = ChessPieceID;
                                        Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 1;//新位置现在有一枚棋子，已翻开

                                        //目标棋子销毁
                                        TargetChessPiece.SetActive(false);

                                        //棋子落下
                                        IsChessPieceLiftUp = false;
                                        Plane.GetComponent<MainLogic>().ChessAttachedToMouseID = 0;
                                    }
                                    else//目标比自身大
                                    {
                                        IsChessPieceLiftUp = true;
                                    }
                                }
                                else if (TargetPositionState == 2)//目标位置有两枚棋子
                                {
                                    TargetID = Plane.GetComponent<MainLogic>().ChessPiecePositionArray[TargetPosY, TargetPosX, 1];
                                    GameObject TargetChessPiece = GameObject.Find("ChessPiece" + Convert.ToString(TargetID));

                                    if (TargetChessPiece.GetComponent<ChessPieceScript>().ChessPieceLevel <= ChessPieceLevel
                                        && TargetChessPiece.GetComponent<ChessPieceScript>().Red0andBlack1 != Red0andBlack1)//目标比自身小或相等
                                    {
                                        //物理移动
                                        TempPos = FormerPosition + new Vector3(0, 0, -1);
                                        TempPos.y = 0.4f;
                                        ChessPiece.transform.position = TempPos;

                                        //原位置处理
                                        if (FormerPositionState == 1)//原位置只有一枚棋子
                                        {
                                            Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 0] = 0;
                                            Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 3;//原位置现在无棋子
                                        }
                                        else if (FormerPositionState == 2)//原位置有两枚棋子
                                        {
                                            Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 1] = 0;//原位置第二层现在无棋子
                                            Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 0;//原位置现在有棋子，未翻开
                                        }

                                        //逻辑移动
                                        ChessPiecePosY -= 1;

                                        //新位置处理
                                        Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 1] = ChessPieceID;
                                        Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 2;//新位置现在有两枚棋子

                                        //目标棋子销毁
                                        TargetChessPiece.SetActive(false);

                                        //棋子落下
                                        IsChessPieceLiftUp = false;
                                        Plane.GetComponent<MainLogic>().ChessAttachedToMouseID = 0;
                                    }
                                    else//目标比自身大
                                    {
                                        IsChessPieceLiftUp = true;
                                    }
                                }
                                else if (TargetPositionState == 3)//目标位置无棋子
                                {
                                    //物理移动
                                    TempPos = FormerPosition + new Vector3(0, 0, -1);
                                    TempPos.y = 0.2f;
                                    ChessPiece.transform.position = TempPos;

                                    //原位置处理
                                    if (FormerPositionState == 1)//原位置只有一枚棋子
                                    {
                                        Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 0] = 0;
                                        Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 3;//原位置现在无棋子
                                    }
                                    else if (FormerPositionState == 2)//原位置有两枚棋子
                                    {
                                        Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 1] = 0;//原位置第二层现在无棋子
                                        Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 0;//原位置现在有棋子，未翻开
                                    }

                                    //逻辑移动
                                    ChessPiecePosY -= 1;

                                    //新位置处理
                                    Plane.GetComponent<MainLogic>().ChessPiecePositionArray[ChessPiecePosX, ChessPiecePosY, 0] = ChessPieceID;
                                    Plane.GetComponent<MainLogic>().CheckerBoardStateArray[ChessPiecePosX, ChessPiecePosY] = 2;//新位置现在有一枚棋子，已翻开

                                    //棋子落下
                                    IsChessPieceLiftUp = false;
                                    Plane.GetComponent<MainLogic>().ChessAttachedToMouseID = 0;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
