namespace _Project.Scripts._Common.UIComponent
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.UI;
    
    public class CutoffMaskUI: Image
    {
        private bool _flag;
        private Material mat;
        public override Material materialForRendering
        {
            get
            {
                if (!_flag)
                {
                    _flag = true;
                    mat = new Material(base.materialForRendering);
                    mat.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
                }
                return mat;

            }
        }
    }
}