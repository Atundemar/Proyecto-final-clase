using UnityEngine;

public static class ExtensionMethods
{
    /// <summary>
    /// M�todo de extensi�n para los canvas group, que activar� o desactivar� el group
    /// </summary>
    /// <param name="group"></param>
    /// <param name="enable"></param>
    public static void Toggle(this CanvasGroup group, bool enable)
    {
        // Lo hacemos visible o no en base a lo que diga enable
        group.alpha = enable ? 1 : 0;
        // Lo hacemos o no interactable en base a enable
        group.interactable = enable;
        // Hacemos que detecte mo raycasts en base a enable
        group.blocksRaycasts = enable;
    }   
}
