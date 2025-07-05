using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Security.Principal;
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
   // private bool isChasing = false;
	/// <summary>
	/// 開始処理
	/// </summary>
	public void Start()
    {
		worldMatrix_ = Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one);
    }

    /// <summary>  
    /// 更新処理  
    /// </summary>  
    public void Update()
	{
		if (!player_) { return; }

		var normalz = new Vector3(0, 0, 1);
		var forward = worldMatrix_ * normalz;

		var toPlayerNormal = (player_.transform.position - transform.position).normalized;

		var dot = Vector3.Dot(toPlayerNormal, forward);

		var inViewCos = Mathf.Cos(20.0f * Mathf.Deg2Rad);

		if (inViewCos <= dot)
		{
			var cross = Vector3.Cross(forward, toPlayerNormal);

			var radian = Mathf.Min(Mathf.Acos(dot), (10.0f * Mathf.Deg2Rad));

			radian *= (cross.y / Mathf.Abs(cross.y));

			var rotMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, Mathf.Rad2Deg * radian, 0));
			worldMatrix_ = worldMatrix_ * rotMatrix;

			var f = new Vector3(0, 0, 0.2f);
			var move = worldMatrix_ * f;

			var pos = worldMatrix_.GetColumn(3) + move;
			worldMatrix_.SetColumn(3, pos);
		}
		transform.position = worldMatrix_.GetColumn(3);
		transform.rotation = worldMatrix_.rotation;
		transform.localScale = worldMatrix_.lossyScale;
		/*ここから自分の
		// プレイヤーの位置とエネミーの位置の差を計算
		Vector3 directionToPlayer = player_.transform.position - transform.position;
		float distance = directionToPlayer.magnitude;

		// 視野範囲内かつ一定距離内にいるかをチェック
		if (distance <= detectionDistance)
		{
			// 視野角チェック
			float dot = Vector3.Dot(transform.forward, directionToPlayer.normalized);
			if (dot >= Mathf.Cos(viewAngle * Mathf.Deg2Rad))
			{
				// 視野内にプレイヤーが入ったら追尾を開始
				if (!isChasing)
				{
					isChasing = true;
					SetTarget(true); //ターゲットにする
				}
			}
			else
			{
				// 視野外に出た場合、追尾を停止
				if (isChasing)
				{
					isChasing = false;
					SetTarget(false); // ターゲットから解除
				}
			}
		}

		// 追尾処理
		if (isChasing)
		{
			// プレイヤー方向に回転
			Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationPerFrame);
			transform.position += transform.forward * moveSpeed * Time.deltaTime;
		}*/
	}
}
