using UnityEngine;
using UnityEngine.UIElements;

namespace NOS.UIElements
{
    [UxmlElement]
    public partial class UIElementRadialFill : VisualElement
    {
        public UIElementRadialFill()
        {
            generateVisualContent += OnGenerateVisualContent;
            pickingMode = PickingMode.Ignore;
        }

        [UxmlAttribute("Fill")]
        public float FillAmount
        {
            get => _fillAmount;
            set
            {
                _fillAmount = Mathf.Clamp01(value);
                MarkDirtyRepaint();
            }
        }

        private float _fillAmount = 1f;

        [UxmlAttribute("Texture")]
        public Texture2D Texture
        {
            get => _texture;
            set
            {
                _texture = value;
                MarkDirtyRepaint();
            }
        }

        private Texture2D _texture;
        
        [UxmlAttribute("Tint")]
        public Color Tint
        {
            get => _tint;
            set
            {
                _tint = value;
                MarkDirtyRepaint();
            }
        }

        private Color _tint = new Vector4(1, 1, 1, 1);

        [UxmlAttribute("Start-Angle")]
        public float StartAngle
        {
            get => _startAngle;
            set
            {
                _startAngle = value;
                MarkDirtyRepaint();
            }
        }

        private float _startAngle = -90f;

        [UxmlAttribute("CounterClockwise")]
        public bool CounterClockwise
        {
            get => _counterClockwise;
            set
            {
                _counterClockwise = value;
                MarkDirtyRepaint();
            }
        }

        private bool _counterClockwise = true;


        private const int Segments = 128;
        private const float Tau = Mathf.PI * 2f;

        // ReSharper disable once CognitiveComplexity
        private void OnGenerateVisualContent(MeshGenerationContext ctx)
        {
            if (_texture == null || _fillAmount <= 0f)
                return;

            
            
            Rect rect = contentRect;

            float texAspect = (float)_texture.width / _texture.height;
            float rectAspect = rect.width / rect.height;

            float drawWidth, drawHeight;
            if (rectAspect > texAspect)
            {
                drawHeight = rect.height;
                drawWidth = drawHeight * texAspect;
            }
            else
            {
                drawWidth = rect.width;
                drawHeight = drawWidth / texAspect;
            }

            Vector2 drawCenter = rect.center;
            float radius = Mathf.Min(drawWidth, drawHeight) * 0.5f;

            int triangleCount = Mathf.CeilToInt(Segments * _fillAmount);
            int vertCount = triangleCount + 2;
            int indexCount = triangleCount * 3;

            MeshWriteData mwd = ctx.Allocate(vertCount, indexCount, _texture);
            if (mwd.vertexCount == 0)
                return;

#pragma warning disable CS0618 // Type or member is obsolete
            Rect uvRegion = mwd.uvRegion;
#pragma warning restore CS0618 // Type or member is obsolete

            float startRad = _startAngle * Mathf.Deg2Rad;
            float fillArc = Tau * _fillAmount;

            float step = fillArc / triangleCount;
            if (_counterClockwise)
            {
                step = -step;
            }

            mwd.SetNextVertex(new Vertex
            {
                position = drawCenter,
                uv = new Vector2(
                    uvRegion.xMin + uvRegion.width * 0.5f,
                    uvRegion.yMin + uvRegion.height * 0.5f
                ),
                tint = _tint
            });

            for (int i = 0; i <= triangleCount; i++)
            {
                float angle = startRad + step * i;

                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);

                Vector2 pos = drawCenter + new Vector2(cos, sin) * radius;

                float u = cos * 0.5f + 0.5f;
                float v = sin * 0.5f + 0.5f;

                Vector2 finalUV = new(
                    Mathf.Lerp(uvRegion.xMin, uvRegion.xMax, u),
                    Mathf.Lerp(uvRegion.yMin, uvRegion.yMax, v)
                );

                mwd.SetNextVertex(new Vertex
                {
                    position = pos,
                    uv = finalUV,
                    tint = _tint
                });
            }

            // ===== INDICES =====
            for (int i = 1; i <= triangleCount; i++)
            {
                if (_counterClockwise)
                {
                    mwd.SetNextIndex(0);
                    mwd.SetNextIndex((ushort)(i + 1));
                    mwd.SetNextIndex((ushort)i);
                }
                else
                {
                    mwd.SetNextIndex(0);
                    mwd.SetNextIndex((ushort)i);
                    mwd.SetNextIndex((ushort)(i + 1));
                }
            }
        }
    }
}