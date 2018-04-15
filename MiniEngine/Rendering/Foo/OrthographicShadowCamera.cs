using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MiniEngine.Rendering.Foo
{
    public class OrthographicShadowCamera : ShadowCamera
    {
        private float _minX;
        private float _minY;
        private float _maxX;
        private float _maxY;

        public float MinX
        {
            get { return _minX; }
            set
            {
                _minX = value;
                CreateProjection();
            }
        }

        public float MinY
        {
            get { return _minY; }
            set
            {
                _minY = value;
                CreateProjection();
            }
        }

        public float MaxX
        {
            get { return _maxX; }
            set
            {
                _maxX = value;
                CreateProjection();
            }
        }

        public float MaxY
        {
            get { return _maxY; }
            set
            {
                _maxY = value;
                CreateProjection();
            }
        }

        public OrthographicShadowCamera(
            float minX,
            float minY,
            float maxX,
            float maxY,
            float nearZ,
            float farZ)
            : base(nearZ, farZ)
        {
            _minX = minX;
            _minY = minY;
            _maxX = maxX;
            _maxY = maxY;

            CreateProjection();
        }

        protected override void CreateProjection()
        {
            Projection = Matrix.CreateOrthographicOffCenter(_minX, _maxX, _minY, _maxY, NearZ, FarZ);
        }
    }
}

