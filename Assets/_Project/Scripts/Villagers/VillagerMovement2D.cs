// VillagerMovement2D.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerMovement2D : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float arrivalDistance = 0.1f;
    
    private Pathfinding2D pathfinding;
    private List<Vector3> currentPath = new List<Vector3>();
    private int currentPathIndex = 0;
    private bool isMoving = false;
    
    // References
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        pathfinding = FindObjectOfType<Pathfinding2D>();
    }
    
    private void Update()
    {
        if (isMoving)
        {
            FollowPath();
        }
    }
    
    // Start moving to target position
    public void MoveTo(Vector3 targetPosition)
    {
        StopAllCoroutines();
        currentPath = pathfinding.FindPath(transform.position, targetPosition);
        
        if (currentPath.Count > 0)
        {
            currentPathIndex = 0;
            isMoving = true;
            
            // Set animation state
            if (animator != null)
            {
                animator.SetBool("IsWalking", true);
            }
        }
    }
    
    // Stop moving
    public void StopMoving()
    {
        isMoving = false;
        currentPath.Clear();
        
        // Set animation state
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
        }
    }
    
    // Follow the current path
    private void FollowPath()
    {
        if (currentPathIndex >= currentPath.Count)
        {
            // Reached end of path
            StopMoving();
            return;
        }
        
        // Get current waypoint
        Vector3 targetPosition = currentPath[currentPathIndex];
        
        // Calculate direction and move
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        
        // Handle sprite facing direction (flip X based on movement direction)
        if (direction.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (direction.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        
        // Check if reached waypoint
        float distanceToWaypoint = Vector3.Distance(transform.position, targetPosition);
        if (distanceToWaypoint < arrivalDistance)
        {
            // Move to next waypoint
            currentPathIndex++;
        }
    }
    
    // Check if currently moving
    public bool IsMoving()
    {
        return isMoving;
    }
}
