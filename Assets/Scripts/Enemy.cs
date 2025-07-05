using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    /// <summary>  
    /// プレイヤー  
    /// </summary>  
    [SerializeField] private Player player_ = null;
	/// 視野角
	public float viewAngle = 20f;
    /// 追尾速度
	public float moveSpeed = 3f;	  
	/// 視野距離
	public float detectionDistance = 10f;
	/// 毎フレーム回転する角度（度）
	public float rotationPerFrame = 10f; // 毎フレーム回転する角度（度） 
	/// <summary>  
	/// ワールド行列   
	/// </summary>  
	private Matrix4x4 worldMatrix_ = Matrix4x4.identity;

	/// <summary>  
	/// ターゲットとして設定する  
	/// </summary>  
	/// <param name="enable">true:設定する / false:解除する</param>  
	public void SetTarget(bool enable)
	{
		// マテリアルの色を変更する  
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.materials[0].color = enable ? Color.red : Color.white;
	}

	//追尾　可否
    private bool isChasing = false;
	/// <summary>
	/// 開始処理
	/// </summary>
	public void Start()
    {
		
    }

    /// <summary>  
    /// 更新処理  
    /// </summary>  
    public void Update()
    {
		// プレイヤーの位置とエネミーの位置の差を計算
		Vector3 directionToPlayer = player_.transform.position - transform.position;
		float distance = directionToPlayer.magnitude;

		// 視野範囲内かつ一定距離内にいるかをチェック
		if (distance <= detectionDistance)
		{
			// 視野角チェック（内積）
			float dot = Vector3.Dot(transform.forward, directionToPlayer.normalized);
			if (dot >= Mathf.Cos(viewAngle * Mathf.Deg2Rad))
			{
				// 視野内にプレイヤーが入ったら追尾を開始
				if (!isChasing)
				{
					isChasing = true;
					SetTarget(true); // プレイヤーをターゲットにする
				}
			}
			else
			{
				// 視野外に出た場合、追尾を停止
				if (isChasing)
				{
					isChasing = false;
					SetTarget(false); // プレイヤーをターゲットから解除
				}
			}
		}

		// 追尾処理
		if (isChasing)
		{
			// プレイヤー方向に回転
			Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

			// 毎フレーム10度回転する
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationPerFrame);

			// プレイヤーの方向へ移動
			transform.position += transform.forward * moveSpeed * Time.deltaTime;
		}
	}
}
