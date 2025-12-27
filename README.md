# Rockman-Style Action Game Logic Prototype

Unityを使用した2Dアクションゲームの挙動・ロジックを重視したプロトタイププロジェクトです。
「マップデザイン」よりも、**「キャラクターの操作性や物理的な反応をロジックで制御すること」**に重点を置いて制作しました。

## 📂 ディレクトリ構成
- `Assets/Scripts/Animation/`:各アセットの2Dイラストとそのアニメーション管理
- `Assets/Scripts/robo/`: プレイヤーの移動・攻撃ロジック
- `Assets/Scripts/enemy/`: 敵のAI、ダメージ・ノックバック処理
- `Assets/Scripts/camera/`: カメラの追従ロジック

## 🚀 プロジェクトの重点ポイント


### 1. プレイヤーの制御（`Assets/Scripts/robo/`）
機能ごとにクラスを分離しています。
- **`Attack.cs`**:
プレイヤー（ロボ）の攻撃処理を行うスクリプトです。
キーCの入力を監視し、攻撃可能なときにビーム弾プレハブ Beam を attackPoint の位置に生成して発射します。
キャラクターの向き（transform.localScale.x）からビームの進行方向を計算し、BeamShot コンポーネントの direction に設定します。
アニメーションコントローラのパラメータ IsShot を切り替えて攻撃アニメーションを再生します。
ShotCoolDown で指定したクールタイム中は canAttack を false にして連射できないように制御します。
- **`BeamShot.cs`**:
ロボのビーム弾そのものの挙動を制御するスクリプトです。
direction と speed をもとに、Rigidbody2D の速度を設定してビームを直線移動させます。
発射方向に合わせて transform.localScale.x を反転させ、見た目の向きを揃えます。
発射位置から一定距離（約 20 ユニット）以上離れた場合、自動的にビームを破棄します。
何かに衝突すると hasHit を true にし、衝突位置 endPoint に移動してヒット用アニメーション（IsHit）を再生します。
コライダー col を無効化し停止させた後、約 0.583 秒後にビームオブジェクトを破棄します。
衝突相手のタグが "Enemy" の場合、その相手に ApplyDamage メッセージを送り、direction をダメージ処理に渡します。
- **`Movement.cs`**:
プレイヤー（ロボ）の基本的な移動入力を受け付けるスクリプトです。
Input.GetAxisRaw("Horizontal") から左右入力を読み取り、その値を PlayerController の Move メソッドに渡して移動処理を行います。
アニメーターのパラメータ Speed に左右入力の絶対値を設定し、移動速度に応じたアニメーションを再生します。
キー Z でジャンプフラグ jump を、キー X でダッシュフラグ dash を立て、FixedUpdate 内で controller.Move(horizontal, jump, dash) に反映します。
FixedUpdate 実行後に jump と dash を false に戻し、1 回の入力につき 1 度だけ処理されるように制御します。
- **`PlayerController.cs`**:
プレイヤー（ロボ）の物理挙動と移動・ジャンプ・ダッシュをまとめて制御する中核スクリプトです。
groundLayer と GroundCheck を使った円形判定で接地状態を検出し、WallCheck で空中時の壁接触も判定します。
Move(float move, bool jump, bool dash) を Movement.cs から呼び出し、move 入力に応じて runSpeed を掛けた目標速度へ Vector3.SmoothDamp でスムーズに加速・減速させます（地上／空中で減衰パラメータを切り替え）。
落下速度を MaxFallSpeed で制限し、空中時の挙動を安定させます。
キー入力に応じた jump フラグで地上ジャンプを行い、さらに空中で 1 回だけ二段ジャンプ（canDoubleJump）ができるように制御します。
dash フラグと Dash() コルーチンにより、一定時間だけ進行方向へ高速移動するダッシュ処理を行い、クールタイム中は再使用できないように制限します。
キャラクターの向きは Flip() で transform.localScale.x を反転させて切り替え、移動方向に合わせて自然に向きが変わるようにします。
アニメーターの各種パラメータ（IsJump, IsDoubleJump, IsFall など）を速度・接地状態に応じて切り替え、ジャンプ／落下／着地のアニメーションを管理します。


### 2. 敵AIと物理レスポンス（`Assets/Scripts/Enemy/`）
- **`BeamShoe_enemy.cs`**:
途中制作
- **`droneEnemy.cs`**:
敵ドローンの移動・被ダメージ・死亡演出を制御するスクリプトです。
初期位置 start から左右に MoveArea の範囲内を往復移動し、端まで到達すると Flip() で向きを反転して折り返します。
transform.localScale.x と speed を使って水平方向の速度を設定し、Rigidbody2D を通して移動させます。
ApplyDamage(Vector2 direction) でダメージを受けると life を減らし、0 以下になると死亡フラグ isDead を立てて死亡処理（アニメーションと削除）に移行します。
まだ生きている場合は、一時的に移動を止めてノックバック方向 direction と倍率 n に応じた速度を与え、HittedCoolDown() コルーチンで短時間後に元の速度へ戻します。
プレイヤーと衝突している間は一度速度を 0 にして停止し、衝突が解消されたタイミングで保存しておいた速度 save を復元します。
死亡時はコライダー col を無効化し、アニメーターのパラメータ Died を true にして死亡アニメーションを再生した後、0.5 秒後にオブジェクトを削除します。
- **`turrentEnemy.cs`**:
固定砲台タイプの敵キャラクター（タレット）の挙動を制御するスクリプトです。
cooldown 秒ごとに miniBeam プレハブを attackPoint の位置に生成し、タレット本体の向きに合わせてビームの transform.localScale を設定します。
内部カウンタ time に Time.deltaTime を加算し、クールタイムに達したタイミングでビーム発射とタイマーのリセットを行います。
ApplyDamage(Vector2 direction) でダメージを受けると life を減らし、0 以下になると isDead を true にして次フレーム以降でオブジェクトを削除します。
アニメーター animator を取得していますが、現時点ではスクリプト内からパラメータを変更しておらず、将来的なアニメーション連動用の準備となっています。

### 3. カメラシステム（`Assets/Scripts/camera/`）
- **`cameraController.cs`**:
プレイヤー追従カメラと、手前の背景レイヤーのパララックス（奥行き感のあるスクロール）を制御するスクリプトです。
カメラの現在位置と前フレーム位置 lastPos の差分から CameraSpeed を計算し、その移動量に応じて nearBackground を少しだけ遅れて動かします（x 方向 0.5 倍、y 方向 0.3 倍）。
StopFollow が true の間はカメラ移動による背景スクロールを停止し、特定シーンやイベント中にパララックスを止めることができます。
minHeight / maxHeight は将来的にカメラの縦方向の移動制限を入れるためのパラメータとして用意されていますが、現状のコードではまだ使用されていません。

### 4. アニメーション
市販のアセットのドット絵を利用し, 各アニメーションを作成しました。

## 🛠 技術スタック
- **Language**: C#
- **Engine**: Unity 20xx.x (ご自身のバージョンに合わせて書き換えてください)
- **Tool**: VSCode / WSL (Ubuntu) / Git



