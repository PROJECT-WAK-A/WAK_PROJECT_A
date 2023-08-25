using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UI
{
    public interface IUIUpdateable
    {
        void UpdateBulletCount(int bulletCount);
    }


    public class UIMgr : MonoBehaviour, IUIUpdateable
    {
        public TextMeshProUGUI bulletCountText;            // 총알 개수를 표시할 텍스트 UI

        public void UpdateBulletCount(int bulletCount)
        {
            bulletCountText.text = bulletCount.ToString();
        }
    }
}