using System;
using UnityEngine;

namespace Share
{
    public sealed class Point : MonoBehaviour
    {
        //pointプレファブのON,OFFを切り替える
        [SerializeField]
        public IdentifiedPoint IdentifiedPoint;

        private Material _material = null;

        public void ChangeFilter(Func<IdentifiedPoint, bool> filterScheme)
        {
            var active = filterScheme(IdentifiedPoint);
            if (this.gameObject.activeSelf != active)
            {
                this.gameObject.SetActive(active);
            }
        }

        public void ChangeColor(Func<IdentifiedPoint, Color> colorScheme)
        {
            if (_material == null)
            {
                _material = GetComponent<Renderer>().material;
            }
            _material.color = colorScheme(IdentifiedPoint);
        }

        private void OnDestroy()
        {
            if (_material != null)
            {
                Destroy(_material);
            }
        }
    }
}
