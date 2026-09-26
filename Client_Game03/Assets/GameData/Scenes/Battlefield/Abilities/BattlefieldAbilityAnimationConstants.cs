namespace Assets.GameData.Scenes.Battlefield
{
    /// <summary>Общие настройки анимаций способностей; длительности заданы для скорости ×1.</summary>
    public static class BattlefieldAbilityAnimationConstants
    {
        /// <summary>Множитель масштаба поднятой карточки.</summary>
        public const float RAISED_CARD_SCALE = 1.3f;

        /// <summary>Множитель масштаба карточки после завершения способности.</summary>
        public const float NORMAL_CARD_SCALE = 1f;

        /// <summary>Длительность увеличения карточки для всех способностей.</summary>
        public const float CARD_RAISE_DURATION = 0.3f;

        /// <summary>Длительность уменьшения карточки и возврата после атаки.</summary>
        public const float CARD_LOWER_DURATION = 0.4f;

        /// <summary>Номинальная длительность рывка атаки до поправки на расстояние до цели.</summary>
        public const float ATTACK_LUNGE_DURATION = 0.5f;

        /// <summary>Пауза после возврата атакующего перед следующим действием.</summary>
        public const float POST_ATTACK_DELAY = 0.5f;

        /// <summary>Пауза с поднятой карточкой после появления числа исцеления.</summary>
        public const float HEALING_HOLD_DURATION = 1f;
    }
}
