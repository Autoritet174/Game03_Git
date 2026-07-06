using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.GameData.Prefabs
{
    public interface IPanelCollectionViewerContext
    {
        void OnElementSelected(Guid elementId, ECollectionMode collectionMode);
        //// опционально — остальное, что уже есть в ваших заготовках:
        //void OnCollectionLoaded(PanelCollection__prefab__scriptMB panelCollection);
        //Guid? GetSelectedElementId(ECollectionMode collectionMode);
        //void OnLayoutChanged();
        //(float width, float height) GetViewerSize();
    }
}
