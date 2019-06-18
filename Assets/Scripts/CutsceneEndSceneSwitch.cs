using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutsceneEndSceneSwitch : MonoBehaviour
{
	private PlayableDirector _director;
	[SerializeField]
	int _scene;

	void Start()
    {
		_director = FindObjectOfType<PlayableDirector>();
	}

    void Update()
    {
		if (_director.state == PlayState.Paused)
		{
			SceneManager.LoadScene(_scene);
		}
    }
}
