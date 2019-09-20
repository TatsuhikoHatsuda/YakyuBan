using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : MonoBehaviour {

    // HingeJointコンポーネント
    private HingeJoint myHingeJoint;
    private Rigidbody myRigidbody;

    // batBodyのオブジェクトの当たり判定
    private CapsuleCollider batBody;

    // StrikeZoneのオブジェクトの当たり判定
    private BoxCollider strikeZone;

    // FoulTerritoryのオブジェクト
    private GameObject foulTerritory;

    // ballのスクリプト
    private BallController ballController;

    // GameControllerのスクリプト
    private GameController gameController;

    // 初期角度
    private float defaultAngle = 70;

    // 目標角度
    private float flickAngle = -100;

	// Use this for initialization
	void Start () {
        // コンポーネント取得
        this.myHingeJoint = GetComponent<HingeJoint>();
        this.myRigidbody = GetComponent<Rigidbody>();

        // オブジェクト取得
        this.batBody = transform.Find("BatBody").gameObject.GetComponent<CapsuleCollider>();
        this.strikeZone = GameObject.Find("StrikeZone").GetComponent<BoxCollider>();
        this.foulTerritory = GameObject.Find("FoulTerritory");

        // スクリプトを取得
        this.ballController = GameObject.Find("Ball").GetComponent<BallController>();
        this.gameController = GameObject.Find("GameController").GetComponent<GameController>();

        // 初期角度に設定
        SetAngle(this.defaultAngle);

        // ファールの当たり判定を無効にする
        this.foulTerritory.SetActive(false);

	}
	
	// Update is called once per frame
	void Update () {
        if (this.gameController.isStartingGame) {
            //左矢印キーを押した時左フリッパーを動かす
            if (Input.GetKeyDown(KeyCode.Z)) {
                // 目標角度に持っていく
                SetAngle(this.flickAngle);
            }
            //矢印キー離された時フリッパーを元に戻す
            if (Input.GetKeyUp(KeyCode.Z)) {
                // 初期角度に戻す
                SetAngle(this.defaultAngle);
            }
        }
	}

    //フリッパーの傾きを設定
    void SetAngle(float angle) {
        JointSpring jointSpr = this.myHingeJoint.spring;
        jointSpr.targetPosition = angle;
        this.myHingeJoint.spring = jointSpr;
    }

    //衝突時に呼ばれる関数
    void OnCollisionEnter(Collision other) {
        // ボールに当たった場合
        if (other.gameObject.tag == "BallTag") {
            // 打たれたフラグをtrueにする
            this.ballController.isBatting = true;

            // バットの判定を無効にする
            this.batBody.enabled = false;
            // ストライクゾーンを判定を無効にする
            this.strikeZone.enabled = false;
            // ファールの当たり判定を有効にする
            this.foulTerritory.SetActive(true);
        }
    }

    // 判定を元に戻す
    public void ResetBat() {
        this.batBody.enabled = true;
        this.strikeZone.enabled = true;
        this.foulTerritory.SetActive(false);
    }
}
