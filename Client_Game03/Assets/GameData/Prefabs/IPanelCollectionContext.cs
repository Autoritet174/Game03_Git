using System;

namespace Assets.GameData.Prefabs
{
    /// <summary>Обрабатывает выбор элемента коллекции в контексте текущей сцены.</summary>
    public interface IPanelCollectionContext
    {
        void OnClick(Guid elementId, ECollectionMode collectionMode);
    }
}
