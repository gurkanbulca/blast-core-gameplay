using System;
using System.Collections.Generic;
using BlockSystem;
using UnityEngine;

namespace GameCore
{
    public class InputController
    {
        public event Action<List<Block>> OnRayCastHit = delegate { };

        private readonly RaycastHit2D[] _hits = new RaycastHit2D[1];
        private readonly Camera _camera;
        private readonly LayerMask _blockLayer;

        public InputController(LayerMask blockLayer)
        {
            _blockLayer = blockLayer;
            _camera = Camera.main;
        }


        public void Tick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RayCastForBlock();
            }
        }

        private void RayCastForBlock()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            var hitCount = Physics2D.RaycastNonAlloc(ray.origin, ray.direction, _hits, Mathf.Infinity);
            if (hitCount <= 0)
                return;
            var block = _hits[0].collider.GetComponent<Block>();
            if (!block)
                return;
            OnRayCastHit(block.blockGroup);
        }
    }
}