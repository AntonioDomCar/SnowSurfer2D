using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp", menuName = "PowerUps/PowerUpData")]
public class PowerUpScriptableObjectScript : ScriptableObject
{
    [SerializeField] private string powerUpType; // Type of the power-up
    [SerializeField] private float powerUpValue; // Value of the power-up
    [SerializeField] private float timeLimit; // Duration of the power-up effect    

    public string PowerUpType { get => powerUpType; set => powerUpType = value; }
    public float PowerUpValue { get => powerUpValue; set => powerUpValue = value; }
    public float TimeLimit { get => timeLimit; set => timeLimit = value; }
    
}
