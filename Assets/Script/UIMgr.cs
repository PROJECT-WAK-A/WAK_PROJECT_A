using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UI
{
    public interface IUIUpdateable
    {
        void UpdateBulletCount(int bulletCount);
        void UpdateFireMode(string fireMode);
    }


    public class UIMgr : MonoBehaviour, IUIUpdateable
    {
        public TextMeshProUGUI bulletCountText;            // 총알 개수를 표시할 텍스트 UI
        public TextMeshProUGUI fireModeText;               // 발사 모드를 표시할 텍스트 UI

        /// <summary>
        /// 총알 개수를 표시할 UI
        /// </summary>
        /// <param name="bulletCount"></param>
        public void UpdateBulletCount(int bulletCount)
        {
            bulletCountText.text = bulletCount.ToString();
        }

        /// <summary>
        /// 발사 모드를 표시할 UI
        /// </summary>
        /// <param name="fireMode"></param>
        public void UpdateFireMode(string fireMode)
        {
            fireModeText.text = fireMode;
        }
    }
}