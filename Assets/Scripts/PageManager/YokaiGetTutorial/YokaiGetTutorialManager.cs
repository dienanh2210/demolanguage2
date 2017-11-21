﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
public class YokaiGetTutorialManager : Page{
    public static YokaiGetTutorialManager instance;
    public GameObject sprYCircle;
    public GameObject sprICircle;
    public GameObject backgroundCam;
    public Sprite red, blue;
    public Material fireMat,itemMat;
    public GameObject objYokai, objItem;
    [SerializeField]
    private GameObject[] yokai = new GameObject[5];
    [SerializeField]
    private GameObject[] item = new GameObject[2];
    GameObject mapEffect;
    GameObject map;
    GameObject yokaiCam;
    GameObject kindOfObject;
    int yCount = 0;
    int iCount = 0;

    void Awake(){
        instance = this;
    }

    void OnEnable(){
        MapPageManager.instance.SetMapPage (0, 0, -6, RenderMode.ScreenSpaceCamera);

        if (PageData.yokaiID != -1)
        {
            objYokai.SetActive (true);
            objItem.SetActive (false);
        }
        if (PageData.itemID != -1)
        {
            objItem.SetActive (true);
            objYokai.SetActive (false);
        }

        kindOfObject = GameObject.FindGameObjectWithTag ("MapEffect").transform.Find ("MapCircle").transform.Find ("FireMagic").gameObject;

        backgroundCam.GetComponent<CameraAsBackground> ().CameraStart ();
        if (GameObject.FindGameObjectWithTag("ParentTutorial").transform.childCount==0) {
            backgroundCam.transform.SetParent (GameObject.FindGameObjectWithTag("ParentTutorial").transform);
        }
        sprYCircle.transform.localScale = new Vector3 (1.2f, 1.2f,1.2f);
        sprICircle.transform.localScale = new Vector3 (1.2f, 1.2f,1.2f);

        #region Enable the map effect
        mapEffect = GameObject.FindGameObjectWithTag("MapEffect");
        mapEffect.transform.GetChild(0).gameObject.SetActive(true);
        #endregion
        #region Enable the third Camera
        yokaiCam = GameObject.FindGameObjectWithTag ("YokaiCamera");
        yokaiCam.transform.GetChild (0).gameObject.SetActive (true);
        #endregion
        #region Pinch Zoom the map
        map = GameObject.FindGameObjectWithTag ("Map");
        map.transform.GetChild (1).gameObject.SetActive (true);
        map.transform.GetChild (1).gameObject.GetComponent<PinchZoom> ().enabled = false;
        map.transform.GetChild (1).gameObject.GetComponent<lb_drag> ().enabled = false;
        #endregion

        if (objYokai.activeSelf) {
            kindOfObject.GetComponent<MeshRenderer> ().material = fireMat;
            kindOfObject.transform.localScale = new Vector3 (.2f,.2f,.4f);
        }else if (objItem.activeSelf) {
            kindOfObject.GetComponent<MeshRenderer> ().material = itemMat;
            kindOfObject.transform.localScale = new Vector3 (.4f,.2f,.4f);
        }

        FireEffect (true);

        for (int i = 0; i < ApplicationData.YokaiGetTutorialData.Count; i++) {
            yokai [i].GetComponentsInChildren<Text> (true) [0].text = ApplicationData.YokaiGetTutorialData [i].localContents [0].text.ToString ();
           
        }

        for (int j = 0; j < item.Length; j++) {
            item[j].GetComponentsInChildren<Text> (true) [0].text = ApplicationData.GetLocaleText (LocaleType.ItemGetTutorial);
        }

        CircleScale ();
    }

    void OnDisable(){
        objYokai.SetActive (false);
        objItem.SetActive (false);
        mapEffect.transform.GetChild(0).gameObject.SetActive (false);
        MirrorFlipCamera.instance.flipHorizontal = false;
       
    }

    public void CircleScale(){
        sprYCircle.transform.DOScale (new Vector3 (.6f, .6f, .6f), 1f).SetEase (Ease.Linear).SetLoops (-1);
        sprICircle.transform.DOScale (new Vector3 (.6f, .6f, .6f), 1f).SetEase (Ease.Linear).SetLoops (-1);
       
    }

    void FireEffect(bool isDown){

        //on map
        if (isDown) {
            mapEffect.GetComponentsInChildren<Transform> (true) [2].DOLocalMoveZ (-.5f, .5f).SetEase (Ease.Linear).OnComplete (() => {
                FireEffect (!isDown);
            });
        } else {
            mapEffect.GetComponentsInChildren<Transform> (true) [2].DOLocalMoveZ (.3f, .5f).SetEase (Ease.Linear).OnComplete (() => {
                FireEffect (!isDown);
            });
        }
    }

    public void OnClickNext(){
        yCount++;
        if (yCount==yokai.Length) {
            yCount = 0;
            UserData.IsShowedYokaiTutorial = true;
            PageManager.Show(PageType.YokaiGetPage);
            Invoke ("ActiveYokaiGetPage", 1);
            return;
        }
        for (int i = 0; i < yokai.Length; i++) {
            if (i == yCount) {
                yokai [i].SetActive (true); 
            } else {
                yokai [i].SetActive (false);
            }
        }
    }

    void ActiveYokaiGetPage(){
        Debug.Log (GameObject.FindGameObjectWithTag("main")); 
         backgroundCam.transform.SetParent (GameObject.FindGameObjectWithTag("main").transform);
         GameObject.FindGameObjectWithTag ("main").transform.GetChild (0).gameObject.SetActive (false);
         backgroundCam.GetComponent<RectTransform> ().localScale = new Vector3 (1,1,1);
    }

    public void OnClickItem(){
        iCount++;
        if (iCount==item.Length) {
            iCount = 0;
            UserData.IsShowItemTutorial = true;
            PageManager.Show(PageType.YokaiGetPage);
            Invoke ("ActiveYokaiGetPage", 1);
        }
        for (int i = 0; i < item.Length; i++) {
            if (i == iCount) {
                item [i].SetActive (true); 
            } else {
                item [i].SetActive (false);
            }
        }
    }
      
    void Update(){
        if (objYokai.activeSelf) {
            if (sprYCircle.transform.localScale.x <= 0.7) {
                sprYCircle.GetComponent<Image> ().DOFade (0,0.00001f);

            } else {
                sprYCircle.GetComponent<Image> ().DOFade (1, 0.00001f);
            }
        }
        else if (objItem.activeSelf) {
            if (sprICircle.transform.localScale.x <= 0.7) {
                sprICircle.GetComponent<Image> ().DOFade (0,0.00001f);
            } else {
                sprICircle.GetComponent<Image> ().DOFade (1, 0.00001f);
            }
        }

        if (sprICircle.transform.localScale.x <= .8f || sprYCircle.transform.localScale.x <= .8f) {
            sprICircle.GetComponent<Image> ().sprite = red;
            sprYCircle.GetComponent<Image> ().sprite = red;
        } else {
            sprICircle.GetComponent<Image> ().sprite = blue;
            sprYCircle.GetComponent<Image> ().sprite = blue;
        }

        if (ClickablePlane.isClick) {
            DOTween.Pause (sprICircle);
            DOTween.Pause (sprYCircle);
        }
    }

}
