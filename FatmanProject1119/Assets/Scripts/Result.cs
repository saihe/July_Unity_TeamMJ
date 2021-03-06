﻿using UnityEngine;
using System.Collections;
using GameSystems;

public class Result : MonoBehaviour {

    //クリア画面
    private GameObject clearScreen;

    //ゲームオーバー画面
    private GameObject gameOverScreen;

    //ゲームステート
    State state = new State();
    
    //クリア情報
    ClearedStage cs = new ClearedStage();

    //シーンチェンジャー
    ScenChanger sc = new ScenChanger();

	void Start () {
        clearScreen = transform.GetChild(0).gameObject;
        gameOverScreen = transform.GetChild(1).gameObject;

        if(state.getState() == GameState.StageClear)
        {
            cs.setCleared(sc.getStageName(), 1);
            clearScreen.SetActive(true);
            gameOverScreen.SetActive(false);
        }
        else if(state.getState() == GameState.GameOver)
        {
            gameOverScreen.SetActive(true);
            clearScreen.SetActive(false);
        }
	}
	

}
