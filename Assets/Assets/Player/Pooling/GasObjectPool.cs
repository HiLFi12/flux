using UnityEngine;

/// <summary>
/// Pool de objetos Gaseosos. Usa un prefab específico (por ejemplo, nubes o partículas gas).
/// </summary>
public class GasObjectPool : ObjectPool
{
    [Header("Etiqueta/Estado opcional")]
    public string requiredTag = "Gas"; // para validación opcional

    public override string ToString()
    {
        return $"GasObjectPool(prefab={prefab?.name}, expandable={expandable})";
    }
}
