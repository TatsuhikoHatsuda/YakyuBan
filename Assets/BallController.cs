using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

    // Rigidbodyコンポーネント
    private Rigidbody myRigidbody;

    // ボールが衝突した相手のタグ
    public string colliderTag = "";

    // 打たれた判定
    public bool isBatting = false;

    // 判定エリアに衝突したか
    private bool hitChecked = false;

    // 投げる合図
    public bool isThrowing = false;

	// Use this for initialization
    void Start() {
        // コンポーネントを取得
        this.myRigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {

        //左矢印キーを押した時左フリッパーを動かす
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            this.transform.position = new Vector3(0f, 1.1f, 84.8f);
        }

        // 投げていい合図が来たら投げる
        if (this.isThrowing) {
            StartCoroutine("ThrowBall");
            this.isThrowing = false;
        }

        // 打たれた後何もないところで停止した場合
        if (this.myRigidbody.IsSleeping() && this.isBatting && !this.hitChecked) {
            // シングルヒット扱いにする
            this.colliderTag = "SingleHitTag";
            this.isBatting = false;
        }
	}

    // 衝突時実行
    void OnTriggerEnter(Collider other) {
        // 衝突した相手のタグを保存
        if (other.gameObject.tag != "Untagged" && this.colliderTag == "" && !this.hitChecked ) {
            this.colliderTag = other.gameObject.tag;
            this.hitChecked = true;
        }
    }

    // ボールを元に戻す
    public void ResetBall() {
        this.transform.position = new Vector3(0f, 1.1f, 21.8f);
        this.myRigidbody.velocity = Vector3.zero;
        this.myRigidbody.isKinematic = false;
        this.hitChecked = false;
        this.colliderTag = "";
        this.isBatting = false;
    }

    // ボールを投げる
    IEnumerator ThrowBall() {
        // ボールを投げるまでの時間を決める
        float throwTime = Random.Range(0.8f, 1.5f);

        // ボールの投げる速度を決める
        float throwVelocity = Random.Range(0, 3) * 1000.0f + 2500.0f;

        // 投げるまで待つ
        yield return new WaitForSeconds(throwTime);

        this.myRigidbody.AddForce(this.transform.forward * -1 * throwVelocity);

    }
}