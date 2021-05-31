﻿using System.Collections.Generic;
#if USE_SKIASHARP
using SkiaSharp;
#else
using ShimSkiaSharp.Primitives;
#endif

namespace Svg.Model.Drawables
{
    public abstract class DrawableContainer : DrawableBase
    {
        public List<DrawableBase> ChildrenDrawables { get; }

        protected DrawableContainer(IAssetLoader assetLoader)
            : base(assetLoader)
        {
            ChildrenDrawables = new List<DrawableBase>();
        }

        protected void CreateChildren(SvgElement svgElement, SKRect skOwnerBounds, DrawableBase? parent, IAssetLoader assetLoader, DrawAttributes ignoreAttributes)
        {
            foreach (var child in svgElement.Children)
            {
                var drawable = DrawableFactory.Create(child, skOwnerBounds, parent, assetLoader, ignoreAttributes);
                if (drawable is { })
                {
                    ChildrenDrawables.Add(drawable);
                }
            }
        }

        protected void CreateGeometryBounds()
        {
            foreach (var drawable in ChildrenDrawables)
            {
                if (GeometryBounds.IsEmpty)
                {
                    GeometryBounds = drawable.GeometryBounds;
                }
                else
                {
                    if (!drawable.GeometryBounds.IsEmpty)
                    {
                        GeometryBounds = SKRect.Union(GeometryBounds, drawable.GeometryBounds);
                    }
                }
            }
        }

        public override void OnDraw(SKCanvas canvas, DrawAttributes ignoreAttributes, DrawableBase? until)
        {
            if (until is { } && this == until)
            {
                return;
            }

            foreach (var drawable in ChildrenDrawables)
            {
                if (until is { } && drawable == until)
                {
                    break;
                }
                drawable.Draw(canvas, ignoreAttributes, until, true);
            }
        }

        public override void PostProcess(SKRect? viewport, SKMatrix totalMatrix)
        {
            base.PostProcess(viewport, totalMatrix);

            foreach (var child in ChildrenDrawables)
            {
                child.PostProcess(viewport, TotalTransform);
            }
        }
    }
}
