using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    // タイトルの文字
    private GameObject titleObject;

    // ルール、操作説明画像のオブジェクト
    private Image ruleImage;

    // ゲームオーバーの文字
    private GameObject gameoverObject;

    // UI
    // ボールカウント全体のオブジェクト
    private GameObject UIBallCountBoard;

    // 1塁のUI
    private Image UIFirstBase;

    // 2塁のUI
    private Image UISecondBase;

    // 3塁のUI
    private Image UIThirdBase;

    // ストライクカウント1つ目のUI
    private Image UIStrikeCount1;

    // ストライクカウント2つ目のUI
    private Image UIStrikeCount2;

    // アウトカウント1つ目のUI
    private Image UIOutCount1;

    // アウトカウント2つ目のUI
    private Image UIOutCount2;

    //得点表示
    private Text UIScoreText;

    // 判定テキスト
    private Text UIHitText;


    // UIに使用する画像
    // 塁の画像(走者なし)
    private Sprite baseImage;

    // 塁の画像(走者あり)
    private Sprite runnerOnBaseImgae;

    // ストライクカウントランプ(消灯)
    private Sprite strikeCountOffImage;

    // ストライクカウントランプ(点灯)
    private Sprite strikeCountOnImage;

    // アウトカウントランプ(消灯)
    private Sprite outCountOffImage;

    // アウトカウントランプ(点灯)
    private Sprite outCountOnImage;


    // Ballのスクリプト
    private BallController ballController;

    // Batのスクリプト
    private BatController batController;


    // ゲーム本編が始まっているかどうか
    public bool isStartingGame = false;

    // ゲームオーバーになったかどうか
    private bool isGameOver = false;

    // タイトル画面の段階
    private int titlePhase = 0;

    // 進める塁の数
    private int AdvanceCounter = 0;

    // 走者を管理する
    private List<int> runnerList = new List<int>();

    // ストライクカウント
    private int strikeCount = 0;

    // アウトカウント
    private int outCount = 0;

    // 得点
    private int score = 0;

    // 表示する文字の種類
    private int hitTextType = 0;

	// Use this for initialization
	void Start () {
        // スクリプトを取得
        this.ballController = GameObject.Find("Ball").GetComponent<BallController>();
        this.batController = GameObject.Find("Bat").GetComponent<BatController>();

        // タイトルのオブジェクトを取得
        this.titleObject = GameObject.Find("TitleText");
        this.ruleImage = GameObject.Find("Rule").GetComponent<Image>();

        // ゲームオーバーのオブジェクトを取得
        this.gameoverObject = GameObject.Find("GameOverText");

        // UIのオブジェクトを取得
        this.UIBallCountBoard = GameObject.Find("UIBackGround");
        this.UIFirstBase = GameObject.Find("FirstBase").GetComponent<Image>();
        this.UISecondBase = GameObject.Find("SecondBase").GetComponent<Image>();
        this.UIThirdBase = GameObject.Find("ThirdBase").GetComponent<Image>();
        this.UIStrikeCount1 = GameObject.Find("StrikeCount_1").GetComponent<Image>();
        this.UIStrikeCount2 = GameObject.Find("StrikeCount_2").GetComponent<Image>();
        this.UIOutCount1 = GameObject.Find("OutCount_1").GetComponent<Image>();
        this.UIOutCount2 = GameObject.Find("OutCount_2").GetComponent<Image>();
        this.UIScoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        this.UIHitText = GameObject.Find("HitText").GetComponent<Text>();

        // 画像の読み込み
        this.baseImage = Resources.Load<Sprite>("base");
        this.runnerOnBaseImgae = Resources.Load<Sprite>("runnerOnBase");
        this.strikeCountOffImage = Resources.Load<Sprite>("strikeLampOff");
        this.strikeCountOnImage = Resources.Load<Sprite>("strikeLampOn");
        this.outCountOffImage = Resources.Load<Sprite>("outLampOff");
        this.outCountOnImage = Resources.Load<Sprite>("outLampOn");


        // ルール画像を非表示
        this.ruleImage.enabled = false;
        // ゲームオーバーの文字を非表示
        this.gameoverObject.SetActive(false);
        // ボールカウントUIを非表示
        this.UIBallCountBoard.SetActive(false);
	}
	
	// Update is called once per frame
    void Update() {
        // ゲームオーバー中
        if (this.isGameOver) {
            // Zキーが押されたらゲームを再スタートする
            if (Input.GetKeyDown(KeyCode.Z)) {
                // 全部リセットする
                ResetGame();
                // ゲームオーバーの文字を消す
                this.gameoverObject.SetActive(false);
            }
        }

        // タイトル中
        else if (!this.isStartingGame) {
            switch (this.titlePhase) {
                // タイトル画面
                case 0:
                    // Zキーが押されたらルール説明に移行する
                    if (Input.GetKeyDown(KeyCode.Z)) {
                        this.titlePhase = 1;

                        // ルール画像を表示
                        this.ruleImage.enabled = true;
                        // タイトルの文字を非表示
                        this.titleObject.SetActive(false);
                    }
                    break;

                // ルール、操作説明
                case 1:
                    // Zキーが押されたらゲームを開始する
                    if (Input.GetKeyDown(KeyCode.Z)) {
                        // ルール画像を非表示
                        this.ruleImage.enabled = false;

                        // ゲーム本編を開始する
                        this.isStartingGame = true;

                        // ボールカウントUIを表示する
                        this.UIBallCountBoard.SetActive(true);

                        // ボールを投げる合図を出す
                        this.ballController.isThrowing = true;
                    }
                    break;
            }
        }

        // ゲーム本編中
        else {
            // ボールが何かの判定に衝突した場合
            CheckBallCollider();

            // ヒットゾーンにボールが入った場合
            if (this.AdvanceCounter > 0) {

                // 打者を出塁させる
                this.runnerList.Add(0);

                // ヒットの種類ごとに塁を進める
                while (this.AdvanceCounter > 0) {
                    this.AdvanceRunner();
                    this.AdvanceCounter--;
                }

                // 塁UIを更新
                UpdateBaseUIImage();
            }

            // 3アウトでゲームオーバー
            if (this.outCount >= 3) {
                // ゲームオーバー
                this.isGameOver = true;
                this.isStartingGame = false;
                // ゲームオーバーの文字を表示する
                this.gameoverObject.SetActive(true);
            }
        }
	}

    // ボールが衝突した相手を調べ処理する
    void CheckBallCollider() {
        if (this.ballController.colliderTag != "") {
            switch (this.ballController.colliderTag) {
                // シングルヒットの場合
                case "SingleHitTag":
                    this.AdvanceCounter = 1;

                    // 表示する文字をセット
                    this.hitTextType = 0;
                    break;
                // ダブルヒットの場合
                case "DoubleHitTag":
                    this.AdvanceCounter = 2;

                    // 表示する文字をセット
                    this.hitTextType = 1;
                    break;
                // トリプルヒットの場合
                case "TripleHitTag":
                    this.AdvanceCounter = 3;

                    // 表示する文字をセット
                    this.hitTextType = 2;
                    break;
                // ホームランの場合
                case "HomerunTag":
                    this.AdvanceCounter = 4;

                    // 表示する文字をセット
                    this.hitTextType = 3;
                    break;

                // ストライクゾーンの場合
                case "StrikeZoneTag":
                    // ストライクカウントを追加
                    this.strikeCount++;

                    // 3ストライクで1アウト
                    if (this.strikeCount >= 3) {
                        this.outCount++;
                        this.strikeCount = 0;
                    }

                    // 表示する文字をセット
                    this.hitTextType = 4;
                    break;
                // ファールエリアの場合
                case "FoulTerritoryTag":
                    // 2ストライク未満でファールになった場合
                    if (this.strikeCount < 2) {
                        // ストライクカウントを追加
                        this.strikeCount++;
                    }

                    // 表示する文字をセット
                    this.hitTextType = 5;
                    break;
                // アウトエリアの場合
                case "OutTerritoryTag":
                    this.outCount++;
                    this.strikeCount = 0;

                    // 表示する文字をセット
                    this.hitTextType = 6;
                    break;
                default:
                    break;
            }
            // タグリセット
            this.ballController.colliderTag = "";

            // ボールカウントUIを更新
            UpdateBallCountImage();

            // 判定表示
            StartCoroutine("DrawHitText", this.hitTextType);
        }
    }

    // 走者をところてん方式で1塁進める
    void AdvanceRunner() {
        // 動かす走者を探す
        bool[] isRun = new bool[this.runnerList.Count];
        for (int i = this.runnerList.Count - 1; i >= 0; i--) {
            // 打者は動かす
            if (i == this.runnerList.Count - 1) {
                isRun[i] = true;
            }

            // 打者以外は自分以前の塁に空白がなければ動く
            else {
                bool checkFlag = true;
                for (int j = i; j < this.runnerList.Count - 1; j++) {
                    if (this.runnerList[j] != this.runnerList[j + 1] + 1) {
                        checkFlag = false;
                    }
                }
                isRun[i] = checkFlag;
            }
        }

        // 動ける走者だけ1塁進める
        for (int i = 0; i < this.runnerList.Count; i++) {
            if (isRun[i]) {
                this.runnerList[i]++;
            }
        }

        // 走者がホームベースに帰ってきたら得点
        if (this.runnerList[0] >= 4) {
            this.score++;
            this.runnerList.RemoveAt(0);

            // 得点表示を更新
            this.UIScoreText.text = this.score.ToString();
        }
    }

    // 塁画像を更新
    void UpdateBaseUIImage() {
        // 塁に走者がいた場合は赤、いない場合は灰色にする
        if (this.runnerList.Contains(1)) {
            this.UIFirstBase.sprite = this.runnerOnBaseImgae;
        }
        else {
            this.UIFirstBase.sprite = this.baseImage;
        }
        if (this.runnerList.Contains(2)) {
            this.UISecondBase.sprite = this.runnerOnBaseImgae;
        }
        else {
            this.UISecondBase.sprite = this.baseImage;
        }
        if (this.runnerList.Contains(3)) {
            this.UIThirdBase.sprite = this.runnerOnBaseImgae;
        }
        else {
            this.UIThirdBase.sprite = this.baseImage;
        }
    }

    // ボールカウント画像の更新
    void UpdateBallCountImage() {
        // ストライクの数だけ点灯させる
        if (this.strikeCount == 0) {
            this.UIStrikeCount1.sprite = this.strikeCountOffImage;
            this.UIStrikeCount2.sprite = this.strikeCountOffImage;
        }
        else if (this.strikeCount == 1) {
            this.UIStrikeCount1.sprite = this.strikeCountOnImage;
            this.UIStrikeCount2.sprite = this.strikeCountOffImage;
        }
        else {
            this.UIStrikeCount1.sprite = this.strikeCountOnImage;
            this.UIStrikeCount2.sprite = this.strikeCountOnImage;
        }

        // アウトの数だけ点灯させる
        if (this.outCount == 0) {
            this.UIOutCount1.sprite = this.outCountOffImage;
            this.UIOutCount2.sprite = this.outCountOffImage;
        }
        else if (this.outCount == 1) {
            this.UIOutCount1.sprite = this.outCountOnImage;
            this.UIOutCount2.sprite = this.outCountOffImage;
        }
        else {
            this.UIOutCount1.sprite = this.outCountOnImage;
            this.UIOutCount2.sprite = this.outCountOnImage;
        }
    }

    // 判定表示
    IEnumerator DrawHitText(int type) {
        switch (type) {
            case 0:
                this.UIHitText.text = "ヒット";
                break;
            case 1:
                this.UIHitText.text = "ツーベース\nヒット";
                break;
            case 2:
                this.UIHitText.text = "スリーベース\nヒット";
                break;
            case 3:
                this.UIHitText.text = "ホームラン";
                break;
            case 4:
                this.UIHitText.text = "ストライク";
                break;
            case 5:
                this.UIHitText.text = "ファール";
                break;
            case 6:
                this.UIHitText.text = "アウト";
                break;
        }

        this.UIHitText.fontSize = 50;
        for (int i = 0; i < 25; i++) {
            this.UIHitText.fontSize += 2;
            yield return null;
        }

        // 1秒後に消す
        yield return new WaitForSeconds(1.0f);
        this.UIHitText.text = "";

        // ボールを元に戻し投げる合図を出す
        this.ballController.ResetBall();
        this.ballController.isThrowing = true;

        // バットとストライクゾーンの判定も元に戻す
        this.batController.ResetBat();
    }

    // ゲームをリセットする
    void ResetGame() {
        this.strikeCount = 0;
        this.outCount = 0;
        this.score = 0;
        this.runnerList.Clear();
        this.isGameOver = false;
        this.isStartingGame = true;
        UpdateBallCountImage();
        UpdateBaseUIImage();
    }
}
