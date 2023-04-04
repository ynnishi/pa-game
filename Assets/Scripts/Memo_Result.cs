using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Communication
{
    // Imageコンポーネントを必要とする
    [RequireComponent(typeof(Image))]

    public class Memo_Result : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
    {
        // ドラッグ前の位置
        private Vector3 prevPos;

        //基準点（マウスの基準は左下だが、オブジェクトの基準は画面中央になるので補正する。）
        private Vector2 rootPos;
        
        // Use this for initialization
        void Start () {
            rootPos = new Vector3(400f, 640f, 0f); //画面の半分（400, 300）
        }
        
        // Update is called once per frame
        void Update () {
            
        }


        //ドラッグ＆ドロップ関係

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(!PlayManager.CheckMarkerSet(this.name)){
                // ドラッグ前の位置を記憶しておく
                prevPos = transform.localPosition;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(!PlayManager.CheckMarkerSet(this.name)){
                //Debug.Log("eventData.position.y：" + eventData.position.y);

                // ドラッグ中は位置を更新する
                transform.localPosition = eventData.position - rootPos;
                //Debug.Log("eventData.position: " + eventData.position);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if(!PlayManager.CheckMarkerSet(this.name)){
                // ドラッグ前の位置に戻す
                //transform.position = prevPos;
                transform.localPosition = eventData.position - rootPos;

                if(PlayManager.CheckMarker(transform.localPosition.x, transform.localPosition.y, this.name)){
                    this.GetComponent<Image>().color = Color.red;
                }else{
                    transform.localPosition = prevPos;
                }
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
        //使ってません。
            /*var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);
            
            foreach (var hit in raycastResults)
            {
                // もし DroppableField の上なら、その位置に固定する
                if (hit.gameObject.CompareTag("DroppableField"))
                {
                    transform.position = hit.gameObject.transform.position;
                    this.enabled = false;
                }
            }*/
        }
    }
}