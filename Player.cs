using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using _Common;
using TMPro;

public abstract class Player : MonoBehaviour
{
    #region References

    public PlayerInput PlayerInput;

    protected PhysicsMovement PhysicsMovement;

    private Transform _meshParent;

    #endregion

    #region Settings

    [SerializeField] protected bool isDummy;
    
    public int playerIndex;
    
    [SerializeField] private MovementAbilityToggle[] movementAbilitiesToggle = 
    {
        new MovementAbilityToggle("Movement", true),
        new MovementAbilityToggle("Jump", true),
        new MovementAbilityToggle("Dash", true),
        new MovementAbilityToggle("Hit", true)
    };

    #endregion

    public static Action<int> OnPlayerKilled;
    
    private PlayerInput GetPlayerInput()
    {
        if (isDummy) return null;
        
        print("looking for player input");
        return PlayerConfigurationManager.Instance.playerConfigurations[playerIndex].Input;
    }

    protected virtual void OnEnable()
    {
        PhysicsMovement = GetComponent<PhysicsMovement>();
        
        if(isDummy) return;
        PlayerInput = GetPlayerInput();
        SubscribeToPlayerInputs();
        SpawnMesh();
    }
    protected virtual void OnDisable()
    {
        if (!isDummy)
        {
            UnsubscribeToPlayerInputs();
            DeSpawnMesh();
        }
        
        PhysicsMovement = null;
    }
    
    private void SpawnMesh()
    {
        if(_meshParent != null) DeSpawnMesh();

        _meshParent = PlayerFunctions.GetChildObjectWithTag( transform, "MeshParent");
        var skin = PlayerFunctions.SpawnSkin(_meshParent, playerIndex);

        var playerName = PlayerConfigurationManager.Instance.playerConfigurations[playerIndex].ActivePlayer.PlayerName;
        var nameText = PlayerFunctions.GetChildObjectWithTag(skin.transform, "TextMesh").GetComponent<TextMeshProUGUI>();
        PlayerFunctions.UpdateSkinName(playerName, ref nameText);
        
        var nameFaceCamera = skin.GetComponentInChildren<NameFaceCamera>();
        nameFaceCamera.UpdateCamera(playerIndex);
        
        PhysicsMovement.SetMeshParent(_meshParent);
    }
    private void DeSpawnMesh()
    {
        if(_meshParent == null) return;
        
        Destroy(PlayerFunctions.GetChildObjectWithTag(_meshParent, "Mesh").gameObject);
        _meshParent = null;
    }

    public void SubscribeToPlayerInputs()
    {
        if (PlayerInput == null)
        {
            print("Found no player input when subscribing");
            return;
        }

        if (movementAbilitiesToggle[0].abilityEnabled)
        {
            PlayerInput.actions["Movement"].performed += PhysicsMovement.SetMovementInput;
            PlayerInput.actions["Movement"].canceled += PhysicsMovement.SetMovementInput;
        }

        if (movementAbilitiesToggle[1].abilityEnabled)
        {
            PlayerInput.actions["Jump"].started += PhysicsMovement.SetJumpInput;
            PlayerInput.actions["Jump"].canceled += PhysicsMovement.SetJumpInput;
        }

        if (movementAbilitiesToggle[2].abilityEnabled)
        {
            PlayerInput.actions["Action"].started += PhysicsMovement.DashInput;
            PlayerInput.actions["Action"].canceled += PhysicsMovement.DashInput;
        }

        if (movementAbilitiesToggle[3].abilityEnabled)
        {
            PlayerInput.actions["Hit"].started += PhysicsMovement.SetHitInput;
            PlayerInput.actions["Hit"].canceled += PhysicsMovement.SetHitInput;
        }
    }
    public void UnsubscribeToPlayerInputs()
    {
        if (PlayerInput == null)
        {
            print("Found no player input when unsubscribing");
            return;
        }
        
        if (movementAbilitiesToggle[0].abilityEnabled)
        {
            PlayerInput.actions["Movement"].performed -= PhysicsMovement.SetMovementInput;
            PlayerInput.actions["Movement"].canceled -= PhysicsMovement.SetMovementInput;
        }

        if (movementAbilitiesToggle[1].abilityEnabled)
        {
            PlayerInput.actions["Jump"].started -= PhysicsMovement.SetJumpInput;
            PlayerInput.actions["Jump"].canceled -= PhysicsMovement.SetJumpInput;
        }

        if (movementAbilitiesToggle[2].abilityEnabled)
        {
            PlayerInput.actions["Action"].started -= PhysicsMovement.DashInput;
            PlayerInput.actions["Action"].canceled -= PhysicsMovement.DashInput;
        }

        if (movementAbilitiesToggle[3].abilityEnabled)
        {
            PlayerInput.actions["Hit"].started -= PhysicsMovement.SetHitInput;
            PlayerInput.actions["Hit"].canceled -= PhysicsMovement.SetHitInput;
        }
    }

    public void ToggleInput(bool canMove)
    {
        PhysicsMovement.canMove = canMove;
    }

    public IEnumerator UpdatePlayerController(int targetPlayerIndex)
    {
        playerIndex = targetPlayerIndex;
        if(isActiveAndEnabled) gameObject.SetActive(false);
        yield return null;
        gameObject.SetActive(true);
    }

    public void StopPlayer()
    {
        PhysicsMovement.rb.velocity = Vector3.zero;
        PhysicsMovement.ResetInputVariables();
    }
}

[Serializable]
public class MovementAbilityToggle
{
    [SerializeField] private string abilityName;
    public bool abilityEnabled;

    public MovementAbilityToggle(string abilityName, bool abilityEnabled)
    {
        this.abilityName = abilityName;
        this.abilityEnabled = abilityEnabled;
    }
}
