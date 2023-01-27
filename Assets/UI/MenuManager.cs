using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    [SerializeField]UIDocument uiDocument;



    private void OnEnable()
    {
        // Get the root of the visual tree
        VisualElement root = uiDocument.rootVisualElement;

        // Get the button from the visual tree
        Button button = root.Q<Button>("SinglePlayer");

        // Register a callback on the button
        button.RegisterCallback<ClickEvent>(OnSinglePlayer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnSinglePlayer(ClickEvent evt)
    {
        Debug.Log("Play button clicked");
        StartCoroutine(LoadScene());
    }


    IEnumerator LoadScene()
    {
        yield return null;

        //Begin to load the Scene you specify
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Scenes/Main");
        //Don't let the Scene activate until you allow it to
        asyncOperation.allowSceneActivation = false;
        Debug.Log("Pro :" + asyncOperation.progress);
        //When the load is still in progress, output the Text and progress bar
        while (!asyncOperation.isDone)
        {
            //Output the current progress
            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                    asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
