﻿using UnityEngine;
using System.Collections;
using GameSystems;

public class RotateCamera : MonoBehaviour {

    private GameObject debu;

    private GameObject target;

    private Vector3 defaultPosition;

    private Vector3 defaultAngles;

    private Vector3 rotate;

    private Vector3 approachPoint;

    private Vector3 leavePoint;

    private GameObject transformFog;

    public TouchPoint touchPoint;

    State state = new State();

    private bool skip;

    void Awake()
    {
        //ステート
        state.setState(GameState.NotPlaying);
        //デブ
        GameObject g = (GameObject)Resources.Load("Debu");
        debu = (GameObject)Instantiate(g, g.transform.position, g.transform.rotation);

        //プレイヤー
        target = GameObject.Find("PlayerSibo");

        //ポジション
        defaultPosition = transform.position;
        defaultAngles = transform.eulerAngles;
        rotate = new Vector3(0, 10f, 0);
        approachPoint = target.transform.position - transform.position;

        //フォグ
        g = (GameObject)Resources.Load("TransformFog");
        transformFog =
        (GameObject)Instantiate(g, g.transform.position, g.transform.rotation);
        transformFog.SetActive(false);

        //スキップ
        skip = false;

        //タッチパッド
        touchPoint = FindObjectOfType<TouchPoint>();
    }

    IEnumerator Start()
    {
        target.SetActive(false);
        while (true)
        {
            StartCoroutine(approach());
            yield return new WaitForSeconds(1.0f);
            StartCoroutine(rotation());
            yield return new WaitForSeconds(4.0f);
            StartCoroutine(leave());
            yield break;
        }
    }
    int i = 0;
    void Update()
    {
        if(i == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                i++;
                skip = true;
                debu.SetActive(false);
                transformFog.SetActive(false);
                transform.eulerAngles = defaultAngles;
                transform.position = defaultPosition;
                StartCoroutine(skipOk());
            }
        }

        if(skip == true)
        {
        }
    }
    
    IEnumerator skipOk()
    {
        target.SetActive(true);
        target.GetComponent<BMIManager>().getSkillButton();
        touchPoint.setController(target);
        yield return new WaitForSeconds(0.3f);
        state.setState(GameState.Playing);
    }

    IEnumerator approach()
    {
        while (true)
        {
            if (skip == true)
            {
                yield break;
            }

            yield return new WaitForSeconds(0.01f);
            if(transform.position.z < -3)
            {
                //print("近づく");
                transform.Translate(approachPoint * Time.deltaTime, Space.World);
            }
            else
            {
                //print("近づかない");
                yield break;
            }
        }
    }
    IEnumerator rotation()
    {
        while (true)
        {
            if (skip == true)
            {
                yield break;
            }

            yield return new WaitForSeconds(0.0001f);
            //print("回る");
            transform.RotateAround(target.transform.position, rotate, 2f);
            if (transform.rotation.y > 0.9f)
            {
                transformFog.SetActive(true);
                yield return new WaitForSeconds(0.01f);
                debu.SetActive(false);
                target.SetActive(true);
                target.GetComponent<BMIManager>().getSkillButton();
            }
            else if(transform.eulerAngles.y >= 356f)
            {
                transform.eulerAngles = defaultAngles;
                transformFog.SetActive(false);
                yield break;
            }
        }
    }

    IEnumerator leave()
    {
        leavePoint = defaultPosition - transform.position;
        while (true)
        {
            if (skip == true)
            {
                yield break;
            }

            yield return new WaitForSeconds(0.01f);
            if (transform.position.z > -9.5)
            {
                //print("離れる");
                transform.Translate(leavePoint * Time.deltaTime, Space.World);
            }
            else
            {
                //print("離れない");
                transform.position = defaultPosition;
                state.setState(GameState.Playing);
                yield break;
            }
        }

    }
}
