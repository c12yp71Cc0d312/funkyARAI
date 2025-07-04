using UnityEngine;               // Core Unity engine namespace
using Firebase;                 // Firebase core namespace
using Firebase.Auth;           // Firebase Authentication namespace

public class FirebaseInit : MonoBehaviour
{
    // Static variables to make FirebaseAuth and FirebaseUser accessible globally
    public static FirebaseAuth auth;
    public static FirebaseUser user;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Check and fix Firebase dependencies asynchronously
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            // Get the result of the dependency check
            var dependencyStatus = task.Result;

            // If all required dependencies are available
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Initialize Firebase Authentication
                auth = FirebaseAuth.DefaultInstance;

                // Log success
                Debug.Log("Firebase initialized successfully.");
            }
            else
            {
                // If there was a problem with dependencies, log the error
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }
}
