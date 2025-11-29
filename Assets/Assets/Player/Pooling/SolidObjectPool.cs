using UnityEngine;

/// <summary>
/// Pool de objetos Sólidos. Usa un prefab específico (por ejemplo, bloques sólidos).
/// </summary>
public class SolidObjectPool : ObjectPool
{
    [Header("Etiqueta/Estado opcional")]
    public string requiredTag = "Solid"; // para validación opcional

    public override string ToString()
    {
        return $"SolidObjectPool(prefab={prefab?.name}, available={GetAvailableCount()})";
    }

    public int GetAvailableCount()
    {
        // No tenemos acceso directo a la cola; podría extender ObjectPool si necesario.
        // Este método es ilustrativo.
        return 0;
    }
}
