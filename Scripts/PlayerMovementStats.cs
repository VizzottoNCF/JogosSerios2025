using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Movement")]
public class PlayerMovementStats : ScriptableObject
{

    [Header("Debug")]
    public bool DebugShowIsGroundedBox = false;
    public bool DebugShowHeadBumpBox = false;

    [Header("Walk")]
    [Range(1f, 100f)] public float maxWalkSpeed = 12.5f;
    [Range(0.25f, 50f)] public float groundAcceleration = 5f;
    [Range(0.25f, 50f)] public float groundDeceleration = 20f;
    [Range(0.25f, 50f)] public float airAcceleration = 5f;
    [Range(0.25f, 50f)] public float airDeceleration = 5f;

    [Header("Run")]
    [Range(1f, 100f)] public float maxRunSpeed = 20f;

    [Header("Grounded / Collision Checks")]
    public LayerMask GroundLayer;
    public float GroundDetectionRayLength = 0.02f;
    public float HeadDetectionRayLength = 0.02f;
    [Range(0f, 1f)] public float HeadWidth = 0.75f;

    [Header("Movement Assist")]
    public float EdgeCorrectionDistance = 0.1f;
    public float LedgeSnapHorizontalRange = 0.5f; // Horizontal range to detect ledges
    public float LedgeSnapVerticalThreshold = 0.2f; // Maximum vertical distance to snap to a ledge
    public float LedgeSnapVerticalOffset = 0.1f; // Small offset to ensure the player stands on the platform

    [Header("Jump")]
    public float JumpHeight = 6.5f;
    [Range(1f, 1.1f)] public float JumpHeightCompensationFactor = 1.054f;
    public float TimeTillJumpApex = 0.35f;
    [Range(0.01f, 5f)] public float GravityOnReleaseMultiplier = 2f;
    public float MaxFallSpeed = 26f;
    [Range(1, 5)] public int NumberOfJumpsAllowed = 2;

    [Header("Jump Cut")]
    [Range(0.02f, 0.3f)] public float TimeForUpwardsCancel = 0.027f;

    [Header("Jump Apex")]
    [Range(0.5f, 1f)] public float ApexThreshold = 0.97f;
    [Range(0.01f, 1f)] public float ApexHangTime = 0.75f;

    [Header("Jump Buffer")]
    [Range(0f, 1f)] public float JumpBufferTime = 0.125f;

    [Header("Jump Coyote Time")]
    [Range(0f, 1f)] public float JumpCoyoteTime = 0.1f;

    [Header("Jump Visualisation Tool")]
    public bool ShowWalkJumpArc = false;
    public bool ShowRunJumpArc = false;
    public bool StopOnCollision = true;
    public bool DrawRight = true;
    [Range(5, 100)] public int ArcResolution = 20;
    [Range(0, 500)] public int VisualizationSteps = 90;


    #region Physics magic
    public float Gravity { get; private set; }

    public float InitialJumpVelocity { get; private set; }
    public float AdjustmentJumpHeight { get; private set; }

    private void OnValidate()
    {
        rf_CalculateValues();
    }
    private void OnEnable()
    {
        rf_CalculateValues();
    }

    // refer to "Math for Game Programmers: Building a Better Jump"
    private void rf_CalculateValues()
    {
        AdjustmentJumpHeight = JumpHeight * JumpHeightCompensationFactor;
        Gravity = -(2f * JumpHeight) / MathF.Pow(TimeTillJumpApex, 2f);
        InitialJumpVelocity = MathF.Abs(Gravity) * TimeTillJumpApex;
    }
    #endregion
}
