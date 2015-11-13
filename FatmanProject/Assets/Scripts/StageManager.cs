﻿using UnityEngine;
using System.Collections;
using GameSystems;
using UnityEngine.UI;

public class StageManager : MonoBehaviour {
	
	//残タイム
	int outTime;
	
	//開始時間
	float startTime;
	
	//経過時間
	float parseTime;
	
	//現在のタイム
	private float nowTime;
	//テキスト用の分・秒
	private int minuts;
	private int seconds;
	
	//タイマーオブジェクト
	private GameObject timer;
	
	//ゲームの状態
	State state = new State();
	
	//シーンチェンジャー
	ScenChanger sc = new ScenChanger();

    //プレイヤー
    private GameObject player;
    private GameObject debu;
    private int debuCnt = 0;
	
	
	//リザルトテロップ
	private GameObject resultTelop;
	
	//音
	public AudioClip[] audioSorce;
	private AudioSource audio;
	
	//Enemyやられたカウント
	private int count = 0;
	
	private int t ;
	private int k ;
	
	// Waveプレハブを格納する
	private GameObject[] waves = new GameObject[4];
    int j = 1;

    // 現在のWave
    private int currentWave = 0;

	private bool objTmp = true;
	
	void Start()
	{
		//タイマー関係
		startTime = Time.time;
		//制限時間
		outTime = 180;
		timer = GameObject.Find("Timer");
		
		//ゲームステート
		state.setState(GameState.Playing);
		
		//テロップ
		resultTelop = GameObject.Find("ResultTelop");
		resultTelop.SetActive(false);
		
		//音
		audio = GetComponent<AudioSource>();

        //プレイヤー
        player = GameObject.Find("PlayerSibo");
        debu = (GameObject)Resources.Load("Debu");

        //Wavesをリソースから取得
        for(int i = 0; i < waves.Length; i++)
        {
            waves[i] = (GameObject)Resources.Load("Waves/Wave" + j);
            j++;
        }
        
        //EnemyWaveを呼び出す
        StartCoroutine ("enemyWave");
		
	}
	
	void Update()
	{
		//タイマー
		setTimer();
        if (Input.GetKeyDown("z"))
        {
            GameObject[] wave = new GameObject[4];
            int i = 0;
            foreach (var val in wave)
            {
                wave[i] = GameObject.Find("ObjectPool").transform.GetChild(i).gameObject;
                if (wave[i].activeSelf == true)
                {
                    wave[i].SetActive(false);
                }
                i++;
            }
        }
        if (Input.GetKeyDown("r"))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
    }

    public void Counter(int i)
	{
		count += i;
		print("Count: " + count);
	} 
		
	//タイマー書き換え
	void setTimer()
	{
        if(state.getState() == GameState.Playing)
        {
            if (nowTime >= 0)
            {
                parseTime += Time.deltaTime;
                nowTime = outTime - parseTime;
                if (nowTime <= 0)
                {
                    setResult(false);
                    StartCoroutine(telop());
                }
                mathTime(nowTime);
                timer.GetComponent<Text>().text = "残タイム　" + minuts + ":" + seconds;
            }
        }
    }
	
	//タイマー計算
	void mathTime(float t)
	{
		if (t < 60)
		{
			minuts = 0;
			seconds = (int)t;
		}
		else
		{
			seconds = (int)t;
			seconds %= 60;
			minuts = ((int)t - seconds) / 60;
		}
	}
	
	//ポーズ状態の遷移
	public void setPause(bool p)
	{
		if(p == false)
		{
			//ポーズ中にする
			state.setState(GameState.Pausing);
		}
		else
		{
			//プレイ中にする
			state.setState(GameState.Playing);
		}
	}
	
	//リザルト状態の遷移
	public void setResult(bool c)
	{
		if(c == true)
		{
			//クリア
			int i = 0;
			while (i < 1)
			{
				audio.Stop();
				audio.clip = audioSorce[1];
				//audio.Play();
				//audio.PlayOneShot(audioSorce[1]);
				i++;
			}
			state.setState(GameState.StageClear);
			resultTelop.GetComponent<Text>().text = "ステージクリア";
		}
		else if(c == false)
		{
			//ゲームオーバー
			int i = 0;
			while (i < 1)
			{
				audio.Stop();
				audio.clip = audioSorce[2];
                //audio.Play();
                //audio.PlayOneShot(audioSorce[2]);
                i++;
			}
            StartCoroutine(insDebu());
			state.setState(GameState.GameOver);
			resultTelop.GetComponent<Text>().text = "ゲームオーバー";
		}
	}

    //BMI0でデブを出す
    int rollTime = 0;
    float lf = 0.1f;
    ParticleSystem dp;
    IEnumerator insDebu()
    {
        debuCnt++;
        while(debuCnt == 1)
        {
            GameObject g = (GameObject)Resources.Load("PlayerFog");
            Instantiate(g, player.transform.position, g.transform.rotation);
            yield return new WaitForSeconds(0.4f);
            player.SetActive(false);
            debu = (GameObject)Instantiate(debu, player.transform.position, debu.transform.rotation);
            debu.transform.position = new Vector3(debu.transform.position.x, debu.transform.position.y - 0.5f, debu.transform.position.z);
            yield return new WaitForSeconds(0.5f);
            debuCnt++;
        }
        while(lf < 10)
        {
            debu.transform.Rotate(new Vector3(lf, 0, lf));
            lf += lf;
        }
        yield return new WaitForSeconds(2.5f);
        while (rollTime < 10)
        {
            debu.transform.Rotate(new Vector3(0, 5f, 0));
            yield return new WaitForSeconds(0.1f);
            rollTime++;
        }
        yield break;
    }

    public IEnumerator telop()
	{
		audio.loop = false;
        audio.Play();
		resultTelop.SetActive(true);
		yield return new WaitForSeconds(3.0f);
		resultTelop.SetActive(false);
		sc.toResult();
	}
	
	
	private GameObject objectPool;
	private	IEnumerator enemyWave ()
	{
        if (objTmp)
        {
            objectPool = new GameObject("ObjectPool");
            objectPool.AddComponent<RectTransform>();
            objectPool.AddComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            objectPool.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            objectPool.AddComponent<GraphicRaycaster>();
            objectPool.transform.position = new Vector3(5, 2, 10);

            foreach (GameObject n in waves)
            {
                GameObject childN = Instantiate(n, transform.position, Quaternion.identity) as GameObject;
                childN.SetActive(false);
                childN.transform.parent = objectPool.transform;
                objTmp = false;
            }

            while (true)
            {
                yield return new WaitForSeconds(0.5f);

                GameObject obp = objectPool.transform.GetChild(k).gameObject;
                obp.SetActive(true);
                int temp = obp.transform.childCount;
                t = t + temp;

                // Waveの子要素のEnemyが全て削除されるまで待機する
                while (t > count)
                {
                    yield return new WaitForEndOfFrame();
                }
                k++;
                // 格納されているWaveを全て実行したらステージクリア
                if (objectPool.transform.childCount <= ++currentWave)
                {
                    print("クリア");
                    setResult(true);
                    StartCoroutine(telop());
                    yield break;
                }
            }
        }
    }
}